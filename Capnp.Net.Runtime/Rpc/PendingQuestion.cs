using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
        [Flags]
        public enum State
        {
            None = 0,
            TailCall = 1,
            Sent = 2,
            Returned = 4,
            FinishRequested = 8,
            Disposed = 16,
            Finalized = 32
        }

        readonly TaskCompletionSource<DeserializerState> _tcs = new TaskCompletionSource<DeserializerState>();
        readonly uint _questionId;
        ConsumedCapability _target;
        SerializerState _inParams;
        int _inhibitFinishCounter;

        internal PendingQuestion(IRpcEndpoint ep, uint id, ConsumedCapability target, SerializerState inParams)
        {
            RpcEndpoint = ep ?? throw new ArgumentNullException(nameof(ep));
            _questionId = id;
            _target = target;
            _inParams = inParams;
            StateFlags = inParams == null ? State.Sent : State.None;

            if (inParams != null)
            {
                foreach (var cap in inParams.Caps)
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
                _tcs.TrySetResult(results);
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

        public ConsumedCapability Access(MemberAccessPath access)
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
                        throw exception.InnerException;
                    }
                }
                else
                {
                    return new RemoteAnswerCapability(this, access);
                }
            }
        }

        static void ReleaseCaps(ConsumedCapability target, SerializerState inParams)
        {
            if (inParams != null)
            {
                foreach (var cap in inParams.Caps)
                {
                    cap?.Release();
                }
            }

            if (target != null)
            {
                target.Release();
            }
        }

        internal void Send()
        {
            SerializerState inParams;
            ConsumedCapability target;

            lock (ReentrancyBlocker)
            {
                Debug.Assert(!StateFlags.HasFlag(State.Sent));

                inParams = _inParams;
                _inParams = null;
                target = _target;
                _target = null;
                StateFlags |= State.Sent;
            }

            var msg = (Message.WRITER)inParams.MsgBuilder.Root;
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

            ReleaseCaps(target, inParams);
        }

        #region IDisposable Support

        void Dispose(bool disposing)
        {
            SerializerState inParams;
            ConsumedCapability target;

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

                        AutoFinish();
                    }
                }
                else
                {
                    StateFlags |= State.Finalized;
                }
            }

            ReleaseCaps(target, inParams);
        }

        ~PendingQuestion()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
