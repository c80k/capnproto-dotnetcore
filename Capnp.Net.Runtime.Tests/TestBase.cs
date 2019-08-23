using Capnp.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Net.Runtime.Tests
{
    public class TestBase
    {
        public static int TcpPort = 49152;
        public static int MediumNonDbgTimeout => Debugger.IsAttached ? Timeout.Infinite : 5000;
        public static int LargeNonDbgTimeout => Debugger.IsAttached ? Timeout.Infinite : 20000;
        public static int ShortTimeout => 500;

        protected ILogger Logger { get; set; }

        protected TcpRpcClient SetupClient() => new TcpRpcClient("localhost", TcpPort);
        protected TcpRpcServer SetupServer() => new TcpRpcServer(IPAddress.Any, TcpPort);

        protected (TcpRpcServer, TcpRpcClient) SetupClientServerPair()
        {
            var server = SetupServer();
            var client = SetupClient();
            return (server, client);
        }

        public static void IncrementTcpPort()
        {
            if (++TcpPort > 49200)
            {
                TcpPort = 49152;
            }
        }

        [TestInitialize]
        public void InitConsoleLogging()
        {
            Logging.LoggerFactory?.Dispose();
            Logging.LoggerFactory = new LoggerFactory().AddConsole((msg, level) => true);
            Logger = Logging.CreateLogger<TcpRpcStress>();
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = $"Test Thread {Thread.CurrentThread.ManagedThreadId}";
        }

        /// <summary>
        /// Somewhat ugly helper method which ensures that both Tcp client and server
        /// are waiting for data reception from each other. This is a "balanced" state, meaning
        /// that nothing will ever happen in the RcpEngines without some other thread requesting
        /// anything.
        /// </summary>
        protected void WaitClientServerIdle(TcpRpcServer server, TcpRpcClient client)
        {
            var conn = server.Connections[0];
            SpinWait.SpinUntil(() => conn.IsWaitingForData && client.IsWaitingForData &&
                conn.RecvCount == client.SendCount &&
                conn.SendCount == client.RecvCount,
                MediumNonDbgTimeout);
        }

        protected void ExpectPromiseThrows(Task task)
        {
            async Task ExpectPromiseThrowsAsync(Task t)
            {
                try
                {
                    await t;
                    Assert.Fail("Did not throw");
                }
                catch (InvalidOperationException)
                {
                    // Happens if the call went to the resolution
                    // (thus, locally). In this case, the original
                    // exception is routed here.
                }
                catch (RpcException)
                {
                    // Happens if the call went to the promise
                    // (thus, remotely). In this case, the original
                    // exception had to be serialized, so we receive
                    // the wrapped version.
                }
                catch (System.Exception exception)
                {
                    Assert.Fail($"Got wrong kind of exception: {exception}");
                }
            }

            Assert.IsTrue(ExpectPromiseThrowsAsync(task).Wait(MediumNonDbgTimeout));
        }

    }
}
