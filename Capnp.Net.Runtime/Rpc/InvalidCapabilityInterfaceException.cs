#nullable enable
namespace Capnp.Rpc
{
    /// <summary>
    /// Will be thrown if a type did not qualify as capability interface.
    /// In order to qualify the type must be properly annotated with a <see cref="ProxyAttribute"/> and <see cref="SkeletonAttribute"/>.
    /// See descriptions of these attributes for further details.
    /// </summary>
    public class InvalidCapabilityInterfaceException : System.Exception
    {
        /// <summary>
        /// Constructs an instance.
        /// </summary>
        public InvalidCapabilityInterfaceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructs an instance with message an inner exception.
        /// </summary>
        public InvalidCapabilityInterfaceException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
#nullable restore