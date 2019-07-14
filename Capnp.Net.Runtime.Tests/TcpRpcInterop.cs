using Capnp.Net.Runtime.Tests.GenImpls;
using Capnp.Rpc;
using Capnproto_test.Capnp.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class TcpRpcInterop: TestBase
    {
        Process StartProcess(ProcessStartInfo processStartInfo)
        {
            try
            {
                return Process.Start(processStartInfo);
            }
            catch (Win32Exception exception) when (exception.ErrorCode == 2 || exception.ErrorCode == 3)
            {
                Assert.Fail($"Did not find test executable {processStartInfo.FileName}. Did you build CapnpCompatTest.sln in Release configuration?");
            }
            catch (System.Exception exception)
            {
                Assert.Fail($"Could not execute {processStartInfo.FileName}: {exception.Message}");
            }
            return null;
        }

        Process _currentProcess;

        void LaunchCompatTestProcess(string whichTest, Action<StreamReader> test)
        {
            string myPath = Path.GetDirectoryName(typeof(TcpRpcInterop).Assembly.Location);
            string config;
#if DEBUG
            config = "Debug";
#else
            config = "Release";
#endif
            string path = Path.Combine(myPath, $@"..\..\..\..\{config}\CapnpCompatTest.exe");
            path = Path.GetFullPath(path);
            string arguments = $"{whichTest} 127.0.0.1:{TcpPort}";
            var startInfo = new ProcessStartInfo(path, arguments)
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            using (_currentProcess = StartProcess(startInfo))
            using (var job = new Job())
            {
                job.AddProcess(_currentProcess.Handle);

                try
                {
                    _currentProcess.StandardError.ReadToEndAsync().ContinueWith(t => Console.Error.WriteLine(t.Result));
                    var firstLine = _currentProcess.StandardOutput.ReadLineAsync();
                    Assert.IsTrue(firstLine.Wait(MediumNonDbgTimeout), "Problem after launching test process");
                    Assert.IsNotNull(firstLine.Result, "Problem after launching test process");
                    Assert.IsTrue(firstLine.Result.StartsWith("Listening") || firstLine.Result.StartsWith("Connecting"), 
                        "Problem after launching test process");

                    test(_currentProcess.StandardOutput);
                }
                finally
                {
                    try
                    {
                        _currentProcess.Kill();
                    }
                    catch
                    {
                    }
                }
            }
        }

        void SendInput(string line)
        {
            _currentProcess.StandardInput.WriteLine(line);
        }

        void AssertOutput(StreamReader stdout, string expected)
        {
            var line = stdout.ReadLineAsync();
            Assert.IsTrue(line.Wait(MediumNonDbgTimeout));
            Assert.AreEqual(expected, line.Result);
        }

        [TestMethod, Timeout(10000)]
        public void BasicClient()
        {
            LaunchCompatTestProcess("server:Interface", stdout =>
            {
                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestInterface>())
                    {
                        var request1 = main.Foo(123, true, default);
                        var request3 = Assert.ThrowsExceptionAsync<RpcException>(() => main.Bar(default));
                        var s = new TestAllTypes();
                        Common.InitTestMessage(s);
                        var request2 = main.Baz(s, default);

                        AssertOutput(stdout, "foo 123 1");
                        Assert.IsTrue(request1.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual("foo", request1.Result);

                        Assert.IsTrue(request2.Wait(MediumNonDbgTimeout));

                        AssertOutput(stdout, "baz");
                        AssertOutput(stdout, "baz fin");
                        Assert.IsTrue(request3.Wait(MediumNonDbgTimeout));
                    }
                }
            });
        }

        [TestMethod, Timeout(10000)]
        public void BasicServer()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestInterfaceImpl(counters);

                LaunchCompatTestProcess("client:Basic", stdout =>
                {
                    AssertOutput(stdout, "Basic test start");
                    AssertOutput(stdout, "Basic test end");
                    Assert.AreEqual(2, counters.CallCount);
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void PipelineClient()
        {
            LaunchCompatTestProcess("server:Pipeline", stdout =>
            {
                stdout.ReadToEndAsync().ContinueWith(t => Console.WriteLine(t.Result));

                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestPipeline>())
                    {
                        var chainedCallCount = new Counters();
                        var request = main.GetCap(234, new TestInterfaceImpl(chainedCallCount), default);
                        using (var box = request.OutBox_Cap())
                        {
                            var pipelineRequest = box.Foo(321, false, default);
                            using (var box2 = ((Proxy)box).Cast<ITestExtends>(false))
                            {
                                var pipelineRequest2 = box2.Grault(default);

                                Assert.IsTrue(pipelineRequest.Wait(MediumNonDbgTimeout));
                                Assert.IsTrue(pipelineRequest2.Wait(MediumNonDbgTimeout));

                                Assert.AreEqual("bar", pipelineRequest.Result);
                                Common.CheckTestMessage(pipelineRequest2.Result);

                                Assert.AreEqual(1, chainedCallCount.CallCount);
                            }
                        }
                        request.ContinueWith(t => t.Result.Item2.Cap.Dispose());
                    }
                }
            });
        }

        [TestMethod, Timeout(10000)]
        public void PipelineServer()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestPipelineImpl(counters);

                LaunchCompatTestProcess("client:Pipelining", stdout =>
                {
                    AssertOutput(stdout, "Pipelining test start");
                    AssertOutput(stdout, "foo 123 1");
                    AssertOutput(stdout, "Pipelining test end");
                    Assert.AreEqual(3, counters.CallCount);
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void ReleaseClient()
        {
            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        var task1 = main.GetHandle(default);
                        var task2 = main.GetHandle(default);
                        Assert.IsTrue(task1.Wait(MediumNonDbgTimeout));
                        Assert.IsTrue(task2.Wait(MediumNonDbgTimeout));

                        AssertOutput(stdout, "getHandle");
                        AssertOutput(stdout, "++");
                        AssertOutput(stdout, "getHandle");
                        AssertOutput(stdout, "++");

                        task1.Result.Dispose();

                        AssertOutput(stdout, "--");

                        task2.Result.Dispose();

                        AssertOutput(stdout, "--");
                    }
                }
            });
        }

        [TestMethod, Timeout(10000)]
        public void ReleaseServer()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:Release", stdout =>
                {
                    AssertOutput(stdout, "Release test start");
                    AssertOutput(stdout, "sync");
                    Assert.AreEqual(2, counters.HandleCount);
                    SendInput("x");
                    AssertOutput(stdout, "handle1 null");
                    Assert.IsTrue(SpinWait.SpinUntil(() => counters.HandleCount == 1, MediumNonDbgTimeout));
                    SendInput("x");
                    AssertOutput(stdout, "handle2 null");
                    Assert.IsTrue(SpinWait.SpinUntil(() => counters.HandleCount == 1, MediumNonDbgTimeout));
                    SendInput("x");
                    AssertOutput(stdout, "promise null");
                    Assert.IsTrue(SpinWait.SpinUntil(() => counters.HandleCount == 0, MediumNonDbgTimeout));
                    SendInput("x");
                    AssertOutput(stdout, "Release test end");
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void ReleaseOnCancelClient()
        {
            // Since we have a threaded model, there is no way to deterministically provoke the situation
            // where Cancel and Finish message cross paths. Instead, we'll do a lot of such requests and
            // later on verify that the handle count is 0.
            int iterationCount = 1000;

            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        ((Proxy)main).WhenResolved.Wait(MediumNonDbgTimeout);

                        async Task VerifyOutput()
                        {
                            int handleCount = 0;

                            for (int i = 0; i < 2 * iterationCount; i++)
                            {
                                string line = await stdout.ReadLineAsync();

                                switch (line)
                                {
                                    case "getHandle":
                                        line = await stdout.ReadLineAsync();
                                        Assert.AreEqual("++", line);
                                        ++handleCount;
                                        Assert.IsTrue(handleCount <= iterationCount);
                                        break;

                                    case "--":
                                        Assert.IsTrue(handleCount > 0);
                                        --handleCount;
                                        break;

                                    default:
                                        Assert.Fail("Unexpected output");
                                        break;
                                }
                            }

                            Assert.AreEqual(0, handleCount);
                        }

                        var verifyOutputTask = VerifyOutput();

                        for (int i = 0; i < iterationCount; i++)
                        {
                            var task = main.GetHandle(default);
                            Impatient.GetAnswer(task).Dispose();
                            task.ContinueWith(t =>
                            {
                                try
                                {
                                    t.Result.Dispose();
                                }
                                catch (TaskCanceledException)
                                {
                                }
                            });
                        }

                        Assert.IsTrue(verifyOutputTask.Wait(LargeNonDbgTimeout));

                        // Not part of original test. Ensure that there is no unwanted extra output
                        // arising from the test sequence above.
                        var sync = main.GetCallSequence(0, default);
                        AssertOutput(stdout, "getCallSequence");
                    }
                }
            });
        }

        [TestMethod, Timeout(10000)]
        public void ReleaseOnCancelServer()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:ReleaseOnCancel", stdout =>
                {
                    AssertOutput(stdout, "ReleaseOnCancel test start");
                    AssertOutput(stdout, "ReleaseOnCancel test end");
                    Assert.IsTrue(SpinWait.SpinUntil(() => counters.HandleCount == 0, MediumNonDbgTimeout));
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void TestTailCallClient()
        {
            LaunchCompatTestProcess("server:TailCaller", stdout =>
            {
                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

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

                        AssertOutput(stdout, "foo");
                        Assert.IsTrue(dependentCall0.Wait(MediumNonDbgTimeout));
                        Assert.IsTrue(dependentCall1.Wait(MediumNonDbgTimeout));
                        Assert.IsTrue(dependentCall2.Wait(MediumNonDbgTimeout));

                        Assert.AreEqual(1, calleeCallCount.CallCount);
                    }
                }
            });
        }

        [TestMethod, Timeout(10000), Ignore]
        public void TestTailCallServer()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestTailCallerImpl(counters);

                LaunchCompatTestProcess("client:TailCall", stdout =>
                {
                    AssertOutput(stdout, "TailCall test start");
                    AssertOutput(stdout, "foo");
                    AssertOutput(stdout, "TailCall test end");

                    Assert.AreEqual(1, counters.CallCount);
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void CancelationServer()
        {
            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                stdout.ReadToEndAsync().ContinueWith(t => Console.WriteLine(t.Result));

                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        var counters = new Counters();
                        var destroyed = new TaskCompletionSource<int>();
                        var impl = new TestInterfaceImpl(counters, destroyed);
                        var cts = new CancellationTokenSource();
                        var cancelTask = main.ExpectCancel(impl, cts.Token);

                        Assert.IsFalse(SpinWait.SpinUntil(() => destroyed.Task.IsCompleted || cancelTask.IsCompleted, ShortTimeout));

                        cts.Cancel();

                        Assert.IsTrue(destroyed.Task.Wait(MediumNonDbgTimeout));
                        Assert.IsFalse(cancelTask.IsCompleted && !cancelTask.IsCanceled);
                    }
                }
            });
        }

        [TestMethod, Timeout(10000)]
        public void CancelationClient()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:Cancelation", stdout =>
                {
                    AssertOutput(stdout, "Cancelation test start");
                    AssertOutput(stdout, "~");
                    AssertOutput(stdout, "Cancelation test end");
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void PromiseResolveServer()
        {
            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        var tcs = new TaskCompletionSource<ITestInterface>();
                        var eager = tcs.Task.PseudoEager();

                        var request = main.CallFoo(eager, default);
                        AssertOutput(stdout, "callFoo");
                        var request2 = main.CallFooWhenResolved(eager, default);
                        AssertOutput(stdout, "callFooWhenResolved");

                        var gcs = main.GetCallSequence(0, default);
                        AssertOutput(stdout, "getCallSequence");
                        Assert.IsTrue(gcs.Wait(MediumNonDbgTimeout));
                        Assert.AreEqual(2u, gcs.Result);

                        var chainedCallCount = new Counters();
                        var tiimpl = new TestInterfaceImpl(chainedCallCount);
                        tcs.SetResult(tiimpl);

                        Assert.IsTrue(request.Wait(MediumNonDbgTimeout));
                        Assert.IsTrue(request2.Wait(MediumNonDbgTimeout));

                        Assert.AreEqual("bar", request.Result);
                        Assert.AreEqual("bar", request2.Result);
                        Assert.AreEqual(2, chainedCallCount.CallCount);

                        AssertOutput(stdout, "fin");
                        AssertOutput(stdout, "fin");
                    }
                }
            });
        }

        [TestMethod, Timeout(10000)]
        public void PromiseResolveClient()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:PromiseResolve", stdout =>
                {
                    AssertOutput(stdout, "PromiseResolve test start");
                    AssertOutput(stdout, "foo 123 1");
                    AssertOutput(stdout, "foo 123 1");
                    AssertOutput(stdout, "PromiseResolve test end");
                    Assert.AreEqual(3, counters.CallCount);
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void RetainAndReleaseServer()
        {
            var destructionPromise = new TaskCompletionSource<int>();
            var destructionTask = destructionPromise.Task;

            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                stdout.ReadToEndAsync().ContinueWith(t => Console.WriteLine(t.Result));

                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

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

                        // At this point, we have a major difference to Cap'n Proto original test suite.
                        // In the original suite, you will find the following comment:
                        //    "We released our client, which should cause the server to be released, which in turn will
                        //     release the cap pointing back to us."
                        // For the situation here, this assumption would be wrong: Releasing the client does NOT release
                        // the server, because it is the bootstrap capability, and there might be other clients in the future.
                        // The bootstrap capbility is held as long as the TCP server is running.
                        // Instead, the test requests the server to hold the "null" capability, which will replace the
                        // existing one, which in turn will release the cap pointing back to us.
                        holdTask = main.Hold(null, default);
                        Assert.IsTrue(holdTask.Wait(MediumNonDbgTimeout));
                        Assert.IsTrue(destructionTask.Wait(MediumNonDbgTimeout));
                    }
                }
            });
        }

        [TestMethod, Timeout(10000)]
        public void RetainAndReleaseClient()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:RetainAndRelease", stdout =>
                {
                    AssertOutput(stdout, "RetainAndRelease test start");
                    AssertOutput(stdout, "foo 123 1");
                    AssertOutput(stdout, "foo 123 1");
                    AssertOutput(stdout, "foo 123 1");
                    AssertOutput(stdout, "~");
                    AssertOutput(stdout, "RetainAndRelease test end");
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void CancelServer()
        {
            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

                    var destructionPromise = new TaskCompletionSource<int>();
                    var destructionTask = destructionPromise.Task;

                    using (var main = client.GetMain<ITestMoreStuff>())
                    using (var cts = new CancellationTokenSource())
                    {
                        var counters = new Counters();
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

                        // Now the cap should be released.
                        Assert.IsTrue(destructionTask.Wait(MediumNonDbgTimeout));
                    }
                }
            });
        }

        [TestMethod, Timeout(10000)]
        public void CancelClient()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:Cancel", stdout =>
                {
                    AssertOutput(stdout, "Cancel test start");
                    AssertOutput(stdout, "~");
                    AssertOutput(stdout, "Cancel test end");
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void SendTwiceServer()
        {
            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        var destructionPromise = new TaskCompletionSource<int>();
                        var destructionTask = destructionPromise.Task;

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
            });
        }

        [TestMethod, Timeout(10000)]
        public void SendTwiceClient()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:SendTwice", stdout =>
                {
                    AssertOutput(stdout, "SendTwice test start");
                    AssertOutput(stdout, "foo 123 1");
                    AssertOutput(stdout, "foo 123 1");
                    AssertOutput(stdout, "foo 123 1");
                    AssertOutput(stdout, "~");
                    AssertOutput(stdout, "SendTwice test end");
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void EmbargoServer()
        {
            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                int retry = 0;

                label:
                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout), "client connect");

                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        var resolving = main as IResolvingCapability;

                        bool success;

                        try
                        {
                            success = resolving.WhenResolved.Wait(MediumNonDbgTimeout);
                        }
                        catch
                        {
                            success = false;
                        }

                        if (!success)
                        {
                            if (++retry == 5)
                            {
                                Assert.Fail("Attempting to obtain bootstrap interface failed. Bailing out.");
                            }
                            goto label;
                        }

                        var cap = new TestCallOrderImpl();

                        var earlyCall = main.GetCallSequence(0, default);

                        var echo = main.Echo(cap, default);

                        using (var pipeline = echo.Eager())
                        {
                            var call0 = pipeline.GetCallSequence(0, default);
                            var call1 = pipeline.GetCallSequence(1, default);

                            Assert.IsTrue(earlyCall.Wait(MediumNonDbgTimeout), "early call returns");

                            var call2 = pipeline.GetCallSequence(2, default);

                            Assert.IsTrue(echo.Wait(MediumNonDbgTimeout));
                            using (var resolved = echo.Result)
                            {
                                var call3 = pipeline.GetCallSequence(3, default);
                                var call4 = pipeline.GetCallSequence(4, default);
                                var call5 = pipeline.GetCallSequence(5, default);

                                Assert.IsTrue(call0.Wait(MediumNonDbgTimeout), "call 0 returns");
                                Assert.IsTrue(call1.Wait(MediumNonDbgTimeout), "call 1 returns");
                                Assert.IsTrue(call2.Wait(MediumNonDbgTimeout), "call 2 returns");
                                Assert.IsTrue(call3.Wait(MediumNonDbgTimeout), "call 3 returns");
                                Assert.IsTrue(call4.Wait(MediumNonDbgTimeout), "call 4 returns");
                                Assert.IsTrue(call5.Wait(MediumNonDbgTimeout), "call 5 returns");

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
            });
        }

        [TestMethod, Timeout(10000)]
        public void EmbargoClient()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:Embargo", stdout =>
                {
                    AssertOutput(stdout, "Embargo test start");
                    AssertOutput(stdout, "Embargo test end");
                });
            }
        }

        public void EmbargoErrorImpl(StreamReader stdout)
        {
            using (var client = new TcpRpcClient("localhost", TcpPort))
            {

                client.WhenConnected.Wait();

                using (var main = client.GetMain<ITestMoreStuff>())
                {
                    var resolving = main as IResolvingCapability;
                    Assert.IsTrue(resolving.WhenResolved.Wait(MediumNonDbgTimeout));

                    var cap = new TaskCompletionSource<ITestCallOrder>();

                    var earlyCall = main.GetCallSequence(0, default);

                    using (var eager = cap.Task.PseudoEager())
                    {
                        var echo = main.Echo(eager, default);

                        using (var pipeline = echo.Eager())
                        {
                            var call0 = pipeline.GetCallSequence(0, default);
                            var call1 = pipeline.GetCallSequence(1, default);

                            Assert.IsTrue(earlyCall.Wait(MediumNonDbgTimeout));

                            var call2 = pipeline.GetCallSequence(2, default);

                            Assert.IsTrue(echo.Wait(MediumNonDbgTimeout));
                            using (var resolved = echo.Result)
                            {
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
                            }
                        }
                    }

                    // Verify that we're still connected (there were no protocol errors).
                    Assert.IsTrue(main.GetCallSequence(1, default).Wait(MediumNonDbgTimeout));
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void EmbargoErrorServer()
        {
            LaunchCompatTestProcess("server:MoreStuff", EmbargoErrorImpl);
        }

        [TestMethod, Timeout(10000)]
        public void RepeatedEmbargoError()
        {
            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                for (int i = 0; i < 100; i++)
                {
                    EmbargoErrorImpl(stdout);
                }
            });
        }

        [TestMethod, Timeout(10000)]
        public void EmbargoErrorClient()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:EmbargoError", stdout =>
                {
                    AssertOutput(stdout, "EmbargoError test start");
                    AssertOutput(stdout, "EmbargoError test end");
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void EmbargoNullServer()
        {
            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                int retry = 0;

                label:
                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        var resolving = main as IResolvingCapability;

                        bool success;

                        try
                        {
                            success = resolving.WhenResolved.Wait(MediumNonDbgTimeout);
                        }
                        catch
                        {
                            success = false;
                        }

                        if (!success)
                        {
                            if (++retry == 5)
                            {
                                Assert.Fail("Attempting to obtain bootstrap interface failed. Bailing out.");
                            }
                            goto label;
                        }

                        var promise = main.GetNull(default);

                        using (var cap = promise.Eager())
                        {
                            var call0 = cap.GetCallSequence(0, default);

                            Assert.IsTrue(promise.Wait(MediumNonDbgTimeout));
                            using (promise.Result)
                            {
                                var call1 = cap.GetCallSequence(1, default);

                                ExpectPromiseThrows(call0);
                                ExpectPromiseThrows(call1);

                                // Verify that we're still connected (there were no protocol errors).
                                Assert.IsTrue(main.GetCallSequence(1, default).Wait(MediumNonDbgTimeout));
                            }
                        }
                    }
                }

            });
        }

        [TestMethod, Timeout(10000)]
        public void EmbargoNullClient()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:EmbargoNull", stdout =>
                {
                    AssertOutput(stdout, "EmbargoNull test start");
                    AssertOutput(stdout, "EmbargoNull test end");
                });
            }
        }

        [TestMethod, Timeout(10000)]
        public void CallBrokenPromiseServer()
        {
            LaunchCompatTestProcess("server:MoreStuff", stdout =>
            {
                using (var client = new TcpRpcClient("localhost", TcpPort))
                {
                    client.WhenConnected.Wait();

                    using (var main = client.GetMain<ITestMoreStuff>())
                    {
                        var resolving = main as IResolvingCapability;
                        Assert.IsTrue(resolving.WhenResolved.Wait(MediumNonDbgTimeout));

                        var tcs = new TaskCompletionSource<ITestInterface>();

                        using (var eager = tcs.Task.PseudoEager())
                        {
                            var req = main.Hold(eager, default);
                            Assert.IsTrue(req.Wait(MediumNonDbgTimeout));
                        }

                        var req2 = main.CallHeld(default);

                        Assert.IsFalse(req2.Wait(ShortTimeout));

                        tcs.SetException(new InvalidOperationException("I'm a promise-breaker!"));

                        ExpectPromiseThrows(req2);

                        // Verify that we're still connected (there were no protocol errors).
                        Assert.IsTrue(main.GetCallSequence(1, default).Wait(MediumNonDbgTimeout));
                    }
                }
            });
        }

        [TestMethod, Timeout(10000)]
        public void CallBrokenPromiseClient()
        {
            using (var server = SetupServer())
            {
                var counters = new Counters();
                server.Main = new TestMoreStuffImpl(counters);

                LaunchCompatTestProcess("client:CallBrokenPromise", stdout =>
                {
                    AssertOutput(stdout, "CallBrokenPromise test start");
                    AssertOutput(stdout, "CallBrokenPromise test end");
                });
            }
        }
    }
}
