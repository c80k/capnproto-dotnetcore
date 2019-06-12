using System;

namespace Capnp.Rpc
{
    /// <summary>
    /// Annotates a capability interface with its Skeleton implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class SkeletonAttribute: Attribute
    {
        /// <summary>
        /// Constructs this attribute.
        /// </summary>
        /// <param name="skeletonClass">Skeleton type. This must be a class which inherits from <see cref="Skeleton"/> and
        /// exposes a public parameterless constructor. Moreover, it must have same amount of generic type
        /// parameters like the annotated interface, with identical generic constraints.</param>
        /// <exception cref="ArgumentNullException"><paramref name="skeletonClass"/> is null.</exception>
        public SkeletonAttribute(Type skeletonClass)
        {
            if (skeletonClass == null)
                throw new ArgumentNullException(nameof(skeletonClass));

            if (!typeof(Skeleton).IsAssignableFrom(skeletonClass))
                throw new ArgumentException("Must inherit from Skeleton");

            SkeletonClass = skeletonClass;
        }

        /// <summary>
        /// Gets the skeleton type.
        /// </summary>
        public Type SkeletonClass { get; }
    }
}
