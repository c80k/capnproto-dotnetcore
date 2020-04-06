using System;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class RemoteAnswerCapability : RemoteResolvingCapability
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

        public RemoteAnswerCapability(PendingQuestion question, MemberAccessPath access, Task<Proxy> proxyTask) : base(question.RpcEndpoint)
        {
            _question = question ?? throw new ArgumentNullException(nameof(question));
            _access = access ?? throw new ArgumentNullException(nameof(access));
            _whenResolvedProxy = proxyTask ?? throw new ArgumentNullException(nameof(proxyTask));
        }

        static async Task<Proxy> TransferOwnershipToDummyProxy(PendingQuestion question, MemberAccessPath access)
        {
            var result = await question.WhenReturned;
            var cap = access.Eval(result);
            var proxy = new Proxy(cap);
            cap?.Release();
            return proxy;
        }

        public RemoteAnswerCapability(PendingQuestion question, MemberAccessPath access) : this(question, access, TransferOwnershipToDummyProxy(question, access))
        {
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

        protected override ConsumedCapability? ResolvedCap
        {
            get
            {
                lock (_question.ReentrancyBlocker)
                {
                    if (!_question.IsTailCall && _question.StateFlags.HasFlag(PendingQuestion.State.Returned))
                    {
                        try
                        {
                            return _whenResolvedProxy.Result.ConsumedCap;
                        }
                        catch (AggregateException exception)
                        {
                            throw exception.InnerException!;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public override Task WhenResolved => _whenResolvedProxy;

        public override T? GetResolvedCapability<T>() where T: class => _whenResolvedProxy.GetResolvedCapability<T>();

        protected override void GetMessageTarget(MessageTarget.WRITER wr)
        {
            wr.which = MessageTarget.WHICH.PromisedAnswer;
            wr.PromisedAnswer!.QuestionId = _question.QuestionId;
            _access.Serialize(wr.PromisedAnswer);
        }

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            lock (_question.ReentrancyBlocker)
            {
                if (!_question.StateFlags.HasFlag(PendingQuestion.State.TailCall) &&
                     _question.StateFlags.HasFlag(PendingQuestion.State.Returned))
                {
                    try
                    {
                        if (ResolvedCap == null)
                        {
                            throw new RpcException("Answer did not resolve to expected capability");
                        }
                    }
                    catch
                    {
                        args.Dispose();
                        throw;
                    }

                    return CallOnResolution(interfaceId, methodId, args);
                }
                else
                {
#if DebugEmbargos
                    Logger.LogDebug("Call by proxy");
#endif
                    if (_question.StateFlags.HasFlag(PendingQuestion.State.CanceledByDispose))
                    {
                        args.Dispose();
                        throw new ObjectDisposedException(nameof(PendingQuestion));
                    }

                    if (_question.StateFlags.HasFlag(PendingQuestion.State.FinishRequested))
                    {
                        args.Dispose();
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
            call.Target.PromisedAnswer!.QuestionId = _question.QuestionId;
            _access.Serialize(call.Target.PromisedAnswer);

            return call;
        }

        internal override Action? Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            lock (_question.ReentrancyBlocker)
            {
                if (_question.StateFlags.HasFlag(PendingQuestion.State.CanceledByDispose))
                    throw new ObjectDisposedException(nameof(PendingQuestion));

                if (_question.StateFlags.HasFlag(PendingQuestion.State.Returned) && !_question.IsTailCall)
                {
                    ResolvedCap!.Export(endpoint, writer);
                }
                else
                {
                    if (_question.StateFlags.HasFlag(PendingQuestion.State.FinishRequested))
                        throw new InvalidOperationException("Finish request was already sent");

                    if (endpoint == _ep)
                    {
                        writer.which = CapDescriptor.WHICH.ReceiverAnswer;
                        _access.Serialize(writer.ReceiverAnswer!);
                        writer.ReceiverAnswer!.QuestionId = _question.QuestionId;
                    }
                    else if (_question.IsTailCall)
                    {
                        var vine = Vine.Create(this);
                        uint id = endpoint.AllocateExport(vine, out bool first);

                        writer.which = CapDescriptor.WHICH.SenderHosted;
                        writer.SenderHosted = id;
                    }
                    else
                    {
                        return this.ExportAsSenderPromise(endpoint, writer);
                    }
                }
            }

            return null;
        }

        protected async override void ReleaseRemotely()
        {
            if (!_question.IsTailCall)
            {
                try { using var _ = await _whenResolvedProxy; }
                catch { }
            }
        }

        internal override void AddRef()
        {
            base.AddRef();
            _question.AddRef();
        }

        internal override void Release()
        {
            _question.Release();
            base.Release();
        }
    }
}