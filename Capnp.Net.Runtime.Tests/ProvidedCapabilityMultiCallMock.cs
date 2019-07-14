using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Capnp.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capnp.Net.Runtime.Tests
{
    class ProvidedCapabilityMultiCallMock : Skeleton
    {
        readonly BufferBlock<CallContext> _ccs = new BufferBlock<CallContext>();

        public override Task<AnswerOrCounterquestion> Invoke(ulong interfaceId, ushort methodId,
            DeserializerState args, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cc = new CallContext(interfaceId, methodId, args, cancellationToken);
            Assert.IsTrue(_ccs.Post(cc));
            return cc.Result.Task;
        }

        public Task<CallContext> WhenCalled => _ccs.ReceiveAsync();
    }
}
