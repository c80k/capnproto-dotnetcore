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

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class TcpRpcPorted: TestBase
    {
        [TestMethod]
        public void Basic()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var counters = new Counters();
                server.Main = new TestInterfaceImpl(counters);
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
                    Assert.AreEqual(2, counters.CallCount);
                }
            }
        }

        [TestMethod]
        public void Pipeline()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var counters = new Counters();
                server.Main = new TestPipelineImpl(counters);
                using (var main = client.GetMain<ITestPipeline>())
                {
                    var chainedCallCount = new Counters();
                    var request = main.GetCap(234, new TestInterfaceImpl(chainedCallCount), default);
                    using (var outBox = request.OutBox_Cap())
                    {
                        var pipelineRequest = outBox.Foo(321, false, default);
                        var pipelineRequest2 = ((Proxy)outBox).Cast<ITestExtends>(false).Grault(default);

                        Assert.IsTrue(pipelineRequest.Wait(MediumNonDbgTimeout));
                        Assert.IsTrue(pipelineRequest2.Wait(MediumNonDbgTimeout));

                        Assert.AreEqual("bar", pipelineRequest.Result);
                        Common.CheckTestMessage(pipelineRequest2.Result);

                        Assert.AreEqual(3, counters.CallCount);
                        Assert.AreEqual(1, chainedCallCount.CallCount);
                    }
                }

            }
        }

        [TestMethod]
        public void Release()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    var task1 = main.GetHandle(default);
                    var task2 = main.GetHandle(default);
                    Assert.IsTrue(task1.Wait(MediumNonDbgTimeout));
                    Assert.IsTrue(task2.Wait(MediumNonDbgTimeout));

                    Assert.AreEqual(2, counters.HandleCount);

                    task1.Result.Dispose();

                    Assert.IsTrue(SpinWait.SpinUntil(() => counters.HandleCount == 1, MediumNonDbgTimeout));

                    task2.Result.Dispose();

                    Assert.IsTrue(SpinWait.SpinUntil(() => counters.HandleCount == 0, MediumNonDbgTimeout));
                }

            }
        }

        [TestMethod]
        public void ReleaseOnCancel()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

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
                            t.Result.Dispose();
                            cts.Dispose();
                        });
                    }

                    Thread.Sleep(ShortTimeout);

                    Assert.IsTrue(SpinWait.SpinUntil(() => counters.HandleCount == 0, MediumNonDbgTimeout));
                }
            }
        }

        [TestMethod]
        public void TestTailCall()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var counters = new Counters();
                server.Main = new TestTailCallerImpl(counters);
                using (var main = client.GetMain<ITestTailCaller>())
                {
                    var calleeCallCount = new Counters();
                    var callee = new TestTailCalleeImpl(calleeCallCount);

                    var promise = main.Foo(456, callee, default);
                    var dependentCall0 = promise.C().GetCallSequence(0, default);

                    Assert.IsTrue(promise.Wait(MediumNonDbgTimeout));
                    Assert.AreEqual(456u, promise.Result.I);
                    Assert.AreEqual("from TestTailCaller", promise.Result.T);

                    var dependentCall1 = promise.C().GetCallSequence(0, default);
                    var dependentCall2 = promise.C().GetCallSequence(0, default);

                    Assert.IsTrue(dependentCall0.Wait(MediumNonDbgTimeout));
                    Assert.IsTrue(dependentCall1.Wait(MediumNonDbgTimeout));
                    Assert.IsTrue(dependentCall2.Wait(MediumNonDbgTimeout));

                    Assert.AreEqual(1, counters.CallCount);
                    Assert.AreEqual(1, calleeCallCount.CallCount);
                }
            }
        }

        [TestMethod]
        public void Cancelation()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    var destroyed = new TaskCompletionSource<int>();
                    var impl = new TestInterfaceImpl(counters, destroyed);
                    var cts = new CancellationTokenSource();
                    var cancelTask = main.ExpectCancel(impl, cts.Token);

                    Assert.IsFalse(SpinWait.SpinUntil(() => destroyed.Task.IsCompleted || cancelTask.IsCompleted, ShortTimeout));

                    cts.Cancel();

                    Assert.IsTrue(destroyed.Task.Wait(MediumNonDbgTimeout));
                    Assert.IsFalse(cancelTask.IsCompletedSuccessfully);
                }

            }
        }

        [TestMethod]
        public void PromiseResolve()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var counters = new Counters();
                var impl = new TestMoreStuffImpl(counters);
                server.Main = impl;
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    var tcs = new TaskCompletionSource<ITestInterface>();
                    var eager = tcs.Task.PseudoEager();

                    var request = main.CallFoo(eager, default);
                    var request2 = main.CallFooWhenResolved(eager, default);

                    var gcs = main.GetCallSequence(0, default);
                    Assert.IsTrue(gcs.Wait(MediumNonDbgTimeout));
                    Assert.AreEqual(2u, gcs.Result);
                    Assert.AreEqual(3, counters.CallCount);

                    var chainedCallCount = new Counters();
                    var tiimpl = new TestInterfaceImpl(chainedCallCount);
                    tcs.SetResult(tiimpl);

                    Assert.IsTrue(request.Wait(MediumNonDbgTimeout));
                    Assert.IsTrue(request2.Wait(MediumNonDbgTimeout));

                    Assert.AreEqual("bar", request.Result);
                    Assert.AreEqual("bar", request2.Result);
                    Assert.AreEqual(3, counters.CallCount);
                    Assert.AreEqual(2, chainedCallCount.CallCount);
                }

            }
        }

        [TestMethod]
        public void RetainAndRelease()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var destructionPromise = new TaskCompletionSource<int>();
                var destructionTask = destructionPromise.Task;

                var counters = new Counters();
                var impl = new TestMoreStuffImpl(counters);
                server.Main = impl;
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    var holdTask = main.Hold(new TestInterfaceImpl(new Counters(), destructionPromise), default);
                    Assert.IsTrue(holdTask.Wait(MediumNonDbgTimeout));

                    var cstask = main.GetCallSequence(0, default);
                    Assert.IsTrue(cstask.Wait(MediumNonDbgTimeout));
                    Assert.AreEqual(1u, cstask.Result);

                    Assert.IsFalse(destructionTask.IsCompleted);

                    var htask = main.CallHeld(default);
                    Assert.IsTrue(htask.Wait(MediumNonDbgTimeout));
                    Assert.AreEqual("bar", htask.Result);

                    var gtask = main.GetHeld(default);
                    Assert.IsTrue(gtask.Wait(MediumNonDbgTimeout));
                    // We can get the cap back from it.
                    using (var cap = gtask.Result)
                    {
                        // Wait for balanced state
                        WaitClientServerIdle(server, client);

                        // And call it, without any network communications.
                        long oldSendCount = client.SendCount;
                        var ftask = cap.Foo(123, true, default);
                        Assert.IsTrue(ftask.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual("foo", ftask.Result);
                        Assert.AreEqual(oldSendCount, client.SendCount);

                        // We can send another copy of the same cap to another method, and it works.
                        var ctask = main.CallFoo(cap, default);
                        Assert.IsTrue(ctask.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual("bar", ctask.Result);

                        // Give some time to settle.
                        cstask = main.GetCallSequence(0, default);
                        Assert.IsTrue(cstask.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual(5u, cstask.Result);
                        cstask = main.GetCallSequence(0, default);
                        Assert.IsTrue(cstask.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual(6u, cstask.Result);
                        cstask = main.GetCallSequence(0, default);
                        Assert.IsTrue(cstask.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual(7u, cstask.Result);

                        // Can't be destroyed, we haven't released it.
                        Assert.IsFalse(destructionTask.IsCompleted);
                    }

                    // In deviation from original test, we have null the held capability on the main interface.
                    // This is because the main interface is the bootstrap capability and, as such, won't be disposed
                    // after disconnect.
                    var holdNullTask = main.Hold(null, default);
                    Assert.IsTrue(holdNullTask.Wait(MediumNonDbgTimeout));
                }

                Assert.IsTrue(destructionTask.Wait(MediumNonDbgTimeout));
            }
        }

        [TestMethod]
        public void Cancel()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var destructionPromise = new TaskCompletionSource<int>();
                var destructionTask = destructionPromise.Task;

                var counters = new Counters();
                var impl = new TestMoreStuffImpl(counters);
                server.Main = impl;
                using (var main = client.GetMain<ITestMoreStuff>())
                using (var cts = new CancellationTokenSource())
                {
                    var ntask = main.NeverReturn(new TestInterfaceImpl(counters, destructionPromise), cts.Token);

                    // Allow some time to settle.
                    var cstask = main.GetCallSequence(0, default);
                    Assert.IsTrue(cstask.Wait(MediumNonDbgTimeout));
                    Assert.AreEqual(1u, cstask.Result);
                    cstask = main.GetCallSequence(0, default);
                    Assert.IsTrue(cstask.Wait(MediumNonDbgTimeout));
                    Assert.AreEqual(2u, cstask.Result);

                    // The cap shouldn't have been destroyed yet because the call never returned.
                    Assert.IsFalse(destructionTask.IsCompleted);

                    // There will be no automatic cancellation just because "ntask" goes of of scope or
                    // because the Proxy is disposed. Even ntask.Dispose() would not cancel the request.
                    // In .NET this needs to be done explicitly.
                    cts.Cancel();
                }

                // Now the cap should be released.
                Assert.IsTrue(destructionTask.Wait(MediumNonDbgTimeout));
            }
        }

        [TestMethod]
        public void SendTwice()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var destructionPromise = new TaskCompletionSource<int>();
                var destructionTask = destructionPromise.Task;

                var counters = new Counters();
                var impl = new TestMoreStuffImpl(counters);
                server.Main = impl;
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    var cap = new TestInterfaceImpl(new Counters(), destructionPromise);

                    Task<string> ftask1, ftask2;

                    using (Skeleton.Claim(cap))
                    {
                        var ftask = main.CallFoo(cap, default);
                        Assert.IsTrue(ftask.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual("bar", ftask.Result);

                        var ctask = main.GetCallSequence(0, default);
                        Assert.IsTrue(ctask.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual(1u, ctask.Result);

                        ftask1 = main.CallFoo(cap, default);
                        ftask2 = main.CallFoo(cap, default);
                    }

                    Assert.IsTrue(ftask1.Wait(MediumNonDbgTimeout));
                    Assert.AreEqual("bar", ftask1.Result);
                    Assert.IsTrue(ftask2.Wait(MediumNonDbgTimeout));
                    Assert.AreEqual("bar", ftask2.Result);

                    // Now the cap should be released.
                    Assert.IsTrue(destructionTask.Wait(MediumNonDbgTimeout));
                }
            }
        }

        [TestMethod]
        public void Embargo()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var counters = new Counters();
                var impl = new TestMoreStuffImpl(counters);
                server.Main = impl;
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    var resolving = main as IResolvingCapability;
                    Assert.IsTrue(resolving.WhenResolved.Wait(MediumNonDbgTimeout));

                    var cap = new TestCallOrderImpl();

                    var earlyCall = main.GetCallSequence(0, default);

                    var echo = main.Echo(cap, default);

                    using (var pipeline = echo.Eager())
                    {
                        var call0 = pipeline.GetCallSequence(0, default);
                        var call1 = pipeline.GetCallSequence(1, default);

                        Assert.IsTrue(earlyCall.Wait(MediumNonDbgTimeout));

                        impl.EnableEcho();

                        var call2 = pipeline.GetCallSequence(2, default);

                        Assert.IsTrue(echo.Wait(MediumNonDbgTimeout));
                        using (var resolved = echo.Result)
                        {
                            var call3 = pipeline.GetCallSequence(3, default);
                            var call4 = pipeline.GetCallSequence(4, default);
                            var call5 = pipeline.GetCallSequence(5, default);

                            Assert.IsTrue(call0.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(call1.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(call2.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(call3.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(call4.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(call5.Wait(MediumNonDbgTimeout));

                            Assert.AreEqual(0u, call0.Result);
                            Assert.AreEqual(1u, call1.Result);
                            Assert.AreEqual(2u, call2.Result);
                            Assert.AreEqual(3u, call3.Result);
                            Assert.AreEqual(4u, call4.Result);
                            Assert.AreEqual(5u, call5.Result);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void EmbargoError()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var counters = new Counters();
                var impl = new TestMoreStuffImpl(counters);
                server.Main = impl;
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    var resolving = main as IResolvingCapability;
                    Assert.IsTrue(resolving.WhenResolved.Wait(MediumNonDbgTimeout));

                    var cap = new TaskCompletionSource<ITestCallOrder>();

                    var earlyCall = main.GetCallSequence(0, default);

                    var echo = main.Echo(cap.Task.PseudoEager(), default);

                    var pipeline = echo.Eager();

                    var call0 = pipeline.GetCallSequence(0, default);
                    var call1 = pipeline.GetCallSequence(1, default);

                    Assert.IsTrue(earlyCall.Wait(MediumNonDbgTimeout));

                    impl.EnableEcho();

                    var call2 = pipeline.GetCallSequence(2, default);

                    Assert.IsTrue(echo.Wait(MediumNonDbgTimeout));
                    var resolved = echo.Result;

                    var call3 = pipeline.GetCallSequence(3, default);
                    var call4 = pipeline.GetCallSequence(4, default);
                    var call5 = pipeline.GetCallSequence(5, default);

                    cap.SetException(new InvalidOperationException("I'm annoying"));

                    ExpectPromiseThrows(call0);
                    ExpectPromiseThrows(call1);
                    ExpectPromiseThrows(call2);
                    ExpectPromiseThrows(call3);
                    ExpectPromiseThrows(call4);
                    ExpectPromiseThrows(call5);

                    // Verify that we're still connected (there were no protocol errors).
                    Assert.IsTrue(main.GetCallSequence(1, default).Wait(MediumNonDbgTimeout));
                }
            }
        }

        [TestMethod]
        public void EmbargoNull()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var counters = new Counters();
                var impl = new TestMoreStuffImpl(counters);
                server.Main = impl;
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    var resolving = main as IResolvingCapability;
                    Assert.IsTrue(resolving.WhenResolved.Wait(MediumNonDbgTimeout));

                    var promise = main.GetNull(default);

                    var cap = promise.Eager();

                    var call0 = cap.GetCallSequence(0, default);

                    Assert.IsTrue(promise.Wait(MediumNonDbgTimeout));

                    var call1 = cap.GetCallSequence(1, default);

                    ExpectPromiseThrows(call0);
                    ExpectPromiseThrows(call1);

                    // Verify that we're still connected (there were no protocol errors).
                    Assert.IsTrue(main.GetCallSequence(1, default).Wait(MediumNonDbgTimeout));
                }
            }
        }

        [TestMethod]
        public void CallBrokenPromise()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));

                var counters = new Counters();
                var impl = new TestMoreStuffImpl(counters);
                server.Main = impl;
                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    var resolving = main as IResolvingCapability;
                    Assert.IsTrue(resolving.WhenResolved.Wait(MediumNonDbgTimeout));

                    var tcs = new TaskCompletionSource<ITestInterface>();

                    var req = main.Hold(tcs.Task.PseudoEager(), default);
                    Assert.IsTrue(req.Wait(MediumNonDbgTimeout));

                    var req2 = main.CallHeld(default);

                    Assert.IsFalse(req2.Wait(ShortTimeout));

                    tcs.SetException(new InvalidOperationException("I'm a promise-breaker!"));

                    ExpectPromiseThrows(req2);

                    // Verify that we're still connected (there were no protocol errors).
                    Assert.IsTrue(main.GetCallSequence(1, default).Wait(MediumNonDbgTimeout));
                }
            }
        }
    }
}
