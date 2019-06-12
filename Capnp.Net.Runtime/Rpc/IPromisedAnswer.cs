using System;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// A promised answer due to RPC.
    /// </summary>
    /// <remarks>
    /// Disposing the instance before the answer is available results in a best effort attempt to cancel
    /// the ongoing call.
    /// </remarks>
    public interface IPromisedAnswer: IDisposable
    {
        /// <summary>
        /// Task which will complete when the RPC returns, delivering its result struct.
        /// </summary>
        Task<DeserializerState> WhenReturned { get; }

        /// <summary>
        /// Creates a low-level capability for promise pipelining.
        /// </summary>
        /// <param name="access">Path to the desired capability inside the result struct.</param>
        /// <returns>Pipelined low-level capability</returns>
        ConsumedCapability Access(MemberAccessPath access);
    }
}
