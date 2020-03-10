using System;
using System.Threading.Tasks;

namespace Capnp.Rpc
{

    class RemoteAnswerCapabilityDeprecated : RemoteResolvingCapability
    {
        // Set DebugEmbargos to true to get logging output for calls. RPC calls are expected to
        // be on the critical path, hence very relevant for performance. We just can't afford
        // additional stuff on this path. Even if the logger filters the outputs away, there is
        // overhead for creating the Logger object, calling the Logger methods and deciding to
        // filter the output. This justifies the precompiler switch.
#if DebugEmbargos
        ILogger Logger { get; } = Logging.CreateLogger<RemoteAnswerCapability>();
#endif

        readonly PendingQuestion _question;
        readonly MemberAccessPath _access;
        readonly Task<Proxy> _whenResolvedProxy;
        ConsumedCapability? _resolvedCap;

        public RemoteAnswerCapabilityDeprecated(PendingQuestion question, MemberAccessPath access): base(question.RpcEndpoint)
        {
            _question = question ?? throw new ArgumentNullException(nameof(question));
            _access = access ?? throw new ArgumentNullException(nameof(access));

            async Task<ConsumedCapability?> AwaitWhenResolved()
            {
                await _question.WhenReturned;

                if (_question.IsTailCall)
                    throw new InvalidOperationException("Question is a tail call, so won't resolve back.");

                return ResolvedCap!;
            }

            WhenResolved = AwaitWhenResolved();

            async Task<Proxy> AwaitProxy() => new Proxy(await WhenResolved);
            _whenResolvedProxy = AwaitProxy();
        }

        async void ReAllowFinishWhenDone(Task task)
        {
            try
            {
                ++_pendingCallsOnPromise;

                await task;
            }
            catch
            {
            }
            finally
            {
                lock (_question.ReentrancyBlocker)
                {
                    --_pendingCallsOnPromise;
                    _question.AllowFinish();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            lock (_question.ReentrancyBlocker)
            {
                using var _ = new Proxy(_resolvedCap);
            }
        }

        protected override ConsumedCapability? ResolvedCap
        {
            get
            {
                lock (_question.ReentrancyBlocker)
                {
                    if (_resolvedCap == null && !_question.IsTailCall && _question.IsReturned)
                    {
                        DeserializerState result;
                        try
                        {
                            result = _question.WhenReturned.Result;
                        }
                        catch (AggregateException exception)
                        {
                            throw exception.InnerException!;
                        }

                        _resolvedCap = _access.Eval(result);
                    }
                    return _resolvedCap;
                }
            }
        }

        public override Task<ConsumedCapability?> WhenResolved { get; }

        protected override void GetMessageTarget(MessageTarget.WRITER wr)
        {
            wr.which = MessageTarget.WHICH.PromisedAnswer;
            wr.PromisedAnswer.QuestionId = _question.QuestionId;
            _access.Serialize(wr.PromisedAnswer);
        }

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            lock (_question.ReentrancyBlocker)
            {
                if (_question.StateFlags.HasFlag(PendingQuestion.State.Returned) &&
                    !_question.StateFlags.HasFlag(PendingQuestion.State.TailCall))
                {
                    if (ResolvedCap == null)
                    {
                        throw new RpcException("Answer did not resolve to expected capability");
                    }

                    return CallOnResolution(interfaceId, methodId, args);
                }
                else
                {
#if DebugEmbargos
                    Logger.LogDebug("Call by proxy");
#endif
                    if (_question.StateFlags.HasFlag(PendingQuestion.State.Disposed))
                    {
                        throw new ObjectDisposedException(nameof(PendingQuestion));
                    }

                    if (_question.StateFlags.HasFlag(PendingQuestion.State.FinishRequested))
                    {
                        throw new InvalidOperationException("Finish request was already sent");
                    }

                    _question.DisallowFinish();
                    ++_pendingCallsOnPromise;
                    var promisedAnswer = base.DoCall(interfaceId, methodId, args);
                    ReAllowFinishWhenDone(promisedAnswer.WhenReturned);

                    async void DecrementPendingCallsOnPromiseWhenReturned()
                    {
                        try
                        {
                            await promisedAnswer.WhenReturned;
                        }
                        catch
                        {
                        }
                        finally
                        {
                            lock (_question.ReentrancyBlocker)
                            {
                                --_pendingCallsOnPromise;
                            }
                        }
                    }

                    DecrementPendingCallsOnPromiseWhenReturned();
                    return promisedAnswer;
                }
            }
        }

        protected override Call.WRITER SetupMessage(DynamicSerializerState args, ulong interfaceId, ushort methodId)
        {
            var call = base.SetupMessage(args, interfaceId, methodId);

            call.Target.which = MessageTarget.WHICH.PromisedAnswer;
            call.Target.PromisedAnswer.QuestionId = _question.QuestionId;
            _access.Serialize(call.Target.PromisedAnswer);

            return call;
        }

        internal override void Freeze(out IRpcEndpoint? boundEndpoint)
        {
            lock (_question.ReentrancyBlocker)
            {
                if (_question.StateFlags.HasFlag(PendingQuestion.State.Returned) &&
                    _pendingCallsOnPromise == 0)
                {
                    if (ResolvedCap == null)
                    {
                        throw new RpcException("Answer did not resolve to expected capability");
                    }

                    ResolvedCap.Freeze(out boundEndpoint);
                }
                else
                {
                    ++_pendingCallsOnPromise;
                    _question.DisallowFinish();
                    boundEndpoint = _ep;
                }
            }
        }

        internal override void Unfreeze()
        {
            lock (_question.ReentrancyBlocker)
            {
                if (_pendingCallsOnPromise > 0)
                {
                    --_pendingCallsOnPromise;
                    _question.AllowFinish();
                }
                else
                {
                    ResolvedCap?.Unfreeze();
                }
            }
        }

        internal override void Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            lock (_question.ReentrancyBlocker)
            {
                if (_question.StateFlags.HasFlag(PendingQuestion.State.Disposed))
                    throw new ObjectDisposedException(nameof(PendingQuestion));

                if (_question.StateFlags.HasFlag(PendingQuestion.State.Returned))
                {
                    ResolvedCap?.Export(endpoint, writer);
                }
                else
                {
                    if (_question.StateFlags.HasFlag(PendingQuestion.State.FinishRequested))
                        throw new InvalidOperationException("Finish request was already sent");

                    if (endpoint == _ep)
                    {
                        writer.which = CapDescriptor.WHICH.ReceiverAnswer;
                        _access.Serialize(writer.ReceiverAnswer);
                        writer.ReceiverAnswer.QuestionId = _question.QuestionId;
                    }
                    else if (_question.IsTailCall)
                    {
                        // FIXME: Resource management! We should prevent finishing this
                        // cap as long as it is exported. Unfortunately, we cannot determine
                        // when it gets removed from the export table.

                        var vine = Vine.Create(this);
                        uint id = endpoint.AllocateExport(vine, out bool first);

                        writer.which = CapDescriptor.WHICH.SenderHosted;
                        writer.SenderHosted = id;
                    }
                    else
                    {
                        this.ExportAsSenderPromise(endpoint, writer);
                    }
                }
            }
        }

        protected async override void ReleaseRemotely()
        {
            try { using var _ = await _whenResolvedProxy; }
            catch { }
        }
    }
}