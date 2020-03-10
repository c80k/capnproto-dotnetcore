using Capnp;
using Capnp.Net.Runtime.Tests.GenImpls;
using Capnp.Rpc;
using Capnproto_test.Capnp.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class TcpRpcErrorHandling: TestBase
    {
        class MemStreamEndpoint : IEndpoint
        {
            readonly FramePump _fromEnginePump;
            readonly BinaryReader _reader;

            public bool Dismissed { get; private set; }

            public MemStreamEndpoint()
            {
                var pipe = new Pipe();
                _fromEnginePump = new FramePump(pipe.Writer.AsStream());
                _reader = new BinaryReader(pipe.Reader.AsStream());
            }

            public void Dismiss()
            {
                Dismissed = true;
            }

            public void Forward(WireFrame frame)
            {
                _fromEnginePump.Send(frame);
            }

            public WireFrame ReadNextFrame()
            {
                return _reader.ReadWireFrame();
            }
        }

        class RpcEngineTester
        {
            readonly MemStreamEndpoint _fromEngine;

            public RpcEngineTester()
            {
                Engine = new RpcEngine();
                _fromEngine = new MemStreamEndpoint();
                RealEnd = Engine.AddEndpoint(_fromEngine);
            }

            public RpcEngine Engine { get; }
            public RpcEngine.RpcEndpoint RealEnd { get; }
            public bool IsDismissed => _fromEngine.Dismissed;

            public void Send(Action<Message.WRITER> build)
            {
                var mb = MessageBuilder.Create();
                mb.InitCapTable();
                build(mb.BuildRoot<Message.WRITER>());
                RealEnd.Forward(mb.Frame);
            }

            public void Recv(Action<Message.READER> verify)
            {
                var task = Task.Run(() => DeserializerState.CreateRoot(_fromEngine.ReadNextFrame()));
                Assert.IsTrue(task.Wait(MediumNonDbgTimeout), "reception timeout");
                verify(new Message.READER(task.Result));
            }

            public void ExpectAbort()
            {
                Recv(_ => { Assert.AreEqual(Message.WHICH.Abort, _.which); });
                Assert.IsTrue(IsDismissed);
                Assert.ThrowsException<InvalidOperationException>(
                    () => Send(_ => { _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 33; }));
            }
        }

        [TestMethod]
        public void DuplicateQuestion1()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => { _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 99; });
            tester.Send(_ => { _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 99; });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                bootCapId = _.Return.Results.CapTable[0].SenderHosted;
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void DuplicateQuestion2()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => { _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 99; });
            tester.Recv(_ => { 
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                bootCapId = _.Return.Results.CapTable[0].SenderHosted;
            });
            tester.Send(_ => { 
                _.which = Message.WHICH.Call; 
                _.Call.QuestionId = 99; 
                _.Call.Target.which = MessageTarget.WHICH.ImportedCap;
                _.Call.Target.ImportedCap = bootCapId;
                _.Call.InterfaceId = ((TypeIdAttribute)typeof(ITestInterface).GetCustomAttributes(typeof(TypeIdAttribute), false)[0]).Id;
                _.Call.MethodId = 0;
                _.Call.Params.Content.Rewrap<TestInterface.Params_foo.WRITER>();
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void DuplicateQuestion3()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => { _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 99; });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                bootCapId = _.Return.Results.CapTable[0].SenderHosted;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Call;
                _.Call.QuestionId = 42;
                _.Call.Target.which = MessageTarget.WHICH.ImportedCap;
                _.Call.Target.ImportedCap = bootCapId;
                _.Call.InterfaceId = ((TypeIdAttribute)typeof(ITestInterface).GetCustomAttributes(typeof(TypeIdAttribute), false)[0]).Id;
                _.Call.MethodId = 0;
                var wr = _.Call.Params.Content.Rewrap<TestInterface.Params_foo.WRITER>();
                wr.I = 123u;
                wr.J = true;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Call;
                _.Call.QuestionId = 42;
                _.Call.Target.which = MessageTarget.WHICH.ImportedCap;
                _.Call.Target.ImportedCap = bootCapId;
                _.Call.InterfaceId = ((TypeIdAttribute)typeof(ITestInterface).GetCustomAttributes(typeof(TypeIdAttribute), false)[0]).Id;
                _.Call.MethodId = 0;
                _.Call.Params.Content.Rewrap<TestInterface.Params_foo.WRITER>();
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void NoBootstrap()
        {
            var tester = new RpcEngineTester();

            tester.Send(_ => { _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 0; });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Exception, _.Return.which);
            });
            Assert.IsFalse(tester.IsDismissed);
            tester.Engine.Main = new TestInterfaceImpl(new Counters());
            tester.Send(_ => { _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 1; });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
            });
        }

        [TestMethod]
        public void DuplicateFinish()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => { 
                _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 99; });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                bootCapId = _.Return.Results.CapTable[0].SenderHosted;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Finish;
                _.Finish.QuestionId = 99;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Finish;
                _.Finish.QuestionId = 99;
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void DuplicateAnswer()
        {
            var tester = new RpcEngineTester();

            var cap = tester.RealEnd.QueryMain();
            var proxy = new BareProxy(cap);
            Assert.IsFalse(proxy.WhenResolved.IsCompleted);
            uint id = 0;

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Bootstrap, _.which);
                id = _.Bootstrap.QuestionId;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Return;
                _.Return.which = Return.WHICH.Results;
                _.Return.Results.CapTable.Init(1);
                _.Return.Results.CapTable[0].which = CapDescriptor.WHICH.SenderHosted;
                _.Return.Results.CapTable[0].SenderHosted = 1;
            });
            Assert.IsTrue(proxy.WhenResolved.IsCompleted);
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Finish, _.which);
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Return;
                _.Return.which = Return.WHICH.Results;
                _.Return.Results.CapTable.Init(1);
                _.Return.Results.CapTable[0].which = CapDescriptor.WHICH.SenderHosted;
                _.Return.Results.CapTable[0].SenderHosted = 1;
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void InvalidReceiverHosted()
        {
            var tester = new RpcEngineTester();

            var cap = tester.RealEnd.QueryMain();
            var proxy = new BareProxy(cap);
            Assert.IsFalse(proxy.WhenResolved.IsCompleted);
            uint id = 0;

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Bootstrap, _.which);
                id = _.Bootstrap.QuestionId;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Return;
                _.Return.which = Return.WHICH.Results;
                _.Return.Results.CapTable.Init(1);
                _.Return.Results.CapTable[0].which = CapDescriptor.WHICH.ReceiverHosted;
                _.Return.Results.CapTable[0].ReceiverHosted = 0;
            });
            Assert.IsTrue(proxy.WhenResolved.IsCompleted);
            tester.ExpectAbort();
        }

        [TestMethod]
        public void InvalidReceiverAnswer()
        {
            var tester = new RpcEngineTester();

            var cap = tester.RealEnd.QueryMain();
            var proxy = new BareProxy(cap);
            Assert.IsFalse(proxy.WhenResolved.IsCompleted);
            uint id = 0;

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Bootstrap, _.which);
                id = _.Bootstrap.QuestionId;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Return;
                _.Return.which = Return.WHICH.Results;
                _.Return.Results.CapTable.Init(1);
                _.Return.Results.CapTable[0].which = CapDescriptor.WHICH.ReceiverAnswer;
                _.Return.Results.CapTable[0].ReceiverAnswer.QuestionId = 0;
                _.Return.Results.CapTable[0].ReceiverAnswer.Transform.Init(1);
                _.Return.Results.CapTable[0].ReceiverAnswer.Transform[0].which = PromisedAnswer.Op.WHICH.GetPointerField;
                _.Return.Results.CapTable[0].ReceiverAnswer.Transform[0].GetPointerField = 0;
            });
            Assert.IsTrue(proxy.WhenResolved.IsCompleted);
            tester.ExpectAbort();
        }

        [TestMethod]
        public void DuplicateResolve()
        {
            var tester = new RpcEngineTester();

            var cap = tester.RealEnd.QueryMain();
            var proxy = new BareProxy(cap);
            Assert.IsFalse(proxy.WhenResolved.IsCompleted);
            uint id = 0;

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Bootstrap, _.which);
                id = _.Bootstrap.QuestionId;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Return;
                _.Return.which = Return.WHICH.Results;
                _.Return.Results.CapTable.Init(1);
                _.Return.Results.CapTable[0].which = CapDescriptor.WHICH.SenderPromise;
                _.Return.Results.CapTable[0].SenderPromise = 0;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Resolve;
                _.Resolve.which = Resolve.WHICH.Cap;
                _.Resolve.Cap.which = CapDescriptor.WHICH.SenderHosted;
                _.Resolve.Cap.SenderHosted = 1;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Finish, _.which);
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Resolve;
                _.Resolve.which = Resolve.WHICH.Exception;
                _.Resolve.Exception.Reason = "problem";
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Release, _.which);
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void DuplicateRelease1()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => {
                _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 99;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                bootCapId = _.Return.Results.CapTable[0].SenderHosted;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Release;
                _.Release.Id = bootCapId;
                _.Release.ReferenceCount = 1;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Release;
                _.Release.Id = bootCapId;
                _.Release.ReferenceCount = 1;
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void DuplicateRelease2()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => {
                _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 99;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                bootCapId = _.Return.Results.CapTable[0].SenderHosted;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Release;
                _.Release.Id = bootCapId;
                _.Release.ReferenceCount = 2;
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void UnimplementedAccept()
        {
            var tester = new RpcEngineTester();

            tester.Send(_ => {
                _.which = Message.WHICH.Accept;
                _.Accept.Embargo = true;
                _.Accept.QuestionId = 47;
                _.Accept.Provision.SetStruct(1, 0);
                _.Accept.Provision.Allocate();
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
                Assert.AreEqual(Message.WHICH.Accept, _.Unimplemented.which);
                Assert.IsTrue(_.Unimplemented.Accept.Embargo);
                Assert.AreEqual(47u, _.Unimplemented.Accept.QuestionId);
                Assert.AreEqual(1, _.Unimplemented.Accept.Provision.StructDataCount);
                Assert.AreEqual(0, _.Unimplemented.Accept.Provision.StructPtrCount);
            });
            Assert.IsFalse(tester.IsDismissed);
        }

        [TestMethod]
        public void UnimplementedJoin()
        {
            var tester = new RpcEngineTester();

            tester.Send(_ => {
                _.which = Message.WHICH.Join;
                _.Join.QuestionId = 74;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
                Assert.AreEqual(Message.WHICH.Join, _.Unimplemented.which);
                Assert.AreEqual(74u, _.Unimplemented.Join.QuestionId);
            });
            Assert.IsFalse(tester.IsDismissed);
        }

        [TestMethod]
        public void UnimplementedProvide()
        {
            var tester = new RpcEngineTester();

            tester.Send(_ => {
                _.which = Message.WHICH.Provide;
                _.Provide.QuestionId = 666;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
                Assert.AreEqual(Message.WHICH.Provide, _.Unimplemented.which);
                Assert.AreEqual(666u, _.Unimplemented.Provide.QuestionId);
            });
            Assert.IsFalse(tester.IsDismissed);
        }

        [TestMethod]
        public void UnimplementedObsoleteDelete()
        {
            var tester = new RpcEngineTester();

            tester.Send(_ => {
                _.which = Message.WHICH.ObsoleteDelete;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
                Assert.AreEqual(Message.WHICH.ObsoleteDelete, _.Unimplemented.which);
            });
            Assert.IsFalse(tester.IsDismissed);
        }

        [TestMethod]
        public void UnimplementedObsoleteSave()
        {
            var tester = new RpcEngineTester();

            tester.Send(_ => {
                _.which = Message.WHICH.ObsoleteSave;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
                Assert.AreEqual(Message.WHICH.ObsoleteSave, _.Unimplemented.which);
            });
            Assert.IsFalse(tester.IsDismissed);
        }

        [TestMethod]
        public void UnimplementedUnknown()
        {
            var tester = new RpcEngineTester();

            tester.Send(_ => {
                _.which = (Message.WHICH)123;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
                Assert.AreEqual((Message.WHICH)123, _.Unimplemented.which);
            });
            Assert.IsFalse(tester.IsDismissed);
        }

        class TestPipelineImpl3 : ITestPipeline
        {
            readonly TestPipelineImpl2 _impl;
            readonly ITestPipeline _proxy;

            public TestPipelineImpl3(Task complete)
            {
                _impl = new TestPipelineImpl2(complete);
                var bproxy = BareProxy.FromImpl(_impl);
                _proxy = bproxy.Cast<ITestPipeline>(true);
            }

            public void Dispose()
            {
            }

            public bool IsGrandsonCapDisposed => _impl.IsChildCapDisposed;

            public Task<(string, TestPipeline.AnyBox)> GetAnyCap(uint n, BareProxy inCap, CancellationToken cancellationToken_ = default)
            {
                throw new NotImplementedException();
            }

            public Task<(string, TestPipeline.Box)> GetCap(uint n, ITestInterface inCap, CancellationToken cancellationToken_ = default)
            {
                return Task.FromResult(("foo", new TestPipeline.Box() { Cap = _proxy.GetCap(0, null).OutBox_Cap() }));
            }

            public Task TestPointers(ITestInterface cap, object obj, IReadOnlyList<ITestInterface> list, CancellationToken cancellationToken_ = default)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void UnimplementedResolve()
        {
            var tcs = new TaskCompletionSource<int>();
            var tester = new RpcEngineTester();
            var impl = new TestPipelineImpl3(tcs.Task);
            tester.Engine.Main = impl;

            uint bootCapId = 0;

            tester.Send(_ => {
                _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 99;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                bootCapId = _.Return.Results.CapTable[0].SenderHosted;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Call;
                _.Call.QuestionId = 42;
                _.Call.Target.which = MessageTarget.WHICH.ImportedCap;
                _.Call.Target.ImportedCap = bootCapId;
                _.Call.InterfaceId = new TestPipeline_Skeleton().InterfaceId;
                _.Call.MethodId = 0;
                var wr = _.Call.Params.Content.Rewrap<TestPipeline.Params_getCap.WRITER>();
                wr.InCap = null;
                _.Call.Params.CapTable.Init(1);
                _.Call.Params.CapTable[0].which = CapDescriptor.WHICH.ReceiverHosted;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                Assert.AreEqual(CapDescriptor.WHICH.SenderPromise, _.Return.Results.CapTable[0].which);
            });
            tcs.SetResult(0);
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Resolve, _.which);
                Assert.AreEqual(Resolve.WHICH.Cap, _.Resolve.which);
                Assert.AreEqual(CapDescriptor.WHICH.SenderHosted, _.Resolve.Cap.which);

                Assert.IsFalse(impl.IsGrandsonCapDisposed);

                tester.Send(_1 =>
                {
                    _1.which = Message.WHICH.Unimplemented;
                    _1.Unimplemented.which = Message.WHICH.Resolve;
                    Reserializing.DeepCopy(_.Resolve, _1.Unimplemented.Resolve);
                });

                Assert.IsFalse(impl.IsGrandsonCapDisposed);

                tester.Send(_1 =>
                {
                    _1.which = Message.WHICH.Finish;
                    _1.Finish.QuestionId = 42;
                    _1.Finish.ReleaseResultCaps = true;
                });

                Assert.IsTrue(impl.IsGrandsonCapDisposed);
            });
            Assert.IsFalse(tester.IsDismissed);
        }
    }
}
