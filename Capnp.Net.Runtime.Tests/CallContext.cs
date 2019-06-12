using System.Threading;
using System.Threading.Tasks;
using Capnp.Rpc;

namespace Capnp.Net.Runtime.Tests
{
    class CallContext
    {
        public CallContext(ulong interfaceId, ushort methodId, DeserializerState args, CancellationToken ct)
        {
            InterfaceId = interfaceId;
            MethodId = methodId;
            Args = args;
            Ct = ct;
            Result = new TaskCompletionSource<AnswerOrCounterquestion>();
        }

        public ulong InterfaceId { get; }
        public ushort MethodId { get; }
        public DeserializerState Args { get; }
        public CancellationToken Ct { get; }
        public TaskCompletionSource<AnswerOrCounterquestion> Result { get; }
    }
}
