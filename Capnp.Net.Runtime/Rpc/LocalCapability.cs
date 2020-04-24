using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class LocalCapability : ConsumedCapability
    {
        static async Task<DeserializerState> AwaitAnswer(Task<AnswerOrCounterquestion> call)
        {
            var aorcq = await call;
            return aorcq.Answer ?? await aorcq.Counterquestion!.WhenReturned;
        }

        public Skeleton ProvidedCap { get; }

        internal override Skeleton AsSkeleton() => ProvidedCap;

        public LocalCapability(Skeleton providedCap)
        {
            ProvidedCap = providedCap ?? throw new ArgumentNullException(nameof(providedCap));
        }

        internal override void AddRef()
        {
            ProvidedCap.Claim();
        }

        internal override void Release()
        {
            ProvidedCap.Relinquish();
        }

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            var cts = new CancellationTokenSource();
            var call = ProvidedCap.Invoke(interfaceId, methodId, args, cts.Token);
            return new LocalAnswer(cts, AwaitAnswer(call));
        }

        internal override Action? Export(IRpcEndpoint endpoint, CapDescriptor.WRITER capDesc)
        {
            capDesc.which = CapDescriptor.WHICH.SenderHosted;
            capDesc.SenderHosted = endpoint.AllocateExport(ProvidedCap, out bool _);
            return null;
        }

        protected override void ReleaseRemotely()
        {
        }
    }
}