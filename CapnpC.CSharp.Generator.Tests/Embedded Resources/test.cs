using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capnproto_test.Capnp.Test
{
    [TypeId(0x9c8e9318b29d9cd3UL)]
    public enum TestEnum : ushort
    {
        foo,
        bar,
        baz,
        qux,
        quux,
        corge,
        grault,
        garply
    }

    [TypeId(0xa0a8f314b80b63fdUL)]
    public class TestAllTypes : ICapnpSerializable
    {
        public const UInt64 typeId = 0xa0a8f314b80b63fdUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            BoolField = reader.BoolField;
            Int8Field = reader.Int8Field;
            Int16Field = reader.Int16Field;
            Int32Field = reader.Int32Field;
            Int64Field = reader.Int64Field;
            UInt8Field = reader.UInt8Field;
            UInt16Field = reader.UInt16Field;
            UInt32Field = reader.UInt32Field;
            UInt64Field = reader.UInt64Field;
            Float32Field = reader.Float32Field;
            Float64Field = reader.Float64Field;
            TextField = reader.TextField;
            DataField = reader.DataField;
            StructField = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(reader.StructField);
            EnumField = reader.EnumField;
            VoidList = reader.VoidList;
            BoolList = reader.BoolList;
            Int8List = reader.Int8List;
            Int16List = reader.Int16List;
            Int32List = reader.Int32List;
            Int64List = reader.Int64List;
            UInt8List = reader.UInt8List;
            UInt16List = reader.UInt16List;
            UInt32List = reader.UInt32List;
            UInt64List = reader.UInt64List;
            Float32List = reader.Float32List;
            Float64List = reader.Float64List;
            TextList = reader.TextList;
            DataList = reader.DataList;
            StructList = reader.StructList?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(_));
            EnumList = reader.EnumList;
            InterfaceList = reader.InterfaceList;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.BoolField = BoolField;
            writer.Int8Field = Int8Field;
            writer.Int16Field = Int16Field;
            writer.Int32Field = Int32Field;
            writer.Int64Field = Int64Field;
            writer.UInt8Field = UInt8Field;
            writer.UInt16Field = UInt16Field;
            writer.UInt32Field = UInt32Field;
            writer.UInt64Field = UInt64Field;
            writer.Float32Field = Float32Field;
            writer.Float64Field = Float64Field;
            writer.TextField = TextField;
            writer.DataField.Init(DataField);
            StructField?.serialize(writer.StructField);
            writer.EnumField = EnumField;
            writer.VoidList.Init(VoidList);
            writer.BoolList.Init(BoolList);
            writer.Int8List.Init(Int8List);
            writer.Int16List.Init(Int16List);
            writer.Int32List.Init(Int32List);
            writer.Int64List.Init(Int64List);
            writer.UInt8List.Init(UInt8List);
            writer.UInt16List.Init(UInt16List);
            writer.UInt32List.Init(UInt32List);
            writer.UInt64List.Init(UInt64List);
            writer.Float32List.Init(Float32List);
            writer.Float64List.Init(Float64List);
            writer.TextList.Init(TextList);
            writer.DataList.Init(DataList, (_s1, _v1) => _s1.Init(_v1));
            writer.StructList.Init(StructList, (_s1, _v1) => _v1?.serialize(_s1));
            writer.EnumList.Init(EnumList);
            writer.InterfaceList.Init(InterfaceList);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public bool BoolField
        {
            get;
            set;
        }

        public sbyte Int8Field
        {
            get;
            set;
        }

        public short Int16Field
        {
            get;
            set;
        }

        public int Int32Field
        {
            get;
            set;
        }

        public long Int64Field
        {
            get;
            set;
        }

        public byte UInt8Field
        {
            get;
            set;
        }

        public ushort UInt16Field
        {
            get;
            set;
        }

        public uint UInt32Field
        {
            get;
            set;
        }

        public ulong UInt64Field
        {
            get;
            set;
        }

        public float Float32Field
        {
            get;
            set;
        }

        public double Float64Field
        {
            get;
            set;
        }

        public string TextField
        {
            get;
            set;
        }

        public IReadOnlyList<byte> DataField
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestAllTypes StructField
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestEnum EnumField
        {
            get;
            set;
        }

        public int VoidList
        {
            get;
            set;
        }

        public IReadOnlyList<bool> BoolList
        {
            get;
            set;
        }

        public IReadOnlyList<sbyte> Int8List
        {
            get;
            set;
        }

        public IReadOnlyList<short> Int16List
        {
            get;
            set;
        }

        public IReadOnlyList<int> Int32List
        {
            get;
            set;
        }

        public IReadOnlyList<long> Int64List
        {
            get;
            set;
        }

        public IReadOnlyList<byte> UInt8List
        {
            get;
            set;
        }

        public IReadOnlyList<ushort> UInt16List
        {
            get;
            set;
        }

        public IReadOnlyList<uint> UInt32List
        {
            get;
            set;
        }

        public IReadOnlyList<ulong> UInt64List
        {
            get;
            set;
        }

        public IReadOnlyList<float> Float32List
        {
            get;
            set;
        }

        public IReadOnlyList<double> Float64List
        {
            get;
            set;
        }

        public IReadOnlyList<string> TextList
        {
            get;
            set;
        }

        public IReadOnlyList<IReadOnlyList<byte>> DataList
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestAllTypes> StructList
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestEnum> EnumList
        {
            get;
            set;
        }

        public int InterfaceList
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public bool BoolField => ctx.ReadDataBool(0UL, false);
            public sbyte Int8Field => ctx.ReadDataSByte(8UL, (sbyte)0);
            public short Int16Field => ctx.ReadDataShort(16UL, (short)0);
            public int Int32Field => ctx.ReadDataInt(32UL, 0);
            public long Int64Field => ctx.ReadDataLong(64UL, 0L);
            public byte UInt8Field => ctx.ReadDataByte(128UL, (byte)0);
            public ushort UInt16Field => ctx.ReadDataUShort(144UL, (ushort)0);
            public uint UInt32Field => ctx.ReadDataUInt(160UL, 0U);
            public ulong UInt64Field => ctx.ReadDataULong(192UL, 0UL);
            public float Float32Field => ctx.ReadDataFloat(256UL, 0F);
            public double Float64Field => ctx.ReadDataDouble(320UL, 0);
            public string TextField => ctx.ReadText(0, null);
            public IReadOnlyList<byte> DataField => ctx.ReadList(1).CastByte();
            public Capnproto_test.Capnp.Test.TestAllTypes.READER StructField => ctx.ReadStruct(2, Capnproto_test.Capnp.Test.TestAllTypes.READER.create);
            public Capnproto_test.Capnp.Test.TestEnum EnumField => (Capnproto_test.Capnp.Test.TestEnum)ctx.ReadDataUShort(288UL, (ushort)0);
            public int VoidList => ctx.ReadList(3).Count;
            public IReadOnlyList<bool> BoolList => ctx.ReadList(4).CastBool();
            public IReadOnlyList<sbyte> Int8List => ctx.ReadList(5).CastSByte();
            public IReadOnlyList<short> Int16List => ctx.ReadList(6).CastShort();
            public IReadOnlyList<int> Int32List => ctx.ReadList(7).CastInt();
            public IReadOnlyList<long> Int64List => ctx.ReadList(8).CastLong();
            public IReadOnlyList<byte> UInt8List => ctx.ReadList(9).CastByte();
            public IReadOnlyList<ushort> UInt16List => ctx.ReadList(10).CastUShort();
            public IReadOnlyList<uint> UInt32List => ctx.ReadList(11).CastUInt();
            public IReadOnlyList<ulong> UInt64List => ctx.ReadList(12).CastULong();
            public IReadOnlyList<float> Float32List => ctx.ReadList(13).CastFloat();
            public IReadOnlyList<double> Float64List => ctx.ReadList(14).CastDouble();
            public IReadOnlyList<string> TextList => ctx.ReadList(15).CastText2();
            public IReadOnlyList<IReadOnlyList<byte>> DataList => ctx.ReadList(16).CastData();
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestAllTypes.READER> StructList => ctx.ReadList(17).Cast(Capnproto_test.Capnp.Test.TestAllTypes.READER.create);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestEnum> EnumList => ctx.ReadList(18).CastEnums(_0 => (Capnproto_test.Capnp.Test.TestEnum)_0);
            public int InterfaceList => ctx.ReadList(19).Count;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(6, 20);
            }

            public bool BoolField
            {
                get => this.ReadDataBool(0UL, false);
                set => this.WriteData(0UL, value, false);
            }

            public sbyte Int8Field
            {
                get => this.ReadDataSByte(8UL, (sbyte)0);
                set => this.WriteData(8UL, value, (sbyte)0);
            }

            public short Int16Field
            {
                get => this.ReadDataShort(16UL, (short)0);
                set => this.WriteData(16UL, value, (short)0);
            }

            public int Int32Field
            {
                get => this.ReadDataInt(32UL, 0);
                set => this.WriteData(32UL, value, 0);
            }

            public long Int64Field
            {
                get => this.ReadDataLong(64UL, 0L);
                set => this.WriteData(64UL, value, 0L);
            }

            public byte UInt8Field
            {
                get => this.ReadDataByte(128UL, (byte)0);
                set => this.WriteData(128UL, value, (byte)0);
            }

            public ushort UInt16Field
            {
                get => this.ReadDataUShort(144UL, (ushort)0);
                set => this.WriteData(144UL, value, (ushort)0);
            }

            public uint UInt32Field
            {
                get => this.ReadDataUInt(160UL, 0U);
                set => this.WriteData(160UL, value, 0U);
            }

            public ulong UInt64Field
            {
                get => this.ReadDataULong(192UL, 0UL);
                set => this.WriteData(192UL, value, 0UL);
            }

            public float Float32Field
            {
                get => this.ReadDataFloat(256UL, 0F);
                set => this.WriteData(256UL, value, 0F);
            }

            public double Float64Field
            {
                get => this.ReadDataDouble(320UL, 0);
                set => this.WriteData(320UL, value, 0);
            }

            public string TextField
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public ListOfPrimitivesSerializer<byte> DataField
            {
                get => BuildPointer<ListOfPrimitivesSerializer<byte>>(1);
                set => Link(1, value);
            }

            public Capnproto_test.Capnp.Test.TestAllTypes.WRITER StructField
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>(2);
                set => Link(2, value);
            }

            public Capnproto_test.Capnp.Test.TestEnum EnumField
            {
                get => (Capnproto_test.Capnp.Test.TestEnum)this.ReadDataUShort(288UL, (ushort)0);
                set => this.WriteData(288UL, (ushort)value, (ushort)0);
            }

            public ListOfEmptySerializer VoidList
            {
                get => BuildPointer<ListOfEmptySerializer>(3);
                set => Link(3, value);
            }

            public ListOfBitsSerializer BoolList
            {
                get => BuildPointer<ListOfBitsSerializer>(4);
                set => Link(4, value);
            }

            public ListOfPrimitivesSerializer<sbyte> Int8List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<sbyte>>(5);
                set => Link(5, value);
            }

            public ListOfPrimitivesSerializer<short> Int16List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<short>>(6);
                set => Link(6, value);
            }

            public ListOfPrimitivesSerializer<int> Int32List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<int>>(7);
                set => Link(7, value);
            }

            public ListOfPrimitivesSerializer<long> Int64List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<long>>(8);
                set => Link(8, value);
            }

            public ListOfPrimitivesSerializer<byte> UInt8List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<byte>>(9);
                set => Link(9, value);
            }

            public ListOfPrimitivesSerializer<ushort> UInt16List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<ushort>>(10);
                set => Link(10, value);
            }

            public ListOfPrimitivesSerializer<uint> UInt32List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<uint>>(11);
                set => Link(11, value);
            }

            public ListOfPrimitivesSerializer<ulong> UInt64List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<ulong>>(12);
                set => Link(12, value);
            }

            public ListOfPrimitivesSerializer<float> Float32List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<float>>(13);
                set => Link(13, value);
            }

            public ListOfPrimitivesSerializer<double> Float64List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<double>>(14);
                set => Link(14, value);
            }

            public ListOfTextSerializer TextList
            {
                get => BuildPointer<ListOfTextSerializer>(15);
                set => Link(15, value);
            }

            public ListOfPointersSerializer<ListOfPrimitivesSerializer<byte>> DataList
            {
                get => BuildPointer<ListOfPointersSerializer<ListOfPrimitivesSerializer<byte>>>(16);
                set => Link(16, value);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER> StructList
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>>(17);
                set => Link(17, value);
            }

            public ListOfPrimitivesSerializer<Capnproto_test.Capnp.Test.TestEnum> EnumList
            {
                get => BuildPointer<ListOfPrimitivesSerializer<Capnproto_test.Capnp.Test.TestEnum>>(18);
                set => Link(18, value);
            }

            public ListOfEmptySerializer InterfaceList
            {
                get => BuildPointer<ListOfEmptySerializer>(19);
                set => Link(19, value);
            }
        }
    }

    [TypeId(0xeb3f9ebe98c73cb6UL)]
    public class TestDefaults : ICapnpSerializable
    {
        public const UInt64 typeId = 0xeb3f9ebe98c73cb6UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            BoolField = reader.BoolField;
            Int8Field = reader.Int8Field;
            Int16Field = reader.Int16Field;
            Int32Field = reader.Int32Field;
            Int64Field = reader.Int64Field;
            UInt8Field = reader.UInt8Field;
            UInt16Field = reader.UInt16Field;
            UInt32Field = reader.UInt32Field;
            UInt64Field = reader.UInt64Field;
            Float32Field = reader.Float32Field;
            Float64Field = reader.Float64Field;
            TextField = reader.TextField;
            DataField = reader.DataField;
            StructField = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(reader.StructField);
            EnumField = reader.EnumField;
            VoidList = reader.VoidList;
            BoolList = reader.BoolList;
            Int8List = reader.Int8List;
            Int16List = reader.Int16List;
            Int32List = reader.Int32List;
            Int64List = reader.Int64List;
            UInt8List = reader.UInt8List;
            UInt16List = reader.UInt16List;
            UInt32List = reader.UInt32List;
            UInt64List = reader.UInt64List;
            Float32List = reader.Float32List;
            Float64List = reader.Float64List;
            TextList = reader.TextList;
            DataList = reader.DataList;
            StructList = reader.StructList?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(_));
            EnumList = reader.EnumList;
            InterfaceList = reader.InterfaceList;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.BoolField = BoolField;
            writer.Int8Field = Int8Field;
            writer.Int16Field = Int16Field;
            writer.Int32Field = Int32Field;
            writer.Int64Field = Int64Field;
            writer.UInt8Field = UInt8Field;
            writer.UInt16Field = UInt16Field;
            writer.UInt32Field = UInt32Field;
            writer.UInt64Field = UInt64Field;
            writer.Float32Field = Float32Field;
            writer.Float64Field = Float64Field;
            writer.TextField = TextField;
            writer.DataField.Init(DataField);
            StructField?.serialize(writer.StructField);
            writer.EnumField = EnumField;
            writer.VoidList.Init(VoidList);
            writer.BoolList.Init(BoolList);
            writer.Int8List.Init(Int8List);
            writer.Int16List.Init(Int16List);
            writer.Int32List.Init(Int32List);
            writer.Int64List.Init(Int64List);
            writer.UInt8List.Init(UInt8List);
            writer.UInt16List.Init(UInt16List);
            writer.UInt32List.Init(UInt32List);
            writer.UInt64List.Init(UInt64List);
            writer.Float32List.Init(Float32List);
            writer.Float64List.Init(Float64List);
            writer.TextList.Init(TextList);
            writer.DataList.Init(DataList, (_s1, _v1) => _s1.Init(_v1));
            writer.StructList.Init(StructList, (_s1, _v1) => _v1?.serialize(_s1));
            writer.EnumList.Init(EnumList);
            writer.InterfaceList.Init(InterfaceList);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
            TextField = TextField ?? "foo";
            DataField = DataField ?? new byte[]{98, 97, 114};
            StructField = StructField ?? new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = true, Int8Field = -12, Int16Field = 3456, Int32Field = -78901234, Int64Field = 56789012345678L, UInt8Field = 90, UInt16Field = 1234, UInt32Field = 56789012U, UInt64Field = 345678901234567890UL, Float32Field = -1.25E-10F, Float64Field = 345, TextField = "baz", DataField = new byte[]{113, 117, 120}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = "nested", DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = "really nested", DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 3, BoolList = new bool[]{false, true, false, true, true}, Int8List = new sbyte[]{12, -34, -128, 127}, Int16List = new short[]{1234, -5678, -32768, 32767}, Int32List = new int[]{12345678, -90123456, -2147483648, 2147483647}, Int64List = new long[]{123456789012345L, -678901234567890L, -9223372036854775808L, 9223372036854775807L}, UInt8List = new byte[]{12, 34, 0, 255}, UInt16List = new ushort[]{1234, 5678, 0, 65535}, UInt32List = new uint[]{12345678U, 90123456U, 0U, 4294967295U}, UInt64List = new ulong[]{123456789012345UL, 678901234567890UL, 0UL, 18446744073709551615UL}, Float32List = new float[]{0F, 1234567F, 1E+37F, -1E+37F, 1E-37F, -1E-37F}, Float64List = new double[]{0, 123456789012345, 1E+306, -1E+306, 1E-306, -1E-306}, TextList = new string[]{"quux", "corge", "grault"}, DataList = new IReadOnlyList<byte>[]{new byte[]{103, 97, 114, 112, 108, 121}, new byte[]{119, 97, 108, 100, 111}, new byte[]{102, 114, 101, 100}}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = "x structlist 1", DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = "x structlist 2", DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = "x structlist 3", DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{Capnproto_test.Capnp.Test.TestEnum.qux, Capnproto_test.Capnp.Test.TestEnum.bar, Capnproto_test.Capnp.Test.TestEnum.grault}, InterfaceList = 0};
            BoolList = BoolList ?? new bool[]{true, false, false, true};
            Int8List = Int8List ?? new sbyte[]{111, -111};
            Int16List = Int16List ?? new short[]{11111, -11111};
            Int32List = Int32List ?? new int[]{111111111, -111111111};
            Int64List = Int64List ?? new long[]{1111111111111111111L, -1111111111111111111L};
            UInt8List = UInt8List ?? new byte[]{111, 222};
            UInt16List = UInt16List ?? new ushort[]{33333, 44444};
            UInt32List = UInt32List ?? new uint[]{3333333333U};
            UInt64List = UInt64List ?? new ulong[]{11111111111111111111UL};
            Float32List = Float32List ?? new float[]{5555.5F, float.PositiveInfinity, float.NegativeInfinity, float.NaN};
            Float64List = Float64List ?? new double[]{7777.75, double.PositiveInfinity, double.NegativeInfinity, double.NaN};
            TextList = TextList ?? new string[]{"plugh", "xyzzy", "thud"};
            DataList = DataList ?? new IReadOnlyList<byte>[]{new byte[]{111, 111, 112, 115}, new byte[]{101, 120, 104, 97, 117, 115, 116, 101, 100}, new byte[]{114, 102, 99, 51, 48, 57, 50}};
            StructList = StructList ?? new Capnproto_test.Capnp.Test.TestAllTypes[]{new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = "structlist 1", DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = "structlist 2", DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = "structlist 3", DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}};
            EnumList = EnumList ?? new Capnproto_test.Capnp.Test.TestEnum[]{Capnproto_test.Capnp.Test.TestEnum.foo, Capnproto_test.Capnp.Test.TestEnum.garply};
        }

        public bool BoolField
        {
            get;
            set;
        }

        = true;
        public sbyte Int8Field
        {
            get;
            set;
        }

        = -123;
        public short Int16Field
        {
            get;
            set;
        }

        = -12345;
        public int Int32Field
        {
            get;
            set;
        }

        = -12345678;
        public long Int64Field
        {
            get;
            set;
        }

        = -123456789012345L;
        public byte UInt8Field
        {
            get;
            set;
        }

        = 234;
        public ushort UInt16Field
        {
            get;
            set;
        }

        = 45678;
        public uint UInt32Field
        {
            get;
            set;
        }

        = 3456789012U;
        public ulong UInt64Field
        {
            get;
            set;
        }

        = 12345678901234567890UL;
        public float Float32Field
        {
            get;
            set;
        }

        = 1234.5F;
        public double Float64Field
        {
            get;
            set;
        }

        = -1.23E+47;
        public string TextField
        {
            get;
            set;
        }

        public IReadOnlyList<byte> DataField
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestAllTypes StructField
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestEnum EnumField
        {
            get;
            set;
        }

        = Capnproto_test.Capnp.Test.TestEnum.corge;
        public int VoidList
        {
            get;
            set;
        }

        = 6;
        public IReadOnlyList<bool> BoolList
        {
            get;
            set;
        }

        public IReadOnlyList<sbyte> Int8List
        {
            get;
            set;
        }

        public IReadOnlyList<short> Int16List
        {
            get;
            set;
        }

        public IReadOnlyList<int> Int32List
        {
            get;
            set;
        }

        public IReadOnlyList<long> Int64List
        {
            get;
            set;
        }

        public IReadOnlyList<byte> UInt8List
        {
            get;
            set;
        }

        public IReadOnlyList<ushort> UInt16List
        {
            get;
            set;
        }

        public IReadOnlyList<uint> UInt32List
        {
            get;
            set;
        }

        public IReadOnlyList<ulong> UInt64List
        {
            get;
            set;
        }

        public IReadOnlyList<float> Float32List
        {
            get;
            set;
        }

        public IReadOnlyList<double> Float64List
        {
            get;
            set;
        }

        public IReadOnlyList<string> TextList
        {
            get;
            set;
        }

        public IReadOnlyList<IReadOnlyList<byte>> DataList
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestAllTypes> StructList
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestEnum> EnumList
        {
            get;
            set;
        }

        public int InterfaceList
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public bool BoolField => ctx.ReadDataBool(0UL, true);
            public sbyte Int8Field => ctx.ReadDataSByte(8UL, (sbyte)-123);
            public short Int16Field => ctx.ReadDataShort(16UL, (short)-12345);
            public int Int32Field => ctx.ReadDataInt(32UL, -12345678);
            public long Int64Field => ctx.ReadDataLong(64UL, -123456789012345L);
            public byte UInt8Field => ctx.ReadDataByte(128UL, (byte)234);
            public ushort UInt16Field => ctx.ReadDataUShort(144UL, (ushort)45678);
            public uint UInt32Field => ctx.ReadDataUInt(160UL, 3456789012U);
            public ulong UInt64Field => ctx.ReadDataULong(192UL, 12345678901234567890UL);
            public float Float32Field => ctx.ReadDataFloat(256UL, 1234.5F);
            public double Float64Field => ctx.ReadDataDouble(320UL, -1.23E+47);
            public string TextField => ctx.ReadText(0, "foo");
            public IReadOnlyList<byte> DataField => ctx.ReadList(1).CastByte();
            public Capnproto_test.Capnp.Test.TestAllTypes.READER StructField => ctx.ReadStruct(2, Capnproto_test.Capnp.Test.TestAllTypes.READER.create);
            public Capnproto_test.Capnp.Test.TestEnum EnumField => (Capnproto_test.Capnp.Test.TestEnum)ctx.ReadDataUShort(288UL, (ushort)5);
            public int VoidList => ctx.ReadList(3).Count;
            public IReadOnlyList<bool> BoolList => ctx.ReadList(4).CastBool();
            public IReadOnlyList<sbyte> Int8List => ctx.ReadList(5).CastSByte();
            public IReadOnlyList<short> Int16List => ctx.ReadList(6).CastShort();
            public IReadOnlyList<int> Int32List => ctx.ReadList(7).CastInt();
            public IReadOnlyList<long> Int64List => ctx.ReadList(8).CastLong();
            public IReadOnlyList<byte> UInt8List => ctx.ReadList(9).CastByte();
            public IReadOnlyList<ushort> UInt16List => ctx.ReadList(10).CastUShort();
            public IReadOnlyList<uint> UInt32List => ctx.ReadList(11).CastUInt();
            public IReadOnlyList<ulong> UInt64List => ctx.ReadList(12).CastULong();
            public IReadOnlyList<float> Float32List => ctx.ReadList(13).CastFloat();
            public IReadOnlyList<double> Float64List => ctx.ReadList(14).CastDouble();
            public IReadOnlyList<string> TextList => ctx.ReadList(15).CastText2();
            public IReadOnlyList<IReadOnlyList<byte>> DataList => ctx.ReadList(16).CastData();
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestAllTypes.READER> StructList => ctx.ReadList(17).Cast(Capnproto_test.Capnp.Test.TestAllTypes.READER.create);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestEnum> EnumList => ctx.ReadList(18).CastEnums(_0 => (Capnproto_test.Capnp.Test.TestEnum)_0);
            public int InterfaceList => ctx.ReadList(19).Count;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(6, 20);
            }

            public bool BoolField
            {
                get => this.ReadDataBool(0UL, true);
                set => this.WriteData(0UL, value, true);
            }

            public sbyte Int8Field
            {
                get => this.ReadDataSByte(8UL, (sbyte)-123);
                set => this.WriteData(8UL, value, (sbyte)-123);
            }

            public short Int16Field
            {
                get => this.ReadDataShort(16UL, (short)-12345);
                set => this.WriteData(16UL, value, (short)-12345);
            }

            public int Int32Field
            {
                get => this.ReadDataInt(32UL, -12345678);
                set => this.WriteData(32UL, value, -12345678);
            }

            public long Int64Field
            {
                get => this.ReadDataLong(64UL, -123456789012345L);
                set => this.WriteData(64UL, value, -123456789012345L);
            }

            public byte UInt8Field
            {
                get => this.ReadDataByte(128UL, (byte)234);
                set => this.WriteData(128UL, value, (byte)234);
            }

            public ushort UInt16Field
            {
                get => this.ReadDataUShort(144UL, (ushort)45678);
                set => this.WriteData(144UL, value, (ushort)45678);
            }

            public uint UInt32Field
            {
                get => this.ReadDataUInt(160UL, 3456789012U);
                set => this.WriteData(160UL, value, 3456789012U);
            }

            public ulong UInt64Field
            {
                get => this.ReadDataULong(192UL, 12345678901234567890UL);
                set => this.WriteData(192UL, value, 12345678901234567890UL);
            }

            public float Float32Field
            {
                get => this.ReadDataFloat(256UL, 1234.5F);
                set => this.WriteData(256UL, value, 1234.5F);
            }

            public double Float64Field
            {
                get => this.ReadDataDouble(320UL, -1.23E+47);
                set => this.WriteData(320UL, value, -1.23E+47);
            }

            public string TextField
            {
                get => this.ReadText(0, "foo");
                set => this.WriteText(0, value, "foo");
            }

            public ListOfPrimitivesSerializer<byte> DataField
            {
                get => BuildPointer<ListOfPrimitivesSerializer<byte>>(1);
                set => Link(1, value);
            }

            public Capnproto_test.Capnp.Test.TestAllTypes.WRITER StructField
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>(2);
                set => Link(2, value);
            }

            public Capnproto_test.Capnp.Test.TestEnum EnumField
            {
                get => (Capnproto_test.Capnp.Test.TestEnum)this.ReadDataUShort(288UL, (ushort)5);
                set => this.WriteData(288UL, (ushort)value, (ushort)5);
            }

            public ListOfEmptySerializer VoidList
            {
                get => BuildPointer<ListOfEmptySerializer>(3);
                set => Link(3, value);
            }

            public ListOfBitsSerializer BoolList
            {
                get => BuildPointer<ListOfBitsSerializer>(4);
                set => Link(4, value);
            }

            public ListOfPrimitivesSerializer<sbyte> Int8List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<sbyte>>(5);
                set => Link(5, value);
            }

            public ListOfPrimitivesSerializer<short> Int16List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<short>>(6);
                set => Link(6, value);
            }

            public ListOfPrimitivesSerializer<int> Int32List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<int>>(7);
                set => Link(7, value);
            }

            public ListOfPrimitivesSerializer<long> Int64List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<long>>(8);
                set => Link(8, value);
            }

            public ListOfPrimitivesSerializer<byte> UInt8List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<byte>>(9);
                set => Link(9, value);
            }

            public ListOfPrimitivesSerializer<ushort> UInt16List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<ushort>>(10);
                set => Link(10, value);
            }

            public ListOfPrimitivesSerializer<uint> UInt32List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<uint>>(11);
                set => Link(11, value);
            }

            public ListOfPrimitivesSerializer<ulong> UInt64List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<ulong>>(12);
                set => Link(12, value);
            }

            public ListOfPrimitivesSerializer<float> Float32List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<float>>(13);
                set => Link(13, value);
            }

            public ListOfPrimitivesSerializer<double> Float64List
            {
                get => BuildPointer<ListOfPrimitivesSerializer<double>>(14);
                set => Link(14, value);
            }

            public ListOfTextSerializer TextList
            {
                get => BuildPointer<ListOfTextSerializer>(15);
                set => Link(15, value);
            }

            public ListOfPointersSerializer<ListOfPrimitivesSerializer<byte>> DataList
            {
                get => BuildPointer<ListOfPointersSerializer<ListOfPrimitivesSerializer<byte>>>(16);
                set => Link(16, value);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER> StructList
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>>(17);
                set => Link(17, value);
            }

            public ListOfPrimitivesSerializer<Capnproto_test.Capnp.Test.TestEnum> EnumList
            {
                get => BuildPointer<ListOfPrimitivesSerializer<Capnproto_test.Capnp.Test.TestEnum>>(18);
                set => Link(18, value);
            }

            public ListOfEmptySerializer InterfaceList
            {
                get => BuildPointer<ListOfEmptySerializer>(19);
                set => Link(19, value);
            }
        }
    }

    [TypeId(0xe3da5a2ccd28c0d8UL)]
    public class TestAnyPointer : ICapnpSerializable
    {
        public const UInt64 typeId = 0xe3da5a2ccd28c0d8UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            AnyPointerField = CapnpSerializable.Create<object>(reader.AnyPointerField);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.AnyPointerField.SetObject(AnyPointerField);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public object AnyPointerField
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public DeserializerState AnyPointerField => ctx.StructReadPointer(0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public DynamicSerializerState AnyPointerField
            {
                get => BuildPointer<DynamicSerializerState>(0);
                set => Link(0, value);
            }
        }
    }

    [TypeId(0xf49850f63c2bfa59UL)]
    public class TestAnyOthers : ICapnpSerializable
    {
        public const UInt64 typeId = 0xf49850f63c2bfa59UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            AnyStructField = CapnpSerializable.Create<object>(reader.AnyStructField);
            AnyListField = reader.AnyListField?.ToReadOnlyList(_ => (object)_);
            CapabilityField = reader.CapabilityField;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.AnyStructField.SetObject(AnyStructField);
            writer.AnyListField.SetObject(AnyListField);
            writer.CapabilityField = CapabilityField;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public object AnyStructField
        {
            get;
            set;
        }

        public IReadOnlyList<object> AnyListField
        {
            get;
            set;
        }

        public BareProxy CapabilityField
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public DeserializerState AnyStructField => ctx.StructReadPointer(0);
            public IReadOnlyList<DeserializerState> AnyListField => (IReadOnlyList<DeserializerState>)ctx.ReadList(1);
            public BareProxy CapabilityField => ctx.ReadCap(2);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 3);
            }

            public DynamicSerializerState AnyStructField
            {
                get => BuildPointer<DynamicSerializerState>(0);
                set => Link(0, value);
            }

            public DynamicSerializerState AnyListField
            {
                get => BuildPointer<DynamicSerializerState>(1);
                set => Link(1, value);
            }

            public BareProxy CapabilityField
            {
                get => ReadCap<BareProxy>(2);
                set => LinkObject(2, value);
            }
        }
    }

    [TypeId(0xa9d5f8efe770022bUL)]
    public class TestOutOfOrder : ICapnpSerializable
    {
        public const UInt64 typeId = 0xa9d5f8efe770022bUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Qux = reader.Qux;
            Grault = reader.Grault;
            Bar = reader.Bar;
            Foo = reader.Foo;
            Corge = reader.Corge;
            Waldo = reader.Waldo;
            Quux = reader.Quux;
            Garply = reader.Garply;
            Baz = reader.Baz;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Qux = Qux;
            writer.Grault = Grault;
            writer.Bar = Bar;
            writer.Foo = Foo;
            writer.Corge = Corge;
            writer.Waldo = Waldo;
            writer.Quux = Quux;
            writer.Garply = Garply;
            writer.Baz = Baz;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Qux
        {
            get;
            set;
        }

        public string Grault
        {
            get;
            set;
        }

        public string Bar
        {
            get;
            set;
        }

        public string Foo
        {
            get;
            set;
        }

        public string Corge
        {
            get;
            set;
        }

        public string Waldo
        {
            get;
            set;
        }

        public string Quux
        {
            get;
            set;
        }

        public string Garply
        {
            get;
            set;
        }

        public string Baz
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public string Qux => ctx.ReadText(0, null);
            public string Grault => ctx.ReadText(1, null);
            public string Bar => ctx.ReadText(2, null);
            public string Foo => ctx.ReadText(3, null);
            public string Corge => ctx.ReadText(4, null);
            public string Waldo => ctx.ReadText(5, null);
            public string Quux => ctx.ReadText(6, null);
            public string Garply => ctx.ReadText(7, null);
            public string Baz => ctx.ReadText(8, null);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 9);
            }

            public string Qux
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public string Grault
            {
                get => this.ReadText(1, null);
                set => this.WriteText(1, value, null);
            }

            public string Bar
            {
                get => this.ReadText(2, null);
                set => this.WriteText(2, value, null);
            }

            public string Foo
            {
                get => this.ReadText(3, null);
                set => this.WriteText(3, value, null);
            }

            public string Corge
            {
                get => this.ReadText(4, null);
                set => this.WriteText(4, value, null);
            }

            public string Waldo
            {
                get => this.ReadText(5, null);
                set => this.WriteText(5, value, null);
            }

            public string Quux
            {
                get => this.ReadText(6, null);
                set => this.WriteText(6, value, null);
            }

            public string Garply
            {
                get => this.ReadText(7, null);
                set => this.WriteText(7, value, null);
            }

            public string Baz
            {
                get => this.ReadText(8, null);
                set => this.WriteText(8, value, null);
            }
        }
    }

    [TypeId(0xf47697362233ce52UL)]
    public class TestUnion : ICapnpSerializable
    {
        public const UInt64 typeId = 0xf47697362233ce52UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Union0 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUnion.union0>(reader.Union0);
            Union1 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUnion.union1>(reader.Union1);
            Union2 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUnion.union2>(reader.Union2);
            Union3 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUnion.union3>(reader.Union3);
            Bit0 = reader.Bit0;
            Bit2 = reader.Bit2;
            Bit3 = reader.Bit3;
            Bit4 = reader.Bit4;
            Bit5 = reader.Bit5;
            Bit6 = reader.Bit6;
            Bit7 = reader.Bit7;
            Byte0 = reader.Byte0;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Union0?.serialize(writer.Union0);
            Union1?.serialize(writer.Union1);
            Union2?.serialize(writer.Union2);
            Union3?.serialize(writer.Union3);
            writer.Bit0 = Bit0;
            writer.Bit2 = Bit2;
            writer.Bit3 = Bit3;
            writer.Bit4 = Bit4;
            writer.Bit5 = Bit5;
            writer.Bit6 = Bit6;
            writer.Bit7 = Bit7;
            writer.Byte0 = Byte0;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestUnion.union0 Union0
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestUnion.union1 Union1
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestUnion.union2 Union2
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestUnion.union3 Union3
        {
            get;
            set;
        }

        public bool Bit0
        {
            get;
            set;
        }

        public bool Bit2
        {
            get;
            set;
        }

        public bool Bit3
        {
            get;
            set;
        }

        public bool Bit4
        {
            get;
            set;
        }

        public bool Bit5
        {
            get;
            set;
        }

        public bool Bit6
        {
            get;
            set;
        }

        public bool Bit7
        {
            get;
            set;
        }

        public byte Byte0
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public union0.READER Union0 => new union0.READER(ctx);
            public union1.READER Union1 => new union1.READER(ctx);
            public union2.READER Union2 => new union2.READER(ctx);
            public union3.READER Union3 => new union3.READER(ctx);
            public bool Bit0 => ctx.ReadDataBool(128UL, false);
            public bool Bit2 => ctx.ReadDataBool(130UL, false);
            public bool Bit3 => ctx.ReadDataBool(131UL, false);
            public bool Bit4 => ctx.ReadDataBool(132UL, false);
            public bool Bit5 => ctx.ReadDataBool(133UL, false);
            public bool Bit6 => ctx.ReadDataBool(134UL, false);
            public bool Bit7 => ctx.ReadDataBool(135UL, false);
            public byte Byte0 => ctx.ReadDataByte(280UL, (byte)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(8, 2);
            }

            public union0.WRITER Union0
            {
                get => Rewrap<union0.WRITER>();
            }

            public union1.WRITER Union1
            {
                get => Rewrap<union1.WRITER>();
            }

            public union2.WRITER Union2
            {
                get => Rewrap<union2.WRITER>();
            }

            public union3.WRITER Union3
            {
                get => Rewrap<union3.WRITER>();
            }

            public bool Bit0
            {
                get => this.ReadDataBool(128UL, false);
                set => this.WriteData(128UL, value, false);
            }

            public bool Bit2
            {
                get => this.ReadDataBool(130UL, false);
                set => this.WriteData(130UL, value, false);
            }

            public bool Bit3
            {
                get => this.ReadDataBool(131UL, false);
                set => this.WriteData(131UL, value, false);
            }

            public bool Bit4
            {
                get => this.ReadDataBool(132UL, false);
                set => this.WriteData(132UL, value, false);
            }

            public bool Bit5
            {
                get => this.ReadDataBool(133UL, false);
                set => this.WriteData(133UL, value, false);
            }

            public bool Bit6
            {
                get => this.ReadDataBool(134UL, false);
                set => this.WriteData(134UL, value, false);
            }

            public bool Bit7
            {
                get => this.ReadDataBool(135UL, false);
                set => this.WriteData(135UL, value, false);
            }

            public byte Byte0
            {
                get => this.ReadDataByte(280UL, (byte)0);
                set => this.WriteData(280UL, value, (byte)0);
            }
        }

        [TypeId(0xfc76a82eecb7a718UL)]
        public class union0 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xfc76a82eecb7a718UL;
            public enum WHICH : ushort
            {
                U0f0s0 = 0,
                U0f0s1 = 1,
                U0f0s8 = 2,
                U0f0s16 = 3,
                U0f0s32 = 4,
                U0f0s64 = 5,
                U0f0sp = 6,
                U0f1s0 = 7,
                U0f1s1 = 8,
                U0f1s8 = 9,
                U0f1s16 = 10,
                U0f1s32 = 11,
                U0f1s64 = 12,
                U0f1sp = 13,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.U0f0s0:
                        which = reader.which;
                        break;
                    case WHICH.U0f0s1:
                        U0f0s1 = reader.U0f0s1;
                        break;
                    case WHICH.U0f0s8:
                        U0f0s8 = reader.U0f0s8;
                        break;
                    case WHICH.U0f0s16:
                        U0f0s16 = reader.U0f0s16;
                        break;
                    case WHICH.U0f0s32:
                        U0f0s32 = reader.U0f0s32;
                        break;
                    case WHICH.U0f0s64:
                        U0f0s64 = reader.U0f0s64;
                        break;
                    case WHICH.U0f0sp:
                        U0f0sp = reader.U0f0sp;
                        break;
                    case WHICH.U0f1s0:
                        which = reader.which;
                        break;
                    case WHICH.U0f1s1:
                        U0f1s1 = reader.U0f1s1;
                        break;
                    case WHICH.U0f1s8:
                        U0f1s8 = reader.U0f1s8;
                        break;
                    case WHICH.U0f1s16:
                        U0f1s16 = reader.U0f1s16;
                        break;
                    case WHICH.U0f1s32:
                        U0f1s32 = reader.U0f1s32;
                        break;
                    case WHICH.U0f1s64:
                        U0f1s64 = reader.U0f1s64;
                        break;
                    case WHICH.U0f1sp:
                        U0f1sp = reader.U0f1sp;
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.U0f0s0:
                            break;
                        case WHICH.U0f0s1:
                            _content = false;
                            break;
                        case WHICH.U0f0s8:
                            _content = 0;
                            break;
                        case WHICH.U0f0s16:
                            _content = 0;
                            break;
                        case WHICH.U0f0s32:
                            _content = 0;
                            break;
                        case WHICH.U0f0s64:
                            _content = 0;
                            break;
                        case WHICH.U0f0sp:
                            _content = null;
                            break;
                        case WHICH.U0f1s0:
                            break;
                        case WHICH.U0f1s1:
                            _content = false;
                            break;
                        case WHICH.U0f1s8:
                            _content = 0;
                            break;
                        case WHICH.U0f1s16:
                            _content = 0;
                            break;
                        case WHICH.U0f1s32:
                            _content = 0;
                            break;
                        case WHICH.U0f1s64:
                            _content = 0;
                            break;
                        case WHICH.U0f1sp:
                            _content = null;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.U0f0s0:
                        break;
                    case WHICH.U0f0s1:
                        writer.U0f0s1 = U0f0s1.Value;
                        break;
                    case WHICH.U0f0s8:
                        writer.U0f0s8 = U0f0s8.Value;
                        break;
                    case WHICH.U0f0s16:
                        writer.U0f0s16 = U0f0s16.Value;
                        break;
                    case WHICH.U0f0s32:
                        writer.U0f0s32 = U0f0s32.Value;
                        break;
                    case WHICH.U0f0s64:
                        writer.U0f0s64 = U0f0s64.Value;
                        break;
                    case WHICH.U0f0sp:
                        writer.U0f0sp = U0f0sp;
                        break;
                    case WHICH.U0f1s0:
                        break;
                    case WHICH.U0f1s1:
                        writer.U0f1s1 = U0f1s1.Value;
                        break;
                    case WHICH.U0f1s8:
                        writer.U0f1s8 = U0f1s8.Value;
                        break;
                    case WHICH.U0f1s16:
                        writer.U0f1s16 = U0f1s16.Value;
                        break;
                    case WHICH.U0f1s32:
                        writer.U0f1s32 = U0f1s32.Value;
                        break;
                    case WHICH.U0f1s64:
                        writer.U0f1s64 = U0f1s64.Value;
                        break;
                    case WHICH.U0f1sp:
                        writer.U0f1sp = U0f1sp;
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public bool? U0f0s1
            {
                get => _which == WHICH.U0f0s1 ? (bool? )_content : null;
                set
                {
                    _which = WHICH.U0f0s1;
                    _content = value;
                }
            }

            public sbyte? U0f0s8
            {
                get => _which == WHICH.U0f0s8 ? (sbyte? )_content : null;
                set
                {
                    _which = WHICH.U0f0s8;
                    _content = value;
                }
            }

            public short? U0f0s16
            {
                get => _which == WHICH.U0f0s16 ? (short? )_content : null;
                set
                {
                    _which = WHICH.U0f0s16;
                    _content = value;
                }
            }

            public int? U0f0s32
            {
                get => _which == WHICH.U0f0s32 ? (int? )_content : null;
                set
                {
                    _which = WHICH.U0f0s32;
                    _content = value;
                }
            }

            public long? U0f0s64
            {
                get => _which == WHICH.U0f0s64 ? (long? )_content : null;
                set
                {
                    _which = WHICH.U0f0s64;
                    _content = value;
                }
            }

            public string U0f0sp
            {
                get => _which == WHICH.U0f0sp ? (string)_content : null;
                set
                {
                    _which = WHICH.U0f0sp;
                    _content = value;
                }
            }

            public bool? U0f1s1
            {
                get => _which == WHICH.U0f1s1 ? (bool? )_content : null;
                set
                {
                    _which = WHICH.U0f1s1;
                    _content = value;
                }
            }

            public sbyte? U0f1s8
            {
                get => _which == WHICH.U0f1s8 ? (sbyte? )_content : null;
                set
                {
                    _which = WHICH.U0f1s8;
                    _content = value;
                }
            }

            public short? U0f1s16
            {
                get => _which == WHICH.U0f1s16 ? (short? )_content : null;
                set
                {
                    _which = WHICH.U0f1s16;
                    _content = value;
                }
            }

            public int? U0f1s32
            {
                get => _which == WHICH.U0f1s32 ? (int? )_content : null;
                set
                {
                    _which = WHICH.U0f1s32;
                    _content = value;
                }
            }

            public long? U0f1s64
            {
                get => _which == WHICH.U0f1s64 ? (long? )_content : null;
                set
                {
                    _which = WHICH.U0f1s64;
                    _content = value;
                }
            }

            public string U0f1sp
            {
                get => _which == WHICH.U0f1sp ? (string)_content : null;
                set
                {
                    _which = WHICH.U0f1sp;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(0U, (ushort)0);
                public bool U0f0s1 => which == WHICH.U0f0s1 ? ctx.ReadDataBool(64UL, false) : default;
                public sbyte U0f0s8 => which == WHICH.U0f0s8 ? ctx.ReadDataSByte(64UL, (sbyte)0) : default;
                public short U0f0s16 => which == WHICH.U0f0s16 ? ctx.ReadDataShort(64UL, (short)0) : default;
                public int U0f0s32 => which == WHICH.U0f0s32 ? ctx.ReadDataInt(64UL, 0) : default;
                public long U0f0s64 => which == WHICH.U0f0s64 ? ctx.ReadDataLong(64UL, 0L) : default;
                public string U0f0sp => which == WHICH.U0f0sp ? ctx.ReadText(0, null) : default;
                public bool U0f1s1 => which == WHICH.U0f1s1 ? ctx.ReadDataBool(64UL, false) : default;
                public sbyte U0f1s8 => which == WHICH.U0f1s8 ? ctx.ReadDataSByte(64UL, (sbyte)0) : default;
                public short U0f1s16 => which == WHICH.U0f1s16 ? ctx.ReadDataShort(64UL, (short)0) : default;
                public int U0f1s32 => which == WHICH.U0f1s32 ? ctx.ReadDataInt(64UL, 0) : default;
                public long U0f1s64 => which == WHICH.U0f1s64 ? ctx.ReadDataLong(64UL, 0L) : default;
                public string U0f1sp => which == WHICH.U0f1sp ? ctx.ReadText(0, null) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                    set => this.WriteData(0U, (ushort)value, (ushort)0);
                }

                public bool U0f0s1
                {
                    get => which == WHICH.U0f0s1 ? this.ReadDataBool(64UL, false) : default;
                    set => this.WriteData(64UL, value, false);
                }

                public sbyte U0f0s8
                {
                    get => which == WHICH.U0f0s8 ? this.ReadDataSByte(64UL, (sbyte)0) : default;
                    set => this.WriteData(64UL, value, (sbyte)0);
                }

                public short U0f0s16
                {
                    get => which == WHICH.U0f0s16 ? this.ReadDataShort(64UL, (short)0) : default;
                    set => this.WriteData(64UL, value, (short)0);
                }

                public int U0f0s32
                {
                    get => which == WHICH.U0f0s32 ? this.ReadDataInt(64UL, 0) : default;
                    set => this.WriteData(64UL, value, 0);
                }

                public long U0f0s64
                {
                    get => which == WHICH.U0f0s64 ? this.ReadDataLong(64UL, 0L) : default;
                    set => this.WriteData(64UL, value, 0L);
                }

                public string U0f0sp
                {
                    get => which == WHICH.U0f0sp ? this.ReadText(0, null) : default;
                    set => this.WriteText(0, value, null);
                }

                public bool U0f1s1
                {
                    get => which == WHICH.U0f1s1 ? this.ReadDataBool(64UL, false) : default;
                    set => this.WriteData(64UL, value, false);
                }

                public sbyte U0f1s8
                {
                    get => which == WHICH.U0f1s8 ? this.ReadDataSByte(64UL, (sbyte)0) : default;
                    set => this.WriteData(64UL, value, (sbyte)0);
                }

                public short U0f1s16
                {
                    get => which == WHICH.U0f1s16 ? this.ReadDataShort(64UL, (short)0) : default;
                    set => this.WriteData(64UL, value, (short)0);
                }

                public int U0f1s32
                {
                    get => which == WHICH.U0f1s32 ? this.ReadDataInt(64UL, 0) : default;
                    set => this.WriteData(64UL, value, 0);
                }

                public long U0f1s64
                {
                    get => which == WHICH.U0f1s64 ? this.ReadDataLong(64UL, 0L) : default;
                    set => this.WriteData(64UL, value, 0L);
                }

                public string U0f1sp
                {
                    get => which == WHICH.U0f1sp ? this.ReadText(0, null) : default;
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0xee0a6b99b7dc7ab2UL)]
        public class union1 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xee0a6b99b7dc7ab2UL;
            public enum WHICH : ushort
            {
                U1f0s0 = 0,
                U1f0s1 = 1,
                U1f1s1 = 2,
                U1f0s8 = 3,
                U1f1s8 = 4,
                U1f0s16 = 5,
                U1f1s16 = 6,
                U1f0s32 = 7,
                U1f1s32 = 8,
                U1f0s64 = 9,
                U1f1s64 = 10,
                U1f0sp = 11,
                U1f1sp = 12,
                U1f2s0 = 13,
                U1f2s1 = 14,
                U1f2s8 = 15,
                U1f2s16 = 16,
                U1f2s32 = 17,
                U1f2s64 = 18,
                U1f2sp = 19,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.U1f0s0:
                        which = reader.which;
                        break;
                    case WHICH.U1f0s1:
                        U1f0s1 = reader.U1f0s1;
                        break;
                    case WHICH.U1f1s1:
                        U1f1s1 = reader.U1f1s1;
                        break;
                    case WHICH.U1f0s8:
                        U1f0s8 = reader.U1f0s8;
                        break;
                    case WHICH.U1f1s8:
                        U1f1s8 = reader.U1f1s8;
                        break;
                    case WHICH.U1f0s16:
                        U1f0s16 = reader.U1f0s16;
                        break;
                    case WHICH.U1f1s16:
                        U1f1s16 = reader.U1f1s16;
                        break;
                    case WHICH.U1f0s32:
                        U1f0s32 = reader.U1f0s32;
                        break;
                    case WHICH.U1f1s32:
                        U1f1s32 = reader.U1f1s32;
                        break;
                    case WHICH.U1f0s64:
                        U1f0s64 = reader.U1f0s64;
                        break;
                    case WHICH.U1f1s64:
                        U1f1s64 = reader.U1f1s64;
                        break;
                    case WHICH.U1f0sp:
                        U1f0sp = reader.U1f0sp;
                        break;
                    case WHICH.U1f1sp:
                        U1f1sp = reader.U1f1sp;
                        break;
                    case WHICH.U1f2s0:
                        which = reader.which;
                        break;
                    case WHICH.U1f2s1:
                        U1f2s1 = reader.U1f2s1;
                        break;
                    case WHICH.U1f2s8:
                        U1f2s8 = reader.U1f2s8;
                        break;
                    case WHICH.U1f2s16:
                        U1f2s16 = reader.U1f2s16;
                        break;
                    case WHICH.U1f2s32:
                        U1f2s32 = reader.U1f2s32;
                        break;
                    case WHICH.U1f2s64:
                        U1f2s64 = reader.U1f2s64;
                        break;
                    case WHICH.U1f2sp:
                        U1f2sp = reader.U1f2sp;
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.U1f0s0:
                            break;
                        case WHICH.U1f0s1:
                            _content = false;
                            break;
                        case WHICH.U1f1s1:
                            _content = false;
                            break;
                        case WHICH.U1f0s8:
                            _content = 0;
                            break;
                        case WHICH.U1f1s8:
                            _content = 0;
                            break;
                        case WHICH.U1f0s16:
                            _content = 0;
                            break;
                        case WHICH.U1f1s16:
                            _content = 0;
                            break;
                        case WHICH.U1f0s32:
                            _content = 0;
                            break;
                        case WHICH.U1f1s32:
                            _content = 0;
                            break;
                        case WHICH.U1f0s64:
                            _content = 0;
                            break;
                        case WHICH.U1f1s64:
                            _content = 0;
                            break;
                        case WHICH.U1f0sp:
                            _content = null;
                            break;
                        case WHICH.U1f1sp:
                            _content = null;
                            break;
                        case WHICH.U1f2s0:
                            break;
                        case WHICH.U1f2s1:
                            _content = false;
                            break;
                        case WHICH.U1f2s8:
                            _content = 0;
                            break;
                        case WHICH.U1f2s16:
                            _content = 0;
                            break;
                        case WHICH.U1f2s32:
                            _content = 0;
                            break;
                        case WHICH.U1f2s64:
                            _content = 0;
                            break;
                        case WHICH.U1f2sp:
                            _content = null;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.U1f0s0:
                        break;
                    case WHICH.U1f0s1:
                        writer.U1f0s1 = U1f0s1.Value;
                        break;
                    case WHICH.U1f1s1:
                        writer.U1f1s1 = U1f1s1.Value;
                        break;
                    case WHICH.U1f0s8:
                        writer.U1f0s8 = U1f0s8.Value;
                        break;
                    case WHICH.U1f1s8:
                        writer.U1f1s8 = U1f1s8.Value;
                        break;
                    case WHICH.U1f0s16:
                        writer.U1f0s16 = U1f0s16.Value;
                        break;
                    case WHICH.U1f1s16:
                        writer.U1f1s16 = U1f1s16.Value;
                        break;
                    case WHICH.U1f0s32:
                        writer.U1f0s32 = U1f0s32.Value;
                        break;
                    case WHICH.U1f1s32:
                        writer.U1f1s32 = U1f1s32.Value;
                        break;
                    case WHICH.U1f0s64:
                        writer.U1f0s64 = U1f0s64.Value;
                        break;
                    case WHICH.U1f1s64:
                        writer.U1f1s64 = U1f1s64.Value;
                        break;
                    case WHICH.U1f0sp:
                        writer.U1f0sp = U1f0sp;
                        break;
                    case WHICH.U1f1sp:
                        writer.U1f1sp = U1f1sp;
                        break;
                    case WHICH.U1f2s0:
                        break;
                    case WHICH.U1f2s1:
                        writer.U1f2s1 = U1f2s1.Value;
                        break;
                    case WHICH.U1f2s8:
                        writer.U1f2s8 = U1f2s8.Value;
                        break;
                    case WHICH.U1f2s16:
                        writer.U1f2s16 = U1f2s16.Value;
                        break;
                    case WHICH.U1f2s32:
                        writer.U1f2s32 = U1f2s32.Value;
                        break;
                    case WHICH.U1f2s64:
                        writer.U1f2s64 = U1f2s64.Value;
                        break;
                    case WHICH.U1f2sp:
                        writer.U1f2sp = U1f2sp;
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public bool? U1f0s1
            {
                get => _which == WHICH.U1f0s1 ? (bool? )_content : null;
                set
                {
                    _which = WHICH.U1f0s1;
                    _content = value;
                }
            }

            public bool? U1f1s1
            {
                get => _which == WHICH.U1f1s1 ? (bool? )_content : null;
                set
                {
                    _which = WHICH.U1f1s1;
                    _content = value;
                }
            }

            public sbyte? U1f0s8
            {
                get => _which == WHICH.U1f0s8 ? (sbyte? )_content : null;
                set
                {
                    _which = WHICH.U1f0s8;
                    _content = value;
                }
            }

            public sbyte? U1f1s8
            {
                get => _which == WHICH.U1f1s8 ? (sbyte? )_content : null;
                set
                {
                    _which = WHICH.U1f1s8;
                    _content = value;
                }
            }

            public short? U1f0s16
            {
                get => _which == WHICH.U1f0s16 ? (short? )_content : null;
                set
                {
                    _which = WHICH.U1f0s16;
                    _content = value;
                }
            }

            public short? U1f1s16
            {
                get => _which == WHICH.U1f1s16 ? (short? )_content : null;
                set
                {
                    _which = WHICH.U1f1s16;
                    _content = value;
                }
            }

            public int? U1f0s32
            {
                get => _which == WHICH.U1f0s32 ? (int? )_content : null;
                set
                {
                    _which = WHICH.U1f0s32;
                    _content = value;
                }
            }

            public int? U1f1s32
            {
                get => _which == WHICH.U1f1s32 ? (int? )_content : null;
                set
                {
                    _which = WHICH.U1f1s32;
                    _content = value;
                }
            }

            public long? U1f0s64
            {
                get => _which == WHICH.U1f0s64 ? (long? )_content : null;
                set
                {
                    _which = WHICH.U1f0s64;
                    _content = value;
                }
            }

            public long? U1f1s64
            {
                get => _which == WHICH.U1f1s64 ? (long? )_content : null;
                set
                {
                    _which = WHICH.U1f1s64;
                    _content = value;
                }
            }

            public string U1f0sp
            {
                get => _which == WHICH.U1f0sp ? (string)_content : null;
                set
                {
                    _which = WHICH.U1f0sp;
                    _content = value;
                }
            }

            public string U1f1sp
            {
                get => _which == WHICH.U1f1sp ? (string)_content : null;
                set
                {
                    _which = WHICH.U1f1sp;
                    _content = value;
                }
            }

            public bool? U1f2s1
            {
                get => _which == WHICH.U1f2s1 ? (bool? )_content : null;
                set
                {
                    _which = WHICH.U1f2s1;
                    _content = value;
                }
            }

            public sbyte? U1f2s8
            {
                get => _which == WHICH.U1f2s8 ? (sbyte? )_content : null;
                set
                {
                    _which = WHICH.U1f2s8;
                    _content = value;
                }
            }

            public short? U1f2s16
            {
                get => _which == WHICH.U1f2s16 ? (short? )_content : null;
                set
                {
                    _which = WHICH.U1f2s16;
                    _content = value;
                }
            }

            public int? U1f2s32
            {
                get => _which == WHICH.U1f2s32 ? (int? )_content : null;
                set
                {
                    _which = WHICH.U1f2s32;
                    _content = value;
                }
            }

            public long? U1f2s64
            {
                get => _which == WHICH.U1f2s64 ? (long? )_content : null;
                set
                {
                    _which = WHICH.U1f2s64;
                    _content = value;
                }
            }

            public string U1f2sp
            {
                get => _which == WHICH.U1f2sp ? (string)_content : null;
                set
                {
                    _which = WHICH.U1f2sp;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(16U, (ushort)0);
                public bool U1f0s1 => which == WHICH.U1f0s1 ? ctx.ReadDataBool(129UL, false) : default;
                public bool U1f1s1 => which == WHICH.U1f1s1 ? ctx.ReadDataBool(129UL, false) : default;
                public sbyte U1f0s8 => which == WHICH.U1f0s8 ? ctx.ReadDataSByte(136UL, (sbyte)0) : default;
                public sbyte U1f1s8 => which == WHICH.U1f1s8 ? ctx.ReadDataSByte(136UL, (sbyte)0) : default;
                public short U1f0s16 => which == WHICH.U1f0s16 ? ctx.ReadDataShort(144UL, (short)0) : default;
                public short U1f1s16 => which == WHICH.U1f1s16 ? ctx.ReadDataShort(144UL, (short)0) : default;
                public int U1f0s32 => which == WHICH.U1f0s32 ? ctx.ReadDataInt(160UL, 0) : default;
                public int U1f1s32 => which == WHICH.U1f1s32 ? ctx.ReadDataInt(160UL, 0) : default;
                public long U1f0s64 => which == WHICH.U1f0s64 ? ctx.ReadDataLong(192UL, 0L) : default;
                public long U1f1s64 => which == WHICH.U1f1s64 ? ctx.ReadDataLong(192UL, 0L) : default;
                public string U1f0sp => which == WHICH.U1f0sp ? ctx.ReadText(1, null) : default;
                public string U1f1sp => which == WHICH.U1f1sp ? ctx.ReadText(1, null) : default;
                public bool U1f2s1 => which == WHICH.U1f2s1 ? ctx.ReadDataBool(129UL, false) : default;
                public sbyte U1f2s8 => which == WHICH.U1f2s8 ? ctx.ReadDataSByte(136UL, (sbyte)0) : default;
                public short U1f2s16 => which == WHICH.U1f2s16 ? ctx.ReadDataShort(144UL, (short)0) : default;
                public int U1f2s32 => which == WHICH.U1f2s32 ? ctx.ReadDataInt(160UL, 0) : default;
                public long U1f2s64 => which == WHICH.U1f2s64 ? ctx.ReadDataLong(192UL, 0L) : default;
                public string U1f2sp => which == WHICH.U1f2sp ? ctx.ReadText(1, null) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(16U, (ushort)0);
                    set => this.WriteData(16U, (ushort)value, (ushort)0);
                }

                public bool U1f0s1
                {
                    get => which == WHICH.U1f0s1 ? this.ReadDataBool(129UL, false) : default;
                    set => this.WriteData(129UL, value, false);
                }

                public bool U1f1s1
                {
                    get => which == WHICH.U1f1s1 ? this.ReadDataBool(129UL, false) : default;
                    set => this.WriteData(129UL, value, false);
                }

                public sbyte U1f0s8
                {
                    get => which == WHICH.U1f0s8 ? this.ReadDataSByte(136UL, (sbyte)0) : default;
                    set => this.WriteData(136UL, value, (sbyte)0);
                }

                public sbyte U1f1s8
                {
                    get => which == WHICH.U1f1s8 ? this.ReadDataSByte(136UL, (sbyte)0) : default;
                    set => this.WriteData(136UL, value, (sbyte)0);
                }

                public short U1f0s16
                {
                    get => which == WHICH.U1f0s16 ? this.ReadDataShort(144UL, (short)0) : default;
                    set => this.WriteData(144UL, value, (short)0);
                }

                public short U1f1s16
                {
                    get => which == WHICH.U1f1s16 ? this.ReadDataShort(144UL, (short)0) : default;
                    set => this.WriteData(144UL, value, (short)0);
                }

                public int U1f0s32
                {
                    get => which == WHICH.U1f0s32 ? this.ReadDataInt(160UL, 0) : default;
                    set => this.WriteData(160UL, value, 0);
                }

                public int U1f1s32
                {
                    get => which == WHICH.U1f1s32 ? this.ReadDataInt(160UL, 0) : default;
                    set => this.WriteData(160UL, value, 0);
                }

                public long U1f0s64
                {
                    get => which == WHICH.U1f0s64 ? this.ReadDataLong(192UL, 0L) : default;
                    set => this.WriteData(192UL, value, 0L);
                }

                public long U1f1s64
                {
                    get => which == WHICH.U1f1s64 ? this.ReadDataLong(192UL, 0L) : default;
                    set => this.WriteData(192UL, value, 0L);
                }

                public string U1f0sp
                {
                    get => which == WHICH.U1f0sp ? this.ReadText(1, null) : default;
                    set => this.WriteText(1, value, null);
                }

                public string U1f1sp
                {
                    get => which == WHICH.U1f1sp ? this.ReadText(1, null) : default;
                    set => this.WriteText(1, value, null);
                }

                public bool U1f2s1
                {
                    get => which == WHICH.U1f2s1 ? this.ReadDataBool(129UL, false) : default;
                    set => this.WriteData(129UL, value, false);
                }

                public sbyte U1f2s8
                {
                    get => which == WHICH.U1f2s8 ? this.ReadDataSByte(136UL, (sbyte)0) : default;
                    set => this.WriteData(136UL, value, (sbyte)0);
                }

                public short U1f2s16
                {
                    get => which == WHICH.U1f2s16 ? this.ReadDataShort(144UL, (short)0) : default;
                    set => this.WriteData(144UL, value, (short)0);
                }

                public int U1f2s32
                {
                    get => which == WHICH.U1f2s32 ? this.ReadDataInt(160UL, 0) : default;
                    set => this.WriteData(160UL, value, 0);
                }

                public long U1f2s64
                {
                    get => which == WHICH.U1f2s64 ? this.ReadDataLong(192UL, 0L) : default;
                    set => this.WriteData(192UL, value, 0L);
                }

                public string U1f2sp
                {
                    get => which == WHICH.U1f2sp ? this.ReadText(1, null) : default;
                    set => this.WriteText(1, value, null);
                }
            }
        }

        [TypeId(0xafc5fd419f0d66d4UL)]
        public class union2 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xafc5fd419f0d66d4UL;
            public enum WHICH : ushort
            {
                U2f0s1 = 0,
                U2f0s8 = 1,
                U2f0s16 = 2,
                U2f0s32 = 3,
                U2f0s64 = 4,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.U2f0s1:
                        U2f0s1 = reader.U2f0s1;
                        break;
                    case WHICH.U2f0s8:
                        U2f0s8 = reader.U2f0s8;
                        break;
                    case WHICH.U2f0s16:
                        U2f0s16 = reader.U2f0s16;
                        break;
                    case WHICH.U2f0s32:
                        U2f0s32 = reader.U2f0s32;
                        break;
                    case WHICH.U2f0s64:
                        U2f0s64 = reader.U2f0s64;
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.U2f0s1:
                            _content = false;
                            break;
                        case WHICH.U2f0s8:
                            _content = 0;
                            break;
                        case WHICH.U2f0s16:
                            _content = 0;
                            break;
                        case WHICH.U2f0s32:
                            _content = 0;
                            break;
                        case WHICH.U2f0s64:
                            _content = 0;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.U2f0s1:
                        writer.U2f0s1 = U2f0s1.Value;
                        break;
                    case WHICH.U2f0s8:
                        writer.U2f0s8 = U2f0s8.Value;
                        break;
                    case WHICH.U2f0s16:
                        writer.U2f0s16 = U2f0s16.Value;
                        break;
                    case WHICH.U2f0s32:
                        writer.U2f0s32 = U2f0s32.Value;
                        break;
                    case WHICH.U2f0s64:
                        writer.U2f0s64 = U2f0s64.Value;
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public bool? U2f0s1
            {
                get => _which == WHICH.U2f0s1 ? (bool? )_content : null;
                set
                {
                    _which = WHICH.U2f0s1;
                    _content = value;
                }
            }

            public sbyte? U2f0s8
            {
                get => _which == WHICH.U2f0s8 ? (sbyte? )_content : null;
                set
                {
                    _which = WHICH.U2f0s8;
                    _content = value;
                }
            }

            public short? U2f0s16
            {
                get => _which == WHICH.U2f0s16 ? (short? )_content : null;
                set
                {
                    _which = WHICH.U2f0s16;
                    _content = value;
                }
            }

            public int? U2f0s32
            {
                get => _which == WHICH.U2f0s32 ? (int? )_content : null;
                set
                {
                    _which = WHICH.U2f0s32;
                    _content = value;
                }
            }

            public long? U2f0s64
            {
                get => _which == WHICH.U2f0s64 ? (long? )_content : null;
                set
                {
                    _which = WHICH.U2f0s64;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(32U, (ushort)0);
                public bool U2f0s1 => which == WHICH.U2f0s1 ? ctx.ReadDataBool(256UL, false) : default;
                public sbyte U2f0s8 => which == WHICH.U2f0s8 ? ctx.ReadDataSByte(264UL, (sbyte)0) : default;
                public short U2f0s16 => which == WHICH.U2f0s16 ? ctx.ReadDataShort(288UL, (short)0) : default;
                public int U2f0s32 => which == WHICH.U2f0s32 ? ctx.ReadDataInt(320UL, 0) : default;
                public long U2f0s64 => which == WHICH.U2f0s64 ? ctx.ReadDataLong(384UL, 0L) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(32U, (ushort)0);
                    set => this.WriteData(32U, (ushort)value, (ushort)0);
                }

                public bool U2f0s1
                {
                    get => which == WHICH.U2f0s1 ? this.ReadDataBool(256UL, false) : default;
                    set => this.WriteData(256UL, value, false);
                }

                public sbyte U2f0s8
                {
                    get => which == WHICH.U2f0s8 ? this.ReadDataSByte(264UL, (sbyte)0) : default;
                    set => this.WriteData(264UL, value, (sbyte)0);
                }

                public short U2f0s16
                {
                    get => which == WHICH.U2f0s16 ? this.ReadDataShort(288UL, (short)0) : default;
                    set => this.WriteData(288UL, value, (short)0);
                }

                public int U2f0s32
                {
                    get => which == WHICH.U2f0s32 ? this.ReadDataInt(320UL, 0) : default;
                    set => this.WriteData(320UL, value, 0);
                }

                public long U2f0s64
                {
                    get => which == WHICH.U2f0s64 ? this.ReadDataLong(384UL, 0L) : default;
                    set => this.WriteData(384UL, value, 0L);
                }
            }
        }

        [TypeId(0xa2fb022ec7f30053UL)]
        public class union3 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xa2fb022ec7f30053UL;
            public enum WHICH : ushort
            {
                U3f0s1 = 0,
                U3f0s8 = 1,
                U3f0s16 = 2,
                U3f0s32 = 3,
                U3f0s64 = 4,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.U3f0s1:
                        U3f0s1 = reader.U3f0s1;
                        break;
                    case WHICH.U3f0s8:
                        U3f0s8 = reader.U3f0s8;
                        break;
                    case WHICH.U3f0s16:
                        U3f0s16 = reader.U3f0s16;
                        break;
                    case WHICH.U3f0s32:
                        U3f0s32 = reader.U3f0s32;
                        break;
                    case WHICH.U3f0s64:
                        U3f0s64 = reader.U3f0s64;
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.U3f0s1:
                            _content = false;
                            break;
                        case WHICH.U3f0s8:
                            _content = 0;
                            break;
                        case WHICH.U3f0s16:
                            _content = 0;
                            break;
                        case WHICH.U3f0s32:
                            _content = 0;
                            break;
                        case WHICH.U3f0s64:
                            _content = 0;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.U3f0s1:
                        writer.U3f0s1 = U3f0s1.Value;
                        break;
                    case WHICH.U3f0s8:
                        writer.U3f0s8 = U3f0s8.Value;
                        break;
                    case WHICH.U3f0s16:
                        writer.U3f0s16 = U3f0s16.Value;
                        break;
                    case WHICH.U3f0s32:
                        writer.U3f0s32 = U3f0s32.Value;
                        break;
                    case WHICH.U3f0s64:
                        writer.U3f0s64 = U3f0s64.Value;
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public bool? U3f0s1
            {
                get => _which == WHICH.U3f0s1 ? (bool? )_content : null;
                set
                {
                    _which = WHICH.U3f0s1;
                    _content = value;
                }
            }

            public sbyte? U3f0s8
            {
                get => _which == WHICH.U3f0s8 ? (sbyte? )_content : null;
                set
                {
                    _which = WHICH.U3f0s8;
                    _content = value;
                }
            }

            public short? U3f0s16
            {
                get => _which == WHICH.U3f0s16 ? (short? )_content : null;
                set
                {
                    _which = WHICH.U3f0s16;
                    _content = value;
                }
            }

            public int? U3f0s32
            {
                get => _which == WHICH.U3f0s32 ? (int? )_content : null;
                set
                {
                    _which = WHICH.U3f0s32;
                    _content = value;
                }
            }

            public long? U3f0s64
            {
                get => _which == WHICH.U3f0s64 ? (long? )_content : null;
                set
                {
                    _which = WHICH.U3f0s64;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(48U, (ushort)0);
                public bool U3f0s1 => which == WHICH.U3f0s1 ? ctx.ReadDataBool(257UL, false) : default;
                public sbyte U3f0s8 => which == WHICH.U3f0s8 ? ctx.ReadDataSByte(272UL, (sbyte)0) : default;
                public short U3f0s16 => which == WHICH.U3f0s16 ? ctx.ReadDataShort(304UL, (short)0) : default;
                public int U3f0s32 => which == WHICH.U3f0s32 ? ctx.ReadDataInt(352UL, 0) : default;
                public long U3f0s64 => which == WHICH.U3f0s64 ? ctx.ReadDataLong(448UL, 0L) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(48U, (ushort)0);
                    set => this.WriteData(48U, (ushort)value, (ushort)0);
                }

                public bool U3f0s1
                {
                    get => which == WHICH.U3f0s1 ? this.ReadDataBool(257UL, false) : default;
                    set => this.WriteData(257UL, value, false);
                }

                public sbyte U3f0s8
                {
                    get => which == WHICH.U3f0s8 ? this.ReadDataSByte(272UL, (sbyte)0) : default;
                    set => this.WriteData(272UL, value, (sbyte)0);
                }

                public short U3f0s16
                {
                    get => which == WHICH.U3f0s16 ? this.ReadDataShort(304UL, (short)0) : default;
                    set => this.WriteData(304UL, value, (short)0);
                }

                public int U3f0s32
                {
                    get => which == WHICH.U3f0s32 ? this.ReadDataInt(352UL, 0) : default;
                    set => this.WriteData(352UL, value, 0);
                }

                public long U3f0s64
                {
                    get => which == WHICH.U3f0s64 ? this.ReadDataLong(448UL, 0L) : default;
                    set => this.WriteData(448UL, value, 0L);
                }
            }
        }
    }

    [TypeId(0x9e2e784c915329b6UL)]
    public class TestUnnamedUnion : ICapnpSerializable
    {
        public const UInt64 typeId = 0x9e2e784c915329b6UL;
        public enum WHICH : ushort
        {
            Foo = 0,
            Bar = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.Foo:
                    Foo = reader.Foo;
                    break;
                case WHICH.Bar:
                    Bar = reader.Bar;
                    break;
            }

            Before = reader.Before;
            Middle = reader.Middle;
            After = reader.After;
            applyDefaults();
        }

        private WHICH _which = WHICH.undefined;
        private object _content;
        public WHICH which
        {
            get => _which;
            set
            {
                if (value == _which)
                    return;
                _which = value;
                switch (value)
                {
                    case WHICH.Foo:
                        _content = 0;
                        break;
                    case WHICH.Bar:
                        _content = 0;
                        break;
                }
            }
        }

        public void serialize(WRITER writer)
        {
            writer.which = which;
            switch (which)
            {
                case WHICH.Foo:
                    writer.Foo = Foo.Value;
                    break;
                case WHICH.Bar:
                    writer.Bar = Bar.Value;
                    break;
            }

            writer.Before = Before;
            writer.Middle = Middle;
            writer.After = After;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Before
        {
            get;
            set;
        }

        public ushort? Foo
        {
            get => _which == WHICH.Foo ? (ushort? )_content : null;
            set
            {
                _which = WHICH.Foo;
                _content = value;
            }
        }

        public ushort Middle
        {
            get;
            set;
        }

        public uint? Bar
        {
            get => _which == WHICH.Bar ? (uint? )_content : null;
            set
            {
                _which = WHICH.Bar;
                _content = value;
            }
        }

        public string After
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public WHICH which => (WHICH)ctx.ReadDataUShort(32U, (ushort)0);
            public string Before => ctx.ReadText(0, null);
            public ushort Foo => which == WHICH.Foo ? ctx.ReadDataUShort(0UL, (ushort)0) : default;
            public ushort Middle => ctx.ReadDataUShort(16UL, (ushort)0);
            public uint Bar => which == WHICH.Bar ? ctx.ReadDataUInt(64UL, 0U) : default;
            public string After => ctx.ReadText(1, null);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 2);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(32U, (ushort)0);
                set => this.WriteData(32U, (ushort)value, (ushort)0);
            }

            public string Before
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public ushort Foo
            {
                get => which == WHICH.Foo ? this.ReadDataUShort(0UL, (ushort)0) : default;
                set => this.WriteData(0UL, value, (ushort)0);
            }

            public ushort Middle
            {
                get => this.ReadDataUShort(16UL, (ushort)0);
                set => this.WriteData(16UL, value, (ushort)0);
            }

            public uint Bar
            {
                get => which == WHICH.Bar ? this.ReadDataUInt(64UL, 0U) : default;
                set => this.WriteData(64UL, value, 0U);
            }

            public string After
            {
                get => this.ReadText(1, null);
                set => this.WriteText(1, value, null);
            }
        }
    }

    [TypeId(0x89a9494f1b900f22UL)]
    public class TestUnionInUnion : ICapnpSerializable
    {
        public const UInt64 typeId = 0x89a9494f1b900f22UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Outer = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUnionInUnion.outer>(reader.Outer);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Outer?.serialize(writer.Outer);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestUnionInUnion.outer Outer
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public outer.READER Outer => new outer.READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 0);
            }

            public outer.WRITER Outer
            {
                get => Rewrap<outer.WRITER>();
            }
        }

        [TypeId(0xd005f6c63707670cUL)]
        public class outer : ICapnpSerializable
        {
            public const UInt64 typeId = 0xd005f6c63707670cUL;
            public enum WHICH : ushort
            {
                Inner = 0,
                Baz = 1,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Inner:
                        Inner = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUnionInUnion.outer.inner>(reader.Inner);
                        break;
                    case WHICH.Baz:
                        Baz = reader.Baz;
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.Inner:
                            _content = null;
                            break;
                        case WHICH.Baz:
                            _content = 0;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.Inner:
                        Inner?.serialize(writer.Inner);
                        break;
                    case WHICH.Baz:
                        writer.Baz = Baz.Value;
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestUnionInUnion.outer.inner Inner
            {
                get => _which == WHICH.Inner ? (Capnproto_test.Capnp.Test.TestUnionInUnion.outer.inner)_content : null;
                set
                {
                    _which = WHICH.Inner;
                    _content = value;
                }
            }

            public int? Baz
            {
                get => _which == WHICH.Baz ? (int? )_content : null;
                set
                {
                    _which = WHICH.Baz;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(64U, (ushort)0);
                public inner.READER Inner => which == WHICH.Inner ? new inner.READER(ctx) : default;
                public int Baz => which == WHICH.Baz ? ctx.ReadDataInt(0UL, 0) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(64U, (ushort)0);
                    set => this.WriteData(64U, (ushort)value, (ushort)0);
                }

                public inner.WRITER Inner
                {
                    get => which == WHICH.Inner ? Rewrap<inner.WRITER>() : default;
                }

                public int Baz
                {
                    get => which == WHICH.Baz ? this.ReadDataInt(0UL, 0) : default;
                    set => this.WriteData(0UL, value, 0);
                }
            }

            [TypeId(0xff9ce111c6f8e5dbUL)]
            public class inner : ICapnpSerializable
            {
                public const UInt64 typeId = 0xff9ce111c6f8e5dbUL;
                public enum WHICH : ushort
                {
                    Foo = 0,
                    Bar = 1,
                    undefined = 65535
                }

                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    switch (reader.which)
                    {
                        case WHICH.Foo:
                            Foo = reader.Foo;
                            break;
                        case WHICH.Bar:
                            Bar = reader.Bar;
                            break;
                    }

                    applyDefaults();
                }

                private WHICH _which = WHICH.undefined;
                private object _content;
                public WHICH which
                {
                    get => _which;
                    set
                    {
                        if (value == _which)
                            return;
                        _which = value;
                        switch (value)
                        {
                            case WHICH.Foo:
                                _content = 0;
                                break;
                            case WHICH.Bar:
                                _content = 0;
                                break;
                        }
                    }
                }

                public void serialize(WRITER writer)
                {
                    writer.which = which;
                    switch (which)
                    {
                        case WHICH.Foo:
                            writer.Foo = Foo.Value;
                            break;
                        case WHICH.Bar:
                            writer.Bar = Bar.Value;
                            break;
                    }
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public int? Foo
                {
                    get => _which == WHICH.Foo ? (int? )_content : null;
                    set
                    {
                        _which = WHICH.Foo;
                        _content = value;
                    }
                }

                public int? Bar
                {
                    get => _which == WHICH.Bar ? (int? )_content : null;
                    set
                    {
                        _which = WHICH.Bar;
                        _content = value;
                    }
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                    public WHICH which => (WHICH)ctx.ReadDataUShort(32U, (ushort)0);
                    public int Foo => which == WHICH.Foo ? ctx.ReadDataInt(0UL, 0) : default;
                    public int Bar => which == WHICH.Bar ? ctx.ReadDataInt(0UL, 0) : default;
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public WHICH which
                    {
                        get => (WHICH)this.ReadDataUShort(32U, (ushort)0);
                        set => this.WriteData(32U, (ushort)value, (ushort)0);
                    }

                    public int Foo
                    {
                        get => which == WHICH.Foo ? this.ReadDataInt(0UL, 0) : default;
                        set => this.WriteData(0UL, value, 0);
                    }

                    public int Bar
                    {
                        get => which == WHICH.Bar ? this.ReadDataInt(0UL, 0) : default;
                        set => this.WriteData(0UL, value, 0);
                    }
                }
            }
        }
    }

    [TypeId(0xdc841556134c3103UL)]
    public class TestGroups : ICapnpSerializable
    {
        public const UInt64 typeId = 0xdc841556134c3103UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Groups = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGroups.groups>(reader.Groups);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Groups?.serialize(writer.Groups);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestGroups.groups Groups
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public groups.READER Groups => new groups.READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 2);
            }

            public groups.WRITER Groups
            {
                get => Rewrap<groups.WRITER>();
            }
        }

        [TypeId(0xe22ae74ff9113268UL)]
        public class groups : ICapnpSerializable
        {
            public const UInt64 typeId = 0xe22ae74ff9113268UL;
            public enum WHICH : ushort
            {
                Foo = 0,
                Baz = 1,
                Bar = 2,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Foo:
                        Foo = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGroups.groups.foo>(reader.Foo);
                        break;
                    case WHICH.Baz:
                        Baz = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGroups.groups.baz>(reader.Baz);
                        break;
                    case WHICH.Bar:
                        Bar = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGroups.groups.bar>(reader.Bar);
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.Foo:
                            _content = null;
                            break;
                        case WHICH.Baz:
                            _content = null;
                            break;
                        case WHICH.Bar:
                            _content = null;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.Foo:
                        Foo?.serialize(writer.Foo);
                        break;
                    case WHICH.Baz:
                        Baz?.serialize(writer.Baz);
                        break;
                    case WHICH.Bar:
                        Bar?.serialize(writer.Bar);
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestGroups.groups.foo Foo
            {
                get => _which == WHICH.Foo ? (Capnproto_test.Capnp.Test.TestGroups.groups.foo)_content : null;
                set
                {
                    _which = WHICH.Foo;
                    _content = value;
                }
            }

            public Capnproto_test.Capnp.Test.TestGroups.groups.baz Baz
            {
                get => _which == WHICH.Baz ? (Capnproto_test.Capnp.Test.TestGroups.groups.baz)_content : null;
                set
                {
                    _which = WHICH.Baz;
                    _content = value;
                }
            }

            public Capnproto_test.Capnp.Test.TestGroups.groups.bar Bar
            {
                get => _which == WHICH.Bar ? (Capnproto_test.Capnp.Test.TestGroups.groups.bar)_content : null;
                set
                {
                    _which = WHICH.Bar;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(32U, (ushort)0);
                public foo.READER Foo => which == WHICH.Foo ? new foo.READER(ctx) : default;
                public baz.READER Baz => which == WHICH.Baz ? new baz.READER(ctx) : default;
                public bar.READER Bar => which == WHICH.Bar ? new bar.READER(ctx) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(32U, (ushort)0);
                    set => this.WriteData(32U, (ushort)value, (ushort)0);
                }

                public foo.WRITER Foo
                {
                    get => which == WHICH.Foo ? Rewrap<foo.WRITER>() : default;
                }

                public baz.WRITER Baz
                {
                    get => which == WHICH.Baz ? Rewrap<baz.WRITER>() : default;
                }

                public bar.WRITER Bar
                {
                    get => which == WHICH.Bar ? Rewrap<bar.WRITER>() : default;
                }
            }

            [TypeId(0xf5fcba89c0c1196fUL)]
            public class foo : ICapnpSerializable
            {
                public const UInt64 typeId = 0xf5fcba89c0c1196fUL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Corge = reader.Corge;
                    Grault = reader.Grault;
                    Garply = reader.Garply;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Corge = Corge;
                    writer.Grault = Grault;
                    writer.Garply = Garply;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public int Corge
                {
                    get;
                    set;
                }

                public long Grault
                {
                    get;
                    set;
                }

                public string Garply
                {
                    get;
                    set;
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                    public int Corge => ctx.ReadDataInt(0UL, 0);
                    public long Grault => ctx.ReadDataLong(64UL, 0L);
                    public string Garply => ctx.ReadText(0, null);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public int Corge
                    {
                        get => this.ReadDataInt(0UL, 0);
                        set => this.WriteData(0UL, value, 0);
                    }

                    public long Grault
                    {
                        get => this.ReadDataLong(64UL, 0L);
                        set => this.WriteData(64UL, value, 0L);
                    }

                    public string Garply
                    {
                        get => this.ReadText(0, null);
                        set => this.WriteText(0, value, null);
                    }
                }
            }

            [TypeId(0xf0fa30304066a4b3UL)]
            public class baz : ICapnpSerializable
            {
                public const UInt64 typeId = 0xf0fa30304066a4b3UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Corge = reader.Corge;
                    Grault = reader.Grault;
                    Garply = reader.Garply;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Corge = Corge;
                    writer.Grault = Grault;
                    writer.Garply = Garply;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public int Corge
                {
                    get;
                    set;
                }

                public string Grault
                {
                    get;
                    set;
                }

                public string Garply
                {
                    get;
                    set;
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                    public int Corge => ctx.ReadDataInt(0UL, 0);
                    public string Grault => ctx.ReadText(0, null);
                    public string Garply => ctx.ReadText(1, null);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public int Corge
                    {
                        get => this.ReadDataInt(0UL, 0);
                        set => this.WriteData(0UL, value, 0);
                    }

                    public string Grault
                    {
                        get => this.ReadText(0, null);
                        set => this.WriteText(0, value, null);
                    }

                    public string Garply
                    {
                        get => this.ReadText(1, null);
                        set => this.WriteText(1, value, null);
                    }
                }
            }

            [TypeId(0xb727c0d0091a001dUL)]
            public class bar : ICapnpSerializable
            {
                public const UInt64 typeId = 0xb727c0d0091a001dUL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Corge = reader.Corge;
                    Grault = reader.Grault;
                    Garply = reader.Garply;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Corge = Corge;
                    writer.Grault = Grault;
                    writer.Garply = Garply;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public int Corge
                {
                    get;
                    set;
                }

                public string Grault
                {
                    get;
                    set;
                }

                public long Garply
                {
                    get;
                    set;
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                    public int Corge => ctx.ReadDataInt(0UL, 0);
                    public string Grault => ctx.ReadText(0, null);
                    public long Garply => ctx.ReadDataLong(64UL, 0L);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public int Corge
                    {
                        get => this.ReadDataInt(0UL, 0);
                        set => this.WriteData(0UL, value, 0);
                    }

                    public string Grault
                    {
                        get => this.ReadText(0, null);
                        set => this.WriteText(0, value, null);
                    }

                    public long Garply
                    {
                        get => this.ReadDataLong(64UL, 0L);
                        set => this.WriteData(64UL, value, 0L);
                    }
                }
            }
        }
    }

    [TypeId(0xf77ed6f7454eec40UL)]
    public class TestInterleavedGroups : ICapnpSerializable
    {
        public const UInt64 typeId = 0xf77ed6f7454eec40UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Group1 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterleavedGroups.group1>(reader.Group1);
            Group2 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterleavedGroups.group2>(reader.Group2);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Group1?.serialize(writer.Group1);
            Group2?.serialize(writer.Group2);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestInterleavedGroups.group1 Group1
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestInterleavedGroups.group2 Group2
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public group1.READER Group1 => new group1.READER(ctx);
            public group2.READER Group2 => new group2.READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(6, 6);
            }

            public group1.WRITER Group1
            {
                get => Rewrap<group1.WRITER>();
            }

            public group2.WRITER Group2
            {
                get => Rewrap<group2.WRITER>();
            }
        }

        [TypeId(0xc7485a3516c7d3c8UL)]
        public class group1 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc7485a3516c7d3c8UL;
            public enum WHICH : ushort
            {
                Qux = 0,
                Corge = 1,
                Fred = 2,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Qux:
                        Qux = reader.Qux;
                        break;
                    case WHICH.Corge:
                        Corge = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterleavedGroups.group1.corge>(reader.Corge);
                        break;
                    case WHICH.Fred:
                        Fred = reader.Fred;
                        break;
                }

                Foo = reader.Foo;
                Bar = reader.Bar;
                Waldo = reader.Waldo;
                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.Qux:
                            _content = 0;
                            break;
                        case WHICH.Corge:
                            _content = null;
                            break;
                        case WHICH.Fred:
                            _content = null;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.Qux:
                        writer.Qux = Qux.Value;
                        break;
                    case WHICH.Corge:
                        Corge?.serialize(writer.Corge);
                        break;
                    case WHICH.Fred:
                        writer.Fred = Fred;
                        break;
                }

                writer.Foo = Foo;
                writer.Bar = Bar;
                writer.Waldo = Waldo;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint Foo
            {
                get;
                set;
            }

            public ulong Bar
            {
                get;
                set;
            }

            public ushort? Qux
            {
                get => _which == WHICH.Qux ? (ushort? )_content : null;
                set
                {
                    _which = WHICH.Qux;
                    _content = value;
                }
            }

            public Capnproto_test.Capnp.Test.TestInterleavedGroups.group1.corge Corge
            {
                get => _which == WHICH.Corge ? (Capnproto_test.Capnp.Test.TestInterleavedGroups.group1.corge)_content : null;
                set
                {
                    _which = WHICH.Corge;
                    _content = value;
                }
            }

            public string Waldo
            {
                get;
                set;
            }

            public string Fred
            {
                get => _which == WHICH.Fred ? (string)_content : null;
                set
                {
                    _which = WHICH.Fred;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(224U, (ushort)0);
                public uint Foo => ctx.ReadDataUInt(0UL, 0U);
                public ulong Bar => ctx.ReadDataULong(64UL, 0UL);
                public ushort Qux => which == WHICH.Qux ? ctx.ReadDataUShort(192UL, (ushort)0) : default;
                public corge.READER Corge => which == WHICH.Corge ? new corge.READER(ctx) : default;
                public string Waldo => ctx.ReadText(0, null);
                public string Fred => which == WHICH.Fred ? ctx.ReadText(2, null) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(224U, (ushort)0);
                    set => this.WriteData(224U, (ushort)value, (ushort)0);
                }

                public uint Foo
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }

                public ulong Bar
                {
                    get => this.ReadDataULong(64UL, 0UL);
                    set => this.WriteData(64UL, value, 0UL);
                }

                public ushort Qux
                {
                    get => which == WHICH.Qux ? this.ReadDataUShort(192UL, (ushort)0) : default;
                    set => this.WriteData(192UL, value, (ushort)0);
                }

                public corge.WRITER Corge
                {
                    get => which == WHICH.Corge ? Rewrap<corge.WRITER>() : default;
                }

                public string Waldo
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public string Fred
                {
                    get => which == WHICH.Fred ? this.ReadText(2, null) : default;
                    set => this.WriteText(2, value, null);
                }
            }

            [TypeId(0xdb0afd413f4a313aUL)]
            public class corge : ICapnpSerializable
            {
                public const UInt64 typeId = 0xdb0afd413f4a313aUL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Grault = reader.Grault;
                    Garply = reader.Garply;
                    Plugh = reader.Plugh;
                    Xyzzy = reader.Xyzzy;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Grault = Grault;
                    writer.Garply = Garply;
                    writer.Plugh = Plugh;
                    writer.Xyzzy = Xyzzy;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public ulong Grault
                {
                    get;
                    set;
                }

                public ushort Garply
                {
                    get;
                    set;
                }

                public string Plugh
                {
                    get;
                    set;
                }

                public string Xyzzy
                {
                    get;
                    set;
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                    public ulong Grault => ctx.ReadDataULong(256UL, 0UL);
                    public ushort Garply => ctx.ReadDataUShort(192UL, (ushort)0);
                    public string Plugh => ctx.ReadText(2, null);
                    public string Xyzzy => ctx.ReadText(4, null);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public ulong Grault
                    {
                        get => this.ReadDataULong(256UL, 0UL);
                        set => this.WriteData(256UL, value, 0UL);
                    }

                    public ushort Garply
                    {
                        get => this.ReadDataUShort(192UL, (ushort)0);
                        set => this.WriteData(192UL, value, (ushort)0);
                    }

                    public string Plugh
                    {
                        get => this.ReadText(2, null);
                        set => this.WriteText(2, value, null);
                    }

                    public string Xyzzy
                    {
                        get => this.ReadText(4, null);
                        set => this.WriteText(4, value, null);
                    }
                }
            }
        }

        [TypeId(0xcc85a335569990e9UL)]
        public class group2 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xcc85a335569990e9UL;
            public enum WHICH : ushort
            {
                Qux = 0,
                Corge = 1,
                Fred = 2,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Qux:
                        Qux = reader.Qux;
                        break;
                    case WHICH.Corge:
                        Corge = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterleavedGroups.group2.corge>(reader.Corge);
                        break;
                    case WHICH.Fred:
                        Fred = reader.Fred;
                        break;
                }

                Foo = reader.Foo;
                Bar = reader.Bar;
                Waldo = reader.Waldo;
                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.Qux:
                            _content = 0;
                            break;
                        case WHICH.Corge:
                            _content = null;
                            break;
                        case WHICH.Fred:
                            _content = null;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.Qux:
                        writer.Qux = Qux.Value;
                        break;
                    case WHICH.Corge:
                        Corge?.serialize(writer.Corge);
                        break;
                    case WHICH.Fred:
                        writer.Fred = Fred;
                        break;
                }

                writer.Foo = Foo;
                writer.Bar = Bar;
                writer.Waldo = Waldo;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint Foo
            {
                get;
                set;
            }

            public ulong Bar
            {
                get;
                set;
            }

            public ushort? Qux
            {
                get => _which == WHICH.Qux ? (ushort? )_content : null;
                set
                {
                    _which = WHICH.Qux;
                    _content = value;
                }
            }

            public Capnproto_test.Capnp.Test.TestInterleavedGroups.group2.corge Corge
            {
                get => _which == WHICH.Corge ? (Capnproto_test.Capnp.Test.TestInterleavedGroups.group2.corge)_content : null;
                set
                {
                    _which = WHICH.Corge;
                    _content = value;
                }
            }

            public string Waldo
            {
                get;
                set;
            }

            public string Fred
            {
                get => _which == WHICH.Fred ? (string)_content : null;
                set
                {
                    _which = WHICH.Fred;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(240U, (ushort)0);
                public uint Foo => ctx.ReadDataUInt(32UL, 0U);
                public ulong Bar => ctx.ReadDataULong(128UL, 0UL);
                public ushort Qux => which == WHICH.Qux ? ctx.ReadDataUShort(208UL, (ushort)0) : default;
                public corge.READER Corge => which == WHICH.Corge ? new corge.READER(ctx) : default;
                public string Waldo => ctx.ReadText(1, null);
                public string Fred => which == WHICH.Fred ? ctx.ReadText(3, null) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(240U, (ushort)0);
                    set => this.WriteData(240U, (ushort)value, (ushort)0);
                }

                public uint Foo
                {
                    get => this.ReadDataUInt(32UL, 0U);
                    set => this.WriteData(32UL, value, 0U);
                }

                public ulong Bar
                {
                    get => this.ReadDataULong(128UL, 0UL);
                    set => this.WriteData(128UL, value, 0UL);
                }

                public ushort Qux
                {
                    get => which == WHICH.Qux ? this.ReadDataUShort(208UL, (ushort)0) : default;
                    set => this.WriteData(208UL, value, (ushort)0);
                }

                public corge.WRITER Corge
                {
                    get => which == WHICH.Corge ? Rewrap<corge.WRITER>() : default;
                }

                public string Waldo
                {
                    get => this.ReadText(1, null);
                    set => this.WriteText(1, value, null);
                }

                public string Fred
                {
                    get => which == WHICH.Fred ? this.ReadText(3, null) : default;
                    set => this.WriteText(3, value, null);
                }
            }

            [TypeId(0xa017f0366827ee37UL)]
            public class corge : ICapnpSerializable
            {
                public const UInt64 typeId = 0xa017f0366827ee37UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Grault = reader.Grault;
                    Garply = reader.Garply;
                    Plugh = reader.Plugh;
                    Xyzzy = reader.Xyzzy;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Grault = Grault;
                    writer.Garply = Garply;
                    writer.Plugh = Plugh;
                    writer.Xyzzy = Xyzzy;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public ulong Grault
                {
                    get;
                    set;
                }

                public ushort Garply
                {
                    get;
                    set;
                }

                public string Plugh
                {
                    get;
                    set;
                }

                public string Xyzzy
                {
                    get;
                    set;
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                    public ulong Grault => ctx.ReadDataULong(320UL, 0UL);
                    public ushort Garply => ctx.ReadDataUShort(208UL, (ushort)0);
                    public string Plugh => ctx.ReadText(3, null);
                    public string Xyzzy => ctx.ReadText(5, null);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public ulong Grault
                    {
                        get => this.ReadDataULong(320UL, 0UL);
                        set => this.WriteData(320UL, value, 0UL);
                    }

                    public ushort Garply
                    {
                        get => this.ReadDataUShort(208UL, (ushort)0);
                        set => this.WriteData(208UL, value, (ushort)0);
                    }

                    public string Plugh
                    {
                        get => this.ReadText(3, null);
                        set => this.WriteText(3, value, null);
                    }

                    public string Xyzzy
                    {
                        get => this.ReadText(5, null);
                        set => this.WriteText(5, value, null);
                    }
                }
            }
        }
    }

    [TypeId(0x94f7e0b103b4b718UL)]
    public class TestUnionDefaults : ICapnpSerializable
    {
        public const UInt64 typeId = 0x94f7e0b103b4b718UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            S16s8s64s8Set = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUnion>(reader.S16s8s64s8Set);
            S0sps1s32Set = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUnion>(reader.S0sps1s32Set);
            Unnamed1 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUnnamedUnion>(reader.Unnamed1);
            Unnamed2 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUnnamedUnion>(reader.Unnamed2);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            S16s8s64s8Set?.serialize(writer.S16s8s64s8Set);
            S0sps1s32Set?.serialize(writer.S0sps1s32Set);
            Unnamed1?.serialize(writer.Unnamed1);
            Unnamed2?.serialize(writer.Unnamed2);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
            S16s8s64s8Set = S16s8s64s8Set ?? new Capnproto_test.Capnp.Test.TestUnion()
            {Union0 = new Capnproto_test.Capnp.Test.TestUnion.union0()
            {}, Union1 = new Capnproto_test.Capnp.Test.TestUnion.union1()
            {}, Union2 = new Capnproto_test.Capnp.Test.TestUnion.union2()
            {}, Union3 = new Capnproto_test.Capnp.Test.TestUnion.union3()
            {}, Bit0 = false, Bit2 = false, Bit3 = false, Bit4 = false, Bit5 = false, Bit6 = false, Bit7 = false, Byte0 = 0};
            S0sps1s32Set = S0sps1s32Set ?? new Capnproto_test.Capnp.Test.TestUnion()
            {Union0 = new Capnproto_test.Capnp.Test.TestUnion.union0()
            {}, Union1 = new Capnproto_test.Capnp.Test.TestUnion.union1()
            {}, Union2 = new Capnproto_test.Capnp.Test.TestUnion.union2()
            {}, Union3 = new Capnproto_test.Capnp.Test.TestUnion.union3()
            {}, Bit0 = false, Bit2 = false, Bit3 = false, Bit4 = false, Bit5 = false, Bit6 = false, Bit7 = false, Byte0 = 0};
            Unnamed1 = Unnamed1 ?? new Capnproto_test.Capnp.Test.TestUnnamedUnion()
            {Before = null, Middle = 0, After = null};
            Unnamed2 = Unnamed2 ?? new Capnproto_test.Capnp.Test.TestUnnamedUnion()
            {Before = "foo", Middle = 0, After = "bar"};
        }

        public Capnproto_test.Capnp.Test.TestUnion S16s8s64s8Set
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestUnion S0sps1s32Set
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestUnnamedUnion Unnamed1
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestUnnamedUnion Unnamed2
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public Capnproto_test.Capnp.Test.TestUnion.READER S16s8s64s8Set => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestUnion.READER.create);
            public Capnproto_test.Capnp.Test.TestUnion.READER S0sps1s32Set => ctx.ReadStruct(1, Capnproto_test.Capnp.Test.TestUnion.READER.create);
            public Capnproto_test.Capnp.Test.TestUnnamedUnion.READER Unnamed1 => ctx.ReadStruct(2, Capnproto_test.Capnp.Test.TestUnnamedUnion.READER.create);
            public Capnproto_test.Capnp.Test.TestUnnamedUnion.READER Unnamed2 => ctx.ReadStruct(3, Capnproto_test.Capnp.Test.TestUnnamedUnion.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 4);
            }

            public Capnproto_test.Capnp.Test.TestUnion.WRITER S16s8s64s8Set
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestUnion.WRITER>(0);
                set => Link(0, value);
            }

            public Capnproto_test.Capnp.Test.TestUnion.WRITER S0sps1s32Set
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestUnion.WRITER>(1);
                set => Link(1, value);
            }

            public Capnproto_test.Capnp.Test.TestUnnamedUnion.WRITER Unnamed1
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestUnnamedUnion.WRITER>(2);
                set => Link(2, value);
            }

            public Capnproto_test.Capnp.Test.TestUnnamedUnion.WRITER Unnamed2
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestUnnamedUnion.WRITER>(3);
                set => Link(3, value);
            }
        }
    }

    [TypeId(0xd9f2b5941a343bcdUL)]
    public class TestNestedTypes : ICapnpSerializable
    {
        public const UInt64 typeId = 0xd9f2b5941a343bcdUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            TheNestedStruct = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct>(reader.TheNestedStruct);
            OuterNestedEnum = reader.OuterNestedEnum;
            InnerNestedEnum = reader.InnerNestedEnum;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            TheNestedStruct?.serialize(writer.TheNestedStruct);
            writer.OuterNestedEnum = OuterNestedEnum;
            writer.InnerNestedEnum = InnerNestedEnum;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct TheNestedStruct
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum OuterNestedEnum
        {
            get;
            set;
        }

        = Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum.bar;
        public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum InnerNestedEnum
        {
            get;
            set;
        }

        = Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum.quux;
        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.READER TheNestedStruct => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.READER.create);
            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum OuterNestedEnum => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum)ctx.ReadDataUShort(0UL, (ushort)1);
            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum InnerNestedEnum => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum)ctx.ReadDataUShort(16UL, (ushort)2);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.WRITER TheNestedStruct
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.WRITER>(0);
                set => Link(0, value);
            }

            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum OuterNestedEnum
            {
                get => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum)this.ReadDataUShort(0UL, (ushort)1);
                set => this.WriteData(0UL, (ushort)value, (ushort)1);
            }

            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum InnerNestedEnum
            {
                get => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum)this.ReadDataUShort(16UL, (ushort)2);
                set => this.WriteData(16UL, (ushort)value, (ushort)2);
            }
        }

        [TypeId(0xb651d2fba42056d4UL)]
        public enum NestedEnum : ushort
        {
            foo,
            bar
        }

        [TypeId(0x82cd03a53b29d76bUL)]
        public class NestedStruct : ICapnpSerializable
        {
            public const UInt64 typeId = 0x82cd03a53b29d76bUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                OuterNestedEnum = reader.OuterNestedEnum;
                InnerNestedEnum = reader.InnerNestedEnum;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.OuterNestedEnum = OuterNestedEnum;
                writer.InnerNestedEnum = InnerNestedEnum;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum OuterNestedEnum
            {
                get;
                set;
            }

            = Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum.bar;
            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum InnerNestedEnum
            {
                get;
                set;
            }

            = Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum.quux;
            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum OuterNestedEnum => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum)ctx.ReadDataUShort(0UL, (ushort)1);
                public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum InnerNestedEnum => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum)ctx.ReadDataUShort(16UL, (ushort)2);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum OuterNestedEnum
                {
                    get => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum)this.ReadDataUShort(0UL, (ushort)1);
                    set => this.WriteData(0UL, (ushort)value, (ushort)1);
                }

                public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum InnerNestedEnum
                {
                    get => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum)this.ReadDataUShort(16UL, (ushort)2);
                    set => this.WriteData(16UL, (ushort)value, (ushort)2);
                }
            }

            [TypeId(0xcfa0d546993a3df3UL)]
            public enum NestedEnum : ushort
            {
                baz,
                qux,
                quux
            }
        }
    }

    [TypeId(0xe78aac389e77b065UL)]
    public class TestUsing : ICapnpSerializable
    {
        public const UInt64 typeId = 0xe78aac389e77b065UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            InnerNestedEnum = reader.InnerNestedEnum;
            OuterNestedEnum = reader.OuterNestedEnum;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.InnerNestedEnum = InnerNestedEnum;
            writer.OuterNestedEnum = OuterNestedEnum;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum InnerNestedEnum
        {
            get;
            set;
        }

        = Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum.quux;
        public Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum OuterNestedEnum
        {
            get;
            set;
        }

        = Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum.bar;
        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum InnerNestedEnum => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum)ctx.ReadDataUShort(0UL, (ushort)2);
            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum OuterNestedEnum => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum)ctx.ReadDataUShort(16UL, (ushort)1);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum InnerNestedEnum
            {
                get => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedStruct.NestedEnum)this.ReadDataUShort(0UL, (ushort)2);
                set => this.WriteData(0UL, (ushort)value, (ushort)2);
            }

            public Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum OuterNestedEnum
            {
                get => (Capnproto_test.Capnp.Test.TestNestedTypes.NestedEnum)this.ReadDataUShort(16UL, (ushort)1);
                set => this.WriteData(16UL, (ushort)value, (ushort)1);
            }
        }
    }

    [TypeId(0xe41885c94393277eUL)]
    public class TestLists : ICapnpSerializable
    {
        public const UInt64 typeId = 0xe41885c94393277eUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            List0 = reader.List0?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestLists.Struct0>(_));
            List1 = reader.List1?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestLists.Struct1>(_));
            List8 = reader.List8?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestLists.Struct8>(_));
            List16 = reader.List16?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestLists.Struct16>(_));
            List32 = reader.List32?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestLists.Struct32>(_));
            List64 = reader.List64?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestLists.Struct64>(_));
            ListP = reader.ListP?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestLists.StructP>(_));
            Int32ListList = reader.Int32ListList;
            TextListList = reader.TextListList;
            StructListList = reader.StructListList?.ToReadOnlyList(_2 => _2?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(_)));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.List0.Init(List0, (_s1, _v1) => _v1?.serialize(_s1));
            writer.List1.Init(List1, (_s1, _v1) => _v1?.serialize(_s1));
            writer.List8.Init(List8, (_s1, _v1) => _v1?.serialize(_s1));
            writer.List16.Init(List16, (_s1, _v1) => _v1?.serialize(_s1));
            writer.List32.Init(List32, (_s1, _v1) => _v1?.serialize(_s1));
            writer.List64.Init(List64, (_s1, _v1) => _v1?.serialize(_s1));
            writer.ListP.Init(ListP, (_s1, _v1) => _v1?.serialize(_s1));
            writer.Int32ListList.Init(Int32ListList, (_s2, _v2) => _s2.Init(_v2));
            writer.TextListList.Init(TextListList, (_s2, _v2) => _s2.Init(_v2));
            writer.StructListList.Init(StructListList, (_s2, _v2) => _s2.Init(_v2, (_s1, _v1) => _v1?.serialize(_s1)));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct0> List0
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct1> List1
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct8> List8
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct16> List16
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct32> List32
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct64> List64
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.StructP> ListP
        {
            get;
            set;
        }

        public IReadOnlyList<IReadOnlyList<int>> Int32ListList
        {
            get;
            set;
        }

        public IReadOnlyList<IReadOnlyList<string>> TextListList
        {
            get;
            set;
        }

        public IReadOnlyList<IReadOnlyList<Capnproto_test.Capnp.Test.TestAllTypes>> StructListList
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct0.READER> List0 => ctx.ReadList(0).Cast(Capnproto_test.Capnp.Test.TestLists.Struct0.READER.create);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct1.READER> List1 => ctx.ReadList(1).Cast(Capnproto_test.Capnp.Test.TestLists.Struct1.READER.create);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct8.READER> List8 => ctx.ReadList(2).Cast(Capnproto_test.Capnp.Test.TestLists.Struct8.READER.create);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct16.READER> List16 => ctx.ReadList(3).Cast(Capnproto_test.Capnp.Test.TestLists.Struct16.READER.create);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct32.READER> List32 => ctx.ReadList(4).Cast(Capnproto_test.Capnp.Test.TestLists.Struct32.READER.create);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.Struct64.READER> List64 => ctx.ReadList(5).Cast(Capnproto_test.Capnp.Test.TestLists.Struct64.READER.create);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestLists.StructP.READER> ListP => ctx.ReadList(6).Cast(Capnproto_test.Capnp.Test.TestLists.StructP.READER.create);
            public IReadOnlyList<IReadOnlyList<int>> Int32ListList => ctx.ReadList(7).Cast(_0 => _0.RequireList().CastInt());
            public IReadOnlyList<IReadOnlyList<string>> TextListList => ctx.ReadList(8).Cast(_0 => _0.RequireList().CastText2());
            public IReadOnlyList<IReadOnlyList<Capnproto_test.Capnp.Test.TestAllTypes.READER>> StructListList => ctx.ReadList(9).Cast(_0 => _0.RequireList().Cast(Capnproto_test.Capnp.Test.TestAllTypes.READER.create));
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 10);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct0.WRITER> List0
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct0.WRITER>>(0);
                set => Link(0, value);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct1.WRITER> List1
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct1.WRITER>>(1);
                set => Link(1, value);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct8.WRITER> List8
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct8.WRITER>>(2);
                set => Link(2, value);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct16.WRITER> List16
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct16.WRITER>>(3);
                set => Link(3, value);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct32.WRITER> List32
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct32.WRITER>>(4);
                set => Link(4, value);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct64.WRITER> List64
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.Struct64.WRITER>>(5);
                set => Link(5, value);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.StructP.WRITER> ListP
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestLists.StructP.WRITER>>(6);
                set => Link(6, value);
            }

            public ListOfPointersSerializer<ListOfPrimitivesSerializer<int>> Int32ListList
            {
                get => BuildPointer<ListOfPointersSerializer<ListOfPrimitivesSerializer<int>>>(7);
                set => Link(7, value);
            }

            public ListOfPointersSerializer<ListOfTextSerializer> TextListList
            {
                get => BuildPointer<ListOfPointersSerializer<ListOfTextSerializer>>(8);
                set => Link(8, value);
            }

            public ListOfPointersSerializer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>> StructListList
            {
                get => BuildPointer<ListOfPointersSerializer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>>>(9);
                set => Link(9, value);
            }
        }

        [TypeId(0x8412c03b75b2cfeeUL)]
        public class Struct0 : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8412c03b75b2cfeeUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xe0fe5870b141ad69UL)]
        public class Struct1 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xe0fe5870b141ad69UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public bool F
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public bool F => ctx.ReadDataBool(0UL, false);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public bool F
                {
                    get => this.ReadDataBool(0UL, false);
                    set => this.WriteData(0UL, value, false);
                }
            }
        }

        [TypeId(0xa6411a353090145bUL)]
        public class Struct8 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xa6411a353090145bUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public byte F
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public byte F => ctx.ReadDataByte(0UL, (byte)0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public byte F
                {
                    get => this.ReadDataByte(0UL, (byte)0);
                    set => this.WriteData(0UL, value, (byte)0);
                }
            }
        }

        [TypeId(0xa8abf7a82928986cUL)]
        public class Struct16 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xa8abf7a82928986cUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ushort F
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public ushort F => ctx.ReadDataUShort(0UL, (ushort)0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public ushort F
                {
                    get => this.ReadDataUShort(0UL, (ushort)0);
                    set => this.WriteData(0UL, value, (ushort)0);
                }
            }
        }

        [TypeId(0xad7beedc4ed30742UL)]
        public class Struct32 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xad7beedc4ed30742UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint F
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public uint F => ctx.ReadDataUInt(0UL, 0U);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public uint F
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }
            }
        }

        [TypeId(0xef9a34f2ff7cc646UL)]
        public class Struct64 : ICapnpSerializable
        {
            public const UInt64 typeId = 0xef9a34f2ff7cc646UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ulong F
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public ulong F => ctx.ReadDataULong(0UL, 0UL);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public ulong F
                {
                    get => this.ReadDataULong(0UL, 0UL);
                    set => this.WriteData(0UL, value, 0UL);
                }
            }
        }

        [TypeId(0xc6abf1b0329e6227UL)]
        public class StructP : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc6abf1b0329e6227UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string F
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string F => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string F
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0x943a234ca336b16aUL)]
        public class Struct0c : ICapnpSerializable
        {
            public const UInt64 typeId = 0x943a234ca336b16aUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Pad = reader.Pad;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Pad = Pad;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Pad
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string Pad => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string Pad
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0x8991bc0e74a594cdUL)]
        public class Struct1c : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8991bc0e74a594cdUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                Pad = reader.Pad;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
                writer.Pad = Pad;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public bool F
            {
                get;
                set;
            }

            public string Pad
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public bool F => ctx.ReadDataBool(0UL, false);
                public string Pad => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public bool F
                {
                    get => this.ReadDataBool(0UL, false);
                    set => this.WriteData(0UL, value, false);
                }

                public string Pad
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0xed267416528c7a24UL)]
        public class Struct8c : ICapnpSerializable
        {
            public const UInt64 typeId = 0xed267416528c7a24UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                Pad = reader.Pad;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
                writer.Pad = Pad;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public byte F
            {
                get;
                set;
            }

            public string Pad
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public byte F => ctx.ReadDataByte(0UL, (byte)0);
                public string Pad => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public byte F
                {
                    get => this.ReadDataByte(0UL, (byte)0);
                    set => this.WriteData(0UL, value, (byte)0);
                }

                public string Pad
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0x9978837b037d58e6UL)]
        public class Struct16c : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9978837b037d58e6UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                Pad = reader.Pad;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
                writer.Pad = Pad;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ushort F
            {
                get;
                set;
            }

            public string Pad
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public ushort F => ctx.ReadDataUShort(0UL, (ushort)0);
                public string Pad => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public ushort F
                {
                    get => this.ReadDataUShort(0UL, (ushort)0);
                    set => this.WriteData(0UL, value, (ushort)0);
                }

                public string Pad
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0xed5fa940f54a7904UL)]
        public class Struct32c : ICapnpSerializable
        {
            public const UInt64 typeId = 0xed5fa940f54a7904UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                Pad = reader.Pad;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
                writer.Pad = Pad;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint F
            {
                get;
                set;
            }

            public string Pad
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public uint F => ctx.ReadDataUInt(0UL, 0U);
                public string Pad => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public uint F
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }

                public string Pad
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0xbc743778f2597c7dUL)]
        public class Struct64c : ICapnpSerializable
        {
            public const UInt64 typeId = 0xbc743778f2597c7dUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                Pad = reader.Pad;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
                writer.Pad = Pad;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ulong F
            {
                get;
                set;
            }

            public string Pad
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public ulong F => ctx.ReadDataULong(0UL, 0UL);
                public string Pad => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public ulong F
                {
                    get => this.ReadDataULong(0UL, 0UL);
                    set => this.WriteData(0UL, value, 0UL);
                }

                public string Pad
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0xc2e364a40182013dUL)]
        public class StructPc : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc2e364a40182013dUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                F = reader.F;
                Pad = reader.Pad;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.F = F;
                writer.Pad = Pad;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string F
            {
                get;
                set;
            }

            public ulong Pad
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string F => ctx.ReadText(0, null);
                public ulong Pad => ctx.ReadDataULong(0UL, 0UL);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public string F
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public ulong Pad
                {
                    get => this.ReadDataULong(0UL, 0UL);
                    set => this.WriteData(0UL, value, 0UL);
                }
            }
        }
    }

    [TypeId(0x92fc29a80f3ddd5cUL)]
    public class TestFieldZeroIsBit : ICapnpSerializable
    {
        public const UInt64 typeId = 0x92fc29a80f3ddd5cUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Bit = reader.Bit;
            SecondBit = reader.SecondBit;
            ThirdField = reader.ThirdField;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Bit = Bit;
            writer.SecondBit = SecondBit;
            writer.ThirdField = ThirdField;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public bool Bit
        {
            get;
            set;
        }

        public bool SecondBit
        {
            get;
            set;
        }

        = true;
        public byte ThirdField
        {
            get;
            set;
        }

        = 123;
        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public bool Bit => ctx.ReadDataBool(0UL, false);
            public bool SecondBit => ctx.ReadDataBool(1UL, true);
            public byte ThirdField => ctx.ReadDataByte(8UL, (byte)123);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public bool Bit
            {
                get => this.ReadDataBool(0UL, false);
                set => this.WriteData(0UL, value, false);
            }

            public bool SecondBit
            {
                get => this.ReadDataBool(1UL, true);
                set => this.WriteData(1UL, value, true);
            }

            public byte ThirdField
            {
                get => this.ReadDataByte(8UL, (byte)123);
                set => this.WriteData(8UL, value, (byte)123);
            }
        }
    }

    [TypeId(0xa851ad32cbc2ffeaUL)]
    public class TestListDefaults : ICapnpSerializable
    {
        public const UInt64 typeId = 0xa851ad32cbc2ffeaUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Lists = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestLists>(reader.Lists);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Lists?.serialize(writer.Lists);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
            Lists = Lists ?? new Capnproto_test.Capnp.Test.TestLists()
            {List0 = new Capnproto_test.Capnp.Test.TestLists.Struct0[]{new Capnproto_test.Capnp.Test.TestLists.Struct0()
            {}, new Capnproto_test.Capnp.Test.TestLists.Struct0()
            {}}, List1 = new Capnproto_test.Capnp.Test.TestLists.Struct1[]{new Capnproto_test.Capnp.Test.TestLists.Struct1()
            {F = true}, new Capnproto_test.Capnp.Test.TestLists.Struct1()
            {F = false}, new Capnproto_test.Capnp.Test.TestLists.Struct1()
            {F = true}, new Capnproto_test.Capnp.Test.TestLists.Struct1()
            {F = true}}, List8 = new Capnproto_test.Capnp.Test.TestLists.Struct8[]{new Capnproto_test.Capnp.Test.TestLists.Struct8()
            {F = 123}, new Capnproto_test.Capnp.Test.TestLists.Struct8()
            {F = 45}}, List16 = new Capnproto_test.Capnp.Test.TestLists.Struct16[]{new Capnproto_test.Capnp.Test.TestLists.Struct16()
            {F = 12345}, new Capnproto_test.Capnp.Test.TestLists.Struct16()
            {F = 6789}}, List32 = new Capnproto_test.Capnp.Test.TestLists.Struct32[]{new Capnproto_test.Capnp.Test.TestLists.Struct32()
            {F = 123456789U}, new Capnproto_test.Capnp.Test.TestLists.Struct32()
            {F = 234567890U}}, List64 = new Capnproto_test.Capnp.Test.TestLists.Struct64[]{new Capnproto_test.Capnp.Test.TestLists.Struct64()
            {F = 1234567890123456UL}, new Capnproto_test.Capnp.Test.TestLists.Struct64()
            {F = 2345678901234567UL}}, ListP = new Capnproto_test.Capnp.Test.TestLists.StructP[]{new Capnproto_test.Capnp.Test.TestLists.StructP()
            {F = "foo"}, new Capnproto_test.Capnp.Test.TestLists.StructP()
            {F = "bar"}}, Int32ListList = new IReadOnlyList<int>[]{new int[]{1, 2, 3}, new int[]{4, 5}, new int[]{12341234}}, TextListList = new IReadOnlyList<string>[]{new string[]{"foo", "bar"}, new string[]{"baz"}, new string[]{"qux", "corge"}}, StructListList = new IReadOnlyList<Capnproto_test.Capnp.Test.TestAllTypes>[]{new Capnproto_test.Capnp.Test.TestAllTypes[]{new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 123, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 456, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}}, new Capnproto_test.Capnp.Test.TestAllTypes[]{new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 0, Int32Field = 789, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}}}};
        }

        public Capnproto_test.Capnp.Test.TestLists Lists
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public Capnproto_test.Capnp.Test.TestLists.READER Lists => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestLists.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public Capnproto_test.Capnp.Test.TestLists.WRITER Lists
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestLists.WRITER>(0);
                set => Link(0, value);
            }
        }
    }

    [TypeId(0xa76e3c9bb7fd56d3UL)]
    public class TestLateUnion : ICapnpSerializable
    {
        public const UInt64 typeId = 0xa76e3c9bb7fd56d3UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Foo = reader.Foo;
            Bar = reader.Bar;
            Baz = reader.Baz;
            TheUnion = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestLateUnion.theUnion>(reader.TheUnion);
            AnotherUnion = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestLateUnion.anotherUnion>(reader.AnotherUnion);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Foo = Foo;
            writer.Bar = Bar;
            writer.Baz = Baz;
            TheUnion?.serialize(writer.TheUnion);
            AnotherUnion?.serialize(writer.AnotherUnion);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public int Foo
        {
            get;
            set;
        }

        public string Bar
        {
            get;
            set;
        }

        public short Baz
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestLateUnion.theUnion TheUnion
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestLateUnion.anotherUnion AnotherUnion
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public int Foo => ctx.ReadDataInt(0UL, 0);
            public string Bar => ctx.ReadText(0, null);
            public short Baz => ctx.ReadDataShort(32UL, (short)0);
            public theUnion.READER TheUnion => new theUnion.READER(ctx);
            public anotherUnion.READER AnotherUnion => new anotherUnion.READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(3, 3);
            }

            public int Foo
            {
                get => this.ReadDataInt(0UL, 0);
                set => this.WriteData(0UL, value, 0);
            }

            public string Bar
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public short Baz
            {
                get => this.ReadDataShort(32UL, (short)0);
                set => this.WriteData(32UL, value, (short)0);
            }

            public theUnion.WRITER TheUnion
            {
                get => Rewrap<theUnion.WRITER>();
            }

            public anotherUnion.WRITER AnotherUnion
            {
                get => Rewrap<anotherUnion.WRITER>();
            }
        }

        [TypeId(0x807280a2901aa079UL)]
        public class theUnion : ICapnpSerializable
        {
            public const UInt64 typeId = 0x807280a2901aa079UL;
            public enum WHICH : ushort
            {
                Qux = 0,
                Corge = 1,
                Grault = 2,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Qux:
                        Qux = reader.Qux;
                        break;
                    case WHICH.Corge:
                        Corge = reader.Corge;
                        break;
                    case WHICH.Grault:
                        Grault = reader.Grault;
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.Qux:
                            _content = null;
                            break;
                        case WHICH.Corge:
                            _content = null;
                            break;
                        case WHICH.Grault:
                            _content = 0F;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.Qux:
                        writer.Qux = Qux;
                        break;
                    case WHICH.Corge:
                        writer.Corge.Init(Corge);
                        break;
                    case WHICH.Grault:
                        writer.Grault = Grault.Value;
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Qux
            {
                get => _which == WHICH.Qux ? (string)_content : null;
                set
                {
                    _which = WHICH.Qux;
                    _content = value;
                }
            }

            public IReadOnlyList<int> Corge
            {
                get => _which == WHICH.Corge ? (IReadOnlyList<int>)_content : null;
                set
                {
                    _which = WHICH.Corge;
                    _content = value;
                }
            }

            public float? Grault
            {
                get => _which == WHICH.Grault ? (float? )_content : null;
                set
                {
                    _which = WHICH.Grault;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(48U, (ushort)0);
                public string Qux => which == WHICH.Qux ? ctx.ReadText(1, null) : default;
                public IReadOnlyList<int> Corge => which == WHICH.Corge ? ctx.ReadList(1).CastInt() : default;
                public float Grault => which == WHICH.Grault ? ctx.ReadDataFloat(64UL, 0F) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(48U, (ushort)0);
                    set => this.WriteData(48U, (ushort)value, (ushort)0);
                }

                public string Qux
                {
                    get => which == WHICH.Qux ? this.ReadText(1, null) : default;
                    set => this.WriteText(1, value, null);
                }

                public ListOfPrimitivesSerializer<int> Corge
                {
                    get => which == WHICH.Corge ? BuildPointer<ListOfPrimitivesSerializer<int>>(1) : default;
                    set => Link(1, value);
                }

                public float Grault
                {
                    get => which == WHICH.Grault ? this.ReadDataFloat(64UL, 0F) : default;
                    set => this.WriteData(64UL, value, 0F);
                }
            }
        }

        [TypeId(0xc1973984dee98e3aUL)]
        public class anotherUnion : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc1973984dee98e3aUL;
            public enum WHICH : ushort
            {
                Qux = 0,
                Corge = 1,
                Grault = 2,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Qux:
                        Qux = reader.Qux;
                        break;
                    case WHICH.Corge:
                        Corge = reader.Corge;
                        break;
                    case WHICH.Grault:
                        Grault = reader.Grault;
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.Qux:
                            _content = null;
                            break;
                        case WHICH.Corge:
                            _content = null;
                            break;
                        case WHICH.Grault:
                            _content = 0F;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.Qux:
                        writer.Qux = Qux;
                        break;
                    case WHICH.Corge:
                        writer.Corge.Init(Corge);
                        break;
                    case WHICH.Grault:
                        writer.Grault = Grault.Value;
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Qux
            {
                get => _which == WHICH.Qux ? (string)_content : null;
                set
                {
                    _which = WHICH.Qux;
                    _content = value;
                }
            }

            public IReadOnlyList<int> Corge
            {
                get => _which == WHICH.Corge ? (IReadOnlyList<int>)_content : null;
                set
                {
                    _which = WHICH.Corge;
                    _content = value;
                }
            }

            public float? Grault
            {
                get => _which == WHICH.Grault ? (float? )_content : null;
                set
                {
                    _which = WHICH.Grault;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(96U, (ushort)0);
                public string Qux => which == WHICH.Qux ? ctx.ReadText(2, null) : default;
                public IReadOnlyList<int> Corge => which == WHICH.Corge ? ctx.ReadList(2).CastInt() : default;
                public float Grault => which == WHICH.Grault ? ctx.ReadDataFloat(128UL, 0F) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(96U, (ushort)0);
                    set => this.WriteData(96U, (ushort)value, (ushort)0);
                }

                public string Qux
                {
                    get => which == WHICH.Qux ? this.ReadText(2, null) : default;
                    set => this.WriteText(2, value, null);
                }

                public ListOfPrimitivesSerializer<int> Corge
                {
                    get => which == WHICH.Corge ? BuildPointer<ListOfPrimitivesSerializer<int>>(2) : default;
                    set => Link(2, value);
                }

                public float Grault
                {
                    get => which == WHICH.Grault ? this.ReadDataFloat(128UL, 0F) : default;
                    set => this.WriteData(128UL, value, 0F);
                }
            }
        }
    }

    [TypeId(0x95b30dd14e01dda8UL)]
    public class TestOldVersion : ICapnpSerializable
    {
        public const UInt64 typeId = 0x95b30dd14e01dda8UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Old1 = reader.Old1;
            Old2 = reader.Old2;
            Old3 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestOldVersion>(reader.Old3);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Old1 = Old1;
            writer.Old2 = Old2;
            Old3?.serialize(writer.Old3);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public long Old1
        {
            get;
            set;
        }

        public string Old2
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestOldVersion Old3
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public long Old1 => ctx.ReadDataLong(0UL, 0L);
            public string Old2 => ctx.ReadText(0, null);
            public Capnproto_test.Capnp.Test.TestOldVersion.READER Old3 => ctx.ReadStruct(1, Capnproto_test.Capnp.Test.TestOldVersion.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 2);
            }

            public long Old1
            {
                get => this.ReadDataLong(0UL, 0L);
                set => this.WriteData(0UL, value, 0L);
            }

            public string Old2
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public Capnproto_test.Capnp.Test.TestOldVersion.WRITER Old3
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestOldVersion.WRITER>(1);
                set => Link(1, value);
            }
        }
    }

    [TypeId(0x8ed75a7469f04ce3UL)]
    public class TestNewVersion : ICapnpSerializable
    {
        public const UInt64 typeId = 0x8ed75a7469f04ce3UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Old1 = reader.Old1;
            Old2 = reader.Old2;
            Old3 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestNewVersion>(reader.Old3);
            New1 = reader.New1;
            New2 = reader.New2;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Old1 = Old1;
            writer.Old2 = Old2;
            Old3?.serialize(writer.Old3);
            writer.New1 = New1;
            writer.New2 = New2;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
            New2 = New2 ?? "baz";
        }

        public long Old1
        {
            get;
            set;
        }

        public string Old2
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestNewVersion Old3
        {
            get;
            set;
        }

        public long New1
        {
            get;
            set;
        }

        = 987L;
        public string New2
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public long Old1 => ctx.ReadDataLong(0UL, 0L);
            public string Old2 => ctx.ReadText(0, null);
            public Capnproto_test.Capnp.Test.TestNewVersion.READER Old3 => ctx.ReadStruct(1, Capnproto_test.Capnp.Test.TestNewVersion.READER.create);
            public long New1 => ctx.ReadDataLong(64UL, 987L);
            public string New2 => ctx.ReadText(2, "baz");
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 3);
            }

            public long Old1
            {
                get => this.ReadDataLong(0UL, 0L);
                set => this.WriteData(0UL, value, 0L);
            }

            public string Old2
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public Capnproto_test.Capnp.Test.TestNewVersion.WRITER Old3
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestNewVersion.WRITER>(1);
                set => Link(1, value);
            }

            public long New1
            {
                get => this.ReadDataLong(64UL, 987L);
                set => this.WriteData(64UL, value, 987L);
            }

            public string New2
            {
                get => this.ReadText(2, "baz");
                set => this.WriteText(2, value, "baz");
            }
        }
    }

    [TypeId(0xbd5fe16e5170c492UL)]
    public class TestOldUnionVersion : ICapnpSerializable
    {
        public const UInt64 typeId = 0xbd5fe16e5170c492UL;
        public enum WHICH : ushort
        {
            A = 0,
            B = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.A:
                    which = reader.which;
                    break;
                case WHICH.B:
                    B = reader.B;
                    break;
            }

            applyDefaults();
        }

        private WHICH _which = WHICH.undefined;
        private object _content;
        public WHICH which
        {
            get => _which;
            set
            {
                if (value == _which)
                    return;
                _which = value;
                switch (value)
                {
                    case WHICH.A:
                        break;
                    case WHICH.B:
                        _content = 0;
                        break;
                }
            }
        }

        public void serialize(WRITER writer)
        {
            writer.which = which;
            switch (which)
            {
                case WHICH.A:
                    break;
                case WHICH.B:
                    writer.B = B.Value;
                    break;
            }
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ulong? B
        {
            get => _which == WHICH.B ? (ulong? )_content : null;
            set
            {
                _which = WHICH.B;
                _content = value;
            }
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public WHICH which => (WHICH)ctx.ReadDataUShort(0U, (ushort)0);
            public ulong B => which == WHICH.B ? ctx.ReadDataULong(64UL, 0UL) : default;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 0);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                set => this.WriteData(0U, (ushort)value, (ushort)0);
            }

            public ulong B
            {
                get => which == WHICH.B ? this.ReadDataULong(64UL, 0UL) : default;
                set => this.WriteData(64UL, value, 0UL);
            }
        }
    }

    [TypeId(0xc7e4c513a975492bUL)]
    public class TestNewUnionVersion : ICapnpSerializable
    {
        public const UInt64 typeId = 0xc7e4c513a975492bUL;
        public enum WHICH : ushort
        {
            A = 0,
            B = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.A:
                    A = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestNewUnionVersion.a>(reader.A);
                    break;
                case WHICH.B:
                    B = reader.B;
                    break;
            }

            applyDefaults();
        }

        private WHICH _which = WHICH.undefined;
        private object _content;
        public WHICH which
        {
            get => _which;
            set
            {
                if (value == _which)
                    return;
                _which = value;
                switch (value)
                {
                    case WHICH.A:
                        _content = null;
                        break;
                    case WHICH.B:
                        _content = 0;
                        break;
                }
            }
        }

        public void serialize(WRITER writer)
        {
            writer.which = which;
            switch (which)
            {
                case WHICH.A:
                    A?.serialize(writer.A);
                    break;
                case WHICH.B:
                    writer.B = B.Value;
                    break;
            }
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestNewUnionVersion.a A
        {
            get => _which == WHICH.A ? (Capnproto_test.Capnp.Test.TestNewUnionVersion.a)_content : null;
            set
            {
                _which = WHICH.A;
                _content = value;
            }
        }

        public ulong? B
        {
            get => _which == WHICH.B ? (ulong? )_content : null;
            set
            {
                _which = WHICH.B;
                _content = value;
            }
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public WHICH which => (WHICH)ctx.ReadDataUShort(0U, (ushort)0);
            public a.READER A => which == WHICH.A ? new a.READER(ctx) : default;
            public ulong B => which == WHICH.B ? ctx.ReadDataULong(64UL, 0UL) : default;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(3, 0);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                set => this.WriteData(0U, (ushort)value, (ushort)0);
            }

            public a.WRITER A
            {
                get => which == WHICH.A ? Rewrap<a.WRITER>() : default;
            }

            public ulong B
            {
                get => which == WHICH.B ? this.ReadDataULong(64UL, 0UL) : default;
                set => this.WriteData(64UL, value, 0UL);
            }
        }

        [TypeId(0x86232c1de4513e84UL)]
        public class a : ICapnpSerializable
        {
            public const UInt64 typeId = 0x86232c1de4513e84UL;
            public enum WHICH : ushort
            {
                A0 = 0,
                A1 = 1,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.A0:
                        which = reader.which;
                        break;
                    case WHICH.A1:
                        A1 = reader.A1;
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.A0:
                            break;
                        case WHICH.A1:
                            _content = 0;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.A0:
                        break;
                    case WHICH.A1:
                        writer.A1 = A1.Value;
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ulong? A1
            {
                get => _which == WHICH.A1 ? (ulong? )_content : null;
                set
                {
                    _which = WHICH.A1;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(64U, (ushort)0);
                public ulong A1 => which == WHICH.A1 ? ctx.ReadDataULong(128UL, 0UL) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(64U, (ushort)0);
                    set => this.WriteData(64U, (ushort)value, (ushort)0);
                }

                public ulong A1
                {
                    get => which == WHICH.A1 ? this.ReadDataULong(128UL, 0UL) : default;
                    set => this.WriteData(128UL, value, 0UL);
                }
            }
        }
    }

    [TypeId(0xfaf781ef89a00e39UL)]
    public class TestStructUnion : ICapnpSerializable
    {
        public const UInt64 typeId = 0xfaf781ef89a00e39UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Un = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestStructUnion.un>(reader.Un);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Un?.serialize(writer.Un);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestStructUnion.un Un
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public un.READER Un => new un.READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public un.WRITER Un
            {
                get => Rewrap<un.WRITER>();
            }
        }

        [TypeId(0x992edc677bef5a3cUL)]
        public class un : ICapnpSerializable
        {
            public const UInt64 typeId = 0x992edc677bef5a3cUL;
            public enum WHICH : ushort
            {
                Struct = 0,
                Object = 1,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Struct:
                        Struct = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestStructUnion.SomeStruct>(reader.Struct);
                        break;
                    case WHICH.Object:
                        Object = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAnyPointer>(reader.Object);
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.Struct:
                            _content = null;
                            break;
                        case WHICH.Object:
                            _content = null;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.Struct:
                        Struct?.serialize(writer.Struct);
                        break;
                    case WHICH.Object:
                        Object?.serialize(writer.Object);
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestStructUnion.SomeStruct Struct
            {
                get => _which == WHICH.Struct ? (Capnproto_test.Capnp.Test.TestStructUnion.SomeStruct)_content : null;
                set
                {
                    _which = WHICH.Struct;
                    _content = value;
                }
            }

            public Capnproto_test.Capnp.Test.TestAnyPointer Object
            {
                get => _which == WHICH.Object ? (Capnproto_test.Capnp.Test.TestAnyPointer)_content : null;
                set
                {
                    _which = WHICH.Object;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(0U, (ushort)0);
                public Capnproto_test.Capnp.Test.TestStructUnion.SomeStruct.READER Struct => which == WHICH.Struct ? ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestStructUnion.SomeStruct.READER.create) : default;
                public Capnproto_test.Capnp.Test.TestAnyPointer.READER Object => which == WHICH.Object ? ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestAnyPointer.READER.create) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                    set => this.WriteData(0U, (ushort)value, (ushort)0);
                }

                public Capnproto_test.Capnp.Test.TestStructUnion.SomeStruct.WRITER Struct
                {
                    get => which == WHICH.Struct ? BuildPointer<Capnproto_test.Capnp.Test.TestStructUnion.SomeStruct.WRITER>(0) : default;
                    set => Link(0, value);
                }

                public Capnproto_test.Capnp.Test.TestAnyPointer.WRITER Object
                {
                    get => which == WHICH.Object ? BuildPointer<Capnproto_test.Capnp.Test.TestAnyPointer.WRITER>(0) : default;
                    set => Link(0, value);
                }
            }
        }

        [TypeId(0x9daec9823f171085UL)]
        public class SomeStruct : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9daec9823f171085UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                SomeText = reader.SomeText;
                MoreText = reader.MoreText;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.SomeText = SomeText;
                writer.MoreText = MoreText;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string SomeText
            {
                get;
                set;
            }

            public string MoreText
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string SomeText => ctx.ReadText(0, null);
                public string MoreText => ctx.ReadText(1, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public string SomeText
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public string MoreText
                {
                    get => this.ReadText(1, null);
                    set => this.WriteText(1, value, null);
                }
            }
        }
    }

    [TypeId(0xdec497819d097c3cUL)]
    public class TestPrintInlineStructs : ICapnpSerializable
    {
        public const UInt64 typeId = 0xdec497819d097c3cUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            SomeText = reader.SomeText;
            StructList = reader.StructList?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestPrintInlineStructs.InlineStruct>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.SomeText = SomeText;
            writer.StructList.Init(StructList, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string SomeText
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestPrintInlineStructs.InlineStruct> StructList
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public string SomeText => ctx.ReadText(0, null);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestPrintInlineStructs.InlineStruct.READER> StructList => ctx.ReadList(1).Cast(Capnproto_test.Capnp.Test.TestPrintInlineStructs.InlineStruct.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 2);
            }

            public string SomeText
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestPrintInlineStructs.InlineStruct.WRITER> StructList
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestPrintInlineStructs.InlineStruct.WRITER>>(1);
                set => Link(1, value);
            }
        }

        [TypeId(0x8e4936003708dac2UL)]
        public class InlineStruct : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8e4936003708dac2UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Int32Field = reader.Int32Field;
                TextField = reader.TextField;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Int32Field = Int32Field;
                writer.TextField = TextField;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public int Int32Field
            {
                get;
                set;
            }

            public string TextField
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public int Int32Field => ctx.ReadDataInt(0UL, 0);
                public string TextField => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public int Int32Field
                {
                    get => this.ReadDataInt(0UL, 0);
                    set => this.WriteData(0UL, value, 0);
                }

                public string TextField
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }
    }

    [TypeId(0x91afd4a864dbb030UL)]
    public class TestWholeFloatDefault : ICapnpSerializable
    {
        public const UInt64 typeId = 0x91afd4a864dbb030UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Field = reader.Field;
            BigField = reader.BigField;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Field = Field;
            writer.BigField = BigField;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public float Field
        {
            get;
            set;
        }

        = 123F;
        public float BigField
        {
            get;
            set;
        }

        = 2E+30F;
        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public float Field => ctx.ReadDataFloat(0UL, 123F);
            public float BigField => ctx.ReadDataFloat(32UL, 2E+30F);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public float Field
            {
                get => this.ReadDataFloat(0UL, 123F);
                set => this.WriteData(0UL, value, 123F);
            }

            public float BigField
            {
                get => this.ReadDataFloat(32UL, 2E+30F);
                set => this.WriteData(32UL, value, 2E+30F);
            }
        }
    }

    [TypeId(0x9d5b8cd8de9922ebUL)]
    public class TestGenerics<TFoo, TBar> : ICapnpSerializable where TFoo : class where TBar : class
    {
        public const UInt64 typeId = 0x9d5b8cd8de9922ebUL;
        public enum WHICH : ushort
        {
            Uv = 0,
            Ug = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.Uv:
                    which = reader.which;
                    break;
                case WHICH.Ug:
                    Ug = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.ug>(reader.Ug);
                    break;
            }

            Foo = CapnpSerializable.Create<TFoo>(reader.Foo);
            Rev = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TBar, TFoo>>(reader.Rev);
            List = reader.List?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner>(_));
            applyDefaults();
        }

        private WHICH _which = WHICH.undefined;
        private object _content;
        public WHICH which
        {
            get => _which;
            set
            {
                if (value == _which)
                    return;
                _which = value;
                switch (value)
                {
                    case WHICH.Uv:
                        break;
                    case WHICH.Ug:
                        _content = null;
                        break;
                }
            }
        }

        public void serialize(WRITER writer)
        {
            writer.which = which;
            switch (which)
            {
                case WHICH.Uv:
                    break;
                case WHICH.Ug:
                    Ug?.serialize(writer.Ug);
                    break;
            }

            writer.Foo.SetObject(Foo);
            Rev?.serialize(writer.Rev);
            writer.List.Init(List, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public TFoo Foo
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<TBar, TFoo> Rev
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.ug Ug
        {
            get => _which == WHICH.Ug ? (Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.ug)_content : null;
            set
            {
                _which = WHICH.Ug;
                _content = value;
            }
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner> List
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public WHICH which => (WHICH)ctx.ReadDataUShort(0U, (ushort)0);
            public DeserializerState Foo => ctx.StructReadPointer(0);
            public Capnproto_test.Capnp.Test.TestGenerics<TBar, TFoo>.READER Rev => ctx.ReadStruct(1, Capnproto_test.Capnp.Test.TestGenerics<TBar, TFoo>.READER.create);
            public ug.READER Ug => which == WHICH.Ug ? new ug.READER(ctx) : default;
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.READER> List => ctx.ReadList(2).Cast(Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 3);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                set => this.WriteData(0U, (ushort)value, (ushort)0);
            }

            public DynamicSerializerState Foo
            {
                get => BuildPointer<DynamicSerializerState>(0);
                set => Link(0, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<TBar, TFoo>.WRITER Rev
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<TBar, TFoo>.WRITER>(1);
                set => Link(1, value);
            }

            public ug.WRITER Ug
            {
                get => which == WHICH.Ug ? Rewrap<ug.WRITER>() : default;
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.WRITER> List
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.WRITER>>(2);
                set => Link(2, value);
            }
        }

        [TypeId(0xb46a779beaf3384eUL)]
        public class ug : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb46a779beaf3384eUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Ugfoo = reader.Ugfoo;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Ugfoo = Ugfoo;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public int Ugfoo
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public int Ugfoo => ctx.ReadDataInt(32UL, 0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public int Ugfoo
                {
                    get => this.ReadDataInt(32UL, 0);
                    set => this.WriteData(32UL, value, 0);
                }
            }
        }

        [TypeId(0xf6a841117e19ac73UL)]
        public class Inner : ICapnpSerializable
        {
            public const UInt64 typeId = 0xf6a841117e19ac73UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Foo = CapnpSerializable.Create<TFoo>(reader.Foo);
                Bar = CapnpSerializable.Create<TBar>(reader.Bar);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Foo.SetObject(Foo);
                writer.Bar.SetObject(Bar);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public TFoo Foo
            {
                get;
                set;
            }

            public TBar Bar
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public DeserializerState Foo => ctx.StructReadPointer(0);
                public DeserializerState Bar => ctx.StructReadPointer(1);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public DynamicSerializerState Foo
                {
                    get => BuildPointer<DynamicSerializerState>(0);
                    set => Link(0, value);
                }

                public DynamicSerializerState Bar
                {
                    get => BuildPointer<DynamicSerializerState>(1);
                    set => Link(1, value);
                }
            }
        }

        [TypeId(0xa9ab42b118d6d435UL)]
        public class Inner2<TBaz> : ICapnpSerializable where TBaz : class
        {
            public const UInt64 typeId = 0xa9ab42b118d6d435UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Bar = CapnpSerializable.Create<TBar>(reader.Bar);
                Baz = CapnpSerializable.Create<TBaz>(reader.Baz);
                InnerBound = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner>(reader.InnerBound);
                InnerUnbound = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner>(reader.InnerUnbound);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Bar.SetObject(Bar);
                writer.Baz.SetObject(Baz);
                InnerBound?.serialize(writer.InnerBound);
                InnerUnbound?.serialize(writer.InnerUnbound);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public TBar Bar
            {
                get;
                set;
            }

            public TBaz Baz
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner InnerBound
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner InnerUnbound
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public DeserializerState Bar => ctx.StructReadPointer(0);
                public DeserializerState Baz => ctx.StructReadPointer(1);
                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.READER InnerBound => ctx.ReadStruct(2, Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.READER.create);
                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.READER InnerUnbound => ctx.ReadStruct(3, Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 4);
                }

                public DynamicSerializerState Bar
                {
                    get => BuildPointer<DynamicSerializerState>(0);
                    set => Link(0, value);
                }

                public DynamicSerializerState Baz
                {
                    get => BuildPointer<DynamicSerializerState>(1);
                    set => Link(1, value);
                }

                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.WRITER InnerBound
                {
                    get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.WRITER>(2);
                    set => Link(2, value);
                }

                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.WRITER InnerUnbound
                {
                    get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.WRITER>(3);
                    set => Link(3, value);
                }
            }

            [TypeId(0xb6a0829c762b06f3UL)]
            public class DeepNest<TQux> : ICapnpSerializable where TQux : class
            {
                public const UInt64 typeId = 0xb6a0829c762b06f3UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Foo = CapnpSerializable.Create<TFoo>(reader.Foo);
                    Bar = CapnpSerializable.Create<TBar>(reader.Bar);
                    Baz = CapnpSerializable.Create<TBaz>(reader.Baz);
                    Qux = CapnpSerializable.Create<TQux>(reader.Qux);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Foo.SetObject(Foo);
                    writer.Bar.SetObject(Bar);
                    writer.Baz.SetObject(Baz);
                    writer.Qux.SetObject(Qux);
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public TFoo Foo
                {
                    get;
                    set;
                }

                public TBar Bar
                {
                    get;
                    set;
                }

                public TBaz Baz
                {
                    get;
                    set;
                }

                public TQux Qux
                {
                    get;
                    set;
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                    public DeserializerState Foo => ctx.StructReadPointer(0);
                    public DeserializerState Bar => ctx.StructReadPointer(1);
                    public DeserializerState Baz => ctx.StructReadPointer(2);
                    public DeserializerState Qux => ctx.StructReadPointer(3);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 4);
                    }

                    public DynamicSerializerState Foo
                    {
                        get => BuildPointer<DynamicSerializerState>(0);
                        set => Link(0, value);
                    }

                    public DynamicSerializerState Bar
                    {
                        get => BuildPointer<DynamicSerializerState>(1);
                        set => Link(1, value);
                    }

                    public DynamicSerializerState Baz
                    {
                        get => BuildPointer<DynamicSerializerState>(2);
                        set => Link(2, value);
                    }

                    public DynamicSerializerState Qux
                    {
                        get => BuildPointer<DynamicSerializerState>(3);
                        set => Link(3, value);
                    }
                }

                [TypeId(0x8839ed86c9794287UL), Proxy(typeof(DeepNestInterface_Proxy<>)), Skeleton(typeof(DeepNestInterface_Skeleton<>))]
                public interface IDeepNestInterface<TQuux> : IDisposable where TQuux : class
                {
                    Task Call(CancellationToken cancellationToken_ = default);
                }

                public class DeepNestInterface_Proxy<TQuux> : Proxy, IDeepNestInterface<TQuux> where TQuux : class
                {
                    public async Task Call(CancellationToken cancellationToken_ = default)
                    {
                        var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<TBaz>.DeepNest<TQux>.DeepNestInterface<TQuux>.Params_Call.WRITER>();
                        var arg_ = new Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<TBaz>.DeepNest<TQux>.DeepNestInterface<TQuux>.Params_Call()
                        {};
                        arg_?.serialize(in_);
                        using (var d_ = await Call(9816138025992274567UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
                        {
                            var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<TBaz>.DeepNest<TQux>.DeepNestInterface<TQuux>.Result_Call>(d_);
                            return;
                        }
                    }
                }

                public class DeepNestInterface_Skeleton<TQuux> : Skeleton<IDeepNestInterface<TQuux>> where TQuux : class
                {
                    public DeepNestInterface_Skeleton()
                    {
                        SetMethodTable(Call);
                    }

                    public override ulong InterfaceId => 9816138025992274567UL;
                    async Task<AnswerOrCounterquestion> Call(DeserializerState d_, CancellationToken cancellationToken_)
                    {
                        using (d_)
                        {
                            await Impl.Call(cancellationToken_);
                            var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<TBaz>.DeepNest<TQux>.DeepNestInterface<TQuux>.Result_Call.WRITER>();
                            return s_;
                        }
                    }
                }

                public static class DeepNestInterface<TQuux>
                    where TQuux : class
                {
                    [TypeId(0xb84eecc799437049UL)]
                    public class Params_Call : ICapnpSerializable
                    {
                        public const UInt64 typeId = 0xb84eecc799437049UL;
                        void ICapnpSerializable.Deserialize(DeserializerState arg_)
                        {
                            var reader = READER.create(arg_);
                            applyDefaults();
                        }

                        public void serialize(WRITER writer)
                        {
                        }

                        void ICapnpSerializable.Serialize(SerializerState arg_)
                        {
                            serialize(arg_.Rewrap<WRITER>());
                        }

                        public void applyDefaults()
                        {
                        }

                        public struct READER
                        {
                            readonly DeserializerState ctx;
                            public READER(DeserializerState ctx)
                            {
                                this.ctx = ctx;
                            }

                            public static READER create(DeserializerState ctx) => new READER(ctx);
                            public static implicit operator DeserializerState(READER reader) => reader.ctx;
                            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                        }

                        public class WRITER : SerializerState
                        {
                            public WRITER()
                            {
                                this.SetStruct(0, 0);
                            }
                        }
                    }

                    [TypeId(0xe080f0fc54614f6fUL)]
                    public class Result_Call : ICapnpSerializable
                    {
                        public const UInt64 typeId = 0xe080f0fc54614f6fUL;
                        void ICapnpSerializable.Deserialize(DeserializerState arg_)
                        {
                            var reader = READER.create(arg_);
                            applyDefaults();
                        }

                        public void serialize(WRITER writer)
                        {
                        }

                        void ICapnpSerializable.Serialize(SerializerState arg_)
                        {
                            serialize(arg_.Rewrap<WRITER>());
                        }

                        public void applyDefaults()
                        {
                        }

                        public struct READER
                        {
                            readonly DeserializerState ctx;
                            public READER(DeserializerState ctx)
                            {
                                this.ctx = ctx;
                            }

                            public static READER create(DeserializerState ctx) => new READER(ctx);
                            public static implicit operator DeserializerState(READER reader) => reader.ctx;
                            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                        }

                        public class WRITER : SerializerState
                        {
                            public WRITER()
                            {
                                this.SetStruct(0, 0);
                            }
                        }
                    }
                }
            }
        }

        [TypeId(0xc9e749e8dd54da5cUL), Proxy(typeof(Interface_Proxy<>)), Skeleton(typeof(Interface_Skeleton<>))]
        public interface IInterface<TQux> : IDisposable where TQux : class
        {
            Task<(TQux, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>)> Call(Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string> arg_, CancellationToken cancellationToken_ = default);
        }

        public class Interface_Proxy<TQux> : Proxy, IInterface<TQux> where TQux : class
        {
            public Task<(TQux, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>)> Call(Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string> arg_, CancellationToken cancellationToken_ = default)
            {
                var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>.WRITER>();
                arg_?.serialize(in_);
                return Impatient.MakePipelineAware(Call(14548678385738242652UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
                {
                    using (d_)
                    {
                        var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Interface<TQux>.Result_Call>(d_);
                        return (r_.Qux, r_.Gen);
                    }
                }

                );
            }
        }

        public class Interface_Skeleton<TQux> : Skeleton<IInterface<TQux>> where TQux : class
        {
            public Interface_Skeleton()
            {
                SetMethodTable(Call);
            }

            public override ulong InterfaceId => 14548678385738242652UL;
            Task<AnswerOrCounterquestion> Call(DeserializerState d_, CancellationToken cancellationToken_)
            {
                using (d_)
                {
                    return Impatient.MaybeTailCall(Impl.Call(CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>>(d_), cancellationToken_), (qux, gen) =>
                    {
                        var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Interface<TQux>.Result_Call.WRITER>();
                        var r_ = new Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Interface<TQux>.Result_Call{Qux = qux, Gen = gen};
                        r_.serialize(s_);
                        return s_;
                    }

                    );
                }
            }
        }

        public static class Interface<TQux>
            where TQux : class
        {
            [TypeId(0xa5b46224e33581adUL)]
            public class Result_Call : ICapnpSerializable
            {
                public const UInt64 typeId = 0xa5b46224e33581adUL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Qux = CapnpSerializable.Create<TQux>(reader.Qux);
                    Gen = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>>(reader.Gen);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Qux.SetObject(Qux);
                    Gen?.serialize(writer.Gen);
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public TQux Qux
                {
                    get;
                    set;
                }

                public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer> Gen
                {
                    get;
                    set;
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                    public DeserializerState Qux => ctx.StructReadPointer(0);
                    public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.READER Gen => ctx.ReadStruct(1, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.READER.create);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 2);
                    }

                    public DynamicSerializerState Qux
                    {
                        get => BuildPointer<DynamicSerializerState>(0);
                        set => Link(0, value);
                    }

                    public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.WRITER Gen
                    {
                        get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.WRITER>(1);
                        set => Link(1, value);
                    }
                }
            }
        }

        [TypeId(0x8e656edfb45ba6cfUL)]
        public class UseAliases : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8e656edfb45ba6cfUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Foo = CapnpSerializable.Create<TFoo>(reader.Foo);
                Inner = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner>(reader.Inner);
                Inner2 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<object>>(reader.Inner2);
                Inner2Bind = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>>(reader.Inner2Bind);
                Inner2Text = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>>(reader.Inner2Text);
                RevFoo = CapnpSerializable.Create<TBar>(reader.RevFoo);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Foo.SetObject(Foo);
                Inner?.serialize(writer.Inner);
                Inner2?.serialize(writer.Inner2);
                Inner2Bind?.serialize(writer.Inner2Bind);
                Inner2Text?.serialize(writer.Inner2Text);
                writer.RevFoo.SetObject(RevFoo);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public TFoo Foo
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner Inner
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<object> Inner2
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string> Inner2Bind
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string> Inner2Text
            {
                get;
                set;
            }

            public TBar RevFoo
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public DeserializerState Foo => ctx.StructReadPointer(0);
                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.READER Inner => ctx.ReadStruct(1, Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.READER.create);
                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<object>.READER Inner2 => ctx.ReadStruct(2, Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<object>.READER.create);
                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>.READER Inner2Bind => ctx.ReadStruct(3, Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>.READER.create);
                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>.READER Inner2Text => ctx.ReadStruct(4, Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>.READER.create);
                public DeserializerState RevFoo => ctx.StructReadPointer(5);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 6);
                }

                public DynamicSerializerState Foo
                {
                    get => BuildPointer<DynamicSerializerState>(0);
                    set => Link(0, value);
                }

                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.WRITER Inner
                {
                    get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner.WRITER>(1);
                    set => Link(1, value);
                }

                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<object>.WRITER Inner2
                {
                    get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<object>.WRITER>(2);
                    set => Link(2, value);
                }

                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>.WRITER Inner2Bind
                {
                    get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>.WRITER>(3);
                    set => Link(3, value);
                }

                public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>.WRITER Inner2Text
                {
                    get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.Inner2<string>.WRITER>(4);
                    set => Link(4, value);
                }

                public DynamicSerializerState RevFoo
                {
                    get => BuildPointer<DynamicSerializerState>(5);
                    set => Link(5, value);
                }
            }
        }
    }

    [TypeId(0xa9b2b1f52dde845dUL)]
    public class TestGenericsWrapper<TFoo, TBar> : ICapnpSerializable where TFoo : class where TBar : class
    {
        public const UInt64 typeId = 0xa9b2b1f52dde845dUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Value = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>>(reader.Value);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Value?.serialize(writer.Value);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar> Value
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.READER Value => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.WRITER Value
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<TFoo, TBar>.WRITER>(0);
                set => Link(0, value);
            }
        }
    }

    [TypeId(0xf28f83667a557a04UL)]
    public class TestGenericsWrapper2 : ICapnpSerializable
    {
        public const UInt64 typeId = 0xf28f83667a557a04UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Value = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>>(reader.Value);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Value?.serialize(writer.Value);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes> Value
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>.READER Value => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>.WRITER Value
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>.WRITER>(0);
                set => Link(0, value);
            }
        }
    }

    [TypeId(0x8b9717a3f8d85a9aUL), Proxy(typeof(TestImplicitMethodParams_Proxy)), Skeleton(typeof(TestImplicitMethodParams_Skeleton))]
    public interface ITestImplicitMethodParams : IDisposable
    {
        Task<Capnproto_test.Capnp.Test.TestGenerics<TT, TU>> Call<TT, TU>(TT Foo, TU Bar, CancellationToken cancellationToken_ = default)
            where TT : class where TU : class;
    }

    public class TestImplicitMethodParams_Proxy : Proxy, ITestImplicitMethodParams
    {
        public Task<Capnproto_test.Capnp.Test.TestGenerics<TT, TU>> Call<TT, TU>(TT Foo, TU Bar, CancellationToken cancellationToken_ = default)
            where TT : class where TU : class
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestImplicitMethodParams.Params_Call<TT, TU>.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestImplicitMethodParams.Params_Call<TT, TU>()
            {Foo = Foo, Bar = Bar};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(10058534285777328794UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TT, TU>>(d_);
                    return r_;
                }
            }

            );
        }
    }

    public class TestImplicitMethodParams_Skeleton : Skeleton<ITestImplicitMethodParams>
    {
        public TestImplicitMethodParams_Skeleton()
        {
            SetMethodTable(Call<AnyPointer, AnyPointer>);
        }

        public override ulong InterfaceId => 10058534285777328794UL;
        Task<AnswerOrCounterquestion> Call<TT, TU>(DeserializerState d_, CancellationToken cancellationToken_)
            where TT : class where TU : class
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestImplicitMethodParams.Params_Call<TT, TU>>(d_);
                return Impatient.MaybeTailCall(Impl.Call<TT, TU>(in_.Foo, in_.Bar, cancellationToken_), r_ =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestGenerics<TT, TU>.WRITER>();
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }
    }

    public static class TestImplicitMethodParams
    {
        [TypeId(0xf83f8caf54bdc486UL)]
        public class Params_Call<TT, TU> : ICapnpSerializable where TT : class where TU : class
        {
            public const UInt64 typeId = 0xf83f8caf54bdc486UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Foo = CapnpSerializable.Create<TT>(reader.Foo);
                Bar = CapnpSerializable.Create<TU>(reader.Bar);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Foo.SetObject(Foo);
                writer.Bar.SetObject(Bar);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public TT Foo
            {
                get;
                set;
            }

            public TU Bar
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public DeserializerState Foo => ctx.StructReadPointer(0);
                public DeserializerState Bar => ctx.StructReadPointer(1);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public DynamicSerializerState Foo
                {
                    get => BuildPointer<DynamicSerializerState>(0);
                    set => Link(0, value);
                }

                public DynamicSerializerState Bar
                {
                    get => BuildPointer<DynamicSerializerState>(1);
                    set => Link(1, value);
                }
            }
        }
    }

    [TypeId(0xdf9ccdeb81a704c9UL), Proxy(typeof(TestImplicitMethodParamsInGeneric_Proxy<>)), Skeleton(typeof(TestImplicitMethodParamsInGeneric_Skeleton<>))]
    public interface ITestImplicitMethodParamsInGeneric<TV> : IDisposable where TV : class
    {
        Task<Capnproto_test.Capnp.Test.TestGenerics<TT, TU>> Call<TT, TU>(TT Foo, TU Bar, CancellationToken cancellationToken_ = default)
            where TT : class where TU : class;
    }

    public class TestImplicitMethodParamsInGeneric_Proxy<TV> : Proxy, ITestImplicitMethodParamsInGeneric<TV> where TV : class
    {
        public Task<Capnproto_test.Capnp.Test.TestGenerics<TT, TU>> Call<TT, TU>(TT Foo, TU Bar, CancellationToken cancellationToken_ = default)
            where TT : class where TU : class
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestImplicitMethodParamsInGeneric<TV>.Params_Call<TT, TU>.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestImplicitMethodParamsInGeneric<TV>.Params_Call<TT, TU>()
            {Foo = Foo, Bar = Bar};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(16112979978201007305UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<TT, TU>>(d_);
                    return r_;
                }
            }

            );
        }
    }

    public class TestImplicitMethodParamsInGeneric_Skeleton<TV> : Skeleton<ITestImplicitMethodParamsInGeneric<TV>> where TV : class
    {
        public TestImplicitMethodParamsInGeneric_Skeleton()
        {
            SetMethodTable(Call<AnyPointer, AnyPointer>);
        }

        public override ulong InterfaceId => 16112979978201007305UL;
        Task<AnswerOrCounterquestion> Call<TT, TU>(DeserializerState d_, CancellationToken cancellationToken_)
            where TT : class where TU : class
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestImplicitMethodParamsInGeneric<TV>.Params_Call<TT, TU>>(d_);
                return Impatient.MaybeTailCall(Impl.Call<TT, TU>(in_.Foo, in_.Bar, cancellationToken_), r_ =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestGenerics<TT, TU>.WRITER>();
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }
    }

    public static class TestImplicitMethodParamsInGeneric<TV>
        where TV : class
    {
        [TypeId(0x9aab8e25c808d71eUL)]
        public class Params_Call<TT, TU> : ICapnpSerializable where TT : class where TU : class
        {
            public const UInt64 typeId = 0x9aab8e25c808d71eUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Foo = CapnpSerializable.Create<TT>(reader.Foo);
                Bar = CapnpSerializable.Create<TU>(reader.Bar);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Foo.SetObject(Foo);
                writer.Bar.SetObject(Bar);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public TT Foo
            {
                get;
                set;
            }

            public TU Bar
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public DeserializerState Foo => ctx.StructReadPointer(0);
                public DeserializerState Bar => ctx.StructReadPointer(1);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public DynamicSerializerState Foo
                {
                    get => BuildPointer<DynamicSerializerState>(0);
                    set => Link(0, value);
                }

                public DynamicSerializerState Bar
                {
                    get => BuildPointer<DynamicSerializerState>(1);
                    set => Link(1, value);
                }
            }
        }
    }

    [TypeId(0xa54870440e919063UL)]
    public class TestGenericsUnion<TFoo, TBar> : ICapnpSerializable where TFoo : class where TBar : class
    {
        public const UInt64 typeId = 0xa54870440e919063UL;
        public enum WHICH : ushort
        {
            Foo = 0,
            Bar = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.Foo:
                    Foo = CapnpSerializable.Create<TFoo>(reader.Foo);
                    break;
                case WHICH.Bar:
                    Bar = CapnpSerializable.Create<TBar>(reader.Bar);
                    break;
            }

            applyDefaults();
        }

        private WHICH _which = WHICH.undefined;
        private object _content;
        public WHICH which
        {
            get => _which;
            set
            {
                if (value == _which)
                    return;
                _which = value;
                switch (value)
                {
                    case WHICH.Foo:
                        _content = null;
                        break;
                    case WHICH.Bar:
                        _content = null;
                        break;
                }
            }
        }

        public void serialize(WRITER writer)
        {
            writer.which = which;
            switch (which)
            {
                case WHICH.Foo:
                    writer.Foo.SetObject(Foo);
                    break;
                case WHICH.Bar:
                    writer.Bar.SetObject(Bar);
                    break;
            }
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public TFoo Foo
        {
            get => _which == WHICH.Foo ? (TFoo)_content : null;
            set
            {
                _which = WHICH.Foo;
                _content = value;
            }
        }

        public TBar Bar
        {
            get => _which == WHICH.Bar ? (TBar)_content : null;
            set
            {
                _which = WHICH.Bar;
                _content = value;
            }
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public WHICH which => (WHICH)ctx.ReadDataUShort(0U, (ushort)0);
            public DeserializerState Foo => which == WHICH.Foo ? ctx.StructReadPointer(0) : default;
            public DeserializerState Bar => which == WHICH.Bar ? ctx.StructReadPointer(0) : default;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                set => this.WriteData(0U, (ushort)value, (ushort)0);
            }

            public DynamicSerializerState Foo
            {
                get => which == WHICH.Foo ? BuildPointer<DynamicSerializerState>(0) : default;
                set => Link(0, value);
            }

            public DynamicSerializerState Bar
            {
                get => which == WHICH.Bar ? BuildPointer<DynamicSerializerState>(0) : default;
                set => Link(0, value);
            }
        }
    }

    [TypeId(0x9427b2a71030338fUL)]
    public class TestUseGenerics : ICapnpSerializable
    {
        public const UInt64 typeId = 0x9427b2a71030338fUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Basic = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>>(reader.Basic);
            Inner = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner>(reader.Inner);
            Inner2 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>>(reader.Inner2);
            Unspecified = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<object, object>>(reader.Unspecified);
            UnspecifiedInner = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<object, object>.Inner2<string>>(reader.UnspecifiedInner);
            Default = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>>(reader.Default);
            DefaultInner = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner>(reader.DefaultInner);
            DefaultUser = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestUseGenerics>(reader.DefaultUser);
            Wrapper = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenericsWrapper<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>>(reader.Wrapper);
            DefaultWrapper = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>>(reader.DefaultWrapper);
            DefaultWrapper2 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenericsWrapper2>(reader.DefaultWrapper2);
            AliasFoo = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(reader.AliasFoo);
            AliasInner = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner>(reader.AliasInner);
            AliasInner2 = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<object>>(reader.AliasInner2);
            AliasInner2Bind = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<IReadOnlyList<uint>>>(reader.AliasInner2Bind);
            AliasInner2Text = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>>(reader.AliasInner2Text);
            AliasRev = reader.AliasRev;
            UseAliases = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.UseAliases>(reader.UseAliases);
            Cap = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.ITestInterface, string>>(reader.Cap);
            GenericCap = reader.GenericCap;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Basic?.serialize(writer.Basic);
            Inner?.serialize(writer.Inner);
            Inner2?.serialize(writer.Inner2);
            Unspecified?.serialize(writer.Unspecified);
            UnspecifiedInner?.serialize(writer.UnspecifiedInner);
            Default?.serialize(writer.Default);
            DefaultInner?.serialize(writer.DefaultInner);
            DefaultUser?.serialize(writer.DefaultUser);
            Wrapper?.serialize(writer.Wrapper);
            DefaultWrapper?.serialize(writer.DefaultWrapper);
            DefaultWrapper2?.serialize(writer.DefaultWrapper2);
            AliasFoo?.serialize(writer.AliasFoo);
            AliasInner?.serialize(writer.AliasInner);
            AliasInner2?.serialize(writer.AliasInner2);
            AliasInner2Bind?.serialize(writer.AliasInner2Bind);
            AliasInner2Text?.serialize(writer.AliasInner2Text);
            writer.AliasRev = AliasRev;
            UseAliases?.serialize(writer.UseAliases);
            Cap?.serialize(writer.Cap);
            writer.GenericCap = GenericCap;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
            Default = Default ?? new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Rev = new Capnproto_test.Capnp.Test.TestGenerics<string, Capnproto_test.Capnp.Test.TestAllTypes>()
            {Foo = "text", Rev = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 321, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Rev = new Capnproto_test.Capnp.Test.TestGenerics<string, Capnproto_test.Capnp.Test.TestAllTypes>()
            {}, List = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner[]{}}, List = new Capnproto_test.Capnp.Test.TestGenerics<string, Capnproto_test.Capnp.Test.TestAllTypes>.Inner[]{}}, List = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner[]{}};
            DefaultInner = DefaultInner ?? new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Bar = "text"};
            DefaultUser = DefaultUser ?? new Capnproto_test.Capnp.Test.TestUseGenerics()
            {Basic = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Rev = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAnyPointer, Capnproto_test.Capnp.Test.TestAllTypes>()
            {}, List = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner[]{}}, Inner = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner()
            {}, Inner2 = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>()
            {}, Unspecified = new Capnproto_test.Capnp.Test.TestGenerics<object, object>()
            {}, UnspecifiedInner = new Capnproto_test.Capnp.Test.TestGenerics<object, object>.Inner2<string>()
            {}, Default = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>()
            {}, DefaultInner = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner()
            {}, DefaultUser = new Capnproto_test.Capnp.Test.TestUseGenerics()
            {}, Wrapper = new Capnproto_test.Capnp.Test.TestGenericsWrapper<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>()
            {}, DefaultWrapper = new Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>()
            {}, DefaultWrapper2 = new Capnproto_test.Capnp.Test.TestGenericsWrapper2()
            {}, AliasFoo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, AliasInner = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner()
            {}, AliasInner2 = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<object>()
            {}, AliasInner2Bind = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<IReadOnlyList<uint>>()
            {}, AliasInner2Text = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>()
            {}, AliasRev = null, UseAliases = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.UseAliases()
            {}, Cap = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.ITestInterface, string>()
            {}};
            DefaultWrapper = DefaultWrapper ?? new Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>()
            {Value = new Capnproto_test.Capnp.Test.TestGenerics<string, Capnproto_test.Capnp.Test.TestAllTypes>()
            {Foo = "text", Rev = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 321, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Rev = new Capnproto_test.Capnp.Test.TestGenerics<string, Capnproto_test.Capnp.Test.TestAllTypes>()
            {}, List = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner[]{}}, List = new Capnproto_test.Capnp.Test.TestGenerics<string, Capnproto_test.Capnp.Test.TestAllTypes>.Inner[]{}}};
            DefaultWrapper2 = DefaultWrapper2 ?? new Capnproto_test.Capnp.Test.TestGenericsWrapper2()
            {Value = new Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>()
            {Value = new Capnproto_test.Capnp.Test.TestGenerics<string, Capnproto_test.Capnp.Test.TestAllTypes>()
            {Foo = "text", Rev = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 321, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Rev = new Capnproto_test.Capnp.Test.TestGenerics<string, Capnproto_test.Capnp.Test.TestAllTypes>()
            {}, List = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner[]{}}, List = new Capnproto_test.Capnp.Test.TestGenerics<string, Capnproto_test.Capnp.Test.TestAllTypes>.Inner[]{}}}};
            AliasFoo = AliasFoo ?? new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0};
            AliasInner = AliasInner ?? new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Bar = new Capnproto_test.Capnp.Test.TestAnyPointer()
            {}};
            AliasInner2 = AliasInner2 ?? new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<object>()
            {Bar = new Capnproto_test.Capnp.Test.TestAnyPointer()
            {}, InnerBound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Bar = new Capnproto_test.Capnp.Test.TestAnyPointer()
            {}}, InnerUnbound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner()
            {}};
            AliasInner2Bind = AliasInner2Bind ?? new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<IReadOnlyList<uint>>()
            {Bar = new Capnproto_test.Capnp.Test.TestAnyPointer()
            {}, Baz = new uint[]{12U, 34U}, InnerBound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Bar = new Capnproto_test.Capnp.Test.TestAnyPointer()
            {}}, InnerUnbound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner()
            {}};
            AliasInner2Text = AliasInner2Text ?? new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>()
            {Bar = new Capnproto_test.Capnp.Test.TestAnyPointer()
            {}, Baz = "text", InnerBound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Bar = new Capnproto_test.Capnp.Test.TestAnyPointer()
            {}}, InnerUnbound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner()
            {}};
            AliasRev = AliasRev ?? "text";
            UseAliases = UseAliases ?? new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.UseAliases()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Inner = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.Inner()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Bar = new uint[]{}}, Inner2 = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.Inner2<object>()
            {Bar = new uint[]{}, InnerBound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.Inner()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Bar = new uint[]{}}, InnerUnbound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.Inner()
            {}}, Inner2Bind = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.Inner2<string>()
            {Bar = new uint[]{}, Baz = "text", InnerBound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.Inner()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Bar = new uint[]{}}, InnerUnbound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.Inner()
            {}}, Inner2Text = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.Inner2<string>()
            {Bar = new uint[]{}, Baz = "text", InnerBound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.Inner()
            {Foo = new Capnproto_test.Capnp.Test.TestAllTypes()
            {BoolField = false, Int8Field = 0, Int16Field = 123, Int32Field = 0, Int64Field = 0L, UInt8Field = 0, UInt16Field = 0, UInt32Field = 0U, UInt64Field = 0UL, Float32Field = 0F, Float64Field = 0, TextField = null, DataField = new byte[]{}, StructField = new Capnproto_test.Capnp.Test.TestAllTypes()
            {}, EnumField = Capnproto_test.Capnp.Test.TestEnum.foo, VoidList = 0, BoolList = new bool[]{}, Int8List = new sbyte[]{}, Int16List = new short[]{}, Int32List = new int[]{}, Int64List = new long[]{}, UInt8List = new byte[]{}, UInt16List = new ushort[]{}, UInt32List = new uint[]{}, UInt64List = new ulong[]{}, Float32List = new float[]{}, Float64List = new double[]{}, TextList = new string[]{}, DataList = new IReadOnlyList<byte>[]{}, StructList = new Capnproto_test.Capnp.Test.TestAllTypes[]{}, EnumList = new Capnproto_test.Capnp.Test.TestEnum[]{}, InterfaceList = 0}, Bar = new uint[]{}}, InnerUnbound = new Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.Inner()
            {}}, RevFoo = new uint[]{12U, 34U, 56U}};
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer> Basic
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner Inner
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string> Inner2
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<object, object> Unspecified
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<object, object>.Inner2<string> UnspecifiedInner
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string> Default
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner DefaultInner
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestUseGenerics DefaultUser
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenericsWrapper<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer> Wrapper
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes> DefaultWrapper
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenericsWrapper2 DefaultWrapper2
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestAllTypes AliasFoo
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner AliasInner
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<object> AliasInner2
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<IReadOnlyList<uint>> AliasInner2Bind
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string> AliasInner2Text
        {
            get;
            set;
        }

        public string AliasRev
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.UseAliases UseAliases
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.ITestInterface, string> Cap
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.IInterface<IReadOnlyList<byte>> GenericCap
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.READER Basic => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner.READER Inner => ctx.ReadStruct(1, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>.READER Inner2 => ctx.ReadStruct(2, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<object, object>.READER Unspecified => ctx.ReadStruct(3, Capnproto_test.Capnp.Test.TestGenerics<object, object>.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<object, object>.Inner2<string>.READER UnspecifiedInner => ctx.ReadStruct(4, Capnproto_test.Capnp.Test.TestGenerics<object, object>.Inner2<string>.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.READER Default => ctx.ReadStruct(5, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner.READER DefaultInner => ctx.ReadStruct(6, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner.READER.create);
            public Capnproto_test.Capnp.Test.TestUseGenerics.READER DefaultUser => ctx.ReadStruct(7, Capnproto_test.Capnp.Test.TestUseGenerics.READER.create);
            public Capnproto_test.Capnp.Test.TestGenericsWrapper<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.READER Wrapper => ctx.ReadStruct(8, Capnproto_test.Capnp.Test.TestGenericsWrapper<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.READER.create);
            public Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>.READER DefaultWrapper => ctx.ReadStruct(9, Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>.READER.create);
            public Capnproto_test.Capnp.Test.TestGenericsWrapper2.READER DefaultWrapper2 => ctx.ReadStruct(10, Capnproto_test.Capnp.Test.TestGenericsWrapper2.READER.create);
            public Capnproto_test.Capnp.Test.TestAllTypes.READER AliasFoo => ctx.ReadStruct(11, Capnproto_test.Capnp.Test.TestAllTypes.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner.READER AliasInner => ctx.ReadStruct(12, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<object>.READER AliasInner2 => ctx.ReadStruct(13, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<object>.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<IReadOnlyList<uint>>.READER AliasInner2Bind => ctx.ReadStruct(14, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<IReadOnlyList<uint>>.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>.READER AliasInner2Text => ctx.ReadStruct(15, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>.READER.create);
            public string AliasRev => ctx.ReadText(16, "text");
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.UseAliases.READER UseAliases => ctx.ReadStruct(17, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.UseAliases.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.ITestInterface, string>.READER Cap => ctx.ReadStruct(18, Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.ITestInterface, string>.READER.create);
            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.IInterface<IReadOnlyList<byte>> GenericCap => ctx.ReadCap<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.IInterface<IReadOnlyList<byte>>>(19);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 20);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.WRITER Basic
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.WRITER>(0);
                set => Link(0, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner.WRITER Inner
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner.WRITER>(1);
                set => Link(1, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>.WRITER Inner2
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>.WRITER>(2);
                set => Link(2, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<object, object>.WRITER Unspecified
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<object, object>.WRITER>(3);
                set => Link(3, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<object, object>.Inner2<string>.WRITER UnspecifiedInner
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<object, object>.Inner2<string>.WRITER>(4);
                set => Link(4, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.WRITER Default
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.WRITER>(5);
                set => Link(5, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner.WRITER DefaultInner
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, string>.Inner.WRITER>(6);
                set => Link(6, value);
            }

            public Capnproto_test.Capnp.Test.TestUseGenerics.WRITER DefaultUser
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestUseGenerics.WRITER>(7);
                set => Link(7, value);
            }

            public Capnproto_test.Capnp.Test.TestGenericsWrapper<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.WRITER Wrapper
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenericsWrapper<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.WRITER>(8);
                set => Link(8, value);
            }

            public Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>.WRITER DefaultWrapper
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenericsWrapper<string, Capnproto_test.Capnp.Test.TestAllTypes>.WRITER>(9);
                set => Link(9, value);
            }

            public Capnproto_test.Capnp.Test.TestGenericsWrapper2.WRITER DefaultWrapper2
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenericsWrapper2.WRITER>(10);
                set => Link(10, value);
            }

            public Capnproto_test.Capnp.Test.TestAllTypes.WRITER AliasFoo
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>(11);
                set => Link(11, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner.WRITER AliasInner
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner.WRITER>(12);
                set => Link(12, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<object>.WRITER AliasInner2
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<object>.WRITER>(13);
                set => Link(13, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<IReadOnlyList<uint>>.WRITER AliasInner2Bind
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<IReadOnlyList<uint>>.WRITER>(14);
                set => Link(14, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>.WRITER AliasInner2Text
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, Capnproto_test.Capnp.Test.TestAnyPointer>.Inner2<string>.WRITER>(15);
                set => Link(15, value);
            }

            public string AliasRev
            {
                get => this.ReadText(16, "text");
                set => this.WriteText(16, value, "text");
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.UseAliases.WRITER UseAliases
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.UseAliases.WRITER>(17);
                set => Link(17, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.ITestInterface, string>.WRITER Cap
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.ITestInterface, string>.WRITER>(18);
                set => Link(18, value);
            }

            public Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.IInterface<IReadOnlyList<byte>> GenericCap
            {
                get => ReadCap<Capnproto_test.Capnp.Test.TestGenerics<Capnproto_test.Capnp.Test.TestAllTypes, IReadOnlyList<uint>>.IInterface<IReadOnlyList<byte>>>(19);
                set => LinkObject(19, value);
            }
        }
    }

    [TypeId(0xc5598844441096dcUL)]
    public class TestEmptyStruct : ICapnpSerializable
    {
        public const UInt64 typeId = 0xc5598844441096dcUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 0);
            }
        }
    }

    [TypeId(0xabed745cd8c92095UL)]
    public class TestConstants : ICapnpSerializable
    {
        public const UInt64 typeId = 0xabed745cd8c92095UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 0);
            }
        }
    }

    [TypeId(0xddc280dbee9c99b3UL)]
    public class TestAnyPointerConstants : ICapnpSerializable
    {
        public const UInt64 typeId = 0xddc280dbee9c99b3UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            AnyKindAsStruct = CapnpSerializable.Create<object>(reader.AnyKindAsStruct);
            AnyStructAsStruct = CapnpSerializable.Create<object>(reader.AnyStructAsStruct);
            AnyKindAsList = CapnpSerializable.Create<object>(reader.AnyKindAsList);
            AnyListAsList = reader.AnyListAsList?.ToReadOnlyList(_ => (object)_);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.AnyKindAsStruct.SetObject(AnyKindAsStruct);
            writer.AnyStructAsStruct.SetObject(AnyStructAsStruct);
            writer.AnyKindAsList.SetObject(AnyKindAsList);
            writer.AnyListAsList.SetObject(AnyListAsList);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public object AnyKindAsStruct
        {
            get;
            set;
        }

        public object AnyStructAsStruct
        {
            get;
            set;
        }

        public object AnyKindAsList
        {
            get;
            set;
        }

        public IReadOnlyList<object> AnyListAsList
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public DeserializerState AnyKindAsStruct => ctx.StructReadPointer(0);
            public DeserializerState AnyStructAsStruct => ctx.StructReadPointer(1);
            public DeserializerState AnyKindAsList => ctx.StructReadPointer(2);
            public IReadOnlyList<DeserializerState> AnyListAsList => (IReadOnlyList<DeserializerState>)ctx.ReadList(3);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 4);
            }

            public DynamicSerializerState AnyKindAsStruct
            {
                get => BuildPointer<DynamicSerializerState>(0);
                set => Link(0, value);
            }

            public DynamicSerializerState AnyStructAsStruct
            {
                get => BuildPointer<DynamicSerializerState>(1);
                set => Link(1, value);
            }

            public DynamicSerializerState AnyKindAsList
            {
                get => BuildPointer<DynamicSerializerState>(2);
                set => Link(2, value);
            }

            public DynamicSerializerState AnyListAsList
            {
                get => BuildPointer<DynamicSerializerState>(3);
                set => Link(3, value);
            }
        }
    }

    [TypeId(0x88eb12a0e0af92b2UL), Proxy(typeof(TestInterface_Proxy)), Skeleton(typeof(TestInterface_Skeleton))]
    public interface ITestInterface : IDisposable
    {
        Task<string> Foo(uint I, bool J, CancellationToken cancellationToken_ = default);
        Task Bar(CancellationToken cancellationToken_ = default);
        Task Baz(Capnproto_test.Capnp.Test.TestAllTypes S, CancellationToken cancellationToken_ = default);
    }

    public class TestInterface_Proxy : Proxy, ITestInterface
    {
        public async Task<string> Foo(uint I, bool J, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Params_Foo.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestInterface.Params_Foo()
            {I = I, J = J};
            arg_?.serialize(in_);
            using (var d_ = await Call(9865999890858873522UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Result_Foo>(d_);
                return (r_.X);
            }
        }

        public async Task Bar(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Params_Bar.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestInterface.Params_Bar()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(9865999890858873522UL, 1, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Result_Bar>(d_);
                return;
            }
        }

        public async Task Baz(Capnproto_test.Capnp.Test.TestAllTypes S, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Params_Baz.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestInterface.Params_Baz()
            {S = S};
            arg_?.serialize(in_);
            using (var d_ = await Call(9865999890858873522UL, 2, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Result_Baz>(d_);
                return;
            }
        }
    }

    public class TestInterface_Skeleton : Skeleton<ITestInterface>
    {
        public TestInterface_Skeleton()
        {
            SetMethodTable(Foo, Bar, Baz);
        }

        public override ulong InterfaceId => 9865999890858873522UL;
        Task<AnswerOrCounterquestion> Foo(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Params_Foo>(d_);
                return Impatient.MaybeTailCall(Impl.Foo(in_.I, in_.J, cancellationToken_), x =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Result_Foo.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestInterface.Result_Foo{X = x};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        async Task<AnswerOrCounterquestion> Bar(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                await Impl.Bar(cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Result_Bar.WRITER>();
                return s_;
            }
        }

        async Task<AnswerOrCounterquestion> Baz(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Params_Baz>(d_);
                await Impl.Baz(in_.S, cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Result_Baz.WRITER>();
                return s_;
            }
        }
    }

    public static class TestInterface
    {
        [TypeId(0xb874edc0d559b391UL)]
        public class Params_Foo : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb874edc0d559b391UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                I = reader.I;
                J = reader.J;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.I = I;
                writer.J = J;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint I
            {
                get;
                set;
            }

            public bool J
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public uint I => ctx.ReadDataUInt(0UL, 0U);
                public bool J => ctx.ReadDataBool(32UL, false);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public uint I
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }

                public bool J
                {
                    get => this.ReadDataBool(32UL, false);
                    set => this.WriteData(32UL, value, false);
                }
            }
        }

        [TypeId(0xb04fcaddab714ba4UL)]
        public class Result_Foo : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb04fcaddab714ba4UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                X = reader.X;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.X = X;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string X
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string X => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string X
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0xd044893357b42568UL)]
        public class Params_Bar : ICapnpSerializable
        {
            public const UInt64 typeId = 0xd044893357b42568UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0x9bf141df4247d52fUL)]
        public class Result_Bar : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9bf141df4247d52fUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xd9ac8abb2a91cfbcUL)]
        public class Params_Baz : ICapnpSerializable
        {
            public const UInt64 typeId = 0xd9ac8abb2a91cfbcUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                S = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(reader.S);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                S?.serialize(writer.S);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestAllTypes S
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.TestAllTypes.READER S => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestAllTypes.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.TestAllTypes.WRITER S
                {
                    get => BuildPointer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>(0);
                    set => Link(0, value);
                }
            }
        }

        [TypeId(0x9b99d14f2f375b2dUL)]
        public class Result_Baz : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9b99d14f2f375b2dUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }
    }

    [TypeId(0xe4e9bac98670b748UL), Proxy(typeof(TestExtends_Proxy)), Skeleton(typeof(TestExtends_Skeleton))]
    public interface ITestExtends : Capnproto_test.Capnp.Test.ITestInterface
    {
        Task Qux(CancellationToken cancellationToken_ = default);
        Task Corge(Capnproto_test.Capnp.Test.TestAllTypes arg_, CancellationToken cancellationToken_ = default);
        Task<Capnproto_test.Capnp.Test.TestAllTypes> Grault(CancellationToken cancellationToken_ = default);
    }

    public class TestExtends_Proxy : Proxy, ITestExtends
    {
        public async Task Qux(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestExtends.Params_Qux.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestExtends.Params_Qux()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(16494920484927878984UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestExtends.Result_Qux>(d_);
                return;
            }
        }

        public async Task Corge(Capnproto_test.Capnp.Test.TestAllTypes arg_, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>();
            arg_?.serialize(in_);
            using (var d_ = await Call(16494920484927878984UL, 1, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestExtends.Result_Corge>(d_);
                return;
            }
        }

        public async Task<Capnproto_test.Capnp.Test.TestAllTypes> Grault(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestExtends.Params_Grault.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestExtends.Params_Grault()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(16494920484927878984UL, 2, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(d_);
                return r_;
            }
        }

        public async Task<string> Foo(uint I, bool J, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Params_Foo.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestInterface.Params_Foo()
            {I = I, J = J};
            arg_?.serialize(in_);
            using (var d_ = await Call(9865999890858873522UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Result_Foo>(d_);
                return (r_.X);
            }
        }

        public async Task Bar(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Params_Bar.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestInterface.Params_Bar()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(9865999890858873522UL, 1, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Result_Bar>(d_);
                return;
            }
        }

        public async Task Baz(Capnproto_test.Capnp.Test.TestAllTypes S, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Params_Baz.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestInterface.Params_Baz()
            {S = S};
            arg_?.serialize(in_);
            using (var d_ = await Call(9865999890858873522UL, 2, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Result_Baz>(d_);
                return;
            }
        }
    }

    public class TestExtends_Skeleton : Skeleton<ITestExtends>
    {
        public TestExtends_Skeleton()
        {
            SetMethodTable(Qux, Corge, Grault);
        }

        public override ulong InterfaceId => 16494920484927878984UL;
        async Task<AnswerOrCounterquestion> Qux(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                await Impl.Qux(cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestExtends.Result_Qux.WRITER>();
                return s_;
            }
        }

        async Task<AnswerOrCounterquestion> Corge(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                await Impl.Corge(CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(d_), cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestExtends.Result_Corge.WRITER>();
                return s_;
            }
        }

        Task<AnswerOrCounterquestion> Grault(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                return Impatient.MaybeTailCall(Impl.Grault(cancellationToken_), r_ =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>();
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }
    }

    public static class TestExtends
    {
        [TypeId(0x83a4bc5471363f17UL)]
        public class Params_Qux : ICapnpSerializable
        {
            public const UInt64 typeId = 0x83a4bc5471363f17UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0x8e4b3d1a3e2753ddUL)]
        public class Result_Qux : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8e4b3d1a3e2753ddUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xacf67532a7e7bad9UL)]
        public class Result_Corge : ICapnpSerializable
        {
            public const UInt64 typeId = 0xacf67532a7e7bad9UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xf3b834e851ea8af6UL)]
        public class Params_Grault : ICapnpSerializable
        {
            public const UInt64 typeId = 0xf3b834e851ea8af6UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }
    }

    [TypeId(0x98d7e0ef61488783UL), Proxy(typeof(TestExtends2_Proxy)), Skeleton(typeof(TestExtends2_Skeleton))]
    public interface ITestExtends2 : Capnproto_test.Capnp.Test.ITestExtends
    {
    }

    public class TestExtends2_Proxy : Proxy, ITestExtends2
    {
        public async Task Qux(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestExtends.Params_Qux.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestExtends.Params_Qux()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(16494920484927878984UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestExtends.Result_Qux>(d_);
                return;
            }
        }

        public async Task Corge(Capnproto_test.Capnp.Test.TestAllTypes arg_, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>();
            arg_?.serialize(in_);
            using (var d_ = await Call(16494920484927878984UL, 1, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestExtends.Result_Corge>(d_);
                return;
            }
        }

        public async Task<Capnproto_test.Capnp.Test.TestAllTypes> Grault(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestExtends.Params_Grault.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestExtends.Params_Grault()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(16494920484927878984UL, 2, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(d_);
                return r_;
            }
        }

        public async Task<string> Foo(uint I, bool J, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Params_Foo.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestInterface.Params_Foo()
            {I = I, J = J};
            arg_?.serialize(in_);
            using (var d_ = await Call(9865999890858873522UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Result_Foo>(d_);
                return (r_.X);
            }
        }

        public async Task Bar(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Params_Bar.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestInterface.Params_Bar()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(9865999890858873522UL, 1, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Result_Bar>(d_);
                return;
            }
        }

        public async Task Baz(Capnproto_test.Capnp.Test.TestAllTypes S, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestInterface.Params_Baz.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestInterface.Params_Baz()
            {S = S};
            arg_?.serialize(in_);
            using (var d_ = await Call(9865999890858873522UL, 2, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestInterface.Result_Baz>(d_);
                return;
            }
        }
    }

    public class TestExtends2_Skeleton : Skeleton<ITestExtends2>
    {
        public TestExtends2_Skeleton()
        {
            SetMethodTable();
        }

        public override ulong InterfaceId => 11013518732491786115UL;
    }

    [TypeId(0xa5a404caa61d4cd0UL), Proxy(typeof(TestPipeline_Proxy)), Skeleton(typeof(TestPipeline_Skeleton))]
    public interface ITestPipeline : IDisposable
    {
        Task<(string, Capnproto_test.Capnp.Test.TestPipeline.Box)> GetCap(uint N, Capnproto_test.Capnp.Test.ITestInterface InCap, CancellationToken cancellationToken_ = default);
        Task TestPointers(Capnproto_test.Capnp.Test.ITestInterface Cap, object Obj, IReadOnlyList<Capnproto_test.Capnp.Test.ITestInterface> List, CancellationToken cancellationToken_ = default);
        Task<(string, Capnproto_test.Capnp.Test.TestPipeline.AnyBox)> GetAnyCap(uint N, BareProxy InCap, CancellationToken cancellationToken_ = default);
    }

    public class TestPipeline_Proxy : Proxy, ITestPipeline
    {
        public Task<(string, Capnproto_test.Capnp.Test.TestPipeline.Box)> GetCap(uint N, Capnproto_test.Capnp.Test.ITestInterface InCap, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestPipeline.Params_GetCap.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestPipeline.Params_GetCap()
            {N = N, InCap = InCap};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(11935670180855499984UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestPipeline.Result_GetCap>(d_);
                    return (r_.S, r_.OutBox);
                }
            }

            );
        }

        public async Task TestPointers(Capnproto_test.Capnp.Test.ITestInterface Cap, object Obj, IReadOnlyList<Capnproto_test.Capnp.Test.ITestInterface> List, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestPipeline.Params_TestPointers.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestPipeline.Params_TestPointers()
            {Cap = Cap, Obj = Obj, List = List};
            arg_?.serialize(in_);
            using (var d_ = await Call(11935670180855499984UL, 1, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestPipeline.Result_TestPointers>(d_);
                return;
            }
        }

        public Task<(string, Capnproto_test.Capnp.Test.TestPipeline.AnyBox)> GetAnyCap(uint N, BareProxy InCap, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestPipeline.Params_GetAnyCap.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestPipeline.Params_GetAnyCap()
            {N = N, InCap = InCap};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(11935670180855499984UL, 2, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestPipeline.Result_GetAnyCap>(d_);
                    return (r_.S, r_.OutBox);
                }
            }

            );
        }
    }

    public class TestPipeline_Skeleton : Skeleton<ITestPipeline>
    {
        public TestPipeline_Skeleton()
        {
            SetMethodTable(GetCap, TestPointers, GetAnyCap);
        }

        public override ulong InterfaceId => 11935670180855499984UL;
        Task<AnswerOrCounterquestion> GetCap(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestPipeline.Params_GetCap>(d_);
                return Impatient.MaybeTailCall(Impl.GetCap(in_.N, in_.InCap, cancellationToken_), (s, outBox) =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestPipeline.Result_GetCap.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestPipeline.Result_GetCap{S = s, OutBox = outBox};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        async Task<AnswerOrCounterquestion> TestPointers(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestPipeline.Params_TestPointers>(d_);
                await Impl.TestPointers(in_.Cap, in_.Obj, in_.List, cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestPipeline.Result_TestPointers.WRITER>();
                return s_;
            }
        }

        Task<AnswerOrCounterquestion> GetAnyCap(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestPipeline.Params_GetAnyCap>(d_);
                return Impatient.MaybeTailCall(Impl.GetAnyCap(in_.N, in_.InCap, cancellationToken_), (s, outBox) =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestPipeline.Result_GetAnyCap.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestPipeline.Result_GetAnyCap{S = s, OutBox = outBox};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }
    }

    public static class TestPipeline
    {
        [TypeId(0xb0b29e51db0e26b1UL)]
        public class Box : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb0b29e51db0e26b1UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestInterface Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestInterface Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestInterface Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0x9442ad5a1d2c8acbUL)]
        public class AnyBox : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9442ad5a1d2c8acbUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public BareProxy Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public BareProxy Cap => ctx.ReadCap(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public BareProxy Cap
                {
                    get => ReadCap<BareProxy>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xc7e8df5096257034UL)]
        public class Params_GetCap : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc7e8df5096257034UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                N = reader.N;
                InCap = reader.InCap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.N = N;
                writer.InCap = InCap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint N
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.ITestInterface InCap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public uint N => ctx.ReadDataUInt(0UL, 0U);
                public Capnproto_test.Capnp.Test.ITestInterface InCap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public uint N
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }

                public Capnproto_test.Capnp.Test.ITestInterface InCap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xb2442a9e0ba28fdfUL)]
        public class Result_GetCap : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb2442a9e0ba28fdfUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                S = reader.S;
                OutBox = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestPipeline.Box>(reader.OutBox);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.S = S;
                OutBox?.serialize(writer.OutBox);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string S
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.TestPipeline.Box OutBox
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string S => ctx.ReadText(0, null);
                public Capnproto_test.Capnp.Test.TestPipeline.Box.READER OutBox => ctx.ReadStruct(1, Capnproto_test.Capnp.Test.TestPipeline.Box.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public string S
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public Capnproto_test.Capnp.Test.TestPipeline.Box.WRITER OutBox
                {
                    get => BuildPointer<Capnproto_test.Capnp.Test.TestPipeline.Box.WRITER>(1);
                    set => Link(1, value);
                }
            }
        }

        [TypeId(0xa604ee63cf37819fUL)]
        public class Params_TestPointers : ICapnpSerializable
        {
            public const UInt64 typeId = 0xa604ee63cf37819fUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                Obj = CapnpSerializable.Create<object>(reader.Obj);
                List = reader.List;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
                writer.Obj.SetObject(Obj);
                writer.List.Init(List);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestInterface Cap
            {
                get;
                set;
            }

            public object Obj
            {
                get;
                set;
            }

            public IReadOnlyList<Capnproto_test.Capnp.Test.ITestInterface> List
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestInterface Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                public DeserializerState Obj => ctx.StructReadPointer(1);
                public IReadOnlyList<Capnproto_test.Capnp.Test.ITestInterface> List => ctx.ReadCapList<Capnproto_test.Capnp.Test.ITestInterface>(2);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 3);
                }

                public Capnproto_test.Capnp.Test.ITestInterface Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                    set => LinkObject(0, value);
                }

                public DynamicSerializerState Obj
                {
                    get => BuildPointer<DynamicSerializerState>(1);
                    set => Link(1, value);
                }

                public ListOfCapsSerializer<Capnproto_test.Capnp.Test.ITestInterface> List
                {
                    get => BuildPointer<ListOfCapsSerializer<Capnproto_test.Capnp.Test.ITestInterface>>(2);
                    set => Link(2, value);
                }
            }
        }

        [TypeId(0x8eda54756c6070d6UL)]
        public class Result_TestPointers : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8eda54756c6070d6UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xf8e36b53ab093d4eUL)]
        public class Params_GetAnyCap : ICapnpSerializable
        {
            public const UInt64 typeId = 0xf8e36b53ab093d4eUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                N = reader.N;
                InCap = reader.InCap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.N = N;
                writer.InCap = InCap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint N
            {
                get;
                set;
            }

            public BareProxy InCap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public uint N => ctx.ReadDataUInt(0UL, 0U);
                public BareProxy InCap => ctx.ReadCap(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public uint N
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }

                public BareProxy InCap
                {
                    get => ReadCap<BareProxy>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xbf44b4c94c26ef79UL)]
        public class Result_GetAnyCap : ICapnpSerializable
        {
            public const UInt64 typeId = 0xbf44b4c94c26ef79UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                S = reader.S;
                OutBox = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestPipeline.AnyBox>(reader.OutBox);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.S = S;
                OutBox?.serialize(writer.OutBox);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string S
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.TestPipeline.AnyBox OutBox
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string S => ctx.ReadText(0, null);
                public Capnproto_test.Capnp.Test.TestPipeline.AnyBox.READER OutBox => ctx.ReadStruct(1, Capnproto_test.Capnp.Test.TestPipeline.AnyBox.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public string S
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public Capnproto_test.Capnp.Test.TestPipeline.AnyBox.WRITER OutBox
                {
                    get => BuildPointer<Capnproto_test.Capnp.Test.TestPipeline.AnyBox.WRITER>(1);
                    set => Link(1, value);
                }
            }
        }
    }

    [TypeId(0xa0e77035bdff0051UL), Proxy(typeof(TestCallOrder_Proxy)), Skeleton(typeof(TestCallOrder_Skeleton))]
    public interface ITestCallOrder : IDisposable
    {
        Task<uint> GetCallSequence(uint Expected, CancellationToken cancellationToken_ = default);
    }

    public class TestCallOrder_Proxy : Proxy, ITestCallOrder
    {
        public async Task<uint> GetCallSequence(uint Expected, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestCallOrder.Params_GetCallSequence.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestCallOrder.Params_GetCallSequence()
            {Expected = Expected};
            arg_?.serialize(in_);
            using (var d_ = await Call(11594359141811814481UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestCallOrder.Result_GetCallSequence>(d_);
                return (r_.N);
            }
        }
    }

    public class TestCallOrder_Skeleton : Skeleton<ITestCallOrder>
    {
        public TestCallOrder_Skeleton()
        {
            SetMethodTable(GetCallSequence);
        }

        public override ulong InterfaceId => 11594359141811814481UL;
        Task<AnswerOrCounterquestion> GetCallSequence(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestCallOrder.Params_GetCallSequence>(d_);
                return Impatient.MaybeTailCall(Impl.GetCallSequence(in_.Expected, cancellationToken_), n =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestCallOrder.Result_GetCallSequence.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestCallOrder.Result_GetCallSequence{N = n};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }
    }

    public static class TestCallOrder
    {
        [TypeId(0x8f1e8cd56ceb74dcUL)]
        public class Params_GetCallSequence : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8f1e8cd56ceb74dcUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Expected = reader.Expected;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Expected = Expected;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint Expected
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public uint Expected => ctx.ReadDataUInt(0UL, 0U);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public uint Expected
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }
            }
        }

        [TypeId(0xdedbb6bf3810eab7UL)]
        public class Result_GetCallSequence : ICapnpSerializable
        {
            public const UInt64 typeId = 0xdedbb6bf3810eab7UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                N = reader.N;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.N = N;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint N
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public uint N => ctx.ReadDataUInt(0UL, 0U);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public uint N
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }
            }
        }
    }

    [TypeId(0xddd699207eb8e23bUL), Proxy(typeof(TestTailCallee_Proxy)), Skeleton(typeof(TestTailCallee_Skeleton))]
    public interface ITestTailCallee : IDisposable
    {
        Task<Capnproto_test.Capnp.Test.TestTailCallee.TailResult> Foo(int I, string T, CancellationToken cancellationToken_ = default);
    }

    public class TestTailCallee_Proxy : Proxy, ITestTailCallee
    {
        public Task<Capnproto_test.Capnp.Test.TestTailCallee.TailResult> Foo(int I, string T, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestTailCallee.Params_Foo.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestTailCallee.Params_Foo()
            {I = I, T = T};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(15985132292242203195UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestTailCallee.TailResult>(d_);
                    return r_;
                }
            }

            );
        }
    }

    public class TestTailCallee_Skeleton : Skeleton<ITestTailCallee>
    {
        public TestTailCallee_Skeleton()
        {
            SetMethodTable(Foo);
        }

        public override ulong InterfaceId => 15985132292242203195UL;
        Task<AnswerOrCounterquestion> Foo(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestTailCallee.Params_Foo>(d_);
                return Impatient.MaybeTailCall(Impl.Foo(in_.I, in_.T, cancellationToken_), r_ =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestTailCallee.TailResult.WRITER>();
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }
    }

    public static class TestTailCallee
    {
        [TypeId(0xa9ed2e5a9fd53d19UL)]
        public class TailResult : ICapnpSerializable
        {
            public const UInt64 typeId = 0xa9ed2e5a9fd53d19UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                I = reader.I;
                T = reader.T;
                C = reader.C;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.I = I;
                writer.T = T;
                writer.C = C;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint I
            {
                get;
                set;
            }

            public string T
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.ITestCallOrder C
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public uint I => ctx.ReadDataUInt(0UL, 0U);
                public string T => ctx.ReadText(0, null);
                public Capnproto_test.Capnp.Test.ITestCallOrder C => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestCallOrder>(1);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 2);
                }

                public uint I
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }

                public string T
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public Capnproto_test.Capnp.Test.ITestCallOrder C
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestCallOrder>(1);
                    set => LinkObject(1, value);
                }
            }
        }

        [TypeId(0xc5e1efc325614957UL)]
        public class Params_Foo : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc5e1efc325614957UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                I = reader.I;
                T = reader.T;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.I = I;
                writer.T = T;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public int I
            {
                get;
                set;
            }

            public string T
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public int I => ctx.ReadDataInt(0UL, 0);
                public string T => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public int I
                {
                    get => this.ReadDataInt(0UL, 0);
                    set => this.WriteData(0UL, value, 0);
                }

                public string T
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }
    }

    [TypeId(0x870bf40110ce3035UL), Proxy(typeof(TestTailCaller_Proxy)), Skeleton(typeof(TestTailCaller_Skeleton))]
    public interface ITestTailCaller : IDisposable
    {
        Task<Capnproto_test.Capnp.Test.TestTailCallee.TailResult> Foo(int I, Capnproto_test.Capnp.Test.ITestTailCallee Callee, CancellationToken cancellationToken_ = default);
    }

    public class TestTailCaller_Proxy : Proxy, ITestTailCaller
    {
        public Task<Capnproto_test.Capnp.Test.TestTailCallee.TailResult> Foo(int I, Capnproto_test.Capnp.Test.ITestTailCallee Callee, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestTailCaller.Params_Foo.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestTailCaller.Params_Foo()
            {I = I, Callee = Callee};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(9731139705278181429UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestTailCallee.TailResult>(d_);
                    return r_;
                }
            }

            );
        }
    }

    public class TestTailCaller_Skeleton : Skeleton<ITestTailCaller>
    {
        public TestTailCaller_Skeleton()
        {
            SetMethodTable(Foo);
        }

        public override ulong InterfaceId => 9731139705278181429UL;
        Task<AnswerOrCounterquestion> Foo(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestTailCaller.Params_Foo>(d_);
                return Impatient.MaybeTailCall(Impl.Foo(in_.I, in_.Callee, cancellationToken_), r_ =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestTailCallee.TailResult.WRITER>();
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }
    }

    public static class TestTailCaller
    {
        [TypeId(0xb07a279515dc8ac5UL)]
        public class Params_Foo : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb07a279515dc8ac5UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                I = reader.I;
                Callee = reader.Callee;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.I = I;
                writer.Callee = Callee;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public int I
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.ITestTailCallee Callee
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public int I => ctx.ReadDataInt(0UL, 0);
                public Capnproto_test.Capnp.Test.ITestTailCallee Callee => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestTailCallee>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public int I
                {
                    get => this.ReadDataInt(0UL, 0);
                    set => this.WriteData(0UL, value, 0);
                }

                public Capnproto_test.Capnp.Test.ITestTailCallee Callee
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestTailCallee>(0);
                    set => LinkObject(0, value);
                }
            }
        }
    }

    [TypeId(0xa38e5efe41e53a15UL), Proxy(typeof(TestHandle_Proxy)), Skeleton(typeof(TestHandle_Skeleton))]
    public interface ITestHandle : IDisposable
    {
    }

    public class TestHandle_Proxy : Proxy, ITestHandle
    {
    }

    public class TestHandle_Skeleton : Skeleton<ITestHandle>
    {
        public TestHandle_Skeleton()
        {
            SetMethodTable();
        }

        public override ulong InterfaceId => 11785461720995412501UL;
    }

    [TypeId(0xddc70bf9784133cfUL), Proxy(typeof(TestMoreStuff_Proxy)), Skeleton(typeof(TestMoreStuff_Skeleton))]
    public interface ITestMoreStuff : Capnproto_test.Capnp.Test.ITestCallOrder
    {
        Task<string> CallFoo(Capnproto_test.Capnp.Test.ITestInterface Cap, CancellationToken cancellationToken_ = default);
        Task<string> CallFooWhenResolved(Capnproto_test.Capnp.Test.ITestInterface Cap, CancellationToken cancellationToken_ = default);
        Task<Capnproto_test.Capnp.Test.ITestInterface> NeverReturn(Capnproto_test.Capnp.Test.ITestInterface Cap, CancellationToken cancellationToken_ = default);
        Task Hold(Capnproto_test.Capnp.Test.ITestInterface Cap, CancellationToken cancellationToken_ = default);
        Task<string> CallHeld(CancellationToken cancellationToken_ = default);
        Task<Capnproto_test.Capnp.Test.ITestInterface> GetHeld(CancellationToken cancellationToken_ = default);
        Task<Capnproto_test.Capnp.Test.ITestCallOrder> Echo(Capnproto_test.Capnp.Test.ITestCallOrder Cap, CancellationToken cancellationToken_ = default);
        Task ExpectCancel(Capnproto_test.Capnp.Test.ITestInterface Cap, CancellationToken cancellationToken_ = default);
        Task<(string, string)> MethodWithDefaults(string A, uint B, string C, CancellationToken cancellationToken_ = default);
        Task<Capnproto_test.Capnp.Test.ITestHandle> GetHandle(CancellationToken cancellationToken_ = default);
        Task<Capnproto_test.Capnp.Test.ITestMoreStuff> GetNull(CancellationToken cancellationToken_ = default);
        Task<string> GetEnormousString(CancellationToken cancellationToken_ = default);
        Task MethodWithNullDefault(string A, Capnproto_test.Capnp.Test.ITestInterface B, CancellationToken cancellationToken_ = default);
    }

    public class TestMoreStuff_Proxy : Proxy, ITestMoreStuff
    {
        public async Task<string> CallFoo(Capnproto_test.Capnp.Test.ITestInterface Cap, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_CallFoo.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_CallFoo()
            {Cap = Cap};
            arg_?.serialize(in_);
            using (var d_ = await Call(15980754968839795663UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_CallFoo>(d_);
                return (r_.S);
            }
        }

        public async Task<string> CallFooWhenResolved(Capnproto_test.Capnp.Test.ITestInterface Cap, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_CallFooWhenResolved.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_CallFooWhenResolved()
            {Cap = Cap};
            arg_?.serialize(in_);
            using (var d_ = await Call(15980754968839795663UL, 1, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_CallFooWhenResolved>(d_);
                return (r_.S);
            }
        }

        public Task<Capnproto_test.Capnp.Test.ITestInterface> NeverReturn(Capnproto_test.Capnp.Test.ITestInterface Cap, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_NeverReturn.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_NeverReturn()
            {Cap = Cap};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(15980754968839795663UL, 2, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_NeverReturn>(d_);
                    return (r_.CapCopy);
                }
            }

            );
        }

        public async Task Hold(Capnproto_test.Capnp.Test.ITestInterface Cap, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_Hold.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_Hold()
            {Cap = Cap};
            arg_?.serialize(in_);
            using (var d_ = await Call(15980754968839795663UL, 3, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_Hold>(d_);
                return;
            }
        }

        public async Task<string> CallHeld(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_CallHeld.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_CallHeld()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(15980754968839795663UL, 4, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_CallHeld>(d_);
                return (r_.S);
            }
        }

        public Task<Capnproto_test.Capnp.Test.ITestInterface> GetHeld(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_GetHeld.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_GetHeld()
            {};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(15980754968839795663UL, 5, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetHeld>(d_);
                    return (r_.Cap);
                }
            }

            );
        }

        public Task<Capnproto_test.Capnp.Test.ITestCallOrder> Echo(Capnproto_test.Capnp.Test.ITestCallOrder Cap, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_Echo.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_Echo()
            {Cap = Cap};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(15980754968839795663UL, 6, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_Echo>(d_);
                    return (r_.Cap);
                }
            }

            );
        }

        public async Task ExpectCancel(Capnproto_test.Capnp.Test.ITestInterface Cap, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_ExpectCancel.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_ExpectCancel()
            {Cap = Cap};
            arg_?.serialize(in_);
            using (var d_ = await Call(15980754968839795663UL, 7, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_ExpectCancel>(d_);
                return;
            }
        }

        public async Task<(string, string)> MethodWithDefaults(string A, uint B, string C, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_MethodWithDefaults.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_MethodWithDefaults()
            {A = A, B = B, C = C};
            arg_?.serialize(in_);
            using (var d_ = await Call(15980754968839795663UL, 8, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_MethodWithDefaults>(d_);
                return (r_.D, r_.E);
            }
        }

        public Task<Capnproto_test.Capnp.Test.ITestHandle> GetHandle(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_GetHandle.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_GetHandle()
            {};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(15980754968839795663UL, 9, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetHandle>(d_);
                    return (r_.Handle);
                }
            }

            );
        }

        public Task<Capnproto_test.Capnp.Test.ITestMoreStuff> GetNull(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_GetNull.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_GetNull()
            {};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(15980754968839795663UL, 10, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetNull>(d_);
                    return (r_.NullCap);
                }
            }

            );
        }

        public async Task<string> GetEnormousString(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_GetEnormousString.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_GetEnormousString()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(15980754968839795663UL, 11, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetEnormousString>(d_);
                return (r_.Str);
            }
        }

        public async Task MethodWithNullDefault(string A, Capnproto_test.Capnp.Test.ITestInterface B, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Params_MethodWithNullDefault.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Params_MethodWithNullDefault()
            {A = A, B = B};
            arg_?.serialize(in_);
            using (var d_ = await Call(15980754968839795663UL, 12, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Result_MethodWithNullDefault>(d_);
                return;
            }
        }

        public async Task<uint> GetCallSequence(uint Expected, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestCallOrder.Params_GetCallSequence.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestCallOrder.Params_GetCallSequence()
            {Expected = Expected};
            arg_?.serialize(in_);
            using (var d_ = await Call(11594359141811814481UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestCallOrder.Result_GetCallSequence>(d_);
                return (r_.N);
            }
        }
    }

    public class TestMoreStuff_Skeleton : Skeleton<ITestMoreStuff>
    {
        public TestMoreStuff_Skeleton()
        {
            SetMethodTable(CallFoo, CallFooWhenResolved, NeverReturn, Hold, CallHeld, GetHeld, Echo, ExpectCancel, MethodWithDefaults, GetHandle, GetNull, GetEnormousString, MethodWithNullDefault);
        }

        public override ulong InterfaceId => 15980754968839795663UL;
        Task<AnswerOrCounterquestion> CallFoo(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Params_CallFoo>(d_);
                return Impatient.MaybeTailCall(Impl.CallFoo(in_.Cap, cancellationToken_), s =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_CallFoo.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Result_CallFoo{S = s};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        Task<AnswerOrCounterquestion> CallFooWhenResolved(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Params_CallFooWhenResolved>(d_);
                return Impatient.MaybeTailCall(Impl.CallFooWhenResolved(in_.Cap, cancellationToken_), s =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_CallFooWhenResolved.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Result_CallFooWhenResolved{S = s};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        Task<AnswerOrCounterquestion> NeverReturn(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Params_NeverReturn>(d_);
                return Impatient.MaybeTailCall(Impl.NeverReturn(in_.Cap, cancellationToken_), capCopy =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_NeverReturn.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Result_NeverReturn{CapCopy = capCopy};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        async Task<AnswerOrCounterquestion> Hold(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Params_Hold>(d_);
                await Impl.Hold(in_.Cap, cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_Hold.WRITER>();
                return s_;
            }
        }

        Task<AnswerOrCounterquestion> CallHeld(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                return Impatient.MaybeTailCall(Impl.CallHeld(cancellationToken_), s =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_CallHeld.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Result_CallHeld{S = s};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        Task<AnswerOrCounterquestion> GetHeld(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                return Impatient.MaybeTailCall(Impl.GetHeld(cancellationToken_), cap =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetHeld.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetHeld{Cap = cap};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        Task<AnswerOrCounterquestion> Echo(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Params_Echo>(d_);
                return Impatient.MaybeTailCall(Impl.Echo(in_.Cap, cancellationToken_), cap =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_Echo.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Result_Echo{Cap = cap};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        async Task<AnswerOrCounterquestion> ExpectCancel(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Params_ExpectCancel>(d_);
                await Impl.ExpectCancel(in_.Cap, cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_ExpectCancel.WRITER>();
                return s_;
            }
        }

        Task<AnswerOrCounterquestion> MethodWithDefaults(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Params_MethodWithDefaults>(d_);
                return Impatient.MaybeTailCall(Impl.MethodWithDefaults(in_.A, in_.B, in_.C, cancellationToken_), (d, e) =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_MethodWithDefaults.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Result_MethodWithDefaults{D = d, E = e};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        Task<AnswerOrCounterquestion> GetHandle(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                return Impatient.MaybeTailCall(Impl.GetHandle(cancellationToken_), handle =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetHandle.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetHandle{Handle = handle};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        Task<AnswerOrCounterquestion> GetNull(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                return Impatient.MaybeTailCall(Impl.GetNull(cancellationToken_), nullCap =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetNull.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetNull{NullCap = nullCap};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        Task<AnswerOrCounterquestion> GetEnormousString(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                return Impatient.MaybeTailCall(Impl.GetEnormousString(cancellationToken_), str =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetEnormousString.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMoreStuff.Result_GetEnormousString{Str = str};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        async Task<AnswerOrCounterquestion> MethodWithNullDefault(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMoreStuff.Params_MethodWithNullDefault>(d_);
                await Impl.MethodWithNullDefault(in_.A, in_.B, cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMoreStuff.Result_MethodWithNullDefault.WRITER>();
                return s_;
            }
        }
    }

    public static class TestMoreStuff
    {
        [TypeId(0x931ba418da60f6e4UL)]
        public class Params_CallFoo : ICapnpSerializable
        {
            public const UInt64 typeId = 0x931ba418da60f6e4UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestInterface Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestInterface Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestInterface Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0x9a28970beccecdd0UL)]
        public class Result_CallFoo : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9a28970beccecdd0UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                S = reader.S;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.S = S;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string S
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string S => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string S
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0xfabc700c2ebe6378UL)]
        public class Params_CallFooWhenResolved : ICapnpSerializable
        {
            public const UInt64 typeId = 0xfabc700c2ebe6378UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestInterface Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestInterface Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestInterface Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xa54ce1e9aa822f90UL)]
        public class Result_CallFooWhenResolved : ICapnpSerializable
        {
            public const UInt64 typeId = 0xa54ce1e9aa822f90UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                S = reader.S;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.S = S;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string S
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string S => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string S
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0x94fe60465c95182bUL)]
        public class Params_NeverReturn : ICapnpSerializable
        {
            public const UInt64 typeId = 0x94fe60465c95182bUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestInterface Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestInterface Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestInterface Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xdef4e5fa6999c5dcUL)]
        public class Result_NeverReturn : ICapnpSerializable
        {
            public const UInt64 typeId = 0xdef4e5fa6999c5dcUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                CapCopy = reader.CapCopy;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.CapCopy = CapCopy;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestInterface CapCopy
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestInterface CapCopy => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestInterface CapCopy
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xfe7c8fbb769d8e58UL)]
        public class Params_Hold : ICapnpSerializable
        {
            public const UInt64 typeId = 0xfe7c8fbb769d8e58UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestInterface Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestInterface Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestInterface Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xf839fb1374d003c9UL)]
        public class Result_Hold : ICapnpSerializable
        {
            public const UInt64 typeId = 0xf839fb1374d003c9UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xf8c5e5ef1edf83beUL)]
        public class Params_CallHeld : ICapnpSerializable
        {
            public const UInt64 typeId = 0xf8c5e5ef1edf83beUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xe59935f160ac7578UL)]
        public class Result_CallHeld : ICapnpSerializable
        {
            public const UInt64 typeId = 0xe59935f160ac7578UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                S = reader.S;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.S = S;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string S
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string S => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string S
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0xfeffc025fce317e3UL)]
        public class Params_GetHeld : ICapnpSerializable
        {
            public const UInt64 typeId = 0xfeffc025fce317e3UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xef4e146185af67ceUL)]
        public class Result_GetHeld : ICapnpSerializable
        {
            public const UInt64 typeId = 0xef4e146185af67ceUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestInterface Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestInterface Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestInterface Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xc07526f7e2e533b9UL)]
        public class Params_Echo : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc07526f7e2e533b9UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestCallOrder Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestCallOrder Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestCallOrder>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestCallOrder Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestCallOrder>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xa6224536593d5b92UL)]
        public class Result_Echo : ICapnpSerializable
        {
            public const UInt64 typeId = 0xa6224536593d5b92UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestCallOrder Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestCallOrder Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestCallOrder>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestCallOrder Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestCallOrder>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xa1cc32d87f3edeb1UL)]
        public class Params_ExpectCancel : ICapnpSerializable
        {
            public const UInt64 typeId = 0xa1cc32d87f3edeb1UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestInterface Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestInterface Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestInterface Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0x8a3eba1758c0916eUL)]
        public class Result_ExpectCancel : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8a3eba1758c0916eUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0x99160a25fa50fbf1UL)]
        public class Params_MethodWithDefaults : ICapnpSerializable
        {
            public const UInt64 typeId = 0x99160a25fa50fbf1UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                A = reader.A;
                B = reader.B;
                C = reader.C;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.A = A;
                writer.B = B;
                writer.C = C;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
                C = C ?? "foo";
            }

            public string A
            {
                get;
                set;
            }

            public uint B
            {
                get;
                set;
            }

            = 123U;
            public string C
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string A => ctx.ReadText(0, null);
                public uint B => ctx.ReadDataUInt(0UL, 123U);
                public string C => ctx.ReadText(1, "foo");
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 2);
                }

                public string A
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public uint B
                {
                    get => this.ReadDataUInt(0UL, 123U);
                    set => this.WriteData(0UL, value, 123U);
                }

                public string C
                {
                    get => this.ReadText(1, "foo");
                    set => this.WriteText(1, value, "foo");
                }
            }
        }

        [TypeId(0x9c7e066f845a6c56UL)]
        public class Result_MethodWithDefaults : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9c7e066f845a6c56UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                D = reader.D;
                E = reader.E;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.D = D;
                writer.E = E;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
                E = E ?? "bar";
            }

            public string D
            {
                get;
                set;
            }

            public string E
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string D => ctx.ReadText(0, null);
                public string E => ctx.ReadText(1, "bar");
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public string D
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public string E
                {
                    get => this.ReadText(1, "bar");
                    set => this.WriteText(1, value, "bar");
                }
            }
        }

        [TypeId(0xead024a301a092a1UL)]
        public class Params_GetHandle : ICapnpSerializable
        {
            public const UInt64 typeId = 0xead024a301a092a1UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xc3490d75420a1fe8UL)]
        public class Result_GetHandle : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc3490d75420a1fe8UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Handle = reader.Handle;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Handle = Handle;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestHandle Handle
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestHandle Handle => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestHandle>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestHandle Handle
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestHandle>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xd8493f0e175d61f2UL)]
        public class Params_GetNull : ICapnpSerializable
        {
            public const UInt64 typeId = 0xd8493f0e175d61f2UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xe6955d8ef1023671UL)]
        public class Result_GetNull : ICapnpSerializable
        {
            public const UInt64 typeId = 0xe6955d8ef1023671UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                NullCap = reader.NullCap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.NullCap = NullCap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.ITestMoreStuff NullCap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.ITestMoreStuff NullCap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestMoreStuff>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.ITestMoreStuff NullCap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestMoreStuff>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0x805df436f55dd07aUL)]
        public class Params_GetEnormousString : ICapnpSerializable
        {
            public const UInt64 typeId = 0x805df436f55dd07aUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0x860e7512dc3925b0UL)]
        public class Result_GetEnormousString : ICapnpSerializable
        {
            public const UInt64 typeId = 0x860e7512dc3925b0UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Str = reader.Str;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Str = Str;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Str
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string Str => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string Str
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0xfb92899aeb0ee74fUL)]
        public class Params_MethodWithNullDefault : ICapnpSerializable
        {
            public const UInt64 typeId = 0xfb92899aeb0ee74fUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                A = reader.A;
                B = reader.B;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.A = A;
                writer.B = B;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string A
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.ITestInterface B
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string A => ctx.ReadText(0, null);
                public Capnproto_test.Capnp.Test.ITestInterface B => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(1);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public string A
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public Capnproto_test.Capnp.Test.ITestInterface B
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(1);
                    set => LinkObject(1, value);
                }
            }
        }

        [TypeId(0x8467348247305cf7UL)]
        public class Result_MethodWithNullDefault : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8467348247305cf7UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }
    }

    [TypeId(0xc07d8dcd80a69c0cUL), Proxy(typeof(TestMembrane_Proxy)), Skeleton(typeof(TestMembrane_Skeleton))]
    public interface ITestMembrane : IDisposable
    {
        Task<Capnproto_test.Capnp.Test.TestMembrane.IThing> MakeThing(CancellationToken cancellationToken_ = default);
        Task<Capnproto_test.Capnp.Test.TestMembrane.Result> CallPassThrough(Capnproto_test.Capnp.Test.TestMembrane.IThing Thing, bool TailCall, CancellationToken cancellationToken_ = default);
        Task<Capnproto_test.Capnp.Test.TestMembrane.Result> CallIntercept(Capnproto_test.Capnp.Test.TestMembrane.IThing Thing, bool TailCall, CancellationToken cancellationToken_ = default);
        Task<Capnproto_test.Capnp.Test.TestMembrane.IThing> Loopback(Capnproto_test.Capnp.Test.TestMembrane.IThing Thing, CancellationToken cancellationToken_ = default);
        Task WaitForever(CancellationToken cancellationToken_ = default);
    }

    public class TestMembrane_Proxy : Proxy, ITestMembrane
    {
        public Task<Capnproto_test.Capnp.Test.TestMembrane.IThing> MakeThing(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Params_MakeThing.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMembrane.Params_MakeThing()
            {};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(13870398341137210380UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMembrane.Result_MakeThing>(d_);
                    return (r_.Thing);
                }
            }

            );
        }

        public async Task<Capnproto_test.Capnp.Test.TestMembrane.Result> CallPassThrough(Capnproto_test.Capnp.Test.TestMembrane.IThing Thing, bool TailCall, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Params_CallPassThrough.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMembrane.Params_CallPassThrough()
            {Thing = Thing, TailCall = TailCall};
            arg_?.serialize(in_);
            using (var d_ = await Call(13870398341137210380UL, 1, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMembrane.Result>(d_);
                return r_;
            }
        }

        public async Task<Capnproto_test.Capnp.Test.TestMembrane.Result> CallIntercept(Capnproto_test.Capnp.Test.TestMembrane.IThing Thing, bool TailCall, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Params_CallIntercept.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMembrane.Params_CallIntercept()
            {Thing = Thing, TailCall = TailCall};
            arg_?.serialize(in_);
            using (var d_ = await Call(13870398341137210380UL, 2, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMembrane.Result>(d_);
                return r_;
            }
        }

        public Task<Capnproto_test.Capnp.Test.TestMembrane.IThing> Loopback(Capnproto_test.Capnp.Test.TestMembrane.IThing Thing, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Params_Loopback.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMembrane.Params_Loopback()
            {Thing = Thing};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(13870398341137210380UL, 3, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMembrane.Result_Loopback>(d_);
                    return (r_.Thing);
                }
            }

            );
        }

        public async Task WaitForever(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Params_WaitForever.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestMembrane.Params_WaitForever()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(13870398341137210380UL, 4, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMembrane.Result_WaitForever>(d_);
                return;
            }
        }
    }

    public class TestMembrane_Skeleton : Skeleton<ITestMembrane>
    {
        public TestMembrane_Skeleton()
        {
            SetMethodTable(MakeThing, CallPassThrough, CallIntercept, Loopback, WaitForever);
        }

        public override ulong InterfaceId => 13870398341137210380UL;
        Task<AnswerOrCounterquestion> MakeThing(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                return Impatient.MaybeTailCall(Impl.MakeThing(cancellationToken_), thing =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Result_MakeThing.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMembrane.Result_MakeThing{Thing = thing};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        Task<AnswerOrCounterquestion> CallPassThrough(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMembrane.Params_CallPassThrough>(d_);
                return Impatient.MaybeTailCall(Impl.CallPassThrough(in_.Thing, in_.TailCall, cancellationToken_), r_ =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Result.WRITER>();
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        Task<AnswerOrCounterquestion> CallIntercept(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMembrane.Params_CallIntercept>(d_);
                return Impatient.MaybeTailCall(Impl.CallIntercept(in_.Thing, in_.TailCall, cancellationToken_), r_ =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Result.WRITER>();
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        Task<AnswerOrCounterquestion> Loopback(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMembrane.Params_Loopback>(d_);
                return Impatient.MaybeTailCall(Impl.Loopback(in_.Thing, cancellationToken_), thing =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Result_Loopback.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestMembrane.Result_Loopback{Thing = thing};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }

        async Task<AnswerOrCounterquestion> WaitForever(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                await Impl.WaitForever(cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Result_WaitForever.WRITER>();
                return s_;
            }
        }
    }

    public static class TestMembrane
    {
        [TypeId(0x9352e4e41f173917UL), Proxy(typeof(Thing_Proxy)), Skeleton(typeof(Thing_Skeleton))]
        public interface IThing : IDisposable
        {
            Task<Capnproto_test.Capnp.Test.TestMembrane.Result> PassThrough(CancellationToken cancellationToken_ = default);
            Task<Capnproto_test.Capnp.Test.TestMembrane.Result> Intercept(CancellationToken cancellationToken_ = default);
        }

        public class Thing_Proxy : Proxy, IThing
        {
            public async Task<Capnproto_test.Capnp.Test.TestMembrane.Result> PassThrough(CancellationToken cancellationToken_ = default)
            {
                var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Thing.Params_PassThrough.WRITER>();
                var arg_ = new Capnproto_test.Capnp.Test.TestMembrane.Thing.Params_PassThrough()
                {};
                arg_?.serialize(in_);
                using (var d_ = await Call(10615798940090972439UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMembrane.Result>(d_);
                    return r_;
                }
            }

            public async Task<Capnproto_test.Capnp.Test.TestMembrane.Result> Intercept(CancellationToken cancellationToken_ = default)
            {
                var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Thing.Params_Intercept.WRITER>();
                var arg_ = new Capnproto_test.Capnp.Test.TestMembrane.Thing.Params_Intercept()
                {};
                arg_?.serialize(in_);
                using (var d_ = await Call(10615798940090972439UL, 1, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestMembrane.Result>(d_);
                    return r_;
                }
            }
        }

        public class Thing_Skeleton : Skeleton<IThing>
        {
            public Thing_Skeleton()
            {
                SetMethodTable(PassThrough, Intercept);
            }

            public override ulong InterfaceId => 10615798940090972439UL;
            Task<AnswerOrCounterquestion> PassThrough(DeserializerState d_, CancellationToken cancellationToken_)
            {
                using (d_)
                {
                    return Impatient.MaybeTailCall(Impl.PassThrough(cancellationToken_), r_ =>
                    {
                        var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Result.WRITER>();
                        r_.serialize(s_);
                        return s_;
                    }

                    );
                }
            }

            Task<AnswerOrCounterquestion> Intercept(DeserializerState d_, CancellationToken cancellationToken_)
            {
                using (d_)
                {
                    return Impatient.MaybeTailCall(Impl.Intercept(cancellationToken_), r_ =>
                    {
                        var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestMembrane.Result.WRITER>();
                        r_.serialize(s_);
                        return s_;
                    }

                    );
                }
            }
        }

        public static class Thing
        {
            [TypeId(0xff9bdcd05085d786UL)]
            public class Params_PassThrough : ICapnpSerializable
            {
                public const UInt64 typeId = 0xff9bdcd05085d786UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 0);
                    }
                }
            }

            [TypeId(0xee94bed3615ee745UL)]
            public class Params_Intercept : ICapnpSerializable
            {
                public const UInt64 typeId = 0xee94bed3615ee745UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 0);
                    }
                }
            }
        }

        [TypeId(0xb0c6163faf291965UL)]
        public class Result : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb0c6163faf291965UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Text = reader.Text;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Text = Text;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Text
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string Text => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string Text
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [TypeId(0xd8ac2acc3ece6556UL)]
        public class Params_MakeThing : ICapnpSerializable
        {
            public const UInt64 typeId = 0xd8ac2acc3ece6556UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xe5d4904814ccbf29UL)]
        public class Result_MakeThing : ICapnpSerializable
        {
            public const UInt64 typeId = 0xe5d4904814ccbf29UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Thing = reader.Thing;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Thing = Thing;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing => ctx.ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0x945d9f634a6a29daUL)]
        public class Params_CallPassThrough : ICapnpSerializable
        {
            public const UInt64 typeId = 0x945d9f634a6a29daUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Thing = reader.Thing;
                TailCall = reader.TailCall;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Thing = Thing;
                writer.TailCall = TailCall;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing
            {
                get;
                set;
            }

            public bool TailCall
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing => ctx.ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
                public bool TailCall => ctx.ReadDataBool(0UL, false);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
                    set => LinkObject(0, value);
                }

                public bool TailCall
                {
                    get => this.ReadDataBool(0UL, false);
                    set => this.WriteData(0UL, value, false);
                }
            }
        }

        [TypeId(0x8749aac3375c5c71UL)]
        public class Params_CallIntercept : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8749aac3375c5c71UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Thing = reader.Thing;
                TailCall = reader.TailCall;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Thing = Thing;
                writer.TailCall = TailCall;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing
            {
                get;
                set;
            }

            public bool TailCall
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing => ctx.ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
                public bool TailCall => ctx.ReadDataBool(0UL, false);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
                    set => LinkObject(0, value);
                }

                public bool TailCall
                {
                    get => this.ReadDataBool(0UL, false);
                    set => this.WriteData(0UL, value, false);
                }
            }
        }

        [TypeId(0x869a1b7ab34b42c9UL)]
        public class Params_Loopback : ICapnpSerializable
        {
            public const UInt64 typeId = 0x869a1b7ab34b42c9UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Thing = reader.Thing;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Thing = Thing;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing => ctx.ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0xecd19398fd88ab5cUL)]
        public class Result_Loopback : ICapnpSerializable
        {
            public const UInt64 typeId = 0xecd19398fd88ab5cUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Thing = reader.Thing;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Thing = Thing;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing => ctx.ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public Capnproto_test.Capnp.Test.TestMembrane.IThing Thing
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
                    set => LinkObject(0, value);
                }
            }
        }

        [TypeId(0x8f6bb30cc62917ffUL)]
        public class Params_WaitForever : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8f6bb30cc62917ffUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xc343a4907280be01UL)]
        public class Result_WaitForever : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc343a4907280be01UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }
    }

    [TypeId(0x949449ad7c11fa5cUL)]
    public class TestContainMembrane : ICapnpSerializable
    {
        public const UInt64 typeId = 0x949449ad7c11fa5cUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Cap = reader.Cap;
            List = reader.List;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Cap = Cap;
            writer.List.Init(List);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestMembrane.IThing Cap
        {
            get;
            set;
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestMembrane.IThing> List
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public Capnproto_test.Capnp.Test.TestMembrane.IThing Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestMembrane.IThing> List => ctx.ReadCapList<Capnproto_test.Capnp.Test.TestMembrane.IThing>(1);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 2);
            }

            public Capnproto_test.Capnp.Test.TestMembrane.IThing Cap
            {
                get => ReadCap<Capnproto_test.Capnp.Test.TestMembrane.IThing>(0);
                set => LinkObject(0, value);
            }

            public ListOfCapsSerializer<Capnproto_test.Capnp.Test.TestMembrane.IThing> List
            {
                get => BuildPointer<ListOfCapsSerializer<Capnproto_test.Capnp.Test.TestMembrane.IThing>>(1);
                set => Link(1, value);
            }
        }
    }

    [TypeId(0xdd2b66a791a279f0UL)]
    public class TestTransferCap : ICapnpSerializable
    {
        public const UInt64 typeId = 0xdd2b66a791a279f0UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            List = reader.List?.ToReadOnlyList(_ => CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestTransferCap.Element>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.List.Init(List, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public IReadOnlyList<Capnproto_test.Capnp.Test.TestTransferCap.Element> List
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public IReadOnlyList<Capnproto_test.Capnp.Test.TestTransferCap.Element.READER> List => ctx.ReadList(0).Cast(Capnproto_test.Capnp.Test.TestTransferCap.Element.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestTransferCap.Element.WRITER> List
            {
                get => BuildPointer<ListOfStructsSerializer<Capnproto_test.Capnp.Test.TestTransferCap.Element.WRITER>>(0);
                set => Link(0, value);
            }
        }

        [TypeId(0xc7263e8f88844abcUL)]
        public class Element : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc7263e8f88844abcUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Text = reader.Text;
                Cap = reader.Cap;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Text = Text;
                writer.Cap = Cap;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Text
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.ITestInterface Cap
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string Text => ctx.ReadText(0, null);
                public Capnproto_test.Capnp.Test.ITestInterface Cap => ctx.ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(1);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public string Text
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public Capnproto_test.Capnp.Test.ITestInterface Cap
                {
                    get => ReadCap<Capnproto_test.Capnp.Test.ITestInterface>(1);
                    set => LinkObject(1, value);
                }
            }
        }
    }

    [TypeId(0x9ae342d394247cfcUL), Proxy(typeof(TestKeywordMethods_Proxy)), Skeleton(typeof(TestKeywordMethods_Skeleton))]
    public interface ITestKeywordMethods : IDisposable
    {
        Task Delete(CancellationToken cancellationToken_ = default);
        Task Class(CancellationToken cancellationToken_ = default);
        Task Void(CancellationToken cancellationToken_ = default);
        Task Return(CancellationToken cancellationToken_ = default);
    }

    public class TestKeywordMethods_Proxy : Proxy, ITestKeywordMethods
    {
        public async Task Delete(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestKeywordMethods.Params_Delete.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestKeywordMethods.Params_Delete()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(11160837778045172988UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestKeywordMethods.Result_Delete>(d_);
                return;
            }
        }

        public async Task Class(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestKeywordMethods.Params_Class.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestKeywordMethods.Params_Class()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(11160837778045172988UL, 1, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestKeywordMethods.Result_Class>(d_);
                return;
            }
        }

        public async Task Void(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestKeywordMethods.Params_Void.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestKeywordMethods.Params_Void()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(11160837778045172988UL, 2, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestKeywordMethods.Result_Void>(d_);
                return;
            }
        }

        public async Task Return(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestKeywordMethods.Params_Return.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestKeywordMethods.Params_Return()
            {};
            arg_?.serialize(in_);
            using (var d_ = await Call(11160837778045172988UL, 3, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestKeywordMethods.Result_Return>(d_);
                return;
            }
        }
    }

    public class TestKeywordMethods_Skeleton : Skeleton<ITestKeywordMethods>
    {
        public TestKeywordMethods_Skeleton()
        {
            SetMethodTable(Delete, Class, Void, Return);
        }

        public override ulong InterfaceId => 11160837778045172988UL;
        async Task<AnswerOrCounterquestion> Delete(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                await Impl.Delete(cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestKeywordMethods.Result_Delete.WRITER>();
                return s_;
            }
        }

        async Task<AnswerOrCounterquestion> Class(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                await Impl.Class(cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestKeywordMethods.Result_Class.WRITER>();
                return s_;
            }
        }

        async Task<AnswerOrCounterquestion> Void(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                await Impl.Void(cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestKeywordMethods.Result_Void.WRITER>();
                return s_;
            }
        }

        async Task<AnswerOrCounterquestion> Return(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                await Impl.Return(cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestKeywordMethods.Result_Return.WRITER>();
                return s_;
            }
        }
    }

    public static class TestKeywordMethods
    {
        [TypeId(0xca3a89cdeb6bd6b7UL)]
        public class Params_Delete : ICapnpSerializable
        {
            public const UInt64 typeId = 0xca3a89cdeb6bd6b7UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xeeb5843598307592UL)]
        public class Result_Delete : ICapnpSerializable
        {
            public const UInt64 typeId = 0xeeb5843598307592UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0x9cf5a8313c5db036UL)]
        public class Params_Class : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9cf5a8313c5db036UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xc0253868ac12e7d8UL)]
        public class Result_Class : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc0253868ac12e7d8UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xa4a08763833c7757UL)]
        public class Params_Void : ICapnpSerializable
        {
            public const UInt64 typeId = 0xa4a08763833c7757UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xde82773089c0aeabUL)]
        public class Result_Void : ICapnpSerializable
        {
            public const UInt64 typeId = 0xde82773089c0aeabUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0x99817360625e8ca3UL)]
        public class Params_Return : ICapnpSerializable
        {
            public const UInt64 typeId = 0x99817360625e8ca3UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xb70872e07eaa992fUL)]
        public class Result_Return : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb70872e07eaa992fUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }
    }

    [TypeId(0xea72cc77253798cdUL), Proxy(typeof(TestAuthenticatedBootstrap_Proxy<>)), Skeleton(typeof(TestAuthenticatedBootstrap_Skeleton<>))]
    public interface ITestAuthenticatedBootstrap<TVatId> : IDisposable where TVatId : class
    {
        Task<TVatId> GetCallerId(CancellationToken cancellationToken_ = default);
    }

    public class TestAuthenticatedBootstrap_Proxy<TVatId> : Proxy, ITestAuthenticatedBootstrap<TVatId> where TVatId : class
    {
        public Task<TVatId> GetCallerId(CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestAuthenticatedBootstrap<TVatId>.Params_GetCallerId.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestAuthenticatedBootstrap<TVatId>.Params_GetCallerId()
            {};
            arg_?.serialize(in_);
            return Impatient.MakePipelineAware(Call(16893789964317726925UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_), d_ =>
            {
                using (d_)
                {
                    var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAuthenticatedBootstrap<TVatId>.Result_GetCallerId>(d_);
                    return (r_.Caller);
                }
            }

            );
        }
    }

    public class TestAuthenticatedBootstrap_Skeleton<TVatId> : Skeleton<ITestAuthenticatedBootstrap<TVatId>> where TVatId : class
    {
        public TestAuthenticatedBootstrap_Skeleton()
        {
            SetMethodTable(GetCallerId);
        }

        public override ulong InterfaceId => 16893789964317726925UL;
        Task<AnswerOrCounterquestion> GetCallerId(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                return Impatient.MaybeTailCall(Impl.GetCallerId(cancellationToken_), caller =>
                {
                    var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestAuthenticatedBootstrap<TVatId>.Result_GetCallerId.WRITER>();
                    var r_ = new Capnproto_test.Capnp.Test.TestAuthenticatedBootstrap<TVatId>.Result_GetCallerId{Caller = caller};
                    r_.serialize(s_);
                    return s_;
                }

                );
            }
        }
    }

    public static class TestAuthenticatedBootstrap<TVatId>
        where TVatId : class
    {
        [TypeId(0x8ec30e2451f1cffeUL)]
        public class Params_GetCallerId : ICapnpSerializable
        {
            public const UInt64 typeId = 0x8ec30e2451f1cffeUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }

        [TypeId(0xc71cf776034a3e67UL)]
        public class Result_GetCallerId : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc71cf776034a3e67UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Caller = CapnpSerializable.Create<TVatId>(reader.Caller);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Caller.SetObject(Caller);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public TVatId Caller
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public DeserializerState Caller => ctx.StructReadPointer(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public DynamicSerializerState Caller
                {
                    get => BuildPointer<DynamicSerializerState>(0);
                    set => Link(0, value);
                }
            }
        }
    }

    [TypeId(0xceba982cb629f6c2UL)]
    public class TestSturdyRef : ICapnpSerializable
    {
        public const UInt64 typeId = 0xceba982cb629f6c2UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            HostId = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestSturdyRefHostId>(reader.HostId);
            ObjectId = CapnpSerializable.Create<object>(reader.ObjectId);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            HostId?.serialize(writer.HostId);
            writer.ObjectId.SetObject(ObjectId);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestSturdyRefHostId HostId
        {
            get;
            set;
        }

        public object ObjectId
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public Capnproto_test.Capnp.Test.TestSturdyRefHostId.READER HostId => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestSturdyRefHostId.READER.create);
            public DeserializerState ObjectId => ctx.StructReadPointer(1);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 2);
            }

            public Capnproto_test.Capnp.Test.TestSturdyRefHostId.WRITER HostId
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestSturdyRefHostId.WRITER>(0);
                set => Link(0, value);
            }

            public DynamicSerializerState ObjectId
            {
                get => BuildPointer<DynamicSerializerState>(1);
                set => Link(1, value);
            }
        }
    }

    [TypeId(0xe02d3bbe1010e342UL)]
    public class TestSturdyRefHostId : ICapnpSerializable
    {
        public const UInt64 typeId = 0xe02d3bbe1010e342UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Host = reader.Host;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Host = Host;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Host
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public string Host => ctx.ReadText(0, null);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public string Host
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }
        }
    }

    [TypeId(0xaeb2ad168e2f5697UL)]
    public class TestSturdyRefObjectId : ICapnpSerializable
    {
        public const UInt64 typeId = 0xaeb2ad168e2f5697UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            TheTag = reader.TheTag;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.TheTag = TheTag;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestSturdyRefObjectId.Tag TheTag
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public Capnproto_test.Capnp.Test.TestSturdyRefObjectId.Tag TheTag => (Capnproto_test.Capnp.Test.TestSturdyRefObjectId.Tag)ctx.ReadDataUShort(0UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public Capnproto_test.Capnp.Test.TestSturdyRefObjectId.Tag TheTag
            {
                get => (Capnproto_test.Capnp.Test.TestSturdyRefObjectId.Tag)this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, (ushort)value, (ushort)0);
            }
        }

        [TypeId(0xef428f2f67c4d439UL)]
        public enum Tag : ushort
        {
            testInterface,
            testExtends,
            testPipeline,
            testTailCallee,
            testTailCaller,
            testMoreStuff
        }
    }

    [TypeId(0x9e5c574772b1d462UL)]
    public class TestProvisionId : ICapnpSerializable
    {
        public const UInt64 typeId = 0x9e5c574772b1d462UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 0);
            }
        }
    }

    [TypeId(0xea2fb7dca9cdbdeaUL)]
    public class TestRecipientId : ICapnpSerializable
    {
        public const UInt64 typeId = 0xea2fb7dca9cdbdeaUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 0);
            }
        }
    }

    [TypeId(0xa805157b98b65469UL)]
    public class TestThirdPartyCapId : ICapnpSerializable
    {
        public const UInt64 typeId = 0xa805157b98b65469UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 0);
            }
        }
    }

    [TypeId(0xf4c58a8ebcd0f600UL)]
    public class TestJoinResult : ICapnpSerializable
    {
        public const UInt64 typeId = 0xf4c58a8ebcd0f600UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 0);
            }
        }
    }

    [TypeId(0xd1fd8e9caf2a5d58UL)]
    public class TestNameAnnotation : ICapnpSerializable
    {
        public const UInt64 typeId = 0xd1fd8e9caf2a5d58UL;
        public enum WHICH : ushort
        {
            BadFieldName = 0,
            Bar = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.BadFieldName:
                    BadFieldName = reader.BadFieldName;
                    break;
                case WHICH.Bar:
                    Bar = reader.Bar;
                    break;
            }

            AnotherBadFieldName = reader.AnotherBadFieldName;
            BadlyNamedUnion = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestNameAnnotation.badlyNamedUnion>(reader.BadlyNamedUnion);
            applyDefaults();
        }

        private WHICH _which = WHICH.undefined;
        private object _content;
        public WHICH which
        {
            get => _which;
            set
            {
                if (value == _which)
                    return;
                _which = value;
                switch (value)
                {
                    case WHICH.BadFieldName:
                        _content = false;
                        break;
                    case WHICH.Bar:
                        _content = 0;
                        break;
                }
            }
        }

        public void serialize(WRITER writer)
        {
            writer.which = which;
            switch (which)
            {
                case WHICH.BadFieldName:
                    writer.BadFieldName = BadFieldName.Value;
                    break;
                case WHICH.Bar:
                    writer.Bar = Bar.Value;
                    break;
            }

            writer.AnotherBadFieldName = AnotherBadFieldName;
            BadlyNamedUnion?.serialize(writer.BadlyNamedUnion);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public bool? BadFieldName
        {
            get => _which == WHICH.BadFieldName ? (bool? )_content : null;
            set
            {
                _which = WHICH.BadFieldName;
                _content = value;
            }
        }

        public sbyte? Bar
        {
            get => _which == WHICH.Bar ? (sbyte? )_content : null;
            set
            {
                _which = WHICH.Bar;
                _content = value;
            }
        }

        public Capnproto_test.Capnp.Test.TestNameAnnotation.BadlyNamedEnum AnotherBadFieldName
        {
            get;
            set;
        }

        public Capnproto_test.Capnp.Test.TestNameAnnotation.badlyNamedUnion BadlyNamedUnion
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public WHICH which => (WHICH)ctx.ReadDataUShort(16U, (ushort)0);
            public bool BadFieldName => which == WHICH.BadFieldName ? ctx.ReadDataBool(0UL, false) : default;
            public sbyte Bar => which == WHICH.Bar ? ctx.ReadDataSByte(0UL, (sbyte)0) : default;
            public Capnproto_test.Capnp.Test.TestNameAnnotation.BadlyNamedEnum AnotherBadFieldName => (Capnproto_test.Capnp.Test.TestNameAnnotation.BadlyNamedEnum)ctx.ReadDataUShort(32UL, (ushort)0);
            public badlyNamedUnion.READER BadlyNamedUnion => new badlyNamedUnion.READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(16U, (ushort)0);
                set => this.WriteData(16U, (ushort)value, (ushort)0);
            }

            public bool BadFieldName
            {
                get => which == WHICH.BadFieldName ? this.ReadDataBool(0UL, false) : default;
                set => this.WriteData(0UL, value, false);
            }

            public sbyte Bar
            {
                get => which == WHICH.Bar ? this.ReadDataSByte(0UL, (sbyte)0) : default;
                set => this.WriteData(0UL, value, (sbyte)0);
            }

            public Capnproto_test.Capnp.Test.TestNameAnnotation.BadlyNamedEnum AnotherBadFieldName
            {
                get => (Capnproto_test.Capnp.Test.TestNameAnnotation.BadlyNamedEnum)this.ReadDataUShort(32UL, (ushort)0);
                set => this.WriteData(32UL, (ushort)value, (ushort)0);
            }

            public badlyNamedUnion.WRITER BadlyNamedUnion
            {
                get => Rewrap<badlyNamedUnion.WRITER>();
            }
        }

        [TypeId(0x89d9d1626b34017cUL)]
        public class badlyNamedUnion : ICapnpSerializable
        {
            public const UInt64 typeId = 0x89d9d1626b34017cUL;
            public enum WHICH : ushort
            {
                BadlyNamedGroup = 0,
                Baz = 1,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.BadlyNamedGroup:
                        BadlyNamedGroup = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestNameAnnotation.badlyNamedUnion.badlyNamedGroup>(reader.BadlyNamedGroup);
                        break;
                    case WHICH.Baz:
                        Baz = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct>(reader.Baz);
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.BadlyNamedGroup:
                            _content = null;
                            break;
                        case WHICH.Baz:
                            _content = null;
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.BadlyNamedGroup:
                        BadlyNamedGroup?.serialize(writer.BadlyNamedGroup);
                        break;
                    case WHICH.Baz:
                        Baz?.serialize(writer.Baz);
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnproto_test.Capnp.Test.TestNameAnnotation.badlyNamedUnion.badlyNamedGroup BadlyNamedGroup
            {
                get => _which == WHICH.BadlyNamedGroup ? (Capnproto_test.Capnp.Test.TestNameAnnotation.badlyNamedUnion.badlyNamedGroup)_content : null;
                set
                {
                    _which = WHICH.BadlyNamedGroup;
                    _content = value;
                }
            }

            public Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct Baz
            {
                get => _which == WHICH.Baz ? (Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct)_content : null;
                set
                {
                    _which = WHICH.Baz;
                    _content = value;
                }
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public WHICH which => (WHICH)ctx.ReadDataUShort(48U, (ushort)0);
                public badlyNamedGroup.READER BadlyNamedGroup => which == WHICH.BadlyNamedGroup ? new badlyNamedGroup.READER(ctx) : default;
                public Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct.READER Baz => which == WHICH.Baz ? ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct.READER.create) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(48U, (ushort)0);
                    set => this.WriteData(48U, (ushort)value, (ushort)0);
                }

                public badlyNamedGroup.WRITER BadlyNamedGroup
                {
                    get => which == WHICH.BadlyNamedGroup ? Rewrap<badlyNamedGroup.WRITER>() : default;
                }

                public Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct.WRITER Baz
                {
                    get => which == WHICH.Baz ? BuildPointer<Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct.WRITER>(0) : default;
                    set => Link(0, value);
                }
            }

            [TypeId(0xc3594bce5b24b722UL)]
            public class badlyNamedGroup : ICapnpSerializable
            {
                public const UInt64 typeId = 0xc3594bce5b24b722UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }
                }
            }
        }

        [TypeId(0xf610d1deb4c9e84aUL)]
        public enum BadlyNamedEnum : ushort
        {
            foo,
            bar,
            baz
        }

        [TypeId(0xbe406b6341d52284UL)]
        public class NestedStruct : ICapnpSerializable
        {
            public const UInt64 typeId = 0xbe406b6341d52284UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                BadNestedFieldName = reader.BadNestedFieldName;
                AnotherBadNestedFieldName = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct>(reader.AnotherBadNestedFieldName);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.BadNestedFieldName = BadNestedFieldName;
                AnotherBadNestedFieldName?.serialize(writer.AnotherBadNestedFieldName);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public bool BadNestedFieldName
            {
                get;
                set;
            }

            public Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct AnotherBadNestedFieldName
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public bool BadNestedFieldName => ctx.ReadDataBool(0UL, false);
                public Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct.READER AnotherBadNestedFieldName => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public bool BadNestedFieldName
                {
                    get => this.ReadDataBool(0UL, false);
                    set => this.WriteData(0UL, value, false);
                }

                public Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct.WRITER AnotherBadNestedFieldName
                {
                    get => BuildPointer<Capnproto_test.Capnp.Test.TestNameAnnotation.NestedStruct.WRITER>(0);
                    set => Link(0, value);
                }
            }

            [TypeId(0xf6cb3f9c7a4322e0UL)]
            public enum DeeplyNestedEnum : ushort
            {
                quux,
                corge,
                grault
            }
        }
    }

    [TypeId(0xd112a69d31ed918bUL), Proxy(typeof(TestNameAnnotationInterface_Proxy)), Skeleton(typeof(TestNameAnnotationInterface_Skeleton))]
    public interface ITestNameAnnotationInterface : IDisposable
    {
        Task BadlyNamedMethod(byte BadlyNamedParam, CancellationToken cancellationToken_ = default);
    }

    public class TestNameAnnotationInterface_Proxy : Proxy, ITestNameAnnotationInterface
    {
        public async Task BadlyNamedMethod(byte BadlyNamedParam, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestNameAnnotationInterface.Params_BadlyNamedMethod.WRITER>();
            var arg_ = new Capnproto_test.Capnp.Test.TestNameAnnotationInterface.Params_BadlyNamedMethod()
            {BadlyNamedParam = BadlyNamedParam};
            arg_?.serialize(in_);
            using (var d_ = await Call(15065286897585459595UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned)
            {
                var r_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestNameAnnotationInterface.Result_BadlyNamedMethod>(d_);
                return;
            }
        }
    }

    public class TestNameAnnotationInterface_Skeleton : Skeleton<ITestNameAnnotationInterface>
    {
        public TestNameAnnotationInterface_Skeleton()
        {
            SetMethodTable(BadlyNamedMethod);
        }

        public override ulong InterfaceId => 15065286897585459595UL;
        async Task<AnswerOrCounterquestion> BadlyNamedMethod(DeserializerState d_, CancellationToken cancellationToken_)
        {
            using (d_)
            {
                var in_ = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestNameAnnotationInterface.Params_BadlyNamedMethod>(d_);
                await Impl.BadlyNamedMethod(in_.BadlyNamedParam, cancellationToken_);
                var s_ = SerializerState.CreateForRpc<Capnproto_test.Capnp.Test.TestNameAnnotationInterface.Result_BadlyNamedMethod.WRITER>();
                return s_;
            }
        }
    }

    public static class TestNameAnnotationInterface
    {
        [TypeId(0xc12efc3b075adfe9UL)]
        public class Params_BadlyNamedMethod : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc12efc3b075adfe9UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                BadlyNamedParam = reader.BadlyNamedParam;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.BadlyNamedParam = BadlyNamedParam;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public byte BadlyNamedParam
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public byte BadlyNamedParam => ctx.ReadDataByte(0UL, (byte)0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public byte BadlyNamedParam
                {
                    get => this.ReadDataByte(0UL, (byte)0);
                    set => this.WriteData(0UL, value, (byte)0);
                }
            }
        }

        [TypeId(0xdcc3cdb4b28f6c86UL)]
        public class Result_BadlyNamedMethod : ICapnpSerializable
        {
            public const UInt64 typeId = 0xdcc3cdb4b28f6c86UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 0);
                }
            }
        }
    }

    public static partial class PipeliningSupportExtensions_test
    {
        static readonly MemberAccessPath Path_capnproto_test_capnp_test_TestPipeline_getCap_OutBox_Cap = new MemberAccessPath(1U, 0U);
        public static Capnproto_test.Capnp.Test.ITestInterface OutBox_Cap(this Task<(string, Capnproto_test.Capnp.Test.TestPipeline.Box)> task)
        {
            async Task<IDisposable> AwaitProxy() => (await task).Item2?.Cap;
            return (Capnproto_test.Capnp.Test.ITestInterface)CapabilityReflection.CreateProxy<Capnproto_test.Capnp.Test.ITestInterface>(Impatient.Access(task, Path_capnproto_test_capnp_test_TestPipeline_getCap_OutBox_Cap, AwaitProxy()));
        }

        static readonly MemberAccessPath Path_capnproto_test_capnp_test_TestPipeline_getAnyCap_OutBox_Cap = new MemberAccessPath(1U, 0U);
        public static BareProxy OutBox_Cap(this Task<(string, Capnproto_test.Capnp.Test.TestPipeline.AnyBox)> task)
        {
            async Task<IDisposable> AwaitProxy() => (await task).Item2?.Cap;
            return (BareProxy)CapabilityReflection.CreateProxy<BareProxy>(Impatient.Access(task, Path_capnproto_test_capnp_test_TestPipeline_getAnyCap_OutBox_Cap, AwaitProxy()));
        }

        static readonly MemberAccessPath Path_capnproto_test_capnp_test_TestTailCallee_foo_C = new MemberAccessPath(1U);
        public static Capnproto_test.Capnp.Test.ITestCallOrder C(this Task<Capnproto_test.Capnp.Test.TestTailCallee.TailResult> task)
        {
            async Task<IDisposable> AwaitProxy() => (await task).C;
            return (Capnproto_test.Capnp.Test.ITestCallOrder)CapabilityReflection.CreateProxy<Capnproto_test.Capnp.Test.ITestCallOrder>(Impatient.Access(task, Path_capnproto_test_capnp_test_TestTailCallee_foo_C, AwaitProxy()));
        }
    }
}