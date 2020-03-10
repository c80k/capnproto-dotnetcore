using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class LazyCapability : RefCountingCapability, IResolvingCapability
    {
        public static LazyCapability CreateBrokenCap(string message)
        {
            var cap = new LazyCapability(Task.FromException<ConsumedCapability?>(new RpcException(message)));
            cap.AddRef(); // Instance shall be persistent
            return cap;
        }

        public static LazyCapability CreateCanceledCap(CancellationToken token)
        {
            var cap = new LazyCapability(Task.FromCanceled<ConsumedCapability?>(token));
            cap.AddRef(); // Instance shall be persistent
            return cap;
        }

        public static LazyCapability Null { get; } = CreateBrokenCap("Null capability");

        readonly Task<Proxy>? _proxyTask;

        public LazyCapability(Task<ConsumedCapability?> capabilityTask)
        {
            WhenResolved = capabilityTask;
        }

        public LazyCapability(Task<Proxy> proxyTask)
        {
            _proxyTask = proxyTask;

            async Task<ConsumedCapability?> AwaitCap() => (await _proxyTask!).ConsumedCap;

            WhenResolved = AwaitCap();
        }

        internal override void Freeze(out IRpcEndpoint? boundEndpoint)
        {
            if (WhenResolved.IsCompleted)
            {
                boundEndpoint = null;

                try
                {
                    WhenResolved.Result?.Freeze(out boundEndpoint);
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

        internal override void Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            if (WhenResolved.ReplacementTaskIsCompletedSuccessfully())
            {
                using var proxy = new Proxy(WhenResolved.Result);
                proxy.Export(endpoint, writer);
            }
            else
            {
                this.ExportAsSenderPromise(endpoint, writer);
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

        public Task<ConsumedCapability?> WhenResolved { get; }

        async Task<DeserializerState> CallImpl(ulong interfaceId, ushort methodId, DynamicSerializerState args, CancellationToken cancellationToken)
        {
            var cap = await WhenResolved;

            cancellationToken.ThrowIfCancellationRequested();

            if (cap == null)
                throw new RpcException("Broken capability");

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