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
    [TestCategory("Coverage")]
    public class EdgeCaseHandling: TestBase
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
                _.Call.Params.Content.Rewrap<TestInterface.Params_Foo.WRITER>();
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
                var wr = _.Call.Params.Content.Rewrap<TestInterface.Params_Foo.WRITER>();
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
                _.Call.Params.Content.Rewrap<TestInterface.Params_Foo.WRITER>();
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
        public void UnimplementedReturnAcceptFromThirdParty()
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
                _.Return.which = Return.WHICH.AcceptFromThirdParty;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
            });
        }

        [TestMethod]
        public void UnimplementedReturnUnknown()
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
                _.Return.which = (Return.WHICH)33;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
            });
        }

        [TestMethod]
        public void InvalidReturnTakeFromOtherQuestion()
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
                _.Return.which = Return.WHICH.TakeFromOtherQuestion;
                _.Return.TakeFromOtherQuestion = 1u;
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
        public void InvalidCallTargetImportedCap()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => { _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 0; });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                bootCapId = _.Return.Results.CapTable[0].SenderHosted;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Call;
                _.Call.QuestionId = 1;
                _.Call.Target.which = MessageTarget.WHICH.ImportedCap;
                _.Call.Target.ImportedCap = bootCapId + 1;
                _.Call.InterfaceId = ((TypeIdAttribute)typeof(ITestInterface).GetCustomAttributes(typeof(TypeIdAttribute), false)[0]).Id;
                _.Call.MethodId = 0;
                _.Call.Params.Content.Rewrap<TestInterface.Params_Foo.WRITER>();
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void InvalidCallTargetPromisedAnswer()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => { _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 0; });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                bootCapId = _.Return.Results.CapTable[0].SenderHosted;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Call;
                _.Call.QuestionId = 1;
                _.Call.Target.which = MessageTarget.WHICH.PromisedAnswer;
                _.Call.Target.PromisedAnswer.QuestionId = 1;
                _.Call.Target.PromisedAnswer.Transform.Init(1);
                _.Call.Target.PromisedAnswer.Transform[0].which = PromisedAnswer.Op.WHICH.GetPointerField;
                _.Call.InterfaceId = ((TypeIdAttribute)typeof(ITestInterface).GetCustomAttributes(typeof(TypeIdAttribute), false)[0]).Id;
                _.Call.MethodId = 0;
                _.Call.Params.Content.Rewrap<TestInterface.Params_Foo.WRITER>();
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void UnimplementedCallTargetUnknown()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => { _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 0; });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Return, _.which);
                Assert.AreEqual(Return.WHICH.Results, _.Return.which);
                Assert.AreEqual(1, _.Return.Results.CapTable.Count);
                bootCapId = _.Return.Results.CapTable[0].SenderHosted;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Call;
                _.Call.QuestionId = 1;
                _.Call.Target.which = (MessageTarget.WHICH)77;
                _.Call.InterfaceId = ((TypeIdAttribute)typeof(ITestInterface).GetCustomAttributes(typeof(TypeIdAttribute), false)[0]).Id;
                _.Call.MethodId = 0;
                _.Call.Params.Content.Rewrap<TestInterface.Params_Foo.WRITER>();
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
            });
            Assert.IsFalse(tester.IsDismissed);
        }

        [TestMethod]
        public void DuplicateResolve1()
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

            // tester.ExpectAbort();

            // Duplicate resolve is only a protocol error if the Rpc engine can prove misbehavior.
            // In this case that proof is not possible because the preliminary cap is release (thus, removed from import table)
            // immediately after the first resolution. Now we get the situation that the 2nd resolution refers to a non-existing
            // cap. This is not considered a protocol error because it might be due to an expected race condition 
            // between receiver-side Release and sender-side Resolve. 
        }

        [TestMethod]
        public void DuplicateResolve2()
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
                _.Return.Results.Content.SetCapability(0);
            });
            proxy.Call(0, 0, DynamicSerializerState.CreateForRpc());
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Finish, _.which);
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Call, _.which);
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Resolve;
                _.Resolve.which = Resolve.WHICH.Cap;
                _.Resolve.Cap.which = CapDescriptor.WHICH.SenderHosted;
                _.Resolve.Cap.SenderHosted = 1;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Resolve;
                _.Resolve.which = Resolve.WHICH.Exception;
                _.Resolve.Exception.Reason = "problem";
            });

            tester.ExpectAbort();
        }

        [TestMethod]
        public void UnimplementedResolveCategory()
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
                _.Resolve.which = (Resolve.WHICH)7;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Finish, _.which);
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
            });
        }

        [TestMethod]
        public void InvalidResolve()
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
                _.Return.Results.CapTable[0].SenderHosted = 7;
            });
            tester.Send(_ => {
                _.which = Message.WHICH.Resolve;
                _.Resolve.which = Resolve.WHICH.Cap;
                _.Resolve.PromiseId = 7;
                _.Resolve.Cap.which = CapDescriptor.WHICH.SenderHosted;
                _.Resolve.Cap.SenderHosted = 1;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Finish, _.which);
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

        [TestMethod]
        public void UnimplementedSendResultsToThirdParty()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => { 
                _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 0; });
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
                _.Call.InterfaceId = new TestInterface_Skeleton().InterfaceId;
                _.Call.MethodId = 0;
                var wr = _.Call.Params.Content.Rewrap<TestInterface.Params_Foo.WRITER>();
                _.Call.Params.CapTable.Init(0);
                _.Call.SendResultsTo.which = Call.sendResultsTo.WHICH.ThirdParty;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
            });
            Assert.IsFalse(tester.IsDismissed);
        }

        [TestMethod]
        public void UnimplementedSendResultsToUnknown()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            uint bootCapId = 0;

            tester.Send(_ => {
                _.which = Message.WHICH.Bootstrap; _.Bootstrap.QuestionId = 0;
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
                _.Call.InterfaceId = new TestInterface_Skeleton().InterfaceId;
                _.Call.MethodId = 0;
                var wr = _.Call.Params.Content.Rewrap<TestInterface.Params_Foo.WRITER>();
                _.Call.Params.CapTable.Init(0);
                _.Call.SendResultsTo.which = (Call.sendResultsTo.WHICH)13;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
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
                var wr = _.Call.Params.Content.Rewrap<TestPipeline.Params_GetCap.WRITER>();
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

        [TestMethod]
        public void SenderLoopbackOnInvalidCap()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            tester.Send(_ => {
                _.which = Message.WHICH.Disembargo;
                _.Disembargo.Target.which = MessageTarget.WHICH.ImportedCap;
                _.Disembargo.Target.ImportedCap = 0;
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void SenderLoopbackOnInvalidPromisedAnswer()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            tester.Send(_ => {
                _.which = Message.WHICH.Disembargo;
                _.Disembargo.Context.which = Disembargo.context.WHICH.SenderLoopback;
                _.Disembargo.Context.SenderLoopback = 0;
                _.Disembargo.Target.which = MessageTarget.WHICH.PromisedAnswer;
                _.Disembargo.Target.PromisedAnswer.QuestionId = 9;
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void SenderLoopbackOnUnknownTarget()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            tester.Send(_ => {
                _.which = Message.WHICH.Disembargo;
                _.Disembargo.Context.which = Disembargo.context.WHICH.SenderLoopback;
                _.Disembargo.Context.SenderLoopback = 0;
                _.Disembargo.Target.which = (MessageTarget.WHICH)12;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
            });
        }

        [TestMethod]
        public void ReceiverLoopbackOnInvalidCap()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            tester.Send(_ => {
                _.which = Message.WHICH.Disembargo;
                _.Disembargo.Context.which = Disembargo.context.WHICH.ReceiverLoopback;
                _.Disembargo.Context.ReceiverLoopback = 0;
                _.Disembargo.Target.which = MessageTarget.WHICH.ImportedCap;
                _.Disembargo.Target.ImportedCap = 0;
            });
            tester.ExpectAbort();
        }

        [TestMethod]
        public void UnimplementedDisembargoAccept()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            tester.Send(_ => {
                _.which = Message.WHICH.Disembargo;
                _.Disembargo.Context.which = Disembargo.context.WHICH.Accept;
                _.Disembargo.Target.which = MessageTarget.WHICH.ImportedCap;
                _.Disembargo.Target.ImportedCap = 0;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
            });
        }

        [TestMethod]
        public void UnimplementedDisembargoProvide()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            tester.Send(_ => {
                _.which = Message.WHICH.Disembargo;
                _.Disembargo.Context.which = Disembargo.context.WHICH.Provide;
                _.Disembargo.Target.which = MessageTarget.WHICH.ImportedCap;
                _.Disembargo.Target.ImportedCap = 0;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
            });
        }

        [TestMethod]
        public void UnimplementedDisembargoUnknown()
        {
            var tester = new RpcEngineTester();
            tester.Engine.Main = new TestInterfaceImpl(new Counters());

            tester.Send(_ => {
                _.which = Message.WHICH.Disembargo;
                _.Disembargo.Context.which = (Disembargo.context.WHICH)50;
                _.Disembargo.Target.which = MessageTarget.WHICH.ImportedCap;
                _.Disembargo.Target.ImportedCap = 0;
            });
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
            });
        }

        [TestMethod]
        public void UnimplementedCall()
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
                _.Return.Results.Content.SetCapability(0);
            });
            Assert.IsTrue(proxy.WhenResolved.IsCompleted);
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Finish, _.which);
            });
            var args = DynamicSerializerState.CreateForRpc();
            var ti = new TestInterfaceImpl(new Counters());
            args.ProvideCapability(ti);
            proxy.Call(1, 2, args);
            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Call, _.which);
                Assert.AreEqual(1ul, _.Call.InterfaceId);
                Assert.AreEqual((ushort)2, _.Call.MethodId);

                Assert.IsFalse(ti.IsDisposed);

                tester.Send(_1 =>
                {
                    _1.which = Message.WHICH.Unimplemented;
                    _1.Unimplemented.which = Message.WHICH.Call;
                    Reserializing.DeepCopy(_.Call, _1.Unimplemented.Call);
                });

                Assert.IsTrue(ti.IsDisposed);
            });
        }

        [TestMethod]
        public void UnimplementedBootstrap()
        {
            var tester = new RpcEngineTester();

            var cap = tester.RealEnd.QueryMain();
            var proxy = new BareProxy(cap);

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Bootstrap, _.which);

                tester.Send(_1 =>
                {
                    _1.which = Message.WHICH.Unimplemented;
                    _1.Unimplemented.which = Message.WHICH.Bootstrap;
                    Reserializing.DeepCopy(_.Bootstrap, _1.Unimplemented.Bootstrap);
                });
            });

            tester.ExpectAbort();
        }

        [TestMethod]
        public void Abort()
        {
            var tester = new RpcEngineTester();

            var cap = tester.RealEnd.QueryMain();
            var proxy = new BareProxy(cap);

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Bootstrap, _.which);
            });

            tester.Send(_ => {
                _.which = Message.WHICH.Abort;
            });
        }

        [TestMethod]
        public void ThirdPartyHostedBootstrap()
        {
            var tester = new RpcEngineTester();

            var cap = tester.RealEnd.QueryMain();
            var proxy = new BareProxy(cap);

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Bootstrap, _.which);

                tester.Send(_1 =>
                {
                    _1.which = Message.WHICH.Return;
                    _1.Return.AnswerId = _.Bootstrap.QuestionId;
                    _1.Return.which = Return.WHICH.Results;
                    _1.Return.Results.CapTable.Init(1);
                    _1.Return.Results.CapTable[0].which = CapDescriptor.WHICH.ThirdPartyHosted;
                    _1.Return.Results.CapTable[0].ThirdPartyHosted.VineId = 27;
                    _1.Return.Results.Content.SetCapability(0);
                });
            });

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Finish, _.which);
            });

            proxy.Call(1, 2, DynamicSerializerState.CreateForRpc());

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Call, _.which);
                Assert.AreEqual(MessageTarget.WHICH.ImportedCap, _.Call.Target.which);
                Assert.AreEqual(27u, _.Call.Target.ImportedCap);

                tester.Send(_1 =>
                {
                    _1.which = Message.WHICH.Return;
                    _1.Return.AnswerId = _.Call.QuestionId;
                    _1.Return.which = Return.WHICH.Results;
                    _1.Return.Results.CapTable.Init(1);
                    _1.Return.Results.CapTable[0].which = CapDescriptor.WHICH.ThirdPartyHosted;
                    _1.Return.Results.CapTable[0].ThirdPartyHosted.VineId = 27;
                    _1.Return.Results.Content.SetCapability(0);
                });
            });
        }

        [TestMethod]
        public void NoneImportBootstrap()
        {
            var tester = new RpcEngineTester();

            var cap = tester.RealEnd.QueryMain();
            var proxy = new BareProxy(cap);

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Bootstrap, _.which);

                tester.Send(_1 =>
                {
                    _1.which = Message.WHICH.Return;
                    _1.Return.AnswerId = _.Bootstrap.QuestionId;
                    _1.Return.which = Return.WHICH.Results;
                    _1.Return.Results.CapTable.Init(1);
                    _1.Return.Results.CapTable[0].which = CapDescriptor.WHICH.None;
                });
            });

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Finish, _.which);
            });

            var answer = proxy.Call(1, 2, DynamicSerializerState.CreateForRpc());
            var task = Impatient.MakePipelineAware(answer, _ => _);
            Assert.IsTrue(task.IsFaulted);
            Assert.IsFalse(tester.IsDismissed);
        }

        [TestMethod]
        public void UnknownImportBootstrap()
        {
            var tester = new RpcEngineTester();

            var cap = tester.RealEnd.QueryMain();
            var proxy = new BareProxy(cap);

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Bootstrap, _.which);

                tester.Send(_1 =>
                {
                    _1.which = Message.WHICH.Return;
                    _1.Return.AnswerId = _.Bootstrap.QuestionId;
                    _1.Return.which = Return.WHICH.Results;
                    _1.Return.Results.CapTable.Init(1);
                    _1.Return.Results.CapTable[0].which = (CapDescriptor.WHICH)27;
                });
            });

            tester.Recv(_ => {
                Assert.AreEqual(Message.WHICH.Unimplemented, _.which);
            });

            Assert.IsFalse(tester.IsDismissed);
        }
    }
}
