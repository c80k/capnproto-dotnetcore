namespace Capnp.Rpc
{
    /// <summary>
    /// A mono skeleton (as opposed to <see cref="PolySkeleton"/>) is a skeleton which implements one particular RPC interface.
    /// </summary>
    public interface IMonoSkeleton: IProvidedCapability
    {
        /// <summary>
        /// Interface ID of this skeleton.
        /// </summary>
        ulong InterfaceId { get; }
    }
}
