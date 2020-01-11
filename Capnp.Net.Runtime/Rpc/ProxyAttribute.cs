using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Capnp.Rpc
{
    /// <summary>
    /// Annotates a capability interface with its Proxy implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ProxyAttribute : Attribute
    {
        /// <summary>
        /// Constructs this attribute.
        /// </summary>
        /// <param name="proxyClass">Proxy type. This must be a class which inherits from <see cref="Proxy"/> and
        /// exposes a public parameterless constructor. Moreover, it must have same amount of generic type
        /// parameters like the annotated interface, with identical generic constraints.</param>
        /// <exception cref="ArgumentNullException"><paramref name="proxyClass"/> is null.</exception>
        public ProxyAttribute(Type proxyClass)
        {
            ProxyClass = proxyClass ?? throw new ArgumentNullException(nameof(proxyClass));
        }

        /// <summary>
        /// The Proxy type.
        /// </summary>
        public Type ProxyClass { get; }
    }
}
#nullable restore