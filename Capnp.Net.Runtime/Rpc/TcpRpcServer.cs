using Capnp.FrameTracing;
using Capnp.Util;
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
    public class TcpRpcServer: ISupportsMidlayers, IDisposable
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

            public void Flush()
            {
                _pump.Flush();
            }
        }

        class Connection: IConnection
        {
            ILogger Logger { get; } = Logging.CreateLogger<Connection>();

            readonly List<IFrameTracer> _tracers = new List<IFrameTracer>();
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

                foreach (var tracer in _tracers)
                {
                    Pump.AttachTracer(tracer);
                }
                _tracers.Clear();

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
                    catch (ThreadInterruptedException)
                    {
                        Logger.LogError($"{Thread.CurrentThread.Name} interrupted at {Environment.StackTrace}");
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
                PumpRunner.Start();
            }

            public ConnectionState State { get; set; } = ConnectionState.Initializing;
            public TcpClient Client { get; private set; }
            public FramePump? Pump { get; private set; }
            public OutboundTcpEndpoint? OutboundEp { get; private set; }
            public RpcEngine.RpcEndpoint? InboundEp { get; private set; }
            public Thread? PumpRunner { get; private set; }
            public int? LocalPort => ((IPEndPoint)Client.Client.LocalEndPoint)?.Port;
            public int? RemotePort => ((IPEndPoint)Client.Client.RemoteEndPoint)?.Port;
            public long RecvCount => InboundEp?.RecvCount ?? 0;
            public long SendCount => InboundEp?.SendCount ?? 0;
            public bool IsComputing => PumpRunner?.ThreadState == ThreadState.Running;
            public bool IsWaitingForData => Pump?.IsWaitingForData ?? false;

            public void AttachTracer(IFrameTracer tracer)
            {
                if (tracer == null)
                    throw new ArgumentNullException(nameof(tracer));

                if (State != ConnectionState.Initializing)
                    throw new InvalidOperationException("Connection is not in state 'Initializing'");

                _tracers.Add(tracer);
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
        readonly object _reentrancyBlocker = new object();
        readonly List<Connection> _connections = new List<Connection>();
        Thread? _acceptorThread;
        TcpListener? _listener;

        /// <summary>
        /// Gets the number of currently active inbound TCP connections.
        /// </summary>
        public int ConnectionCount { get; private set; }

        void AcceptClients(TcpListener listener)
        {
            try
            {
                if (Thread.CurrentThread.Name == null)
                    Thread.CurrentThread.Name = $"TCP RPC Acceptor Thread {Thread.CurrentThread.ManagedThreadId}";

                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    var connection = new Connection(this, client);

                    lock (_reentrancyBlocker)
                    {
                        ++ConnectionCount;
                        _connections.Add(connection);

                        OnConnectionChanged?.Invoke(this, new ConnectionEventArgs(connection));
                        connection.Start();
                    }
                }
            }
            catch (SocketException)
            {
                // Listener was stopped. Maybe a little bit rude, but this is
                // our way of shutting down the acceptor thread.
            }
            catch (ThreadInterruptedException)
            {
                Logger.LogError($"{Thread.CurrentThread.Name} interrupted at {Environment.StackTrace}");
            }
            catch (System.Exception exception)
            {
                // Any other exception might be due to some other problem.
                Logger.LogError(exception.Message);
            }
        }

        /// <summary>
        /// Stops accepting incoming attempts and closes all existing connections.
        /// </summary>
        public void Dispose()
        {
            if (_listener != null)
            {
                StopListening();
            }

            var connections = new List<Connection>();

            lock (_reentrancyBlocker)
            {
                connections.AddRange(_connections);
            }

            foreach (var connection in connections)
            {
                connection.Client.Dispose();
                connection.Pump?.Dispose();
                connection.PumpRunner?.SafeJoin(Logger);
            }

            _rpcEngine.BootstrapCap = null;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Stops accepting incoming attempts.
        /// </summary>
        public void StopListening()
        {
            if (_listener == null)
                throw new InvalidOperationException("Listening was never started");

            try
            {
                _listener.Stop();
            }
            catch (SocketException)
            {
            }
            finally
            {
                _listener = null;
                if (Thread.CurrentThread != _acceptorThread)
                    _acceptorThread?.Join();
                _acceptorThread = null;
            }
        }

        /// <summary>
        /// Installs a midlayer.
        /// A midlayer is a protocal layer that resides somewhere between capnp serialization and the raw TCP stream.
        /// Thus, we have a hook mechanism for transforming data before it is sent to the TCP connection or after it was received
        /// by the TCP connection, respectively. This mechanism can be used for buffering, various (de-)compression algorithms, and more.
        /// </summary>
        /// <param name="createFunc"></param>
        public void InjectMidlayer(Func<Stream, Stream> createFunc)
        {
            OnConnectionChanged += (_, e) =>
            {
                if (e.Connection.State == ConnectionState.Initializing)
                {
                    e.Connection.InjectMidlayer(createFunc);
                }
            };
        }

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        public TcpRpcServer()
        {
            _rpcEngine = new RpcEngine();

        }

        /// <summary>
        /// Constructs an instance, starts listening to the specified TCP/IP endpoint and accepting clients.
        /// If you intend configuring a midlayer or consuming the <see cref="OnConnectionChanged"/> event, 
        /// you should not use this constructor, since it may lead to an early-client race condition.
        /// Instead, use the parameterless constructor, configure, then call <see cref="StartAccepting(IPAddress, int)"/>.
        /// </summary>
        /// <param name="localAddr">An System.Net.IPAddress that represents the local IP address.</param>
        /// <param name="port">The port on which to listen for incoming connection attempts.</param>
        /// <exception cref="ArgumentNullException"><paramref name="localAddr"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> is not between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort.</exception>
        /// <exception cref="SocketException">The underlying <see cref="TcpListener"/> detected an error condition, such as the desired endpoint is already occupied.</exception>
        public TcpRpcServer(IPAddress localAddr, int port): this()
        {
            StartAccepting(localAddr, port);
        }

        /// <summary>
        /// Starts listening to the specified TCP/IP endpoint and accepting clients. 
        /// </summary>
        /// <param name="localAddr">An System.Net.IPAddress that represents the local IP address.</param>
        /// <param name="port">The port on which to listen for incoming connection attempts.</param>
        /// <exception cref="ArgumentNullException"><paramref name="localAddr"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> is not between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort.</exception>
        /// <exception cref="InvalidOperationException">Listening activity was already started</exception>
        /// <exception cref="SocketException">The underlying <see cref="TcpListener"/> detected an error condition, such as the desired endpoint is already occupied.</exception>
        public void StartAccepting(IPAddress localAddr, int port)
        {
            if (_listener != null)
                throw new InvalidOperationException("Listening activity was already started");

            var listener = new TcpListener(localAddr, port)
            {
                ExclusiveAddressUse = false
            };

            int attempt = 0;

            while (true)
            {
                try
                {
                    listener.Start();
                    break;
                }
                catch (SocketException socketException)
                {
                    if (attempt == 5)
                        throw;

                    Logger.LogWarning($"Failed to listen on port {port}, attempt {attempt}: {socketException}");
                }

                ++attempt;
                Thread.Sleep(10);
            }

            _acceptorThread = new Thread(() => AcceptClients(listener));
            _listener = listener;
            _acceptorThread.Start();
        }

        /// <summary>
        /// Whether the thread which is responsible for acception incoming attempts is still alive.
        /// The thread will die after calling <see cref="StopListening"/>, upon disposal, but also in case of a socket error condition.
        /// Errors which occur on a particular connection will just close that connection and won't interfere
        /// with the acceptor thread.
        /// </summary>
        public bool IsAlive => _acceptorThread?.IsAlive ?? false;

        /// <summary>
        /// Sets the bootstrap capability. It must be an object which implements a valid capability interface
        /// (<see cref="SkeletonAttribute"/>).
        /// </summary>
        public object Main
        {
            set { _rpcEngine.Main = value; }
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