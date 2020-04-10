using System;

namespace Capnp.Rpc
{
    /// <summary>
    /// Null capability
    /// </summary>
    public sealed class NullCapability : ConsumedCapability
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static readonly NullCapability Instance = new NullCapability();

        NullCapability()
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        protected override void ReleaseRemotely()
        {
        }

        internal override void AddRef()
        {
        }

        internal override Skeleton AsSkeleton() => NullSkeleton.Instance;

        internal override IPromisedAnswer DoCall(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            args.Dispose();
            throw new InvalidOperationException("Cannot call null capability");
        }

        internal override Action? Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            writer.which = CapDescriptor.WHICH.None;
            return null;
        }

        internal override void Release()
        {
        }

        /// <summary>
        /// String hint
        /// </summary>
        /// <returns>"Null capability"</returns>
        public override string ToString() => "Null capability";
    }
}