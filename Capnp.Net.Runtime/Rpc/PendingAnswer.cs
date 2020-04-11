using Capnp.Util;
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
        readonly StrictlyOrderedAwaitTask<AnswerOrCounterquestion> _answerTask;

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
            _answerTask = CancelableAwaitWhenReady().EnforceAwaitOrder();

            TakeCapTableOwnership();
        }

        async void TakeCapTableOwnership()
        {
            try
            {
                var aorcq = await _answerTask;

                if (aorcq.Answer != null)
                {
                    if (aorcq.Answer.Caps != null)
                    {
                        foreach (var cap in aorcq.Answer.Caps)
                        {
                            cap.AddRef();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        async void ReleaseCapTableOwnership()
        {
            try
            {
                var aorcq = await _answerTask;
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
            }
            catch
            {
            }
        }

        public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

        public IReadOnlyList<CapDescriptor.WRITER>? CapTable { get; set; }

        public void Cancel()
        {
            _cts?.Cancel();
            _cancelCompleter.SetCanceled();
        }

        public void Chain(Action<StrictlyOrderedAwaitTask<AnswerOrCounterquestion>> func)
        {
            func(_answerTask);
        }

        public void Chain(PromisedAnswer.READER rd, Action<Task<Proxy>> func)
        {
            async Task<Proxy> EvaluateProxy()
            {
                var aorcq = await _answerTask;

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
                                    throw new RpcException("Illegal pointer field in transformation operation");
                                }
                                break;

                            case PromisedAnswer.Op.WHICH.Noop:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException("Unknown transformation operation");
                        }
                    }

                    switch (cur.Kind)
                    {
                        case ObjectKind.Capability:
                            try
                            {
                                return new Proxy(aorcq.Answer.Caps![(int)cur.CapabilityIndex]);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                throw new RpcException("Capability index out of range");
                            }

                        case ObjectKind.Nil:
                            return new Proxy(NullCapability.Instance);

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
        }

        public void Dispose()
        {
            _cts?.Dispose();
            ReleaseCapTableOwnership();
        }
    }
}