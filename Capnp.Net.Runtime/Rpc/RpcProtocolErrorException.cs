namespace Capnp.Rpc
{
    class RpcProtocolErrorException : System.Exception
    {
        public RpcProtocolErrorException(string reason): base(reason)
        {
        }
    }
}