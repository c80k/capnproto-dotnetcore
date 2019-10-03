using Capnp;
using System.Collections.Generic;

namespace CapnpC.CSharp.Generator.Schema
{
    namespace Superclass
    {
        public struct Reader
        {
            public DeserializerState State { get; }

            public Reader(DeserializerState ctx)
            {
                State = ctx;
            }

            public static Reader Create(DeserializerState ctx) => new Reader(ctx);

            public ulong Id => State.ReadDataULong(0);
            public Brand.Reader Brand => State.ReadStruct(0, Schema.Brand.Reader.Create);
        }

        public class Writer: SerializerState
        {
            public Writer()
            {
                SetStruct(1, 1);
            }

            public ulong Id
            {
                get => this.ReadDataULong(0);
                set => this.WriteData(0, value);
            }

            public Brand.Writer Brand
            {
                get => BuildPointer<Schema.Brand.Writer>(0);
                set => Link(0, value);
            }
        }
    }

    namespace Method
    {
        public struct Reader
        {
            public DeserializerState State { get; }

            public Reader(DeserializerState ctx)
            {
                State = ctx;
            }

            public static Reader Create(DeserializerState ctx) => new Reader(ctx);

            public string Name => State.ReadText(0);
            public ushort CodeOrder => State.ReadDataUShort(0);
            public IReadOnlyList<Node.Parameter.Reader> ImplicitParameters => State.ReadListOfStructs(4, Node.Parameter.Reader.Create);
            public ulong ParamStructType => State.ReadDataULong(64);
            public Brand.Reader ParamBrand => State.ReadStruct(2, Brand.Reader.Create);
            public ulong ResultStructType => State.ReadDataULong(128);
            public Brand.Reader ResultBrand => State.ReadStruct(3, Brand.Reader.Create);
            public IReadOnlyList<Annotation.Reader> Annotations => State.ReadListOfStructs(1, Annotation.Reader.Create);
        }

        public class Writer: SerializerState
        {
            public Writer()
            {
                SetStruct(5, 3);
            }

            public string Name
            {
                get => ReadText(0);
                set => WriteText(0, value);
            }

            public ushort CodeOrder
            {
                get => this.ReadDataUShort(0);
                set => this.WriteData(0, value);
            }

            public ListOfStructsSerializer<Node.Parameter.Writer> ImplicitParameters
            {
                get => BuildPointer<ListOfStructsSerializer<Node.Parameter.Writer>>(4);
                set => Link(4, value);
            }

            public ref ulong ParamStructType => ref this.RefData<ulong>(8);

            public Brand.Writer ParamBrand
            {
                get => BuildPointer<Brand.Writer>(2);
                set => Link(2, value);
            }

            public ulong ResultStructType
            {
                get => this.ReadDataULong(128);
                set => this.WriteData(128, value);
            }

            public Brand.Writer ResultBrand
            {
                get => BuildPointer<Brand.Writer>(3);
                set => Link(3, value);
            }

            public ListOfStructsSerializer<Annotation.Writer> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<Annotation.Writer>>(1);
                set => Link(1, value);
            }
        }
    }

    namespace Type
    {
        public struct Reader
        {
            public DeserializerState State { get; }

            public Reader(DeserializerState ctx)
            {
                State = ctx;
            }

            public static Reader Create(DeserializerState ctx) => new Reader(ctx);

            public ushort Tag => State.ReadDataUShort(0);
            public bool IsVoid => Tag == 0;
            public bool IsBool => Tag == 1;
            public bool IsInt8 => Tag == 2;
            public bool IsInt16 => Tag == 3;
            public bool IsInt32 => Tag == 4;
            public bool IsInt64 => Tag == 5;
            public bool IsUInt8 => Tag == 6;
            public bool IsUInt16 => Tag == 7;
            public bool IsUInt32 => Tag == 8;
            public bool IsUInt64 => Tag == 9;
            public bool IsFloat32 => Tag == 10;
            public bool IsFloat64 => Tag == 11;
            public bool IsText => Tag == 12;
            public bool IsData => Tag == 13;
            public bool IsList => Tag == 14;
            public Reader List_ElementType => IsList ? State.ReadStruct(0, Create) : default;
            public bool IsEnum => Tag == 15;
            public ulong Enum_TypeId => IsEnum ? State.ReadDataULong(64) : 0;
            public Brand.Reader Enum_Brand => IsEnum ? State.ReadStruct(0, Brand.Reader.Create) : default;
            public bool IsStruct => Tag == 16;
            public ulong Struct_TypeId => IsStruct ? State.ReadDataULong(64) : 0;
            public Brand.Reader Struct_Brand => IsStruct ? State.ReadStruct(0, Brand.Reader.Create) : default;
            public bool IsInterface => Tag == 17;
            public ulong Interface_TypeId => IsInterface ? State.ReadDataULong(64) : 0;
            public Brand.Reader Interface_Brand => IsInterface ? State.ReadStruct(0, Brand.Reader.Create) : default;
            public bool IsAnyPointer => Tag == 18;
            public ushort AnyPointer_Tag => IsAnyPointer ? State.ReadDataUShort(64) : default;
            public bool AnyPointer_IsUnconstrained => IsAnyPointer && AnyPointer_Tag == 0;
            public ushort AnyPointer_Unconstrained_Tag => AnyPointer_IsUnconstrained ? State.ReadDataUShort(80) : (ushort)0;
            public bool AnyPointer_Unconstrained_IsAnyKind => AnyPointer_IsUnconstrained && AnyPointer_Unconstrained_Tag == 0;
            public bool AnyPointer_Unconstrained_IsStruct => AnyPointer_IsUnconstrained && AnyPointer_Unconstrained_Tag == 1;
            public bool AnyPointer_Unconstrained_IsList => AnyPointer_IsUnconstrained && AnyPointer_Unconstrained_Tag == 2;
            public bool AnyPointer_Unconstrained_IsCapability => AnyPointer_IsUnconstrained && AnyPointer_Unconstrained_Tag == 3;
            public bool AnyPointer_IsParameter => IsAnyPointer && AnyPointer_Tag == 1;
            public ulong AnyPointer_Parameter_ScopeId => AnyPointer_IsParameter ? State.ReadDataULong(128) : 0;
            public ushort AnyPointer_Parameter_ParameterIndex => AnyPointer_IsParameter ? State.ReadDataUShort(80) : (ushort)0;
            public bool AnyPointer_IsImplicitMethodParameter => AnyPointer_Tag == 2;
            public ushort AnyPointer_ImplicitMethodParameter_ParameterIndex => AnyPointer_IsImplicitMethodParameter ? State.ReadDataUShort(80) : default;
        }

        public class Writer: SerializerState
        {
            public Writer()
            {
                SetStruct(3, 1);
            }

            public ref ushort Tag => ref this.RefData<ushort>(0);

            public bool IsVoid
            {
                get => Tag == 0;
                set => Tag = 0;
            }

            public bool IsBool
            {
                get => Tag == 1;
                set => Tag = 1;
            }

            public bool IsInt8
            {
                get => Tag == 2;
                set => Tag = 2;
            }

            public bool IsInt16
            {
                get => Tag == 3;
                set => Tag = 3;
            }

            public bool IsInt32
            {
                get => Tag == 4;
                set => Tag = 4;
            }

            public bool IsInt64
            {
                get => Tag == 5;
                set => Tag = 5;
            }

            public bool IsUInt8
            {
                get => Tag == 6;
                set => Tag = 6;
            }

            public bool IsUInt16
            {
                get => Tag == 7;
                set => Tag = 7;
            }

            public bool IsUInt32
            {
                get => Tag == 8;
                set => Tag = 8;
            }

            public bool IsUInt64
            {
                get => Tag == 9;
                set => Tag = 9;
            }

            public bool IsFloat32
            {
                get => Tag == 10;
                set => Tag = 10;
            }

            public bool IsFloat64
            {
                get => Tag == 11;
                set => Tag = 11;
            }

            public bool IsText
            {
                get => Tag == 12;
                set => Tag = 12;
            }

            public bool IsData
            {
                get => Tag == 13;
                set => Tag = 13;
            }

            public bool IsList
            {
                get => Tag == 14;
                set => Tag = 14;
            }

            public Writer List_ElementType
            {
                get => IsList ? BuildPointer<Writer>(0) : default;
                set { Link(0, value); }
            }

            public bool IsEnum
            {
                get => Tag == 15;
                set => Tag = 15;
            }

            public ulong Enum_TypeId
            {
                get => IsEnum ? this.ReadDataULong(64) : 0;
                set { this.WriteData(64, value); }
            }

            public Brand.Writer Enum_Brand
            {
                get => IsEnum ? BuildPointer<Brand.Writer>(0) : default;
                set => Link(0, value);
            }

            public bool IsStruct
            {
                get => Tag == 16;
                set => Tag = 16;
            }

            public ulong Struct_TypeId
            {
                get => IsStruct ? this.ReadDataULong(64) : 0;
                set => this.WriteData(64, value);
            }

            public Brand.Writer Struct_Brand
            {
                get => IsStruct ? BuildPointer<Brand.Writer>(0) : default;
                set => Link(0, value);
            }

            public bool IsInterface
            {
                get => Tag == 17;
                set => Tag = 17;
            }

            public ulong Interface_TypeId
            {
                get => IsStruct ? this.ReadDataULong(64) : 0;
                set => this.WriteData(64, value);
            }

            public Brand.Writer Interface_Brand
            {
                get => IsStruct ? BuildPointer<Brand.Writer>(0) : default;
                set => Link(0, value);
            }

            public bool IsAnyPointer
            {
                get => Tag == 18;
                set => Tag = 18;
            }

            public ushort AnyPointer_Tag
            {
                get => IsAnyPointer ? this.ReadDataUShort(64) : default;
                set => this.WriteData(64, value);
            }

            public bool AnyPointer_IsUnconstrained
            {
                get => IsAnyPointer && AnyPointer_Tag == 0;
                set => AnyPointer_Tag = 0;
            }

            public ushort AnyPointer_Unconstrained_Tag
            {
                get => AnyPointer_IsUnconstrained ? this.ReadDataUShort(80) : (ushort)0;
                set => this.WriteData(80, value);
            }

            public bool AnyPointer_Unconstrained_IsAnyKind
            {
                get => AnyPointer_IsUnconstrained && AnyPointer_Unconstrained_Tag == 0;
                set => AnyPointer_Unconstrained_Tag = 0;
            }

            public bool AnyPointer_Unconstrained_IsStruct
            {
                get => AnyPointer_IsUnconstrained && AnyPointer_Unconstrained_Tag == 1;
                set => AnyPointer_Unconstrained_Tag = 1;
            }

            public bool AnyPointer_Unconstrained_IsList
            {
                get => AnyPointer_IsUnconstrained && AnyPointer_Unconstrained_Tag == 2;
                set => AnyPointer_Unconstrained_Tag = 2;
            }

            public bool AnyPointer_Unconstrained_IsCapability
            {
                get => AnyPointer_IsUnconstrained && AnyPointer_Unconstrained_Tag == 3;
                set => AnyPointer_Unconstrained_Tag = 3;
            }

            public bool AnyPointer_IsParameter
            {
                get => IsAnyPointer && AnyPointer_Tag == 1;
                set => AnyPointer_Tag = 1;
            }

            public ulong AnyPointer_Parameter_ScopeId
            {
                get => AnyPointer_IsParameter ? this.ReadDataULong(128) : 0;
                set => this.WriteData(128, value);
            }

            public ushort AnyPointer_Parameter_ParameterIndex
            {
                get => AnyPointer_IsParameter ? this.ReadDataUShort(80) : (ushort)0;
                set => this.WriteData(80, value);
            }

            public bool AnyPointer_IsImplicitMethodParameter
            {
                get => AnyPointer_Tag == 2;
                set => AnyPointer_Tag = 2;
            }

            public ushort AnyPointer_ImplicitMethodParameter_ParameterIndex
            {
                get => AnyPointer_IsImplicitMethodParameter ? this.ReadDataUShort(80) : default;
                set => this.WriteData(80, value);
            }
        }
    }

    namespace Brand
    {
        namespace Scope
        {
            public struct Reader
            {
                public DeserializerState State { get; }

                public Reader(DeserializerState ctx)
                {
                    State = ctx;
                }

                public static Reader Create(DeserializerState ctx) => new Reader(ctx);

                public ulong ScopeId => State.ReadDataULong(0);
                public ushort Tag => State.ReadDataUShort(64);
                public bool IsBind => Tag == 0;
                public IReadOnlyList<Binding.Reader> Bind => IsBind ? State.ReadListOfStructs(0, Binding.Reader.Create) : null;
                public bool IsInherit => Tag == 1;
            }

            public class Writer: SerializerState
            {
                public Writer()
                {
                    SetStruct(2, 1);
                }

                public ulong ScopeId
                {
                    get => this.ReadDataULong(0);
                    set => this.WriteData(0, value);
                }

                public ushort Tag
                {
                    get => this.ReadDataUShort(64);
                    set => this.WriteData(64, value);
                }

                public bool IsBind
                {
                    get => Tag == 0;
                    set => Tag = 0;
                }

                public ListOfStructsSerializer<Binding.Writer> Bind
                {
                    get => IsBind ? BuildPointer<ListOfStructsSerializer<Binding.Writer>>(0) : default;
                    set => Link(0, value);
                }

                public bool IsInherit
                {
                    get => Tag == 1;
                    set => Tag = 1;
                }
            }

            namespace Binding
            {
                public struct Reader
                {
                    public DeserializerState State { get; }

                    public Reader(DeserializerState ctx)
                    {
                        State = ctx;
                    }

                    public static Reader Create(DeserializerState ctx) => new Reader(ctx);

                    public ushort Tag => State.ReadDataUShort(0);
                    public bool IsUnbound => Tag == 0;
                    public bool IsType => Tag == 1;
                    public Type.Reader Type => IsType ? State.ReadStruct(0, Schema.Type.Reader.Create) : default;
                }

                public class Writer: SerializerState
                {
                    public Writer()
                    {
                        SetStruct(1, 1);
                    }

                    public ushort Tag
                    {
                        get => this.ReadDataUShort(0);
                        set => this.WriteData(0, value);
                    }

                    public bool IsUnbound
                    {
                        get => Tag == 0;
                        set => Tag = 0;
                    }

                    public bool IsType
                    {
                        get => Tag == 1;
                        set => Tag = 1;
                    }

                    public Type.Writer Type
                    {
                        get => IsType ? BuildPointer<Schema.Type.Writer>(0) : default;
                        set => Link(0, value);
                    }
                }
            }
        }

        public struct Reader
        {
            public DeserializerState State { get; }

            public Reader(DeserializerState ctx)
            {
                State = ctx;
            }

            public static Reader Create(DeserializerState ctx) => new Reader(ctx);

            public IReadOnlyList<Scope.Reader> Scopes => State.ReadListOfStructs(0, Scope.Reader.Create);
        }

        public class Writer: SerializerState
        {
            public Writer()
            {
                SetStruct(0, 1);
            }

            public ListOfStructsSerializer<Scope.Writer> Scopes
            {
                get => BuildPointer<ListOfStructsSerializer<Scope.Writer>>(0);
                set => Link(0, value);
            }
        }
    }

    namespace Value
    {
        public struct Reader
        {
            public DeserializerState State { get; }

            public Reader(DeserializerState ctx)
            {
                State = ctx;
            }

            public static Reader Create(DeserializerState ctx) => new Reader(ctx);

            public ushort Tag => State.ReadDataUShort(0);
            public bool IsVoid => Tag == 0;
            public bool IsBool => Tag == 1;
            public bool Bool => IsBool ? State.ReadDataBool(16) : default;
            public bool IsInt8 => Tag == 2;
            public sbyte Int8 => IsInt8 ? State.ReadDataSByte(16) : default;
            public bool IsInt16 => Tag == 3;
            public short Int16 => IsInt16 ? State.ReadDataShort(16) : default;
            public bool IsInt32 => Tag == 4;
            public int Int32 => IsInt32 ? State.ReadDataInt(32) : default;
            public bool IsInt64 => Tag == 5;
            public long Int64 => IsInt64 ? State.ReadDataLong(64) : default;
            public bool IsUInt8 => Tag == 6;
            public byte UInt8 => IsUInt8 ? State.ReadDataByte(16) : default;
            public bool IsUInt16 => Tag == 7;
            public ushort UInt16 => IsUInt16 ? State.ReadDataUShort(16) : default;
            public bool IsUInt32 => Tag == 8;
            public uint UInt32 => IsUInt32 ? State.ReadDataUInt(32) : default;
            public bool IsUInt64 => Tag == 9;
            public ulong UInt64 => IsUInt64 ? State.ReadDataULong(64) : default;
            public bool IsFloat32 => Tag == 10;
            public float Float32 => IsFloat32 ? State.ReadDataFloat(32) : default;
            public bool IsFloat64 => Tag == 11;
            public double Float64 => IsFloat64 ? State.ReadDataDouble(64) : default;
            public bool IsText => Tag == 12;
            public string Text => IsText ? State.ReadText(0) : default;
            public bool IsData => Tag == 13;
            public ListDeserializer Data => IsData ? State.ReadList(0) : default;
            public bool IsList => Tag == 14;
            public DeserializerState List => IsList ? State.StructReadPointer(0) : default;
            public bool IsEnum => Tag == 15;
            public ushort Enum => IsEnum ? State.ReadDataUShort(16) : default;
            public bool IsStruct => Tag == 16;
            public DeserializerState Struct => IsStruct ? State.StructReadPointer(0) : default;
            public bool IsInterface => Tag == 17;
            public bool IsAnyPointer => Tag == 18;
            public DeserializerState AnyPointer => IsAnyPointer ? State.StructReadPointer(0) : default;
        }

        public class Writer: SerializerState
        {
            public Writer()
            {
                SetStruct(2, 1);
            }

            public ushort Tag
            {
                get => this.ReadDataUShort(0);
                set => this.WriteData(0, value);
            }

            public bool IsVoid
            {
                get => Tag == 0;
                set => Tag = 0;
            }

            public bool IsBool
            {
                get => Tag == 1;
                set => Tag = 1;
            }

            public bool Bool
            {
                get => IsBool ? this.ReadDataBool(16) : default;
                set => this.WriteData(16, value);
            }

            public bool IsInt8
            {
                get => Tag == 2;
                set => Tag = 2;
            }

            public sbyte Int8
            {
                get => IsInt8 ? this.ReadDataSByte(16) : default;
                set => this.WriteData(16, value);
            }

            public bool IsInt16
            {
                get => Tag == 3;
                set => Tag = 3;
            }

            public short Int16
            {
                get => IsInt16 ? this.ReadDataShort(16) : default;
                set => this.WriteData(16, value);
            }

            public bool IsInt32
            {
                get => Tag == 4;
                set => Tag = 4;
            }

            public int Int32
            {
                get => IsInt32 ? this.ReadDataInt(32) : default;
                set => this.WriteData(32, value);
            }

            public bool IsInt64
            {
                get => Tag == 5;
                set => Tag = 5;
            }

            public long Int64
            {
                get => IsInt64 ? this.ReadDataLong(64) : default;
                set => this.WriteData(64, value);
            }

            public bool IsUInt8
            {
                get => Tag == 6;
                set => Tag = 6;
            }

            public byte UInt8
            {
                get => IsUInt8 ? this.ReadDataByte(16) : default;
                set => this.WriteData(16, value);
            }

            public bool IsUInt16
            {
                get => Tag == 7;
                set => Tag = 7;
            }

            public ushort UInt16
            {
                get => IsUInt16 ? this.ReadDataUShort(16) : default;
                set => this.WriteData(16, value);
            }

            public bool IsUInt32
            {
                get => Tag == 8;
                set => Tag = 8;
            }

            public uint UInt32
            {
                get => IsUInt32 ? this.ReadDataUInt(32) : default;
                set => this.WriteData(32, value);
            }

            public bool IsUInt64
            {
                get => Tag == 9;
                set => Tag = 9;
            }

            public ulong UInt64
            {
                get => IsUInt64 ? this.ReadDataULong(64) : default;
                set => this.WriteData(64, value);
            }

            public bool IsFloat32
            {
                get => Tag == 10;
                set => Tag = 10;
            }

            public float Float32
            {
                get => IsFloat32 ? this.ReadDataFloat(32) : default;
                set => this.WriteData(32, value);
            }

            public bool IsFloat64
            {
                get => Tag == 11;
                set => Tag = 11;
            }

            public double Float64
            {
                get => IsFloat64 ? this.ReadDataDouble(64) : default;
                set => this.WriteData(64, value);
            }

            public bool IsText
            {
                get => Tag == 12;
                set => Tag = 12;
            }

            public string Text
            {
                get => IsText ? ReadText(0) : default;
                set => WriteText(0, value);
            }

            public bool IsData
            {
                get => Tag == 13;
                set => Tag = 13;
            }

            public SerializerState Data
            {
                get => IsData ? BuildPointer(0) : default;
                set => Link(0, value);
            }

            public bool IsList
            {
                get => Tag == 14;
                set => Tag = 14;
            }

            public SerializerState List
            {
                get => IsList ? BuildPointer(0) : default;
                set => Link(0, value);
            }

            public bool IsEnum
            {
                get => Tag == 15;
                set => Tag = 15;
            }

            public ushort Enum
            {
                get => IsEnum ? this.ReadDataUShort(16) : default;
                set => this.WriteData(16, value);
            }

            public bool IsStruct
            {
                get => Tag == 16;
                set => Tag = 16;
            }

            public SerializerState Struct
            {
                get => IsStruct ? BuildPointer(0) : default;
                set => Link(0, value);
            }

            public bool IsInterface
            {
                get => Tag == 17;
                set => Tag = 17;
            }

            public bool IsAnyPointer
            {
                get => Tag == 18;
                set => Tag = 18;
            }

            public SerializerState AnyPointer
            {
                get => IsAnyPointer ? BuildPointer(0) : default;
                set => Link(0, value);
            }
        }
    }

    namespace Annotation
    {
        public struct Reader
        {
            public DeserializerState State { get; }

            public Reader(DeserializerState ctx)
            {
                State = ctx;
            }

            public static Reader Create(DeserializerState ctx) => new Reader(ctx);

            public ulong Id => State.ReadDataULong(0);
            public Brand.Reader Brand => State.ReadStruct(1, Schema.Brand.Reader.Create);
            public Value.Reader Value => State.ReadStruct(0, Schema.Value.Reader.Create);
        }

        public class Writer: SerializerState
        {
            public Writer()
            {
                SetStruct(1, 2);
            }

            public ref ulong Id => ref this.RefData<ulong>(0);

            public Brand.Writer Brand
            {
                get => BuildPointer<Schema.Brand.Writer>(1);
                set => Link(1, value);
            }

            public Value.Writer Value
            {
                get => BuildPointer<Schema.Value.Writer>(0);
                set => Link(0, value);
            }
        }
    }

    public enum ElementSize: ushort
    {
        Empty = 0,
        Bit = 1,
        Byte = 2,
        TwoBytes = 3,
        FourBytes = 4,
        EightBytes = 5,
        Pointer = 6,
        InlineComposite = 7
    }

    namespace Field
    {
        public struct Reader
        {
            public DeserializerState State { get; }

            public Reader(DeserializerState ctx)
            {
                State = ctx;
            }

            public static Reader Create(DeserializerState ctx) => new Reader(ctx);

            public string Name => State.ReadText(0);
            public ushort CodeOrder => State.ReadDataUShort(0);
            public IReadOnlyList<Annotation.Reader> Annotations => State.ReadListOfStructs(1, Annotation.Reader.Create);
            public ushort DiscriminantValue => State.ReadDataUShort(16, 65535);
            public ushort Tag => State.ReadDataUShort(64);
            public bool IsSlot => Tag == 0;
            public uint Slot_Offset => IsSlot ? State.ReadDataUInt(32) : default;
            public Type.Reader Slot_Type => IsSlot ? State.ReadStruct(2, Type.Reader.Create) : default;
            public Value.Reader Slot_DefaultValue => IsSlot ? State.ReadStruct(3, Value.Reader.Create) : default;
            public bool Slot_HadExplicitDefault => IsSlot ? State.ReadDataBool(128) : default;
            public bool IsGroup => Tag == 1;
            public ulong Group_TypeId => IsGroup ? State.ReadDataULong(128) : default;
            public ushort Ordinal_Tag => State.ReadDataUShort(80);
            public bool Ordinal_IsImplicit => Ordinal_Tag == 0;
            public bool Ordinal_IsExplicit => Ordinal_Tag == 1;
            public ushort Ordinal_Explicit => Ordinal_IsExplicit ? State.ReadDataUShort(96) : default;

            public const ushort NoDiscriminant = 0xffff;
        }

        public class Writer: SerializerState
        {
            public Writer()
            {
                SetStruct(3, 3);
            }

            public string Name
            {
                get => ReadText(0);
                set => WriteText(0, value);
            }

            public ref ushort CodeOrder => ref this.RefData<ushort>(0);

            public ListOfStructsSerializer<Annotation.Writer> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<Annotation.Writer>>(1);
                set => Link(1, value);
            }

            public ushort DiscriminantValue
            {
                get => this.ReadDataUShort(16, 65535);
                set => this.WriteData(16, value, (ushort)65535);
            }

            public ref ushort Tag => ref this.RefData<ushort>(8);

            public bool IsSlot
            {
                get => Tag == 0;
                set => Tag = 0;
            }

            public uint Slot_Offset
            {
                get => IsSlot ? this.ReadDataUInt(32) : default;
                set => this.WriteData(32, value);
            }

            public Type.Writer Slot_Type
            {
                get => IsSlot ? BuildPointer<Type.Writer>(2) : default;
                set => Link(2, value);
            }

            public Value.Writer Slot_DefaultValue
            {
                get => IsSlot ? BuildPointer<Value.Writer>(3) : default;
                set => Link(3, value);
            }

            public bool Slot_HadExplicitDefault
            {
                get => IsSlot ? this.ReadDataBool(128) : default;
                set => this.WriteData(128, value);
            }

            public bool IsGroup
            {
                get => Tag == 1;
                set => Tag = 1;
            }

            public ref ulong Group_TypeId => ref this.RefData<ulong>(2);

            public ref ushort Ordinal_Tag => ref this.RefData<ushort>(5);

            public bool Ordinal_IsImplicit
            {
                get => Ordinal_Tag == 0;
                set => Ordinal_Tag = 0;
            }

            public bool Ordinal_IsExplicit
            {
                get => Ordinal_Tag == 1;
                set => Ordinal_Tag = 1;
            }

            public ref ushort Ordinal_Explicit => ref this.RefData<ushort>(6);
        }
    }

    namespace Node
    {
        namespace Parameter
        {
            public struct Reader
            {
                public DeserializerState State { get; }

                public Reader(DeserializerState ctx)
                {
                    State = ctx;
                }

                public static Reader Create(DeserializerState ctx) => new Reader(ctx);

                public string Name => State.ReadText(0);
            }

            public class Writer: SerializerState
            {
                public Writer()
                {
                    SetStruct(0, 1);
                }

                public string Name
                {
                    get => ReadText(0);
                    set => WriteText(0, value);
                }
            }
        }

        namespace NestedNode
        {
            public struct Reader
            {
                public DeserializerState State { get; }

                public Reader(DeserializerState ctx)
                {
                    State = ctx;
                }

                public static Reader Create(DeserializerState ctx) => new Reader(ctx);

                public string Name => State.ReadText(0);
                public ulong Id => State.ReadDataULong(0);
            }

            public class Writer: SerializerState
            {
                public Writer()
                {
                    SetStruct(1, 1);
                }

                public string Name
                {
                    get => ReadText(0);
                    set => WriteText(0, value);
                }

                public ref ulong Id => ref this.RefData<ulong>(0);
            }
        }

        namespace SourceInfo
        {
            namespace Member
            {
                public struct Reader
                {
                    public DeserializerState State { get; }

                    public Reader(DeserializerState ctx)
                    {
                        State = ctx;
                    }

                    public static Reader Create(DeserializerState ctx) => new Reader(ctx);

                    public string DocComment => State.ReadText(0);
                }

                public class Writer: SerializerState
                {
                    public Writer()
                    {
                        SetStruct(0, 1);
                    }

                    public string DocComment
                    {
                        get => ReadText(0);
                        set => WriteText(0, value);
                    }
                }
            }

            public struct Reader
            {
                public DeserializerState State { get; }

                public Reader(DeserializerState ctx)
                {
                    State = ctx;
                }

                public static Reader Create(DeserializerState ctx) => new Reader(ctx);

                public ulong Id => State.ReadDataULong(0);
                public string DocComment => State.ReadText(0);
                public IReadOnlyList<Member.Reader> Members => State.ReadListOfStructs(1, Member.Reader.Create);
            }

            public class Writer: SerializerState
            {
                public Writer()
                {
                    SetStruct(1, 2);
                }

                public ref ulong Id => ref this.RefData<ulong>(0);

                public string DocComment
                {
                    get => ReadText(0);
                    set => WriteText(0, value);
                }

                public ListOfStructsSerializer<Member.Writer> Members
                {
                    get => BuildPointer<ListOfStructsSerializer<Member.Writer>>(1);
                    set => Link(1, value);
                }
            }
        }

        public struct Reader
        {
            public DeserializerState State { get; }

            public Reader(DeserializerState ctx)
            {
                State = ctx;
            }

            public static Reader Create(DeserializerState ctx) => new Reader(ctx);

            public ulong Id => State.ReadDataULong(0);
            public string DisplayName => State.ReadText(0);
            public uint DisplayNamePrefixLength => State.ReadDataUInt(64);
            public ulong ScopeId => State.ReadDataULong(128);
            public IReadOnlyList<Parameter.Reader> Parameters => State.ReadListOfStructs(5, Parameter.Reader.Create);
            public bool IsGeneric => State.ReadDataBool(288);
            public IReadOnlyList<NestedNode.Reader> NestedNodes => State.ReadListOfStructs(1, NestedNode.Reader.Create);
            public IReadOnlyList<Annotation.Reader> Annotations => State.ReadListOfStructs(2, Annotation.Reader.Create);
            public ushort Tag => State.ReadDataUShort(96);
            public bool IsFile => Tag == 0;
            public bool IsStruct => Tag == 1;
            public ushort Struct_DataWordCount => IsStruct ? State.ReadDataUShort(112) : default;
            public ushort Struct_PointerCount => IsStruct ? State.ReadDataUShort(192) : default;
            public ElementSize Struct_PreferredListEncoding => IsStruct ? (ElementSize)State.ReadDataUShort(208) : default;
            public bool Struct_IsGroup => IsStruct ? State.ReadDataBool(224) : default;
            public ushort Struct_DiscriminantCount => IsStruct ? State.ReadDataUShort(240) : default;
            public uint Struct_DiscriminantOffset => IsStruct ? State.ReadDataUInt(256) : default;
            public IReadOnlyList<Field.Reader> Fields => IsStruct ? State.ReadListOfStructs(3, Field.Reader.Create) : default;
            public bool IsEnum => Tag == 2;
            public IReadOnlyList<Field.Reader> Enumerants => IsEnum ? State.ReadListOfStructs(3, Field.Reader.Create) : default;
            public bool IsInterface => Tag == 3;
            public IReadOnlyList<Method.Reader> Interface_Methods => IsInterface ? State.ReadListOfStructs(3, Method.Reader.Create) : default;
            public IReadOnlyList<Superclass.Reader> Interface_Superclasses => IsInterface ? State.ReadListOfStructs(4, Superclass.Reader.Create) : default;
            public bool IsConst => Tag == 4;
            public Type.Reader Const_Type => IsConst ? State.ReadStruct(3, Type.Reader.Create) : default;
            public Value.Reader Const_Value => IsConst ? State.ReadStruct(4, Value.Reader.Create) : default;
            public bool IsAnnotation => Tag == 5;
            public Type.Reader Annotation_Type => IsAnnotation ? State.ReadStruct(3, Type.Reader.Create) : default;
            public bool Annotation_TargetsFile => IsAnnotation ? State.ReadDataBool(112) : default;
            public bool Annotation_TargetsConst => IsAnnotation ? State.ReadDataBool(113) : default;
            public bool Annotation_TargetsEnum => IsAnnotation ? State.ReadDataBool(114) : default;
            public bool Annotation_TargetsEnumerant => IsAnnotation ? State.ReadDataBool(115) : default;
            public bool Annotation_TargetsStruct => IsAnnotation ? State.ReadDataBool(116) : default;
            public bool Annotation_TargetsField => IsAnnotation ? State.ReadDataBool(117) : default;
            public bool Annotation_TargetsUnion => IsAnnotation ? State.ReadDataBool(118) : default;
            public bool Annotation_TargetsGroup => IsAnnotation ? State.ReadDataBool(119) : default;
            public bool Annotation_TargetsInterface => IsAnnotation ? State.ReadDataBool(120) : default;
            public bool Annotation_TargetsMethod => IsAnnotation ? State.ReadDataBool(121) : default;
            public bool Annotation_TargetsParam => IsAnnotation ? State.ReadDataBool(122) : default;
            public bool Annotation_TargetsAnnotation => IsAnnotation ? State.ReadDataBool(123) : default;
        }

        public class Writer: SerializerState
        {
            public Writer()
            {
                SetStruct(5, 6);
            }

            public ulong Id
            {
                get => this.ReadDataULong(0);
                set => this.WriteData(0, value);
            }

            public string DisplayName
            {
                get => ReadText(0);
                set => WriteText(0, value);
            }

            public ref uint DisplayNamePrefixLength => ref this.RefData<uint>(2);

            public ref ulong ScopeId => ref this.RefData<ulong>(2);

            public ListOfStructsSerializer<Parameter.Writer> Parameters
            {
                get => BuildPointer<ListOfStructsSerializer<Parameter.Writer>>(5);
                set => Link(5, value);
            }

            public bool IsGeneric
            {
                get => this.ReadDataBool(288);
                set => this.WriteData(288, value);
            }

            public ListOfStructsSerializer<NestedNode.Writer> NestedNodes
            {
                get => BuildPointer<ListOfStructsSerializer<NestedNode.Writer>>(1);
                set => Link(1, value);
            }

            public ListOfStructsSerializer<Annotation.Writer> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<Annotation.Writer>>(2);
                set => Link(2, value);
            }

            public ref ushort Tag => ref this.RefData<ushort>(6);

            public bool IsFile
            {
                get => Tag == 0;
                set => Tag = 0;
            }

            public bool IsStruct
            {
                get => Tag == 1;
                set => Tag = 1;
            }

            public ref ushort Struct_DataWordCount => ref this.RefData<ushort>(7);

            public ref ushort Struct_PointerCount => ref this.RefData<ushort>(12);

            public ref ElementSize Struct_PreferredListEncoding => ref this.RefData<ElementSize>(13);

            public bool Struct_IsGroup
            {
                get => IsStruct ? this.ReadDataBool(224) : default;
                set => this.WriteData(224, value);
            }

            public ref ushort Struct_DiscriminantCount => ref this.RefData<ushort>(15);

            public ref uint Struct_DiscriminantOffset => ref this.RefData<uint>(8);

            public ListOfStructsSerializer<Field.Writer> Fields
            {
                get => BuildPointer<ListOfStructsSerializer<Field.Writer>>(3);
                set => Link(3, value);
            }

            public bool IsEnum
            {
                get => Tag == 2;
                set => Tag = 2;
            }

            public ListOfStructsSerializer<Field.Writer> Enumerants
            {
                get => BuildPointer<ListOfStructsSerializer<Field.Writer>>(3);
                set => Link(3, value);
            }

            public bool IsInterface
            {
                get => Tag == 3;
                set => Tag = 3;
            }

            public ListOfStructsSerializer<Method.Writer> Interface_Methods
            {
                get => BuildPointer<ListOfStructsSerializer<Method.Writer>>(3);
                set => Link(3, value);
            }

            public ListOfStructsSerializer<Superclass.Writer> Interface_Superclasses
            {
                get => IsInterface ? BuildPointer<ListOfStructsSerializer<Superclass.Writer>>(4) : default;
                set => Link(4, value);
            }

            public bool IsConst
            {
                get => Tag == 4;
                set => Tag = 4;
            }

            public Type.Writer Const_Type
            {
                get => IsConst ? BuildPointer<Type.Writer>(3) : default;
                set => Link(3, value);
            }

            public Value.Writer Const_Value
            {
                get => IsConst ? BuildPointer<Value.Writer>(4) : default;
                set => Link(4, value);
            }

            public bool IsAnnotation
            {
                get => Tag == 5;
                set => Tag = 5;
            }

            public Type.Writer Annotation_Type
            {
                get => IsAnnotation ? BuildPointer<Type.Writer>(3) : default;
                set => Link(3, value);
            }

            public bool Annotation_TargetsFile
            {
                get => IsAnnotation ? this.ReadDataBool(112) : default;
                set => this.WriteData(112, value);
            }

            public bool Annotation_TargetsConst
            {
                get => IsAnnotation ? this.ReadDataBool(113) : default;
                set => this.WriteData(113, value);
            }

            public bool Annotation_TargetsEnum
            {
                get => IsAnnotation ? this.ReadDataBool(114) : default;
                set => this.WriteData(114, value);
            }

            public bool Annotation_TargetsEnumerant
            {
                get => IsAnnotation ? this.ReadDataBool(115) : default;
                set => this.WriteData(115, value);
            }

            public bool Annotation_TargetsStruct
            {
                get => IsAnnotation ? this.ReadDataBool(116) : default;
                set => this.WriteData(116, value);
            }

            public bool Annotation_TargetsField
            {
                get => IsAnnotation ? this.ReadDataBool(117) : default;
                set => this.WriteData(117, value);
            }

            public bool Annotation_TargetsUnion
            {
                get => IsAnnotation ? this.ReadDataBool(118) : default;
                set => this.WriteData(118, value);
            }

            public bool Annotation_TargetsGroup
            {
                get => IsAnnotation ? this.ReadDataBool(119) : default;
                set => this.WriteData(119, value);
            }

            public bool Annotation_TargetsInterface
            {
                get => IsAnnotation ? this.ReadDataBool(120) : default;
                set => this.WriteData(120, value);
            }

            public bool Annotation_TargetsMethod
            {
                get => IsAnnotation ? this.ReadDataBool(121) : default;
                set => this.WriteData(121, value);
            }

            public bool Annotation_TargetsParam
            {
                get => IsAnnotation ? this.ReadDataBool(122) : default;
                set => this.WriteData(122, value);
            }

            public bool Annotation_TargetsAnnotation
            {
                get => IsAnnotation ? this.ReadDataBool(123) : default;
                set => this.WriteData(123, value);
            }
        }
    }

    namespace CapnpVersion
    {
        public struct Reader
        {
            public DeserializerState State { get; }

            public Reader(DeserializerState ctx)
            {
                State = ctx;
            }

            public static Reader Create(DeserializerState ctx) => new Reader(ctx);

            public ushort Major => State.ReadDataUShort(0);
            public byte Minor => State.ReadDataByte(16);
            public byte Micro => State.ReadDataByte(24);
        }

        public class Writer: SerializerState
        {
            public Writer()
            {
                SetStruct(1, 0);
            }

            public ref ushort Major => ref this.RefData<ushort>(0);
            public ref byte Minor => ref this.RefData<byte>(2);
            public ref byte Micro => ref this.RefData<byte>(3);
        }
    }

    namespace CodeGeneratorRequest
    {
        namespace RequestedFile
        {
            namespace Import
            {
                public struct Reader
                {
                    public DeserializerState State { get; }

                    public Reader(DeserializerState ctx)
                    {
                        State = ctx;
                    }

                    public static Reader Create(DeserializerState ctx) => new Reader(ctx);

                    public ulong Id => State.ReadDataULong(0);
                    public string Name => State.ReadText(0);
                }

                public class Writer: SerializerState
                {
                    public Writer()
                    {
                        SetStruct(1, 1);
                    }

                    public ref ulong Id => ref this.RefData<ulong>(0);

                    public string Name
                    {
                        get => ReadText(0);
                        set => WriteText(0, value);
                    }
                }
            }

            public struct Reader
            {
                public DeserializerState State { get; }

                public Reader(DeserializerState ctx)
                {
                    State = ctx;
                }

                public static Reader Create(DeserializerState ctx) => new Reader(ctx);

                public ulong Id => State.ReadDataULong(0);
                public string Filename => State.ReadText(0);
                public IReadOnlyList<Import.Reader> Imports => State.ReadListOfStructs(1, Import.Reader.Create);
            }

            public class Writer: SerializerState
            {
                public Writer()
                {
                    SetStruct(1, 2);
                }

                public ref ulong Id => ref this.RefData<ulong>(0);

                public string Filename
                {
                    get => ReadText(0);
                    set => WriteText(0, value);
                }

                public ListOfStructsSerializer<Import.Writer> Imports
                {
                    get => BuildPointer<ListOfStructsSerializer<Import.Writer>>(1);
                    set => Link(1, value);
                }
            }
        }

        public struct Reader
        {
            public DeserializerState State { get; }

            public Reader(DeserializerState ctx)
            {
                State = ctx;
            }

            public static Reader Create(DeserializerState ctx) => new Reader(ctx);

            public CapnpVersion.Reader CapnpVersion => State.ReadStruct(2, Schema.CapnpVersion.Reader.Create);
            public IReadOnlyList<Node.Reader> Nodes => State.ReadListOfStructs(0, Node.Reader.Create);
            public IReadOnlyList<Node.SourceInfo.Reader> SourceInfo => State.ReadListOfStructs(3, Node.SourceInfo.Reader.Create);
            public IReadOnlyList<RequestedFile.Reader> RequestedFiles => State.ReadListOfStructs(1, RequestedFile.Reader.Create);
        }

        public class Writer: SerializerState
        {
            public Writer()
            {
                SetStruct(0, 3);
            }

            public CapnpVersion.Writer CapnpVersion
            {
                get => BuildPointer<Schema.CapnpVersion.Writer>(2);
                set => Link(2, value);
            }

            public ListOfStructsSerializer<Node.Writer> Nodes
            {
                get => BuildPointer<ListOfStructsSerializer<Node.Writer>>(0);
                set => Link(0, value);
            }

            public ListOfStructsSerializer<Node.SourceInfo.Writer> SourceInfo
            {
                get => BuildPointer<ListOfStructsSerializer<Node.SourceInfo.Writer>>(3);
                set => Link(3, value);
            }

            public ListOfStructsSerializer<RequestedFile.Writer> RequestedFiles
            {
                get => BuildPointer<ListOfStructsSerializer<RequestedFile.Writer>>(1);
                set => Link(1, value);
            }
        }
    }
}
