using Capnp.FrameTracing;
using System;
using System.IO;

namespace Capnp.Rpc
{
    /// <summary>
    /// Models an RPC connection.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Returns the state of this connection.
        /// </summary>
        ConnectionState State { get; }

        /// <summary>
        /// TCP port (local end), or null if the connection is not yet established.
        /// </summary>
        int? LocalPort { get; }

        /// <summary>
        /// TCP port (remote end), or null if the connection is not yet established.
        /// </summary>
        int? RemotePort { get; }

        /// <summary>
        /// Receive message counter
        /// </summary>
        long RecvCount { get; }

        /// <summary>
        /// Sent message counter
        /// </summary>
        long SendCount { get; }

        /// <summary>
        /// Whether the RPC engine is currently computing.
        /// </summary>
        bool IsComputing { get; }

        /// <summary>
        /// Whether the connection is idle, waiting for data to receive.
        /// </summary>
        bool IsWaitingForData { get; }

        /// <summary>
        /// Attaches a tracer to this connection. Only allowed in state 'Initializing'.
        /// </summary>
        /// <param name="tracer">Tracer to attach</param>
        /// <exception cref="ArgumentNullException"><paramref name="tracer"/> is null</exception>
        /// <exception cref="InvalidOperationException">Connection is not in state 'Initializing'</exception>
        void AttachTracer(IFrameTracer tracer);

        /// <summary>
        /// Installs a midlayer. A midlayer is a protocal layer that resides somewhere between capnp serialization and the raw TCP stream.
        /// Thus, we have a hook mechanism for transforming data before it is sent to the TCP connection or after it was received
        /// by the TCP connection, respectively. This mechanism may be used for integrating various (de-)compression algorithms.
        /// </summary>
        /// <param name="createFunc">Callback for wrapping the midlayer around its underlying stream</param>
        /// <exception cref="ArgumentNullException"><paramref name="createFunc"/> is null</exception>
        void InjectMidlayer(Func<Stream, Stream> createFunc);

        /// <summary>
        /// Prematurely closes this connection. Note that there is usually no need to close a connection manually. The typical use case
        /// of this method is to refuse an incoming connection in the <code>TcpRpcServer.OnConnectionChanged</code> callback.
        /// </summary>
        void Close();
    }
}