using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
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

            public void Release()
            {
                --RefCount;
                CheckDispose();
            }

            public void ReleaseAll()
            {
                RefCount = 0;
                CheckDispose();
            }

            public void Release(int count)
            {
                if (count > RefCount)
                    throw new ArgumentOutOfRangeException(nameof(count));

                RefCount -= count;
                CheckDispose();
            }

            void CheckDispose()
            {
                if (RefCount == 0 && Cap is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        internal class RpcEndpoint : IEndpoint, IRpcEndpoint
        {
            static readonly ThreadLocal<Action> _exportCapTablePostActions = new ThreadLocal<Action>();
            static readonly ThreadLocal<PendingQuestion> _tailCall = new ThreadLocal<PendingQuestion>();
            static readonly ThreadLocal<bool> _canDeferCalls = new ThreadLocal<bool>();

            ILogger Logger { get; } = Logging.CreateLogger<RpcEndpoint>();

            readonly RpcEngine _host;
            readonly IEndpoint _tx;

            readonly Dictionary<uint, RefCounted<WeakReference<RemoteCapability>>> _importTable = new Dictionary<uint, RefCounted<WeakReference<RemoteCapability>>>();
            readonly Dictionary<uint, RefCounted<Skeleton>> _exportTable = new Dictionary<uint, RefCounted<Skeleton>>();
            readonly Dictionary<Skeleton, uint> _revExportTable = new Dictionary<Skeleton, uint>();
            readonly Dictionary<uint, PendingQuestion> _questionTable = new Dictionary<uint, PendingQuestion>();
            readonly Dictionary<uint, PendingAnswer> _answerTable = new Dictionary<uint, PendingAnswer>();
            readonly Dictionary<uint, TaskCompletionSource<int>> _pendingDisembargos = new Dictionary<uint, TaskCompletionSource<int>>();
            readonly object _reentrancyBlocker = new object();

            long _recvCount;
            long _sendCount;
            uint _nextId;

            internal RpcEndpoint(RpcEngine host, IEndpoint tx)
            {
                _host = host;
                _tx = tx;
            }

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
                }

                _tx.Dismiss();
            }

            public void Forward(WireFrame frame)
            {
                Interlocked.Increment(ref _recvCount);
                ProcessFrame(frame);
            }

            public long SendCount => Interlocked.Read(ref _sendCount);
            public long RecvCount => Interlocked.Read(ref _recvCount);

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
                var mb = MessageBuilder.Create();
                var msg = mb.BuildRoot<Message.WRITER>();
                msg.which = Message.WHICH.Abort;
                msg.Abort.Reason = reason;
                Tx(mb.Frame);
            }

            void IRpcEndpoint.Resolve(uint preliminaryId, Skeleton preliminaryCap, 
                Func<ConsumedCapability> resolvedCapGetter)
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
                var resolve = msg.Resolve;

                try
                {
                    var resolvedCap = resolvedCapGetter();
                    resolve.which = Resolve.WHICH.Cap;
                    resolvedCap.Export(this, resolve.Cap);
                }
                catch (System.Exception ex)
                {
                    resolve.which = Resolve.WHICH.Exception;
                    resolve.Exception.Reason = ex.Message;
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
                        first = false;

                        if (_exportTable.TryGetValue(id, out var rc))
                        {
                            rc.AddRef();
                        }
                        else
                        {
                            Logger.LogError("Inconsistent export table: Capability with id {0} exists in reverse table only", id);
                        }
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

            PendingQuestion AllocateQuestion(ConsumedCapability target, SerializerState inParams)
            {
                lock (_reentrancyBlocker)
                {
                    uint questionId = NextId();
                    var question = new PendingQuestion(this, questionId, target, inParams);

                    while (!_questionTable.ReplacementTryAdd(questionId, question))
                    {
                        questionId = NextId();
                        var oldQuestion = question;
                        question = new PendingQuestion(this, questionId, target, inParams);
                        oldQuestion.Dispose();
                    }

                    return question;
                }
            }

            void DeleteQuestion(uint id, PendingQuestion question)
            {
                lock (_reentrancyBlocker)
                {
                    if (!_questionTable.TryGetValue(id, out var existingQuestion))
                    {
                        Logger.LogError("Attempting to delete unknown question ID. Race condition?");
                        return;
                    }

                    if (question != existingQuestion)
                    {
                        Logger.LogError("Found different question under given ID. WTF???");
                        return;
                    }

                    _questionTable.Remove(id);
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
                var ans = bootstrap.MsgBuilder.BuildRoot<Message.WRITER>();

                ans.which = Message.WHICH.Return;
                var ret = ans.Return;
                ret.AnswerId = q;

                Task<AnswerOrCounterquestion> bootstrapTask;
                var bootstrapCap = _host.BootstrapCap;

                if (bootstrapCap != null)
                {
                    ret.which = Return.WHICH.Results;
                    bootstrap.SetCapability(bootstrap.ProvideCapability(LocalCapability.Create(_host.BootstrapCap)));
                    ret.Results.Content = bootstrap;

                    bootstrapTask = Task.FromResult<AnswerOrCounterquestion>(bootstrap);
                }
                else
                {
                    Logger.LogWarning("Peer asked for bootstrap capability, but no bootstrap capability was set.");

                    ret.which = Return.WHICH.Exception;
                    ret.Exception.Reason = "No bootstrap capability present";

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
                }


                if (bootstrapCap != null)
                {
                    ExportCapTableAndSend(bootstrap, ret.Results);
                }
                else
                {
                    Tx(bootstrap.MsgBuilder.Frame);
                }
            }


            void ProcessCall(Call.READER req)
            {
                Return.WRITER SetupReturn(MessageBuilder mb)
                {
                    var rmsg = mb.BuildRoot<Message.WRITER>();
                    rmsg.which = Message.WHICH.Return;
                    var ret = rmsg.Return;
                    ret.AnswerId = req.QuestionId;

                    return ret;
                }

                void ReturnCall(Action<Return.WRITER> why)
                {
                    var mb = MessageBuilder.Create();
                    mb.InitCapTable();
                    var ret = SetupReturn(mb);

                    why(ret);

                    try
                    {
                        Tx(mb.Frame);
                    }
                    catch (RpcException exception)
                    {
                        Logger.LogWarning($"Unable to return call: {exception.InnerException.Message}");
                    }
                }

                IProvidedCapability cap;
                PendingAnswer pendingAnswer = null;
                bool releaseParamCaps = false;

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

                        SendAbort($"There is another pending answer for the same question ID {req.QuestionId}.");

                        return;
                    }

                    switch (req.SendResultsTo.which)
                    {
                        case Call.sendResultsTo.WHICH.Caller:
                            pendingAnswer.Chain(false, async t =>
                            {
                                try
                                {
                                    var aorcq = await t;

                                    if (aorcq.Answer == null && aorcq.Counterquestion == null)
                                    {
                                        Debug.Fail("Either answer or counter question must be present");
                                    }
                                    else if (aorcq.Answer != null || aorcq.Counterquestion != _tailCall.Value)
                                    {
                                        var results = aorcq.Answer ?? (DynamicSerializerState)(await aorcq.Counterquestion.WhenReturned);
                                        var ret = SetupReturn(results.MsgBuilder);

                                        switch (req.SendResultsTo.which)
                                        {
                                            case Call.sendResultsTo.WHICH.Caller:
                                                ret.which = Return.WHICH.Results;
                                                ret.Results.Content = results.Rewrap<DynamicSerializerState>();
                                                ret.ReleaseParamCaps = releaseParamCaps;
                                                ExportCapTableAndSend(results, ret.Results);
                                                break;

                                            case Call.sendResultsTo.WHICH.Yourself:
                                                ReturnCall(ret2 =>
                                                {
                                                    ret2.which = Return.WHICH.ResultsSentElsewhere;
                                                    ret2.ReleaseParamCaps = releaseParamCaps;
                                                });
                                                break;
                                        }                                        
                                    }
                                    else if (aorcq.Counterquestion != null)
                                    {
                                        _tailCall.Value = null;
                                        aorcq.Counterquestion.IsTailCall = true;
                                        aorcq.Counterquestion.Send();

                                        ReturnCall(ret2 =>
                                        {
                                            ret2.which = Return.WHICH.TakeFromOtherQuestion;
                                            ret2.TakeFromOtherQuestion = aorcq.Counterquestion.QuestionId;
                                            ret2.ReleaseParamCaps = releaseParamCaps;
                                        });
                                    }
                                }
                                catch (TaskCanceledException)
                                {
                                    ReturnCall(ret =>
                                    {
                                        ret.which = Return.WHICH.Canceled;
                                        ret.ReleaseParamCaps = releaseParamCaps;
                                    });
                                }
                                catch (System.Exception exception)
                                {
                                    ReturnCall(ret =>
                                    {
                                        ret.which = Return.WHICH.Exception;
                                        ret.Exception.Reason = exception.Message;
                                        ret.ReleaseParamCaps = releaseParamCaps;
                                    });
                                }
                            });
                            break;

                        case Call.sendResultsTo.WHICH.Yourself:
                            pendingAnswer.Chain(false, async t =>
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
                                    ReturnCall(ret =>
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

                    if (cap == null)
                    {
                        releaseParamCaps = true;
                        pendingAnswer = new PendingAnswer(
                            Task.FromException<AnswerOrCounterquestion>(
                                new RpcException("Call target resolved to null")), null);
                    }
                    else
                    {
                        try
                        {
                            var cts = new CancellationTokenSource();
                            var callTask = cap.Invoke(req.InterfaceId, req.MethodId, inParams, cts.Token);
                            pendingAnswer = new PendingAnswer(callTask, cts);
                        }
                        catch (System.Exception exception)
                        {
                            releaseParamCaps = true;
                            pendingAnswer = new PendingAnswer(
                                Task.FromException<AnswerOrCounterquestion>(exception), null);
                        }
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
                Impatient.AskingEndpoint = this;

                try
                {
                    switch (req.Target.which)
                    {
                        case MessageTarget.WHICH.ImportedCap:

                            lock (_reentrancyBlocker)
                            {
                                if (_exportTable.TryGetValue(req.Target.ImportedCap, out var rc))
                                {
                                    cap = rc.Cap;
                                }
                                else
                                {
                                    Logger.LogWarning("Incoming RPC call: Peer asked for invalid (already released?) capability ID.");

                                    SendAbort($"Requested capability with ID {req.Target.ImportedCap} does not exist.");
                                    return;
                                }
                            }

                            CreateAnswerAwaitItAndReply();

                            break;

                        case MessageTarget.WHICH.PromisedAnswer:
                            {
                                bool exists;
                                PendingAnswer previousAnswer;

                                lock (_reentrancyBlocker)
                                {
                                    exists = _answerTable.TryGetValue(req.Target.PromisedAnswer.QuestionId, out previousAnswer);
                                }

                                if (exists)
                                {
                                    previousAnswer.Chain(
                                        false,
                                        req.Target.PromisedAnswer,
                                        async t =>
                                        {
                                            try
                                            {
                                                using (var proxy = await t)
                                                {
                                                    cap = proxy?.GetProvider();
                                                    CreateAnswerAwaitItAndReply();
                                                }
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
                                    SendAbort($"Did not find a promised answer for given ID {req.Target.PromisedAnswer.QuestionId}");
                                    return;
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
                    Impatient.AskingEndpoint = null;
                    _tailCall.Value?.Send();
                    _tailCall.Value = null;
                }
            }

            void ProcessReturn(Return.READER req)
            {
                PendingQuestion question;

                lock (_reentrancyBlocker)
                {
                    if (!_questionTable.TryGetValue(req.AnswerId, out question))
                    {
                        Logger.LogWarning("Incoming RPC return: Unknown answer ID.");

                        return;
                    }
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
                            PendingAnswer pendingAnswer;

                            lock (_reentrancyBlocker)
                            {
                                exists = _answerTable.TryGetValue(req.TakeFromOtherQuestion, out pendingAnswer);
                            }

                            if (exists)
                            {
                                pendingAnswer.Chain(false, async t =>
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
                            }
                        }
                        break;

                    default:
                        throw new RpcUnimplementedException();
                }
            }

            void ProcessResolve(Resolve.READER resolve)
            {
                if (!_importTable.TryGetValue(resolve.PromiseId, out var rcw))
                {
                    Logger.LogWarning("Received a resolve message with invalid ID");
                    return;
                }

                if (!rcw.Cap.TryGetTarget(out var cap))
                {
                    // Silently discard
                    return;
                }

                if (!(cap is PromisedCapability resolvableCap))
                {
                    Logger.LogWarning("Received a resolve message for a capability which is not a promise");
                    return;
                }

                switch (resolve.which)
                {
                    case Resolve.WHICH.Cap:
                        lock (_reentrancyBlocker)
                        {
                            var resolvedCap = ImportCap(resolve.Cap);
                            resolvableCap.ResolveTo(resolvedCap);
                        }
                        break;

                    case Resolve.WHICH.Exception:
                        resolvableCap.Break(resolve.Exception.Reason);
                        break;

                    default:
                        Logger.LogWarning("Received a resolve message with unknown category.");
                        throw new RpcUnimplementedException();
                }
            }

            void ProcessSenderLoopback(Disembargo.READER disembargo)
            {
                var mb = MessageBuilder.Create();
                mb.InitCapTable();
                var wr = mb.BuildRoot<Message.WRITER>();
                wr.which = Message.WHICH.Disembargo;
                wr.Disembargo.Context.which = Disembargo.context.WHICH.ReceiverLoopback;
                wr.Disembargo.Context.ReceiverLoopback = disembargo.Context.SenderLoopback;
                var reply = wr.Disembargo;

                switch (disembargo.Target.which)
                {
                    case MessageTarget.WHICH.ImportedCap:

                        if (!_exportTable.TryGetValue(disembargo.Target.ImportedCap, out var cap))
                        {
                            Logger.LogWarning("Sender loopback request: Peer asked for invalid (already released?) capability ID.");

                            SendAbort("'Disembargo': Invalid capability ID");
                            Dismiss();

                            return;
                        }

                        reply.Target.which = MessageTarget.WHICH.ImportedCap;
                        reply.Target.ImportedCap = disembargo.Target.ImportedCap;

                        Tx(mb.Frame);

                        break;

                    case MessageTarget.WHICH.PromisedAnswer:

                        var promisedAnswer = disembargo.Target.PromisedAnswer;
                        reply.Target.which = MessageTarget.WHICH.PromisedAnswer;
                        var replyPromisedAnswer = reply.Target.PromisedAnswer;
                        replyPromisedAnswer.QuestionId = disembargo.Target.PromisedAnswer.QuestionId;
                        int count = promisedAnswer.Transform.Count;
                        replyPromisedAnswer.Transform.Init(count);

                        for (int i = 0; i < count; i++)
                        {
                            replyPromisedAnswer.Transform[i].which = promisedAnswer.Transform[i].which;
                            replyPromisedAnswer.Transform[i].GetPointerField = promisedAnswer.Transform[i].GetPointerField;
                        }

                        if (_answerTable.TryGetValue(promisedAnswer.QuestionId, out var previousAnswer))
                        {
                            previousAnswer.Chain(true,
                                disembargo.Target.PromisedAnswer,
                                async t =>
                                {
                                    try
                                    {
                                        using (var proxy = await t)
                                        {
                                            proxy.Freeze(out var boundEndpoint);

                                            try
                                            {
                                                if (boundEndpoint == this)
                                                {
#if DebugEmbargos
                                            Logger.LogDebug($"Sender loopback disembargo. Thread = {Thread.CurrentThread.Name}");
#endif
                                                    Tx(mb.Frame);
                                                }
                                                else
                                                {
                                                    Logger.LogWarning("Sender loopback request: Peer asked for disembargoing an answer which does not resolve back to the sender.");

                                                    SendAbort("'Disembargo': Answer does not resolve back to me");
                                                    Dismiss();
                                                }
                                            }
                                            finally
                                            {
                                                proxy.Unfreeze();
                                            }
                                        }
                                    }
                                    catch (System.Exception exception)
                                    {
                                        Logger.LogWarning($"Sender loopback request: Peer asked for disembargoing an answer which either has not yet returned, was canceled, or faulted: {exception.Message}");

                                        SendAbort($"'Disembargo' failure: {exception}");
                                        Dismiss();

                                    }
                                });
                        }
                        else
                        {
                            Logger.LogWarning("Sender loopback request: Peer asked for non-existing answer ID.");

                            SendAbort("'Disembargo': Invalid answer ID");
                            Dismiss();

                            return;
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

                    if (!tcs.TrySetResult(0))
                    {
                        Logger.LogError("Attempting to grant disembargo failed. Looks like an internal error / race condition.");
                    }
                }
                else
                {
                    Logger.LogWarning("Peer sent receiver loopback with unknown ID");
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

            void ReleaseResultCaps(PendingAnswer answer)
            {
                try
                {
                    answer.Chain(false, async t =>
                    {
                        var aorcq = await t;
                        var results = aorcq.Answer;

                        if (results != null && results.Caps != null)
                        {
                            foreach (var cap in results.Caps)
                            {
                                cap.Release();
                            }
                        }
                    });
                }
                catch
                {
                }
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
                            rc.Release(icount);
                            rc.Cap.Relinquish(icount);

                            if (rc.RefCount == 0)
                            {
                                _exportTable.Remove(id);
                                _revExportTable.ReplacementTryRemove(rc.Cap, out uint _);
                            }
                        }
                        catch (System.Exception exception)
                        {
                            Logger.LogWarning($"Attempting to release capability with invalid reference count: {exception.Message}");
                        }
                    }
                }

                if (!exists)
                {
                    Logger.LogWarning("Attempting to release unknown capability ID");
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
                            ReleaseExport(resolve.Cap.SenderPromise, 1);
                            break;

                        default:
                            break;
                    }
                }
            }

            void ProcessUnimplementedCall(Call.READER call)
            {
                Finish(call.QuestionId);
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
                        SendAbort("It's hopeless if you don't implement the bootstrap message");
                        Dismiss();
                        break;

                    default:
                        // Looking at the various message types it feels OK to just not care about other 'unimplemented'
                        // responses: You don't support Abort? Not my problem, I will drop the connection anyway. Don't
                        // support Disembargo? At least I tried. Don't support Finish/Release/Return? Why should I care?
                        // Don't support Unimplemented? Umm, well. Don't support Accept/Join/Provide? Interesting, I never
                        // send such messages, since I'm not a level 3 implementation.
                        break;
                }
            }

            public ConsumedCapability QueryMain()
            {
                var mb = MessageBuilder.Create();
                mb.InitCapTable();
                var req = mb.BuildRoot<Message.WRITER>();
                req.which = Message.WHICH.Bootstrap;
                var pendingBootstrap = AllocateQuestion(null, null);
                req.Bootstrap.QuestionId = pendingBootstrap.QuestionId;

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
                    Reserializing.DeepCopy(msg, reply.Unimplemented.Rewrap<DynamicSerializerState>());

                    Tx(mb.Frame);
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
                            if (_importTable.TryGetValue(capDesc.SenderHosted, out var rcw))
                            {
                                if (rcw.Cap.TryGetTarget(out var impCap))
                                {
                                    impCap.Validate();
                                    rcw.AddRef();
                                    return impCap;
                                }
                                else
                                {
                                    impCap = new ImportedCapability(this, capDesc.SenderHosted);
                                    rcw.Cap.SetTarget(impCap);
                                }

                                Debug.Assert(impCap != null);
                                return impCap;
                            }
                            else
                            {
                                var newCap = new ImportedCapability(this, capDesc.SenderHosted);
                                rcw = new RefCounted<WeakReference<RemoteCapability>>(
                                    new WeakReference<RemoteCapability>(newCap));
                                _importTable.Add(capDesc.SenderHosted, rcw);
                                Debug.Assert(newCap != null);
                                return newCap;
                            }

                        case CapDescriptor.WHICH.SenderPromise:
                            if (_importTable.TryGetValue(capDesc.SenderPromise, out var rcwp))
                            {
                                if (rcwp.Cap.TryGetTarget(out var impCap))
                                {
                                    impCap.Validate();
                                    rcwp.AddRef();
                                    return impCap;
                                }
                                else
                                {
                                    impCap = new PromisedCapability(this, capDesc.SenderPromise);
                                    rcwp.Cap.SetTarget(impCap);
                                }

                                Debug.Assert(impCap != null);
                                return impCap;
                            }
                            else
                            {
                                var newCap = new PromisedCapability(this, capDesc.SenderPromise);
                                rcw = new RefCounted<WeakReference<RemoteCapability>>(
                                    new WeakReference<RemoteCapability>(newCap));
                                _importTable.Add(capDesc.SenderPromise, rcw);
                                Debug.Assert(newCap != null);
                                return newCap;
                            }

                        case CapDescriptor.WHICH.ReceiverHosted:
                            if (_exportTable.TryGetValue(capDesc.ReceiverHosted, out var rc))
                            {
                                Debug.Assert(rc.Cap != null);
                                return LocalCapability.Create(rc.Cap);
                            }
                            else
                            {
                                Logger.LogWarning("Peer refers to receiver-hosted capability which does not exist.");
                                return null;
                            }

                        case CapDescriptor.WHICH.ReceiverAnswer:
                            if (_answerTable.TryGetValue(capDesc.ReceiverAnswer.QuestionId, out var pendingAnswer))
                            {
                                var tcs = new TaskCompletionSource<Proxy>();

                                pendingAnswer.Chain(false,
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
                                return null;
                            }

                        case CapDescriptor.WHICH.ThirdPartyHosted:
                            if (_importTable.TryGetValue(capDesc.ThirdPartyHosted.VineId, out var rcv))
                            {
                                if (rcv.Cap.TryGetTarget(out var impCap))
                                {
                                    rcv.AddRef();
                                    impCap.Validate();
                                    return impCap;
                                }
                                else
                                {
                                    impCap = new ImportedCapability(this, capDesc.ThirdPartyHosted.VineId);
                                    rcv.Cap.SetTarget(impCap);
                                }

                                Debug.Assert(impCap != null);
                                return impCap;
                            }
                            else
                            {
                                var newCap = new ImportedCapability(this, capDesc.ThirdPartyHosted.VineId);
                                rcv = new RefCounted<WeakReference<RemoteCapability>>(
                                    new WeakReference<RemoteCapability>(newCap));
                                Debug.Assert(newCap != null);
                                return newCap;
                            }

                        default:
                            Logger.LogWarning("Unknown capability descriptor category");
                            throw new RpcUnimplementedException();
                    }
                }
            }

            public IReadOnlyList<ConsumedCapability> ImportCapTable(Payload.READER payload)
            {
                var list = new List<ConsumedCapability>();

                if (payload.CapTable != null)
                {
                    lock (_reentrancyBlocker)
                    {
                        foreach (var capDesc in payload.CapTable)
                        {
                            list.Add(ImportCap(capDesc));
                        }
                    }
                }

                return list;
            }

            void IRpcEndpoint.RequestPostAction(Action postAction)
            {
                _exportCapTablePostActions.Value += postAction;
            }

            void ExportCapTableAndSend(
                SerializerState state,
                Payload.WRITER payload)
            {
                Debug.Assert(_exportCapTablePostActions.Value == null);
                _exportCapTablePostActions.Value = null;

                payload.CapTable.Init(state.MsgBuilder.Caps.Count);

                int i = 0;
                foreach (var cap in state.MsgBuilder.Caps)
                {
                    var capDesc = payload.CapTable[i++];

                    if (cap == null)
                    {
                        LazyCapability.Null.Export(this, capDesc);
                    }
                    else
                    {
                        cap.Export(this, capDesc);
                        cap.Release();
                    }
                }

                Tx(state.MsgBuilder.Frame);

                // The reason for this seemingly cumbersome postAction handling is as follows:
                // If a sender-promise capability happens to resolve concurrently, we must not
                // send the "resolve" message before even sending the sender-promise descriptor.
                // To avoid that situation, calls to "ReExportCapWhenResolved" are queued (and
                // therefore deferred) to the postAction.

                var pa = _exportCapTablePostActions.Value;
                _exportCapTablePostActions.Value = null;
                pa?.Invoke();
            }

            PendingQuestion IRpcEndpoint.BeginQuestion(ConsumedCapability target, SerializerState inParams)
            {
                var question = AllocateQuestion(target, inParams);

                if (_canDeferCalls.Value)
                {
                    _tailCall.Value?.Send();
                    _tailCall.Value = question;
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
                msg.Finish.QuestionId = questionId;
                msg.Finish.ReleaseResultCaps = false;

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
                    msg.Release.Id = importId;
                    msg.Release.ReferenceCount = (uint)count;

                    try
                    {
                        Tx(mb.Frame);
                    }
                    catch (RpcException exception)
                    {
                        Logger.LogWarning($"Unable to release import: {exception.InnerException.Message}");
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
                describe(msg.Disembargo.Target);
                var ctx = msg.Disembargo.Context;
                ctx.which = Disembargo.context.WHICH.SenderLoopback;
                ctx.SenderLoopback = id;

                Tx(mb.Frame);

                return tcs.Task;
            }

            void IRpcEndpoint.DeleteQuestion(PendingQuestion question)
            {
                DeleteQuestion(question.QuestionId, question);
            }
        }

        readonly ConcurrentBag<RpcEndpoint> _inboundEndpoints = new ConcurrentBag<RpcEndpoint>();

        internal RpcEndpoint AddEndpoint(IEndpoint outboundEndpoint)
        {
            var inboundEndpoint = new RpcEndpoint(this, outboundEndpoint);
            _inboundEndpoints.Add(inboundEndpoint);
            return inboundEndpoint;
        }

        Skeleton _bootstrapCap;

        /// <summary>
        /// Gets or sets the bootstrap capability.
        /// </summary>
        public Skeleton BootstrapCap
        {
            get => _bootstrapCap;
            set
            {
                value?.Claim();
                _bootstrapCap?.Relinquish();
                _bootstrapCap = value;
            }
        }
    }
}
