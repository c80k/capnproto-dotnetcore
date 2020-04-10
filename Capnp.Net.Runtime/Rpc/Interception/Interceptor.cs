using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Capnp.Rpc.Interception
{
    /// <summary>
    /// This static class provides extension methods for intercepting and unintercepting capabilities.
    /// </summary>
    public static class Interceptor
    {
        static readonly ConditionalWeakTable<ConsumedCapability, CensorCapability> _interceptMap =
            new ConditionalWeakTable<ConsumedCapability, CensorCapability>();

        /// <summary>
        /// Attach this policy to given capability.
        /// </summary>
        /// <typeparam name="TCap">Capability interface type</typeparam>
        /// <param name="policy">Policy to attach</param>
        /// <param name="cap">Capability to censor</param>
        /// <returns>Censored capability instance</returns>
        /// <exception cref="ArgumentNullException"><paramref name="policy"/> is null or 
        /// <paramref name="cap"/> is null</exception>
        public static TCap Attach<TCap>(this IInterceptionPolicy policy, TCap cap)
            where TCap: class
        {
            if (policy == null)
                throw new ArgumentNullException(nameof(policy));

            if (cap == null)
                throw new ArgumentNullException(nameof(cap));

            var cur = cap as CensorCapability;

            while (cur != null)
            {
                if (policy.Equals(cur.Policy))
                {
                    return cap;
                }

                cur = cur.InterceptedCapability as CensorCapability;
            }

            switch (cap)
            {
                case Proxy proxy:
                    return (CapabilityReflection.CreateProxy<TCap>(
                        Attach(policy, proxy.ConsumedCap!)) as TCap)!;

                case ConsumedCapability ccap:
                    return (new CensorCapability(ccap, policy) as TCap)!;

                default:
                    var temp = (CapabilityReflection.CreateProxy<TCap>(
                                CapabilityReflection.CreateSkeletonInternal(cap).AsCapability())) as TCap;
                    return Attach(policy, temp!)!;
            }
        }

        /// <summary>
        /// Detach this policy from given (censored) capability.
        /// </summary>
        /// <typeparam name="TCap">Capability interface type</typeparam>
        /// <param name="policy">Policy to detach</param>
        /// <param name="cap">Capability to clean</param>
        /// <returns>Clean capability instance (at least, without this interception policy)</returns>
        /// <exception cref="ArgumentNullException"><paramref name="policy"/> is null or 
        /// <paramref name="cap"/> is null</exception>
        public static TCap Detach<TCap>(this IInterceptionPolicy policy, TCap cap)
            where TCap: class
        {
            if (policy == null)
                throw new ArgumentNullException(nameof(policy));

            if (cap == null)
                throw new ArgumentNullException(nameof(cap));

            switch (cap)
            {
                case Proxy proxy:
                    return (CapabilityReflection.CreateProxy<TCap>(Detach(policy, proxy.ConsumedCap!)) as TCap)!;

                case CensorCapability ccap:
                    {
                        CensorCapability? cur = ccap;
                        var stk = new Stack<IInterceptionPolicy>();

                        do
                        {
                            if (policy.Equals(cur.Policy))
                            {
                                var cur2 = cur.InterceptedCapability;

                                foreach (var p in stk)
                                {
                                    cur2 = p.Attach(cur2);
                                }
                                return (cur2 as TCap)!;
                            }

                            stk.Push(cur.Policy);
                            cur = cur.InterceptedCapability as CensorCapability;
                        }
                        while (cur != null);

                        return (ccap as TCap)!;
                    }

                default:
                    return cap;
            }
        }
    }
}