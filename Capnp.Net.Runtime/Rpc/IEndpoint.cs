namespace Capnp.Rpc
{
    /// <summary>
    /// A uni-directional endpoint, used in conjunction with the <see cref="RpcEngine"/>.
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// Transmit the given Cap'n Proto message over this endpoint.
        /// </summary>
        void Forward(WireFrame frame);

        void Flush();

        /// <summary>
        /// Close this endpoint.
        /// </summary>
        void Dismiss();
    }
}