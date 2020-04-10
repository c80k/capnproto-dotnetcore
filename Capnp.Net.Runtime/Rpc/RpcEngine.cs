using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// Implements the Cap'n Proto RPC protocol.
    /// </summary>
    public class RpcEngine
    {
        class RefCounted<T>
        {
            public T Cap { get; }
            public int RefCount { get; private set; }

            public RefCounted(T cap)
            {
                Cap = cap;
                RefCount = 1;
            }

            public void AddRef()
            {
                ++RefCount;
            }

            public void ReleaseAll()
            {
                RefCount = 0;
            }

            public void Release(int count)
            {
                if (count > RefCount)
                    throw new ArgumentOutOfRangeException(nameof(count));

                RefCount -= count;
            }
        }

        /// <summary>
        /// Stateful implementation for hosting a two-party RPC session. <see cref="RpcEngine"/> may own multiple mutually 
        /// independent endpoints.
        /// </summary>
        public class RpcEndpoint : IEndpoint, IRpcEndpoint
        {
            /// <summary>
            /// Endpoint state
            /// </summary>
            public enum EndpointState
            {
                /// <summary>
                /// Active means ready for exchanging RPC messages.
                /// </summary>
                Active,

                /// <summary>
                /// The session is closed, either deliberately or due to an error condition.
                /// </summary>
                Dismissed
            }

            static readonly ThreadLocal<PendingQuestion?> _deferredCall = new ThreadLocal<PendingQuestion?>();
            static readonly ThreadLocal<bool> _canDeferCalls = new ThreadLocal<bool>();

            ILogger Logger { get; } = Logging.CreateLogger<RpcEndpoint>();

            readonly RpcEngine _host;
            readonly IEndpoint _tx;

            readonly Dictionary<uint, RefCounted<RemoteCapability>> _importTable = new Dictionary<uint, RefCounted<RemoteCapability>>();
            readonly Dictionary<uint, RefCounted<Skeleton>> _exportTable = new Dictionary<uint, RefCounted<Skeleton>>();
            readonly Dictionary<Skeleton, uint> _revExportTable = new Dictionary<Skeleton, uint>();
            readonly Dictionary<uint, PendingQuestion> _questionTable = new Dictionary<uint, PendingQuestion>();
            readonly Dictionary<uint, PendingAnswer> _answerTable = new Dictionary<uint, PendingAnswer>();
            readonly Dictionary<uint, TaskCompletionSource<int>> _pendingDisembargos = new Dictionary<uint, TaskCompletionSource<int>>();
            readonly object _reentrancyBlocker = new object();
            readonly object _callReturnBlocker = new object();

            long _recvCount;
            long _sendCount;
            uint _nextId;

            internal RpcEndpoint(RpcEngine host, IEndpoint tx)
            {
                _host = host;
                _tx = tx;
                State = EndpointState.Active;
            }

            /// <summary>
            /// Session state
            /// </summary>
            public EndpointState State { get; private set; }

            /// <summary>
            /// Closes the session, clears export table, terminates all pending questions and enters 'Dismissed' state.
            /// </summary>
            public void Dismiss()
            {
                lock (_reentrancyBlocker)
                {
                    _exportTable.Clear();
                    _revExportTable.Clear();

                    foreach (var question in _questionTable.Values.ToList())
                    {
                        question.OnException(new RpcException("RPC connection is broken. Task would never return."));
                    }

                    Debug.Assert(_questionTable.Count == 0);

                    _answerTable.Clear();
                    _pendingDisembargos.Clear();

                    State = EndpointState.Dismissed;
                }

                _tx.Dismiss();
            }

            /// <summary>
            /// Feeds a frame for processing
            /// </summary>
            /// <param name="frame">frame to process</param>
            public void Forward(WireFrame frame)
            {
                if (State == EndpointState.Dismissed)
                    throw new InvalidOperationException("Endpoint is in dismissed state and doesn't accept frames anymore");

                Interlocked.Increment(ref _recvCount);
                ProcessFrame(frame);
            }

            /// <summary>
            /// Number of frames sent so far
            /// </summary>
            public long SendCount => Interlocked.Read(ref _sendCount);

            /// <summary>
            /// Number of frames received so far
            /// </summary>
            public long RecvCount => Interlocked.Read(ref _recvCount);

            /// <summary>
            /// Current number of entries in import table
            /// </summary>
            public int ImportedCapabilityCount
            {
                get
                {
                    lock (_reentrancyBlocker)
                    {
                        return _importTable.Count;
                    }
                }
            }

            /// <summary>
            /// Current number of entries in export table
            /// </summary>
            public int ExportedCapabilityCount
            {
                get
                {
                    lock (_reentrancyBlocker)
                    {
                        return _exportTable.Count;
                    }
                }
            }

            /// <summary>
            /// Current number of unanswered questions
            /// </summary>
            public int PendingQuestionCount
            {
                get
                {
                    lock (_reentrancyBlocker)
                    {
                        return _questionTable.Count;
                    }
                }
            }

            /// <summary>
            /// Current number of unfinished answers
            /// </summary>
            public int PendingAnswerCount
            {
                get
                {
                    lock (_reentrancyBlocker)
                    {
                        return _answerTable.Count;
                    }
                }
            }

            void Tx(WireFrame frame)
            {
                try
                {
                    _tx.Forward(frame);
                    Interlocked.Increment(ref _sendCount);
                }
                catch (System.Exception exception)
                {
                    Logger.LogWarning(exception, "Unable to send frame");
                    throw new RpcException("Unable to send frame", exception);
                }
            }

            void SendAbort(string reason)
            {
                try
                {
                    var mb = MessageBuilder.Create();
                    var msg = mb.BuildRoot<Message.WRITER>();
                    msg.which = Message.WHICH.Abort;
                    msg.Abort!.Reason = reason;
                    Tx(mb.Frame);
                }
                catch // Take care that an exception does not prevent shutdown.
                {
                }
            }

            void IRpcEndpoint.Resolve(uint preliminaryId, Skeleton preliminaryCap, Func<ConsumedCapability> resolvedCapGetter)
            {
                lock (_reentrancyBlocker)
                {
                    if (!_exportTable.TryGetValue(preliminaryId, out var existing) ||
                        existing.Cap != preliminaryCap ||
                        existing.RefCount == 0)
                    {
                        // Resolved too late. Capability was already released.
                        return;
                    }

                    existing.AddRef();
                    existing.Cap.Claim();
                }

                var mb = MessageBuilder.Create();
                var msg = mb.BuildRoot<Message.WRITER>();
                msg.which = Message.WHICH.Resolve;
                var resolve = msg.Resolve!;

                try
                {
                    var resolvedCap = resolvedCapGetter();
                    resolve.which = Resolve.WHICH.Cap;
                    resolvedCap.Export(this, resolve.Cap!);
                }
                catch (System.Exception ex)
                {
                    resolve.which = Resolve.WHICH.Exception;
                    resolve.Exception!.Reason = ex.Message;
                }
                resolve.PromiseId = preliminaryId;

                Tx(mb.Frame);

                ReleaseExport(preliminaryId, 1);
            }

            uint NextId()
            {
                return _nextId++;
            }

            uint AllocateExport(Skeleton providedCapability, out bool first)
            {
                lock (_reentrancyBlocker)
                {
                    providedCapability.Claim();

                    if (_revExportTable.TryGetValue(providedCapability, out uint id))
                    {
                        _exportTable[id].AddRef();
                        first = false;
                    }
                    else
                    {
                        do
                        {
                            id = NextId();

                        } while (_exportTable.ContainsKey(id));

                        _revExportTable.Add(providedCapability, id);
                        _exportTable.Add(id, new RefCounted<Skeleton>(providedCapability));
                        first = true;
                    }

                    return id;
                }
            }

            uint IRpcEndpoint.AllocateExport(Skeleton providedCapability, out bool first)
            {
                return AllocateExport(providedCapability, out first);
            }

            PendingQuestion AllocateQuestion(ConsumedCapability target, SerializerState? inParams)
            {
                lock (_reentrancyBlocker)
                {
                    uint questionId = NextId();
                    while (_questionTable.ContainsKey(questionId))
                        questionId = NextId();

                    var question = new PendingQuestion(this, questionId, target, inParams);
                    _questionTable.Add(questionId, question);

                    return question;
                }
            }

            (TaskCompletionSource<int>, uint) AllocateDisembargo()
            {
                var tcs = new TaskCompletionSource<int>();

                lock (_reentrancyBlocker)
                {
                    uint id = NextId();

                    while (!_pendingDisembargos.ReplacementTryAdd(id, tcs))
                    {
                        id = NextId();
                    }

                    return (tcs, id);
                }
            }

            void ProcessBootstrap(Bootstrap.READER req)
            {
                uint q = req.QuestionId;

                var bootstrap = DynamicSerializerState.CreateForRpc();
                var ans = bootstrap.MsgBuilder!.BuildRoot<Message.WRITER>();

                ans.which = Message.WHICH.Return;
                var ret = ans.Return!;
                ret.AnswerId = q;

                Task<AnswerOrCounterquestion> bootstrapTask;
                var bootstrapCap = _host.BootstrapCap;

                if (bootstrapCap != null)
                {
                    ret.which = Return.WHICH.Results;
                    bootstrap.SetCapability(bootstrap.ProvideCapability(bootstrapCap.AsCapability()));
                    ret.Results!.Content = bootstrap;

                    bootstrapTask = Task.FromResult<AnswerOrCounterquestion>(bootstrap);
                }
                else
                {
                    Logger.LogWarning("Peer asked for bootstrap capability, but no bootstrap capability was set.");

                    ret.which = Return.WHICH.Exception;
                    ret.Exception!.Reason = "No bootstrap capability present";

                    bootstrapTask = Task.FromException<AnswerOrCounterquestion>(new RpcException(ret.Exception.Reason));
                }

                var pendingAnswer = new PendingAnswer(bootstrapTask, null);

                bool added;
                lock (_reentrancyBlocker)
                {
                    added = _answerTable.ReplacementTryAdd(req.QuestionId, pendingAnswer);
                }

                if (!added)
                {
                    Logger.LogWarning("Incoming bootstrap request: Peer specified duplicate (not yet released?) answer ID.");
                    throw new RpcProtocolErrorException("Duplicate question ID");
                }


                if (ret.Results != null)
                {
                    ExportCapTableAndSend(bootstrap, ret.Results);
                }
                else
                {
                    Tx(bootstrap.MsgBuilder.Frame);
                }
            }

            void DispatchDeferredCalls()
            {
                var call = _deferredCall.Value;
                _deferredCall.Value = null;
                call?.Send();
            }

            void ProcessCall(Call.READER req)
            {
                lock (_callReturnBlocker)
                {
                    ProcessCallLocked(req);
                }
            }

            void ProcessCallLocked(Call.READER req)
            {
                Return.WRITER SetupReturn(MessageBuilder mb)
                {
                    var rmsg = mb.BuildRoot<Message.WRITER>();
                    rmsg.which = Message.WHICH.Return;
                    var ret = rmsg.Return!;
                    ret.AnswerId = req.QuestionId;

                    return ret;
                }

                void ReturnCallNoCapTable(Action<Return.WRITER> why)
                {
                    DispatchDeferredCalls();

                    var mb = MessageBuilder.Create();
                    mb.InitCapTable();
                    var ret = SetupReturn(mb);

                    why(ret);

                    try
                    {
                        lock (_callReturnBlocker)
                        {
                            Tx(mb.Frame);
                        }
                    }
                    catch (RpcException exception)
                    {
                        Logger.LogWarning($"Unable to return call: {exception.InnerException?.Message ?? exception.Message}");
                    }
                }

                Skeleton callTargetCap;
                PendingAnswer pendingAnswer;
                bool releaseParamCaps = true;

                void AwaitAnswerAndReply()
                {
                    bool added;
                    lock (_reentrancyBlocker)
                    {
                        added = _answerTable.ReplacementTryAdd(req.QuestionId, pendingAnswer);
                    }

                    if (!added)
                    {
                        Logger.LogWarning("Incoming RPC call: Peer specified duplicate (not yet released?) answer ID.");

                        pendingAnswer.Cancel();
                        pendingAnswer.Dispose();

                        throw new RpcProtocolErrorException($"There is another pending answer for the same question ID {req.QuestionId}.");
                    }

                    switch (req.SendResultsTo.which)
                    {
                        case Call.sendResultsTo.WHICH.Caller:
                            pendingAnswer.Chain(async t =>
                            {
                                try
                                {
                                    var aorcq = await t;

                                    if (aorcq.Answer == null && aorcq.Counterquestion == null)
                                    {
                                        Debug.Fail("Either answer or counter question must be present");
                                    }
                                    else if (aorcq.Answer != null || aorcq.Counterquestion != _deferredCall.Value)
                                    {
                                        var results = aorcq.Answer ?? (DynamicSerializerState)(await aorcq.Counterquestion!.WhenReturned);
                                        var ret = SetupReturn(results.MsgBuilder!);

                                        switch (req.SendResultsTo.which)
                                        {
                                            case Call.sendResultsTo.WHICH.Caller:
                                                ret.which = Return.WHICH.Results;
                                                ret.Results!.Content = results.Rewrap<DynamicSerializerState>();
                                                ret.ReleaseParamCaps = releaseParamCaps;
                                                DispatchDeferredCalls();
                                                ExportCapTableAndSend(results, ret.Results);
                                                pendingAnswer.CapTable = ret.Results.CapTable;
                                                break;

                                            case Call.sendResultsTo.WHICH.Yourself:
                                                ReturnCallNoCapTable(ret2 =>
                                                {
                                                    ret2.which = Return.WHICH.ResultsSentElsewhere;
                                                    ret2.ReleaseParamCaps = releaseParamCaps;
                                                });
                                                break;
                                        }                                        
                                    }
                                    else if (aorcq.Counterquestion != null)
                                    {
                                        _deferredCall.Value = null;
                                        aorcq.Counterquestion.IsTailCall = true;
                                        aorcq.Counterquestion.Send();

                                        ReturnCallNoCapTable(ret2 =>
                                        {
                                            ret2.which = Return.WHICH.TakeFromOtherQuestion;
                                            ret2.TakeFromOtherQuestion = aorcq.Counterquestion.QuestionId;
                                            ret2.ReleaseParamCaps = releaseParamCaps;
                                        });
                                    }
                                }
                                catch (TaskCanceledException)
                                {
                                    ReturnCallNoCapTable(ret =>
                                    {
                                        ret.which = Return.WHICH.Canceled;
                                        ret.ReleaseParamCaps = releaseParamCaps;
                                    });
                                }
                                catch (System.Exception exception)
                                {
                                    ReturnCallNoCapTable(ret =>
                                    {
                                        ret.which = Return.WHICH.Exception;
                                        ret.Exception!.Reason = exception.Message;
                                        ret.ReleaseParamCaps = releaseParamCaps;
                                    });
                                }
                            });
                            break;

                        case Call.sendResultsTo.WHICH.Yourself:
                            pendingAnswer.Chain(async t =>
                            {
                                try
                                {
                                    await t;
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    ReturnCallNoCapTable(ret =>
                                    {
                                        ret.which = Return.WHICH.ResultsSentElsewhere;
                                        ret.ReleaseParamCaps = releaseParamCaps;
                                    });
                                }
                            });
                            break;
                    }
                }

                void CreateAnswerAwaitItAndReply()
                {
                    var inParams = req.Params.Content;
                    inParams.Caps = ImportCapTable(req.Params);
                    releaseParamCaps = false;

                    try
                    {
                        var cts = new CancellationTokenSource();
                        var callTask = callTargetCap.Invoke(req.InterfaceId, req.MethodId, inParams, cts.Token);
                        pendingAnswer = new PendingAnswer(callTask, cts);
                    }
                    catch (System.Exception exception)
                    {
                        foreach (var cap in inParams.Caps)
                        {
                            cap?.Release();
                        }

                        pendingAnswer = new PendingAnswer(
                            Task.FromException<AnswerOrCounterquestion>(exception), null);
                    }
                    finally
                    {
                        callTargetCap.Relinquish();
                    }

                    AwaitAnswerAndReply();
                }

                switch (req.SendResultsTo.which)
                {
                    case Call.sendResultsTo.WHICH.Caller:
                    case Call.sendResultsTo.WHICH.Yourself:
                        break;

                    case Call.sendResultsTo.WHICH.ThirdParty:
                        Logger.LogWarning("Incoming RPC call: Peer requested sending results to 3rd party, which is not (yet) supported.");
                        throw new RpcUnimplementedException();

                    default:
                        Logger.LogWarning("Incoming RPC call: Peer requested unknown send-results-to mode.");
                        throw new RpcUnimplementedException();
                }

                _canDeferCalls.Value = true;
                Impatient.PushAskingEndpoint(this);

                try
                {
                    switch (req.Target.which)
                    {
                        case MessageTarget.WHICH.ImportedCap:

                            lock (_reentrancyBlocker)
                            {
                                if (_exportTable.TryGetValue(req.Target.ImportedCap, out var rc))
                                {
                                    callTargetCap = rc.Cap;
                                    callTargetCap.Claim();
                                }
                                else
                                {
                                    Logger.LogWarning("Incoming RPC call: Peer asked for invalid (already released?) capability ID.");

                                    throw new RpcProtocolErrorException($"Requested capability with ID {req.Target.ImportedCap} does not exist.");
                                }
                            }

                            CreateAnswerAwaitItAndReply();

                            break;

                        case MessageTarget.WHICH.PromisedAnswer:
                            {
                                bool exists;
                                PendingAnswer? previousAnswer;

                                lock (_reentrancyBlocker)
                                {
                                    exists = _answerTable.TryGetValue(req.Target.PromisedAnswer.QuestionId, out previousAnswer);
                                }

                                if (exists)
                                {
                                    previousAnswer!.Chain(
                                        req.Target.PromisedAnswer,
                                        async t =>
                                        {
                                            try
                                            {
                                                using var proxy = await t;
                                                callTargetCap = await proxy.GetProvider();
                                                callTargetCap.Claim();
                                                CreateAnswerAwaitItAndReply();
                                            }
                                            catch (TaskCanceledException)
                                            {
                                                pendingAnswer = new PendingAnswer(
                                                    Task.FromCanceled<AnswerOrCounterquestion>(previousAnswer.CancellationToken), null);

                                                AwaitAnswerAndReply();
                                            }
                                            catch (System.Exception exception)
                                            {
                                                pendingAnswer = new PendingAnswer(
                                                    Task.FromException<AnswerOrCounterquestion>(exception), null);

                                                AwaitAnswerAndReply();
                                            }
                                        });
                                }
                                else
                                {
                                    Logger.LogWarning("Incoming RPC call: Peer asked for non-existing answer ID.");
                                    throw new RpcProtocolErrorException($"Did not find a promised answer for given ID {req.Target.PromisedAnswer.QuestionId}");
                                }
                            }
                            break;

                        default:
                            Logger.LogWarning("Incoming RPC call: Peer specified unknown call target.");

                            throw new RpcUnimplementedException();
                    }
                }
                finally
                {
                    _canDeferCalls.Value = false;
                    Impatient.PopAskingEndpoint();
                    DispatchDeferredCalls();
                }
            }

            void ProcessReturn(Return.READER req)
            {
                PendingQuestion? question;

                lock (_reentrancyBlocker)
                {
                    if (!_questionTable.TryGetValue(req.AnswerId, out question))
                    {
                        Logger.LogWarning("Incoming RPC return: Unknown answer ID.");

                        throw new RpcProtocolErrorException("Unknown answer ID");
                    }
                }

                if (req.ReleaseParamCaps)
                {
                    ReleaseExports(question.CapTable);
                }

                switch (req.which)
                {
                    case Return.WHICH.Results:
                        var content = req.Results.Content;
                        content.Caps = ImportCapTable(req.Results);
                        question.OnReturn(content);
                        break;

                    case Return.WHICH.AcceptFromThirdParty:
                        Logger.LogWarning(
                            "Incoming RPC return: Peer requested to accept results from 3rd party, which is not (yet) supported.");

                        throw new RpcUnimplementedException();

                    case Return.WHICH.Canceled:
                        question.OnCanceled();
                        break;

                    case Return.WHICH.Exception:
                        question.OnException(req.Exception);
                        break;

                    case Return.WHICH.ResultsSentElsewhere:
                        question.OnTailCallReturn();
                        break;

                    case Return.WHICH.TakeFromOtherQuestion:
                        {
                            bool exists;
                            PendingAnswer? pendingAnswer;

                            lock (_reentrancyBlocker)
                            {
                                exists = _answerTable.TryGetValue(req.TakeFromOtherQuestion, out pendingAnswer);
                            }

                            if (exists)
                            {
                                pendingAnswer!.Chain(async t =>
                                {
                                    try
                                    {
                                        var aorcq = await t;
                                        var results = aorcq.Answer;
                                        
                                        if (results != null)
                                        {
                                            question.OnReturn(results);
                                        }
                                        else
                                        {
                                            question.OnTailCallReturn();
                                        }
                                    }
                                    catch (TaskCanceledException)
                                    {
                                        question.OnCanceled();
                                    }
                                    catch (System.Exception exception)
                                    {
                                        question.OnException(exception);
                                    }
                                });
                            }
                            else
                            {
                                Logger.LogWarning("Incoming RPC return: Peer requested to take results from other question, but specified ID is unknown (already released?)");
                                throw new RpcProtocolErrorException("Invalid ID");
                            }
                        }
                        break;

                    default:
                        throw new RpcUnimplementedException();
                }
            }

            void ProcessResolve(Resolve.READER resolve)
            {
                lock (_reentrancyBlocker)
                {
                    if (!_importTable.TryGetValue(resolve.PromiseId, out var rcc))
                    {
                        // May happen if Resolve arrives late. Not an actual error.

                        if (resolve.which == Resolve.WHICH.Cap)
                        {
                            // Import and release immediately
                            var imp = ImportCap(resolve.Cap);
                            imp.AddRef();
                            imp.Release();
                        }

                        return;
                    }

                    var cap = rcc.Cap;

                    if (!(cap is PromisedCapability resolvableCap))
                    {
                        Logger.LogWarning("Received a resolve message for a capability which is not a promise");
                        throw new RpcProtocolErrorException($"Not a promise {resolve.PromiseId}");
                    }

                    try
                    {
                        switch (resolve.which)
                        {
                            case Resolve.WHICH.Cap:
                                var resolvedCap = ImportCap(resolve.Cap);
                                resolvableCap.ResolveTo(resolvedCap);
                                break;

                            case Resolve.WHICH.Exception:
                                resolvableCap.Break(resolve.Exception.Reason ?? "unknown reason");
                                break;

                            default:
                                Logger.LogWarning("Received a resolve message with unknown category.");
                                throw new RpcUnimplementedException();
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        throw new RpcProtocolErrorException($"Capability {resolve.PromiseId} was already resolved");
                    }
                }
            }

            void ProcessSenderLoopback(Disembargo.READER disembargo)
            {
                var mb = MessageBuilder.Create();
                mb.InitCapTable();
                var wr = mb.BuildRoot<Message.WRITER>();
                wr.which = Message.WHICH.Disembargo;
                wr.Disembargo!.Context.which = Disembargo.context.WHICH.ReceiverLoopback;
                wr.Disembargo!.Context.ReceiverLoopback = disembargo.Context.SenderLoopback;
                var reply = wr.Disembargo;

                switch (disembargo.Target.which)
                {
                    case MessageTarget.WHICH.ImportedCap:

                        if (!_exportTable.TryGetValue(disembargo.Target.ImportedCap, out var cap))
                        {
                            Logger.LogWarning("Sender loopback request: Peer asked for invalid (already released?) capability ID.");

                            throw new RpcProtocolErrorException("'Disembargo': Invalid capability ID");
                        }

                        reply.Target.which = MessageTarget.WHICH.ImportedCap;
                        reply.Target.ImportedCap = disembargo.Target.ImportedCap;

                        Tx(mb.Frame);

                        break;

                    case MessageTarget.WHICH.PromisedAnswer:

                        var promisedAnswer = disembargo.Target.PromisedAnswer;
                        reply.Target.which = MessageTarget.WHICH.PromisedAnswer;
                        var replyPromisedAnswer = reply.Target.PromisedAnswer;
                        replyPromisedAnswer!.QuestionId = disembargo.Target.PromisedAnswer.QuestionId;
                        int count = promisedAnswer.Transform.Count;
                        replyPromisedAnswer.Transform.Init(count);

                        for (int i = 0; i < count; i++)
                        {
                            replyPromisedAnswer.Transform[i].which = promisedAnswer.Transform[i].which;
                            replyPromisedAnswer.Transform[i].GetPointerField = promisedAnswer.Transform[i].GetPointerField;
                        }

                        if (_answerTable.TryGetValue(promisedAnswer.QuestionId, out var previousAnswer))
                        {
                            previousAnswer.Chain(
                                disembargo.Target.PromisedAnswer,
                                async t =>
                                {
                                    using var proxy = await t;

                                    if (proxy.ConsumedCap is RemoteCapability remote && remote.Endpoint == this)
                                    {
#if DebugEmbargos
                                        Logger.LogDebug($"Sender loopback disembargo. Thread = {Thread.CurrentThread.Name}");
#endif
                                        Tx(mb.Frame);
                                    }
                                    else
                                    {
                                        Logger.LogWarning("Sender loopback request: Peer asked for disembargoing an answer which does not resolve back to the sender.");

                                        throw new RpcProtocolErrorException("'Disembargo': Answer does not resolve back to me");
                                    }
                                });
                        }
                        else
                        {
                            Logger.LogWarning("Sender loopback request: Peer asked for non-existing answer ID.");

                            throw new RpcProtocolErrorException("'Disembargo': Invalid answer ID");
                        }

                        break;

                    default:
                        Logger.LogWarning("Sender loopback request: Peer specified unknown call target.");

                        throw new RpcUnimplementedException();
                }
            }

            void ProcessReceiverLoopback(Disembargo.READER disembargo)
            {
                bool exists;
                TaskCompletionSource<int> tcs;

                lock (_reentrancyBlocker)
                {
                    exists = _pendingDisembargos.ReplacementTryRemove(disembargo.Context.ReceiverLoopback, out tcs);
                }

                if (exists)
                {
                    // FIXME: The current design does not admit for verifying the target/context components of
                    // the disembargo message. We just rely on the peer echoing the stuff back that we sent.
                    // Indeed, this should never be an issue in the absence of bugs and/or attacks. Even unsure,
                    // whether this is a security issue: Can the sloppy checking be exploited in some way?

#if DebugEmbargos
                    Logger.LogDebug($"Receiver loopback disembargo, Thread = {Thread.CurrentThread.Name}");
#endif

                    tcs.SetResult(0);
                }
                else
                {
                    Logger.LogWarning("Peer sent receiver loopback with unknown ID");
                    throw new RpcProtocolErrorException("Invalid ID");
                }
            }

            void ProcessDisembargo(Disembargo.READER disembargo)
            {
                switch (disembargo.Context.which)
                {
                    case Disembargo.context.WHICH.ReceiverLoopback:
                        ProcessReceiverLoopback(disembargo);
                        break;

                    case Disembargo.context.WHICH.SenderLoopback:
                        ProcessSenderLoopback(disembargo);
                        break;

                    case Disembargo.context.WHICH.Accept:
                    case Disembargo.context.WHICH.Provide:
                    default:
                        throw new RpcUnimplementedException();
                }
            }

            void ReleaseExports(IReadOnlyList<CapDescriptor.WRITER>? caps)
            {
                if (caps != null)
                {
                    foreach (var capDesc in caps)
                    {
                        switch (capDesc.which)
                        {
                            case CapDescriptor.WHICH.SenderHosted:
                                ReleaseExport(capDesc.SenderHosted, 1);
                                break;

                            case CapDescriptor.WHICH.SenderPromise:
                                ReleaseExport(capDesc.SenderPromise, 1);
                                break;
                        }
                    }
                }
            }

            void ReleaseResultCaps(PendingAnswer answer)
            {
                answer.Chain(async t =>
                {
                    try
                    {
                        var aorcq = await t;
                        ReleaseExports(answer.CapTable);
                    }
                    catch
                    {
                    }
                });
            }

            void ProcessFinish(Finish.READER finish)
            {
                bool exists;
                PendingAnswer answer;

                lock (_reentrancyBlocker)
                {
                    exists = _answerTable.ReplacementTryRemove(finish.QuestionId, out answer);
                }

                if (exists)
                {
                    if (finish.ReleaseResultCaps)
                    {
                        ReleaseResultCaps(answer);
                    }

                    answer.Cancel();
                    answer.Dispose();
                }
                else
                {
                    Logger.LogWarning("Peer sent 'finish' message with unknown question ID");

                    throw new RpcProtocolErrorException("unknown question ID");
                }
            }

            void ReleaseExport(uint id, uint count)
            {
                bool exists;

                lock (_reentrancyBlocker)
                {
                    exists = _exportTable.TryGetValue(id, out var rc);

                    if (exists)
                    {
                        try
                        {
                            int icount = checked((int)count);
                            rc!.Release(icount);
                            rc!.Cap.Relinquish(icount);

                            if (rc.RefCount == 0)
                            {
                                _exportTable.Remove(id);
                                _revExportTable.ReplacementTryRemove(rc.Cap, out uint _);
                            }
                        }
                        catch (System.Exception exception)
                        {
                            Logger.LogWarning($"Attempting to release capability with invalid reference count: {exception.Message}");

                            throw new RpcProtocolErrorException("Invalid reference count");
                        }
                    }
                }

                if (!exists)
                {
                    Logger.LogWarning("Attempting to release unknown capability ID");

                    throw new RpcProtocolErrorException("Invalid export ID");
                }
            }

            void ProcessRelease(Release.READER release)
            {
                ReleaseExport(release.Id, release.ReferenceCount);
            }

            void ProcessUnimplementedResolve(Resolve.READER resolve)
            {
                if (resolve.which == Resolve.WHICH.Cap)
                {
                    switch (resolve.Cap.which)
                    {
                        case CapDescriptor.WHICH.SenderHosted:
                            ReleaseExport(resolve.Cap.SenderHosted, 1);
                            break;

                        case CapDescriptor.WHICH.SenderPromise:
                            // Not really expected that a promise gets resolved to another promise.
                            ReleaseExport(resolve.Cap.SenderPromise, 1);
                            break;

                        default:
                            break;
                    }
                }
            }

            void ProcessUnimplementedCall(Call.READER call)
            {
                PendingQuestion? question;

                lock (_reentrancyBlocker)
                {
                    if (!_questionTable.TryGetValue(call.QuestionId, out question))
                    {
                        Logger.LogWarning("Unimplemented call: Unknown question ID.");

                        throw new RpcProtocolErrorException("Unknown question ID");
                    }
                }

                ReleaseExports(question.CapTable);
            }

            void ProcessUnimplemented(Message.READER unimplemented)
            {
                switch (unimplemented.which)
                {
                    case Message.WHICH.Resolve:
                        //# For example, say `resolve` is received by a level 0 implementation (because a previous call
                        //# or return happened to contain a promise).  The level 0 implementation will echo it back as
                        //# `unimplemented`.  The original sender can then simply release the cap to which the promise
                        //# had resolved, thus avoiding a leak.
                        ProcessUnimplementedResolve(unimplemented.Resolve);
                        break;

                    case Message.WHICH.Call:
                        //# For any message type that introduces a question, if the message comes back unimplemented,
                        //# the original sender may simply treat it as if the question failed with an exception.
                        ProcessUnimplementedCall(unimplemented.Call);
                        break;

                    case Message.WHICH.Bootstrap:
                        //# In cases where there is no sensible way to react to an `unimplemented` message (without
                        //# resource leaks or other serious problems), the connection may need to be aborted.  This is
                        //# a gray area; different implementations may take different approaches.
                        throw new RpcProtocolErrorException("It's hopeless if you don't implement the bootstrap message");

                    default:
                        // Looking at the various message types it feels OK to just not care about other 'unimplemented'
                        // responses: You don't support Abort? Not my problem, I will drop the connection anyway. Don't
                        // support Disembargo? At least I tried. Don't support Finish/Release/Return? Why should I care?
                        // Don't support Unimplemented? Umm, well. Don't support Accept/Join/Provide? Interesting, I never
                        // send such messages, since I'm not a level 3 implementation.
                        break;
                }
            }

            /// <summary>
            /// Queries the peer for its bootstrap capability
            /// </summary>
            /// <returns>low-level capability</returns>
            public ConsumedCapability QueryMain()
            {
                var mb = MessageBuilder.Create();
                mb.InitCapTable();
                var req = mb.BuildRoot<Message.WRITER>();
                req.which = Message.WHICH.Bootstrap;
                var pendingBootstrap = AllocateQuestion(NullCapability.Instance, null);
                req.Bootstrap!.QuestionId = pendingBootstrap.QuestionId;

                Tx(mb.Frame);

                var main = new RemoteAnswerCapability(
                    pendingBootstrap,
                    MemberAccessPath.BootstrapAccess);

                return main;
            }

            void ProcessFrame(WireFrame frame)
            {
                var dec = DeserializerState.CreateRoot(frame);
                var msg = Message.READER.create(dec);

                try
                {
                    switch (msg.which)
                    {
                        case Message.WHICH.Abort:
                            Logger.LogInformation($"Got 'abort' '{msg.Abort.TheType}' message from peer: {msg.Abort.Reason}");
                            break;

                        case Message.WHICH.Bootstrap:
                            ProcessBootstrap(msg.Bootstrap);
                            break;

                        case Message.WHICH.Call:
                            ProcessCall(msg.Call);
                            break;

                        case Message.WHICH.Disembargo:
                            ProcessDisembargo(msg.Disembargo);
                            break;

                        case Message.WHICH.Finish:
                            ProcessFinish(msg.Finish);
                            break;

                        case Message.WHICH.Release:
                            ProcessRelease(msg.Release);
                            break;

                        case Message.WHICH.Resolve:
                            ProcessResolve(msg.Resolve);
                            break;

                        case Message.WHICH.Return:
                            ProcessReturn(msg.Return);
                            break;

                        case Message.WHICH.Unimplemented:
                            ProcessUnimplemented(msg.Unimplemented);
                            break;

                        case Message.WHICH.Accept:
                        case Message.WHICH.Join:
                        case Message.WHICH.Provide:
                            Logger.LogWarning("Received level-3 message from peer. I don't support that.");
                            throw new RpcUnimplementedException();

                        case Message.WHICH.ObsoleteDelete:
                        case Message.WHICH.ObsoleteSave:
                        default:
                            Logger.LogWarning("Received unknown or unimplemented message from peer");
                            throw new RpcUnimplementedException();
                    }
                }
                catch (RpcUnimplementedException)
                {
                    var mb = MessageBuilder.Create();
                    mb.InitCapTable();
                    var reply = mb.BuildRoot<Message.WRITER>();
                    reply.which = Message.WHICH.Unimplemented;
                    Reserializing.DeepCopy(msg, reply.Unimplemented!.Rewrap<DynamicSerializerState>());

                    Tx(mb.Frame);
                }
                catch (RpcProtocolErrorException error)
                {
                    SendAbort(error.Message);
                    Dismiss();
                }
                catch (System.Exception exception)
                {
                    Logger.LogError(exception, "Uncaught exception during message processing.");

                    // A first intuition might be to send the caught exception message. But this is probably a bad idea:
                    // First, we send implementation specific details of a - maybe internal - error, not very valuable for the
                    // receiver. But worse: From a security point of view, we might even reveil a secret here.
                    SendAbort("Uncaught exception during RPC processing. This may also happen due to invalid input.");
                    Dismiss();
                }
            }

            ConsumedCapability ImportCap(CapDescriptor.READER capDesc)
            {
                lock (_reentrancyBlocker)
                {
                    switch (capDesc.which)
                    {
                        case CapDescriptor.WHICH.SenderHosted:
                            if (_importTable.TryGetValue(capDesc.SenderHosted, out var rcc))
                            {
                                var impCap = rcc.Cap;
                                impCap.Validate();
                                rcc.AddRef();
                                return impCap;
                            }
                            else
                            {
                                var newCap = new ImportedCapability(this, capDesc.SenderHosted);
                                rcc = new RefCounted<RemoteCapability>(newCap);
                                _importTable.Add(capDesc.SenderHosted, rcc);
                                return newCap;
                            }

                        case CapDescriptor.WHICH.SenderPromise:
                            if (_importTable.TryGetValue(capDesc.SenderPromise, out var rccp))
                            {
                                var impCap = rccp.Cap;
                                impCap.Validate();
                                rccp.AddRef();
                                return impCap;
                            }
                            else
                            {
                                var newCap = new PromisedCapability(this, capDesc.SenderPromise);
                                rccp = new RefCounted<RemoteCapability>(newCap);
                                _importTable.Add(capDesc.SenderPromise, rccp);
                                return newCap;
                            }

                        case CapDescriptor.WHICH.ReceiverHosted:
                            if (_exportTable.TryGetValue(capDesc.ReceiverHosted, out var rc))
                            {
                                return rc.Cap.AsCapability();
                            }
                            else
                            {
                                Logger.LogWarning("Peer refers to receiver-hosted capability which does not exist.");
                                throw new RpcProtocolErrorException($"Receiver-hosted capability {capDesc.ReceiverHosted} does not exist.");
                            }

                        case CapDescriptor.WHICH.ReceiverAnswer:
                            if (_answerTable.TryGetValue(capDesc.ReceiverAnswer.QuestionId, out var pendingAnswer))
                            {
                                var tcs = new TaskCompletionSource<Proxy>();

                                pendingAnswer.Chain(
                                    capDesc.ReceiverAnswer,
                                    async t =>
                                    {
                                        try
                                        {
                                            var proxy = await t;
                                            tcs.SetResult(proxy);
                                        }
                                        catch (TaskCanceledException)
                                        {
                                            tcs.SetCanceled();
                                        }
                                        catch (System.Exception exception)
                                        {
                                            tcs.SetException(exception);
                                        }
                                    });

                                return new LazyCapability(tcs.Task);
                            }
                            else
                            {
                                Logger.LogWarning("Peer refers to pending answer which does not exist.");
                                throw new RpcProtocolErrorException($"Invalid question ID {capDesc.ReceiverAnswer.QuestionId}");
                            }

                        case CapDescriptor.WHICH.ThirdPartyHosted:
                            if (_importTable.TryGetValue(capDesc.ThirdPartyHosted.VineId, out var rcv))
                            {
                                var impCap = rcv.Cap;
                                rcv.AddRef();
                                impCap.Validate();
                                return impCap;
                            }
                            else
                            {
                                var newCap = new ImportedCapability(this, capDesc.ThirdPartyHosted.VineId);
                                rcv = new RefCounted<RemoteCapability>(newCap);
                                return newCap;
                            }

                        case CapDescriptor.WHICH.None:
                            return NullCapability.Instance;

                        default:
                            Logger.LogWarning("Unknown capability descriptor category");
                            throw new RpcUnimplementedException();
                    }
                }
            }

            internal IList<ConsumedCapability> ImportCapTable(Payload.READER payload)
            {
                var list = new List<ConsumedCapability>();

                if (payload.CapTable != null)
                {
                    lock (_reentrancyBlocker)
                    {
                        foreach (var capDesc in payload.CapTable)
                        {
                            var cap = ImportCap(capDesc);
                            cap.AddRef();
                            list.Add(cap);
                        }
                    }
                }

                return list;
            }

            void ExportCapTableAndSend(
                SerializerState state,
                Payload.WRITER payload)
            {
                payload.CapTable.Init(state.MsgBuilder!.Caps!.Count);

                Action? postAction = null;
                int i = 0;
                foreach (var cap in state.MsgBuilder.Caps)
                {
                    var capDesc = payload.CapTable[i++];
                    postAction += cap.Export(this, capDesc);
                    cap.Release();
                }

                Tx(state.MsgBuilder.Frame);

                // The reason for this seemingly cumbersome postAction handling is as follows:
                // If a sender-promise capability happens to resolve concurrently, we must not
                // send the "resolve" message before even sending the sender-promise descriptor.
                // To avoid that situation, calls to "ReExportCapWhenResolved" are queued (and
                // therefore deferred) to the postAction.

                postAction?.Invoke();
            }

            PendingQuestion IRpcEndpoint.BeginQuestion(ConsumedCapability target, SerializerState inParams)
            {
                var question = AllocateQuestion(target, inParams);

                if (_canDeferCalls.Value)
                {
                    DispatchDeferredCalls();
                    _deferredCall.Value = question;
                }
                else
                {
                    question.Send();
                }

                return question;
            }

            void IRpcEndpoint.SendQuestion(SerializerState inParams, Payload.WRITER payload)
            {
                ExportCapTableAndSend(inParams, payload);
            }

            void Finish(uint questionId)
            {
                var mb = MessageBuilder.Create();
                var msg = mb.BuildRoot<Message.WRITER>();
                msg.which = Message.WHICH.Finish;
                msg.Finish!.QuestionId = questionId;
                msg.Finish!.ReleaseResultCaps = false;

                try
                {
                    Tx(mb.Frame);
                }
                catch (System.Exception exception)
                {
                    Logger.LogWarning($"Unable to send 'finish' message: {exception.Message}");
                }

                // Note: Keep question ID in the table, since a "return" message with either "canceled" or
                // "results" is still expected (at least according to the spec).
            }

            void IRpcEndpoint.Finish(uint questionId)
            {
                Finish(questionId);
            }

            void IRpcEndpoint.ReleaseImport(uint importId)
            {
                bool exists;
                int count = 0;

                lock (_reentrancyBlocker)
                {
                    exists = _importTable.TryGetValue(importId, out var rc);
                    if (rc != null)
                    {
                        count = rc.RefCount;
                        rc.ReleaseAll();
                    }

                    if (exists)
                    {
                        _importTable.Remove(importId);
                    }
                }

                if (exists && count > 0)
                {
                    var mb = MessageBuilder.Create();
                    var msg = mb.BuildRoot<Message.WRITER>();
                    msg.which = Message.WHICH.Release;
                    msg.Release!.Id = importId;
                    msg.Release!.ReferenceCount = (uint)count;

                    try
                    {
                        Tx(mb.Frame);
                    }
                    catch (RpcException exception)
                    {
                        Logger.LogWarning($"Unable to release import: {exception.InnerException?.Message ?? exception.Message}");
                    }
                }
            }

            Task IRpcEndpoint.RequestSenderLoopback(Action<MessageTarget.WRITER> describe)
            {
                (var tcs, uint id) = AllocateDisembargo();

                var mb = MessageBuilder.Create();
                mb.InitCapTable();
                var msg = mb.BuildRoot<Message.WRITER>();
                msg.which = Message.WHICH.Disembargo;
                describe(msg.Disembargo!.Target);
                var ctx = msg.Disembargo.Context;
                ctx.which = Disembargo.context.WHICH.SenderLoopback;
                ctx.SenderLoopback = id;

                Tx(mb.Frame);

                return tcs.Task;
            }

            void IRpcEndpoint.DeleteQuestion(PendingQuestion question)
            {
                lock (_reentrancyBlocker)
                {
                    if (!_questionTable.Remove(question.QuestionId))
                    {
                        Logger.LogError("Attempting to delete unknown question ID.");
                    }
                }
            }
        }

        readonly ConcurrentBag<RpcEndpoint> _inboundEndpoints = new ConcurrentBag<RpcEndpoint>();

        /// <summary>
        /// Adds an endpoint
        /// </summary>
        /// <param name="outboundEndpoint">endpoint for handling outgoing messages</param>
        /// <returns>endpoint for handling incoming messages</returns>
        public RpcEndpoint AddEndpoint(IEndpoint outboundEndpoint)
        {
            var inboundEndpoint = new RpcEndpoint(this, outboundEndpoint);
            _inboundEndpoints.Add(inboundEndpoint);
            return inboundEndpoint;
        }

        Skeleton? _bootstrapCap;

        /// <summary>
        /// Gets or sets the bootstrap capability.
        /// </summary>
        public Skeleton? BootstrapCap
        {
            get => _bootstrapCap;
            set
            {
                value?.Claim();
                _bootstrapCap?.Relinquish();
                _bootstrapCap = value;
            }
        }

        /// <summary>
        /// Sets the bootstrap capability. It must be an object which implements a valid capability interface
        /// (<see cref="SkeletonAttribute"/>).
        /// </summary>
        public object Main
        {
            set { BootstrapCap = value is Skeleton skeleton ? skeleton : CapabilityReflection.CreateSkeletonInternal(value); }
        }
    }
}