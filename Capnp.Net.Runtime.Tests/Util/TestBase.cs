using Capnp.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Net.Runtime.Tests
{
    public interface ITestbed
    {
        T ConnectMain<T>(object main) where T : class, IDisposable;
        void MustComplete(params Task[] tasks);
        void MustNotComplete(params Task[] tasks);
        void FlushCommunication();

        long ClientSendCount { get; }
    }

    public interface ITestController
    {
        void RunTest(Action<ITestbed> action);
    }

    public class TestBase
    {

        protected class EnginePair
        {
            class EngineChannel : IEndpoint
            {
                readonly Queue<WireFrame> _frameBuffer = new Queue<WireFrame>();
                readonly Func<bool> _decide;
                bool _recursion;
                bool _dismissed;

                public EngineChannel(Func<bool> decide)
                {
                    _decide = decide;
                }

                public RpcEngine.RpcEndpoint OtherEndpoint { get; set; }
                public bool HasBufferedFrames => _frameBuffer.Count > 0;
                public int FrameCounter { get; private set; }


                public void Dismiss()
                {
                    if (!_dismissed)
                    {
                        _dismissed = true;
                        OtherEndpoint.Dismiss();
                    }
                }

                public void Forward(WireFrame frame)
                {
                    if (_dismissed)
                        return;

                    ++FrameCounter;

                    if (_recursion || _frameBuffer.Count > 0 || _decide())
                    {
                        _frameBuffer.Enqueue(frame);
                    }
                    else
                    {
                        _recursion = true;
                        OtherEndpoint.Forward(frame);
                        _recursion = false;
                    }
                }

                public bool Flush()
                {
                    if (_frameBuffer.Count > 0)
                    {
                        var frame = _frameBuffer.Dequeue();
                        _recursion = true;
                        OtherEndpoint.Forward(frame);
                        _recursion = false;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            readonly DecisionTree _decisionTree;
            readonly EngineChannel _channel1, _channel2;

            public RpcEngine Engine1 { get; }
            public RpcEngine Engine2 { get; }
            public RpcEngine.RpcEndpoint Endpoint1 { get; }
            public RpcEngine.RpcEndpoint Endpoint2 { get; }

            public EnginePair(DecisionTree decisionTree)
            {
                _decisionTree = decisionTree;
                Engine1 = new RpcEngine();
                Engine2 = new RpcEngine();
                _channel1 = new EngineChannel(decisionTree.MakeDecision);
                Endpoint1 = Engine1.AddEndpoint(_channel1);
                _channel2 = new EngineChannel(() => false);
                Endpoint2 = Engine2.AddEndpoint(_channel2);
                _channel1.OtherEndpoint = Endpoint2;
                _channel2.OtherEndpoint = Endpoint1;
            }

            public void FlushChannels(Func<bool> pred)
            {
                while (!pred())
                {
                    if (!_channel1.Flush() &&
                        !_channel2.Flush())
                        return;
                }

                while ((_channel1.HasBufferedFrames || _channel2.HasBufferedFrames) && _decisionTree.MakeDecision())
                {
                    if (_channel1.HasBufferedFrames)
                    {
                        int mark = _channel2.FrameCounter;

                        while (_channel1.Flush() && _channel2.FrameCounter == mark)
                            ;
                    }
                    else if (_channel2.HasBufferedFrames)
                    {
                        int mark = _channel1.FrameCounter;

                        while (_channel2.Flush() && _channel1.FrameCounter == mark)
                            ;
                    }

                }
            }

            public int Channel1SendCount => _channel1.FrameCounter;
            public int Channel2SendCount => _channel2.FrameCounter;
        }

        protected class LocalTestbed : ITestbed, ITestController
        {
            long ITestbed.ClientSendCount => 0;

            public void RunTest(Action<ITestbed> action)
            {
                action(this);
            }

            T ITestbed.ConnectMain<T>(object main)
            {
                return Proxy.Share<T>((T)main);
            }

            void ITestbed.FlushCommunication()
            {
            }

            void ITestbed.MustComplete(params Task[] tasks)
            {
                Assert.IsTrue(tasks.All(t => t.IsCompleted));
            }

            void ITestbed.MustNotComplete(params Task[] tasks)
            {
                Assert.IsFalse(tasks.Any(t => t.IsCompleted));
            }
        }

        protected class DtbdctTestbed : ITestbed, ITestController
        {
            readonly DecisionTree _decisionTree = new DecisionTree();
            EnginePair _enginePair;

            public void RunTest(Action<ITestbed> action)
            {
                _decisionTree.Iterate(() => {
                    
                    action(this);
                    _enginePair.FlushChannels(() => false);
                    
                    Assert.AreEqual(0, _enginePair.Endpoint1.ExportedCapabilityCount);
                    Assert.AreEqual(0, _enginePair.Endpoint1.ImportedCapabilityCount);
                    Assert.AreEqual(0, _enginePair.Endpoint1.PendingQuestionCount);
                    Assert.AreEqual(0, _enginePair.Endpoint1.PendingAnswerCount);

                    Assert.AreEqual(0, _enginePair.Endpoint2.ExportedCapabilityCount);
                    Assert.AreEqual(0, _enginePair.Endpoint2.ImportedCapabilityCount);
                    Assert.AreEqual(0, _enginePair.Endpoint2.PendingQuestionCount);
                    Assert.AreEqual(0, _enginePair.Endpoint2.PendingAnswerCount);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                });
            }

            T ITestbed.ConnectMain<T>(object main)
            {
                return SetupEnginePair<T>(main, _decisionTree, out _enginePair);
            }

            void ITestbed.MustComplete(params Task[] tasks)
            {
                bool AllDone() => tasks.All(t => t.IsCompleted);
                _enginePair.FlushChannels(AllDone);
                _decisionTree.Freeze();
                Assert.IsTrue(AllDone());
            }

            void ITestbed.MustNotComplete(params Task[] tasks)
            {
                Assert.IsFalse(tasks.Any(t => t.IsCompleted));
            }

            void ITestbed.FlushCommunication()
            {
                _enginePair.FlushChannels(() => false);
            }

            long ITestbed.ClientSendCount => _enginePair.Channel2SendCount;
        }

        protected class LocalhostTcpTestbed : ITestbed, ITestController
        {
            TcpRpcServer _server;
            TcpRpcClient _client;

            public void RunTest(Action<ITestbed> action)
            {
                (_server, _client) = SetupClientServerPair();
                //_client.WhenConnected.Wait(MediumNonDbgTimeout);
                Assert.IsTrue(SpinWait.SpinUntil(() => _server.ConnectionCount > 0, MediumNonDbgTimeout));
                var conn = _server.Connections[0];

                using (_server)
                using (_client)
                {
                    action(this);

                    Assert.IsTrue(SpinWait.SpinUntil(() => _client.SendCount == conn.RecvCount, MediumNonDbgTimeout));
                    Assert.IsTrue(SpinWait.SpinUntil(() => conn.SendCount == _client.RecvCount, MediumNonDbgTimeout));
                }
            }

            T ITestbed.ConnectMain<T>(object main)
            {
                _server.Main = main;
                return _client.GetMain<T>();
            }

            static Task[] GulpExceptions(Task[] tasks)
            {
                async Task Gulp(Task t)
                {
                    try
                    {
                        await t;
                    }
                    catch
                    {
                    }
                }

                return tasks.Select(Gulp).ToArray();
            }

            void ITestbed.MustComplete(params Task[] tasks)
            {
                Assert.IsTrue(Task.WaitAll(GulpExceptions(tasks), MediumNonDbgTimeout));
            }

            void ITestbed.MustNotComplete(params Task[] tasks)
            {
                Assert.AreEqual(-1, Task.WaitAny(GulpExceptions(tasks), ShortTimeout));
            }

            void ITestbed.FlushCommunication()
            {
                WaitClientServerIdle(_server, _client);
            }

            long ITestbed.ClientSendCount => _client.SendCount;
        }

        public static int TcpPort = 49152;
        public static int MediumNonDbgTimeout => Debugger.IsAttached ? Timeout.Infinite : 5000;
        public static int LargeNonDbgTimeout => Debugger.IsAttached ? Timeout.Infinite : 20000;
        public static int ShortTimeout => 500;

        protected ILogger Logger { get; set; }

        protected static TcpRpcClient SetupClient()
        {
            var client = new TcpRpcClient();
            client.AddBuffering();
            client.Connect("localhost", TcpPort);
            return client;
        }

        protected static TcpRpcServer SetupServer()
        {
            int attempt = 0;

            while (true)
            {
                try
                {
                    var server = new TcpRpcServer(IPAddress.Any, TcpPort);
                    server.AddBuffering();
                    return server;
                }
                catch (SocketException)
                {
                    // If the TCP listening port is occupied by some other process, retry with a different one
                    // TIME_WAIT is 4min: http://blog.davidvassallo.me/2010/07/13/time_wait-and-port-reuse/
                    if (attempt == 2400)
                        throw;
                }

                IncrementTcpPort();
                ++attempt;
                Thread.Sleep(100);
            }
        }

        protected static (TcpRpcServer, TcpRpcClient) SetupClientServerPair()
        {
            var server = SetupServer();
            var client = SetupClient();
            return (server, client);
        }

        protected static T SetupEnginePair<T>(object main, DecisionTree decisionTree, out EnginePair pair) where T: class
        {
            pair = new EnginePair(decisionTree);
            pair.Engine1.Main = main;
            return (CapabilityReflection.CreateProxy<T>(pair.Endpoint2.QueryMain()) as T);
        }

        public static void IncrementTcpPort()
        {
            if (++TcpPort > 49200)
            {
                TcpPort = 49152;
            }
        }

        protected static DtbdctTestbed NewDtbdctTestbed() => new DtbdctTestbed();
        protected static LocalhostTcpTestbed NewLocalhostTcpTestbed() => new LocalhostTcpTestbed();

        protected static LocalTestbed NewLocalTestbed() => new LocalTestbed();

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
        static protected void WaitClientServerIdle(TcpRpcServer server, TcpRpcClient client)
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
