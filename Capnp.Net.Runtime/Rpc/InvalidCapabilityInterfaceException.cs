namespace Capnp.Rpc
{
    /// <summary>
    /// Will be thrown if a type did not qualify as capability interface.
    /// In order to qualify the type must be properly annotated with a <see cref="ProxyAttribute"/> and <see cref="SkeletonAttribute"/>.
    /// See descriptions of these attributes for further details.
    /// </summary>
    public class InvalidCapabilityInterfaceException : System.Exception
    {
        public InvalidCapabilityInterfaceException(string message) : base(message)
        {
        }

        public InvalidCapabilityInterfaceException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
