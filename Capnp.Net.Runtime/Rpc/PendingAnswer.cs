using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class PendingAnswer: IDisposable
    {
        readonly CancellationTokenSource? _cts;
        readonly TaskCompletionSource<AnswerOrCounterquestion> _cancelCompleter;
        readonly Task<AnswerOrCounterquestion> _answerTask;

        public PendingAnswer(Task<AnswerOrCounterquestion> callTask, CancellationTokenSource? cts)
        {
            async Task<AnswerOrCounterquestion> CancelableAwaitWhenReady()
            {
                return await await Task.WhenAny(callTask, _cancelCompleter.Task);
            }

            if (callTask == null)
                throw new ArgumentNullException(nameof(callTask));

            _cts = cts;
            _cancelCompleter = new TaskCompletionSource<AnswerOrCounterquestion>();
            _answerTask = CancelableAwaitWhenReady();

            Chain(async t =>
            {
                var aorcq = default(AnswerOrCounterquestion);

                try
                {
                    aorcq = await t;
                }
                catch
                {
                }

                if (aorcq.Answer != null)
                {
                    if (aorcq.Answer.Caps != null)
                    {
                        foreach (var cap in aorcq.Answer.Caps)
                        {
                            cap?.AddRef();
                        }
                    }
                }
            });
        }

        public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

        public IReadOnlyList<CapDescriptor.WRITER>? CapTable { get; set; }

        public void Cancel()
        {
            _cts?.Cancel();
            _cancelCompleter.SetCanceled();
        }

        public void Chain(Action<Task<AnswerOrCounterquestion>> func)
        {
            func(_answerTask);
        }

        public void Chain(PromisedAnswer.READER rd, Action<Task<Proxy>> func)
        {
            Chain(t =>
            {
                async Task<Proxy> EvaluateProxy()
                {
                    var aorcq = await t;

                    if (aorcq.Answer != null)
                    {
                        DeserializerState cur = aorcq.Answer;

                        foreach (var op in rd.Transform)
                        {
                            switch (op.which)
                            {
                                case PromisedAnswer.Op.WHICH.GetPointerField:
                                    try
                                    {
                                        cur = cur.StructReadPointer(op.GetPointerField);
                                    }
                                    catch (System.Exception)
                                    {
                                        throw new ArgumentOutOfRangeException("Illegal pointer field in transformation operation");
                                    }
                                    break;

                                case PromisedAnswer.Op.WHICH.Noop:
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException("Unknown transformation operation");
                            }
                        }

                        Proxy proxy;

                        switch (cur.Kind)
                        {
                            case ObjectKind.Capability:
                                try
                                {
                                    var cap = aorcq.Answer.Caps![(int)cur.CapabilityIndex];
                                    proxy = new Proxy(cap);
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    throw new ArgumentOutOfRangeException("Bad capability table in internal answer - internal error?");
                                }
                                return proxy;

                            default:
                                throw new ArgumentOutOfRangeException("Transformation did not result in a capability");
                        }
                    }
                    else
                    {
                        var path = MemberAccessPath.Deserialize(rd);
                        var cap = new RemoteAnswerCapability(aorcq.Counterquestion!, path);
                        return new Proxy(cap);
                    }
                }

                func(EvaluateProxy());
            });
        }

        public void Dispose()
        {
            _cts?.Dispose();

            Chain(async t =>
            {
                AnswerOrCounterquestion aorcq;

                try
                {
                    aorcq = await t;
                }
                catch
                {
                    return;
                }

                if (aorcq.Answer != null)
                {
                    if (aorcq.Answer.Caps != null)
                    {
                        foreach (var cap in aorcq.Answer.Caps)
                        {
                            cap?.Release();
                        }
                    }
                }
            });
        }
    }
}