using System;

namespace Capnp
{
    /// <summary>
    /// An implementations of this interface represents a struct which is being deserialized from a Cap'n Proto object.
    /// </summary>
    public interface IStructDeserializer
    {
        /// <summary>
        /// Reads a slice of up to 64 bits from this struct's data section, starting from the specified bit offset.
        /// The slice must be aligned within a 64 bit word boundary.
        /// </summary>
        /// <param name="bitOffset">Start bit offset relative to the data section, little endian</param>
        /// <param name="bitCount">numbers of bits to read</param>
        /// <returns>the data</returns>
        /// <exception cref="ArgumentOutOfRangeException">non-aligned access</exception>
        /// <exception cref="IndexOutOfRangeException">bitOffset exceeds the data section</exception>
        /// <exception cref="DeserializationException">this state does not represent a struct</exception>
        ulong StructReadData(ulong bitOffset, int bitCount);
    }
}
