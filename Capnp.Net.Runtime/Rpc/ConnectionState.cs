namespace Capnp.Rpc
{
    /// <summary>
    /// State of an RPC connection
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// Connection is being initialized. This is a transient state. For TcpRpcServer it is active during
        /// the OnConnectionChanged event callback. For TcpRpcClient it is active before the connection is established.
        /// </summary>
        Initializing,

        /// <summary>
        /// Connection is active.
        /// </summary>
        Active,

        /// <summary>
        /// Connection is down. It will never be active again (re-connecting means to establish a new connection).
        /// </summary>
        Down
    }
}