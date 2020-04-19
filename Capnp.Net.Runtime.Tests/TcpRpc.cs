using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Capnp.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using Capnp.Net.Runtime.Tests.GenImpls;
using Capnproto_test.Capnp.Test;

namespace Capnp.Net.Runtime.Tests
{

    [TestClass]
    [TestCategory("Coverage")]
    public class TcpRpc
    {
        public static int TcpPort = 49153;

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

            Logging.LoggerFactory?.Dispose();
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            Logging.LoggerFactory = new LoggerFactory().AddConsole((msg, level) =>
            {
                if (!ExpectingLogOutput && level != LogLevel.Debug)
                {
                    Assert.Fail("Did not expect any logging output, but got this: " + msg);
                }
                return true;
            });
#pragma warning restore CS0618 // Typ oder Element ist veraltet
        }

        int MediumNonDbgTimeout => Debugger.IsAttached ? Timeout.Infinite : 2000;

        [TestMethod]
        public void CreateAndDispose()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
            }
        }

        [TestMethod]
        public void ConnectAndDispose()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                try
                {
                    Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                    SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                    Assert.AreEqual(1, server.ConnectionCount);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        [TestMethod]
        public void ConnectNoServer()
        {
            using (var client = new TcpRpcClient("localhost", TcpPort))
            {
                Assert.IsTrue(Assert.ThrowsExceptionAsync<RpcException>(() => client.WhenConnected).Wait(10000));
            }
        }

        [TestMethod]
        public void ConnectAndBootstrap()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                server.Main = new ProvidedCapabilityMock();
                var main = client.GetMain<BareProxy>();
                var resolving = main as IResolvingCapability;
                Assert.IsTrue(resolving.WhenResolved.Wait(MediumNonDbgTimeout));
            }
        }

        [TestMethod]
        public void ConnectNoBootstrap()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var main = client.GetMain<BareProxy>();
                var resolving = main as IResolvingCapability;
                Assert.IsTrue(Assert.ThrowsExceptionAsync<RpcException>(() => resolving.WhenResolved).Wait(MediumNonDbgTimeout));
            }
        }

        [TestMethod]
        public void CallReturn()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumNonDbgTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumNonDbgTimeout));
                    (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                    Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                    Assert.AreEqual<ushort>(0x3333, methodId);
                    Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                    Assert.AreEqual(123456, inargs.ReadDataInt(0));

                    var result = DynamicSerializerState.CreateForRpc();
                    result.SetStruct(1, 0);
                    result.WriteData(0, 654321);
                    mock.Return.SetResult(result);

                    Assert.IsTrue(answer.WhenReturned.Wait(MediumNonDbgTimeout));
                    var outresult = answer.WhenReturned.Result;
                    Assert.AreEqual(ObjectKind.Struct, outresult.Kind);
                    Assert.AreEqual(654321, outresult.ReadDataInt(0));
                }
            }
        }

        [TestMethod]
        public void CallCancelOnServer()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumNonDbgTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumNonDbgTimeout));
                    (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                    Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                    Assert.AreEqual<ushort>(0x3333, methodId);
                    Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                    Assert.AreEqual(123456, inargs.ReadDataInt(0));

                    mock.Return.SetCanceled();

                    Assert.IsTrue(Assert.ThrowsExceptionAsync<TaskCanceledException>(() => answer.WhenReturned).Wait(MediumNonDbgTimeout));
                }
            }
        }

        [TestMethod]
        public void CallCancelOnClient()
        {
            ExpectingLogOutput = false;

            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                try
                {
                    Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                    SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                    Assert.AreEqual(1, server.ConnectionCount);

                    var mock = new ProvidedCapabilityMock();
                    server.Main = mock;
                    var main = client.GetMain<BareProxy>();
                    var resolving = main as IResolvingCapability;
                    Assert.IsTrue(resolving.WhenResolved.Wait(MediumNonDbgTimeout));
                    var args = DynamicSerializerState.CreateForRpc();
                    args.SetStruct(1, 0);
                    args.WriteData(0, 123456);
                    CancellationToken ctx;
                    using (var answer = main.Call(0x1234567812345678, 0x3333, args))
                    {
                        Assert.IsTrue(mock.WhenCalled.Wait(MediumNonDbgTimeout));
                        (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                        Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                        Assert.AreEqual<ushort>(0x3333, methodId);
                        Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                        Assert.AreEqual(123456, inargs.ReadDataInt(0));
                        ctx = ct;
                    }

                    Assert.IsTrue(SpinWait.SpinUntil(() => ctx.IsCancellationRequested, MediumNonDbgTimeout));
                }
                finally
                {
                    ExpectingLogOutput = true;
                }
            }
        }

        [TestMethod]
        public void CallReturnAfterClientSideCancel()
        {
            ExpectingLogOutput = false;
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                try
                {
                    Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                    SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                    Assert.AreEqual(1, server.ConnectionCount);

                    var mock = new ProvidedCapabilityMock();
                    server.Main = mock;
                    var main = client.GetMain<BareProxy>();
                    Assert.IsTrue(main.WhenResolved.Wait(MediumNonDbgTimeout));
                    var args = DynamicSerializerState.CreateForRpc();
                    args.SetStruct(1, 0);
                    args.WriteData(0, 123456);
                    CancellationToken ctx;
                    IPromisedAnswer answer;
                    using (answer = main.Call(0x1234567812345678, 0x3333, args))
                    {
                        Assert.IsTrue(mock.WhenCalled.Wait(MediumNonDbgTimeout));
                        (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                        Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                        Assert.AreEqual<ushort>(0x3333, methodId);
                        Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                        Assert.AreEqual(123456, inargs.ReadDataInt(0));
                        ctx = ct;
                    }

                    Assert.IsTrue(SpinWait.SpinUntil(() => ctx.IsCancellationRequested, MediumNonDbgTimeout));

                    var mbr = MessageBuilder.Create();
                    mbr.InitCapTable();
                    var result = new DynamicSerializerState(mbr);
                    result.SetStruct(1, 0);
                    result.WriteData(0, 654321);
                    mock.Return.SetResult(result);

                    // Even after the client cancelled the call, the server must still send
                    // a response.
                    Assert.IsTrue(answer.WhenReturned.ContinueWith(t => { }).Wait(MediumNonDbgTimeout));
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

        [TestMethod]
        public void CallServerSideException()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumNonDbgTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumNonDbgTimeout));
                    (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                    Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                    Assert.AreEqual<ushort>(0x3333, methodId);
                    Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                    Assert.AreEqual(123456, inargs.ReadDataInt(0));

                    mock.Return.SetException(new MyTestException());

                    var exTask = Assert.ThrowsExceptionAsync<RpcException>(() => answer.WhenReturned);
                    Assert.IsTrue(exTask.Wait(MediumNonDbgTimeout));
                    Assert.IsTrue(exTask.Result.Message.Contains(new MyTestException().Message));
                }
            }
        }

        [TestMethod]
        public void PipelineBeforeReturn()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumNonDbgTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumNonDbgTimeout));

                    var pipelined = (BareProxy)CapabilityReflection.CreateProxy<BareProxy>(answer.Access(
                        new MemberAccessPath(new MemberAccessPath.MemberAccess[] { new MemberAccessPath.StructMemberAccess(1) })));

                    var args2 = DynamicSerializerState.CreateForRpc();
                    args2.SetStruct(1, 0);
                    args2.WriteData(0, 654321);

                    using (var answer2 = pipelined.Call(0x8765432187654321, 0x4444, args2))
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
                        uint id = result.ProvideCapability(mock2).Value;
                        result.LinkToCapability(1, id);

                        mock.Return.SetResult(result);

                        Assert.IsTrue(answer.WhenReturned.Wait(MediumNonDbgTimeout));
                        Assert.IsFalse(ct.IsCancellationRequested);

                        Assert.IsTrue(mock2.WhenCalled.Wait(MediumNonDbgTimeout));

                        (var interfaceId2, var methodId2, var inargs2, var ct2) = mock2.WhenCalled.Result;
                        Assert.AreEqual<ulong>(0x8765432187654321, interfaceId2);
                        Assert.AreEqual<ushort>(0x4444, methodId2);
                        Assert.AreEqual(ObjectKind.Struct, inargs2.Kind);
                        Assert.AreEqual(654321, inargs2.ReadDataInt(0));

                        var result2 = DynamicSerializerState.CreateForRpc();
                        result2.SetStruct(1, 0);
                        result2.WriteData(0, 222222);
                        mock2.Return.SetResult(result2);

                        Assert.IsTrue(answer2.WhenReturned.Wait(MediumNonDbgTimeout));
                        var outresult2 = answer2.WhenReturned.Result;
                        Assert.AreEqual(ObjectKind.Struct, outresult2.Kind);
                        Assert.AreEqual(222222, outresult2.ReadDataInt(0));
                    }
                }
            }
        }

        [TestMethod]
        public void PipelineAfterReturn()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumNonDbgTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumNonDbgTimeout));

                    (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;
                    Assert.AreEqual<ulong>(0x1234567812345678, interfaceId);
                    Assert.AreEqual<ushort>(0x3333, methodId);
                    Assert.AreEqual(ObjectKind.Struct, inargs.Kind);
                    Assert.AreEqual(123456, inargs.ReadDataInt(0));

                    var mock2 = new ProvidedCapabilityMock();

                    var result = DynamicSerializerState.CreateForRpc();
                    result.SetStruct(1, 2);
                    result.WriteData(0, 654321);
                    uint id = result.ProvideCapability(mock2).Value;
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

                        using (var answer2 = pipelined.Call(0x8765432187654321, 0x4444, args2))
                        {
                            Assert.IsTrue(answer.WhenReturned.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(mock2.WhenCalled.Wait(MediumNonDbgTimeout));

                            (var interfaceId2, var methodId2, var inargs2, var ct2) = mock2.WhenCalled.Result;
                            Assert.AreEqual<ulong>(0x8765432187654321, interfaceId2);
                            Assert.AreEqual<ushort>(0x4444, methodId2);
                            Assert.AreEqual(ObjectKind.Struct, inargs2.Kind);
                            Assert.AreEqual(654321, inargs2.ReadDataInt(0));

                            var result2 = DynamicSerializerState.CreateForRpc();
                            result2.SetStruct(1, 0);
                            result2.WriteData(0, 222222);
                            mock2.Return.SetResult(result2);

                            Assert.IsTrue(answer2.WhenReturned.Wait(MediumNonDbgTimeout));
                            var outresult2 = answer2.WhenReturned.Result;
                            Assert.AreEqual(ObjectKind.Struct, outresult2.Kind);
                            Assert.AreEqual(222222, outresult2.ReadDataInt(0));
                        }
                    }

                }
            }
        }

        [TestMethod]
        public void PipelineMultiple()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumNonDbgTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                using (var answer = main.Call(0x1234567812345678, 0x3333, args))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumNonDbgTimeout));

                    var pipelined = (BareProxy)CapabilityReflection.CreateProxy<BareProxy>(answer.Access(new MemberAccessPath(new MemberAccessPath.MemberAccess[] { new MemberAccessPath.StructMemberAccess(1) })));

                    var args2 = DynamicSerializerState.CreateForRpc();
                    args2.SetStruct(1, 0);
                    args2.WriteData(0, 111111);

                    var args3 = DynamicSerializerState.CreateForRpc();
                    args3.SetStruct(1, 0);
                    args3.WriteData(0, 222222);

                    using (var answer2 = pipelined.Call(0x1111111111111111, 0x1111, args2))
                    using (var answer3 = pipelined.Call(0x2222222222222222, 0x2222, args3))
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
                        uint id = result.ProvideCapability(mock2).Value;
                        result.LinkToCapability(1, id);

                        mock.Return.SetResult(result);

                        Assert.IsTrue(answer.WhenReturned.Wait(MediumNonDbgTimeout));
                        Assert.IsFalse(ct.IsCancellationRequested);

                        var args4 = DynamicSerializerState.CreateForRpc();
                        args4.SetStruct(1, 0);
                        args4.WriteData(0, 333333);

                        var args5 = DynamicSerializerState.CreateForRpc();
                        args5.SetStruct(1, 0);
                        args5.WriteData(0, 444444);

                        using (var answer4 = pipelined.Call(0x3333333333333333, 0x3333, args4))
                        using (var answer5 = pipelined.Call(0x4444444444444444, 0x4444, args5))
                        {
                            var call2 = mock2.WhenCalled;
                            var call3 = mock2.WhenCalled;
                            var call4 = mock2.WhenCalled;
                            var call5 = mock2.WhenCalled;

                            Assert.IsTrue(call2.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(call3.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(call4.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(call5.Wait(MediumNonDbgTimeout));

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

                            Assert.IsTrue(answer2.WhenReturned.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(answer3.WhenReturned.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(answer4.WhenReturned.Wait(MediumNonDbgTimeout));
                            Assert.IsTrue(answer5.WhenReturned.Wait(MediumNonDbgTimeout));

                            Assert.AreEqual(-1, answer2.WhenReturned.Result.ReadDataInt(0));
                            Assert.AreEqual(-2, answer3.WhenReturned.Result.ReadDataInt(0));
                            Assert.AreEqual(-3, answer4.WhenReturned.Result.ReadDataInt(0));
                            Assert.AreEqual(-4, answer5.WhenReturned.Result.ReadDataInt(0));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void PipelineCallAfterDisposal()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumNonDbgTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                BareProxy pipelined;
                using (var answer = main.Call(0x1234567812345678, 0x3333, args))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumNonDbgTimeout));

                    pipelined = (BareProxy)CapabilityReflection.CreateProxy<BareProxy>(
                        answer.Access(new MemberAccessPath(new MemberAccessPath.MemberAccess[] { new MemberAccessPath.StructMemberAccess(1) })));

                }

                var args2 = DynamicSerializerState.CreateForRpc();
                args2.SetStruct(1, 0);
                args2.WriteData(0, 654321);

                try
                {
                    pipelined.Call(0x8765432187654321, 0x4444, args2);
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

        [TestMethod]
        public void PipelineCallDuringDisposal()
        {
            (var server, var client) = SetupClientServerPair();

            using (server)
            using (client)
            {
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                SpinWait.SpinUntil(() => server.ConnectionCount > 0, MediumNonDbgTimeout);
                Assert.AreEqual(1, server.ConnectionCount);

                var mock = new ProvidedCapabilityMock();
                server.Main = mock;
                var main = client.GetMain<BareProxy>();
                Assert.IsTrue(main.WhenResolved.Wait(MediumNonDbgTimeout));
                var args = DynamicSerializerState.CreateForRpc();
                args.SetStruct(1, 0);
                args.WriteData(0, 123456);
                IPromisedAnswer answer2;
                using (var answer = main.Call(0x1234567812345678, 0x3333, args))
                {
                    Assert.IsTrue(mock.WhenCalled.Wait(MediumNonDbgTimeout));

                    var pipelined = (BareProxy)CapabilityReflection.CreateProxy<BareProxy>(answer.Access(new MemberAccessPath(new MemberAccessPath.MemberAccess[] { new MemberAccessPath.StructMemberAccess(1) })));

                    var args2 = DynamicSerializerState.CreateForRpc();
                    args2.SetStruct(1, 0);
                    args2.WriteData(0, 654321);

                    answer2 = pipelined.Call(0x8765432187654321, 0x4444, args2);
                }

                using (answer2)
                {
                    (var interfaceId, var methodId, var inargs, var ct) = mock.WhenCalled.Result;

                    var tcs = new TaskCompletionSource<int>();
                    using (ct.Register(() => tcs.SetResult(0)))
                    {
                        Assert.IsTrue(tcs.Task.Wait(MediumNonDbgTimeout));
                    }

                    var mock2 = new ProvidedCapabilityMock();

                    var result = DynamicSerializerState.CreateForRpc();
                    result.SetStruct(1, 2);
                    result.WriteData(0, 654321);
                    uint id = result.ProvideCapability(mock2).Value;
                    result.LinkToCapability(1, id);

                    mock.Return.SetResult(result);

                    Assert.IsTrue(Assert.ThrowsExceptionAsync<TaskCanceledException>(
                        () => answer2.WhenReturned).Wait(MediumNonDbgTimeout));
                }
            }
        }

        [TestMethod]
        public void Server1()
        {
            var cbb = new BufferBlock<IConnection>();
            var server = new TcpRpcServer();
            server.Main = new TestInterfaceImpl2();
            bool init = true;
            var tracer = new FrameTracing.RpcFrameTracer(Console.Out);
            server.OnConnectionChanged += (s, a) =>
            {
                var c = a.Connection;
                if (init)
                {
                    Assert.ThrowsException<ArgumentNullException>(() => c.AttachTracer(null));
                    c.AttachTracer(tracer);
                    Assert.ThrowsException<ArgumentNullException>(() => c.InjectMidlayer(null));
                    c.InjectMidlayer(_ => _);
                    Assert.IsFalse(c.IsComputing);
                    Assert.IsFalse(c.IsWaitingForData);
                    Assert.AreEqual(ConnectionState.Initializing, c.State);
                    Assert.IsNotNull(c.RemotePort);
                    Assert.AreEqual(TcpPort, c.LocalPort);
                    Assert.AreEqual(0L, c.RecvCount);
                    Assert.AreEqual(0L, c.SendCount);
                }
                else
                {
                    Assert.ThrowsException<InvalidOperationException>(() => c.AttachTracer(tracer));
                    Assert.ThrowsException<InvalidOperationException>(() => c.InjectMidlayer(_ => _));
                    Assert.AreEqual(ConnectionState.Down, c.State);
                }

                cbb.Post(c);
            };

            Assert.ThrowsException<InvalidOperationException>(() => server.StopListening());

            server.StartAccepting(IPAddress.Any, TcpPort);
            Assert.IsTrue(server.IsAlive);
            Assert.ThrowsException<InvalidOperationException>(() => server.StartAccepting(IPAddress.Any, TcpPort));

            var server2 = new TcpRpcServer();
            Assert.ThrowsException<SocketException>(() => server2.StartAccepting(IPAddress.Any, TcpPort));

            var client1 = new TcpRpcClient("localhost", TcpPort);
            var c1 = cbb.Receive(TimeSpan.FromMilliseconds(MediumNonDbgTimeout));
            Assert.IsNotNull(c1);
            Assert.AreEqual(1, server.ConnectionCount);
            Assert.AreEqual(c1, server.Connections[0]);
            Assert.AreEqual(ConnectionState.Active, c1.State);
            var proxy = client1.GetMain<ITestInterface>();
            Assert.IsTrue(proxy is IResolvingCapability r && r.WhenResolved.Wait(MediumNonDbgTimeout));
            Assert.IsTrue(c1.RecvCount > 0);
            Assert.IsTrue(c1.SendCount > 0);

            var client2 = new TcpRpcClient("localhost", TcpPort);
            var c2 = cbb.Receive(TimeSpan.FromMilliseconds(MediumNonDbgTimeout));
            Assert.IsNotNull(c2);
            Assert.AreEqual(2, server.ConnectionCount);
            Assert.AreEqual(c2, server.Connections[1]);

            init = false;

            client1.Dispose();
            var c1d = cbb.Receive(TimeSpan.FromMilliseconds(MediumNonDbgTimeout));
            Assert.IsNotNull(c1d);
            Assert.AreEqual(1, server.ConnectionCount);
            Assert.AreEqual(c2, server.Connections[0]);
            Assert.IsTrue(SpinWait.SpinUntil(() => c1d.State == ConnectionState.Down, MediumNonDbgTimeout));

            client2.Dispose();
            var c2d = cbb.Receive(TimeSpan.FromMilliseconds(MediumNonDbgTimeout));
            Assert.IsNotNull(c2d);
            Assert.AreEqual(0, server.ConnectionCount);
            Assert.IsTrue(SpinWait.SpinUntil(() => c2d.State == ConnectionState.Down, MediumNonDbgTimeout));

            server.StopListening();
            Assert.IsFalse(server.IsAlive);
            Assert.ThrowsException<InvalidOperationException>(() => server.StopListening());

            for (int i = 0; i < 100; i++)
            {
                server.StartAccepting(IPAddress.Any, TcpPort);
                Assert.IsTrue(server.IsAlive);
                server.StopListening();
                Assert.IsFalse(server.IsAlive);
            }

            server.Dispose();
        }

        [TestMethod]
        public void Server2()
        {
            var server = new TcpRpcServer();
            server.Main = new TestInterfaceImpl2();
            var tracer = new FrameTracing.RpcFrameTracer(Console.Out);
            server.OnConnectionChanged += (s, a) =>
            {
                server.Dispose();
            };

            server.StartAccepting(IPAddress.Any, TcpPort);

            var client1 = new TcpRpcClient("localhost", TcpPort);
            Assert.IsTrue(client1.WhenConnected.Wait(MediumNonDbgTimeout), "Did not connect");
            Assert.IsTrue(SpinWait.SpinUntil(() => client1.State == ConnectionState.Down, MediumNonDbgTimeout),
                $"Connection did not go down: {client1.State}");
        }

        [TestMethod]
        public void Server3()
        {
            using (var server = new TcpRpcServer())
            {
                server.Main = new TestInterfaceImpl2();
                var tracer = new FrameTracing.RpcFrameTracer(Console.Out);
                server.OnConnectionChanged += (s, a) =>
                {
                    a.Connection.Close();
                };

                server.StartAccepting(IPAddress.Any, TcpPort);

                var client1 = new TcpRpcClient("localhost", TcpPort);
                Assert.IsTrue(client1.WhenConnected.Wait(MediumNonDbgTimeout));
                Assert.IsTrue(SpinWait.SpinUntil(() => client1.State == ConnectionState.Down, MediumNonDbgTimeout));
            }
        }

        [TestMethod]
        public void Client()
        {
            using (var server = new TcpRpcServer())
            using (var client = new TcpRpcClient())
            {
                Assert.IsFalse(client.IsComputing);
                Assert.IsFalse(client.IsWaitingForData);
                Assert.AreEqual(0L, client.SendCount);
                Assert.AreEqual(0L, client.RecvCount);
                Assert.ThrowsException<InvalidOperationException>(() => client.GetMain<ITestInterface>());
                Assert.ThrowsException<ArgumentNullException>(() => client.AttachTracer(null));
                Assert.ThrowsException<ArgumentNullException>(() => client.InjectMidlayer(null));
                server.StartAccepting(IPAddress.Any, TcpPort);
                client.Connect("localhost", TcpPort);
                Assert.ThrowsException<InvalidOperationException>(() => client.Connect("localhost", TcpPort));
                Assert.IsTrue(client.WhenConnected.Wait(MediumNonDbgTimeout));
                Assert.ThrowsException<InvalidOperationException>(() => client.AttachTracer(new FrameTracing.RpcFrameTracer(Console.Out, false)));
                Assert.ThrowsException<InvalidOperationException>(() => client.InjectMidlayer(_ => _));
                Assert.AreEqual(TcpPort, client.RemotePort);
                Assert.IsTrue(client.LocalPort != 0);
                Assert.AreEqual(0L, client.SendCount);
                Assert.AreEqual(0L, client.RecvCount);
                Assert.IsTrue(SpinWait.SpinUntil(() => client.IsWaitingForData, MediumNonDbgTimeout));
                ((IConnection)client).Close();
                Assert.IsTrue(SpinWait.SpinUntil(() => client.State == ConnectionState.Down, MediumNonDbgTimeout));
            }
        }
    }
}
