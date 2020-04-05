namespace Capnp.Rpc
{
    public class TailCallNoDataException: System.Exception
    {
        public TailCallNoDataException(): base("Because the question was asked as tail call, it won't return data")
        {
        }
    }
}