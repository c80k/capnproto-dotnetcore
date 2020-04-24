using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{

    class Vine : Skeleton
    {
        public Vine(ConsumedCapability consumedCap)
        {
            Cap = consumedCap;
        }

        public ConsumedCapability Cap { get; }

        internal override ConsumedCapability AsCapability() => Cap;

        internal override void Claim()
        {
            Cap.AddRef();
        }

        internal override void Relinquish()
        {
            Cap.Release();
        }

        public async override Task<AnswerOrCounterquestion> Invoke(
            ulong interfaceId, ushort methodId, DeserializerState args, 
            CancellationToken cancellationToken = default)
        {
            using var proxy = new Proxy(Cap);
            var promisedAnswer = proxy.Call(interfaceId, methodId, (DynamicSerializerState)args, false, cancellationToken);

            if (promisedAnswer is PendingQuestion pendingQuestion && pendingQuestion.RpcEndpoint == Impatient.AskingEndpoint)
            {
                return pendingQuestion;
            }
            else
            {
                using var registration = cancellationToken.Register(promisedAnswer.Dispose);
                return (DynamicSerializerState)await promisedAnswer.WhenReturned;
            }
        }
    }
}