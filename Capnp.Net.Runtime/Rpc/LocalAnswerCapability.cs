using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Capnp.Rpc
{
    class LocalAnswerCapability : RefCountingCapability, IResolvingCapability
    {
        readonly Task<DeserializerState> _answer;
        readonly MemberAccessPath _access;

        public LocalAnswerCapability(Task<DeserializerState> answer, MemberAccessPath access)
        {
            _answer = answer;
            _access = access;
        }

        internal override void Freeze(out IRpcEndpoint? boundEndpoint)
        {
            boundEndpoint = null;
        }

        internal override void Unfreeze()
        {
        }

        async Task<Proxy> AwaitResolved()
        {
            var state = await _answer;
            return new Proxy(_access.Eval(state));
        }

        public Task<Proxy> WhenResolved => AwaitResolved();

        internal override void Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            if (_answer.IsCompleted)
            {
                DeserializerState result;
                try
                {
                    result = _answer.Result;
                }
                catch (AggregateException exception)
                {
                    throw exception.InnerException!;
                }

                using (var proxy = new Proxy(_access.Eval(result)))
                {
                    proxy.Export(endpoint, writer);
                }
            }
            else
            {
                this.ExportAsSenderPromise(endpoint, writer);
            }
        }

        async Task<DeserializerState> CallImpl(ulong interfaceId, ushort methodId, DynamicSerializerState args, CancellationToken cancellationToken)
        {
            var cap = await AwaitResolved();

            cancellationToken.ThrowIfCancellationRequested();

            if (cap == null)
                throw new RpcException("Broken capability");

            var call = cap.Call(interfaceId, methodId, args, default);
            var whenReturned = call.WhenReturned;

            using (var registration = cancellationToken.Register(() => call.Dispose()))
            {
                return await whenReturned;
            }
        }

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            var cts = new CancellationTokenSource();
            return new LocalAnswer(cts, CallImpl(interfaceId, methodId, args, cts.Token));
        }

        protected override void ReleaseRemotely()
        {
            this.DisposeWhenResolved();
        }
    }
}
#nullable restore