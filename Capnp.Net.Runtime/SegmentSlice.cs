using System;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// Helper struct to represent the tuple (segment index, offset)
    /// </summary>
    public struct SegmentSlice
    {
        /// <summary>
        /// Segment index
        /// </summary>
        public uint SegmentIndex;

        /// <summary>
        /// Word offset within segment
        /// </summary>
        public int Offset;
    }
}
#nullable restore