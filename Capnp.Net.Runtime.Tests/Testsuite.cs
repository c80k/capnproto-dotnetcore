using System;
using System.Linq;
using Capnp.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capnp.Net.Runtime.Tests.GenImpls;
using Capnproto_test.Capnp.Test;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Capnp.Net.Runtime.Tests
{
    static class Testsuite
    {
        static void ExpectPromiseThrows(this ITestbed testbed, params Task[] tasks)
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
                catch (InvalidTimeZoneException)
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

            var ftasks = tasks.Select(ExpectPromiseThrowsAsync).ToArray();
            testbed.MustComplete(ftasks);

            foreach (var ftask in ftasks)
                ftask.GetAwaiter().GetResult(); // re-throw exception
        }

        public static void Embargo(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestMoreStuffImpl(counters);

            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            {
                if (main is IResolvingCapability resolving)
                    testbed.MustComplete(resolving.WhenResolved);

                var cap = new TestCallOrderImpl();
                cap.CountToDispose = 6;

                var earlyCall = main.GetCallSequence(0, default);

                var echo = main.Echo(cap, default);
                testbed.MustComplete(Task.CompletedTask);
                using (var pipeline = echo.Eager(true))
                {
                    var call0 = pipeline.GetCallSequence(0, default);
                    var call1 = pipeline.GetCallSequence(1, default);

                    testbed.MustComplete(earlyCall);

                    impl.EnableEcho();

                    var call2 = pipeline.GetCallSequence(2, default);

                    testbed.MustComplete(echo);
                    using (var resolved = echo.Result)
                    {
                        var call3 = pipeline.GetCallSequence(3, default);
                        var call4 = pipeline.GetCallSequence(4, default);
                        var call5 = pipeline.GetCallSequence(5, default);

                        try
                        {
                            testbed.MustComplete(call0);
                            testbed.MustComplete(call1);
                            testbed.MustComplete(call2);
                            testbed.MustComplete(call3);
                            testbed.MustComplete(call4);
                            testbed.MustComplete(call5);
                        }
                        catch (System.Exception)
                        {
                            cap.CountToDispose = null;
                            throw;
                        }

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
 
        public static void EmbargoError(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestMoreStuffImpl(counters);
            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            {
                if (main is IResolvingCapability resolving)
                    testbed.MustComplete(resolving.WhenResolved);

                var cap = new TaskCompletionSource<ITestCallOrder>();

                var earlyCall = main.GetCallSequence(0, default);
                var echo = main.Echo(cap.Task.Eager(true), default);

                using (var pipeline = echo.Eager(true))
                {
                    var call0 = pipeline.GetCallSequence(0, default);
                    var call1 = pipeline.GetCallSequence(1, default);

                    testbed.MustComplete(earlyCall);

                    impl.EnableEcho();

                    var call2 = pipeline.GetCallSequence(2, default);

                    testbed.MustComplete(echo);
                    var resolved = echo.Result;
                    var call3 = pipeline.GetCallSequence(3, default);
                    var call4 = pipeline.GetCallSequence(4, default);
                    var call5 = pipeline.GetCallSequence(5, default);

                    cap.SetException(new InvalidTimeZoneException("I'm annoying"));

                    testbed.ExpectPromiseThrows(call0, call1, call2, call3, call4, call5);

                    // Verify that we're still connected (there were no protocol errors).
                    testbed.MustComplete(main.GetCallSequence(1, default));
                }
            }
        }

        public static void EmbargoNull(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestMoreStuffImpl(counters);
            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            {
                if (main is IResolvingCapability resolving)
                    testbed.MustComplete(resolving.WhenResolved);

                var promise = main.GetNull(default);

                using (var cap = promise.Eager(true))
                {
                    var call0 = cap.GetCallSequence(0, default);

                    testbed.MustComplete(promise);

                    var call1 = cap.GetCallSequence(1, default);

                    testbed.ExpectPromiseThrows(call0, call1);
                }

                // Verify that we're still connected (there were no protocol errors).
                testbed.MustComplete(main.GetCallSequence(1, default));
            }
        }

        public static void CallBrokenPromise(ITestbed testbed)
        {
            var counters = new Counters();
            using (var impl = new TestMoreStuffImpl(counters))
            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            {
                if (main is IResolvingCapability resolving)
                    testbed.MustComplete(resolving.WhenResolved);

                var tcs = new TaskCompletionSource<ITestInterface>();

                var req = main.Hold(tcs.Task.Eager(true), default);
                testbed.MustComplete(req);

                var req2 = main.CallHeld(default);

                testbed.MustNotComplete(req2);

                tcs.SetException(new InvalidOperationException("I'm a promise-breaker!"));

                testbed.ExpectPromiseThrows(req2);

                // Verify that we're still connected (there were no protocol errors).
                testbed.MustComplete(main.GetCallSequence(1, default));
            }
        }

        public static void TailCall(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestTailCallerImpl(counters);
            using (var main = testbed.ConnectMain<ITestTailCaller>(impl))
            {
                var calleeCallCount = new Counters();
                var callee = new TestTailCalleeImpl(calleeCallCount);

                var promise = main.Foo(456, callee, default);
                using (var c = promise.C())
                {
                    var dependentCall0 = c.GetCallSequence(0, default);

                    testbed.MustComplete(promise);
                    Assert.AreEqual(456u, promise.Result.I);
                    Assert.AreEqual("from TestTailCaller", promise.Result.T);

                    var dependentCall1 = c.GetCallSequence(0, default);
                    var dependentCall2 = c.GetCallSequence(0, default);

                    testbed.MustComplete(dependentCall0, dependentCall1, dependentCall2);

                    Assert.IsTrue(callee.IsDisposed);
                    Assert.AreEqual(1, counters.CallCount);
                    Assert.AreEqual(1, calleeCallCount.CallCount);
                }
            }
        }

        public static void SendTwice(ITestbed testbed)
        {
            var destructionPromise = new TaskCompletionSource<int>();
            var destructionTask = destructionPromise.Task;

            var counters = new Counters();
            var impl = new TestMoreStuffImpl(counters);
            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            {
                var cap = new TestInterfaceImpl(new Counters(), destructionPromise);

                Task<string> ftask1, ftask2;

                using (var claimer = Skeleton.Claim(cap))
                {
                    var ftask = main.CallFoo(cap, default);
                    testbed.MustComplete(ftask);
                    Assert.AreEqual("bar", ftask.Result);

                    var ctask = main.GetCallSequence(0, default);
                    testbed.MustComplete(ctask);
                    Assert.AreEqual(1u, ctask.Result);

                    ftask1 = main.CallFoo(cap, default);
                    ftask2 = main.CallFoo(cap, default);
                }

                testbed.MustComplete(ftask1);
                Assert.AreEqual("bar", ftask1.Result);
                testbed.MustComplete(ftask2);
                Assert.AreEqual("bar", ftask2.Result);

                // Now the cap should be released.
                testbed.MustComplete(destructionTask);
            }
        }

        public static void Cancel(ITestbed testbed)
        {
            var destructionPromise = new TaskCompletionSource<int>();
            var destructionTask = destructionPromise.Task;

            var counters = new Counters();
            var impl = new TestMoreStuffImpl(counters);
            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            using (var cts = new CancellationTokenSource())
            {
                var ntask = main.NeverReturn(new TestInterfaceImpl(counters, destructionPromise), cts.Token);

                // Allow some time to settle.
                var cstask = main.GetCallSequence(0, default);
                testbed.MustComplete(cstask);
                Assert.AreEqual(1u, cstask.Result);
                cstask = main.GetCallSequence(0, default);
                testbed.MustComplete(cstask);
                Assert.AreEqual(2u, cstask.Result);

                // The cap shouldn't have been destroyed yet because the call never returned.
                Assert.IsFalse(destructionTask.IsCompleted);

                // There will be no automatic cancellation just because "ntask" goes of of scope or
                // because the Proxy is disposed. Even ntask.Dispose() would not cancel the request.
                // In .NET this needs to be done explicitly.
                cts.Cancel();
            }

            // Now the cap should be released.
            testbed.MustComplete(destructionTask);
        }

        public static void RetainAndRelease(ITestbed testbed)
        {
            var destructionPromise = new TaskCompletionSource<int>();
            var destructionTask = destructionPromise.Task;

            var counters = new Counters();
            using (var impl = new TestMoreStuffImpl(counters))
            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            {
                var holdTask = main.Hold(new TestInterfaceImpl(new Counters(), destructionPromise), default);
                testbed.MustComplete(holdTask);

                var cstask = main.GetCallSequence(0, default);
                testbed.MustComplete(cstask);
                Assert.AreEqual(1u, cstask.Result);

                Assert.IsFalse(destructionTask.IsCompleted);

                var htask = main.CallHeld(default);
                testbed.MustComplete(htask);
                Assert.AreEqual("bar", htask.Result);

                var gtask = main.GetHeld(default);
                testbed.MustComplete(gtask);
                // We can get the cap back from it.
                using (var cap = gtask.Result)
                {
                    // Wait for balanced state
                    testbed.FlushCommunication();

                    // And call it, without any network communications.
                    long oldSendCount = testbed.ClientSendCount;
                    var ftask = cap.Foo(123, true, default);
                    testbed.MustComplete(ftask);
                    Assert.AreEqual("foo", ftask.Result);
                    Assert.AreEqual(oldSendCount, testbed.ClientSendCount);

                    // We can send another copy of the same cap to another method, and it works.
                    // Note that this was a bug in previous versions: 
                    // Since passing a cap has move semantics, we need to create an explicit copy.
                    var copy = Proxy.Share(cap);
                    var ctask = main.CallFoo(copy, default);
                    testbed.MustComplete(ctask);
                    Assert.AreEqual("bar", ctask.Result);

                    // Give some time to settle.
                    cstask = main.GetCallSequence(0, default);
                    testbed.MustComplete(cstask);
                    Assert.AreEqual(5u, cstask.Result);
                    cstask = main.GetCallSequence(0, default);
                    testbed.MustComplete(cstask);
                    Assert.AreEqual(6u, cstask.Result);
                    cstask = main.GetCallSequence(0, default);
                    testbed.MustComplete(cstask);
                    Assert.AreEqual(7u, cstask.Result);

                    // Can't be destroyed, we haven't released it.
                    Assert.IsFalse(destructionTask.IsCompleted);
                }
            }

            testbed.MustComplete(destructionTask);
        }

        public static void PromiseResolve(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestMoreStuffImpl(counters);
            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            {
                var tcs = new TaskCompletionSource<ITestInterface>();
                using (var eager = tcs.Task.Eager(true))
                {
                    var request = main.CallFoo(Proxy.Share(eager), default);
                    var request2 = main.CallFooWhenResolved(eager, default);

                    var gcs = main.GetCallSequence(0, default);
                    testbed.MustComplete(gcs);
                    Assert.AreEqual(2u, gcs.Result);
                    Assert.AreEqual(3, counters.CallCount);

                    var chainedCallCount = new Counters();
                    var tiimpl = new TestInterfaceImpl(chainedCallCount);
                    tcs.SetResult(tiimpl);

                    testbed.MustComplete(request, request2);

                    Assert.AreEqual("bar", request.Result);
                    Assert.AreEqual("bar", request2.Result);
                    Assert.AreEqual(3, counters.CallCount);
                    Assert.AreEqual(2, chainedCallCount.CallCount);
                }
            }
        }

        public static void Cancelation(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestMoreStuffImpl(counters);
            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            {
                var destroyed = new TaskCompletionSource<int>();
                var impl2 = new TestInterfaceImpl(counters, destroyed);
                var cts = new CancellationTokenSource();
                var cancelTask = main.ExpectCancel(impl2, cts.Token);

                testbed.MustNotComplete(destroyed.Task, cancelTask);

                cts.Cancel();

                testbed.MustComplete(destroyed.Task);
                Assert.IsFalse(cancelTask.IsCompleted && !cancelTask.IsCanceled);
            }
        }

        public static void ReleaseOnCancel(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestMoreStuffImpl(counters);
            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            {
                using (var cts = new CancellationTokenSource())
                {
                    var task = main.GetHandle(cts.Token);
                    testbed.MustComplete(Task.CompletedTask); // turn event loop
                    cts.Cancel();
                    testbed.MustComplete(task);
                    try
                    {
                        task.Result.Dispose();
                    }
                    catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
                    {
                    }
                }

                testbed.FlushCommunication();
                Assert.AreEqual(0, counters.HandleCount);
            }
        }

        public static void Release(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestMoreStuffImpl(counters);
            using (var main = testbed.ConnectMain<ITestMoreStuff>(impl))
            {
                var task1 = main.GetHandle(default);
                var task2 = main.GetHandle(default);
                testbed.MustComplete(task1, task2);

                Assert.AreEqual(2, counters.HandleCount);

                task1.Result.Dispose();

                testbed.FlushCommunication();
                Assert.AreEqual(1, counters.HandleCount);

                task2.Result.Dispose();

                testbed.FlushCommunication();
                Assert.AreEqual(0, counters.HandleCount);
            }
        }

        public static void Pipeline(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestPipelineImpl(counters);
            using (var main = testbed.ConnectMain<ITestPipeline>(impl))
            {
                var chainedCallCount = new Counters();
                var request = main.GetCap(234, new TestInterfaceImpl(chainedCallCount), default);
                using (var outBox = request.OutBox_Cap())
                {
                    var pipelineRequest = outBox.Foo(321, false, default);
                    using (var testx = ((Proxy)outBox).Cast<ITestExtends>(false))
                    {
                        var pipelineRequest2 = testx.Grault(default);

                        testbed.MustComplete(pipelineRequest, pipelineRequest2);

                        Assert.AreEqual("bar", pipelineRequest.Result);
                        Common.CheckTestMessage(pipelineRequest2.Result);

                        Assert.AreEqual(3, counters.CallCount);
                        Assert.AreEqual(1, chainedCallCount.CallCount);
                    }
                }
            }
        }

        public static void Basic(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestInterfaceImpl(counters);
            using (var main = testbed.ConnectMain<ITestInterface>(impl))
            {
                var request1 = main.Foo(123, true, default);
                var request3 = Assert.ThrowsExceptionAsync<RpcException>(() => main.Bar(default));
                var s = new TestAllTypes();
                Common.InitTestMessage(s);
                var request2 = main.Baz(s, default);

                testbed.MustComplete(request1, request2, request3);

                Assert.AreEqual("foo", request1.Result);
                Assert.AreEqual(2, counters.CallCount);
            }
        }

        public static void BootstrapReuse(ITestbed testbed)
        {
            var counters = new Counters();
            var impl = new TestInterfaceImpl(counters);
            for (int i = 0; i < 10; i++)
            {
                using (var main = testbed.ConnectMain<ITestInterface>(impl))
                {
                }
                Assert.IsFalse(impl.IsDisposed);
            }
        }
    }
}
