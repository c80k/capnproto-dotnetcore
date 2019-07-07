using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Capnp.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Capnp.Net.Runtime.Tests
{

    [TestClass]
    public class TcpRpc
    {
        public static int TcpPort = 33444;

        (TcpRpcServer, TcpRpcClient) SetupClientServerPair()
        {
            var server = new TcpRpcServer(IPAddress.Any, TcpPort);
            var client = new TcpRpcClient("localhost", TcpPort);
            return (server, client);
        }

        bool ExpectingLogOutput { get; set; }

        [TestInitialize]
        public void InitConsoleLogging()
        {
            ExpectingLogOutput = true;

            Logging.LoggerFactory = new LoggerFactory().AddConsole((msg, level) =>
            {
                if (!ExpectingLogOutput && level != LogLevel.Debug)
                {
                    Assert.Fail("Did not expect any logging output, but got this: " + msg);
                }
                return true;
            });
        }

        int MediumTimeout => Debugger.IsAttached ? Timeout.Infinite : 2000;

        [TestMethod, Timeout(10000)]
        public void CreateAndDispose()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
            }
        }

        [TestMethod, Timeout(10000)]
        public void ConnectAndDispose()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                try
                {
                    client.WhenConnected.Wait();
                    SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                    Assert.AreEqual(1, server.ConnectionCount);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void ConnectNoServer()
        {
            using (var client = new TcpRpcClient("localhost", TcpPort))
            {
                Assert.IsTrue(Assert.ThrowsExceptionAsync<RpcException>(() => client.WhenConnected).Wait(10000));
            }
        }

        [TestMethod, Timeout(10000)]
        public void ConnectAndBootstrap()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                client.WhenConnected.Wait();
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                server.Main = new ProvidedCapabilityMock();
                var main = client.GetMain<BareProxy>();
                var resolving = main as IResolvingCapability;
                Assert.IsTrue(resolving.WhenResolved.Wait(MediumTimeout));
            }
        }

        [TestMethod, Timeout(10000)]
        public void ConnectNoBootstrap()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                client.WhenConnected.Wait();
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var main = client.GetMain<BareProxy>();
                var resolving = main as IResolvingCapability;
                Assert.IsTrue(Assert.ThrowsExceptionAsync<RpcException>(() => resolving.WhenResolved).Wait(MediumTimeout));
            }
        }

        [TestMethod, Timeout(10000)]
        public void CallReturn()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                client.WhenConnected.Wait();
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args, false))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumTimeout));
                    (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                    Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                    Assert.AreEqual<ushort>(0x3333, methodId);
                    Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                    Assert.AreEqual(123456, inargs.ReadDataInt(0));

                    var result = DynamicSerializerState.CreateForRpc();
                    result.SetStruct(1, 0);
                    result.WriteData(0, 654321);
                    mock.Return.SetResult(result);

                    Assert.IsTrue(answer.WhenReturned.Wait(MediumTimeout));
                    var outresult = answer.WhenReturned.Result;
                    Assert.AreEqual(ObjectKind.Struct, outresult.Kind);
                    Assert.AreEqual(654321, outresult.ReadDataInt(0));
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void CallCancelOnServer()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                client.WhenConnected.Wait();
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args, false))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumTimeout));
                    (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                    Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                    Assert.AreEqual<ushort>(0x3333, methodId);
                    Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                    Assert.AreEqual(123456, inargs.ReadDataInt(0));

                    mock.Return.SetCanceled();

                    Assert.IsTrue(Assert.ThrowsExceptionAsync<TaskCanceledException>(() => answer.WhenReturned).Wait(MediumTimeout));
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void CallCancelOnClient()
        {
            ExpectingLogOutput = false;

            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                try
                {
                    client.WhenConnected.Wait();
                    SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                    Assert.AreEqual(1, server.ConnectionCount);

                    var mock = new ProvidedCapabilityMock();
                    server.Main = mock;
                    var main = client.GetMain<BareProxy>();
                    var resolving = main as IResolvingCapability;
                    Assert.IsTrue(resolving.WhenResolved.Wait(MediumTimeout));
                    var args = DynamicSerializerState.CreateForRpc();
                    args.SetStruct(1, 0);
                    args.WriteData(0, 123456);
                    CancellationToken ctx;
                    using (var answer = main.Call(0x1234567812345678, 0x3333, args, false))
                    {
                        Assert.IsTrue(mock.WhenCalled.Wait(MediumTimeout));
                        (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                        Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                        Assert.AreEqual<ushort>(0x3333, methodId);
                        Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                        Assert.AreEqual(123456, inargs.ReadDataInt(0));
                        ctx = ct;
                    }

                    Assert.IsTrue(SpinWait.SpinUntil(() => ctx.IsCancellationRequested, MediumTimeout));
                }
                finally
                {
                    ExpectingLogOutput = true;
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void CallReturnAfterClientSideCancel()
        {
            ExpectingLogOutput = false;
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                try
                {
                    client.WhenConnected.Wait();
                    SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                    Assert.AreEqual(1, server.ConnectionCount);

                    var mock = new ProvidedCapabilityMock();
                    server.Main = mock;
                    var main = client.GetMain<BareProxy>();
                    Assert.IsTrue(main.WhenResolved.Wait(MediumTimeout));
                    var args = DynamicSerializerState.CreateForRpc();
                    args.SetStruct(1, 0);
                    args.WriteData(0, 123456);
                    CancellationToken ctx;
                    IPromisedAnswer answer;
                    using (answer = main.Call(0x1234567812345678, 0x3333, args, false))
                    {
                        Assert.IsTrue(mock.WhenCalled.Wait(MediumTimeout));
                        (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                        Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                        Assert.AreEqual<ushort>(0x3333, methodId);
                        Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                        Assert.AreEqual(123456, inargs.ReadDataInt(0));
                        ctx = ct;
                    }

                    Assert.IsTrue(SpinWait.SpinUntil(() => ctx.IsCancellationRequested, MediumTimeout));

                    var mbr = MessageBuilder.Create();
                    mbr.InitCapTable();
                    var result = new DynamicSerializerState(mbr);
                    result.SetStruct(1, 0);
                    result.WriteData(0, 654321);
                    mock.Return.SetResult(result);

                    // Even after the client cancelled the call, the server must still send
                    // a response.
                    Assert.IsTrue(answer.WhenReturned.ContinueWith(t => { }).Wait(MediumTimeout));
                }
                finally
                {
                    ExpectingLogOutput = true;
                }
            }
        }

        class MyTestException: System.Exception
        {
            public MyTestException(): base("Test exception")
            {
            }
        }

        [TestMethod, Timeout(10000)]
        public void CallServerSideException()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                client.WhenConnected.Wait();
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args, false))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumTimeout));
                    (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                    Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                    Assert.AreEqual<ushort>(0x3333, methodId);
                    Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                    Assert.AreEqual(123456, inargs.ReadDataInt(0));

                    mock.Return.SetException(new MyTestException());

                    var exTask = Assert.ThrowsExceptionAsync<RpcException>(() => answer.WhenReturned);
                    Assert.IsTrue(exTask.Wait(MediumTimeout));
                    Assert.IsTrue(exTask.Result.Message.Contains(new MyTestException().Message));
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void PipelineBeforeReturn()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                client.WhenConnected.Wait();
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args, true))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumTimeout));

                    var pipelined = (BareProxy)CapabilityReflection.CreateProxy<BareProxy>(answer.Access(
                        new MemberAccessPath(new MemberAccessPath.MemberAccess[] { new MemberAccessPath.StructMemberAccess(1) })));

                    var args2 = DynamicSerializerState.CreateForRpc();
                    args2.SetStruct(1, 0);
                    args2.WriteData(0, 654321);

                    using (var answer2 = pipelined.Call(0x8765432187654321, 0x4444, args2, false))
                    {
                        (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                        Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                        Assert.AreEqual<ushort>(0x3333, methodId);
                        Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                        Assert.AreEqual(123456, inargs.ReadDataInt(0));

                        var mock2 = new ProvidedCapabilityMock();

                        var result = DynamicSerializerState.CreateForRpc();
                        result.SetStruct(1, 2);
                        result.WriteData(0, 654321);
                        uint id = result.ProvideCapability(mock2);
                        result.LinkToCapability(1, id);

                        mock.Return.SetResult(result);

                        Assert.IsTrue(answer.WhenReturned.Wait(MediumTimeout));
                        Assert.IsFalse(ct.IsCancellationRequested);

                        Assert.IsTrue(mock2.WhenCalled.Wait(MediumTimeout));

                        (var interfaceId2, var methodId2, var inargs2, var ct2) = mock2.WhenCalled.Result;
                        Assert.AreEqual<ulong>(0x8765432187654321, interfaceId2);
                        Assert.AreEqual<ushort>(0x4444, methodId2);
                        Assert.AreEqual(ObjectKind.Struct, inargs2.Kind);
                        Assert.AreEqual(654321, inargs2.ReadDataInt(0));

                        var result2 = DynamicSerializerState.CreateForRpc();
                        result2.SetStruct(1, 0);
                        result2.WriteData(0, 222222);
                        mock2.Return.SetResult(result2);

                        Assert.IsTrue(answer2.WhenReturned.Wait(MediumTimeout));
                        var outresult2 = answer2.WhenReturned.Result;
                        Assert.AreEqual(ObjectKind.Struct, outresult2.Kind);
                        Assert.AreEqual(222222, outresult2.ReadDataInt(0));
                    }
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void PipelineAfterReturn()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                client.WhenConnected.Wait();
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args, true))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumTimeout));

                    (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                    Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                    Assert.AreEqual<ushort>(0x3333, methodId);
                    Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                    Assert.AreEqual(123456, inargs.ReadDataInt(0));

                    var mock2 = new ProvidedCapabilityMock();

                    var result = DynamicSerializerState.CreateForRpc();
                    result.SetStruct(1, 2);
                    result.WriteData(0, 654321);
                    uint id = result.ProvideCapability(mock2);
                    result.LinkToCapability(1, id);

                    mock.Return.SetResult(result);

                    using (var pipelined = (BareProxy)CapabilityReflection.CreateProxy<BareProxy>(
                        answer.Access(
                            new MemberAccessPath(
                                new MemberAccessPath.MemberAccess[] {
                                    new MemberAccessPath.StructMemberAccess(1) }))))
                    {
                        var args2 = DynamicSerializerState.CreateForRpc();
                        args2.SetStruct(1, 0);
                        args2.WriteData(0, 654321);

                        using (var answer2 = pipelined.Call(0x8765432187654321, 0x4444, args2, false))
                        {
                            Assert.IsTrue(answer.WhenReturned.Wait(MediumTimeout));
                            Assert.IsTrue(mock2.WhenCalled.Wait(MediumTimeout));

                            (var interfaceId2, var methodId2, var inargs2, var ct2) = mock2.WhenCalled.Result;
                            Assert.AreEqual<ulong>(0x8765432187654321, interfaceId2);
                            Assert.AreEqual<ushort>(0x4444, methodId2);
                            Assert.AreEqual(ObjectKind.Struct, inargs2.Kind);
                            Assert.AreEqual(654321, inargs2.ReadDataInt(0));

                            var result2 = DynamicSerializerState.CreateForRpc();
                            result2.SetStruct(1, 0);
                            result2.WriteData(0, 222222);
                            mock2.Return.SetResult(result2);

                            Assert.IsTrue(answer2.WhenReturned.Wait(MediumTimeout));
                            var outresult2 = answer2.WhenReturned.Result;
                            Assert.AreEqual(ObjectKind.Struct, outresult2.Kind);
                            Assert.AreEqual(222222, outresult2.ReadDataInt(0));
                        }
                    }

                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void PipelineMultiple()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                client.WhenConnected.Wait();
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args, true))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumTimeout));

                    var pipelined = (BareProxy)CapabilityReflection.CreateProxy<BareProxy>(answer.Access(new MemberAccessPath(new MemberAccessPath.MemberAccess[] { new MemberAccessPath.StructMemberAccess(1) })));

                    var args2 = DynamicSerializerState.CreateForRpc();
                    args2.SetStruct(1, 0);
                    args2.WriteData(0, 111111);

                    var args3 = DynamicSerializerState.CreateForRpc();
                    args3.SetStruct(1, 0);
                    args3.WriteData(0, 222222);

                    using (var answer2 = pipelined.Call(0x1111111111111111, 0x1111, args2, false))
                    using (var answer3 = pipelined.Call(0x2222222222222222, 0x2222, args3, false))
                    {
                        (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;

                        Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                        Assert.AreEqual<ushort>(0x3333, methodId);
                        Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                        Assert.AreEqual(123456, inargs.ReadDataInt(0));

                        var mock2 = new ProvidedCapabilityMultiCallMock();

                        var result = DynamicSerializerState.CreateForRpc();
                        result.SetStruct(1, 2);
                        result.WriteData(0, 654321);
                        uint id = result.ProvideCapability(mock2);
                        result.LinkToCapability(1, id);

                        mock.Return.SetResult(result);

                        Assert.IsTrue(answer.WhenReturned.Wait(MediumTimeout));
                        Assert.IsFalse(ct.IsCancellationRequested);

                        var args4 = DynamicSerializerState.CreateForRpc();
                        args4.SetStruct(1, 0);
                        args4.WriteData(0, 333333);

                        var args5 = DynamicSerializerState.CreateForRpc();
                        args5.SetStruct(1, 0);
                        args5.WriteData(0, 444444);

                        using (var answer4 = pipelined.Call(0x3333333333333333, 0x3333, args4, false))
                        using (var answer5 = pipelined.Call(0x4444444444444444, 0x4444, args5, false))
                        {
                            var call2 = mock2.WhenCalled;
                            var call3 = mock2.WhenCalled;
                            var call4 = mock2.WhenCalled;
                            var call5 = mock2.WhenCalled;

                            Assert.IsTrue(call2.Wait(MediumTimeout));
                            Assert.IsTrue(call3.Wait(MediumTimeout));
                            Assert.IsTrue(call4.Wait(MediumTimeout));
                            Assert.IsTrue(call5.Wait(MediumTimeout));

                            Assert.AreEqual<ulong>(0x1111111111111111, call2.Result.InterfaceId);
                            Assert.AreEqual<ulong>(0x2222222222222222, call3.Result.InterfaceId);
                            Assert.AreEqual<ulong>(0x3333333333333333, call4.Result.InterfaceId);
                            Assert.AreEqual<ulong>(0x4444444444444444, call5.Result.InterfaceId);

                            var ret2 = DynamicSerializerState.CreateForRpc();
                            ret2.SetStruct(1, 0);
                            ret2.WriteData(0, -1);
                            call2.Result.Result.SetResult(ret2);

                            var ret3 = DynamicSerializerState.CreateForRpc();
                            ret3.SetStruct(1, 0);
                            ret3.WriteData(0, -2);
                            call3.Result.Result.SetResult(ret3);

                            var ret4 = DynamicSerializerState.CreateForRpc();
                            ret4.SetStruct(1, 0);
                            ret4.WriteData(0, -3);
                            call4.Result.Result.SetResult(ret4);

                            var ret5 = DynamicSerializerState.CreateForRpc();
                            ret5.SetStruct(1, 0);
                            ret5.WriteData(0, -4);
                            call5.Result.Result.SetResult(ret5);

                            Assert.IsTrue(answer2.WhenReturned.Wait(MediumTimeout));
                            Assert.IsTrue(answer3.WhenReturned.Wait(MediumTimeout));
                            Assert.IsTrue(answer4.WhenReturned.Wait(MediumTimeout));
                            Assert.IsTrue(answer5.WhenReturned.Wait(MediumTimeout));

                            Assert.AreEqual(-1, answer2.WhenReturned.Result.ReadDataInt(0));
                            Assert.AreEqual(-2, answer3.WhenReturned.Result.ReadDataInt(0));
                            Assert.AreEqual(-3, answer4.WhenReturned.Result.ReadDataInt(0));
                            Assert.AreEqual(-4, answer5.WhenReturned.Result.ReadDataInt(0));
                        }
                    }
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void PipelineCallAfterDisposal()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                client.WhenConnected.Wait();
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                BareProxy pipelined;
                using (var answer = main.Call(0x1234567812345678, 0x3333, args, true))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumTimeout));

                    pipelined = (BareProxy)CapabilityReflection.CreateProxy<BareProxy>(
                        answer.Access(new MemberAccessPath(new MemberAccessPath.MemberAccess[] { new MemberAccessPath.StructMemberAccess(1) })));

                }

                var args2 = DynamicSerializerState.CreateForRpc();
                args2.SetStruct(1, 0);
                args2.WriteData(0, 654321);

                try
                {
                    pipelined.Call(0x8765432187654321, 0x4444, args2, false);
                    Assert.Fail("Expected an exception here");
                }
                catch (ObjectDisposedException)
                {
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        [TestMethod, Timeout(10000)]
        public void PipelineCallDuringDisposal()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                client.WhenConnected.Wait();
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                IPromisedAnswer answer2;
                using (var answer = main.Call(0x1234567812345678, 0x3333, args, true))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumTimeout));

                    var pipelined = (BareProxy)CapabilityReflection.CreateProxy<BareProxy>(answer.Access(new MemberAccessPath(new MemberAccessPath.MemberAccess[] { new MemberAccessPath.StructMemberAccess(1) })));

                    var args2 = DynamicSerializerState.CreateForRpc();
                    args2.SetStruct(1, 0);
                    args2.WriteData(0, 654321);

                    answer2 = pipelined.Call(0x8765432187654321, 0x4444, args2, false);
                }

                using (answer2)
                {
                    (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;

                    var tcs = new TaskCompletionSource<int>();
                    using (ct.Register(() => tcs.SetResult(0)))
                    {
                        Assert.IsTrue(tcs.Task.Wait(MediumTimeout));
                    }

                    var mock2 = new ProvidedCapabilityMock();

                    var result = DynamicSerializerState.CreateForRpc();
                    result.SetStruct(1, 2);
                    result.WriteData(0, 654321);
                    uint id = result.ProvideCapability(mock2);
                    result.LinkToCapability(1, id);

                    mock.Return.SetResult(result);

                    Assert.IsTrue(Assert.ThrowsExceptionAsync<TaskCanceledException>(
                        () => answer2.WhenReturned).Wait(MediumTimeout));
                }
            }
        }
    }
}
