using System;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    internal interface IRpcEndpoint
    {
        PendingQuestion BeginQuestion(ConsumedCapability target, SerializerState inParams);
        void SendQuestion(SerializerState inParams, Payload.WRITER payload);
        uint AllocateExport(Skeleton providedCapability, out bool first);
        void Finish(uint questionId);
        void ReleaseImport(uint importId);
        void Resolve(uint preliminaryId, Skeleton preliminaryCap, Func<ConsumedCapability> resolvedCapGetter);

        Task RequestSenderLoopback(Action<MessageTarget.WRITER> writer);
        void DeleteQuestion(PendingQuestion question);
    }
}