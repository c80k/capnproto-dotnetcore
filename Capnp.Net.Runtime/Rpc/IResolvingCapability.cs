using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// A promised capability.
    /// </summary>
    public interface IResolvingCapability
    {
        /// <summary>
        /// Completes when the capability gets resolved.
        /// </summary>
        Task WhenResolved { get; }

        /// <summary>
        /// Returns the resolved capability
        /// </summary>
        /// <typeparam name="T">Capability interface or <see cref="BareProxy"/></typeparam>
        /// <returns>the resolved capability, or null if it did not resolve yet</returns>
        T? GetResolvedCapability<T>() where T: class;
    }
}