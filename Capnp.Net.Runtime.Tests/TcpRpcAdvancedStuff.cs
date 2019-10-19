using Capnp.Net.Runtime.Tests.GenImpls;
using Capnp.Rpc;
using Capnproto_test.Capnp.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class TcpRpcAdvancedStuff : TestBase
    {
        [TestMethod, Timeout(10000)]
        public void MultiConnect()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                var tcs = new TaskCompletionSource<int>();
                server.Main = new TestInterfaceImpl(counters, tcs);

                for (int i = 1; i <= 10; i++)
                {
                    using (var client = SetupClient())
                    {
                        client.WhenConnected.Wait();

                        using (var main = client.GetMain<ITestInterface>())
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
                            Assert.AreEqual(2 * i, counters.CallCount);
                        }
                    }

                    // Bootstrap capability must not be disposed
                    Assert.IsFalse(tcs.Task.IsCompleted);
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void TwoClients()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                using (var client1 = SetupClient())
                using (var client2 = SetupClient())
                {
                    Assert.IsTrue(client1.WhenConnected.Wait(MediumNonDbgTimeout));
                    Assert.IsTrue(client2.WhenConnected.Wait(MediumNonDbgTimeout));

                    using (var main = client1.GetMain<ITestMoreStuff>())
                    {
                        Assert.IsTrue(main.Hold(new TestInterfaceImpl(counters)).Wait(MediumNonDbgTimeout));
                    }

                    using (var main = client2.GetMain<ITestMoreStuff>())
                    {
                        Assert.IsTrue(main.CallHeld().Wait(MediumNonDbgTimeout));
                        var getHeld = main.GetHeld();
                        Assert.IsTrue(getHeld.Wait(MediumNonDbgTimeout));
                        var foo = getHeld.Result.Foo(123, true);
                        Assert.IsTrue(foo.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual("foo", foo.Result);
                    }

                    client1.Dispose();

                    using (var main = client2.GetMain<ITestMoreStuff>())
                    {
                        ExpectPromiseThrows(main.CallHeld());
                    }
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void ClosingServerWhileRequestingBootstrap()
        {
            for (int i = 0; i < 100; i++)
            {
                var server = SetupServer();
                var counters = new Counters();
                var tcs = new TaskCompletionSource<int>();
                server.Main = new TestInterfaceImpl(counters, tcs);

                using (var client = SetupClient())
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestInterface>())
                    {
                        server.Dispose();

                        // Resolution must either succeed or be cancelled. A hanging resolution would be inacceptable.

                        try
                        {
                            Assert.IsTrue(((IResolvingCapability)main).WhenResolved.Wait(MediumNonDbgTimeout));
                        }
                        catch (AggregateException)
                        {
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void InheritFromGenericInterface()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new B2Impl();

                using (var client = SetupClient())
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<CapnpGen.IB2>())
                    {
                        Assert.IsTrue(main.MethodA("42").Wait(MediumNonDbgTimeout));
                        var b = main.MethodB(123);
                        Assert.IsTrue(b.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual("42", b.Result);
                    }
                }
            }
        }
    }
}
