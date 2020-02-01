using Capnp.Net.Runtime.Tests.GenImpls;
using Capnp.Rpc;
using Capnproto_test.Capnp.Test;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class TcpRpcStress: TestBase
    {
        void Repeat(int count, Action action)
        {
            for (int i = 0; i < count; i++)
            {
                Logger.LogTrace("Repetition {0}", i);
                IncrementTcpPort();
                action();
            }
        }

        [TestMethod]
        public void ResolveMain()
        {
            Repeat(5000, () =>
            {
                (var server, var client) = SetupClientServerPair();

                using (server)
                using (client)
                {
                    client.WhenConnected.Wait();

                    var counters = new Counters();
                    var impl = new TestMoreStuffImpl(counters);
                    server.Main = impl;
                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        var resolving = main as IResolvingCapability;
                        Assert.IsTrue(resolving.WhenResolved.Wait(MediumNonDbgTimeout));
                    }
                }
            });
        }

        [TestMethod]
        public void Cancel()
        {
            var t = new TcpRpcPorted();
            Repeat(1000, t.Cancel);
        }

        [TestMethod]
        public void Embargo()
        {
            var t = new TcpRpcPorted();
            Repeat(100, t.Embargo);

            var t2 = new TcpRpcInterop();
            Repeat(100, t2.EmbargoServer);
        }

        [TestMethod]
        public void EmbargoNull()
        {
            // Some code paths are really rare during this test, therefore increased repetition count.

            var t = new TcpRpcPorted();
            Repeat(1000, t.EmbargoNull);

            var t2 = new TcpRpcInterop();
            Repeat(100, t2.EmbargoNullServer);
        }

        [TestMethod]
        public void RetainAndRelease()
        {
            var t = new TcpRpcPorted();
            Repeat(100, t.RetainAndRelease);
        }

        [TestMethod]
        public void PipelineAfterReturn()
        {
            var t = new TcpRpc();
            Repeat(100, t.PipelineAfterReturn);
        }

        [TestMethod]
        public void ScatteredTransfer()
        {

            using (var server = new TcpRpcServer(IPAddress.Any, TcpPort))
            using (var client = new TcpRpcClient())
            {
                server.OnConnectionChanged += (_, e) =>
                {
                    if (e.Connection.State == ConnectionState.Initializing)
                    {
                        e.Connection.InjectMidlayer(s => new ScatteringStream(s, 7));
                    }
                };

                client.InjectMidlayer(s => new ScatteringStream(s, 10));
                client.Connect("localhost", TcpPort);
                client.WhenConnected.Wait();

                var counters = new Counters();
                server.Main = new TestInterfaceImpl(counters);
                using (var main = client.GetMain<ITestInterface>())
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var request1 = main.Foo(123, true, default);
                        var request3 = Assert.ThrowsExceptionAsync<RpcException>(() => main.Bar(default));
                        var s = new TestAllTypes();
                        Common.InitTestMessage(s);
                        var request2 = main.Baz(s, default);

                        Assert.IsTrue(request1.Wait(MediumNonDbgTimeout));
                        Assert.IsTrue(request2.Wait(MediumNonDbgTimeout));
                        Assert.IsTrue(request3.Wait(MediumNonDbgTimeout));

                        Assert.AreEqual("foo", request1.Result);
                        Assert.AreEqual(2, counters.CallCount);
                        counters.CallCount = 0;
                    }
                }
            }
        }
    }
}
