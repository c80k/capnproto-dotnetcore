using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class LazyCapability : RefCountingCapability, IResolvingCapability
    {
        public static LazyCapability CreateBrokenCap(string message)
        {
            return new LazyCapability(Task.FromException<ConsumedCapability?>(new RpcException(message)));
        }

        public static LazyCapability CreateCanceledCap(CancellationToken token)
        {
            return new LazyCapability(Task.FromCanceled<ConsumedCapability?>(token));
        }

        readonly Task<Proxy>? _proxyTask;
        readonly Task<ConsumedCapability?> _capTask;

        public LazyCapability(Task<ConsumedCapability?> capabilityTask)
        {
            _capTask = capabilityTask;
        }

        public LazyCapability(Task<Proxy> proxyTask)
        {
            _proxyTask = proxyTask;

            async Task<ConsumedCapability?> AwaitCap() => (await _proxyTask!).ConsumedCap;

            _capTask = AwaitCap();
        }

        internal override void Freeze(out IRpcEndpoint? boundEndpoint)
        {
            if (WhenResolved.IsCompleted)
            {
                boundEndpoint = null;

                try
                {
                    _capTask.Result?.Freeze(out boundEndpoint);
                }
                catch (AggregateException exception)
                {
                    throw exception.InnerException!;
                }
            }
            else
            {
                boundEndpoint = null;
            }
        }

        internal override void Unfreeze()
        {
        }

        internal override Action? Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            if (WhenResolved.ReplacementTaskIsCompletedSuccessfully())
            {
                using var proxy = GetResolvedCapability<BareProxy>()!;
                return proxy.Export(endpoint, writer);
            }
            else
            {
                return this.ExportAsSenderPromise(endpoint, writer);
            }
        }

        protected override void ReleaseRemotely()
        {
            if (_proxyTask != null)
            {
                async void DisposeProxyWhenResolved()
                {
                    try { using var _ = await _proxyTask!; }
                    catch { }
                }

                DisposeProxyWhenResolved();
            }
        }

        public Task WhenResolved => _capTask;

        public T? GetResolvedCapability<T>() where T: class
        {
            if (_capTask.IsCompleted)
            {
                try
                {
                    return CapabilityReflection.CreateProxy<T>(_capTask.Result) as T;
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

        async Task<DeserializerState> CallImpl(ulong interfaceId, ushort methodId, DynamicSerializerState args, CancellationToken cancellationToken)
        {
            ConsumedCapability? cap;
            try
            {
                cap = await _capTask;
            }
            catch
            {
                args.Dispose();
                throw;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                args.Dispose();
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (cap == null)
            {
                args.Dispose();
                throw new RpcException("Broken capability");
            }

            using var proxy = new Proxy(cap);
            var call = proxy.Call(interfaceId, methodId, args, default);
            var whenReturned = call.WhenReturned;

            using (var registration = cancellationToken.Register(call.Dispose))
            {
                return await whenReturned;
            }
        }

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            var cts = new CancellationTokenSource();
            return new LocalAnswer(cts, CallImpl(interfaceId, methodId, args, cts.Token));
        }
    }
}