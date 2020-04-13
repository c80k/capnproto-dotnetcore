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
        readonly Task<T> _awaitedTask;
        object? _lock;
        long _inOrder, _outOrder;

        public StrictlyOrderedAwaitTask(Task<T> awaitedTask)
        {
            _awaitedTask = awaitedTask;
            _lock = new object();
        }

        public StrictlyOrderedAwaitTask<T> GetAwaiter()
        {
            return this;
        }

        public async void OnCompleted(Action continuation)
        {
            object? safeLock = Volatile.Read(ref _lock);

            if (safeLock == null)
            {
                continuation();
                return;
            }

            long sequence = Interlocked.Increment(ref _inOrder) - 1;

            try
            {
                if (_awaitedTask.IsCompleted)
                {
                    Interlocked.Exchange(ref _lock, null);
                }

                await _awaitedTask;
            }
            catch
            {
            }
            finally
            {
                SpinWait.SpinUntil(() =>
                {
                    lock (safeLock)
                    {
                        if (Volatile.Read(ref _outOrder) != sequence)
                        {
                            return false;
                        }

                        Interlocked.Increment(ref _outOrder);

                        continuation();

                        return true;
                    }
                });
            }
        }

        public bool IsCompleted => Volatile.Read(ref _lock) == null || (_awaitedTask.IsCompleted && Volatile.Read(ref _inOrder) == 0);

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
