using Capnp.Util;
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

        public abstract Task WhenResolved { get; }
        public abstract T? GetResolvedCapability<T>() where T : class;

        protected RemoteResolvingCapability(IRpcEndpoint ep) : base(ep)
        {
        }

        protected int _pendingCallsOnPromise;
        StrictlyOrderedAwaitTask? _disembargo;

        protected abstract ConsumedCapability? ResolvedCap { get; }

        protected abstract void GetMessageTarget(MessageTarget.WRITER wr);

        protected IPromisedAnswer CallOnResolution(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            var resolvedCap = ResolvedCap!;

            try
            {
                if (resolvedCap is NullCapability ||
                    // Must not request disembargo on null cap

                    resolvedCap is RemoteCapability ||
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
                    using var proxy = new Proxy(resolvedCap);
                    return proxy.Call(interfaceId, methodId, args, default);
                }
                else
                {
                    if (_disembargo == null)
                    {
#if DebugEmbargos
                        Logger.LogDebug("Requesting disembargo");
#endif
                        _disembargo = _ep.RequestSenderLoopback(GetMessageTarget).EnforceAwaitOrder();
                    }
                    else
                    {
#if DebugEmbargos
                        Logger.LogDebug("Waiting for requested disembargo");
#endif
                    }

                    var cancellationTokenSource = new CancellationTokenSource();

                    async Task<DeserializerState> AwaitAnswer()
                    {
                        await _disembargo!;

                        // Two reasons for ignoring exceptions on the previous task (i.e. not _.Wait()ing):
                        // 1. A faulting predecessor, especially due to cancellation, must not have any impact on this one.
                        // 2. A faulting disembargo request would imply that the other side cannot send pending requests anyway.

                        if (cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            args.Dispose();
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }

                        using var proxy = new Proxy(resolvedCap);
                        var promisedAnswer = proxy.Call(interfaceId, methodId, args, default);

                        using (cancellationTokenSource.Token.Register(promisedAnswer.Dispose))
                        {
                            return await promisedAnswer.WhenReturned;
                        }
                    }

                    return new LocalAnswer(cancellationTokenSource, AwaitAnswer());
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