using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// A promised capability.
    /// </summary>
    public interface IResolvingCapability
    {
        /// <summary>
        /// Will eventually give the resolved capability.
        /// </summary>
        Task<Proxy> WhenResolved { get; }
    }
}
