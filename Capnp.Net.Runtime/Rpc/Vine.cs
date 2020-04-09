using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class Vine : Skeleton
    {
        public static Skeleton Create(ConsumedCapability? cap)
        {
            if (cap is LocalCapability lcap)
                return lcap.ProvidedCap;
            else
                return new Vine(cap);
        }

        Vine(ConsumedCapability? consumedCap)
        {
            Proxy = new Proxy(consumedCap);

#if DebugFinalizers
            CreatorStackTrace = Environment.StackTrace;
#endif
        }

#if DebugFinalizers
        ~Vine()
        {
            Logger.LogWarning($"Caught orphaned Vine, created from here: {CreatorStackTrace}.");

            Dispose(false);
        }

        ILogger Logger { get; } = Logging.CreateLogger<Vine>();
        string CreatorStackTrace { get; }
#endif

        internal override void Bind(object impl)
        {
            throw new NotImplementedException();
        }

        public Proxy Proxy { get; }

        public async override Task<AnswerOrCounterquestion> Invoke(
            ulong interfaceId, ushort methodId, DeserializerState args, 
            CancellationToken cancellationToken = default)
        {
            var promisedAnswer = Proxy.Call(interfaceId, methodId, (DynamicSerializerState)args, default);

            if (promisedAnswer is PendingQuestion pendingQuestion && pendingQuestion.RpcEndpoint == Impatient.AskingEndpoint)
            {
                async void SetupCancellation()
                {
                    try
                    {
                        using var registration = cancellationToken.Register(promisedAnswer.Dispose);
                        await promisedAnswer.WhenReturned;
                    }
                    catch
                    {
                    }
                }

                SetupCancellation();

                return pendingQuestion;
            }
            else
            {
                using var registration = cancellationToken.Register(promisedAnswer.Dispose);
                return (DynamicSerializerState)await promisedAnswer.WhenReturned;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Proxy.Dispose();
            else
                try { Proxy.Dispose(); }
                catch { }

            base.Dispose(disposing);
        }
    }
}