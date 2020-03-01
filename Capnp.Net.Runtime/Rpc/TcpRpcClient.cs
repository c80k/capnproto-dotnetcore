using Capnp.FrameTracing;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// TCP-based RPC implementation which will establish a connection to a TCP server implementing 
    /// the Cap'n Proto RPC protocol.
    /// </summary>
    public class TcpRpcClient: IConnection, IDisposable
    {
        ILogger Logger { get; } = Logging.CreateLogger<TcpRpcClient>();

        class OutboundTcpEndpoint : IEndpoint
        {
            readonly FramePump _pump;

            public OutboundTcpEndpoint(FramePump pump)
            {
                _pump = pump;
            }

            public void Dismiss()
            {
                _pump.Dispose();
            }

            public void Forward(WireFrame frame)
            {
                _pump.Send(frame);
            }
        }

        readonly RpcEngine _rpcEngine;
        readonly TcpClient _client;
        Func<Stream, Stream> _createLayers = _ => _;
        RpcEngine.RpcEndpoint? _inboundEndpoint;
        OutboundTcpEndpoint? _outboundEndpoint;
        FramePump? _pump;
        Thread? _pumpThread;
        Action? _attachTracerAction;

        /// <summary>
        /// Gets a Task which completes when TCP is connected. Will be
        /// null until connection is actually requested (either by calling Connect or using appropriate constructor).
        /// </summary>
        public Task? WhenConnected { get; private set; }

        async Task ConnectAsync(string host, int port)
        {
            for (int retry = 0; ; retry++)
            {
                try
                {
                    await _client.ConnectAsync(host, port);

                    return;
                }
                catch (SocketException exception) when (retry < 240 && exception.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    await Task.Delay(1000);
                }
                catch (SocketException exception)
                {
                    throw new RpcException("TcpRpcClient is unable to connect", exception);
                }
            }
        }

        async Task ConnectAndRunAsync(string host, int port)
        {
            await ConnectAsync(host, port);

            State = ConnectionState.Active;

            var stream = _createLayers(_client.GetStream());
            _pump = new FramePump(stream);
            _attachTracerAction?.Invoke();
            _outboundEndpoint = new OutboundTcpEndpoint(_pump);
            _inboundEndpoint = _rpcEngine.AddEndpoint(_outboundEndpoint);
            _pumpThread = new Thread(() =>
            {
                try
                {
                    Thread.CurrentThread.Name = $"TCP RPC Client Thread {Thread.CurrentThread.ManagedThreadId}";

                    _pump.Run();
                }
                finally
                {
                    State = ConnectionState.Down;
                    _outboundEndpoint.Dismiss();
                    _inboundEndpoint.Dismiss();
                    _pump.Dispose();
                }
            });

            _pump.FrameReceived += _inboundEndpoint.Forward;
            _pumpThread.Start();
        }

        /// <summary>
        /// Constructs an instance and attempts to connect it to given host.
        /// </summary>
        /// <param name="host">The DNS name of the remote RPC host</param>
        /// <param name="port">The port number of the remote RPC host</param>
        /// <exception cref="ArgumentNullException"><paramref name="host"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> is not between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort.</exception>
        /// <exception cref="System.Net.Sockets.SocketException">An error occurred when accessing the socket.</exception>
        public TcpRpcClient(string host, int port): this()
        {
            Connect(host, port);
        }

        /// <summary>
        /// Constructs an instance but does not yet attempt to connect.
        /// </summary>
        public TcpRpcClient()
        {
            _rpcEngine = new RpcEngine();
            _client = new TcpClient();
            _client.ExclusiveAddressUse = false;
        }

        /// <summary>
        /// Attempts to connect it to given host.
        /// </summary>
        /// <param name="host">The DNS name of the remote RPC host</param>
        /// <param name="port">The port number of the remote RPC host</param>
        /// <exception cref="ArgumentNullException"><paramref name="host"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> is not between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort.</exception>
        /// <exception cref="System.Net.Sockets.SocketException">An error occurred when accessing the socket.</exception>
        /// <exception cref="InvalidOperationException">Connection was already requested</exception>
        public void Connect(string host, int port)
        {
            if (WhenConnected != null)
                throw new InvalidOperationException("Connection was already requested");

            WhenConnected = ConnectAndRunAsync(host, port);
        }

        /// <summary>
        /// Returns the remote bootstrap capability.
        /// </summary>
        /// <typeparam name="TProxy">Bootstrap capability interface</typeparam>
        /// <returns>A proxy for the bootstrap capability</returns>
        /// <exception cref="InvalidOperationException">Not connected</exception>
        public TProxy GetMain<TProxy>() where TProxy: class
        {
            if (WhenConnected == null)
            {
                throw new InvalidOperationException("Not connecting");
            }

            if (!WhenConnected.IsCompleted)
            {
                throw new InvalidOperationException("Connection not yet established");
            }

            if (!WhenConnected.ReplacementTaskIsCompletedSuccessfully())
            {
                throw new InvalidOperationException("Connection not successfully established");
            }

            return (CapabilityReflection.CreateProxy<TProxy>(_inboundEndpoint!.QueryMain()) as TProxy)!;
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();

            try
            {
                if (WhenConnected != null && !WhenConnected.Wait(500))
                {
                    Logger.LogError("Unable to join connection task within timeout");
                }
            }
            catch (System.Exception e)
            {
                Logger.LogError(e, "Failure disposing client");
            }

            if (_pumpThread != null && !_pumpThread.Join(500))
            {
                Logger.LogError("Unable to join pump thread within timeout");
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Attaches a tracer to this connection. Only allowed in state 'Initializing'. To avoid race conditions, 
        /// this method should only be used  in conjunction with the parameterless constructor (no auto-connect).
        /// Call this method *before* calling Connect.
        /// </summary>
        /// <param name="tracer">Tracer to attach</param>
        /// <exception cref="ArgumentNullException"><paramref name="tracer"/> is null</exception>
        /// <exception cref="InvalidOperationException">Connection is not in state 'Initializing'</exception>
        public void AttachTracer(IFrameTracer tracer)
        {
            if (tracer == null)
                throw new ArgumentNullException(nameof(tracer));

            if (State != ConnectionState.Initializing)
                throw new InvalidOperationException("Connection is not in state 'Initializing'");

            _attachTracerAction += () =>
            {
                _pump?.AttachTracer(tracer);
            };
        }

        /// <summary>
        /// Installs a midlayer. A midlayer is a protocal layer that resides somewhere between capnp serialization and the raw TCP stream.
        /// Thus, we have a hook mechanism for transforming data before it is sent to the TCP connection or after it was received
        /// by the TCP connection, respectively. This mechanism may be used for integrating various (de-)compression algorithms.
        /// </summary>
        /// <param name="createFunc">Callback for wrapping the midlayer around its underlying stream</param>
        /// <exception cref="ArgumentNullException"><paramref name="createFunc"/> is null</exception>
        public void InjectMidlayer(Func<Stream, Stream> createFunc)
        {
            if (createFunc == null)
                throw new ArgumentNullException(nameof(createFunc));

            if (State != ConnectionState.Initializing)
                throw new InvalidOperationException("Connection is not in state 'Initializing'");

            var last = _createLayers;
            _createLayers = _ => createFunc(last(_));
        }

        /// <summary>
        /// Prematurely closes this connection. Note that there is usually no need to close a connection manually.
        /// </summary>
        void IConnection.Close()
        {
            _client.Dispose();
        }

        /// <summary>
        /// Returns the state of this connection.
        /// </summary>
        public ConnectionState State { get; private set; } = ConnectionState.Initializing;

        /// <summary>
        /// Gets the number of RPC protocol messages sent by this client so far.
        /// </summary>
        public long SendCount => _inboundEndpoint?.SendCount ?? 0;

        /// <summary>
        /// Gets the number of RPC protocol messages received by this client so far.
        /// </summary>
        public long RecvCount => _inboundEndpoint?.RecvCount ?? 0;

        /// <summary>
        /// Gets the remote port number which this client is connected to, 
        /// or null if the connection is not yet established.
        /// </summary>
        public int? RemotePort => ((IPEndPoint)_client.Client.RemoteEndPoint)?.Port;

        /// <summary>
        /// Gets the local port number which this client using, 
        /// or null if the connection is not yet established.
        /// </summary>
        public int? LocalPort => ((IPEndPoint)_client.Client.LocalEndPoint)?.Port;

        /// <summary>
        /// Whether the I/O thread is currently running
        /// </summary>
        public bool IsComputing => _pumpThread?.ThreadState == System.Threading.ThreadState.Running;

        /// <summary>
        /// Whether the I/O thread is waiting for data to receive
        /// </summary>
        public bool IsWaitingForData => _pump?.IsWaitingForData ?? false;
    }
}