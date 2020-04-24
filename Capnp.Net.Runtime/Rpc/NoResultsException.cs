namespace Capnp.Rpc
{
    /// <summary>
    /// Thrown when a pending question did return, but was not configured to deliver the result back to the sender
    /// (typcial for tail calls).
    /// </summary>
    public class NoResultsException: System.Exception
    {
        /// <summary>
        /// Creates an instance
        /// </summary>
        public NoResultsException(): base("Pending question did return, but was not configured to deliver the result back to the sender")
        {
        }
    }
}