using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Capnp
{
    /// <summary>
    /// Supports the deserialization of Cap'n Proto messages from a stream (see https://capnproto.org/encoding.html#serialization-over-a-stream).
    /// Packing and compression cannot be handled yet.
    /// </summary>
    public static class Framing
    {
        /// <summary>
        /// Deserializes a message from given stream.
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <returns>The deserialized message</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="ArgumentException">The stream does not support reading, is null, or is already closed.</exception>
        /// <exception cref="EndOfStreamException">The end of the stream is reached.</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /// <exception cref="InvalidDataException">Encountered invalid framing data.</exception>
        /// <exception cref="OutOfMemoryException">Too many or too large segments, probably due to invalid framing data.</exception>
        public static WireFrame ReadSegments(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.Default, true))
            {
                return reader.ReadWireFrame();
            }
        }

        public static WireFrame ReadWireFrame(this BinaryReader reader)
        {
            uint scount = reader.ReadUInt32();
            if(scount++ == UInt32.MaxValue) throw new InvalidDataException("Encountered Invalid Framing Data");
            var buffers = new Memory<ulong>[scount];

            for (uint i = 0; i < scount; i++)
            {
                uint size = reader.ReadUInt32();
                if(size==0) throw new EndOfStreamException("Stream Closed");
                buffers[i] = new Memory<ulong>(new ulong[size]);
            }

            if ((scount & 1) == 0)
            {
                // Padding
                reader.ReadUInt32();
            }

            FillBuffersFromFrames(buffers, scount, reader);

            return new WireFrame(buffers);
        }
        
        public static void FillBuffersFromFrames(Memory<ulong>[] buffers, uint segmentCount, BinaryReader reader)
        {
            for (uint i = 0; i < segmentCount; i++)
            {
                var buffer = MemoryMarshal.Cast<ulong, byte>(buffers[i].Span.ToArray());
                var tmpBuffer = reader.ReadBytes(buffer.Length);

                if (tmpBuffer.Length != buffer.Length)
                {
                    throw new InvalidDataException("Expected more bytes according to framing header");
                }
                
                // Fastest way to do this without /unsafe
                for (int j = 0; j < buffers[i].Length; j++)
                {
                    var value = BitConverter.ToUInt64(tmpBuffer, j*8);
                    buffers[i].Span[j] = value;
                }
            }
        }
    }
}
