using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class LocalCapability : ConsumedCapability
    {
        static readonly ConditionalWeakTable<Skeleton, LocalCapability> _localCaps =
            new ConditionalWeakTable<Skeleton, LocalCapability>();

        public static ConsumedCapability Create(Skeleton skeleton)
        {
            if (skeleton is Vine vine)
                return vine.Proxy.ConsumedCap;
            else
                return _localCaps.GetValue(skeleton, _ => new LocalCapability(_));
        }

        static async Task<DeserializerState> AwaitAnswer(Task<AnswerOrCounterquestion> call)
        {
            var aorcq = await call;
            return aorcq.Answer ?? await aorcq.Counterquestion.WhenReturned;
        }

        public Skeleton ProvidedCap { get; }

        LocalCapability(Skeleton providedCap)
        {
            ProvidedCap = providedCap ?? throw new ArgumentNullException(nameof(providedCap));
        }

        internal override void AddRef()
        {
            ProvidedCap.Claim();
        }

        internal override void Release(
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            ProvidedCap.Relinquish();
        }

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            var cts = new CancellationTokenSource();
            var call = ProvidedCap.Invoke(interfaceId, methodId, args, cts.Token);
            return new LocalAnswer(cts, AwaitAnswer(call));
        }

        internal override void Export(IRpcEndpoint endpoint, CapDescriptor.WRITER capDesc)
        {
            capDesc.which = CapDescriptor.WHICH.SenderHosted;
            capDesc.SenderHosted = endpoint.AllocateExport(ProvidedCap, out bool _);
        }

        internal override void Freeze(out IRpcEndpoint boundEndpoint)
        {
            boundEndpoint = null;
        }

        internal override void Unfreeze()
        {
        }

        protected override void ReleaseRemotely()
        {
        }
    }
}
