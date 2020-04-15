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
    [TestCategory("Coverage")]
    public class TcpRpcAdvancedStuff : TestBase
    {
        [TestMethod]
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
                        //client.WhenConnected.Wait();

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

        [TestMethod]
        public void TwoClients()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                using (var client1 = SetupClient())
                using (var client2 = SetupClient())
                {
                    //Assert.IsTrue(client1.WhenConnected.Wait(MediumNonDbgTimeout));
                    //Assert.IsTrue(client2.WhenConnected.Wait(MediumNonDbgTimeout));

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

        [TestMethod]
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
                    //client.WhenConnected.Wait();

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

        [TestMethod]
        public void Issue25()
        {
            using (var server = SetupServer())
            {
                server.Main = new Issue25BImpl();

                using (var client = SetupClient())
                {
                    //client.WhenConnected.Wait();

                    using (var main = client.GetMain<CapnpGen.IIssue25B>())
                    {
                        var capholderAPT = main.GetAinCapHolderAnyPointer();
                        Assert.IsTrue(capholderAPT.Wait(MediumNonDbgTimeout));
                        var capholderAP = capholderAPT.Result;
                        var capAPT = capholderAP.Cap();
                        Assert.IsTrue(capAPT.Wait(MediumNonDbgTimeout));
                        using (var a = ((BareProxy)capAPT.Result).Cast<CapnpGen.IIssue25A>(true))
                        {
                            var r = a.MethodA();
                            Assert.IsTrue(r.Wait(MediumNonDbgTimeout));
                            Assert.AreEqual(123L, r.Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ExportCapToThirdParty()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl3();

                using (var client = SetupClient())
                {
                    //Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        var held = main.GetHeld().Eager();

                        using (var server2 = SetupServer())
                        {
                            server2.Main = new TestMoreStuffImpl2();

                            using (var client2 = SetupClient())
                            {
                                //Assert.IsTrue(client2.WhenConnected.Wait(MediumNonDbgTimeout));

                                using (var main2 = client2.GetMain<ITestMoreStuff>())
                                {
                                    var fooTask = main2.CallFoo(held);
                                    Assert.IsTrue(main.Hold(new TestInterfaceImpl(new Counters())).Wait(MediumNonDbgTimeout));
                                    Assert.IsTrue(fooTask.Wait(MediumNonDbgTimeout));
                                    Assert.AreEqual("bar", fooTask.Result);
                                }
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ExportTailCallCapToThirdParty()
        {
            using (var server = SetupServer())
            {
                server.Main = new TestTailCallerImpl2();

                using (var client = SetupClient())
                {
                    //Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                    using (var main = client.GetMain<ITestTailCaller>())
                    {
                        var callee = new TestTailCalleeImpl(new Counters());
                        var fooTask = main.Foo(123, callee);
                        Assert.IsTrue(fooTask.Wait(MediumNonDbgTimeout));

                        using (var c = fooTask.Result.C)
                        using (var client2 = SetupClient())
                        {
                            //Assert.IsTrue(client2.WhenConnected.Wait(MediumNonDbgTimeout));

                            using (var main2 = client2.GetMain<ITestTailCaller>())
                            {
                                var fooTask2 = main2.Foo(123, null);
                                Assert.IsTrue(fooTask2.Wait(MediumNonDbgTimeout));
                                Assert.IsTrue(fooTask2.C().GetCallSequence(1).Wait(MediumNonDbgTimeout));
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SalamiTactics()
        {
            using (var server = SetupServer())
            {
                server.Main = new TestMoreStuffImpl3();

                using (var client = SetupClient())
                {
                    //client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        var echoTask = main.Echo(Proxy.Share<ITestCallOrder>(main));
                        Assert.IsTrue(echoTask.Wait(MediumNonDbgTimeout));
                        using (var echo = echoTask.Result)
                        {
                            var list = new Task<uint>[200];
                            for (uint i = 0; i < list.Length; i++)
                            {
                                list[i] = echo.GetCallSequence(i);
                            }
                            Assert.IsTrue(Task.WaitAll(list, MediumNonDbgTimeout));
                            for (uint i = 0; i < list.Length; i++)
                            {
                                Assert.AreEqual(i, list[i].Result);
                            }
                        }
                    }
                }
            }
        }


        [TestMethod]
        public void NoTailCallMt()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.NoTailCallMt);
        }

        [TestMethod]
        public void CallAfterFinish1()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.CallAfterFinish1);
        }

        [TestMethod]
        public void CallAfterFinish2()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.CallAfterFinish2);
        }
    }
}
