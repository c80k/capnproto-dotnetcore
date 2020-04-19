using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Net.Runtime.Tests
{
    /// <summary>
    /// Imitates the behavior of a TCP connection by real hardware, which splits data transfer into multiple packets.
    /// </summary>
    class ScatteringStream : Stream
    {
        readonly Stream _baseStream;
        readonly int _mtu;

        public ScatteringStream(Stream baseStream, int mtu)
        {
            _baseStream = baseStream;
            _mtu = mtu;
        }

        public override bool CanRead => _baseStream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => _baseStream.CanWrite;

        public override long Length => _baseStream.Length;

        public override long Position 
        { 
            get => _baseStream.Position; 
            set => throw new NotImplementedException(); 
        }

        public override void Flush() => _baseStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => _baseStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                int amount = Math.Min(count, _mtu);
                _baseStream.Write(buffer, offset, amount);
                _baseStream.Flush();
                offset += amount;
                count -= amount;
            }
        }
    }
}
