using System;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// Annotates an enum, class or interface with its schema type identifier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class |AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class TypeIdAttribute : Attribute
    {
        /// <summary>
        /// Constructs this attribute.
        /// </summary>
        /// <param name="typeId">The 64-bit type identifier from the schema file.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="typeId"/> is zero.</exception>
        public TypeIdAttribute(ulong typeId)
        {
            if (typeId == 0) throw new ArgumentOutOfRangeException(nameof(typeId), "The value cannot be zero.");
            Id = typeId;
        }

        /// <summary>
        /// The schema type identifier.
        /// </summary>
        public ulong Id { get; }
    }
}
#nullable restore