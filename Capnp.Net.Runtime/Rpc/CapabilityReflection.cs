using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Capnp.Rpc
{
    /// <summary>
    /// Provides functionality to construct Proxy and Skeleton instances from capability interfaces and objects implementing capability interfaces.
    /// A capability interface is any .NET interface which is annotated with <see cref="ProxyAttribute"/> and <see cref="SkeletonAttribute"/>.
    /// There are some intricacies to consider that you usually don't need to care about, since all that stuff will be generated.
    /// </summary>
    public static class CapabilityReflection
    {
        interface IBrokenFactory
        {
            System.Exception Exception { get; }
        }

        abstract class ProxyFactory
        {
            public abstract Proxy NewProxy();
        }

        class ProxyFactory<T>: ProxyFactory where T: Proxy, new()
        {
            public override Proxy NewProxy() => new T();
        }

        class BrokenProxyFactory: ProxyFactory, IBrokenFactory
        {
            readonly System.Exception _exception;

            public BrokenProxyFactory(System.Exception exception)
            {
                _exception = exception;
            }

            public System.Exception Exception => _exception;

            public override Proxy NewProxy()
            {
                throw _exception;
            }
        }

        abstract class SkeletonFactory
        {
            public abstract Skeleton NewSkeleton();
        }

        class SkeletonFactory<T>: SkeletonFactory where T: Skeleton, new()
        {
            public override Skeleton NewSkeleton() => new T();
        }

        class BrokenSkeletonFactory: SkeletonFactory, IBrokenFactory
        {
            System.Exception _exception;

            public BrokenSkeletonFactory(System.Exception exception)
            {
                _exception = exception;
            }

            public System.Exception Exception => _exception;

            public override Skeleton NewSkeleton()
            {
                throw _exception;
            }
        }

        class PolySkeletonFactory: SkeletonFactory
        {
            readonly SkeletonFactory[] _monoFactories;

            public PolySkeletonFactory(SkeletonFactory[] monoFactories)
            {
                _monoFactories = monoFactories;
            }

            public override Skeleton NewSkeleton()
            {
                var poly = new PolySkeleton();
                foreach (var fac in _monoFactories)
                {
                    poly.AddInterface(fac.NewSkeleton());
                }
                return poly;
            }
        }

        static ConditionalWeakTable<Type, ProxyFactory> _proxyMap =
            new ConditionalWeakTable<Type, ProxyFactory>();
        static ConditionalWeakTable<Type, SkeletonFactory> _skeletonMap =
            new ConditionalWeakTable<Type, SkeletonFactory>();

        static CapabilityReflection()
        {
            _proxyMap.Add(typeof(BareProxy), new ProxyFactory<BareProxy>());
        }

        static SkeletonFactory CreateMonoSkeletonFactory(SkeletonAttribute attr, Type[] genericArguments)
        {
            var skeletonClass = attr.SkeletonClass;
            if (genericArguments.Length > 0)
                skeletonClass = skeletonClass.MakeGenericType(genericArguments);

            return (SkeletonFactory)Activator.CreateInstance(
                typeof(SkeletonFactory<>)
                .MakeGenericType(skeletonClass));
        }

        static SkeletonFactory GetSkeletonFactory(Type type)
        {
            return _skeletonMap.GetValue(type, _ =>
            {
                try
                {
                    var attrs = (from iface in _.GetInterfaces()
                                 from attr in iface.GetCustomAttributes(typeof(SkeletonAttribute), true)
                                 select (SkeletonAttribute)attr).ToArray();

                    if (attrs.Length == 0)
                        throw new InvalidCapabilityInterfaceException("No 'Skeleton' attribute defined, don't know how to create the skeleton");

                    Type[] genericArguments = type.GetGenericArguments();

                    if (attrs.Length == 1)
                    {
                        return CreateMonoSkeletonFactory(attrs[0], genericArguments);
                    }
                    else
                    {
                        var monoFactories = attrs.Select(a => CreateMonoSkeletonFactory(a, genericArguments)).ToArray();
                        return new PolySkeletonFactory(monoFactories);
                    }
                }
                catch (System.Exception exception)
                {
                    return new BrokenSkeletonFactory(exception);
                }
            });
        }

        /// <summary>
        /// Creates a Skeleton for a given interface implementation.
        /// </summary>
        /// <param name="obj">Interface implementation. Must implement at least one interface which is annotated with a <see cref="SkeletonAttribute"/>.</param>
        /// <returns>The Skeleton</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null.</exception>
        /// <exception cref="InvalidCapabilityInterfaceException">No <see cref="SkeletonAttribute"/> found on implemented interface(s).</exception>
        /// <exception cref="InvalidOperationException">Mismatch between generic type arguments (if capability interface is generic).</exception>
        /// <exception cref="ArgumentException">Mismatch between generic type arguments (if capability interface is generic).</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">Problem with instatiating the Skeleton (constructor threw exception).</exception>
        /// <exception cref="MemberAccessException">Caller does not have permission to invoke the Skeleton constructor.</exception>
        /// <exception cref="TypeLoadException">Problem with building the Skeleton type, or problem with loading some dependent class.</exception>
        public static Skeleton CreateSkeleton(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var factory = GetSkeletonFactory(obj.GetType());
            var skeleton = factory.NewSkeleton();
            skeleton.Bind(obj);
            return skeleton;
        }

        static ProxyFactory GetProxyFactory(Type type)
        {
            return _proxyMap.GetValue(type, _ =>
            {
                try
                {
                    var attrs = type
                        .GetCustomAttributes(typeof(ProxyAttribute), true)
                        .Cast<ProxyAttribute>()
                        .ToArray();

                    if (attrs.Length == 0)
                        throw new InvalidCapabilityInterfaceException("No 'Proxy' attribute defined, don't know how to create the proxy");

                    if (attrs.Length == 1)
                    {
                        Type proxyClass = attrs[0].ProxyClass;
                        Type[] genericArguments = type.GetGenericArguments();
                        if (genericArguments.Length > 0)
                            proxyClass = proxyClass.MakeGenericType(proxyClass);

                        return (ProxyFactory)Activator.CreateInstance(
                            typeof(ProxyFactory<>)
                            .MakeGenericType(proxyClass));
                    }
                    else
                    {
                        throw new InvalidCapabilityInterfaceException("Multiple 'Proxy' attributes defined, don't know which one to take");
                    }
                }
                catch (System.Exception exception)
                {
                    return new BrokenProxyFactory(exception);
                }
            });
        }


        /// <summary>
        /// Validates that a given type qualifies as cpapbility interface, throws <see cref="InvalidCapabilityInterfaceException"/> on failure.
        /// </summary>
        /// <param name="interfaceType">type to check</param>
        /// <exception cref="ArgumentNullException"><paramref name="interfaceType"/> is null.</exception>
        /// <exception cref="InvalidCapabilityInterfaceException">Given typ did not qualify as capability interface. 
        /// Message and probably InnterException give more details.</exception>
        public static void ValidateCapabilityInterface(Type interfaceType)
        {
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            var proxyFactory = GetProxyFactory(interfaceType);

            if (proxyFactory is IBrokenFactory brokenFactory)
            {
                throw new InvalidCapabilityInterfaceException(
                    "Given type did not qualify as capability interface, see inner exception.",
                    brokenFactory.Exception);
            }
        }

        /// <summary>
        /// Checkes whether a given type qualifies as cpapbility interface./> on failure.
        /// </summary>
        /// <param name="interfaceType">type to check</param>
        /// <returns>true when <paramref name="interfaceType"/> is a capability interface</returns>
        /// <exception cref="ArgumentNullException"><paramref name="interfaceType"/> is null.</exception>
        public static bool IsValidCapabilityInterface(Type interfaceType)
        {
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            var proxyFactory = GetProxyFactory(interfaceType);

            return !(proxyFactory is IBrokenFactory);
        }

        /// <summary>
        /// Constructs a Proxy for given capability interface and wraps it around given low-level capability.
        /// </summary>
        /// <typeparam name="TInterface">Capability interface. Must be annotated with <see cref="ProxyAttribute"/>.</typeparam>
        /// <param name="cap">low-level capability</param>
        /// <returns>The Proxy instance which implements <typeparamref name="TInterface"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="cap"/> is null.</exception>
        /// <exception cref="InvalidCapabilityInterfaceException"><typeparamref name="TInterface"/> did not qualify as capability interface.</exception>
        /// <exception cref="InvalidOperationException">Mismatch between generic type arguments (if capability interface is generic).</exception>
        /// <exception cref="ArgumentException">Mismatch between generic type arguments (if capability interface is generic).</exception>
        /// <exception cref="System.Reflection.TargetInvocationException">Problem with instatiating the Proxy (constructor threw exception).</exception>
        /// <exception cref="MemberAccessException">Caller does not have permission to invoke the Proxy constructor.</exception>
        /// <exception cref="TypeLoadException">Problem with building the Proxy type, or problem with loading some dependent class.</exception>
        public static Proxy CreateProxy<TInterface>(ConsumedCapability cap,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            var factory = GetProxyFactory(typeof(TInterface));
            var proxy = factory.NewProxy();
            proxy.Bind(cap);
#if DebugFinalizers
            proxy.CreatorMemberName = memberName;
            proxy.CreatorFilePath = sourceFilePath;
            proxy.CreatorLineNumber = sourceLineNumber;
            if (cap != null)
            {
                cap.CreatorFilePath = proxy.CreatorFilePath;
                cap.CreatorLineNumber = proxy.CreatorLineNumber;
                cap.CreatorMemberName = proxy.CreatorMemberName;
            }
#endif
            return proxy;
        }
    }
}
