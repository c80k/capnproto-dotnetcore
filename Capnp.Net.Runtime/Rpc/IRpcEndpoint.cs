using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Capnp.Rpc
{
    internal interface IRpcEndpoint
    {
        PendingQuestion BeginQuestion(ConsumedCapability target, SerializerState inParams);
        void SendQuestion(SerializerState inParams, Payload.WRITER payload);
        uint AllocateExport(Skeleton providedCapability, out bool first);
        void RequestPostAction(Action postAction);
        void Finish(uint questionId);
        void ReleaseImport(uint importId);
        void Resolve(uint preliminaryId, Skeleton preliminaryCap, Func<ConsumedCapability> resolvedCapGetter);

        Task RequestSenderLoopback(Action<MessageTarget.WRITER> writer);
        void DeleteQuestion(PendingQuestion question);
    }
}
#nullable restore