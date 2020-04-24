using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// Null skeleton
    /// </summary>
    public sealed class NullSkeleton : Skeleton
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static readonly NullSkeleton Instance = new NullSkeleton();

        NullSkeleton()
        {
        }

        /// <summary>
        /// Always throws an exception
        /// </summary>
        /// <exception cref="InvalidOperationException">always thrown</exception>
        public override Task<AnswerOrCounterquestion> Invoke(ulong interfaceId, ushort methodId, DeserializerState args, CancellationToken cancellationToken = default)
        {
            args.Dispose();
            throw new InvalidOperationException("Cannot call null capability");
        }

        internal override ConsumedCapability AsCapability() => NullCapability.Instance;

        internal override void Claim()
        {
        }

        internal override void Relinquish()
        {
        }
    }
}