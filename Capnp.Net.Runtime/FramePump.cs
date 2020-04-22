using Capnp.FrameTracing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Capnp
{
    /// <summary>
    /// The FramePump handles sending and receiving Cap'n Proto messages over a stream. It exposes a Send method for writing frames to the stream, and an
    /// event handler for processing received frames. It does not fork any new thread by itself, but instead exposes a synchronous blocking Run method that
    /// implements the receive loop. Invoke this method in the thread context of your choice.
    /// </summary>
    public class FramePump: IDisposable
    {
        ILogger Logger { get; } = Logging.CreateLogger<FramePump>();

        int _disposing;
        readonly Stream _stream;
        readonly BinaryWriter? _writer;
        readonly object _writeLock = new object();
        readonly List<IFrameTracer> _tracers = new List<IFrameTracer>();

        /// <summary>
        /// Constructs a new instance for given stream.
        /// </summary>
        /// <param name="stream">The stream for message I/O. 
        /// If you intend to receive messages, the stream must support reading (CanRead).
        /// If you intend to send messages, the stream must support writing (CanWrite).
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        public FramePump(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            if (stream.CanWrite)
                _writer = new BinaryWriter(_stream);
        }

        /// <summary>
        /// Disposes this instance and the underlying stream. This will also cause the Run method to return.
        /// </summary>
        public void Dispose()
        {
            if (0 == Interlocked.Exchange(ref _disposing, 1))
            {
                foreach (var tracer in _tracers)
                {
                    tracer.Dispose();
                }

                _writer?.Dispose();
                _stream.Dispose();
            }
        }

        /// <summary>
        /// Event handler for frame reception.
        /// </summary>
        public event Action<WireFrame>? FrameReceived;

        /// <summary>
        /// Sends a message over the stream.
        /// </summary>
        /// <param name="frame">Message to be sent</param>
        /// <exception cref="InvalidOperationException">The underlying stream does not support writing.</exception>
        /// <exception cref="ArgumentException">The message does not provide at least one segment, or one of its segments is empty.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /// <exception cref="ObjectDisposedException">This instance or stream is diposed.</exception>
        public void Send(WireFrame frame)
        {
            if (_writer == null)
                throw new InvalidOperationException("Stream is not writable");

            if (frame.Segments == null)
                throw new ArgumentException("Do not pass default(WireFrame)");

            if (frame.Segments.Count == 0)
                throw new ArgumentException("Expected at least one segment");
            
            foreach (var segment in frame.Segments)
            {
                if (segment.Length == 0)
                    throw new ArgumentException("Segment must not have zero length");
            }

            lock (_writeLock)
            {
                foreach (var tracer in _tracers)
                {
                    tracer.TraceFrame(FrameDirection.Tx, frame);
                }

                _writer.Write(frame.Segments.Count - 1);

                foreach (var segment in frame.Segments)
                {
                    _writer.Write(segment.Length);
                }

                if ((frame.Segments.Count & 1) == 0)
                {
                    // Padding
                    _writer.Write(0);
                }

                foreach (var segment in frame.Segments)
                {
#if NETSTANDARD2_0
                    var bytes = MemoryMarshal.Cast<ulong, byte>(segment.Span).ToArray();
#else
                    var bytes = MemoryMarshal.Cast<ulong, byte>(segment.Span);
#endif
                    _writer.Write(bytes);
                }
            }
        }

        /// <summary>
        /// Flushes all buffered frames.
        /// </summary>
        public void Flush()
        {
            if (Monitor.TryEnter(_writeLock))
            {
                try
                {
                    _writer?.Flush();
                }
                finally
                {
                    Monitor.Exit(_writeLock);
                }
            }
        }

        long _waitingForData;

        /// <summary>
        /// Whether the pump is currently waiting for data to receive.
        /// </summary>
        public bool IsWaitingForData
        {
            get => Interlocked.Read(ref _waitingForData) != 0;
            private set => Interlocked.Exchange(ref _waitingForData, value ? 1 : 0);
        }

        /// <summary>
        /// Synchronously runs the frame reception loop. Will only return after calling Dispose() or upon error condition.
        /// The method does not propagate EndOfStreamException or ObjectDisposedException to the caller, since these conditions are considered 
        /// to be part of normal operation. It does pass exceptions which arise due to I/O errors or invalid data.
        /// </summary>
        /// <exception cref="ArgumentException">The underlying stream does not support reading or is already closed.</exception>
        /// <exception cref="InvalidDataException">Encountered Invalid Framing Data</exception>
        /// <exception cref="OutOfMemoryException">Received a message with too many or too big segments, probably dues to invalid data.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public void Run()
        {
            try
            {
                using (var reader = new BinaryReader(_stream, Encoding.Default))
                {
                    while (true)
                    {
                        IsWaitingForData = true;
                        var frame = reader.ReadWireFrame();
                        IsWaitingForData = false;
                        foreach (var tracer in _tracers)
                        {
                            tracer.TraceFrame(FrameDirection.Rx, frame);
                        }
                        FrameReceived?.Invoke(frame);
                    }
                }
            }
            catch (Exception exception) when (exception is EndOfStreamException ||
                   exception is IOException ioex && 
                   ioex.InnerException is SocketException sockex && 
                   sockex.SocketErrorCode == SocketError.Interrupted)
            {
                Logger.LogInformation("Encountered end of stream");
            }
            catch (InvalidDataException e)
            {
                Logger.LogWarning(e.Message);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception exception)
            {
                // When disposing, all kinds of errors might happen here,
                // not worth logging.
                if (_disposing == 0)
                {
                    Logger.LogWarning(exception.Message);
                }
            }
            finally
            {
                IsWaitingForData = false;
            }
        }

        /// <summary>
        /// Attaches an observer for tracing RPC traffic
        /// </summary>
        /// <param name="tracer">observer implementation</param>
        public void AttachTracer(IFrameTracer tracer)
        {
            _tracers.Add(tracer);
        }
    }
}