using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class LocalAnswerCapabilityDeprecated : RefCountingCapability, IResolvingCapability
    {
        readonly Task<DeserializerState> _answer;
        readonly MemberAccessPath _access;

        public LocalAnswerCapabilityDeprecated(Task<DeserializerState> answer, MemberAccessPath access)
        {
            _answer = answer;
            _access = access;

            async Task<ConsumedCapability?> AwaitResolved() => access.Eval(await _answer);
            WhenResolved = AwaitResolved();
        }

        internal override void Freeze(out IRpcEndpoint? boundEndpoint)
        {
            boundEndpoint = null;
        }

        internal override void Unfreeze()
        {
        }


        public Task<ConsumedCapability?> WhenResolved { get; private set; }

        internal override Action? Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
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

                using var proxy = new Proxy(_access.Eval(result));
                proxy.Export(endpoint, writer);
                return null;
            }
            else
            {
                return this.ExportAsSenderPromise(endpoint, writer);
            }
        }

        async Task<DeserializerState> CallImpl(ulong interfaceId, ushort methodId, DynamicSerializerState args, CancellationToken cancellationToken)
        {
            var cap = await WhenResolved;

            cancellationToken.ThrowIfCancellationRequested();

            if (cap == null)
                throw new RpcException("Broken capability");

            using var proxy = new Proxy(cap);
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

        protected override void ReleaseRemotely()
        {
        }
    }
}