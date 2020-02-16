using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    abstract class RefCountingCapability: ConsumedCapability
    {
        readonly object _reentrancyBlocker = new object();

        // Note on reference counting: Works in analogy to COM. AddRef() adds a reference,
        // Release() removes it. When the reference count reaches zero, the capability must be
        // released remotely, i.e. we need to tell the remote peer that it can remove this
        // capability from its export table. To be on the safe side, 
        // this class also implements a finalizer which will auto-release this capability
        // remotely. This might happen if one forgets to Dispose() a Proxy. It might also happen
        // if no Proxy is ever created. The latter situation occurs if the using client never
        // deserializes the capability Proxy from its RPC state. This situation is nearly
        // impossible to handle without relying on GC, since we never know when deserialization
        // happens, and there is no RAII like in C++. Since this situation is expected to happen rarely, 
        // it seems acceptable to rely on the finalizer. There are three possible states.
        // A: Initial state after construction: No reference, capability is *not* released.
        // B: Some positive reference count.
        // C: Released state: No reference anymore, capability *is* released.
        // In order to distinguish state A from C, the member _refCount stores the reference count *plus one*. 
        // Value 0 has the special meaning of being in state C.
        long _refCount = 1;

#if DebugCapabilityLifecycle
        ILogger Logger { get; } = Logging.CreateLogger<RefCountingCapability>();

        string _releasingMethodName;
        string _releasingFilePath;
        int _releasingLineNumber;
#endif

        ~RefCountingCapability()
        {
            Dispose(false);
        }

        /// <summary>
        /// Part of the Dispose pattern implementation.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    ReleaseRemotely();
                }
                catch
                {
                }
            }
            else
            {
                if (_refCount > 0)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            ReleaseRemotely();
                        }
                        catch
                        {
                        }
                    });
                }
            }
        }

        internal sealed override void AddRef()
        {
            lock (_reentrancyBlocker)
            {
                if (++_refCount <= 1)
                {
                    --_refCount;

#if DebugCapabilityLifecycle
                    Logger.LogError($"Attempted to add reference to capability which was already released. " +
                                    $"Releasing entity: {_releasingFilePath}, line {_releasingLineNumber}, method {_releasingMethodName}" +
                                    $"Current stack trace: {Environment.StackTrace}");
#endif
                    throw new ObjectDisposedException(ToString(), "Attempted to add reference to capability which was already released");
                }
            }
        }

        internal sealed override void Release(
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            lock (_reentrancyBlocker)
            {
                switch (_refCount)
                {
                    case 1: // initial state, actually ref. count 0
                    case 2: // actually ref. count 1
                        _refCount = 0;

#if DebugCapabilityLifecycle
                        _releasingMethodName = methodName;
                        _releasingFilePath = filePath;
                        _releasingLineNumber = lineNumber;
#endif

                        Dispose(true);
                        GC.SuppressFinalize(this);
                        break;

                    case var _ when _refCount > 2:
                        --_refCount;
                        break;

                    default:
                        throw new InvalidOperationException("Capability is already disposed");
                }
            }
        }

        internal void Validate()
        {
            lock (_reentrancyBlocker)
            {
                if (_refCount <= 0)
                {
                    throw new ObjectDisposedException(ToString(), "Validation failed, capability is already disposed");
                }
            }
        }
    }
}