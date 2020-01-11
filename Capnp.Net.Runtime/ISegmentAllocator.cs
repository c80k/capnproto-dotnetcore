using System;
using System.Collections.Generic;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// Implements a segment allocation policy for Cap'n Proto building messages.
    /// </summary>
    public interface ISegmentAllocator
    {
        /// <summary>
        /// Currently allocated segments.
        /// </summary>
        IReadOnlyList<Memory<ulong>> Segments { get; }

        /// <summary>
        /// Attempts to allocate a memory block. The first allocation attempt is made inside the segment specified by <paramref name="preferredSegment"/>.
        /// If that segment does not provide enough space or does not exist, further actions depend on the <paramref name="forcePreferredSegment"/> flag.
        /// If that flag is true, allocation will fail (return false). Otherwise, the allocation shall scan existing segments for the requested amount of space,
        /// and create a new segment if none provides enough space.
        /// </summary>
        /// <param name="nwords">Number of words to allocate</param>
        /// <param name="preferredSegment">Index of preferred segment wherein the block should be allocated</param>
        /// <param name="slice">Position of allocated memory block (undefined in case of failure)</param>
        /// <param name="forcePreferredSegment">Whether the segment specified by <paramref name="preferredSegment"/> is mandatory</param>
        /// <returns>Whether allocation was successful</returns>
        bool Allocate(uint nwords, uint preferredSegment, out SegmentSlice slice, bool forcePreferredSegment);
    }
}
#nullable restore