using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Capnp.Rpc
{

    /// <summary>
    /// Low-level interface of a capability at provider side.
    /// </summary>
    public interface IProvidedCapability
    {
        /// <summary>
        /// Calls an interface method of this capability.
        /// </summary>
        /// <param name="interfaceId">ID of interface to call</param>
        /// <param name="methodId">ID of method to call</param>
        /// <param name="args">Method arguments ("params struct")</param>
        /// <param name="cancellationToken">Cancellation token, indicating when the call should cancelled.</param>
        /// <returns>A Task which will resolve to the call result ("result struct")</returns>
        Task<AnswerOrCounterquestion> Invoke(ulong interfaceId, ushort methodId,
            DeserializerState args, CancellationToken cancellationToken = default);
    }
}
#nullable restore