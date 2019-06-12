using Capnp.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class RpcSchemaTests
    {
        [TestMethod]
        public void MessageAbort()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Abort;
                Assert.AreEqual(Message.WHICH.Abort, w.which);
                w.Abort.Reason = "reason";
                Assert.AreEqual("reason", w.Abort.Reason);
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));
            Assert.AreEqual(Message.WHICH.Abort, r.which);
            Assert.AreEqual("reason", r.Abort.Reason);
        }

        [TestMethod]
        public void MessageAccept()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Accept;
                Assert.AreEqual(Message.WHICH.Accept, w.which);
                w.Accept.Embargo = true;
                Assert.IsTrue(w.Accept.Embargo);
                w.Accept.Provision.SetStruct(2, 0);
                w.Accept.Provision.WriteData(0, long.MinValue);
                w.Accept.Provision.WriteData(64, long.MaxValue);
                Assert.AreEqual(long.MinValue, w.Accept.Provision.ReadDataLong(0));
                Assert.AreEqual(long.MaxValue, w.Accept.Provision.ReadDataLong(64));
                w.Accept.QuestionId = 0x87654321u;
                Assert.AreEqual(0x87654321u, w.Accept.QuestionId);
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));
            Assert.AreEqual(Message.WHICH.Accept, r.which);
            Assert.IsTrue(r.Accept.Embargo);
            Assert.AreEqual(ObjectKind.Struct, r.Accept.Provision.Kind);
            Assert.AreEqual(long.MinValue, r.Accept.Provision.ReadDataLong(0));
            Assert.AreEqual(long.MaxValue, r.Accept.Provision.ReadDataLong(64));
            Assert.AreEqual(0x87654321u, r.Accept.QuestionId);
        }

        [TestMethod]
        public void MessageBootstrap()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Bootstrap;
                Assert.AreEqual(Message.WHICH.Bootstrap, w.which);
                w.Bootstrap.QuestionId = 0xaa55aa55u;
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));
            Assert.AreEqual(Message.WHICH.Bootstrap, r.which);
            Assert.AreEqual(0xaa55aa55u, r.Bootstrap.QuestionId);
        }

        [TestMethod]
        public void MessageCall()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Call;
                Assert.AreEqual(Message.WHICH.Call, w.which);
                w.Call.AllowThirdPartyTailCall = true;
                w.Call.InterfaceId = ulong.MaxValue;
                w.Call.MethodId = 0x1111;
                w.Call.Params.CapTable.Init(6);
                w.Call.Params.CapTable[0].which = CapDescriptor.WHICH.None;
                w.Call.Params.CapTable[1].which = CapDescriptor.WHICH.ReceiverAnswer;
                w.Call.Params.CapTable[1].ReceiverAnswer.QuestionId = 0x12345678u;
                w.Call.Params.CapTable[1].ReceiverAnswer.Transform.Init(2);
                w.Call.Params.CapTable[1].ReceiverAnswer.Transform[0].which = PromisedAnswer.Op.WHICH.GetPointerField;
                w.Call.Params.CapTable[1].ReceiverAnswer.Transform[0].GetPointerField = 0x2222;
                w.Call.Params.CapTable[1].ReceiverAnswer.Transform[1].which = PromisedAnswer.Op.WHICH.Noop;
                w.Call.Params.CapTable[2].which = CapDescriptor.WHICH.ReceiverHosted;
                w.Call.Params.CapTable[2].ReceiverHosted = 12345678u;
                w.Call.Params.CapTable[3].which = CapDescriptor.WHICH.SenderHosted;
                w.Call.Params.CapTable[3].SenderHosted = 23456789u;
                w.Call.Params.CapTable[4].which = CapDescriptor.WHICH.SenderPromise;
                w.Call.Params.CapTable[4].SenderPromise = 34567890u;
                w.Call.Params.CapTable[5].which = CapDescriptor.WHICH.ThirdPartyHosted;
                w.Call.Params.CapTable[5].ThirdPartyHosted.Id.SetStruct(1, 0);
                w.Call.Params.CapTable[5].ThirdPartyHosted.Id.WriteData(0, double.Epsilon);
                w.Call.Params.CapTable[5].ThirdPartyHosted.VineId = 111111u;

                Assert.AreEqual(CapDescriptor.WHICH.None, w.Call.Params.CapTable[0].which);
                Assert.AreEqual(CapDescriptor.WHICH.ReceiverAnswer, w.Call.Params.CapTable[1].which);
                Assert.AreEqual(CapDescriptor.WHICH.ReceiverHosted, w.Call.Params.CapTable[2].which);
                Assert.AreEqual(CapDescriptor.WHICH.SenderHosted, w.Call.Params.CapTable[3].which);
                Assert.AreEqual(CapDescriptor.WHICH.SenderPromise, w.Call.Params.CapTable[4].which);
                Assert.AreEqual(CapDescriptor.WHICH.ThirdPartyHosted, w.Call.Params.CapTable[5].which);

                var content = w.Call.Params.Content.Rewrap<DynamicSerializerState>();
                content.SetStruct(1, 0);
                content.WriteData(0, double.PositiveInfinity);
                w.Call.QuestionId = 0x77777777u;
                w.Call.SendResultsTo.which = Call.sendResultsTo.WHICH.ThirdParty;
                w.Call.SendResultsTo.ThirdParty.SetStruct(1, 0);
                w.Call.SendResultsTo.ThirdParty.WriteData(0, double.NegativeInfinity);
                w.Call.Target.which = MessageTarget.WHICH.PromisedAnswer;
                w.Call.Target.PromisedAnswer.QuestionId = 5555555u;
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));
            Assert.AreEqual(Message.WHICH.Call, r.which);
            Assert.IsTrue(r.Call.AllowThirdPartyTailCall);
            Assert.AreEqual(ulong.MaxValue, r.Call.InterfaceId);
            Assert.AreEqual((ushort)0x1111, r.Call.MethodId);
            var capTable = r.Call.Params.CapTable;
            Assert.AreEqual(6, capTable.Count);
            Assert.AreEqual(CapDescriptor.WHICH.None, capTable[0].which);
            Assert.AreEqual(CapDescriptor.WHICH.ReceiverAnswer, capTable[1].which);
            Assert.AreEqual(0x12345678u, capTable[1].ReceiverAnswer.QuestionId);
            var transform = capTable[1].ReceiverAnswer.Transform;
            Assert.AreEqual(2, transform.Count);
            Assert.AreEqual(PromisedAnswer.Op.WHICH.GetPointerField, transform[0].which);
            Assert.AreEqual((ushort)0x2222, transform[0].GetPointerField);
            Assert.AreEqual(PromisedAnswer.Op.WHICH.Noop, transform[1].which);
            Assert.AreEqual(CapDescriptor.WHICH.ReceiverHosted, capTable[2].which);
            Assert.AreEqual(12345678u, capTable[2].ReceiverHosted);
            Assert.AreEqual(CapDescriptor.WHICH.SenderHosted, capTable[3].which);
            Assert.AreEqual(23456789u, capTable[3].SenderHosted);
            Assert.AreEqual(CapDescriptor.WHICH.SenderPromise, capTable[4].which);
            Assert.AreEqual(34567890u, capTable[4].SenderPromise);
            Assert.AreEqual(CapDescriptor.WHICH.ThirdPartyHosted, capTable[5].which);
            var tph = capTable[5].ThirdPartyHosted;
            Assert.AreEqual(ObjectKind.Struct, tph.Id.Kind);
            Assert.AreEqual(double.Epsilon, tph.Id.ReadDataDouble(0));
            Assert.AreEqual(111111u, tph.VineId);
            Assert.AreEqual(ObjectKind.Struct, r.Call.Params.Content.Kind);
            Assert.AreEqual(double.PositiveInfinity, r.Call.Params.Content.ReadDataDouble(0));
            Assert.AreEqual(0x77777777u, r.Call.QuestionId);
            var srt = r.Call.SendResultsTo;
            Assert.AreEqual(Call.sendResultsTo.WHICH.ThirdParty, srt.which);
            Assert.AreEqual(ObjectKind.Struct, srt.ThirdParty.Kind);
            Assert.AreEqual(double.NegativeInfinity, srt.ThirdParty.ReadDataDouble(0));
            Assert.AreEqual(MessageTarget.WHICH.PromisedAnswer, r.Call.Target.which);
            Assert.AreEqual(5555555u, r.Call.Target.PromisedAnswer.QuestionId);
        }

        [TestMethod]
        public void MessageDisembargo()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Disembargo;

                var ctx = w.Disembargo.Context;
                ctx.which = Disembargo.context.WHICH.SenderLoopback;
                ctx.SenderLoopback = 1234567u;
                var tgt = w.Disembargo.Target;
                tgt.which = MessageTarget.WHICH.PromisedAnswer;
                tgt.PromisedAnswer.QuestionId = 7654321u;
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));
            {
                Assert.AreEqual(Message.WHICH.Disembargo, r.which);
                Assert.AreEqual(Disembargo.context.WHICH.SenderLoopback, r.Disembargo.Context.which);
                Assert.AreEqual(1234567u, r.Disembargo.Context.SenderLoopback);
                Assert.AreEqual(MessageTarget.WHICH.PromisedAnswer, r.Disembargo.Target.which);
                Assert.AreEqual(7654321u, r.Disembargo.Target.PromisedAnswer.QuestionId);
            }
        }

        [TestMethod]
        public void MessageFinish()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Finish;

                w.Finish.QuestionId = 0xaaaaaaaa;
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));

            {
                Assert.AreEqual(Message.WHICH.Finish, r.which);
                Assert.AreEqual(0xaaaaaaaa, r.Finish.QuestionId);
            }
        }

        [TestMethod]
        public void MessageJoin()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Join;
                w.Join.KeyPart.SetStruct(2, 0);
                w.Join.KeyPart.WriteData(0, long.MinValue);
                w.Join.KeyPart.WriteData(64, long.MaxValue);
                w.Join.QuestionId = 0x88888888;
                w.Join.Target.which = MessageTarget.WHICH.ImportedCap;
                w.Join.Target.ImportedCap = 0x99999999;
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));

            {
                Assert.AreEqual(Message.WHICH.Join, r.which);
                Assert.AreEqual(ObjectKind.Struct, r.Join.KeyPart.Kind);
                Assert.AreEqual(long.MinValue, r.Join.KeyPart.ReadDataLong(0));
                Assert.AreEqual(long.MaxValue, r.Join.KeyPart.ReadDataLong(64));
                Assert.AreEqual(0x88888888, r.Join.QuestionId);
                Assert.AreEqual(MessageTarget.WHICH.ImportedCap, r.Join.Target.which);
                Assert.AreEqual(0x99999999, r.Join.Target.ImportedCap);
            }
        }

        [TestMethod]
        public void MessageProvide()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Provide;
                w.Provide.QuestionId = 0xbbbbbbbb;
                w.Provide.Recipient.SetStruct(1, 0);
                w.Provide.Recipient.WriteData(0, -1);
                w.Provide.Target.which = MessageTarget.WHICH.PromisedAnswer;
                w.Provide.Target.PromisedAnswer.QuestionId = 0xcccccccc;
                w.Provide.Target.PromisedAnswer.Transform.Init(1);
                w.Provide.Target.PromisedAnswer.Transform[0].which = PromisedAnswer.Op.WHICH.Noop;
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));

            {
                Assert.AreEqual(Message.WHICH.Provide, r.which);
                Assert.AreEqual(0xbbbbbbbb, r.Provide.QuestionId);
                Assert.AreEqual(-1, r.Provide.Recipient.ReadDataInt(0));
                Assert.AreEqual(MessageTarget.WHICH.PromisedAnswer, r.Provide.Target.which);
                Assert.AreEqual(0xcccccccc, r.Provide.Target.PromisedAnswer.QuestionId);
                Assert.AreEqual(1, r.Provide.Target.PromisedAnswer.Transform.Count);
                Assert.AreEqual(PromisedAnswer.Op.WHICH.Noop, r.Provide.Target.PromisedAnswer.Transform[0].which);
            }
        }

        [TestMethod]
        public void MessageRelease()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Release;
                w.Release.Id = 0xdddddddd;
                w.Release.ReferenceCount = 27;
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));

            {
                Assert.AreEqual(Message.WHICH.Release, r.which);
                Assert.AreEqual(0xdddddddd, r.Release.Id);
                Assert.AreEqual(27u, r.Release.ReferenceCount);
            }
        }

        [TestMethod]
        public void MessageResolve()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Resolve;
                w.Resolve.which = Resolve.WHICH.Cap;
                w.Resolve.Cap.which = CapDescriptor.WHICH.SenderHosted;
                w.Resolve.Cap.SenderHosted = 0xeeeeeeee;
                w.Resolve.PromiseId = 0x11111111;
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));

            {
                Assert.AreEqual(Message.WHICH.Resolve, r.which);
                Assert.AreEqual(CapDescriptor.WHICH.SenderHosted, r.Resolve.Cap.which);
                Assert.AreEqual(0xeeeeeeee, r.Resolve.Cap.SenderHosted);
                Assert.AreEqual(0x11111111u, r.Resolve.PromiseId);
            }
        }

        [TestMethod]
        public void MessageReturn()
        {
            var mb = MessageBuilder.Create();

            {
                var w = mb.BuildRoot<Message.WRITER>();
                w.which = Message.WHICH.Return;
                w.Return.which = Return.WHICH.Results;
                w.Return.Results.CapTable.Init(1);
                w.Return.Results.CapTable[0].which = CapDescriptor.WHICH.SenderHosted;
                w.Return.Results.CapTable[0].SenderHosted = 0x22222222;
                var content = w.Return.Results.Content.Rewrap<DynamicSerializerState>();
                content.SetStruct(2, 0);
                content.WriteData(0, double.MinValue);
                content.WriteData(64, double.MaxValue);
                Assert.IsTrue(w.Return.ReleaseParamCaps);
                w.Return.ReleaseParamCaps = false;
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));

            {
                Assert.AreEqual(Message.WHICH.Return, r.which);
                Assert.AreEqual(Return.WHICH.Results, r.Return.which);
                Assert.AreEqual(1, r.Return.Results.CapTable.Count);
                Assert.AreEqual(CapDescriptor.WHICH.SenderHosted, r.Return.Results.CapTable[0].which);
                Assert.AreEqual(0x22222222u, r.Return.Results.CapTable[0].SenderHosted);
                Assert.AreEqual(double.MinValue, r.Return.Results.Content.ReadDataDouble(0));
                Assert.AreEqual(double.MaxValue,
                    r.Return.Results.Content.ReadDataDouble(64));
                Assert.IsFalse(r.Return.ReleaseParamCaps);
            }
        }

        [TestMethod]
        public void MessageUnimplemented()
        {
            var mb = MessageBuilder.Create();

            {
                var u = mb.BuildRoot<Message.WRITER>();
                u.which = Message.WHICH.Unimplemented;
                var w = u.Unimplemented;
                w.which = Message.WHICH.Resolve;
                w.Resolve.which = Resolve.WHICH.Exception;
                w.Resolve.Exception.Reason = "reason";
            }

            var r = Message.READER.create(DeserializerState.CreateRoot(mb.Frame));

            {
                Assert.AreEqual(Message.WHICH.Unimplemented, r.which);
                Assert.AreEqual(Message.WHICH.Resolve, r.Unimplemented.which);
                Assert.AreEqual(Resolve.WHICH.Exception, r.Unimplemented.Resolve.which);
                Assert.AreEqual("reason", r.Unimplemented.Resolve.Exception.Reason);
            }
        }
    }
}
