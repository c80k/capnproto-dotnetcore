using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Util
{
    /// <summary>
    /// A task-like object which enforces that all await operations from the same thread leave in the exact order they were issued.
    /// Note that an ordinary .NET Task does not fulfill this requirement if completed by a thread which is different from the
    /// awaiting thread.
    /// </summary>
    public class StrictlyOrderedAwaitTask: INotifyCompletion
    {
        class Cover { }
        class Seal { }

        static readonly Cover s_cover = new Cover();
        static readonly Seal s_seal = new Seal();

        readonly Task _awaitedTask;
        object? _state;

        /// <summary>
        /// Constructs an instance
        /// </summary>
        /// <param name="awaitedTask">Task on which the order shall be enforced</param>
        public StrictlyOrderedAwaitTask(Task awaitedTask)
        {
            _awaitedTask = awaitedTask;
            _state = s_cover;
        }

        /// <summary>
        /// await pattern implementation
        /// </summary>
        /// <returns>An object suitable for the await pattern</returns>
        public StrictlyOrderedAwaitTask GetAwaiter()
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

        /// <summary>
        /// Part of await pattern implementation. Do not use directly.
        /// </summary>
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

        /// <summary>
        /// Whether the underlying task did complete and it is safe to skip continuation registration.
        /// </summary>
        public bool IsCompleted => _awaitedTask.IsCompleted && (_state == s_cover || _state == s_seal);

        /// <summary>
        /// Part of await pattern implementation. Do not use directly.
        /// </summary>
        public void GetResult() => _awaitedTask.GetAwaiter().GetResult();

        /// <summary>
        /// Task on which the order shall be enforced.
        /// </summary>
        public Task WrappedTask => _awaitedTask;
    }

    /// <summary>
    /// A task-like object which enforces that all await operations from the same thread leave in the exact order they were issued.
    /// Note that an ordinary .NET Task does not fulfill this requirement if completed by a thread which is different from the
    /// awaiting thread.
    /// </summary>
    public class StrictlyOrderedAwaitTask<T> : StrictlyOrderedAwaitTask
    {

        /// <summary>
        /// Constructs an instance
        /// </summary>
        /// <param name="awaitedTask">Task on which the order shall be enforced</param>
        public StrictlyOrderedAwaitTask(Task<T> awaitedTask): base(awaitedTask)
        {
        }

        /// <summary>
        /// Task on which the order shall be enforced.
        /// </summary>
        public new Task<T> WrappedTask => (Task<T>)base.WrappedTask;

        /// <summary>
        /// await pattern implementation
        /// </summary>
        /// <returns>An object suitable for the await pattern</returns>
        public new StrictlyOrderedAwaitTask<T> GetAwaiter() => this;

        /// <summary>
        /// Part of await pattern implementation. Do not use directly.
        /// </summary>
        public new T GetResult() => WrappedTask.GetAwaiter().GetResult();

        /// <summary>
        /// Redirects to the wrapped Task's result.
        /// </summary>
        public T Result => WrappedTask.Result;

    }

    /// <summary>
    /// Extension methods to simplify the use of <see cref="StrictlyOrderedAwaitTask"/>
    /// </summary>
    public static class StrictlyOrderedTaskExtensions
    {
        /// <summary>
        /// Converts the task to a task-like object which enforces that all await operations from the same thread leave in the exact order they were issued.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the Task</typeparam>
        /// <param name="task">Task to wrap</param>
        /// <returns>awaitable object</returns>
        public static StrictlyOrderedAwaitTask<T> EnforceAwaitOrder<T>(this Task<T> task)
        {
            return new StrictlyOrderedAwaitTask<T>(task);
        }

        /// <summary>
        /// Converts the task to a task-like object which enforces that all await operations from the same thread leave in the exact order they were issued.
        /// </summary>
        /// <param name="task">Task to wrap</param>
        /// <returns>awaitable object</returns>
        public static StrictlyOrderedAwaitTask EnforceAwaitOrder(this Task task)
        {
            return new StrictlyOrderedAwaitTask(task);
        }
    }
}
