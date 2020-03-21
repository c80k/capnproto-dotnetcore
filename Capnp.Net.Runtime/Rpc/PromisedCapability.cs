using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    class PromisedCapability : RemoteResolvingCapability
    {
        readonly uint _remoteId;
        readonly object _reentrancyBlocker = new object();
        readonly TaskCompletionSource<ConsumedCapability?> _resolvedCap = new TaskCompletionSource<ConsumedCapability?>();
        readonly Task<Proxy> _whenResolvedProxy;
        bool _released;

        public PromisedCapability(IRpcEndpoint ep, uint remoteId): base(ep)
        {
            _remoteId = remoteId;

            async Task<Proxy> AwaitProxy() => new Proxy(await WhenResolved);
            _whenResolvedProxy = AwaitProxy();
        }

        public override Task<ConsumedCapability?> WhenResolved => _resolvedCap.Task;

        internal override void Freeze(out IRpcEndpoint? boundEndpoint)
        {
            lock (_reentrancyBlocker)
            {
                if (_resolvedCap.Task.IsCompleted && _pendingCallsOnPromise == 0)
                {
                    boundEndpoint = null;

                    try
                    {
                        _resolvedCap.Task.Result?.Freeze(out boundEndpoint);
                    }
                    catch (AggregateException exception)
                    {
                        throw exception.InnerException!;
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
                    _resolvedCap.Task.Result?.Unfreeze();
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

        internal override Action? Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            lock (_reentrancyBlocker)
            {
                
                if (_resolvedCap.Task.ReplacementTaskIsCompletedSuccessfully())
                {
                    using var proxy = new Proxy(_resolvedCap.Task.Result);
                    proxy.Export(endpoint, writer);
                }
                else
                {
                    if (_ep == endpoint)
                    {
                        writer.which = CapDescriptor.WHICH.ReceiverHosted;
                        writer.ReceiverHosted = _remoteId;

                        Debug.Assert(!_released);
                        ++_pendingCallsOnPromise;

                        return () =>
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
                        };
                    }
                    else
                    {
                        this.ExportAsSenderPromise(endpoint, writer);
                    }
                }
            }

            return null;
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

        protected override ConsumedCapability? ResolvedCap
        {
            get
            {
                try
                {
                    return _resolvedCap.Task.IsCompleted ? _resolvedCap.Task.Result : null;
                }
                catch (AggregateException exception)
                {
                    throw exception.InnerException!;
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
#if DebugFinalizers
                resolvedCap.ResolvingCap = this;
#endif
                _resolvedCap.SetResult(resolvedCap);

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
                _resolvedCap.SetResult(LazyCapability.CreateBrokenCap(message));
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

        protected async override void ReleaseRemotely()
        {
            if (!_released)
            {
                _released = true;
                _ep.ReleaseImport(_remoteId);
            }

            try { using var _ = await _whenResolvedProxy; }
            catch { }
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