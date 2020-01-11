using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// The segment allocator default implementation.
    /// </summary>
    public class SegmentAllocator : ISegmentAllocator
    {
        class Segment
        {
            public Segment(Memory<ulong> mem, uint id)
            {
                Mem = mem;
                Id = id;
            }

            public uint Id { get; }
            public Memory<ulong> Mem { get; }
            public int FreeOffset { get; set; }

            public bool TryAllocate(uint nwords, out int offset)
            {
                if (checked(FreeOffset + (int)nwords) <= Mem.Length)
                {
                    offset = FreeOffset;
                    int count = (int)nwords;
                    FreeOffset += count;
                    return true;
                }
                else
                {
                    offset = -1;
                    return false;
                }
            }

            public bool IsFull => FreeOffset >= Mem.Length;
        }

        readonly int _defaultSegmentSize;
        readonly List<Segment> _segments = new List<Segment>();
        readonly List<Segment> _nonFullSegments = new List<Segment>();

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="defaultSegmentSize">Default size (in words) of a newly allocated segment. If a single allocation requires 
        /// a bigger size, a bigger dedicated segment will be allocated. On the wire, segments will be truncated to their actual
        /// occupancies.</param>
        public SegmentAllocator(int defaultSegmentSize = 128)
        {
            _defaultSegmentSize = defaultSegmentSize;
        }

        /// <summary>
        /// The list of currently allocated segments, each one truncated to its actual occupancy.
        /// </summary>
        public IReadOnlyList<Memory<ulong>> Segments => _segments.LazyListSelect(s => s.Mem.Slice(0, s.FreeOffset));

        /// <summary>
        /// Allocates memory.
        /// </summary>
        /// <param name="nwords">Number of words to allocate</param>
        /// <param name="preferredSegmentIndex">Preferred segment index. If enough space is available, 
        /// memory will be allocated inside that segment. Otherwise, a different segment will be chosen, or
        /// a new one will be allocated, or allocation will fail (depending on <paramref name="forcePreferredSegment"/>).</param>
        /// <param name="result">The allocated memory slice in case of success (<code>default(SegmentSlice) otherwise)</code></param>
        /// <param name="forcePreferredSegment">Whether using the preferred segment is mandatory. If it is and there is not
        /// enough space available, allocation will fail.</param>
        /// <returns>Whether allocation was successful.</returns>
        public bool Allocate(uint nwords, uint preferredSegmentIndex, out SegmentSlice result, bool forcePreferredSegment)
        {
            result = default;
            Segment segment;

            if (preferredSegmentIndex < _segments.Count)
            {
                segment = _segments[(int)preferredSegmentIndex];

                if (segment.TryAllocate(nwords, out result.Offset))
                {
                    result.SegmentIndex = preferredSegmentIndex;
                    return true;
                }
            }

            if (forcePreferredSegment)
            {
                return false;
            }

            for (int i = 0; i < _nonFullSegments.Count; i++)
            {
                segment = _nonFullSegments[i];

                if (segment.TryAllocate(nwords, out result.Offset))
                {
                    result.SegmentIndex = segment.Id;

                    if (segment.IsFull)
                    {
                        int n = _nonFullSegments.Count - 1;
                        var tmp = _nonFullSegments[i];
                        _nonFullSegments[i] = _nonFullSegments[n];
                        _nonFullSegments[n] = tmp;
                        _nonFullSegments.RemoveAt(n);
                    }
                    else if (i > 0)
                    {
                        var tmp = _nonFullSegments[i];
                        _nonFullSegments[i] = _nonFullSegments[0];
                        _nonFullSegments[0] = tmp;
                    }

                    return true;
                }
            }

            int size = Math.Max((int)nwords, _defaultSegmentSize);
            var storage = new ulong[size];
            var mem = new Memory<ulong>(storage);
            segment = new Segment(mem, (uint)_segments.Count);

            _segments.Add(segment);

            if (!segment.TryAllocate(nwords, out result.Offset))
                throw new InvalidProgramException();

            result.SegmentIndex = segment.Id;

            if (!segment.IsFull)
            {
                _nonFullSegments.Add(segment);

                int n = _nonFullSegments.Count - 1;
                var tmp = _nonFullSegments[0];
                _nonFullSegments[0] = _nonFullSegments[n];
                _nonFullSegments[n] = tmp;
            }

            return true;
        }
    }
}
#nullable restore