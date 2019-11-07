using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class PromisedCapability : RemoteResolvingCapability
    {
        readonly uint _remoteId;
        readonly object _reentrancyBlocker = new object();
        readonly TaskCompletionSource<Proxy> _resolvedCap = new TaskCompletionSource<Proxy>();
        bool _released;

        public PromisedCapability(IRpcEndpoint ep, uint remoteId): base(ep)
        {
            _remoteId = remoteId;
        }

        public override Task<Proxy> WhenResolved => _resolvedCap.Task;

        internal override void Freeze(out IRpcEndpoint boundEndpoint)
        {
            lock (_reentrancyBlocker)
            {
                if (_resolvedCap.Task.IsCompleted && _pendingCallsOnPromise == 0)
                {
                    try
                    {
                        _resolvedCap.Task.Result.Freeze(out boundEndpoint);
                    }
                    catch (AggregateException exception)
                    {
                        throw exception.InnerException;
                    }
                }
                else
                {
                    Debug.Assert(!_released);
                    ++_pendingCallsOnPromise;

                    boundEndpoint = _ep;
                }
            }
        }

        internal override void Unfreeze()
        {
            bool release = false;

            lock (_reentrancyBlocker)
            {
                if (_pendingCallsOnPromise == 0)
                {
                    _resolvedCap.Task.Result.Unfreeze();
                }
                else
                {
                    Debug.Assert(_pendingCallsOnPromise > 0);
                    Debug.Assert(!_released);

                    if (--_pendingCallsOnPromise == 0 && _resolvedCap.Task.IsCompleted)
                    {
                        release = true;
                        _released = true;
                    }
                }
            }

            if (release)
            {
                _ep.ReleaseImport(_remoteId);
            }
        }

        internal override void Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            lock (_reentrancyBlocker)
            {
                
                if (_resolvedCap.Task.ReplacementTaskIsCompletedSuccessfully())
                {
                    _resolvedCap.Task.Result.Export(endpoint, writer);
                }
                else
                {
                    if (_ep == endpoint)
                    {
                        writer.which = CapDescriptor.WHICH.ReceiverHosted;
                        writer.ReceiverHosted = _remoteId;

                        Debug.Assert(!_released);
                        ++_pendingCallsOnPromise;

                        _ep.RequestPostAction(() =>
                        {
                            bool release = false;

                            lock (_reentrancyBlocker)
                            {
                                if (--_pendingCallsOnPromise == 0 && _resolvedCap.Task.IsCompleted)
                                {
                                    _released = true;
                                    release = true;
                                }
                            }

                            if (release)
                            {
                                _ep.ReleaseImport(_remoteId);
                            }
                        });
                    }
                    else
                    {
                        this.ExportAsSenderPromise(endpoint, writer);
                    }
                }
            }
        }

        async void TrackCall(Task call)
        {
            try
            {
                await call;
            }
            catch
            {
            }
            finally
            {
                bool release = false;

                lock (_reentrancyBlocker)
                {
                    if (--_pendingCallsOnPromise == 0 && _resolvedCap.Task.IsCompleted)
                    {
                        release = true;
                        _released = true;
                    }
                }

                if (release)
                {
                    _ep.ReleaseImport(_remoteId);
                }
            }
        }

        protected override Proxy ResolvedCap
        {
            get
            {
                try
                {
                    return _resolvedCap.Task.IsCompleted ? _resolvedCap.Task.Result : null;
                }
                catch (AggregateException exception)
                {
                    throw exception.InnerException;
                }
            }
        }

        protected override void GetMessageTarget(MessageTarget.WRITER wr)
        {
            wr.which = MessageTarget.WHICH.ImportedCap;
            wr.ImportedCap = _remoteId;
        }

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            lock (_reentrancyBlocker)
            {
                if (_resolvedCap.Task.IsCompleted)
                {
                    return CallOnResolution(interfaceId, methodId, args);
                }
                else
                {
                    Debug.Assert(!_released);
                    ++_pendingCallsOnPromise;
                }
            }

            var promisedAnswer = base.DoCall(interfaceId, methodId, args);
            TrackCall(promisedAnswer.WhenReturned);
            return promisedAnswer;
        }

        public void ResolveTo(ConsumedCapability resolvedCap)
        {
            bool release = false;

            lock (_reentrancyBlocker)
            {
                _resolvedCap.SetResult(new Proxy(resolvedCap));

                if (_pendingCallsOnPromise == 0)
                {
                    release = true;
                    _released = true;
                }
            }

            if (release)
            {
                _ep.ReleaseImport(_remoteId);
            }
        }

        public void Break(string message)
        {
            bool release = false;

            lock (_reentrancyBlocker)
            {
#if false
                _resolvedCap.SetException(new RpcException(message));
#else
                _resolvedCap.SetResult(new Proxy(LazyCapability.CreateBrokenCap(message)));
#endif

                if (_pendingCallsOnPromise == 0)
                {
                    release = true;
                    _released = true;
                }
            }

            if (release)
            {
                _ep.ReleaseImport(_remoteId);
            }
        }

        protected override void ReleaseRemotely()
        {
            if (!_released)
            {
                _ep.ReleaseImport(_remoteId);
            }

            _ep.ReleaseImport(_remoteId);

            this.DisposeWhenResolved();
        }

        protected override Call.WRITER SetupMessage(DynamicSerializerState args, ulong interfaceId, ushort methodId)
        {
            var call = base.SetupMessage(args, interfaceId, methodId);

            call.Target.which = MessageTarget.WHICH.ImportedCap;
            call.Target.ImportedCap = _remoteId;

            return call;
        }
    }
}
