using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// Cap'n Proto RPC TCP server.
    /// </summary>
    public class TcpRpcServer: IDisposable
    {
        public interface IConnection
        {
            int LocalPort { get; }
            long RecvCount { get; }
            long SendCount { get; }
            bool IsComputing { get; }
            bool IsWaitingForData { get; }
        }

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
            public Connection(TcpRpcServer server, TcpClient client, FramePump pump, OutboundTcpEndpoint outboundEp, RpcEngine.RpcEndpoint inboundEp)
            {
                Client = client;
                Pump = pump;
                OutboundEp = outboundEp;
                InboundEp = inboundEp;

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
                        lock (server._reentrancyBlocker)
                        {
                            --server.ConnectionCount;
                            server._connections.Remove(this);
                        }
                    }
                });
            }

            public TcpClient Client { get; private set; }
            public FramePump Pump { get; private set; }
            public OutboundTcpEndpoint OutboundEp { get; private set; }
            public RpcEngine.RpcEndpoint InboundEp { get; private set; }
            public Thread PumpRunner { get; private set; }
            public int LocalPort => ((IPEndPoint)Client.Client.LocalEndPoint).Port;
            public long RecvCount => InboundEp.RecvCount;
            public long SendCount => InboundEp.SendCount;
            public bool IsComputing => PumpRunner.ThreadState == ThreadState.Running;
            public bool IsWaitingForData => Pump.IsWaitingForData;
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
                    var pump = new FramePump(client.GetStream());
                    var outboundEndpoint = new OutboundTcpEndpoint(this, pump);
                    var inboundEndpoint = _rpcEngine.AddEndpoint(outboundEndpoint);
                    pump.FrameReceived += inboundEndpoint.Forward;

                    var connection = new Connection(this, client, pump, outboundEndpoint, inboundEndpoint);

                    lock (_reentrancyBlocker)
                    {
                        ++ConnectionCount;
                        _connections.Add(connection);
                    }

                    connection.PumpRunner.Start();
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

        /// <summary>
        /// Stops accepting incoming attempts and closes all existing connections.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _listener.Stop();
            }
            catch (SocketException)
            {
            }

            var connections = new List<Connection>();

            lock (_reentrancyBlocker)
            {
                connections.AddRange(_connections);
            }

            foreach (var connection in connections)
            {
                connection.Client.Dispose();
                connection.Pump.Dispose();
                if (!connection.PumpRunner.Join(500))
                {
                    Logger.LogError("Unable to join frame pumping thread within timeout");
                }
            }

            if (!_acceptorThread.Join(500))
            {
                Logger.LogError("Unable to join TCP acceptor thread within timeout");
            }

            GC.SuppressFinalize(this);
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
            _listener.Start();

            _acceptorThread = new Thread(() =>
            {
                AcceptClients();
            });

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
    }
}
