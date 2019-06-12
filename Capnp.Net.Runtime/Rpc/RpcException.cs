namespace Capnp.Rpc
{
    /// <summary>
    /// Thrown when an RPC-related error condition occurs.
    /// </summary>
    public class RpcException : System.Exception
    {
        public RpcException(string message) : base(message)
        {
        }

        public RpcException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
