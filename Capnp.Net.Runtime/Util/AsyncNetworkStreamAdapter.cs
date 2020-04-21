using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Capnp.Util
{
    internal class AsyncNetworkStreamAdapter : Stream
    {
        // Async I/O pays off for large payloads. Perf. profiling gave a threshold around 200kB
        const int DefaultAsyncThreshold = 200000;

        readonly NetworkStream _stream;
        readonly int _asyncThreshold;
        readonly object _reentrancyBlocker = new object();
        Exception? _bufferedException;

        public AsyncNetworkStreamAdapter(Stream stream, int asyncThreshold)
        {
            _asyncThreshold = asyncThreshold;
            _stream = stream as NetworkStream ?? throw new ArgumentException("stream argument must be a NetworkStream");
        }

        public AsyncNetworkStreamAdapter(Stream stream): this(stream, DefaultAsyncThreshold)
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
            _stream.FlushAsync();
            //_stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        //void WriteCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        _stream.EndWrite(ar);
        //    }
        //    catch (Exception exception)
        //    {
        //        Volatile.Write(ref _bufferedException, exception);
        //    }
        //}

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.WriteAsync(buffer, offset, count);
            //var exception = Volatile.Read(ref _bufferedException);
            
            //if (exception != null)
            //{
            //    Dispose();
            //    throw exception;
            //}

            //if (count >= _asyncThreshold)
            //    _stream.BeginWrite(buffer, offset, count, WriteCallback, null);
            //else
            //    _stream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_reentrancyBlocker)
                {
                    try
                    {
                        _stream.Dispose();
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
