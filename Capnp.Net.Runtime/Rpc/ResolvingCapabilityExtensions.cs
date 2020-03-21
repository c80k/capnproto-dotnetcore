using System;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    static class ResolvingCapabilityExtensions
    {
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
                        var resolvedCap = await cap.WhenResolved;
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

        public static async Task<Proxy> AsProxyTask(this Task<IDisposable?> task)
        {
            var obj = await task;
            switch (obj)
            {
                case Proxy proxy: return proxy;
                case null: return new Proxy(LazyCapability.Null);
                default: return BareProxy.FromImpl(obj);
            }
        }
    }
}