using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Util
{
    public class StrictlyOrderedAwaitTask<T>: INotifyCompletion
    {
        static readonly Action Capsule = () => throw new InvalidProgramException("Not invocable");

        readonly Task<T> _awaitedTask;
        Action? _state;

        public StrictlyOrderedAwaitTask(Task<T> awaitedTask)
        {
            _awaitedTask = awaitedTask;
            AwaitInternal();
        }

        public StrictlyOrderedAwaitTask<T> GetAwaiter()
        {
            return this;
        }

        async void AwaitInternal()
        {
            try
            {
                await _awaitedTask;
            }
            catch
            {
            }
            finally
            {
                SpinWait.SpinUntil(() =>
                {
                    Action? continuations;
                    do
                    {
                        continuations = Interlocked.Exchange(ref _state, null);
                        continuations?.Invoke();

                    } while (continuations != null);

                    return Interlocked.CompareExchange(ref _state, Capsule, null) == null;
                });
            }
        }

        public void OnCompleted(Action continuation)
        {
            SpinWait.SpinUntil(() => {
                Action? cur, next;
                cur = Volatile.Read(ref _state);
                switch (cur)
                {
                    case null:
                        next = continuation;
                        break;

                    case Action capsule when capsule == Capsule:
                        continuation();
                        return true;

                    case Action action:
                        next = action + continuation;
                        break;
                }

                return Interlocked.CompareExchange(ref _state, next, cur) == cur;
            });
        }

        public bool IsCompleted => _awaitedTask.IsCompleted && _state == Capsule;

        public T GetResult() => _awaitedTask.GetAwaiter().GetResult();

        public T Result => _awaitedTask.Result;

        public Task<T> WrappedTask => _awaitedTask;
    }

    public static class StrictlyOrderedTaskExtensions
    {
        public static StrictlyOrderedAwaitTask<T> EnforceAwaitOrder<T>(this Task<T> task)
        {
            return new StrictlyOrderedAwaitTask<T>(task);
        }
    }
}
