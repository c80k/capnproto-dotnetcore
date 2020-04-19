using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Capnp.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capnp.Net.Runtime.Tests.GenImpls;
using Capnproto_test.Capnp.Test;
using Microsoft.Extensions.Logging;

namespace Capnp.Net.Runtime.Tests
{

    [TestClass]
    [TestCategory("Coverage")]
    public class TcpRpcPorted: TestBase
    {
        [TestMethod]
        public void Basic()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Basic);
        }

        [TestMethod]
        public void Pipeline()
        {
            NewLocalhostTcpTestbed(TcpRpcTestOptions.ClientTracer).RunTest(Testsuite.Pipeline);
        }

        [TestMethod]
        public void Release()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Release);
        }

        [TestMethod]
        public void ReleaseOnCancel()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                //client.WhenConnected.Wait();

                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    ((Proxy)main).WhenResolved.Wait(MediumNonDbgTimeout);

                    // Since we have a threaded model, there is no way to deterministically provoke the situation
                    // where Cancel and Finish message cross paths. Instead, we'll do a lot of such requests and
                    // later on verify that the handle count is 0.

                    for (int i = 0; i < 1000; i++)
                    {
                        var cts = new CancellationTokenSource();
                        var task = main.GetHandle(cts.Token);
                        cts.Cancel();
                        task.ContinueWith(t =>
                        {
                            try
                            {
                                t.Result.Dispose();
                            }
                            catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
                            {
                            }
                            cts.Dispose();
                        });
                    }

                    Thread.Sleep(ShortTimeout);

                    Assert.IsTrue(SpinWait.SpinUntil(() => counters.HandleCount == 0, MediumNonDbgTimeout));
                }
            }
        }

        [TestMethod]
        public void TailCall()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.TailCall);
        }

        [TestMethod]
        public void Cancelation()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Cancelation);
        }

        [TestMethod]
        public void PromiseResolve()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.PromiseResolve);
        }

        [TestMethod]
        public void RetainAndRelease()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.RetainAndRelease);
        }

        [TestMethod]
        public void Cancel()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Cancel);
        }

        [TestMethod]
        public void SendTwice()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.SendTwice);
        }

        [TestMethod]
        public void Embargo()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.EmbargoOnPromisedAnswer);
        }

        [TestMethod]
        public void EmbargoError()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.EmbargoError);
        }

        [TestMethod]
        public void EmbargoNull()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.EmbargoNull);
        }

        [TestMethod]
        public void CallBrokenPromise()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.CallBrokenPromise);
        }

        [TestMethod]
        public void BootstrapReuse()
        {
            (var server, var client) = SetupClientServerPair();

            var counters = new Counters();
            var impl = new TestInterfaceImpl(counters);

            using (server)
            using (client)
            {
                //client.WhenConnected.Wait();

                server.Main = impl;
                for (int i = 0; i < 10; i++)
                {
                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        ((Proxy)main).WhenResolved.Wait(MediumNonDbgTimeout);
                    }
                    Assert.IsFalse(impl.IsDisposed);
                }
            }

            Assert.IsTrue(impl.IsDisposed);
        }

        [TestMethod]
        public void Ownership1()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Ownership1);
        }

        [TestMethod]
        public void Ownership2()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Ownership2);
        }

        [TestMethod]
        public void Ownership3()
        {
            NewLocalhostTcpTestbed().RunTest(Testsuite.Ownership3);
        }
    }
}
