using Capnp.FrameTracing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Capnp.Rpc
{
    /// <summary>
    /// Carries information on RPC connection state changes.
    /// </summary>
    public class ConnectionEventArgs: EventArgs
    {
        /// <summary>
        /// Affected connection
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Constructs an instance
        /// </summary>
        /// <param name="connection">RPC connection object</param>
        public ConnectionEventArgs(IConnection connection)
        {
            Connection = connection;
        }
    }

    /// <summary>
    /// Cap'n Proto RPC TCP server.
    /// </summary>
    public class TcpRpcServer: IDisposable
    {
        ILogger Logger { get; } = Logging.CreateLogger<TcpRpcServer>();

        class OutboundTcpEndpoint : IEndpoint
        {
            readonly TcpRpcServer _server;
            readonly FramePump _pump;

            public OutboundTcpEndpoint(TcpRpcServer server, FramePump pump)
            {
                _server = server;
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

        class Connection: IConnection
        {
            readonly TcpRpcServer _server;
            Stream _stream;

            public Connection(TcpRpcServer server, TcpClient client)
            {
                _server = server;
                Client = client;
                _stream = client.GetStream();
            }

            public void Start()
            {
                Pump = new FramePump(_stream);
                OutboundEp = new OutboundTcpEndpoint(_server, Pump);
                InboundEp = _server._rpcEngine.AddEndpoint(OutboundEp);
                Pump.FrameReceived += InboundEp.Forward;

                State = ConnectionState.Active;

                PumpRunner = new Thread(o =>
                {
                    try
                    {
                        Thread.CurrentThread.Name = $"TCP RPC Server Thread {Thread.CurrentThread.ManagedThreadId}";

                        Pump.Run();
                    }
                    finally
                    {
                        OutboundEp.Dismiss();
                        InboundEp.Dismiss();
                        Pump.Dispose();
                        Client.Dispose();
                        lock (_server._reentrancyBlocker)
                        {
                            --_server.ConnectionCount;
                            _server._connections.Remove(this);
                            State = ConnectionState.Down;
                            _server.OnConnectionChanged?.Invoke(_server, new ConnectionEventArgs(this));
                        }
                    }
                });
            }

            public ConnectionState State { get; set; } = ConnectionState.Initializing;
            public TcpClient Client { get; private set; }
            public FramePump Pump { get; private set; }
            public OutboundTcpEndpoint OutboundEp { get; private set; }
            public RpcEngine.RpcEndpoint InboundEp { get; private set; }
            public Thread? PumpRunner { get; private set; }
            public int? LocalPort => ((IPEndPoint)Client.Client.LocalEndPoint)?.Port;
            public int? RemotePort => ((IPEndPoint)Client.Client.RemoteEndPoint)?.Port;
            public long RecvCount => InboundEp.RecvCount;
            public long SendCount => InboundEp.SendCount;
            public bool IsComputing => PumpRunner?.ThreadState == ThreadState.Running;
            public bool IsWaitingForData => Pump.IsWaitingForData;

            public void AttachTracer(IFrameTracer tracer)
            {
                if (tracer == null)
                    throw new ArgumentNullException(nameof(tracer));

                if (State != ConnectionState.Initializing)
                    throw new InvalidOperationException("Connection is not in state 'Initializing'");

                Pump.AttachTracer(tracer);
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

                _stream = createFunc(_stream);
            }

            public void Close()
            {
                Client.Dispose();
            }
        }

        readonly RpcEngine _rpcEngine;
        readonly TcpListener _listener;
        readonly object _reentrancyBlocker = new object();
        readonly Thread _acceptorThread;
        readonly List<Connection> _connections = new List<Connection>();

        /// <summary>
        /// Gets the number of currently active inbound TCP connections.
        /// </summary>
        public int ConnectionCount { get; private set; }

        void AcceptClients()
        {
            try
            {
                if (Thread.CurrentThread.Name == null)
                    Thread.CurrentThread.Name = $"TCP RPC Acceptor Thread {Thread.CurrentThread.ManagedThreadId}";

                while (true)
                {
                    var client = _listener.AcceptTcpClient();
                    var connection = new Connection(this, client);

                    lock (_reentrancyBlocker)
                    {
                        ++ConnectionCount;
                        _connections.Add(connection);

                        OnConnectionChanged?.Invoke(this, new ConnectionEventArgs(connection));
                        connection.Start();
                    }

                    connection.PumpRunner!.Start();
                }
            }
            catch (SocketException)
            {
                // Listener was stopped. Maybe a little bit rude, but this is
                // our way of shutting down the acceptor thread.
            }
            catch (System.Exception exception)
            {
                // Any other exception might be due to some other problem.
                Logger.LogError(exception.Message);
            }
        }

        void SafeJoin(Thread? thread)
        {
            if (thread == null)
            {
                return;
            }

            for (int retry = 0; retry < 5; ++retry)
            {
                try
                {
                    if (!thread.Join(500))
                    {
                        Logger.LogError($"Unable to join {thread.Name} within timeout");
                    }
                    break;
                }
                catch (ThreadStateException)
                {
                    // In rare cases it happens that despite thread.Start() was called, the thread did not actually start yet.
                    Logger.LogDebug("Waiting for thread to start in order to join it");
                    Thread.Sleep(100);
                }
                catch (System.Exception exception)
                {
                    Logger.LogError($"Unable to join {thread.Name}: {exception.Message}");
                    break;
                }
            }
        }


        /// <summary>
        /// Stops accepting incoming attempts and closes all existing connections.
        /// </summary>
        public void Dispose()
        {
            StopListening();

            var connections = new List<Connection>();

            lock (_reentrancyBlocker)
            {
                connections.AddRange(_connections);
            }

            foreach (var connection in connections)
            {
                connection.Client.Dispose();
                connection.Pump.Dispose();
                SafeJoin(connection.PumpRunner);
            }

            SafeJoin(_acceptorThread);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Stops accepting incoming attempts.
        /// </summary>
        public void StopListening()
        {
            try
            {
                _listener.Stop();
            }
            catch (SocketException)
            {
            }
        }

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="localAddr">An System.Net.IPAddress that represents the local IP address.</param>
        /// <param name="port">The port on which to listen for incoming connection attempts.</param>
        /// <exception cref="ArgumentNullException"><paramref name="localAddr"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> is not between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort.</exception>
        public TcpRpcServer(IPAddress localAddr, int port)
        {
            _rpcEngine = new RpcEngine();
            _listener = new TcpListener(localAddr, port);
            _listener.ExclusiveAddressUse = false;

            for (int retry = 0; retry < 5; retry++)
            {
                try
                {
                    _listener.Start();
                    break;
                }
                catch (SocketException socketException)
                {
                    Logger.LogWarning($"Failed to listen on port {port}, attempt {retry}: {socketException}");
                    Thread.Sleep(10);
                }
            }

            _acceptorThread = new Thread(AcceptClients);

            _acceptorThread.Start();
        }

        /// <summary>
        /// Whether the thread which is responsible for acception incoming attempts is still alive.
        /// The thread will die upon disposal, but also in case of a socket error condition.
        /// Errors which occur on a particular connection will just close that connection and won't interfere
        /// with the acceptor thread.
        /// </summary>
        public bool IsAlive => _acceptorThread.IsAlive;

        /// <summary>
        /// Sets the bootstrap capability. It must be an object which implements a valid capability interface
        /// (<see cref="SkeletonAttribute"/>).
        /// </summary>
        public object Main
        {
            set { _rpcEngine.BootstrapCap = Skeleton.GetOrCreateSkeleton(value, false); }
        }

        /// <summary>
        /// Gets a snapshot of currently active connections.
        /// </summary>
        public IConnection[] Connections
        {
            get
            {
                lock (_reentrancyBlocker)
                {
                    return _connections.ToArray();
                }
            }
        }

        /// <summary>
        /// Fires when a new incoming connection was accepted, or when an active connection is closed.
        /// </summary>
        public event Action<object, ConnectionEventArgs>? OnConnectionChanged;
    }
}