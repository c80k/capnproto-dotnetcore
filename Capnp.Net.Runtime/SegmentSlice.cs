using System;

namespace Capnp
{
    /// <summary>
    /// Helper struct to represent the tuple (segment index, offset)
    /// </summary>
    public struct SegmentSlice
    {
        public uint SegmentIndex;
        public int Offset;
    }
}
