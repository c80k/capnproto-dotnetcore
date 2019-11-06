using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class PendingAnswer: IDisposable
    {
        readonly object _reentrancyBlocker = new object();
        readonly CancellationTokenSource _cts;
        readonly TaskCompletionSource<int> _whenCanceled;
        Task<AnswerOrCounterquestion> _callTask;
        Task _initialTask;
        Task _chainedTask;
        bool _disposed;

        public PendingAnswer(Task<AnswerOrCounterquestion> callTask, CancellationTokenSource cts)
        {
            _cts = cts;
            _callTask = callTask ?? throw new ArgumentNullException(nameof(callTask));
            _whenCanceled = new TaskCompletionSource<int>();
        }

        public void Cancel()
        {
            _cts?.Cancel();
            _whenCanceled.SetResult(0);
        }

        async Task InitialAwaitWhenReady()
        {
            var which = await Task.WhenAny(_callTask, _whenCanceled.Task);

            if (which != _callTask)
            {
                throw new TaskCanceledException();
            }
        }

        async Task AwaitChainedTask(Task chainedTask, Func<Task<AnswerOrCounterquestion>, Task> func)
        {
            try
            {
                await chainedTask;
            }
            catch (System.Exception exception)
            {
                await func(Task.FromException<AnswerOrCounterquestion>(exception));
                throw;
            }

            await func(_callTask);
        }

        static async Task AwaitSeq(Task task1, Task task2)
        {
            await task1;
            await task2;
        }

        public void Chain(bool strictSync, Func<Task<AnswerOrCounterquestion>, Task> func)
        {

            lock (_reentrancyBlocker)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(PendingAnswer));
                }

                if (_initialTask == null)
                {
                    _initialTask = InitialAwaitWhenReady();
                }

                Task followUpTask;

                if (strictSync)
                {
                    followUpTask = AwaitChainedTask(_chainedTask ?? _initialTask, func);
                }
                else
                {
                    followUpTask = AwaitChainedTask(_initialTask, func);
                }

                if (_chainedTask != null)
                {
                    _chainedTask = AwaitSeq(_chainedTask, followUpTask);
                }
                else
                {
                    _chainedTask = followUpTask;
                }
            }
        }

        public void Chain(bool strictSync, PromisedAnswer.READER rd, Func<Task<Proxy>, Task> func)
        {
            Chain(strictSync, async t =>
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
                                    var cap = aorcq.Answer.Caps[(int)cur.CapabilityIndex];
                                    proxy = new Proxy(cap ?? LazyCapability.Null);
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
                        var cap = new RemoteAnswerCapability(aorcq.Counterquestion, path);
                        return new Proxy(cap);
                    }
                }

                await func(EvaluateProxy());
            });
        }

        public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

        public async void Dispose()
        {
            if (_cts != null)
            {
                Task chainedTask;

                lock (_reentrancyBlocker)
                {
                    if (_disposed)
                    {
                        return;
                    }
                    chainedTask = _chainedTask;
                    _disposed = true;
                }

                if (chainedTask != null)
                {
                    try
                    {
                        await chainedTask;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _cts.Dispose();
                    }
                }
            }
        }
    }
}
