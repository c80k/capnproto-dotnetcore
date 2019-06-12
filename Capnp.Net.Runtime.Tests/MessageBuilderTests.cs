using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class MessageBuilderTests
    {
        class Struct2D0P : SerializerState
        {
            public Struct2D0P()
            {
                SetStruct(2, 0);
            }
        }

        class Struct0D1P : SerializerState
        {
            public Struct0D1P()
            {
                SetStruct(0, 1);
            }
        }

        [TestMethod]
        public void BuildDynamicMessage()
        {
            var mb = MessageBuilder.Create(128);
            Assert.IsNull(mb.Root);
            var root = mb.BuildRoot<Struct2D0P>();
            Assert.IsNotNull(root);
            Assert.AreSame(root, mb.Root);
            root.WriteData(0, long.MinValue);
            root.WriteData(64, long.MaxValue);
            var frame = mb.Frame;

            var droot = DeserializerState.CreateRoot(frame);
            Assert.AreEqual(ObjectKind.Struct, droot.Kind);
            Assert.AreEqual(2, droot.StructDataCount);
            Assert.AreEqual(long.MinValue, droot.ReadDataLong(0));
            Assert.AreEqual(long.MaxValue, droot.ReadDataLong(64));
        }

        [TestMethod]
        public void SmallSegments()
        {
            WireFrame frame;

            for (int i = 1; i <= 8; i++)
            {
                {
                    var mb = MessageBuilder.Create(128);
                    var root = mb.BuildRoot<Struct0D1P>();
                    var p = root.BuildPointer(0);
                    p.SetListOfValues(64, i);
                    frame = mb.Frame;
                }

                {
                    var root = DeserializerState.CreateRoot(frame);
                    Assert.AreEqual(i, root.StructReadPointer(0).ListElementCount);
                }
            }
        }
    }
}
