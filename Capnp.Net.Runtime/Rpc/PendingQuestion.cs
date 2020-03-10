using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// A promised answer due to RPC.
    /// </summary>
    /// <remarks>
    /// Disposing the instance before the answer is available results in a best effort attempt to cancel
    /// the ongoing call.
    /// </remarks>
    public sealed class PendingQuestion: IPromisedAnswer
    {
        /// <summary>
        /// Question lifetime management state
        /// </summary>
        [Flags]
        public enum State
        {
            /// <summary>
            /// The question has not yet been sent.
            /// </summary>
            None = 0,

            /// <summary>
            /// Tail call flag
            /// </summary>
            TailCall = 1,

            /// <summary>
            /// The question has been sent.
            /// </summary>
            Sent = 2,

            /// <summary>
            /// The question has been answered.
            /// </summary>
            Returned = 4,

            /// <summary>
            /// A 'finish' request was sent to the peer, indicating that no further requests will refer
            /// to this question.
            /// </summary>
            FinishRequested = 8,

            /// <summary>
            /// Question object was disposed.
            /// </summary>
            Disposed = 16,

            /// <summary>
            /// Question object was finalized by GC. 
            /// This flag should only be observable when debugging the finalizer itself.
            /// </summary>
            Finalized = 32
        }

        readonly TaskCompletionSource<DeserializerState> _tcs = new TaskCompletionSource<DeserializerState>();
        readonly uint _questionId;
        ConsumedCapability? _target;
        SerializerState? _inParams;
        int _inhibitFinishCounter;

        internal PendingQuestion(IRpcEndpoint ep, uint id, ConsumedCapability? target, SerializerState? inParams)
        {
            RpcEndpoint = ep ?? throw new ArgumentNullException(nameof(ep));
            _questionId = id;
            _target = target;
            _inParams = inParams;
            StateFlags = inParams == null ? State.Sent : State.None;

            if (inParams != null)
            {
                foreach (var cap in inParams.Caps!)
                {
                    cap?.AddRef();
                }
            }

            if (target != null)
            {
                target.AddRef();
            }
        }

        internal IRpcEndpoint RpcEndpoint { get; }
        internal object ReentrancyBlocker { get; } = new object();
        internal uint QuestionId => _questionId;
        internal State StateFlags { get; private set; }

        /// <summary>
        /// Eventually returns the server answer
        /// </summary>
        public Task<DeserializerState> WhenReturned => _tcs.Task;

        internal bool IsTailCall
        {
            get => StateFlags.HasFlag(State.TailCall);
            set
            {
                if (value)
                    StateFlags |= State.TailCall;
                else
                    StateFlags &= ~State.TailCall;
            }
        }
        internal bool IsReturned => StateFlags.HasFlag(State.Returned);

        internal void DisallowFinish()
        {
            ++_inhibitFinishCounter;
        }

        internal void AllowFinish()
        {
            --_inhibitFinishCounter;
            AutoFinish();
        }

        const string ReturnDespiteTailCallMessage = "Peer sent actual results despite the question was sent as tail call. This was not expected and is a protocol error.";

        internal void OnReturn(DeserializerState results)
        {
            lock (ReentrancyBlocker)
            {
                SetReturned();
            }

            if (StateFlags.HasFlag(State.TailCall))
            {
                _tcs.TrySetException(new RpcException(ReturnDespiteTailCallMessage));
            }
            else
            {
                if (!_tcs.TrySetResult(results))
                {
                    ReleaseOutCaps(results);
                }
            }
        }

        internal void OnTailCallReturn()
        {
            lock (ReentrancyBlocker)
            {
                SetReturned();
            }

            if (!StateFlags.HasFlag(State.TailCall))
            {
                _tcs.TrySetException(new RpcException("Peer sent the results of this questions somewhere else. This was not expected and is a protocol error."));
            }
            else
            {
                _tcs.TrySetResult(default);
            }
        }

        internal void OnException(Exception.READER exception)
        {
            lock (ReentrancyBlocker)
            {
                SetReturned();
            }

            _tcs.TrySetException(new RpcException(exception.Reason));
        }

        internal void OnException(System.Exception exception)
        {
            lock (ReentrancyBlocker)
            {
                SetReturned();
            }

            _tcs.TrySetException(exception);
        }

        internal void OnCanceled()
        {
            lock (ReentrancyBlocker)
            {
                SetReturned();
            }

            _tcs.TrySetCanceled();
        }

        void DeleteMyQuestion()
        {
            RpcEndpoint.DeleteQuestion(this);
        }

        internal void RequestFinish()
        {
            RpcEndpoint.Finish(_questionId);
        }

        void AutoFinish()
        {
            if (StateFlags.HasFlag(State.FinishRequested))
            {
                return;
            }

            if ((_inhibitFinishCounter == 0 && StateFlags.HasFlag(State.Returned) && !StateFlags.HasFlag(State.TailCall)) 
                || StateFlags.HasFlag(State.Disposed))
            {
                StateFlags |= State.FinishRequested;

                RequestFinish();
            }
        }

        void SetReturned()
        {
            if (StateFlags.HasFlag(State.Returned))
            {
                throw new InvalidOperationException("Return state already set");
            }

            StateFlags |= State.Returned;

            AutoFinish();
            DeleteMyQuestion();
        }

        /// <summary>
        /// Refer to a (possibly nested) member of this question's (possibly future) result and return
        /// it as a capability.
        /// </summary>
        /// <param name="access">Access path</param>
        /// <returns>Low-level capability</returns>
        /// <exception cref="DeserializationException">The referenced member does not exist or does not resolve to a capability pointer.</exception>
        public ConsumedCapability? Access(MemberAccessPath access)
        {
            lock (ReentrancyBlocker)
            {
                if ( StateFlags.HasFlag(State.Returned) && 
                    !StateFlags.HasFlag(State.TailCall))
                {
                    try
                    {
                        return access.Eval(WhenReturned.Result);
                    }
                    catch (AggregateException exception)
                    {
                        throw exception.InnerException!;
                    }
                }
                else
                {
                    return new RemoteAnswerCapabilityDeprecated(this, access);
                }
            }
        }

        /// <summary>
        /// Refer to a (possibly nested) member of this question's (possibly future) result and return
        /// it as a capability.
        /// </summary>
        /// <param name="task">promises the cap whose ownership is transferred to this object</param>
        /// <returns>Low-level capability</returns>
        /// <exception cref="DeserializationException">The referenced member does not exist or does not resolve to a capability pointer.</exception>
        public ConsumedCapability? Access(MemberAccessPath access, Task<IDisposable> task)
        {
            var proxyTask = task.AsProxyTask();

            lock (ReentrancyBlocker)
            {
                if (proxyTask.IsCompleted && !StateFlags.HasFlag(State.TailCall))
                {
                    try
                    {
                        using var proxy = proxyTask.Result;
                        return proxy.ConsumedCap;
                    }
                    catch (AggregateException exception)
                    {
                        throw exception.InnerException!;
                    }
                }
                else
                {
                    return new RemoteAnswerCapabilityDeprecated(this, access);
                }
            }
        }

        static void ReleaseCaps(ConsumedCapability? target, SerializerState? inParams)
        {
            if (inParams != null)
            {
                foreach (var cap in inParams.Caps!)
                {
                    cap?.Release(false);
                }
            }

            if (target != null)
            {
                target.Release(false);
            }
        }

        static void ReleaseOutCaps(DeserializerState outParams)
        {
            foreach (var cap in outParams.Caps!)
            {
                cap?.Release(false);
            }
        }

        internal void Send()
        {
            SerializerState? inParams;
            ConsumedCapability? target;

            lock (ReentrancyBlocker)
            {
                if (StateFlags.HasFlag(State.Sent))
                    throw new InvalidOperationException("Already sent");

                inParams = _inParams;
                _inParams = null;
                target = _target;
                _target = null;
                StateFlags |= State.Sent;
            }

            var msg = (Message.WRITER)inParams!.MsgBuilder!.Root!;
            Debug.Assert(msg.Call.Target.which != MessageTarget.WHICH.undefined);
            var call = msg.Call;
            call.QuestionId = QuestionId;
            call.SendResultsTo.which = IsTailCall ? 
                Call.sendResultsTo.WHICH.Yourself : 
                Call.sendResultsTo.WHICH.Caller;

            try
            {
                RpcEndpoint.SendQuestion(inParams, call.Params);
            }
            catch (System.Exception exception)
            {
                OnException(exception);
            }

            ReleaseCaps(target!, inParams);
        }

        #region IDisposable Support

        void Dispose(bool disposing)
        {
            SerializerState? inParams;
            ConsumedCapability? target;
            bool justDisposed = false;

            lock (ReentrancyBlocker)
            {
                inParams = _inParams;
                _inParams = null;
                target = _target;
                _target = null;

                if (disposing)
                {
                    if (!StateFlags.HasFlag(State.Disposed))
                    {
                        StateFlags |= State.Disposed;
                        justDisposed = true;

                        AutoFinish();
                    }
                }
                else
                {
                    StateFlags |= State.Finalized;
                }
            }

            ReleaseCaps(target, inParams);

            if (justDisposed)
            {
                _tcs.TrySetCanceled();
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~PendingQuestion()
        {
            Dispose(false);
        }

        /// <summary>
        /// Implements <see cref="IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}