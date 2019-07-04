using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    /// <summary>
    /// TCP-based RPC implementation which will establish a connection to a TCP server implementing 
    /// the Cap'n Proto RPC protocol.
    /// </summary>
    public class TcpRpcClient: IDisposable
    {
        ILogger Logger { get; } = Logging.CreateLogger<TcpRpcClient>();

        class OutboundTcpEndpoint : IEndpoint
        {
            readonly TcpRpcClient _client;
            readonly FramePump _pump;

            public OutboundTcpEndpoint(TcpRpcClient client, FramePump pump)
            {
                _client = client;
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
        RpcEngine.RpcEndpoint _inboundEndpoint;
        OutboundTcpEndpoint _outboundEndpoint;
        FramePump _pump;
        Thread _pumpThread;

        /// <summary>
        /// Gets a Task which completes when TCP is connected.
        /// </summary>
        public Task WhenConnected { get; }

        async Task ConnectAsync(string host, int port)
        {
            try
            {
                await _client.ConnectAsync(host, port);
            }
            catch (SocketException exception)
            {
                throw new RpcException("TcpRpcClient is unable to connect", exception);
            }
        }

        async Task Connect(string host, int port)
        {
            await ConnectAsync(host, port);

            _pump = new FramePump(_client.GetStream());
            _outboundEndpoint = new OutboundTcpEndpoint(this, _pump);
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
        public TcpRpcClient(string host, int port)
        {
            _rpcEngine = new RpcEngine();
            _client = new TcpClient();
            _client.ExclusiveAddressUse = false;

            WhenConnected = Connect(host, port);
        }

        /// <summary>
        /// Returns the remote bootstrap capability.
        /// </summary>
        /// <typeparam name="TProxy">Bootstrap capability interface</typeparam>
        /// <returns>A proxy for the bootstrap capability</returns>
        public TProxy GetMain<TProxy>() where TProxy: class
        {
            if (!WhenConnected.IsCompleted)
            {
                throw new InvalidOperationException("Connection not yet established");
            }

            if (!WhenConnected.ReplacementTaskIsCompletedSuccessfully())
            {
                throw new InvalidOperationException("Connection not successfully established");
            }

            Debug.Assert(_inboundEndpoint != null);

            return CapabilityReflection.CreateProxy<TProxy>(_inboundEndpoint.QueryMain()) as TProxy;
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();

            try
            {
                if (!WhenConnected.Wait(500))
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
        /// Gets the number of RPC protocol messages sent by this client so far.
        /// </summary>
        public long SendCount => _inboundEndpoint.SendCount;

        /// <summary>
        /// Gets the number of RPC protocol messages received by this client so far.
        /// </summary>
        public long RecvCount => _inboundEndpoint.RecvCount;

        /// <summary>
        /// Gets the remote port number which this client is connected to, 
        /// or null if the connection is not yet established.
        /// </summary>
        public int? RemotePort => ((IPEndPoint)_client.Client?.RemoteEndPoint)?.Port;

        /// <summary>
        /// Whether the I/O thread is currently running
        /// </summary>
        public bool IsComputing => _pumpThread.ThreadState == System.Threading.ThreadState.Running;

        /// <summary>
        /// Whether the I/O thread is waiting for data to receive
        /// </summary>
        public bool IsWaitingForData => _pump.IsWaitingForData;
    }
}
