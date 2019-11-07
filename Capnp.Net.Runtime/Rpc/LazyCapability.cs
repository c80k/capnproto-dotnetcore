using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class LazyCapability : RefCountingCapability, IResolvingCapability
    {
        public static LazyCapability CreateBrokenCap(string message)
        {
            var cap = new LazyCapability(Task.FromException<Proxy>(new RpcException(message)));
            cap.AddRef(); // Instance shall be persistent
            return cap;
        }

        public static LazyCapability CreateCanceledCap(CancellationToken token)
        {
            var cap = new LazyCapability(Task.FromCanceled<Proxy>(token));
            cap.AddRef(); // Instance shall be persistent
            return cap;
        }

        public static LazyCapability Null { get; } = CreateBrokenCap("Null capability");

        public LazyCapability(Task<Proxy> capabilityTask)
        {
            WhenResolved = capabilityTask;
        }

        internal override void Freeze(out IRpcEndpoint boundEndpoint)
        {
            if (WhenResolved.IsCompleted)
            {
                try
                {
                    WhenResolved.Result.Freeze(out boundEndpoint);
                }
                catch (AggregateException exception)
                {
                    throw exception.InnerException;
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
                WhenResolved.Result.Export(endpoint, writer);
            }
            else
            {
                this.ExportAsSenderPromise(endpoint, writer);
            }
        }

        async void DisposeProxyWhenResolved()
        {
            try
            {
                var cap = await WhenResolved;
                if (cap != null) cap.Dispose();
            }
            catch
            {
            }
        }

        protected override void ReleaseRemotely()
        {
            DisposeProxyWhenResolved();
        }

        public Task<Proxy> WhenResolved { get; }

        async Task<DeserializerState> CallImpl(ulong interfaceId, ushort methodId, DynamicSerializerState args, CancellationToken cancellationToken)
        {
            var cap = await WhenResolved;

            cancellationToken.ThrowIfCancellationRequested();

            if (cap == null)
                throw new RpcException("Broken capability");

            var call = cap.Call(interfaceId, methodId, args, default);
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
