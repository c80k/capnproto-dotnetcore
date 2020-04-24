using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// Combines multiple skeletons to represent objects which implement multiple interfaces.
    /// </summary>
    public class PolySkeleton: RefCountingSkeleton
    {
        readonly Dictionary<ulong, Skeleton> _ifmap = new Dictionary<ulong, Skeleton>();

        /// <summary>
        /// Adds a skeleton to this instance.
        /// </summary>
        /// <param name="interfaceId">Interface ID</param>
        /// <param name="skeleton">Skeleton to add</param>
        /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is null.</exception>
        /// <exception cref="InvalidOperationException">A skeleton with <paramref name="interfaceId"/> was already added.</exception>
        public void AddInterface(ulong interfaceId, Skeleton skeleton)
        {
            if (skeleton == null)
                throw new ArgumentNullException(nameof(skeleton));

            _ifmap.Add(interfaceId, skeleton);
            if (_ifmap.Count == 1) // Claiming only the first one is sufficient
                skeleton.Claim();
        }

        internal void AddInterface(Skeleton skeleton)
        {
            AddInterface(((IMonoSkeleton)skeleton).InterfaceId, skeleton);
        }

        /// <summary>
        /// Calls an interface method of this capability.
        /// </summary>
        /// <param name="interfaceId">ID of interface to call</param>
        /// <param name="methodId">ID of method to call</param>
        /// <param name="args">Method arguments ("params struct")</param>
        /// <param name="cancellationToken">Cancellation token, indicating when the call should cancelled.</param>
        /// <returns>A Task which will resolve to the call result</returns>
        public override Task<AnswerOrCounterquestion> Invoke(ulong interfaceId, ushort methodId, DeserializerState args, CancellationToken cancellationToken = default)
        {
            if (_ifmap.TryGetValue(interfaceId, out var skel))
                return skel.Invoke(interfaceId, methodId, args, cancellationToken);

            throw new NotImplementedException("Unknown interface id");
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            foreach (var cap in _ifmap.Values.Take(1))
            // releasing first skeleton is sufficient. Avoid double-Dispose!
            {
                cap.Relinquish();
            }
        }

        internal override void Bind(object impl)
        {
            foreach (Skeleton skel in _ifmap.Values) 
            {
                skel.Bind(impl);
            }
        }
    }
}