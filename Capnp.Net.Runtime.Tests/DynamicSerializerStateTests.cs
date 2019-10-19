using CapnpGen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Capnp.Net.Runtime.Tests
{
    [TestClass]
    public class DynamicSerializerStateTests
    {
        [TestMethod]
        public void Struct()
        {
            var mb = MessageBuilder.Create(8);
            var ds = new DynamicSerializerState(mb);
            var alloc = mb.Allocator;
            ds.SetStruct(3, 2);
            Assert.IsFalse(ds.IsAllocated);
            Assert.ThrowsException<InvalidOperationException>(() => ds.SetListOfPointers(1));
            Assert.ThrowsException<InvalidOperationException>(() => ds.SetListOfStructs(3, 2, 1));
            Assert.ThrowsException<InvalidOperationException>(() => ds.SetListOfValues(8, 3));
            Assert.ThrowsException<InvalidOperationException>(() => ds.SetStruct(2, 3));
            ds.SetStruct(3, 2);
            ds.Allocate();
            Assert.IsTrue(ds.IsAllocated);
            Assert.AreEqual(3, ds.StructDataSection.Length);
            ds.StructWriteData(0, 16, 0x4321);
            ds.StructWriteData(64, 32, 0x87654321);
            ds.StructWriteData(128, 64, 0x1234567812345678);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => ds.StructWriteData(191, 2, 1));

            var ds2 = ds.BuildPointer(0);
            ds2.SetStruct(1, 0);
            ds2.WriteData(0, 1.23);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => ds.Link(2, ds));

            Assert.AreEqual(1, alloc.Segments.Count);
            Assert.AreEqual(7, alloc.Segments[0].Length);

            DeserializerState d = ds;
            Assert.AreEqual(ObjectKind.Struct, d.Kind);
            Assert.AreEqual(3, d.StructDataCount);
            Assert.AreEqual(2, d.StructPtrCount);
            Assert.AreEqual(0x4321, d.ReadDataUShort(0));
            Assert.AreEqual(0x87654321, d.ReadDataUInt(64));
            Assert.IsTrue(0x1234567812345678 == d.ReadDataULong(128));
            var p0 = d.StructReadPointer(0);
            Assert.AreEqual(ObjectKind.Struct, p0.Kind);
            Assert.AreEqual(1.23, p0.ReadDataDouble(0));
            var p1 = d.StructReadPointer(1);
            Assert.AreEqual(ObjectKind.Nil, p1.Kind);
        }

        [TestMethod]
        public void StructWithPrimitives()
        {
            var mb = MessageBuilder.Create(8);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            ds.SetStruct(10, 0);
            ds.WriteData(0, ulong.MaxValue - 1, ulong.MaxValue);
            ds.WriteData(64, long.MaxValue - 1, -123);
            ds.WriteData(128, uint.MaxValue - 1, 123);
            ds.WriteData(160, int.MaxValue - 1, -123);
            ds.WriteData(192, (ushort)(ushort.MaxValue - 1), (ushort)321);
            ds.WriteData(208, (short)(short.MaxValue - 1), (short)321);
            ds.WriteData(224, (byte)(byte.MaxValue - 1), (byte)111);
            ds.WriteData(232, (sbyte)(sbyte.MaxValue - 1), (sbyte)(-111));
            ds.WriteData(240, false, false);
            ds.WriteData(241, false, true);
            ds.WriteData(242, true, false);
            ds.WriteData(243, true, true);
            ds.WriteData(256, 12.34, 0.5);
            ds.WriteData(320, 1.2f, 0.5f);

            DeserializerState d = ds;
            Assert.AreEqual(ulong.MaxValue - 1, d.ReadDataULong(0, ulong.MaxValue));
            Assert.AreEqual(long.MaxValue - 1, d.ReadDataLong(64, -123));
            Assert.AreEqual(uint.MaxValue - 1, d.ReadDataUInt(128, 123));
            Assert.AreEqual(int.MaxValue - 1, d.ReadDataInt(160, -123));
            Assert.AreEqual(ushort.MaxValue - 1, d.ReadDataUShort(192, 321));
            Assert.AreEqual(short.MaxValue - 1, d.ReadDataShort(208, 321));
            Assert.AreEqual(byte.MaxValue - 1, d.ReadDataByte(224, 111));
            Assert.AreEqual(sbyte.MaxValue - 1, d.ReadDataSByte(232, -111));
            Assert.AreEqual(false, d.ReadDataBool(240, false));
            Assert.AreEqual(false, d.ReadDataBool(241, true));
            Assert.AreEqual(true, d.ReadDataBool(242, false));
            Assert.AreEqual(true, d.ReadDataBool(243, true));
            Assert.AreEqual(12.34, d.ReadDataDouble(256, 0.5));
            Assert.AreEqual(1.2f, d.ReadDataFloat(320, 0.5f));
        }

        [TestMethod]
        public void StructRecursion()
        {
            var mb = MessageBuilder.Create(8);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            ds.SetStruct(1, 1);
            ds.WriteData(0, 123);
            ds.Link(0, ds, false);

            DeserializerState d = ds;
            Assert.AreEqual(123, d.ReadDataInt(0));
            Assert.AreEqual(123, d.StructReadPointer(0).ReadDataInt(0));
            Assert.ThrowsException<DeserializationException>(() =>
            {
                for (int i = 0; i < 64000000; i++)
                {
                    d = d.StructReadPointer(0);
                }
            });
        }

        [TestMethod]
        public void StructWithLists()
        {
            var mb = MessageBuilder.Create(8);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            {
                ds.SetStruct(0, 2);
                ds.WriteText(0, "Lorem ipsum");
                var los = ds.BuildPointer(1);
                los.SetListOfStructs(3, 1, 1);
                var e0 = los.ListBuildStruct(0);
                e0.WriteData(0, long.MinValue);
                e0.WriteText(0, long.MinValue.ToString());
                var e1 = los.ListBuildStruct(1);
                e1.WriteData(0, 0L);
                e1.WriteText(0, 0L.ToString());
                var e2 = los.ListBuildStruct(2);
                e2.WriteData(0, long.MaxValue);
                e2.WriteText(0, long.MaxValue.ToString());
            }

            {
                DeserializerState d = ds;
                Assert.AreEqual(ObjectKind.Struct, d.Kind);
                Assert.AreEqual("Lorem ipsum", d.ReadText(0));
                var los = d.ReadListOfStructs(1, _ => _);
                Assert.AreEqual(3, los.Count);
                Assert.AreEqual(long.MinValue, los[0].ReadDataLong(0));
                Assert.AreEqual(long.MinValue.ToString(), los[0].ReadText(0));
                Assert.AreEqual(0L, los[1].ReadDataLong(0));
                Assert.AreEqual(0L.ToString(), los[1].ReadText(0));
                Assert.AreEqual(long.MaxValue, los[2].ReadDataLong(0));
                Assert.AreEqual(long.MaxValue.ToString(), los[2].ReadText(0));
            }
        }

        [TestMethod]
        public void MultiSegmentStruct()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            ds.SetStruct(1, 2);
            ds.WriteData(0, 815);
            string s0 = "Lorem ipsum dolor sit amet";
            ds.WriteText(0, s0);
            string s1 = "All men are created equal";
            ds.WriteText(1, s1);
            Assert.IsTrue(alloc.Segments.Count >= 3);

            DeserializerState d = ds;
            Assert.AreEqual(815, d.ReadDataInt(0));
            Assert.AreEqual(s0, d.ReadText(0));
            Assert.AreEqual(s1, d.ReadText(1));
        }

        [TestMethod]
        public void ListOfEmpty()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            ds.SetListOfValues(0, int.MaxValue);
            Assert.ThrowsException<InvalidOperationException>(
                () => ds.ListWriteValue(0, 1, 0));

            DeserializerState d = ds;
            Assert.AreEqual(ObjectKind.ListOfEmpty, d.Kind);
            Assert.AreEqual(int.MaxValue, d.ListElementCount);
        }

        [TestMethod]
        public void ListOfBits()
        {
            bool ValueGen(int index)
            {
                return (index % 3) == 0;
            }

            DynamicSerializerState CreateListBulk(int count)
            {
                var mb = MessageBuilder.Create(8);
                var alloc = mb.Allocator;
                var ds = new DynamicSerializerState(mb);
                ds.SetListOfValues(1, count);
                var list = new List<bool>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(ValueGen(i));
                }
                ds.ListWriteValues(list);

                return ds;
            }

            DynamicSerializerState CreateListSingle(int count)
            {
                var mb = MessageBuilder.Create(8);
                var alloc = mb.Allocator;
                var ds = new DynamicSerializerState(mb);
                ds.SetListOfValues(1, count);
                for (int i = 0; i < count; i++)
                {
                    ds.ListWriteValue(i, ValueGen(i));
                }

                return ds;
            }

            void VerifyList(DeserializerState d, int count)
            {
                Assert.AreEqual(ObjectKind.ListOfBits, d.Kind);
                Assert.AreEqual(count, d.ListElementCount);
                var rlist = d.RequireList().CastBool();
                Assert.AreEqual(count, rlist.Count);
                for (int i = 0; i < count; i++)
                {
                    bool expected = ValueGen(i);
                    bool actual = rlist[i];
                    Assert.AreEqual(expected, actual);
                }
                Assert.ThrowsException<IndexOutOfRangeException>(() =>
                {
                    var dummy = rlist[-1];
                });
                Assert.ThrowsException<IndexOutOfRangeException>(() =>
                {
                    var dummy = rlist[count];
                });
            }

            int c = 123;
            VerifyList(CreateListBulk(c), c);
            VerifyList(CreateListSingle(c), c);
        }

        [TestMethod]
        public void ListOfSBytes()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int count = 100;
            ds.SetListOfValues(8, count);
            for (int i = 0; i < count; i++)
            {
                ds.ListWriteValue(i, (sbyte)i);
            }

            DeserializerState d = ds;
            Assert.AreEqual(count, d.ListElementCount);
            var sbytes = d.RequireList().CastSByte();
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual((sbyte)i, sbytes[i]);
            }

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = sbytes[-1];
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = sbytes[count];
            });
        }

        [TestMethod]
        public void ListOfBytes()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int count = 277;
            ds.SetListOfValues(8, count);
            for (int i = 0; i < count; i++)
            {
                ds.ListWriteValue(i, (byte)i);
            }

            DeserializerState d = ds;
            Assert.AreEqual(count, d.ListElementCount);
            var bytes = d.RequireList().CastByte();
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual((byte)i, bytes[i]);
            }

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = bytes[-1];
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = bytes[count];
            });
        }

        [TestMethod]
        public void ListOfShorts()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int count = 333;
            ds.SetListOfValues(16, count);
            for (int i = 0; i < count; i++)
            {
                ds.ListWriteValue(i, (short)i);
            }

            DeserializerState d = ds;
            Assert.AreEqual(count, d.ListElementCount);
            var shorts = d.RequireList().CastShort();
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual((short)i, shorts[i]);
            }

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = shorts[-1];
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = shorts[count];
            });
        }

        [TestMethod]
        public void ListOfUShorts()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int count = 234;
            ds.SetListOfValues(16, count);
            for (int i = 0; i < count; i++)
            {
                ds.ListWriteValue(i, (ushort)i);
            }

            DeserializerState d = ds;
            Assert.AreEqual(count, d.ListElementCount);
            var ushorts = d.RequireList().CastUShort();
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual((ushort)i, ushorts[i]);
            }

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = ushorts[-1];
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = ushorts[count];
            });
        }

        [TestMethod]
        public void ListOfInts()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int count = 1234;
            ds.SetListOfValues(32, count);
            for (int i = 0; i < count; i++)
            {
                ds.ListWriteValue(i, i);
            }

            DeserializerState d = ds;
            Assert.AreEqual(count, d.ListElementCount);
            var ints = d.RequireList().CastInt();
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(i, ints[i]);
            }

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = ints[-1];
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = ints[count];
            });
        }

        [TestMethod]
        public void ListOfUInts()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int count = 321;
            ds.SetListOfValues(32, count);
            for (int i = 0; i < count; i++)
            {
                ds.ListWriteValue(i, (uint)i);
            }

            DeserializerState d = ds;
            Assert.AreEqual(count, d.ListElementCount);
            var uints = d.RequireList().CastUInt();
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual((uint)i, uints[i]);
            }

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = uints[-1];
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = uints[count];
            });
        }

        [TestMethod]
        public void ListOfLongs()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int count = 432;
            ds.SetListOfValues(64, count);
            for (int i = 0; i < count; i++)
            {
                ds.ListWriteValue(i, (long)i);
            }

            DeserializerState d = ds;
            Assert.AreEqual(count, d.ListElementCount);
            var longs = d.RequireList().CastLong();
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual((long)i, longs[i]);
            }
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = longs[-1];
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = longs[count];
            });
        }

        [TestMethod]
        public void ListOfULongs()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int count = 345;
            ds.SetListOfValues(64, count);
            for (int i = 0; i < count; i++)
            {
                ds.ListWriteValue(i, (ulong)i);
            }

            DeserializerState d = ds;
            Assert.AreEqual(count, d.ListElementCount);
            var ulongs = d.RequireList().CastULong();
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual((ulong)i, ulongs[i]);
            }
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = ulongs[-1];
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = ulongs[count];
            });
        }

        [TestMethod]
        public void ListOfPointers()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int count = 77;
            ds.SetListOfPointers(count);
            var dse0 = ds.BuildPointer(0);
            dse0.SetStruct(1, 0);
            dse0.WriteData(0, 654);
            var dse1 = new DynamicSerializerState(mb);
            dse1.SetStruct(1, 0);
            dse1.WriteData(0, 555);
            ds.Link(1, dse1);
            ds.Link(2, dse1);
            var dse3 = ds.BuildPointer(3);
            dse3.SetListOfValues(32, 10);

            DeserializerState d = ds;
            Assert.AreEqual(ObjectKind.ListOfPointers, d.Kind);
            Assert.AreEqual(count, d.ListElementCount);
            var pointers = d.RequireList().Cast(_ => _);
            Assert.AreEqual(ObjectKind.Struct, pointers[0].Kind);
            Assert.AreEqual(654, pointers[0].ReadDataInt(0));
            Assert.AreEqual(ObjectKind.Struct, pointers[1].Kind);
            Assert.AreEqual(555, pointers[1].ReadDataInt(0));
            Assert.AreEqual(ObjectKind.Struct, pointers[2].Kind);
            Assert.AreEqual(555, pointers[2].ReadDataInt(0));
            Assert.AreEqual(ObjectKind.ListOfInts, pointers[3].Kind);
            Assert.AreEqual(10, pointers[3].ListElementCount);
            Assert.AreEqual(ObjectKind.Nil, pointers[4].Kind);
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = pointers[-1];
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = pointers[count];
            });
        }

        [TestMethod]
        public void ListOfStructs()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int count = 23;
            ds.SetListOfStructs(count, 2, 1);
            var dse0 = ds.ListBuildStruct(0);
            dse0.WriteData(0, 1);
            dse0.WriteData(64, 2);
            var dse1 = ds.ListBuildStruct(1);
            dse1.WriteData(0, 3);
            dse1.WriteData(64, 4);
            Assert.ThrowsException<InvalidOperationException>(
                () => ds.Link(2, ds));
            Assert.ThrowsException<InvalidOperationException>(
                () => ds.BuildPointer(2));

            DeserializerState d = ds;
            Assert.AreEqual(ObjectKind.ListOfStructs, d.Kind);
            Assert.AreEqual(count, d.ListElementCount);
            var pointers = d.RequireList().Cast(_ => _);
            Assert.AreEqual(ObjectKind.Struct, pointers[0].Kind);
            Assert.AreEqual(1, pointers[0].ReadDataInt(0));
            Assert.AreEqual(2, pointers[0].ReadDataInt(64));
            Assert.AreEqual(ObjectKind.Struct, pointers[1].Kind);
            Assert.AreEqual(3, pointers[1].ReadDataInt(0));
            Assert.AreEqual(4, pointers[1].ReadDataInt(64));
            Assert.AreEqual(ObjectKind.Struct, pointers[2].Kind);
            Assert.AreEqual(0, pointers[2].ReadDataInt(0));
            Assert.AreEqual(0, pointers[2].ReadDataInt(64));
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = pointers[-1];
            });
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                var dummy = pointers[count];
            });
        }

        [TestMethod]
        public void Capability()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            ds.SetStruct(0, 1);
            ds.LinkToCapability(0, 13);

            DeserializerState d = ds;
            Assert.AreEqual(ObjectKind.Struct, d.Kind);
            var p = d.StructReadPointer(0);
            Assert.AreEqual(ObjectKind.Capability, p.Kind);
            Assert.AreEqual(13u, p.CapabilityIndex);
        }

        [TestMethod]
        public void Abuse()
        {
            var mb = MessageBuilder.Create(1);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                ds.SetListOfPointers(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                ds.SetListOfStructs(-10, 100, 200));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                ds.SetListOfValues(2, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                ds.SetListOfValues(1, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                ds.SetListOfValues(65, 1));
        }

        [TestMethod]
        public void SharedPointer()
        {
            var mb = MessageBuilder.Create(128);
            var alloc = mb.Allocator;

            var ds0 = new DynamicSerializerState(mb);
            ds0.SetStruct(1, 0);
            ds0.WriteData(0, 123);

            var ds1 = new DynamicSerializerState(mb);
            ds1.SetStruct(0, 1);
            ds1.Link(0, ds0);

            var ds2 = new DynamicSerializerState(mb);
            ds2.SetStruct(0, 1);
            ds2.Link(0, ds0);

            DeserializerState d1 = ds1;
            Assert.AreEqual(ObjectKind.Struct, d1.Kind);
            var e1 = d1.StructReadPointer(0);
            Assert.AreEqual(ObjectKind.Struct, e1.Kind);
            Assert.AreEqual(123, e1.ReadDataInt(0));

            DeserializerState d2 = ds2;
            Assert.AreEqual(ObjectKind.Struct, d2.Kind);
            var e2 = d2.StructReadPointer(0);
            Assert.AreEqual(ObjectKind.Struct, e2.Kind);
            Assert.AreEqual(123, e2.ReadDataInt(0));
        }

        [TestMethod]
        public void DeepCopy()
        {
            var mb = MessageBuilder.Create(128);
            var alloc1 = mb.Allocator;
            var ds1 = new DynamicSerializerState(mb);
            ds1.SetStruct(10, 18);
            for (int i = 0; i < 10; i++)
            {
                ds1.WriteData(64 * (ulong)i, i);
                var el = ds1.BuildPointer(i);
                el.SetStruct(1, 0);
                el.WriteData(0, -i);
            }
            var los1 = ds1.BuildPointer(10);
            los1.SetListOfStructs(1, 1, 0);
            var el1 = los1.ListBuildStruct(0);
            el1.WriteData(0, 111);
            var lov1 = ds1.BuildPointer(11);
            lov1.SetListOfValues(1, 7);
            lov1.ListWriteValue(2, true);
            lov1.ListWriteValue(3, true);
            lov1.ListWriteValue(5, true);
            var lovu8 = ds1.BuildPointer(12);
            lovu8.SetListOfValues(8, 5);
            lovu8.ListWriteValue(1, (byte)0x55);
            lovu8.ListWriteValue(2, (byte)0xaa);
            var lovu16 = ds1.BuildPointer(13);
            lovu16.SetListOfValues(16, 3);
            lovu16.ListWriteValue(0, (ushort)0x1111);
            lovu16.ListWriteValue(1, (ushort)0x2222);
            lovu16.ListWriteValue(2, (ushort)0x3333);
            var lovu32 = ds1.BuildPointer(14);
            lovu32.SetListOfValues(32, 9);
            lovu32.ListWriteValue(1, 1u);
            lovu32.ListWriteValue(2, 4u);
            lovu32.ListWriteValue(3, 9u);
            lovu32.ListWriteValue(4, 16u);
            lovu32.ListWriteValue(5, 25u);
            lovu32.ListWriteValue(6, 36u);
            lovu32.ListWriteValue(7, 49u);
            lovu32.ListWriteValue(8, 64u);
            var lov64 = ds1.BuildPointer(15);
            lov64.SetListOfValues(64, 2);
            lov64.ListWriteValue(0, long.MinValue);
            lov64.ListWriteValue(1, long.MaxValue);
            var lop1 = ds1.BuildPointer(16);
            lop1.SetListOfPointers(1);
            lop1.LinkToCapability(0, 19);
            var loe1 = ds1.BuildPointer(17);
            loe1.SetListOfValues(0, 100);

            var mb2 = MessageBuilder.Create(128);
            var alloc2 = mb2.Allocator;
            var ds2 = new DynamicSerializerState(mb2);
            ds2.SetStruct(0, 3);
            Assert.ThrowsException<InvalidOperationException>(() =>
                ds2.Link(0, ds1, false));
            ds2.Link(0, ds1, true);
            var lop = ds2.BuildPointer(1);
            lop.SetListOfPointers(1);
            lop.Link(0, ds1);
            ds2.Link(2, el1, true);

            void VerifyBigStruct(DeserializerState ds)
            {
                Assert.AreEqual(ObjectKind.Struct, ds.Kind);
                Assert.AreEqual<ushort>(10, ds.StructDataCount);
                Assert.AreEqual<ushort>(18, ds.StructPtrCount);
                for (int i = 0; i < 10; i++)
                {
                    Assert.AreEqual(i, ds.ReadDataInt(64 * (ulong)i));
                    var el = ds.StructReadPointer(i);
                    Assert.AreEqual(ObjectKind.Struct, el.Kind);
                    Assert.AreEqual(-i, el.ReadDataInt(0));
                }
                var elx = ds.StructReadPointer(10);
                Assert.AreEqual(ObjectKind.ListOfStructs, elx.Kind);
                Assert.AreEqual(1, elx.ListElementCount);
                var el0 = elx.RequireList().Cast(_ => _)[0];
                Assert.AreEqual(111, el0.ReadDataInt(0));

                var e11 = ds.StructReadPointer(11);
                Assert.AreEqual(ObjectKind.ListOfBits, e11.Kind);
                Assert.AreEqual(7, e11.ListElementCount);
                Assert.IsTrue(e11.RequireList().CastBool()[2]);
                Assert.IsTrue(e11.RequireList().CastBool()[3]);
                Assert.IsTrue(e11.RequireList().CastBool()[5]);

                var e12 = ds.StructReadPointer(12);
                Assert.AreEqual(ObjectKind.ListOfBytes, e12.Kind);
                Assert.AreEqual(5, e12.ListElementCount);
                Assert.AreEqual((byte)0x55, e12.RequireList().CastByte()[1]);
                Assert.AreEqual((byte)0xaa, e12.RequireList().CastByte()[2]);

                var e13 = ds.StructReadPointer(13);
                Assert.AreEqual(ObjectKind.ListOfShorts, e13.Kind);
                Assert.AreEqual(3, e13.ListElementCount);
                Assert.AreEqual((ushort)0x1111, e13.RequireList().CastUShort()[0]);
                Assert.AreEqual((ushort)0x2222, e13.RequireList().CastUShort()[1]);
                Assert.AreEqual((ushort)0x3333, e13.RequireList().CastUShort()[2]);

                var e14 = ds.StructReadPointer(14);
                Assert.AreEqual(ObjectKind.ListOfInts, e14.Kind);
                Assert.AreEqual(9, e14.ListElementCount);
                Assert.AreEqual((uint)0, e14.RequireList().CastUInt()[0]);
                Assert.AreEqual((uint)1, e14.RequireList().CastUInt()[1]);
                Assert.AreEqual((uint)4, e14.RequireList().CastUInt()[2]);
                Assert.AreEqual((uint)9, e14.RequireList().CastUInt()[3]);
                Assert.AreEqual((uint)16, e14.RequireList().CastUInt()[4]);
                Assert.AreEqual((uint)25, e14.RequireList().CastUInt()[5]);
                Assert.AreEqual((uint)36, e14.RequireList().CastUInt()[6]);
                Assert.AreEqual((uint)49, e14.RequireList().CastUInt()[7]);
                Assert.AreEqual((uint)64, e14.RequireList().CastUInt()[8]);

                var e15 = ds.StructReadPointer(15);
                Assert.AreEqual(ObjectKind.ListOfLongs, e15.Kind);
                Assert.AreEqual(2, e15.ListElementCount);
                Assert.AreEqual(long.MinValue, e15.RequireList().CastLong()[0]);
                Assert.AreEqual(long.MaxValue, e15.RequireList().CastLong()[1]);

                var e16 = ds.StructReadPointer(16);
                Assert.AreEqual(ObjectKind.ListOfPointers, e16.Kind);
                Assert.AreEqual(1, e16.ListElementCount);
                var cap = e16.RequireList().Cast(_ => _)[0];
                Assert.AreEqual(ObjectKind.Capability, cap.Kind);
                Assert.AreEqual(19u, cap.CapabilityIndex);

                var e17 = ds.StructReadPointer(17);
                Assert.AreEqual(ObjectKind.ListOfEmpty, e17.Kind);
                Assert.AreEqual(100, e17.ListElementCount);
            }

            DeserializerState d = ds2;
            Assert.AreEqual(ObjectKind.Struct, d.Kind);
            var p0 = d.StructReadPointer(0);
            VerifyBigStruct(p0);
            var p1 = d.StructReadPointer(1);
            Assert.AreEqual(ObjectKind.ListOfPointers, p1.Kind);
            var p1el0 = p1.RequireList().Cast(_ => _)[0];
            VerifyBigStruct(p1el0);
            var p2 = d.StructReadPointer(2);
            Assert.AreEqual(ObjectKind.Struct, p2.Kind);
            Assert.AreEqual(111, p2.ReadDataInt(0));
        }

        [TestMethod]
        public void List2D()
        {
            var mb = MessageBuilder.Create(128);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int w = 4;
            int h = 5;
            ds.SetListOfPointers(w);
            for (int i = 0; i < w; i++)
            {
                var p = ds.BuildPointer(i);
                p.SetListOfValues(32, h);
                for (int j = 0; j < h; j++)
                {
                    p.ListWriteValue(j, i - j);
                }
            }

            DeserializerState d = ds;
            var matrix = d.RequireList().Cast2D<int>();
            Assert.AreEqual(matrix.Count, w);
            for (int i = 0; i < w; i++)
            {
                var v = matrix[i];
                Assert.AreEqual(h, v.Count);
                for (int j = 0; j < h; j++)
                {
                    Assert.AreEqual(i - j, v[j]);
                }
            }
        }

        [TestMethod]
        public void List3D()
        {
            var mb = MessageBuilder.Create(128);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int d0 = 3;
            int d1 = 2;
            int d2 = 4;
            ds.SetListOfPointers(d0);
            for (int i = 0; i < d0; i++)
            {
                var p = ds.BuildPointer(i);
                p.SetListOfPointers(d1);
                for (int j = 0; j < d1; j++)
                {
                    var q = p.BuildPointer(j);
                    q.SetListOfValues(32, d2);
                    for (int k = 0; k < d2; k++)
                    {
                        q.ListWriteValue(k, i ^ j ^ k);
                    }
                }
            }

            DeserializerState d = ds;
            var qube = d.RequireList().Cast3D<int>();
            Assert.AreEqual(qube.Count, d0);
            for (int i = 0; i < d0; i++)
            {
                var matrix = qube[i];
                Assert.AreEqual(d1, matrix.Count);
                for (int j = 0; j < d1; j++)
                {
                    var vector = matrix[j];
                    for (int k = 0; k < d2; k++)
                    {
                        Assert.AreEqual(i ^ j ^ k, vector[k]);
                    }
                }
            }
        }

        [TestMethod]
        public void List4D()
        {
            var mb = MessageBuilder.Create(128);
            var alloc = mb.Allocator;
            var ds = new DynamicSerializerState(mb);
            int d0 = 3;
            int d1 = 2;
            int d2 = 4;
            int d3 = 5;
            ds.SetListOfPointers(d0);
            for (int i = 0; i < d0; i++)
            {
                var p = ds.BuildPointer(i);
                p.SetListOfPointers(d1);
                for (int j = 0; j < d1; j++)
                {
                    var q = p.BuildPointer(j);
                    q.SetListOfPointers(d2);
                    for (int k = 0; k < d2; k++)
                    {
                        var r = q.BuildPointer(k);
                        r.SetListOfValues(32, d3);
                        for (int l = 0; l < d3; l++)
                        {
                            r.ListWriteValue(l, (float)(i * j + k * l));
                        }
                    }
                }
            }

            DeserializerState d = ds;

            var hqube = (IReadOnlyList<object>) d.RequireList().CastND<float>(4);

            Assert.AreEqual(hqube.Count, d0);
            for (int i = 0; i < d0; i++)
            {
                var qube = (IReadOnlyList<object>)hqube[i];
                Assert.AreEqual(d1, qube.Count);
                for (int j = 0; j < d1; j++)
                {
                    var matrix = (IReadOnlyList<object>)qube[j];
                    Assert.AreEqual(d2, matrix.Count);
                    for (int k = 0; k < d2; k++)
                    {
                        var vector = (IReadOnlyList<float>)matrix[k];
                        Assert.AreEqual(d3, vector.Count);
                        for (int l = 0; l < d3; l++)
                        {
                            Assert.AreEqual((float)(i * j + k * l), vector[l]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Issue20()
        {
            RpcRequest<ArithmeticOperationRequest> rpcRequest = new RpcRequest<ArithmeticOperationRequest>();
            rpcRequest.Method = "AddTwoNumbers";

            ArithmeticOperationRequest request = new ArithmeticOperationRequest();

            request.NumA = 5;
            request.NumB = 8;

            rpcRequest.Request = request;

            var msg = MessageBuilder.Create();
            var root = msg.BuildRoot<RpcRequest<ArithmeticOperationRequest>.WRITER>();
            rpcRequest.serialize(root);

            var mems = new MemoryStream();
            var pump = new FramePump(mems);
            pump.Send(msg.Frame);
            mems.Seek(0, SeekOrigin.Begin);

            var frame = Framing.ReadSegments(mems);
            var deserializer = DeserializerState.CreateRoot(frame);
            var mainRequest = new RpcRequest<ArithmeticOperationRequest>.READER(deserializer);
            var innerRequest = new ArithmeticOperationRequest.READER(mainRequest.Request);

            Console.WriteLine("Method Name: " + mainRequest.Method);
            Console.WriteLine("NumA: " + innerRequest.NumA.ToString());
            Console.WriteLine("NumB: " + innerRequest.NumB.ToString());
        }
    }
}
