using Capnp.Net.Runtime.Tests.GenImpls;
using Capnproto_test.Capnp.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Capnproto_test.Capnp.Test.TestStructUnion;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    [TestCategory("Coverage")]
    public class SerializationTests
    {
        [TestMethod]
        public void ListOfBits()
        {
            void CheckList(IEnumerable<bool> items)
            {
                int i = 0;
                foreach (bool bit in items)
                {
                    if (i == 63 || i == 66 || i == 129)
                        Assert.IsTrue(bit);
                    else
                        Assert.IsFalse(bit);

                    ++i;
                }
                Assert.AreEqual(130, i);
            }

            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfBitsSerializer>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Init(-1));
            // Assert.AreEqual(0, list.Count); // Bug or feature? Uninitialized list's Count is -1
            Assert.ThrowsException<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.ThrowsException<InvalidOperationException>(() => { list[0] = false; });
            list.Init(130);
            list[63] = true;
            list[65] = true;
            list[66] = true;
            list[65] = false;
            list[129] = true;
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = list[130]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { list[130] = false; });
            Assert.IsFalse(list[0]);
            Assert.IsTrue(list[63]);
            Assert.IsFalse(list[64]);
            Assert.IsFalse(list[65]);
            Assert.IsTrue(list[66]);
            Assert.IsTrue(list[129]);
            var list2 = b.CreateObject<ListOfBitsSerializer>();
            list2.Init(null);
            list2.Init(list.ToArray());
            Assert.IsFalse(list2[0]);
            Assert.IsTrue(list2[63]);
            Assert.IsFalse(list2[64]);
            Assert.IsFalse(list2[65]);
            Assert.IsTrue(list2[66]);
            Assert.IsTrue(list2[129]);
            CheckList(list2);
            Assert.ThrowsException<InvalidOperationException>(() => list.Init(4));
            DeserializerState d = list2;
            var list3 = d.RequireList().CastBool();
            CheckList(list3);
        }

        [TestMethod]
        public void ListOfCaps()
        {
            var b = MessageBuilder.Create();
            b.InitCapTable();
            var list = b.CreateObject<ListOfCapsSerializer<ITestInterface>>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.ThrowsException<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.ThrowsException<InvalidOperationException>(() => { list[0] = null; });
            list.Init(5);
            Assert.ThrowsException<InvalidOperationException>(() => list.Init(1));
            var c1 = new Counters();
            var cap1 = new TestInterfaceImpl(c1);
            var c2 = new Counters();
            var cap2 = new TestInterfaceImpl(c2);
            list[0] = null;
            list[1] = cap1;
            list[2] = cap2;
            list[3] = cap1;
            list[4] = cap2;
            list[3] = null;
            Assert.IsTrue(list.All(p => p is Rpc.Proxy));
            var proxies = list.Cast<Rpc.Proxy>().ToArray();
            Assert.IsTrue(proxies[0].IsNull);
            Assert.IsFalse(proxies[1].IsNull);
            Assert.IsTrue(proxies[3].IsNull);
            list[2].Foo(123u, true).Wait();
            Assert.AreEqual(0, c1.CallCount);
            Assert.AreEqual(1, c2.CallCount);
            list[4].Foo(123u, true).Wait();
            Assert.AreEqual(2, c2.CallCount);

            var list2 = b.CreateObject<ListOfCapsSerializer<ITestInterface>>();
            list2.Init(null);
            list2.Init(list.ToArray());
            proxies = list2.Cast<Rpc.Proxy>().ToArray();
            Assert.IsTrue(proxies[0].IsNull);
            Assert.IsFalse(proxies[1].IsNull);
            Assert.IsTrue(proxies[3].IsNull);
            list2[2].Foo(123u, true).Wait();
            Assert.AreEqual(0, c1.CallCount);
            Assert.AreEqual(3, c2.CallCount);
            list2[4].Foo(123u, true).Wait();
            Assert.AreEqual(4, c2.CallCount);

            DeserializerState d = list2;
            var list3 = d.RequireList().CastCapList<ITestInterface>();
            proxies = list3.Cast<Rpc.Proxy>().ToArray();
            Assert.IsTrue(proxies[0].IsNull);
            Assert.IsFalse(proxies[1].IsNull);
            Assert.IsTrue(proxies[3].IsNull);
            list3[2].Foo(123u, true).Wait();
            Assert.AreEqual(0, c1.CallCount);
            Assert.AreEqual(5, c2.CallCount);
            list3[4].Foo(123u, true).Wait();
            Assert.AreEqual(6, c2.CallCount);
        }

        [TestMethod]
        public void ListOfEmpty()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfEmptySerializer>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Init(-1));
            list.Init(987654321);
            Assert.AreEqual(987654321, list.Count);
            Assert.ThrowsException<InvalidOperationException>(() => list.Init(42));
            DeserializerState d = list;
            int list2 = d.RequireList().CastVoid();
            Assert.AreEqual(987654321, list2);
        }

        [TestMethod]
        public void ListOfPointers()
        {
            var b = MessageBuilder.Create();
            b.InitCapTable();
            var list = b.CreateObject<ListOfPointersSerializer<DynamicSerializerState>>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.ThrowsException<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.ThrowsException<InvalidOperationException>(() => { list[0] = null; });
            list.Init(7);
            Assert.ThrowsException<InvalidOperationException>(() => list.Init(1));
            Assert.AreEqual(7, list.Count);
            var c1 = new Counters();
            var cap1 = new TestInterfaceImpl(c1);
            var obj1 = b.CreateObject<DynamicSerializerState>();
            obj1.SetObject(cap1);
            var obj2 = b.CreateObject<DynamicSerializerState>();
            obj2.SetStruct(1, 1);
            var lobs = b.CreateObject<ListOfBitsSerializer>();
            lobs.Init(1);
            var obj3 = lobs.Rewrap<DynamicSerializerState>();
            list[1] = obj1;
            list[2] = obj2;
            list[3] = obj3;
            Assert.IsNotNull(list[0]);
            Assert.AreEqual(ObjectKind.Nil, list[0].Kind);
            Assert.AreEqual(obj1, list[1]);
            Assert.AreEqual(obj2, list[2]);
            Assert.AreEqual(obj3, list[3]);
            var list2 = list.ToArray();
            Assert.IsNotNull(list2[0]);
            Assert.AreEqual(ObjectKind.Nil, list2[0].Kind);
            Assert.AreEqual(obj1, list2[1]);
            Assert.AreEqual(obj2, list2[2]);
            Assert.AreEqual(obj3, list2[3]);

            DeserializerState d = list;
            var list3 = d.RequireList().Cast(_ => _);
            Assert.AreEqual(7, list3.Count);
            Assert.IsNotNull(list3[0]);
            Assert.AreEqual(ObjectKind.Nil, list3[0].Kind);
            Assert.AreEqual(ObjectKind.Capability, list3[1].Kind);
            Assert.AreEqual(ObjectKind.Struct, list3[2].Kind);
            Assert.AreEqual(ObjectKind.ListOfBits, list3[3].Kind);
        }

        [TestMethod]
        public void ListOfPrimitives()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<float>>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.ThrowsException<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.ThrowsException<InvalidOperationException>(() => { list[0] = 1.0f; });
            list.Init(4);
            Assert.ThrowsException<InvalidOperationException>(() => list.Init(1));
            Assert.AreEqual(4, list.Count);
            list[0] = 0.0f;
            list[1] = 1.0f;
            list[2] = 2.0f;
            list[3] = 3.0f;
            Assert.AreEqual(0.0f, list[0]);
            Assert.AreEqual(1.0f, list[1]);
            Assert.AreEqual(2.0f, list[2]);
            Assert.AreEqual(3.0f, list[3]);

            var list2 = b.CreateObject<ListOfPrimitivesSerializer<float>>();
            list2.Init(null);
            list2.Init(list.ToArray());
            Assert.AreEqual(4, list2.Count);
            Assert.AreEqual(0.0f, list2[0]);
            Assert.AreEqual(1.0f, list2[1]);
            Assert.AreEqual(2.0f, list2[2]);
            Assert.AreEqual(3.0f, list2[3]);

            DeserializerState d = list2;
            var list3 = d.RequireList().CastFloat();
            Assert.AreEqual(4, list3.Count);
            Assert.AreEqual(0.0f, list3[0]);
            Assert.AreEqual(1.0f, list3[1]);
            Assert.AreEqual(2.0f, list3[2]);
            Assert.AreEqual(3.0f, list3[3]);
        }

        [TestMethod]
        public void ListOfStructs()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfStructsSerializer<SomeStruct.WRITER>>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.ThrowsException<InvalidOperationException>(() => { var _ = list[0]; });
            list.Init(4);
            Assert.ThrowsException<InvalidOperationException>(() => list.Init(1));
            Assert.AreEqual(4, list.Count);
            list[0].SomeText = "0";
            list[1].SomeText = "1";
            list[2].SomeText = "2";
            list[3].SomeText = "3";
            Assert.AreEqual("0", list[0].SomeText);
            Assert.AreEqual("3", list[3].SomeText);

            var list2 = b.CreateObject<ListOfStructsSerializer<SomeStruct.WRITER>>();
            list2.Init(list.ToArray(), (dst, src) => { dst.SomeText = src.SomeText; dst.MoreText = src.MoreText; });
            Assert.AreEqual(4, list2.Count);
            Assert.AreEqual("0", list2[0].SomeText);
            Assert.AreEqual("3", list2[3].SomeText);

            DeserializerState d = list2;
            var list3 = d.RequireList().Cast(_ => new SomeStruct.READER(_));
            Assert.AreEqual(4, list3.Count);
            Assert.AreEqual("0", list3[0].SomeText);
            Assert.AreEqual("3", list3[3].SomeText);
        }

        [TestMethod]
        public void ListOfText()
        {
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfTextSerializer>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Init(-1));
            Assert.ThrowsException<InvalidOperationException>(() => { var _ = list[0]; });
            Assert.ThrowsException<InvalidOperationException>(() => { list[0] = "foo"; });
            list.Init(4);
            Assert.ThrowsException<InvalidOperationException>(() => list.Init(1));
            Assert.AreEqual(4, list.Count);
            list[0] = "0";
            list[2] = null;
            list[3] = "3";
            Assert.AreEqual("0", list[0]);
            Assert.IsNull(list[1]);
            Assert.IsNull(list[2]);
            Assert.AreEqual("3", list[3]);

            var list2 = b.CreateObject<ListOfTextSerializer>();
            list2.Init(list.ToArray());
            Assert.AreEqual(4, list2.Count);
            Assert.AreEqual("0", list2[0]);
            Assert.IsNull(list2[1]);
            Assert.IsNull(list2[2]);
            Assert.AreEqual("3", list2[3]);

            DeserializerState d = list2;
            var tmp = d.RequireList();
            var list3 = tmp.CastText2();
            Assert.AreEqual(4, list3.Count);
            Assert.AreEqual("0", list3[0]);
            Assert.IsNull(list3[1]);
            Assert.IsNull(list3[2]);
            Assert.AreEqual("3", list3[3]);
        }
    }
}
