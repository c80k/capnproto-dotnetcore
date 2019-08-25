using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// Provides support for promise pipelining.
    /// </summary>
    public static class Impatient
    {
        static readonly ConditionalWeakTable<Task, IPromisedAnswer> _taskTable = new ConditionalWeakTable<Task, IPromisedAnswer>();
        static readonly ThreadLocal<IRpcEndpoint> _askingEndpoint = new ThreadLocal<IRpcEndpoint>();

        /// <summary>
        /// Attaches a continuation to the given promise and registers the resulting task for pipelining.
        /// </summary>
        /// <typeparam name="T">Task result type</typeparam>
        /// <param name="promise">The promise</param>
        /// <param name="then">The continuation</param>
        /// <returns>Task representing the future answer</returns>
        /// <exception cref="ArgumentNullException"><paramref name="promise"/> or <paramref name="then"/> is null.</exception>
        /// <exception cref="ArgumentException">The pomise was already registered.</exception>
        public static Task<T> MakePipelineAware<T>(IPromisedAnswer promise, Func<DeserializerState, T> then)
        {
            async Task<T> AwaitAnswer()
            {
                return then(await promise.WhenReturned);
            }

            var rtask = AwaitAnswer();

            // Really weird: We'd expect AwaitAnswer() to initialize a new Task instance upon each invocation.
            // However, this does not seem to be always true (as indicated by CI test suite). An explanation might be
            // that the underlying implementation recycles Task instances (um, really? doesn't make sense. But the
            // obsevation doesn't make sense, either).

            try
            {
                _taskTable.Add(rtask, promise);
            }
            catch (ArgumentException)
            {
                // Force .NET to create a new Task instance

                async Task<T> AwaitAgain()
                {
                    return await rtask;
                }

                rtask = AwaitAgain();

                _taskTable.Add(rtask, promise);
            }

            return rtask;
        }

        /// <summary>
        /// Looks up the underlying promise which was previously registered for the given Task using MakePipelineAware.
        /// </summary>
        /// <param name="task"></param>
        /// <returns>The underlying promise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is null.</exception>
        /// <exception cref="ArgumentException">The task was not registered using MakePipelineAware.</exception>
        public static IPromisedAnswer GetAnswer(Task task)
        {
            if (!_taskTable.TryGetValue(task, out var answer))
            {
                throw new ArgumentException("Unknown task");
            }

            return answer;
        }

        internal static IPromisedAnswer TryGetAnswer(Task task)
        {
            _taskTable.TryGetValue(task, out var answer);
            return answer;
        }

        static async Task<Proxy> AwaitProxy<T>(Task<T> task) where T: class
        {
            var item = await task;

            switch (item)
            {
                case Proxy proxy:
                    return proxy;

                case null:
                    return null;
            }

            var skel = Skeleton.GetOrCreateSkeleton(item, false);
            var localCap = LocalCapability.Create(skel);
            return CapabilityReflection.CreateProxy<T>(localCap);
        }

        /// <summary>
        /// Returns a local "lazy" proxy for a given Task. 
        /// This is not real promise pipelining and will probably be removed.
        /// </summary>
        /// <typeparam name="TInterface">Capability interface type</typeparam>
        /// <param name="task">The task</param>
        /// <param name="memberName">debugging aid</param>
        /// <param name="sourceFilePath">debugging aid</param>
        /// <param name="sourceLineNumber">debugging aid</param>
        /// <returns>A proxy for the given task.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is null.</exception>
        /// <exception cref="InvalidCapabilityInterfaceException"><typeparamref name="TInterface"/> did not
        /// quality as capability interface.</exception>
        public static TInterface PseudoEager<TInterface>(this Task<TInterface> task,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
            where TInterface : class
        {
            var lazyCap = new LazyCapability(AwaitProxy(task));
            return CapabilityReflection.CreateProxy<TInterface>(lazyCap, memberName, sourceFilePath, sourceLineNumber) as TInterface;
        }

        internal static IRpcEndpoint AskingEndpoint
        {
            get => _askingEndpoint.Value;
            set { _askingEndpoint.Value = value; }
        }

        /// <summary>
        /// Checks whether a given task belongs to a pending RPC and requests a tail call if applicable.
        /// </summary>
        /// <typeparam name="T">Task result type</typeparam>
        /// <param name="task">Task to request</param>
        /// <param name="func">Converts the task's result to a SerializerState</param>
        /// <returns>Tail-call aware task</returns>
        public static async Task<AnswerOrCounterquestion> MaybeTailCall<T>(Task<T> task, Func<T, SerializerState> func)
        {
            if (TryGetAnswer(task) is PendingQuestion pendingQuestion &&
                pendingQuestion.RpcEndpoint == AskingEndpoint)
            {
                pendingQuestion.IsTailCall = true;
                return pendingQuestion;
            }
            else
            {
                return func(await task);
            }
        }

        /// <summary>
        /// Overload for tuple-typed tasks
        /// </summary>
        public static Task<AnswerOrCounterquestion> MaybeTailCall<T1, T2>(Task<(T1, T2)> task, Func<T1, T2, SerializerState> func)
        {
            return MaybeTailCall(task, (ValueTuple<T1, T2> t) => func(t.Item1, t.Item2));
        }

        /// <summary>
        /// Overload for tuple-typed tasks
        /// </summary>
        public static Task<AnswerOrCounterquestion> MaybeTailCall<T1, T2, T3>(Task<(T1, T2, T3)> task, Func<T1, T2, T3, SerializerState> func)
        {
            return MaybeTailCall(task, (ValueTuple<T1, T2, T3> t) => func(t.Item1, t.Item2, t.Item3));
        }

        /// <summary>
        /// Overload for tuple-typed tasks
        /// </summary>
        public static Task<AnswerOrCounterquestion> MaybeTailCall<T1, T2, T3, T4>(Task<(T1, T2, T3, T4)> task, Func<T1, T2, T3, T4, SerializerState> func)
        {
            return MaybeTailCall(task, (ValueTuple<T1, T2, T3, T4> t) => func(t.Item1, t.Item2, t.Item3, t.Item4));
        }

        /// <summary>
        /// Overload for tuple-typed tasks
        /// </summary>
        public static Task<AnswerOrCounterquestion> MaybeTailCall<T1, T2, T3, T4, T5>(Task<(T1, T2, T3, T4, T5)> task, Func<T1, T2, T3, T4, T5, SerializerState> func)
        {
            return MaybeTailCall(task, (ValueTuple<T1, T2, T3, T4, T5> t) => func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));
        }

        /// <summary>
        /// Overload for tuple-typed tasks
        /// </summary>
        public static Task<AnswerOrCounterquestion> MaybeTailCall<T1, T2, T3, T4, T5, T6>(Task<(T1, T2, T3, T4, T5, T6)> task, Func<T1, T2, T3, T4, T5, T6, SerializerState> func)
        {
            return MaybeTailCall(task, (ValueTuple<T1, T2, T3, T4, T5, T6> t) => func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6));
        }

        /// <summary>
        /// Overload for tuple-typed tasks
        /// </summary>
        public static Task<AnswerOrCounterquestion> MaybeTailCall<T1, T2, T3, T4, T5, T6, T7>(Task<(T1, T2, T3, T4, T5, T6, T7)> task, Func<T1, T2, T3, T4, T5, T6, T7, SerializerState> func)
        {
            return MaybeTailCall(task, (ValueTuple<T1, T2, T3, T4, T5, T6, T7> t) => func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7));
        }
    }
}
