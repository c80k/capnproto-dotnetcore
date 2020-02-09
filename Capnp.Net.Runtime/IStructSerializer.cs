using System;

namespace Capnp
{
    /// <summary>
    /// An implementations of this interface represents a struct which is being serialized as Cap'n Proto object.
    /// </summary>
    public interface IStructSerializer: IStructDeserializer
    {
        /// <summary>
        /// Writes data (up to 64 bits) into the underlying struct's data section. 
        /// The write operation must be aligned to fit within a single word.
        /// </summary>
        /// <param name="bitOffset">Start bit relative to the struct's data section, little endian</param>
        /// <param name="bitCount">Number of bits to write</param>
        /// <param name="data">Data bits to write</param>
        /// <exception cref="InvalidOperationException">The object was not determined to be a struct</exception>
        /// <exception cref="ArgumentOutOfRangeException">The data slice specified by <paramref name="bitOffset"/> and <paramref name="bitCount"/>
        /// is not completely within the struct's data section, misaligned, exceeds one word, or <paramref name="bitCount"/> is negative</exception>
        void StructWriteData(ulong bitOffset, int bitCount, ulong data);

        /// <summary>
        /// The struct's data section as memory span.
        /// </summary>
        Span<ulong> StructDataSection { get; }
    }
}