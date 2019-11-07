using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// A skeleton is a wrapper around a capability interface implementation which adapts it in the way it is
    /// expected by the <see cref="RpcEngine"/>.
    /// </summary>
    public abstract class Skeleton: IProvidedCapability
    {
        class SkeletonRelinquisher: IDisposable
        {
            readonly Skeleton _skeleton;

            public SkeletonRelinquisher(Skeleton skeleton)
            {
                _skeleton = skeleton;
            }

            public void Dispose()
            {
                _skeleton.Relinquish();
            }
        }

#if DEBUG_DISPOSE
        const int NoDisposeFlag = 0x4000000;
#endif

        static readonly ConditionalWeakTable<object, Skeleton> _implMap =
            new ConditionalWeakTable<object, Skeleton>();

        internal static Skeleton GetOrCreateSkeleton<T>(T impl, bool addRef)
            where T: class
        {
            if (impl == null)
                throw new ArgumentNullException(nameof(impl));

            if (impl is Skeleton skel)
                return skel;

            skel = _implMap.GetValue(impl, _ => CapabilityReflection.CreateSkeleton(_));

            if (addRef)
            {
                skel.Claim();
            }

            return skel;
        }

        /// <summary>
        /// Claims ownership on the given capability, preventing its automatic disposal.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <param name="impl">Capability implementation</param>
        /// <returns>A disposable object. Calling Dispose() on the returned instance relinquishes ownership again.</returns>
        public static IDisposable Claim<T>(T impl) where T: class
        {
            return new SkeletonRelinquisher(GetOrCreateSkeleton(impl, true));
        }

#if DEBUG_DISPOSE
        /// <summary>
        /// This DEBUG-only diagnostic method states that the Skeleton corresponding to a given capability is not expected to
        /// be disposed until the next call to EndAssertNotDisposed().
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <param name="impl">Capability implementation</param>
        public static void BeginAssertNotDisposed<T>(T impl) where T : class
        {
            GetOrCreateSkeleton(impl, false).BeginAssertNotDisposed();
        }

        /// <summary>
        /// This DEBUG-only diagnostic method ends a non-disposal period started with BeginAssertNotDisposed.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <param name="impl">Capability implementation</param>
        public static void EndAssertNotDisposed<T>(T impl) where T : class
        {
            GetOrCreateSkeleton(impl, false).EndAssertNotDisposed();
        }
#endif

        int _refCount = 0;

        /// <summary>
        /// Calls an interface method of this capability.
        /// </summary>
        /// <param name="interfaceId">ID of interface to call</param>
        /// <param name="methodId">ID of method to call</param>
        /// <param name="args">Method arguments ("params struct")</param>
        /// <param name="cancellationToken">Cancellation token, indicating when the call should cancelled.</param>
        /// <returns>A Task which will resolve to the call result</returns>
        public abstract Task<AnswerOrCounterquestion> Invoke(ulong interfaceId, ushort methodId, DeserializerState args, CancellationToken cancellationToken = default);

        internal void Claim()
        {
            Interlocked.Increment(ref _refCount);
        }

#if DEBUG_DISPOSE
        internal void BeginAssertNotDisposed()
        {
            if ((Interlocked.Add(ref _refCount, NoDisposeFlag) & NoDisposeFlag) == 0)
            {
                throw new InvalidOperationException("Flag already set. State is now broken.");
            }
        }
        internal void EndAssertNotDisposed()
        {
            if ((Interlocked.Add(ref _refCount, -NoDisposeFlag) & NoDisposeFlag) != 0)
            {
                throw new InvalidOperationException("Flag already cleared. State is now broken.");
            }
        }
#endif

        internal void Relinquish()
        {
            int count = Interlocked.Decrement(ref _refCount);

            if (0 == count)
            {
#if DEBUG_DISPOSE
                if ((count & NoDisposeFlag) != 0)
                    throw new InvalidOperationException("Unexpected Skeleton disposal");
#endif

                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        internal void Relinquish(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            while (count-- > 0)
                Relinquish();
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~Skeleton()
        {
            Dispose(false);
        }

        internal virtual void Bind(object impl)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Skeleton for a specific capability interface.
    /// </summary>
    /// <typeparam name="T">Capability interface</typeparam>
    public abstract class Skeleton<T> : Skeleton, IMonoSkeleton
    {
#if DebugEmbargos
        ILogger Logger { get; } = Logging.CreateLogger<Skeleton<T>>();
#endif

        Func<DeserializerState, CancellationToken, Task<AnswerOrCounterquestion>>[] _methods;
        CancellationTokenSource _disposed = new CancellationTokenSource();
        readonly object _reentrancyBlocker = new object();
        int _pendingCalls;

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        public Skeleton()
        {
        }

        /// <summary>
        /// Populates this skeleton's method table. The method table maps method IDs (which are consecutively numbered from 0 
        /// onwards) to the underlying capability's method implementations.
        /// </summary>
        /// <param name="methods">The method table. Index is method ID.</param>
        protected void SetMethodTable(params Func<DeserializerState, CancellationToken, Task<AnswerOrCounterquestion>>[] methods)
        {
            _methods = methods;
        }

        /// <summary>
        /// Gets the underlying capability implementation.
        /// </summary>
        protected T Impl { get; private set; }

        /// <summary>
        /// Gets the ID of the implemented interface.
        /// </summary>
        public abstract ulong InterfaceId { get; }

        /// <summary>
        /// Calls an interface method of this capability.
        /// </summary>
        /// <param name="interfaceId">ID of interface to call</param>
        /// <param name="methodId">ID of method to call</param>
        /// <param name="args">Method arguments ("params struct")</param>
        /// <param name="cancellationToken">Cancellation token, indicating when the call should cancelled.</param>
        /// <returns>A Task which will resolve to the call result</returns>
        /// <exception cref="ObjectDisposedException">This Skeleton was disposed</exception>
        public override async Task<AnswerOrCounterquestion> Invoke(ulong interfaceId, ushort methodId, DeserializerState args, CancellationToken cancellationToken = default)
        {
            if (InterfaceId != InterfaceId)
                throw new NotImplementedException("Wrong interface id");

            if (methodId >= _methods.Length)
                throw new NotImplementedException("Wrong method id");

            lock (_reentrancyBlocker)
            {
                if (_disposed == null || _disposed.IsCancellationRequested)
                {
                    throw new ObjectDisposedException(nameof(Skeleton<T>));
                }

                ++_pendingCalls;
            }

            var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(_disposed.Token, cancellationToken);

            try
            {
                return await _methods[methodId](args, linkedSource.Token);
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                lock (_reentrancyBlocker)
                {
                    --_pendingCalls;
                }

                linkedSource.Dispose();
                CheckCtsDisposal();
            }
        }

        void CheckCtsDisposal()
        {
            if (_pendingCalls == 0 && _disposed != null && _disposed.IsCancellationRequested)
            {
                _disposed.Dispose();
                _disposed = null;
            }
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            lock (_reentrancyBlocker)
            {
                if (_disposed == null || _disposed.IsCancellationRequested)
                    return;

                _disposed.Cancel();

                CheckCtsDisposal();
            }

            if (Impl is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        internal override void Bind(object impl)
        {
            if (Impl != null)
                throw new InvalidOperationException("Skeleton was already bound");

            Impl = (T)impl;
        }
    }
}
