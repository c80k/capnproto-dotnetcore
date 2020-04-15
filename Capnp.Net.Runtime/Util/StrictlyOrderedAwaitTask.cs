using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Util
{
    internal class StrictlyOrderedAwaitTask<T>: INotifyCompletion
    {
        class Cover { }
        class Seal { }

        static readonly Cover s_cover = new Cover();
        static readonly Seal s_seal = new Seal();

        readonly Task<T> _awaitedTask;
        object? _state;

        public StrictlyOrderedAwaitTask(Task<T> awaitedTask)
        {
            _awaitedTask = awaitedTask;
            _state = s_cover;
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
                        continuations = (Action?)Interlocked.Exchange(ref _state, null);
                        continuations?.Invoke();

                    } while (continuations != null);

                    return Interlocked.CompareExchange(ref _state, s_seal, null) == null;
                });
            }
        }

        public void OnCompleted(Action continuation)
        {
            bool first = false;

            SpinWait.SpinUntil(() => {
                object? cur, next;
                cur = Volatile.Read(ref _state);
                first = false;
                switch (cur)
                {
                    case Cover cover:
                        next = continuation;
                        first = true;
                        break;

                    case null:
                        next = continuation;
                        break;

                    case Action action:
                        next = action + continuation;
                        break;

                    default:
                        continuation();
                        return true;
                }

                return Interlocked.CompareExchange(ref _state, next, cur) == cur;
            });

            if (first)
            {
                AwaitInternal();
            }
        }

        public bool IsCompleted => _awaitedTask.IsCompleted && _state == s_seal;

        public T GetResult() => _awaitedTask.GetAwaiter().GetResult();

        public T Result => _awaitedTask.Result;

        public Task<T> WrappedTask => _awaitedTask;
    }

    internal static class StrictlyOrderedTaskExtensions
    {
        public static StrictlyOrderedAwaitTask<T> EnforceAwaitOrder<T>(this Task<T> task)
        {
            return new StrictlyOrderedAwaitTask<T>(task);
        }
    }
}
