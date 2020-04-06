using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{

    class LocalAnswerCapability : RefCountingCapability, IResolvingCapability
    {
        static async Task<Proxy> TransferOwnershipToDummyProxy(Task<DeserializerState> answer, MemberAccessPath access)
        {
            var result = await answer;
            var cap = access.Eval(result);
            var proxy = new Proxy(cap);
            cap?.Release();
            return proxy;
        }

        readonly Task<Proxy> _whenResolvedProxy;

        public LocalAnswerCapability(Task<Proxy> proxyTask)
        {
            _whenResolvedProxy = proxyTask;
        }

        public LocalAnswerCapability(Task<DeserializerState> answer, MemberAccessPath access):
            this(TransferOwnershipToDummyProxy(answer, access))
        {

        }

        public Task WhenResolved => _whenResolvedProxy;

        public T? GetResolvedCapability<T>() where T : class => _whenResolvedProxy.GetResolvedCapability<T>();

        internal override Action? Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            if (_whenResolvedProxy.IsCompleted)
            {
                try
                {
                    _whenResolvedProxy.Result.Export(endpoint, writer);
                }
                catch (AggregateException exception)
                {
                    throw exception.InnerException!;
                }

                return null;
            }
            else
            {
                return this.ExportAsSenderPromise(endpoint, writer);
            }
        }

        async Task<DeserializerState> CallImpl(ulong interfaceId, ushort methodId, DynamicSerializerState args, CancellationToken cancellationToken)
        {
            var proxy = await _whenResolvedProxy;

            cancellationToken.ThrowIfCancellationRequested();

            if (proxy.IsNull)
            {
                args.Dispose();
                throw new RpcException("Broken capability");
            }

            var call = proxy.Call(interfaceId, methodId, args, default);
            var whenReturned = call.WhenReturned;

            using var registration = cancellationToken.Register(() => call.Dispose());
            return await whenReturned;
        }

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            var cts = new CancellationTokenSource();
            return new LocalAnswer(cts, CallImpl(interfaceId, methodId, args, cts.Token));
        }

        protected async override void ReleaseRemotely()
        {
            try { using var _ = await _whenResolvedProxy; }
            catch { }
        }
    }
}