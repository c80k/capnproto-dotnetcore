namespace Capnp.Rpc
{
    /// <summary>
    /// Generic Proxy implementation which exposes the (usually protected) Call method.
    /// </summary>
    public class BareProxy: Proxy
    {
        /// <summary>
        /// Wraps a capability implementation in a Proxy.
        /// </summary>
        /// <param name="impl">Capability implementation</param>
        /// <returns>Proxy</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="impl"/> is null.</exception>
        /// <exception cref="InvalidCapabilityInterfaceException">No <see cref="SkeletonAttribute"/> found on implemented interface(s).</exception>
        /// <exception cref="System.InvalidOperationException">Mismatch between generic type arguments (if capability interface is generic).</exception>
        /// <exception cref="System.ArgumentException">Mismatch between generic type arguments (if capability interface is generic).</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">Problem with instatiating the Skeleton (constructor threw exception).</exception>
        /// <exception cref="System.MemberAccessException">Caller does not have permission to invoke the Skeleton constructor.</exception>
        /// <exception cref="System.TypeLoadException">Problem with building the Skeleton type, or problem with loading some dependent class.</exception>
        public static BareProxy FromImpl(object impl)
        {
            return new BareProxy(LocalCapability.Create(CapabilityReflection.CreateSkeleton(impl)));
        }

        /// <summary>
        /// Constructs an unbound instance.
        /// </summary>
        public BareProxy()
        {
        }

        /// <summary>
        /// Constructs an instance and binds it to the given low-level capability.
        /// </summary>
        /// <param name="cap">low-level capability</param>
        public BareProxy(ConsumedCapability cap): base(cap)
        {
        }

        /// <summary>
        /// Requests a method call.
        /// </summary>
        /// <param name="interfaceId">Target interface ID</param>
        /// <param name="methodId">Target method ID</param>
        /// <param name="args">Method arguments</param>
        /// <param name="tailCall">Whether it is a tail call</param>
        /// <returns>Answer promise</returns>
        public IPromisedAnswer Call(ulong interfaceId, ushort methodId, DynamicSerializerState args)
        {
            return base.Call(interfaceId, methodId, args, default);
        }
    }
}
