namespace Capnp.Rpc
{
    /// <summary>
    /// Thrown when an RPC-related error condition occurs.
    /// </summary>
    public class RpcException : System.Exception
    {
        /// <summary>
        /// Constructs an instance.
        /// </summary>
        public RpcException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructs an instance with message and inner exception.
        /// </summary>
        public RpcException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
