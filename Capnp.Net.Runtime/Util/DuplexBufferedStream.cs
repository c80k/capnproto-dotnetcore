using System;
using System.IO;

namespace Capnp.Util
{
    internal class DuplexBufferedStream : Stream
    {
        readonly BufferedStream _readStream;
        readonly BufferedStream _writeStream;
        readonly object _reentrancyBlocker = new object();

        public DuplexBufferedStream(Stream stream)
        {
            _readStream = new BufferedStream(stream);
            _writeStream = new BufferedStream(stream);
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
