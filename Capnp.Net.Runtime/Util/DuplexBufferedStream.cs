using System;
using System.IO;

namespace Capnp.Util
{
    internal class DuplexBufferedStream : Stream
    {
        // A buffer size of 1024 bytes seems to be a good comprise, giving good performance 
        // in TCP/IP-over-localhost scenarios for small to medium (200kiB) frame sizes.
        const int DefaultBufferSize = 1024;

        readonly BufferedStream _readStream;
        readonly BufferedStream _writeStream;
        readonly int _bufferSize;
        readonly object _reentrancyBlocker = new object();

        public DuplexBufferedStream(Stream stream, int bufferSize)
        {
            _readStream = new BufferedStream(stream, bufferSize);
            _writeStream = new BufferedStream(stream, bufferSize);
            _bufferSize = bufferSize;
        }

        public DuplexBufferedStream(Stream stream): this(stream, DefaultBufferSize)
        {
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => 0;

        public override long Position 
        { 
            get => 0; 
            set => throw new NotSupportedException(); 
        }

        public override void Flush()
        {
            _writeStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _readStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer.Length > _bufferSize) // avoid moiré-like timing effects
                _writeStream.Flush();        

            _writeStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_reentrancyBlocker)
                {
                    try
                    {
                        _readStream.Dispose();
                    }
                    catch
                    {
                    }
                    try
                    {
                        _writeStream.Dispose();
                    }
                    catch
                    {
                    }
                }
            }

            base.Dispose(disposing);
        }
    }
}
