using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    abstract class RemoteResolvingCapability : RemoteCapability, IResolvingCapability
    {
        // Set DebugEmbargos to true to get logging output for calls. RPC calls are expected to
        // be on the critical path, hence very relevant for performance. We just can't afford
        // additional stuff on this path. Even if the logger filters the outputs away, there is
        // overhead for creating the Logger object, calling the Logger methods and deciding to
        // filter the output. This justifies the precompiler switch.
#if DebugEmbargos
        ILogger Logger { get; } = Logging.CreateLogger<RemoteResolvingCapability>();
#endif

        public abstract Task<Proxy> WhenResolved { get; }

        protected RemoteResolvingCapability(IRpcEndpoint ep) : base(ep)
        {
        }

        protected int _pendingCallsOnPromise;
        Task? _disembargo;

        protected abstract Proxy? ResolvedCap { get; }

        protected abstract void GetMessageTarget(MessageTarget.WRITER wr);

        protected IPromisedAnswer CallOnResolution(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            if (ResolvedCap == null)
                throw new InvalidOperationException("Capability not yet resolved, calling on resolution not possible");

            try
            {
                ResolvedCap.Freeze(out var resolvedCapEndpoint);

                try
                {
                    if (resolvedCapEndpoint != null && resolvedCapEndpoint != _ep)
                    {
                        // Carol lives in a different Vat C.
                        throw new NotImplementedException("Sorry, level 3 RPC is not yet supported.");
                    }

                    if (ResolvedCap.IsNull ||
                        // If the capability resolves to null, disembargo must not be requested.
                        // Take the direct path, well-knowing that the call will result in an exception.

                        resolvedCapEndpoint != null ||
                        //# Note that in the case where Carol actually lives in Vat B (i.e., the same vat that the promise
                        //# already pointed at), no embargo is needed, because the pipelined calls are delivered over the
                        //# same path as the later direct calls.

                        (_disembargo == null && _pendingCallsOnPromise == 0) ||
                        // No embargo is needed since all outstanding replies have returned

                        _disembargo?.IsCompleted == true
                        // Disembargo has returned
                        )
                    {
#if DebugEmbargos
                Logger.LogDebug("Direct call");
#endif
                        return ResolvedCap.Call(interfaceId, methodId, args, default);
                    }
                    else
                    {
                        if (_disembargo == null)
                        {
#if DebugEmbargos
                    Logger.LogDebug("Requesting disembargo");
#endif
                            _disembargo = _ep.RequestSenderLoopback(GetMessageTarget);
                        }
                        else
                        {
#if DebugEmbargos
                    Logger.LogDebug("Waiting for requested disembargo");
#endif
                        }

                        var cancellationTokenSource = new CancellationTokenSource();

                        var callAfterDisembargo = _disembargo.ContinueWith(_ =>
                        {
                            // Two reasons for ignoring exceptions on the previous task (i.e. not _.Wait()ing):
                            // 1. A faulting predecessor, especially due to cancellation, must not have any impact on this one.
                            // 2. A faulting disembargo request would be a fatal protocol error, resulting in Abort() - we're dead anyway.

                            cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            return ResolvedCap.Call(interfaceId, methodId, args, default);

                        }, TaskContinuationOptions.ExecuteSynchronously);

                        _disembargo = callAfterDisembargo;

                        async Task<DeserializerState> AwaitAnswer()
                        {
                            var promisedAnswer = await callAfterDisembargo;

                            using (cancellationTokenSource.Token.Register(promisedAnswer.Dispose))
                            {
                                return await promisedAnswer.WhenReturned;
                            }
                        }

                        return new LocalAnswer(cancellationTokenSource, AwaitAnswer());
                    }
                }
                finally
                {
                    ResolvedCap.Unfreeze();
                }
            }
            catch (System.Exception exception)
            {
                // Wrap exception into local answer, since otherwise we'd get an AggregateException (which we don't want).
                return new LocalAnswer(
                    new CancellationTokenSource(),
                    Task.FromException<DeserializerState>(exception));
            }
        }
    }
}