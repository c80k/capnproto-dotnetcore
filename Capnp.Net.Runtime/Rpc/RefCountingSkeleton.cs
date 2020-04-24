using System;
using System.Threading;

namespace Capnp.Rpc
{
    /// <summary>
    /// Skeleton with reference counting and dispose pattern
    /// </summary>
    public abstract class RefCountingSkeleton: Skeleton
    {
        int _refCount;
        LocalCapability? _localCap;

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~RefCountingSkeleton()
        {
            Dispose(false);
        }

        internal sealed override void Claim()
        {
            int count, newCount;

            do
            {
                count = Volatile.Read(ref _refCount);
                if (count < 0)
                    throw new ObjectDisposedException(nameof(RefCountingSkeleton));

                newCount = count + 1;

            } while (Interlocked.CompareExchange(ref _refCount, newCount, count) != count);
        }

        internal override void Relinquish()
        {
            int count, newCount;

            do
            {
                count = Volatile.Read(ref _refCount);
                if (count < 0)
                    throw new ObjectDisposedException(nameof(RefCountingSkeleton));

                newCount = count > 0 ? count - 1 : int.MinValue;

            } while (Interlocked.CompareExchange(ref _refCount, newCount, count) != count);

            if (newCount == 0)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Whether this instance is in disposed state.
        /// </summary>
        public bool IsDisposed => Volatile.Read(ref _refCount) < 0;

        internal override ConsumedCapability AsCapability()
        {
            var cap = Volatile.Read(ref _localCap);
            if (cap == null)
            {
                Interlocked.CompareExchange(ref _localCap, new LocalCapability(this), null);
            }
            return Volatile.Read(ref _localCap)!;
        }
    }
}