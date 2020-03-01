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
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = list[5]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { list[-1] = null; });
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
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = list[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { list[7] = null; });
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
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = list[5]; });
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
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = list[5]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { list[-1] = null; });
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

        [TestMethod]
        public void CapnpSerializableListBool()
        {
            var expected = new bool[] { true, false, true };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfBitsSerializer>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<bool>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableListByte()
        {
            var expected = new byte[] { 1, 2, 3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<byte>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<byte>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableListSByte()
        {
            var expected = new sbyte[] { -1, -2, -3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<sbyte>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<sbyte>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableListUShort()
        {
            var expected = new ushort[] { 1, 2, 3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<ushort>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<ushort>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableListShort()
        {
            var expected = new short[] { -1, -2, -3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<short>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<short>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableListInt()
        {
            var expected = new int[] { -1, -2, -3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<int>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<int>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableListUInt()
        {
            var expected = new uint[] { 1, 2, 3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<uint>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<uint>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableListLong()
        {
            var expected = new long[] { -1, -2, -3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<long>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<long>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableListULong()
        {
            var expected = new ulong[] { 1, 2, 3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<ulong>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<ulong>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableListFloat()
        {
            var expected = new float[] { -1.0f, 2.0f, -3.0f };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<float>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<float>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableListDouble()
        {
            var expected = new double[] { -1, -2, -3 };
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<double>>();
            list.Init(expected);
            DeserializerState d = list;
            var list2 = CapnpSerializable.Create<IReadOnlyList<double>>(d);
            CollectionAssert.AreEqual(expected, list2.ToArray());
        }

        [TestMethod]
        public void CapnpSerializableStruct()
        {
            var b = MessageBuilder.Create();
            var obj = b.CreateObject<SomeStruct.WRITER>();
            obj.SomeText = "hello";
            obj.MoreText = "world";
            DeserializerState d = obj;
            var obj2 = CapnpSerializable.Create<SomeStruct>(d);
            Assert.AreEqual("hello", obj2.SomeText);
            Assert.AreEqual("world", obj2.MoreText);
        }

        [TestMethod]
        public void CapnpSerializableCapList()
        {
            var b = MessageBuilder.Create();
            b.InitCapTable();
            var wr = b.CreateObject<ListOfCapsSerializer<ITestInterface>>();
            var c1 = new Counters();
            var cap1 = new TestInterfaceImpl(c1);
            var c2 = new Counters();
            var cap2 = new TestInterfaceImpl(c2);
            wr.Init(new ITestInterface[] { null, cap1, cap2 });
            DeserializerState d = wr;
            var list = CapnpSerializable.Create<IReadOnlyList<ITestInterface>>(d);
            list[1].Foo(123u, true);
            Assert.AreEqual(1, c1.CallCount);
            list[2].Foo(123u, true);
            Assert.AreEqual(1, c2.CallCount);
        }

        [TestMethod]
        public void CapnpSerializableString()
        {
            string expected = "1, 2, 3";
            var b = MessageBuilder.Create();
            var list = b.CreateObject<ListOfPrimitivesSerializer<byte>>();
            var bytes = Encoding.UTF8.GetBytes(expected);
            list.Init(bytes.Length + 1);
            bytes.CopyTo(list.Data);
            DeserializerState d = list;
            var str = CapnpSerializable.Create<string>(d);
            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void CapnpSerializableAnyPointer()
        {
            var b = MessageBuilder.Create();
            var obj = b.CreateObject<SomeStruct.WRITER>();
            obj.SomeText = "hello";
            obj.MoreText = "world";
            DeserializerState d = obj;
            var any = CapnpSerializable.Create<AnyPointer>(d);
            var obj2 = new SomeStruct.READER(any.State);
            Assert.AreEqual("hello", obj2.SomeText);
            Assert.AreEqual("world", obj2.MoreText);

        }

        class Unconstructible1 : ICapnpSerializable
        {
            public Unconstructible1(int annoyingParameter)
            {

            }

            public void Deserialize(DeserializerState state)
            {
                throw new NotImplementedException();
            }

            public void Serialize(SerializerState state)
            {
                throw new NotImplementedException();
            }
        }

        class Unconstructible2 : ICapnpSerializable
        {
            public Unconstructible2()
            {
                throw new NotImplementedException();
            }

            public void Deserialize(DeserializerState state)
            {
                throw new NotImplementedException();
            }

            public void Serialize(SerializerState state)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void CapnpSerializableWrongUse()
        {
            var d = default(DeserializerState);
            Assert.ThrowsException<ArgumentException>(() => CapnpSerializable.Create<SerializationTests>(d));
            Assert.ThrowsException<ArgumentException>(() => CapnpSerializable.Create<IReadOnlyList<SerializationTests>>(d));
            Assert.ThrowsException<ArgumentException>(() => CapnpSerializable.Create<IReadOnlyList<DeserializerState>>(d));
            Assert.ThrowsException<ArgumentException>(() => CapnpSerializable.Create<Unconstructible1>(d));
            Assert.ThrowsException<ArgumentException>(() => CapnpSerializable.Create<Unconstructible2>(d));
        }

        [TestMethod]
        public void DynamicSerializerStateBytes()
        {
            var expected = new byte[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastByte().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateSBytes()
        {
            var expected = new sbyte[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastSByte().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateShorts()
        {
            var expected = new short[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastShort().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateUShorts()
        {
            var expected = new ushort[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastUShort().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateInts()
        {
            var expected = new int[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastInt().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateUInts()
        {
            var expected = new uint[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastUInt().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateLongs()
        {
            var expected = new long[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastLong().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateULongs()
        {
            var expected = new ulong[] { 1, 2, 3 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastULong().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateFloats()
        {
            var expected = new float[] { 1.0f, 2.0f, 3.0f };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastFloat().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateDoubles()
        {
            var expected = new double[] { 1.0, 2.0, 3.0 };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastDouble().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateBools()
        {
            var expected = new bool[] { true, true, false };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastBool().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateStrings()
        {
            var expected = new string[] { "foo", "bar", "baz" };

            var b = MessageBuilder.Create();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            CollectionAssert.AreEqual(expected, d.RequireList().CastText2().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateObjects()
        {
            var c = new Counters();
            var cap = new TestInterfaceImpl(c);
            var expected = new object[] { null, "foo", cap };

            var b = MessageBuilder.Create();
            b.InitCapTable();
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(expected);
            DeserializerState d = dss;
            var list = d.RequireList().Cast(_ => _);
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list[0].Kind == ObjectKind.Nil);
            Assert.AreEqual("foo", list[1].RequireList().CastText());
            var proxy = list[2].RequireCap<ITestInterface>();
            proxy.Foo(123u, true);
            Assert.AreEqual(1, c.CallCount);
        }

        [TestMethod]
        public void DynamicSerializerStateFromDeserializerState()
        {
            var expected = new string[] { "foo", "bar", "baz" };

            var b = MessageBuilder.Create();
            var lots = b.CreateObject<ListOfTextSerializer>();
            lots.Init(expected);
            DeserializerState d = lots;
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(d);
            DeserializerState d2 = dss;
            CollectionAssert.AreEqual(expected, d2.RequireList().CastText2().ToArray());
        }

        [TestMethod]
        public void DynamicSerializerStateFromSerializerState()
        {
            var expected = new string[] { "foo", "bar", "baz" };

            var b = MessageBuilder.Create();
            var lots = b.CreateObject<ListOfTextSerializer>();
            lots.Init(expected);
            var dss = b.CreateObject<DynamicSerializerState>();
            dss.SetObject(lots);
            DeserializerState d2 = dss;
            CollectionAssert.AreEqual(expected, d2.RequireList().CastText2().ToArray());
        }

        [TestMethod]
        public void EmptyListContract()
        {
            var list = new EmptyList<string>();
            Assert.AreEqual(0, list.Count);
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = list[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = list[0]; });
            Assert.AreEqual(0, list.ToArray().Length);
        }
    }
}
