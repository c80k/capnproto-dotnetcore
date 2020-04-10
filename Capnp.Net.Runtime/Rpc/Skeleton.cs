using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// A skeleton is a wrapper around a capability interface implementation which adapts it in the way it is
    /// expected by the <see cref="RpcEngine"/>.
    /// </summary>
    public abstract class Skeleton: IProvidedCapability
    {
        class SkeletonRelinquisher: IDisposable
        {
            readonly Skeleton _skeleton;

            public SkeletonRelinquisher(Skeleton skeleton)
            {
                _skeleton = skeleton;
                _skeleton.Claim();
            }

            public void Dispose()
            {
                _skeleton.Relinquish();
            }
        }

        /// <summary>
        /// Claims ownership on the given capability, preventing its automatic disposal.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <param name="impl">Capability implementation</param>
        /// <returns>A disposable object. Calling Dispose() on the returned instance relinquishes ownership again.</returns>
        public static IDisposable Claim<T>(T impl) where T: class
        {
            return new SkeletonRelinquisher(CapabilityReflection.CreateSkeletonInternal(impl));
        }

        /// <summary>
        /// Calls an interface method of this capability.
        /// </summary>
        /// <param name="interfaceId">ID of interface to call</param>
        /// <param name="methodId">ID of method to call</param>
        /// <param name="args">Method arguments ("params struct")</param>
        /// <param name="cancellationToken">Cancellation token, indicating when the call should cancelled.</param>
        /// <returns>A Task which will resolve to the call result</returns>
        public abstract Task<AnswerOrCounterquestion> Invoke(ulong interfaceId, ushort methodId, DeserializerState args, CancellationToken cancellationToken = default);

        internal abstract void Claim();
        internal abstract void Relinquish();

        internal void Relinquish(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            while (count-- > 0)
                Relinquish();
        }

        internal virtual void Bind(object impl)
        {
            throw new NotSupportedException("Cannot bind");
        }

        internal abstract ConsumedCapability AsCapability();
    }

    /// <summary>
    /// Skeleton for a specific capability interface.
    /// </summary>
    /// <typeparam name="T">Capability interface</typeparam>
    public abstract class Skeleton<T> : RefCountingSkeleton, IMonoSkeleton
    {
        Func<DeserializerState, CancellationToken, Task<AnswerOrCounterquestion>>[] _methods = null!;

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        public Skeleton()
        {
        }

        /// <summary>
        /// Populates this skeleton's method table. The method table maps method IDs (which are consecutively numbered from 0 
        /// onwards) to the underlying capability's method implementations.
        /// </summary>
        /// <param name="methods">The method table. Index is method ID.</param>
        protected void SetMethodTable(params Func<DeserializerState, CancellationToken, Task<AnswerOrCounterquestion>>[] methods)
        {
            _methods = methods;
        }

        /// <summary>
        /// Gets the underlying capability implementation.
        /// </summary>
        protected T Impl { get; private set; } = default!;

        /// <summary>
        /// Gets the ID of the implemented interface.
        /// </summary>
        public abstract ulong InterfaceId { get; }

        /// <summary>
        /// Calls an interface method of this capability.
        /// </summary>
        /// <param name="interfaceId">ID of interface to call</param>
        /// <param name="methodId">ID of method to call</param>
        /// <param name="args">Method arguments ("params struct")</param>
        /// <param name="cancellationToken">Cancellation token, indicating when the call should cancelled.</param>
        /// <returns>A Task which will resolve to the call result</returns>
        /// <exception cref="ObjectDisposedException">This Skeleton was disposed</exception>
        public override async Task<AnswerOrCounterquestion> Invoke(ulong interfaceId, ushort methodId, DeserializerState args, CancellationToken cancellationToken = default)
        {
            if (InterfaceId != InterfaceId)
                throw new NotImplementedException("Wrong interface id");

            if (methodId >= _methods.Length)
                throw new NotImplementedException("Wrong method id");

            return await _methods[methodId](args, cancellationToken);
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && Impl is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        internal override void Bind(object impl)
        {
            if (Impl != null)
                throw new InvalidOperationException("Skeleton was already bound");

            Impl = (T)impl;
        }
    }
}