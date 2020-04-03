using System;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    static class ResolvingCapabilityExtensions
    {
        public static async Task<ConsumedCapability?> Unwrap(this ConsumedCapability? cap)
        {
            while (cap is IResolvingCapability resolving)
            {
                await resolving.WhenResolved;
                using var proxy = resolving.GetResolvedCapability<BareProxy>()!;
                cap = proxy.ConsumedCap;
            }

            return cap;
        }

        public static Action? ExportAsSenderPromise<T>(this T cap, IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
            where T: ConsumedCapability, IResolvingCapability
        {
            var vine = Vine.Create(cap);
            uint preliminaryId = endpoint.AllocateExport(vine, out bool first);

            writer.which = CapDescriptor.WHICH.SenderPromise;
            writer.SenderPromise = preliminaryId;

            if (first)
            {
                return async () => {

                    try
                    {
                        await cap.WhenResolved;
                        using var proxy = cap.GetResolvedCapability<BareProxy>()!;
                        var resolvedCap = await Unwrap(proxy.ConsumedCap);
                        endpoint.Resolve(preliminaryId, vine, () => resolvedCap!);
                    }
                    catch (System.Exception exception)
                    {
                        endpoint.Resolve(preliminaryId, vine, () => throw exception);
                    }

                };
            }

            return null;
        }

        public static async Task<Proxy> AsProxyTask<T>(this Task<T> task) 
            where T: IDisposable?
        {
            IDisposable? obj;
            try
            {
                obj = await task;
            }
            catch (TaskCanceledException exception)
            {
                return new Proxy(LazyCapability.CreateCanceledCap(exception.CancellationToken));
            }
            catch (System.Exception exception)
            {
                return new Proxy(LazyCapability.CreateBrokenCap(exception.Message));
            }

            switch (obj)
            {
                case Proxy proxy: return proxy;
                case null: return new Proxy(null);
                default: return BareProxy.FromImpl(obj);
            }
        }

        public static T? GetResolvedCapability<T>(this Task<Proxy> proxyTask) where T: class
        {
            if (proxyTask.IsCompleted)
            {
                try
                {
                    return proxyTask.Result.Cast<T>(false);
                }
                catch (AggregateException exception)
                {
                    throw exception.InnerException!;
                }
            }
            else
            {
                return null;
            }
        }
    }
}