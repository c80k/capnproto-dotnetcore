using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Capnp.Util
{
    internal class BufferedNetworkStreamAdapter : Stream
    {
        // A buffer size of 1024 bytes seems to be a good comprise, giving good performance 
        // in TCP/IP-over-localhost scenarios for small to medium (200kiB) frame sizes.
        const int DefaultBufferSize = 1024;

        readonly BufferedStream _readStream;
        readonly NetworkStream _writeStream;
        readonly object _reentrancyBlocker = new object();
        Exception? _bufferedException;

        public BufferedNetworkStreamAdapter(Stream stream, int bufferSize)
        {
            _readStream = new BufferedStream(stream, bufferSize);
            _writeStream = stream as NetworkStream ?? throw new ArgumentException("stream argument must be a NetworkStream");
        }

        public BufferedNetworkStreamAdapter(Stream stream) : this(stream, DefaultBufferSize)
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

        void WriteCallback(IAsyncResult ar)
        {
            try
            {
                _writeStream.EndWrite(ar);
            }
            catch (Exception exception)
            {
                Volatile.Write(ref _bufferedException, exception);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var exception = Volatile.Read(ref _bufferedException);
            
            if (exception != null)
            {
                Dispose();
                throw exception;
            }

            _writeStream.BeginWrite(buffer, offset, count, WriteCallback, null);
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
