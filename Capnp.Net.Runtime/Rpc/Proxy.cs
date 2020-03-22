using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// Application-level wrapper for consumer-side capabilities.
    /// The code generator will produce a Proxy specialization for each capability interface.
    /// </summary>
    public class Proxy : IDisposable, IResolvingCapability
    {
        /// <summary>
        /// Creates a new proxy object for an existing implementation or proxy, sharing its ownership.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <param name="obj">instance to share</param>
        /// <returns></returns>
        public static T Share<T>(T obj) where T: class
        {
            if (obj is Proxy proxy)
                return proxy.Cast<T>(false);
            else
                return BareProxy.FromImpl(obj).Cast<T>(true);
        }

#if DebugFinalizers
        ILogger Logger { get; } = Logging.CreateLogger<Proxy>();
#endif

        bool _disposedValue = false;

        /// <summary>
        /// Will eventually give the resolved capability, if this is a promised capability.
        /// </summary>
        public Task<ConsumedCapability?> WhenResolved
        {
            get
            {
                return ConsumedCap is IResolvingCapability resolving ? 
                    resolving.WhenResolved : 
                    Task.FromResult(ConsumedCap);
            }
        }

        /// <summary>
        /// Underlying low-level capability
        /// </summary>
        protected internal ConsumedCapability? ConsumedCap { get; private set; }

        /// <summary>
        /// Whether is this a broken capability.
        /// </summary>
        public bool IsNull => ConsumedCap == null;

        static async void DisposeCtrWhenReturned(CancellationTokenRegistration ctr, IPromisedAnswer answer)
        {
            try { await answer.WhenReturned; }
            catch { }
            finally { ctr.Dispose(); }
        }

        /// <summary>
        /// Calls a method of this capability.
        /// </summary>
        /// <param name="interfaceId">Interface ID to call</param>
        /// <param name="methodId">Method ID to call</param>
        /// <param name="args">Method arguments ("param struct")</param>
        /// <param name="obsoleteAndIgnored">This flag is ignored. It is there to preserve compatibility with the
        /// code generator and will be removed in future versions.</param>
        /// <param name="cancellationToken">For cancelling an ongoing method call</param>
        /// <returns>An answer promise</returns>
        /// <exception cref="ObjectDisposedException">This instance was disposed, or transport-layer stream was disposed.</exception>
        /// <exception cref="InvalidOperationException">Capability is broken.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        protected internal IPromisedAnswer Call(ulong interfaceId, ushort methodId, DynamicSerializerState args, 
            bool obsoleteAndIgnored, CancellationToken cancellationToken = default)
        {
            if (_disposedValue)
            {
                args.Dispose();
                throw new ObjectDisposedException(nameof(Proxy));
            }

            if (ConsumedCap == null)
            {
                args.Dispose();
                throw new InvalidOperationException("Cannot call null capability");
            }

            var answer = ConsumedCap.DoCall(interfaceId, methodId, args);

            if (cancellationToken.CanBeCanceled)
            {
                DisposeCtrWhenReturned(cancellationToken.Register(answer.Dispose), answer);
            }

            return answer;
        }

        /// <summary>
        /// Constructs a null instance.
        /// </summary>
        public Proxy()
        {
#if DebugFinalizers
            CreatorStackTrace = Environment.StackTrace;
#endif
        }

        internal Proxy(ConsumedCapability? cap): this()
        {
            Bind(cap);
        }

        internal void Bind(ConsumedCapability? cap)
        {
            if (ConsumedCap != null)
                throw new InvalidOperationException("Proxy was already bound");

            if (cap == null)
                return;

            ConsumedCap = cap;
            cap.AddRef();

#if DebugFinalizers
            if (ConsumedCap != null)
                ConsumedCap.OwningProxy = this;
#endif
        }

        internal Skeleton? GetProvider()
        {
            switch (ConsumedCap)
            {
                case LocalCapability lcap:
                    return lcap.ProvidedCap;

                case null:
                    return null;

                default:
                    return Vine.Create(ConsumedCap);
            }
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    ConsumedCap?.Release();
                }
                else
                {
                    // When called from the Finalizer, we must not throw.
                    // But when reference counting goes wrong, ConsumedCapability.Release() will throw an InvalidOperationException.
                    // The only option here is to suppress that exception.
                    try { ConsumedCap?.Release(); }
                    catch { }
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~Proxy()
        {
#if DebugFinalizers
            Logger?.LogWarning($"Caught orphaned Proxy, created from here: {CreatorStackTrace}.");
#endif

            Dispose(false);
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Casts this Proxy to a different capability interface.
        /// </summary>
        /// <typeparam name="T">Desired capability interface</typeparam>
        /// <param name="disposeThis">Whether to Dispose() this Proxy instance</param>
        /// <returns>Proxy for desired capability interface</returns>
        /// <exception cref="InvalidCapabilityInterfaceException"><typeparamref name="T"/> did not qualify as capability interface.</exception>
        /// <exception cref="InvalidOperationException">This capability is broken, or mismatch between generic type arguments (if capability interface is generic).</exception>
        /// <exception cref="ArgumentException">Mismatch between generic type arguments (if capability interface is generic).</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">Problem with instatiating the Proxy (constructor threw exception).</exception>
        /// <exception cref="MemberAccessException">Caller does not have permission to invoke the Proxy constructor.</exception>
        /// <exception cref="TypeLoadException">Problem with building the Proxy type, or problem with loading some dependent class.</exception>
        public T Cast<T>(bool disposeThis) where T: class
        {
            if (IsNull)
                throw new InvalidOperationException("Capability is broken");

            using (disposeThis ? this : null)
            {
                return (CapabilityReflection.CreateProxy<T>(ConsumedCap) as T)!;
            }
        }

        internal void Export(IRpcEndpoint endpoint, CapDescriptor.WRITER writer)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(Proxy));

            if (ConsumedCap == null)
                writer.which = CapDescriptor.WHICH.None;
            else
                ConsumedCap.Export(endpoint, writer);
        }

        internal void Freeze(out IRpcEndpoint? boundEndpoint)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(Proxy));

            boundEndpoint = null;
            ConsumedCap?.Freeze(out boundEndpoint);
        }

        internal void Unfreeze()
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(Proxy));

            ConsumedCap?.Unfreeze();
        }

#if DebugFinalizers
        string CreatorStackTrace { get; set; }
#endif
    }
}