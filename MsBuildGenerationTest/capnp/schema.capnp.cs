using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Schema
{
    public class Node : ICapnpSerializable
    {
        public enum WHICH : ushort
        {
            File = 0,
            Struct = 1,
            Enum = 2,
            Interface = 3,
            Const = 4,
            Annotation = 5,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.File:
                    which = reader.which;
                    break;
                case WHICH.Struct:
                    Struct = CapnpSerializable.Create<Capnp.Schema.Node.@struct>(reader.Struct);
                    break;
                case WHICH.Enum:
                    Enum = CapnpSerializable.Create<Capnp.Schema.Node.@enum>(reader.Enum);
                    break;
                case WHICH.Interface:
                    Interface = CapnpSerializable.Create<Capnp.Schema.Node.@interface>(reader.Interface);
                    break;
                case WHICH.Const:
                    Const = CapnpSerializable.Create<Capnp.Schema.Node.@const>(reader.Const);
                    break;
                case WHICH.Annotation:
                    Annotation = CapnpSerializable.Create<Capnp.Schema.Node.@annotation>(reader.Annotation);
                    break;
            }

            Id = reader.Id;
            DisplayName = reader.DisplayName;
            DisplayNamePrefixLength = reader.DisplayNamePrefixLength;
            ScopeId = reader.ScopeId;
            NestedNodes = reader.NestedNodes.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Node.NestedNode>(_));
            Annotations = reader.Annotations.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Annotation>(_));
            Parameters = reader.Parameters.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Node.Parameter>(_));
            IsGeneric = reader.IsGeneric;
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
                    case WHICH.File:
                        break;
                    case WHICH.Struct:
                        _content = null;
                        break;
                    case WHICH.Enum:
                        _content = null;
                        break;
                    case WHICH.Interface:
                        _content = null;
                        break;
                    case WHICH.Const:
                        _content = null;
                        break;
                    case WHICH.Annotation:
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
                case WHICH.File:
                    break;
                case WHICH.Struct:
                    Struct?.serialize(writer.Struct);
                    break;
                case WHICH.Enum:
                    Enum?.serialize(writer.Enum);
                    break;
                case WHICH.Interface:
                    Interface?.serialize(writer.Interface);
                    break;
                case WHICH.Const:
                    Const?.serialize(writer.Const);
                    break;
                case WHICH.Annotation:
                    Annotation?.serialize(writer.Annotation);
                    break;
            }

            writer.Id = Id;
            writer.DisplayName = DisplayName;
            writer.DisplayNamePrefixLength = DisplayNamePrefixLength;
            writer.ScopeId = ScopeId;
            writer.NestedNodes.Init(NestedNodes, (_s1, _v1) => _v1?.serialize(_s1));
            writer.Annotations.Init(Annotations, (_s1, _v1) => _v1?.serialize(_s1));
            writer.Parameters.Init(Parameters, (_s1, _v1) => _v1?.serialize(_s1));
            writer.IsGeneric = IsGeneric;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ulong Id
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public uint DisplayNamePrefixLength
        {
            get;
            set;
        }

        public ulong ScopeId
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.Schema.Node.NestedNode> NestedNodes
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.Schema.Annotation> Annotations
        {
            get;
            set;
        }

        public Capnp.Schema.Node.@struct Struct
        {
            get => _which == WHICH.Struct ? (Capnp.Schema.Node.@struct)_content : null;
            set
            {
                _which = WHICH.Struct;
                _content = value;
            }
        }

        public Capnp.Schema.Node.@enum Enum
        {
            get => _which == WHICH.Enum ? (Capnp.Schema.Node.@enum)_content : null;
            set
            {
                _which = WHICH.Enum;
                _content = value;
            }
        }

        public Capnp.Schema.Node.@interface Interface
        {
            get => _which == WHICH.Interface ? (Capnp.Schema.Node.@interface)_content : null;
            set
            {
                _which = WHICH.Interface;
                _content = value;
            }
        }

        public Capnp.Schema.Node.@const Const
        {
            get => _which == WHICH.Const ? (Capnp.Schema.Node.@const)_content : null;
            set
            {
                _which = WHICH.Const;
                _content = value;
            }
        }

        public Capnp.Schema.Node.@annotation Annotation
        {
            get => _which == WHICH.Annotation ? (Capnp.Schema.Node.@annotation)_content : null;
            set
            {
                _which = WHICH.Annotation;
                _content = value;
            }
        }

        public IReadOnlyList<Capnp.Schema.Node.Parameter> Parameters
        {
            get;
            set;
        }

        public bool IsGeneric
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
            public WHICH which => (WHICH)ctx.ReadDataUShort(96U, (ushort)0);
            public ulong Id => ctx.ReadDataULong(0UL, 0UL);
            public string DisplayName => ctx.ReadText(0, "");
            public uint DisplayNamePrefixLength => ctx.ReadDataUInt(64UL, 0U);
            public ulong ScopeId => ctx.ReadDataULong(128UL, 0UL);
            public IReadOnlyList<Capnp.Schema.Node.NestedNode.READER> NestedNodes => ctx.ReadList(1).Cast(Capnp.Schema.Node.NestedNode.READER.create);
            public IReadOnlyList<Capnp.Schema.Annotation.READER> Annotations => ctx.ReadList(2).Cast(Capnp.Schema.Annotation.READER.create);
            public @struct.READER Struct => which == WHICH.Struct ? new @struct.READER(ctx) : default;
            public @enum.READER Enum => which == WHICH.Enum ? new @enum.READER(ctx) : default;
            public @interface.READER Interface => which == WHICH.Interface ? new @interface.READER(ctx) : default;
            public @const.READER Const => which == WHICH.Const ? new @const.READER(ctx) : default;
            public @annotation.READER Annotation => which == WHICH.Annotation ? new @annotation.READER(ctx) : default;
            public IReadOnlyList<Capnp.Schema.Node.Parameter.READER> Parameters => ctx.ReadList(5).Cast(Capnp.Schema.Node.Parameter.READER.create);
            public bool IsGeneric => ctx.ReadDataBool(288UL, false);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(5, 6);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(96U, (ushort)0);
                set => this.WriteData(96U, (ushort)value, (ushort)0);
            }

            public ulong Id
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public string DisplayName
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }

            public uint DisplayNamePrefixLength
            {
                get => this.ReadDataUInt(64UL, 0U);
                set => this.WriteData(64UL, value, 0U);
            }

            public ulong ScopeId
            {
                get => this.ReadDataULong(128UL, 0UL);
                set => this.WriteData(128UL, value, 0UL);
            }

            public ListOfStructsSerializer<Capnp.Schema.Node.NestedNode.WRITER> NestedNodes
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Node.NestedNode.WRITER>>(1);
                set => Link(1, value);
            }

            public ListOfStructsSerializer<Capnp.Schema.Annotation.WRITER> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Annotation.WRITER>>(2);
                set => Link(2, value);
            }

            public @struct.WRITER Struct
            {
                get => which == WHICH.Struct ? Rewrap<@struct.WRITER>() : default;
            }

            public @enum.WRITER Enum
            {
                get => which == WHICH.Enum ? Rewrap<@enum.WRITER>() : default;
            }

            public @interface.WRITER Interface
            {
                get => which == WHICH.Interface ? Rewrap<@interface.WRITER>() : default;
            }

            public @const.WRITER Const
            {
                get => which == WHICH.Const ? Rewrap<@const.WRITER>() : default;
            }

            public @annotation.WRITER Annotation
            {
                get => which == WHICH.Annotation ? Rewrap<@annotation.WRITER>() : default;
            }

            public ListOfStructsSerializer<Capnp.Schema.Node.Parameter.WRITER> Parameters
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Node.Parameter.WRITER>>(5);
                set => Link(5, value);
            }

            public bool IsGeneric
            {
                get => this.ReadDataBool(288UL, false);
                set => this.WriteData(288UL, value, false);
            }
        }

        public class @struct : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                DataWordCount = reader.DataWordCount;
                PointerCount = reader.PointerCount;
                PreferredListEncoding = reader.PreferredListEncoding;
                IsGroup = reader.IsGroup;
                DiscriminantCount = reader.DiscriminantCount;
                DiscriminantOffset = reader.DiscriminantOffset;
                Fields = reader.Fields.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Field>(_));
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.DataWordCount = DataWordCount;
                writer.PointerCount = PointerCount;
                writer.PreferredListEncoding = PreferredListEncoding;
                writer.IsGroup = IsGroup;
                writer.DiscriminantCount = DiscriminantCount;
                writer.DiscriminantOffset = DiscriminantOffset;
                writer.Fields.Init(Fields, (_s1, _v1) => _v1?.serialize(_s1));
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ushort DataWordCount
            {
                get;
                set;
            }

            public ushort PointerCount
            {
                get;
                set;
            }

            public Capnp.Schema.ElementSize PreferredListEncoding
            {
                get;
                set;
            }

            public bool IsGroup
            {
                get;
                set;
            }

            public ushort DiscriminantCount
            {
                get;
                set;
            }

            public uint DiscriminantOffset
            {
                get;
                set;
            }

            public IReadOnlyList<Capnp.Schema.Field> Fields
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
                public ushort DataWordCount => ctx.ReadDataUShort(112UL, (ushort)0);
                public ushort PointerCount => ctx.ReadDataUShort(192UL, (ushort)0);
                public Capnp.Schema.ElementSize PreferredListEncoding => (Capnp.Schema.ElementSize)ctx.ReadDataUShort(208UL, (ushort)0);
                public bool IsGroup => ctx.ReadDataBool(224UL, false);
                public ushort DiscriminantCount => ctx.ReadDataUShort(240UL, (ushort)0);
                public uint DiscriminantOffset => ctx.ReadDataUInt(256UL, 0U);
                public IReadOnlyList<Capnp.Schema.Field.READER> Fields => ctx.ReadList(3).Cast(Capnp.Schema.Field.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public ushort DataWordCount
                {
                    get => this.ReadDataUShort(112UL, (ushort)0);
                    set => this.WriteData(112UL, value, (ushort)0);
                }

                public ushort PointerCount
                {
                    get => this.ReadDataUShort(192UL, (ushort)0);
                    set => this.WriteData(192UL, value, (ushort)0);
                }

                public Capnp.Schema.ElementSize PreferredListEncoding
                {
                    get => (Capnp.Schema.ElementSize)this.ReadDataUShort(208UL, (ushort)0);
                    set => this.WriteData(208UL, (ushort)value, (ushort)0);
                }

                public bool IsGroup
                {
                    get => this.ReadDataBool(224UL, false);
                    set => this.WriteData(224UL, value, false);
                }

                public ushort DiscriminantCount
                {
                    get => this.ReadDataUShort(240UL, (ushort)0);
                    set => this.WriteData(240UL, value, (ushort)0);
                }

                public uint DiscriminantOffset
                {
                    get => this.ReadDataUInt(256UL, 0U);
                    set => this.WriteData(256UL, value, 0U);
                }

                public ListOfStructsSerializer<Capnp.Schema.Field.WRITER> Fields
                {
                    get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Field.WRITER>>(3);
                    set => Link(3, value);
                }
            }
        }

        public class @enum : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Enumerants = reader.Enumerants.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Enumerant>(_));
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Enumerants.Init(Enumerants, (_s1, _v1) => _v1?.serialize(_s1));
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public IReadOnlyList<Capnp.Schema.Enumerant> Enumerants
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
                public IReadOnlyList<Capnp.Schema.Enumerant.READER> Enumerants => ctx.ReadList(3).Cast(Capnp.Schema.Enumerant.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public ListOfStructsSerializer<Capnp.Schema.Enumerant.WRITER> Enumerants
                {
                    get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Enumerant.WRITER>>(3);
                    set => Link(3, value);
                }
            }
        }

        public class @interface : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Methods = reader.Methods.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Method>(_));
                Superclasses = reader.Superclasses.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Superclass>(_));
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Methods.Init(Methods, (_s1, _v1) => _v1?.serialize(_s1));
                writer.Superclasses.Init(Superclasses, (_s1, _v1) => _v1?.serialize(_s1));
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public IReadOnlyList<Capnp.Schema.Method> Methods
            {
                get;
                set;
            }

            public IReadOnlyList<Capnp.Schema.Superclass> Superclasses
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
                public IReadOnlyList<Capnp.Schema.Method.READER> Methods => ctx.ReadList(3).Cast(Capnp.Schema.Method.READER.create);
                public IReadOnlyList<Capnp.Schema.Superclass.READER> Superclasses => ctx.ReadList(4).Cast(Capnp.Schema.Superclass.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public ListOfStructsSerializer<Capnp.Schema.Method.WRITER> Methods
                {
                    get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Method.WRITER>>(3);
                    set => Link(3, value);
                }

                public ListOfStructsSerializer<Capnp.Schema.Superclass.WRITER> Superclasses
                {
                    get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Superclass.WRITER>>(4);
                    set => Link(4, value);
                }
            }
        }

        public class @const : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Type = CapnpSerializable.Create<Capnp.Schema.Type>(reader.Type);
                Value = CapnpSerializable.Create<Capnp.Schema.Value>(reader.Value);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                Type?.serialize(writer.Type);
                Value?.serialize(writer.Value);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnp.Schema.Type Type
            {
                get;
                set;
            }

            public Capnp.Schema.Value Value
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
                public Capnp.Schema.Type.READER Type => ctx.ReadStruct(3, Capnp.Schema.Type.READER.create);
                public Capnp.Schema.Value.READER Value => ctx.ReadStruct(4, Capnp.Schema.Value.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public Capnp.Schema.Type.WRITER Type
                {
                    get => BuildPointer<Capnp.Schema.Type.WRITER>(3);
                    set => Link(3, value);
                }

                public Capnp.Schema.Value.WRITER Value
                {
                    get => BuildPointer<Capnp.Schema.Value.WRITER>(4);
                    set => Link(4, value);
                }
            }
        }

        public class @annotation : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Type = CapnpSerializable.Create<Capnp.Schema.Type>(reader.Type);
                TargetsFile = reader.TargetsFile;
                TargetsConst = reader.TargetsConst;
                TargetsEnum = reader.TargetsEnum;
                TargetsEnumerant = reader.TargetsEnumerant;
                TargetsStruct = reader.TargetsStruct;
                TargetsField = reader.TargetsField;
                TargetsUnion = reader.TargetsUnion;
                TargetsGroup = reader.TargetsGroup;
                TargetsInterface = reader.TargetsInterface;
                TargetsMethod = reader.TargetsMethod;
                TargetsParam = reader.TargetsParam;
                TargetsAnnotation = reader.TargetsAnnotation;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                Type?.serialize(writer.Type);
                writer.TargetsFile = TargetsFile;
                writer.TargetsConst = TargetsConst;
                writer.TargetsEnum = TargetsEnum;
                writer.TargetsEnumerant = TargetsEnumerant;
                writer.TargetsStruct = TargetsStruct;
                writer.TargetsField = TargetsField;
                writer.TargetsUnion = TargetsUnion;
                writer.TargetsGroup = TargetsGroup;
                writer.TargetsInterface = TargetsInterface;
                writer.TargetsMethod = TargetsMethod;
                writer.TargetsParam = TargetsParam;
                writer.TargetsAnnotation = TargetsAnnotation;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnp.Schema.Type Type
            {
                get;
                set;
            }

            public bool TargetsFile
            {
                get;
                set;
            }

            public bool TargetsConst
            {
                get;
                set;
            }

            public bool TargetsEnum
            {
                get;
                set;
            }

            public bool TargetsEnumerant
            {
                get;
                set;
            }

            public bool TargetsStruct
            {
                get;
                set;
            }

            public bool TargetsField
            {
                get;
                set;
            }

            public bool TargetsUnion
            {
                get;
                set;
            }

            public bool TargetsGroup
            {
                get;
                set;
            }

            public bool TargetsInterface
            {
                get;
                set;
            }

            public bool TargetsMethod
            {
                get;
                set;
            }

            public bool TargetsParam
            {
                get;
                set;
            }

            public bool TargetsAnnotation
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
                public Capnp.Schema.Type.READER Type => ctx.ReadStruct(3, Capnp.Schema.Type.READER.create);
                public bool TargetsFile => ctx.ReadDataBool(112UL, false);
                public bool TargetsConst => ctx.ReadDataBool(113UL, false);
                public bool TargetsEnum => ctx.ReadDataBool(114UL, false);
                public bool TargetsEnumerant => ctx.ReadDataBool(115UL, false);
                public bool TargetsStruct => ctx.ReadDataBool(116UL, false);
                public bool TargetsField => ctx.ReadDataBool(117UL, false);
                public bool TargetsUnion => ctx.ReadDataBool(118UL, false);
                public bool TargetsGroup => ctx.ReadDataBool(119UL, false);
                public bool TargetsInterface => ctx.ReadDataBool(120UL, false);
                public bool TargetsMethod => ctx.ReadDataBool(121UL, false);
                public bool TargetsParam => ctx.ReadDataBool(122UL, false);
                public bool TargetsAnnotation => ctx.ReadDataBool(123UL, false);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public Capnp.Schema.Type.WRITER Type
                {
                    get => BuildPointer<Capnp.Schema.Type.WRITER>(3);
                    set => Link(3, value);
                }

                public bool TargetsFile
                {
                    get => this.ReadDataBool(112UL, false);
                    set => this.WriteData(112UL, value, false);
                }

                public bool TargetsConst
                {
                    get => this.ReadDataBool(113UL, false);
                    set => this.WriteData(113UL, value, false);
                }

                public bool TargetsEnum
                {
                    get => this.ReadDataBool(114UL, false);
                    set => this.WriteData(114UL, value, false);
                }

                public bool TargetsEnumerant
                {
                    get => this.ReadDataBool(115UL, false);
                    set => this.WriteData(115UL, value, false);
                }

                public bool TargetsStruct
                {
                    get => this.ReadDataBool(116UL, false);
                    set => this.WriteData(116UL, value, false);
                }

                public bool TargetsField
                {
                    get => this.ReadDataBool(117UL, false);
                    set => this.WriteData(117UL, value, false);
                }

                public bool TargetsUnion
                {
                    get => this.ReadDataBool(118UL, false);
                    set => this.WriteData(118UL, value, false);
                }

                public bool TargetsGroup
                {
                    get => this.ReadDataBool(119UL, false);
                    set => this.WriteData(119UL, value, false);
                }

                public bool TargetsInterface
                {
                    get => this.ReadDataBool(120UL, false);
                    set => this.WriteData(120UL, value, false);
                }

                public bool TargetsMethod
                {
                    get => this.ReadDataBool(121UL, false);
                    set => this.WriteData(121UL, value, false);
                }

                public bool TargetsParam
                {
                    get => this.ReadDataBool(122UL, false);
                    set => this.WriteData(122UL, value, false);
                }

                public bool TargetsAnnotation
                {
                    get => this.ReadDataBool(123UL, false);
                    set => this.WriteData(123UL, value, false);
                }
            }
        }

        public class Parameter : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Name = reader.Name;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Name = Name;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Name
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
                public string Name => ctx.ReadText(0, "");
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string Name
                {
                    get => this.ReadText(0, "");
                    set => this.WriteText(0, value, "");
                }
            }
        }

        public class NestedNode : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Name = reader.Name;
                Id = reader.Id;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Name = Name;
                writer.Id = Id;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Name
            {
                get;
                set;
            }

            public ulong Id
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
                public string Name => ctx.ReadText(0, "");
                public ulong Id => ctx.ReadDataULong(0UL, 0UL);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public string Name
                {
                    get => this.ReadText(0, "");
                    set => this.WriteText(0, value, "");
                }

                public ulong Id
                {
                    get => this.ReadDataULong(0UL, 0UL);
                    set => this.WriteData(0UL, value, 0UL);
                }
            }
        }

        public class SourceInfo : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Id = reader.Id;
                DocComment = reader.DocComment;
                Members = reader.Members.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Node.SourceInfo.Member>(_));
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Id = Id;
                writer.DocComment = DocComment;
                writer.Members.Init(Members, (_s1, _v1) => _v1?.serialize(_s1));
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ulong Id
            {
                get;
                set;
            }

            public string DocComment
            {
                get;
                set;
            }

            public IReadOnlyList<Capnp.Schema.Node.SourceInfo.Member> Members
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
                public ulong Id => ctx.ReadDataULong(0UL, 0UL);
                public string DocComment => ctx.ReadText(0, "");
                public IReadOnlyList<Capnp.Schema.Node.SourceInfo.Member.READER> Members => ctx.ReadList(1).Cast(Capnp.Schema.Node.SourceInfo.Member.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 2);
                }

                public ulong Id
                {
                    get => this.ReadDataULong(0UL, 0UL);
                    set => this.WriteData(0UL, value, 0UL);
                }

                public string DocComment
                {
                    get => this.ReadText(0, "");
                    set => this.WriteText(0, value, "");
                }

                public ListOfStructsSerializer<Capnp.Schema.Node.SourceInfo.Member.WRITER> Members
                {
                    get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Node.SourceInfo.Member.WRITER>>(1);
                    set => Link(1, value);
                }
            }

            public class Member : ICapnpSerializable
            {
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    DocComment = reader.DocComment;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.DocComment = DocComment;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public string DocComment
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
                    public string DocComment => ctx.ReadText(0, "");
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 1);
                    }

                    public string DocComment
                    {
                        get => this.ReadText(0, "");
                        set => this.WriteText(0, value, "");
                    }
                }
            }
        }
    }

    public class Field : ICapnpSerializable
    {
        public enum WHICH : ushort
        {
            Slot = 0,
            Group = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.Slot:
                    Slot = CapnpSerializable.Create<Capnp.Schema.Field.@slot>(reader.Slot);
                    break;
                case WHICH.Group:
                    Group = CapnpSerializable.Create<Capnp.Schema.Field.@group>(reader.Group);
                    break;
            }

            Name = reader.Name;
            CodeOrder = reader.CodeOrder;
            Annotations = reader.Annotations.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Annotation>(_));
            DiscriminantValue = reader.DiscriminantValue;
            Ordinal = CapnpSerializable.Create<Capnp.Schema.Field.@ordinal>(reader.Ordinal);
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
                    case WHICH.Slot:
                        _content = null;
                        break;
                    case WHICH.Group:
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
                case WHICH.Slot:
                    Slot?.serialize(writer.Slot);
                    break;
                case WHICH.Group:
                    Group?.serialize(writer.Group);
                    break;
            }

            writer.Name = Name;
            writer.CodeOrder = CodeOrder;
            writer.Annotations.Init(Annotations, (_s1, _v1) => _v1?.serialize(_s1));
            writer.DiscriminantValue = DiscriminantValue;
            Ordinal?.serialize(writer.Ordinal);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Name
        {
            get;
            set;
        }

        public ushort CodeOrder
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.Schema.Annotation> Annotations
        {
            get;
            set;
        }

        public ushort DiscriminantValue
        {
            get;
            set;
        }

        = 65535;
        public Capnp.Schema.Field.@slot Slot
        {
            get => _which == WHICH.Slot ? (Capnp.Schema.Field.@slot)_content : null;
            set
            {
                _which = WHICH.Slot;
                _content = value;
            }
        }

        public Capnp.Schema.Field.@group Group
        {
            get => _which == WHICH.Group ? (Capnp.Schema.Field.@group)_content : null;
            set
            {
                _which = WHICH.Group;
                _content = value;
            }
        }

        public Capnp.Schema.Field.@ordinal Ordinal
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
            public WHICH which => (WHICH)ctx.ReadDataUShort(64U, (ushort)0);
            public string Name => ctx.ReadText(0, "");
            public ushort CodeOrder => ctx.ReadDataUShort(0UL, (ushort)0);
            public IReadOnlyList<Capnp.Schema.Annotation.READER> Annotations => ctx.ReadList(1).Cast(Capnp.Schema.Annotation.READER.create);
            public ushort DiscriminantValue => ctx.ReadDataUShort(16UL, (ushort)65535);
            public @slot.READER Slot => which == WHICH.Slot ? new @slot.READER(ctx) : default;
            public @group.READER Group => which == WHICH.Group ? new @group.READER(ctx) : default;
            public @ordinal.READER Ordinal => new @ordinal.READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(3, 4);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(64U, (ushort)0);
                set => this.WriteData(64U, (ushort)value, (ushort)0);
            }

            public string Name
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }

            public ushort CodeOrder
            {
                get => this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, value, (ushort)0);
            }

            public ListOfStructsSerializer<Capnp.Schema.Annotation.WRITER> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Annotation.WRITER>>(1);
                set => Link(1, value);
            }

            public ushort DiscriminantValue
            {
                get => this.ReadDataUShort(16UL, (ushort)65535);
                set => this.WriteData(16UL, value, (ushort)65535);
            }

            public @slot.WRITER Slot
            {
                get => which == WHICH.Slot ? Rewrap<@slot.WRITER>() : default;
            }

            public @group.WRITER Group
            {
                get => which == WHICH.Group ? Rewrap<@group.WRITER>() : default;
            }

            public @ordinal.WRITER Ordinal
            {
                get => Rewrap<@ordinal.WRITER>();
            }
        }

        public class @slot : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Offset = reader.Offset;
                Type = CapnpSerializable.Create<Capnp.Schema.Type>(reader.Type);
                DefaultValue = CapnpSerializable.Create<Capnp.Schema.Value>(reader.DefaultValue);
                HadExplicitDefault = reader.HadExplicitDefault;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Offset = Offset;
                Type?.serialize(writer.Type);
                DefaultValue?.serialize(writer.DefaultValue);
                writer.HadExplicitDefault = HadExplicitDefault;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint Offset
            {
                get;
                set;
            }

            public Capnp.Schema.Type Type
            {
                get;
                set;
            }

            public Capnp.Schema.Value DefaultValue
            {
                get;
                set;
            }

            public bool HadExplicitDefault
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
                public uint Offset => ctx.ReadDataUInt(32UL, 0U);
                public Capnp.Schema.Type.READER Type => ctx.ReadStruct(2, Capnp.Schema.Type.READER.create);
                public Capnp.Schema.Value.READER DefaultValue => ctx.ReadStruct(3, Capnp.Schema.Value.READER.create);
                public bool HadExplicitDefault => ctx.ReadDataBool(128UL, false);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public uint Offset
                {
                    get => this.ReadDataUInt(32UL, 0U);
                    set => this.WriteData(32UL, value, 0U);
                }

                public Capnp.Schema.Type.WRITER Type
                {
                    get => BuildPointer<Capnp.Schema.Type.WRITER>(2);
                    set => Link(2, value);
                }

                public Capnp.Schema.Value.WRITER DefaultValue
                {
                    get => BuildPointer<Capnp.Schema.Value.WRITER>(3);
                    set => Link(3, value);
                }

                public bool HadExplicitDefault
                {
                    get => this.ReadDataBool(128UL, false);
                    set => this.WriteData(128UL, value, false);
                }
            }
        }

        public class @group : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                TypeId = reader.TypeId;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.TypeId = TypeId;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ulong TypeId
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
                public ulong TypeId => ctx.ReadDataULong(128UL, 0UL);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public ulong TypeId
                {
                    get => this.ReadDataULong(128UL, 0UL);
                    set => this.WriteData(128UL, value, 0UL);
                }
            }
        }

        public class @ordinal : ICapnpSerializable
        {
            public enum WHICH : ushort
            {
                Implicit = 0,
                Explicit = 1,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Implicit:
                        which = reader.which;
                        break;
                    case WHICH.Explicit:
                        Explicit = reader.Explicit;
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
                        case WHICH.Implicit:
                            break;
                        case WHICH.Explicit:
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
                    case WHICH.Implicit:
                        break;
                    case WHICH.Explicit:
                        writer.Explicit = Explicit.Value;
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

            public ushort? Explicit
            {
                get => _which == WHICH.Explicit ? (ushort? )_content : null;
                set
                {
                    _which = WHICH.Explicit;
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
                public WHICH which => (WHICH)ctx.ReadDataUShort(80U, (ushort)0);
                public ushort Explicit => which == WHICH.Explicit ? ctx.ReadDataUShort(96UL, (ushort)0) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(80U, (ushort)0);
                    set => this.WriteData(80U, (ushort)value, (ushort)0);
                }

                public ushort Explicit
                {
                    get => which == WHICH.Explicit ? this.ReadDataUShort(96UL, (ushort)0) : default;
                    set => this.WriteData(96UL, value, (ushort)0);
                }
            }
        }
    }

    public class Enumerant : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Name = reader.Name;
            CodeOrder = reader.CodeOrder;
            Annotations = reader.Annotations.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Annotation>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Name = Name;
            writer.CodeOrder = CodeOrder;
            writer.Annotations.Init(Annotations, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Name
        {
            get;
            set;
        }

        public ushort CodeOrder
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.Schema.Annotation> Annotations
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
            public string Name => ctx.ReadText(0, "");
            public ushort CodeOrder => ctx.ReadDataUShort(0UL, (ushort)0);
            public IReadOnlyList<Capnp.Schema.Annotation.READER> Annotations => ctx.ReadList(1).Cast(Capnp.Schema.Annotation.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 2);
            }

            public string Name
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }

            public ushort CodeOrder
            {
                get => this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, value, (ushort)0);
            }

            public ListOfStructsSerializer<Capnp.Schema.Annotation.WRITER> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Annotation.WRITER>>(1);
                set => Link(1, value);
            }
        }
    }

    public class Superclass : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Id = reader.Id;
            Brand = CapnpSerializable.Create<Capnp.Schema.Brand>(reader.Brand);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Id = Id;
            Brand?.serialize(writer.Brand);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ulong Id
        {
            get;
            set;
        }

        public Capnp.Schema.Brand Brand
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
            public ulong Id => ctx.ReadDataULong(0UL, 0UL);
            public Capnp.Schema.Brand.READER Brand => ctx.ReadStruct(0, Capnp.Schema.Brand.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public ulong Id
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public Capnp.Schema.Brand.WRITER Brand
            {
                get => BuildPointer<Capnp.Schema.Brand.WRITER>(0);
                set => Link(0, value);
            }
        }
    }

    public class Method : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Name = reader.Name;
            CodeOrder = reader.CodeOrder;
            ParamStructType = reader.ParamStructType;
            ResultStructType = reader.ResultStructType;
            Annotations = reader.Annotations.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Annotation>(_));
            ParamBrand = CapnpSerializable.Create<Capnp.Schema.Brand>(reader.ParamBrand);
            ResultBrand = CapnpSerializable.Create<Capnp.Schema.Brand>(reader.ResultBrand);
            ImplicitParameters = reader.ImplicitParameters.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Node.Parameter>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Name = Name;
            writer.CodeOrder = CodeOrder;
            writer.ParamStructType = ParamStructType;
            writer.ResultStructType = ResultStructType;
            writer.Annotations.Init(Annotations, (_s1, _v1) => _v1?.serialize(_s1));
            ParamBrand?.serialize(writer.ParamBrand);
            ResultBrand?.serialize(writer.ResultBrand);
            writer.ImplicitParameters.Init(ImplicitParameters, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Name
        {
            get;
            set;
        }

        public ushort CodeOrder
        {
            get;
            set;
        }

        public ulong ParamStructType
        {
            get;
            set;
        }

        public ulong ResultStructType
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.Schema.Annotation> Annotations
        {
            get;
            set;
        }

        public Capnp.Schema.Brand ParamBrand
        {
            get;
            set;
        }

        public Capnp.Schema.Brand ResultBrand
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.Schema.Node.Parameter> ImplicitParameters
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
            public string Name => ctx.ReadText(0, "");
            public ushort CodeOrder => ctx.ReadDataUShort(0UL, (ushort)0);
            public ulong ParamStructType => ctx.ReadDataULong(64UL, 0UL);
            public ulong ResultStructType => ctx.ReadDataULong(128UL, 0UL);
            public IReadOnlyList<Capnp.Schema.Annotation.READER> Annotations => ctx.ReadList(1).Cast(Capnp.Schema.Annotation.READER.create);
            public Capnp.Schema.Brand.READER ParamBrand => ctx.ReadStruct(2, Capnp.Schema.Brand.READER.create);
            public Capnp.Schema.Brand.READER ResultBrand => ctx.ReadStruct(3, Capnp.Schema.Brand.READER.create);
            public IReadOnlyList<Capnp.Schema.Node.Parameter.READER> ImplicitParameters => ctx.ReadList(4).Cast(Capnp.Schema.Node.Parameter.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(3, 5);
            }

            public string Name
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }

            public ushort CodeOrder
            {
                get => this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, value, (ushort)0);
            }

            public ulong ParamStructType
            {
                get => this.ReadDataULong(64UL, 0UL);
                set => this.WriteData(64UL, value, 0UL);
            }

            public ulong ResultStructType
            {
                get => this.ReadDataULong(128UL, 0UL);
                set => this.WriteData(128UL, value, 0UL);
            }

            public ListOfStructsSerializer<Capnp.Schema.Annotation.WRITER> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Annotation.WRITER>>(1);
                set => Link(1, value);
            }

            public Capnp.Schema.Brand.WRITER ParamBrand
            {
                get => BuildPointer<Capnp.Schema.Brand.WRITER>(2);
                set => Link(2, value);
            }

            public Capnp.Schema.Brand.WRITER ResultBrand
            {
                get => BuildPointer<Capnp.Schema.Brand.WRITER>(3);
                set => Link(3, value);
            }

            public ListOfStructsSerializer<Capnp.Schema.Node.Parameter.WRITER> ImplicitParameters
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Node.Parameter.WRITER>>(4);
                set => Link(4, value);
            }
        }
    }

    public class Type : ICapnpSerializable
    {
        public enum WHICH : ushort
        {
            Void = 0,
            Bool = 1,
            Int8 = 2,
            Int16 = 3,
            Int32 = 4,
            Int64 = 5,
            Uint8 = 6,
            Uint16 = 7,
            Uint32 = 8,
            Uint64 = 9,
            Float32 = 10,
            Float64 = 11,
            Text = 12,
            Data = 13,
            List = 14,
            Enum = 15,
            Struct = 16,
            Interface = 17,
            AnyPointer = 18,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.Void:
                    which = reader.which;
                    break;
                case WHICH.Bool:
                    which = reader.which;
                    break;
                case WHICH.Int8:
                    which = reader.which;
                    break;
                case WHICH.Int16:
                    which = reader.which;
                    break;
                case WHICH.Int32:
                    which = reader.which;
                    break;
                case WHICH.Int64:
                    which = reader.which;
                    break;
                case WHICH.Uint8:
                    which = reader.which;
                    break;
                case WHICH.Uint16:
                    which = reader.which;
                    break;
                case WHICH.Uint32:
                    which = reader.which;
                    break;
                case WHICH.Uint64:
                    which = reader.which;
                    break;
                case WHICH.Float32:
                    which = reader.which;
                    break;
                case WHICH.Float64:
                    which = reader.which;
                    break;
                case WHICH.Text:
                    which = reader.which;
                    break;
                case WHICH.Data:
                    which = reader.which;
                    break;
                case WHICH.List:
                    List = CapnpSerializable.Create<Capnp.Schema.Type.@list>(reader.List);
                    break;
                case WHICH.Enum:
                    Enum = CapnpSerializable.Create<Capnp.Schema.Type.@enum>(reader.Enum);
                    break;
                case WHICH.Struct:
                    Struct = CapnpSerializable.Create<Capnp.Schema.Type.@struct>(reader.Struct);
                    break;
                case WHICH.Interface:
                    Interface = CapnpSerializable.Create<Capnp.Schema.Type.@interface>(reader.Interface);
                    break;
                case WHICH.AnyPointer:
                    AnyPointer = CapnpSerializable.Create<Capnp.Schema.Type.@anyPointer>(reader.AnyPointer);
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
                    case WHICH.Void:
                        break;
                    case WHICH.Bool:
                        break;
                    case WHICH.Int8:
                        break;
                    case WHICH.Int16:
                        break;
                    case WHICH.Int32:
                        break;
                    case WHICH.Int64:
                        break;
                    case WHICH.Uint8:
                        break;
                    case WHICH.Uint16:
                        break;
                    case WHICH.Uint32:
                        break;
                    case WHICH.Uint64:
                        break;
                    case WHICH.Float32:
                        break;
                    case WHICH.Float64:
                        break;
                    case WHICH.Text:
                        break;
                    case WHICH.Data:
                        break;
                    case WHICH.List:
                        _content = null;
                        break;
                    case WHICH.Enum:
                        _content = null;
                        break;
                    case WHICH.Struct:
                        _content = null;
                        break;
                    case WHICH.Interface:
                        _content = null;
                        break;
                    case WHICH.AnyPointer:
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
                case WHICH.Void:
                    break;
                case WHICH.Bool:
                    break;
                case WHICH.Int8:
                    break;
                case WHICH.Int16:
                    break;
                case WHICH.Int32:
                    break;
                case WHICH.Int64:
                    break;
                case WHICH.Uint8:
                    break;
                case WHICH.Uint16:
                    break;
                case WHICH.Uint32:
                    break;
                case WHICH.Uint64:
                    break;
                case WHICH.Float32:
                    break;
                case WHICH.Float64:
                    break;
                case WHICH.Text:
                    break;
                case WHICH.Data:
                    break;
                case WHICH.List:
                    List?.serialize(writer.List);
                    break;
                case WHICH.Enum:
                    Enum?.serialize(writer.Enum);
                    break;
                case WHICH.Struct:
                    Struct?.serialize(writer.Struct);
                    break;
                case WHICH.Interface:
                    Interface?.serialize(writer.Interface);
                    break;
                case WHICH.AnyPointer:
                    AnyPointer?.serialize(writer.AnyPointer);
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

        public Capnp.Schema.Type.@list List
        {
            get => _which == WHICH.List ? (Capnp.Schema.Type.@list)_content : null;
            set
            {
                _which = WHICH.List;
                _content = value;
            }
        }

        public Capnp.Schema.Type.@enum Enum
        {
            get => _which == WHICH.Enum ? (Capnp.Schema.Type.@enum)_content : null;
            set
            {
                _which = WHICH.Enum;
                _content = value;
            }
        }

        public Capnp.Schema.Type.@struct Struct
        {
            get => _which == WHICH.Struct ? (Capnp.Schema.Type.@struct)_content : null;
            set
            {
                _which = WHICH.Struct;
                _content = value;
            }
        }

        public Capnp.Schema.Type.@interface Interface
        {
            get => _which == WHICH.Interface ? (Capnp.Schema.Type.@interface)_content : null;
            set
            {
                _which = WHICH.Interface;
                _content = value;
            }
        }

        public Capnp.Schema.Type.@anyPointer AnyPointer
        {
            get => _which == WHICH.AnyPointer ? (Capnp.Schema.Type.@anyPointer)_content : null;
            set
            {
                _which = WHICH.AnyPointer;
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
            public @list.READER List => which == WHICH.List ? new @list.READER(ctx) : default;
            public @enum.READER Enum => which == WHICH.Enum ? new @enum.READER(ctx) : default;
            public @struct.READER Struct => which == WHICH.Struct ? new @struct.READER(ctx) : default;
            public @interface.READER Interface => which == WHICH.Interface ? new @interface.READER(ctx) : default;
            public @anyPointer.READER AnyPointer => which == WHICH.AnyPointer ? new @anyPointer.READER(ctx) : default;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(3, 1);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                set => this.WriteData(0U, (ushort)value, (ushort)0);
            }

            public @list.WRITER List
            {
                get => which == WHICH.List ? Rewrap<@list.WRITER>() : default;
            }

            public @enum.WRITER Enum
            {
                get => which == WHICH.Enum ? Rewrap<@enum.WRITER>() : default;
            }

            public @struct.WRITER Struct
            {
                get => which == WHICH.Struct ? Rewrap<@struct.WRITER>() : default;
            }

            public @interface.WRITER Interface
            {
                get => which == WHICH.Interface ? Rewrap<@interface.WRITER>() : default;
            }

            public @anyPointer.WRITER AnyPointer
            {
                get => which == WHICH.AnyPointer ? Rewrap<@anyPointer.WRITER>() : default;
            }
        }

        public class @list : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                ElementType = CapnpSerializable.Create<Capnp.Schema.Type>(reader.ElementType);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                ElementType?.serialize(writer.ElementType);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public Capnp.Schema.Type ElementType
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
                public Capnp.Schema.Type.READER ElementType => ctx.ReadStruct(0, Capnp.Schema.Type.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public Capnp.Schema.Type.WRITER ElementType
                {
                    get => BuildPointer<Capnp.Schema.Type.WRITER>(0);
                    set => Link(0, value);
                }
            }
        }

        public class @enum : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                TypeId = reader.TypeId;
                Brand = CapnpSerializable.Create<Capnp.Schema.Brand>(reader.Brand);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.TypeId = TypeId;
                Brand?.serialize(writer.Brand);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ulong TypeId
            {
                get;
                set;
            }

            public Capnp.Schema.Brand Brand
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
                public ulong TypeId => ctx.ReadDataULong(64UL, 0UL);
                public Capnp.Schema.Brand.READER Brand => ctx.ReadStruct(0, Capnp.Schema.Brand.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public ulong TypeId
                {
                    get => this.ReadDataULong(64UL, 0UL);
                    set => this.WriteData(64UL, value, 0UL);
                }

                public Capnp.Schema.Brand.WRITER Brand
                {
                    get => BuildPointer<Capnp.Schema.Brand.WRITER>(0);
                    set => Link(0, value);
                }
            }
        }

        public class @struct : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                TypeId = reader.TypeId;
                Brand = CapnpSerializable.Create<Capnp.Schema.Brand>(reader.Brand);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.TypeId = TypeId;
                Brand?.serialize(writer.Brand);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ulong TypeId
            {
                get;
                set;
            }

            public Capnp.Schema.Brand Brand
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
                public ulong TypeId => ctx.ReadDataULong(64UL, 0UL);
                public Capnp.Schema.Brand.READER Brand => ctx.ReadStruct(0, Capnp.Schema.Brand.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public ulong TypeId
                {
                    get => this.ReadDataULong(64UL, 0UL);
                    set => this.WriteData(64UL, value, 0UL);
                }

                public Capnp.Schema.Brand.WRITER Brand
                {
                    get => BuildPointer<Capnp.Schema.Brand.WRITER>(0);
                    set => Link(0, value);
                }
            }
        }

        public class @interface : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                TypeId = reader.TypeId;
                Brand = CapnpSerializable.Create<Capnp.Schema.Brand>(reader.Brand);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.TypeId = TypeId;
                Brand?.serialize(writer.Brand);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ulong TypeId
            {
                get;
                set;
            }

            public Capnp.Schema.Brand Brand
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
                public ulong TypeId => ctx.ReadDataULong(64UL, 0UL);
                public Capnp.Schema.Brand.READER Brand => ctx.ReadStruct(0, Capnp.Schema.Brand.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public ulong TypeId
                {
                    get => this.ReadDataULong(64UL, 0UL);
                    set => this.WriteData(64UL, value, 0UL);
                }

                public Capnp.Schema.Brand.WRITER Brand
                {
                    get => BuildPointer<Capnp.Schema.Brand.WRITER>(0);
                    set => Link(0, value);
                }
            }
        }

        public class @anyPointer : ICapnpSerializable
        {
            public enum WHICH : ushort
            {
                Unconstrained = 0,
                Parameter = 1,
                ImplicitMethodParameter = 2,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Unconstrained:
                        Unconstrained = CapnpSerializable.Create<Capnp.Schema.Type.@anyPointer.@unconstrained>(reader.Unconstrained);
                        break;
                    case WHICH.Parameter:
                        Parameter = CapnpSerializable.Create<Capnp.Schema.Type.@anyPointer.@parameter>(reader.Parameter);
                        break;
                    case WHICH.ImplicitMethodParameter:
                        ImplicitMethodParameter = CapnpSerializable.Create<Capnp.Schema.Type.@anyPointer.@implicitMethodParameter>(reader.ImplicitMethodParameter);
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
                        case WHICH.Unconstrained:
                            _content = null;
                            break;
                        case WHICH.Parameter:
                            _content = null;
                            break;
                        case WHICH.ImplicitMethodParameter:
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
                    case WHICH.Unconstrained:
                        Unconstrained?.serialize(writer.Unconstrained);
                        break;
                    case WHICH.Parameter:
                        Parameter?.serialize(writer.Parameter);
                        break;
                    case WHICH.ImplicitMethodParameter:
                        ImplicitMethodParameter?.serialize(writer.ImplicitMethodParameter);
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

            public Capnp.Schema.Type.@anyPointer.@unconstrained Unconstrained
            {
                get => _which == WHICH.Unconstrained ? (Capnp.Schema.Type.@anyPointer.@unconstrained)_content : null;
                set
                {
                    _which = WHICH.Unconstrained;
                    _content = value;
                }
            }

            public Capnp.Schema.Type.@anyPointer.@parameter Parameter
            {
                get => _which == WHICH.Parameter ? (Capnp.Schema.Type.@anyPointer.@parameter)_content : null;
                set
                {
                    _which = WHICH.Parameter;
                    _content = value;
                }
            }

            public Capnp.Schema.Type.@anyPointer.@implicitMethodParameter ImplicitMethodParameter
            {
                get => _which == WHICH.ImplicitMethodParameter ? (Capnp.Schema.Type.@anyPointer.@implicitMethodParameter)_content : null;
                set
                {
                    _which = WHICH.ImplicitMethodParameter;
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
                public @unconstrained.READER Unconstrained => which == WHICH.Unconstrained ? new @unconstrained.READER(ctx) : default;
                public @parameter.READER Parameter => which == WHICH.Parameter ? new @parameter.READER(ctx) : default;
                public @implicitMethodParameter.READER ImplicitMethodParameter => which == WHICH.ImplicitMethodParameter ? new @implicitMethodParameter.READER(ctx) : default;
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

                public @unconstrained.WRITER Unconstrained
                {
                    get => which == WHICH.Unconstrained ? Rewrap<@unconstrained.WRITER>() : default;
                }

                public @parameter.WRITER Parameter
                {
                    get => which == WHICH.Parameter ? Rewrap<@parameter.WRITER>() : default;
                }

                public @implicitMethodParameter.WRITER ImplicitMethodParameter
                {
                    get => which == WHICH.ImplicitMethodParameter ? Rewrap<@implicitMethodParameter.WRITER>() : default;
                }
            }

            public class @unconstrained : ICapnpSerializable
            {
                public enum WHICH : ushort
                {
                    AnyKind = 0,
                    Struct = 1,
                    List = 2,
                    Capability = 3,
                    undefined = 65535
                }

                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    switch (reader.which)
                    {
                        case WHICH.AnyKind:
                            which = reader.which;
                            break;
                        case WHICH.Struct:
                            which = reader.which;
                            break;
                        case WHICH.List:
                            which = reader.which;
                            break;
                        case WHICH.Capability:
                            which = reader.which;
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
                            case WHICH.AnyKind:
                                break;
                            case WHICH.Struct:
                                break;
                            case WHICH.List:
                                break;
                            case WHICH.Capability:
                                break;
                        }
                    }
                }

                public void serialize(WRITER writer)
                {
                    writer.which = which;
                    switch (which)
                    {
                        case WHICH.AnyKind:
                            break;
                        case WHICH.Struct:
                            break;
                        case WHICH.List:
                            break;
                        case WHICH.Capability:
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
                    public WHICH which => (WHICH)ctx.ReadDataUShort(80U, (ushort)0);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public WHICH which
                    {
                        get => (WHICH)this.ReadDataUShort(80U, (ushort)0);
                        set => this.WriteData(80U, (ushort)value, (ushort)0);
                    }
                }
            }

            public class @parameter : ICapnpSerializable
            {
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    ScopeId = reader.ScopeId;
                    ParameterIndex = reader.ParameterIndex;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.ScopeId = ScopeId;
                    writer.ParameterIndex = ParameterIndex;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public ulong ScopeId
                {
                    get;
                    set;
                }

                public ushort ParameterIndex
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
                    public ulong ScopeId => ctx.ReadDataULong(128UL, 0UL);
                    public ushort ParameterIndex => ctx.ReadDataUShort(80UL, (ushort)0);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public ulong ScopeId
                    {
                        get => this.ReadDataULong(128UL, 0UL);
                        set => this.WriteData(128UL, value, 0UL);
                    }

                    public ushort ParameterIndex
                    {
                        get => this.ReadDataUShort(80UL, (ushort)0);
                        set => this.WriteData(80UL, value, (ushort)0);
                    }
                }
            }

            public class @implicitMethodParameter : ICapnpSerializable
            {
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    ParameterIndex = reader.ParameterIndex;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.ParameterIndex = ParameterIndex;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public ushort ParameterIndex
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
                    public ushort ParameterIndex => ctx.ReadDataUShort(80UL, (ushort)0);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public ushort ParameterIndex
                    {
                        get => this.ReadDataUShort(80UL, (ushort)0);
                        set => this.WriteData(80UL, value, (ushort)0);
                    }
                }
            }
        }
    }

    public class Brand : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Scopes = reader.Scopes.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Brand.Scope>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Scopes.Init(Scopes, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public IReadOnlyList<Capnp.Schema.Brand.Scope> Scopes
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
            public IReadOnlyList<Capnp.Schema.Brand.Scope.READER> Scopes => ctx.ReadList(0).Cast(Capnp.Schema.Brand.Scope.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public ListOfStructsSerializer<Capnp.Schema.Brand.Scope.WRITER> Scopes
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Brand.Scope.WRITER>>(0);
                set => Link(0, value);
            }
        }

        public class Scope : ICapnpSerializable
        {
            public enum WHICH : ushort
            {
                Bind = 0,
                Inherit = 1,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Bind:
                        Bind = reader.Bind.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Brand.Binding>(_));
                        break;
                    case WHICH.Inherit:
                        which = reader.which;
                        break;
                }

                ScopeId = reader.ScopeId;
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
                        case WHICH.Bind:
                            _content = null;
                            break;
                        case WHICH.Inherit:
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.Bind:
                        writer.Bind.Init(Bind, (_s1, _v1) => _v1?.serialize(_s1));
                        break;
                    case WHICH.Inherit:
                        break;
                }

                writer.ScopeId = ScopeId;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ulong ScopeId
            {
                get;
                set;
            }

            public IReadOnlyList<Capnp.Schema.Brand.Binding> Bind
            {
                get => _which == WHICH.Bind ? (IReadOnlyList<Capnp.Schema.Brand.Binding>)_content : null;
                set
                {
                    _which = WHICH.Bind;
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
                public ulong ScopeId => ctx.ReadDataULong(0UL, 0UL);
                public IReadOnlyList<Capnp.Schema.Brand.Binding.READER> Bind => which == WHICH.Bind ? ctx.ReadList(0).Cast(Capnp.Schema.Brand.Binding.READER.create) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(2, 1);
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(64U, (ushort)0);
                    set => this.WriteData(64U, (ushort)value, (ushort)0);
                }

                public ulong ScopeId
                {
                    get => this.ReadDataULong(0UL, 0UL);
                    set => this.WriteData(0UL, value, 0UL);
                }

                public ListOfStructsSerializer<Capnp.Schema.Brand.Binding.WRITER> Bind
                {
                    get => which == WHICH.Bind ? BuildPointer<ListOfStructsSerializer<Capnp.Schema.Brand.Binding.WRITER>>(0) : default;
                    set => Link(0, value);
                }
            }
        }

        public class Binding : ICapnpSerializable
        {
            public enum WHICH : ushort
            {
                Unbound = 0,
                Type = 1,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Unbound:
                        which = reader.which;
                        break;
                    case WHICH.Type:
                        Type = CapnpSerializable.Create<Capnp.Schema.Type>(reader.Type);
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
                        case WHICH.Unbound:
                            break;
                        case WHICH.Type:
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
                    case WHICH.Unbound:
                        break;
                    case WHICH.Type:
                        Type?.serialize(writer.Type);
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

            public Capnp.Schema.Type Type
            {
                get => _which == WHICH.Type ? (Capnp.Schema.Type)_content : null;
                set
                {
                    _which = WHICH.Type;
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
                public Capnp.Schema.Type.READER Type => which == WHICH.Type ? ctx.ReadStruct(0, Capnp.Schema.Type.READER.create) : default;
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

                public Capnp.Schema.Type.WRITER Type
                {
                    get => which == WHICH.Type ? BuildPointer<Capnp.Schema.Type.WRITER>(0) : default;
                    set => Link(0, value);
                }
            }
        }
    }

    public class Value : ICapnpSerializable
    {
        public enum WHICH : ushort
        {
            Void = 0,
            Bool = 1,
            Int8 = 2,
            Int16 = 3,
            Int32 = 4,
            Int64 = 5,
            Uint8 = 6,
            Uint16 = 7,
            Uint32 = 8,
            Uint64 = 9,
            Float32 = 10,
            Float64 = 11,
            Text = 12,
            Data = 13,
            List = 14,
            Enum = 15,
            Struct = 16,
            Interface = 17,
            AnyPointer = 18,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.Void:
                    which = reader.which;
                    break;
                case WHICH.Bool:
                    Bool = reader.Bool;
                    break;
                case WHICH.Int8:
                    Int8 = reader.Int8;
                    break;
                case WHICH.Int16:
                    Int16 = reader.Int16;
                    break;
                case WHICH.Int32:
                    Int32 = reader.Int32;
                    break;
                case WHICH.Int64:
                    Int64 = reader.Int64;
                    break;
                case WHICH.Uint8:
                    Uint8 = reader.Uint8;
                    break;
                case WHICH.Uint16:
                    Uint16 = reader.Uint16;
                    break;
                case WHICH.Uint32:
                    Uint32 = reader.Uint32;
                    break;
                case WHICH.Uint64:
                    Uint64 = reader.Uint64;
                    break;
                case WHICH.Float32:
                    Float32 = reader.Float32;
                    break;
                case WHICH.Float64:
                    Float64 = reader.Float64;
                    break;
                case WHICH.Text:
                    Text = reader.Text;
                    break;
                case WHICH.Data:
                    Data = reader.Data;
                    break;
                case WHICH.List:
                    List = CapnpSerializable.Create<AnyPointer>(reader.List);
                    break;
                case WHICH.Enum:
                    Enum = reader.Enum;
                    break;
                case WHICH.Struct:
                    Struct = CapnpSerializable.Create<AnyPointer>(reader.Struct);
                    break;
                case WHICH.Interface:
                    which = reader.which;
                    break;
                case WHICH.AnyPointer:
                    AnyPointer = CapnpSerializable.Create<AnyPointer>(reader.AnyPointer);
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
                    case WHICH.Void:
                        break;
                    case WHICH.Bool:
                        _content = false;
                        break;
                    case WHICH.Int8:
                        _content = 0;
                        break;
                    case WHICH.Int16:
                        _content = 0;
                        break;
                    case WHICH.Int32:
                        _content = 0;
                        break;
                    case WHICH.Int64:
                        _content = 0;
                        break;
                    case WHICH.Uint8:
                        _content = 0;
                        break;
                    case WHICH.Uint16:
                        _content = 0;
                        break;
                    case WHICH.Uint32:
                        _content = 0;
                        break;
                    case WHICH.Uint64:
                        _content = 0;
                        break;
                    case WHICH.Float32:
                        _content = 0F;
                        break;
                    case WHICH.Float64:
                        _content = 0;
                        break;
                    case WHICH.Text:
                        _content = null;
                        break;
                    case WHICH.Data:
                        _content = null;
                        break;
                    case WHICH.List:
                        _content = null;
                        break;
                    case WHICH.Enum:
                        _content = 0;
                        break;
                    case WHICH.Struct:
                        _content = null;
                        break;
                    case WHICH.Interface:
                        break;
                    case WHICH.AnyPointer:
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
                case WHICH.Void:
                    break;
                case WHICH.Bool:
                    writer.Bool = Bool.Value;
                    break;
                case WHICH.Int8:
                    writer.Int8 = Int8.Value;
                    break;
                case WHICH.Int16:
                    writer.Int16 = Int16.Value;
                    break;
                case WHICH.Int32:
                    writer.Int32 = Int32.Value;
                    break;
                case WHICH.Int64:
                    writer.Int64 = Int64.Value;
                    break;
                case WHICH.Uint8:
                    writer.Uint8 = Uint8.Value;
                    break;
                case WHICH.Uint16:
                    writer.Uint16 = Uint16.Value;
                    break;
                case WHICH.Uint32:
                    writer.Uint32 = Uint32.Value;
                    break;
                case WHICH.Uint64:
                    writer.Uint64 = Uint64.Value;
                    break;
                case WHICH.Float32:
                    writer.Float32 = Float32.Value;
                    break;
                case WHICH.Float64:
                    writer.Float64 = Float64.Value;
                    break;
                case WHICH.Text:
                    writer.Text = Text;
                    break;
                case WHICH.Data:
                    writer.Data.Init(Data);
                    break;
                case WHICH.List:
                    writer.List.SetObject(List);
                    break;
                case WHICH.Enum:
                    writer.Enum = Enum.Value;
                    break;
                case WHICH.Struct:
                    writer.Struct.SetObject(Struct);
                    break;
                case WHICH.Interface:
                    break;
                case WHICH.AnyPointer:
                    writer.AnyPointer.SetObject(AnyPointer);
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

        public bool? Bool
        {
            get => _which == WHICH.Bool ? (bool? )_content : null;
            set
            {
                _which = WHICH.Bool;
                _content = value;
            }
        }

        public sbyte? Int8
        {
            get => _which == WHICH.Int8 ? (sbyte? )_content : null;
            set
            {
                _which = WHICH.Int8;
                _content = value;
            }
        }

        public short? Int16
        {
            get => _which == WHICH.Int16 ? (short? )_content : null;
            set
            {
                _which = WHICH.Int16;
                _content = value;
            }
        }

        public int? Int32
        {
            get => _which == WHICH.Int32 ? (int? )_content : null;
            set
            {
                _which = WHICH.Int32;
                _content = value;
            }
        }

        public long? Int64
        {
            get => _which == WHICH.Int64 ? (long? )_content : null;
            set
            {
                _which = WHICH.Int64;
                _content = value;
            }
        }

        public byte? Uint8
        {
            get => _which == WHICH.Uint8 ? (byte? )_content : null;
            set
            {
                _which = WHICH.Uint8;
                _content = value;
            }
        }

        public ushort? Uint16
        {
            get => _which == WHICH.Uint16 ? (ushort? )_content : null;
            set
            {
                _which = WHICH.Uint16;
                _content = value;
            }
        }

        public uint? Uint32
        {
            get => _which == WHICH.Uint32 ? (uint? )_content : null;
            set
            {
                _which = WHICH.Uint32;
                _content = value;
            }
        }

        public ulong? Uint64
        {
            get => _which == WHICH.Uint64 ? (ulong? )_content : null;
            set
            {
                _which = WHICH.Uint64;
                _content = value;
            }
        }

        public float? Float32
        {
            get => _which == WHICH.Float32 ? (float? )_content : null;
            set
            {
                _which = WHICH.Float32;
                _content = value;
            }
        }

        public double? Float64
        {
            get => _which == WHICH.Float64 ? (double? )_content : null;
            set
            {
                _which = WHICH.Float64;
                _content = value;
            }
        }

        public string Text
        {
            get => _which == WHICH.Text ? (string)_content : null;
            set
            {
                _which = WHICH.Text;
                _content = value;
            }
        }

        public IReadOnlyList<byte> Data
        {
            get => _which == WHICH.Data ? (IReadOnlyList<byte>)_content : null;
            set
            {
                _which = WHICH.Data;
                _content = value;
            }
        }

        public AnyPointer List
        {
            get => _which == WHICH.List ? (AnyPointer)_content : null;
            set
            {
                _which = WHICH.List;
                _content = value;
            }
        }

        public ushort? Enum
        {
            get => _which == WHICH.Enum ? (ushort? )_content : null;
            set
            {
                _which = WHICH.Enum;
                _content = value;
            }
        }

        public AnyPointer Struct
        {
            get => _which == WHICH.Struct ? (AnyPointer)_content : null;
            set
            {
                _which = WHICH.Struct;
                _content = value;
            }
        }

        public AnyPointer AnyPointer
        {
            get => _which == WHICH.AnyPointer ? (AnyPointer)_content : null;
            set
            {
                _which = WHICH.AnyPointer;
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
            public bool Bool => which == WHICH.Bool ? ctx.ReadDataBool(16UL, false) : default;
            public sbyte Int8 => which == WHICH.Int8 ? ctx.ReadDataSByte(16UL, (sbyte)0) : default;
            public short Int16 => which == WHICH.Int16 ? ctx.ReadDataShort(16UL, (short)0) : default;
            public int Int32 => which == WHICH.Int32 ? ctx.ReadDataInt(32UL, 0) : default;
            public long Int64 => which == WHICH.Int64 ? ctx.ReadDataLong(64UL, 0L) : default;
            public byte Uint8 => which == WHICH.Uint8 ? ctx.ReadDataByte(16UL, (byte)0) : default;
            public ushort Uint16 => which == WHICH.Uint16 ? ctx.ReadDataUShort(16UL, (ushort)0) : default;
            public uint Uint32 => which == WHICH.Uint32 ? ctx.ReadDataUInt(32UL, 0U) : default;
            public ulong Uint64 => which == WHICH.Uint64 ? ctx.ReadDataULong(64UL, 0UL) : default;
            public float Float32 => which == WHICH.Float32 ? ctx.ReadDataFloat(32UL, 0F) : default;
            public double Float64 => which == WHICH.Float64 ? ctx.ReadDataDouble(64UL, 0) : default;
            public string Text => which == WHICH.Text ? ctx.ReadText(0, "") : default;
            public IReadOnlyList<byte> Data => which == WHICH.Data ? ctx.ReadList(0).CastByte() : default;
            public DeserializerState List => which == WHICH.List ? ctx.StructReadPointer(0) : default;
            public ushort Enum => which == WHICH.Enum ? ctx.ReadDataUShort(16UL, (ushort)0) : default;
            public DeserializerState Struct => which == WHICH.Struct ? ctx.StructReadPointer(0) : default;
            public DeserializerState AnyPointer => which == WHICH.AnyPointer ? ctx.StructReadPointer(0) : default;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 1);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                set => this.WriteData(0U, (ushort)value, (ushort)0);
            }

            public bool Bool
            {
                get => which == WHICH.Bool ? this.ReadDataBool(16UL, false) : default;
                set => this.WriteData(16UL, value, false);
            }

            public sbyte Int8
            {
                get => which == WHICH.Int8 ? this.ReadDataSByte(16UL, (sbyte)0) : default;
                set => this.WriteData(16UL, value, (sbyte)0);
            }

            public short Int16
            {
                get => which == WHICH.Int16 ? this.ReadDataShort(16UL, (short)0) : default;
                set => this.WriteData(16UL, value, (short)0);
            }

            public int Int32
            {
                get => which == WHICH.Int32 ? this.ReadDataInt(32UL, 0) : default;
                set => this.WriteData(32UL, value, 0);
            }

            public long Int64
            {
                get => which == WHICH.Int64 ? this.ReadDataLong(64UL, 0L) : default;
                set => this.WriteData(64UL, value, 0L);
            }

            public byte Uint8
            {
                get => which == WHICH.Uint8 ? this.ReadDataByte(16UL, (byte)0) : default;
                set => this.WriteData(16UL, value, (byte)0);
            }

            public ushort Uint16
            {
                get => which == WHICH.Uint16 ? this.ReadDataUShort(16UL, (ushort)0) : default;
                set => this.WriteData(16UL, value, (ushort)0);
            }

            public uint Uint32
            {
                get => which == WHICH.Uint32 ? this.ReadDataUInt(32UL, 0U) : default;
                set => this.WriteData(32UL, value, 0U);
            }

            public ulong Uint64
            {
                get => which == WHICH.Uint64 ? this.ReadDataULong(64UL, 0UL) : default;
                set => this.WriteData(64UL, value, 0UL);
            }

            public float Float32
            {
                get => which == WHICH.Float32 ? this.ReadDataFloat(32UL, 0F) : default;
                set => this.WriteData(32UL, value, 0F);
            }

            public double Float64
            {
                get => which == WHICH.Float64 ? this.ReadDataDouble(64UL, 0) : default;
                set => this.WriteData(64UL, value, 0);
            }

            public string Text
            {
                get => which == WHICH.Text ? this.ReadText(0, "") : default;
                set => this.WriteText(0, value, "");
            }

            public ListOfPrimitivesSerializer<byte> Data
            {
                get => which == WHICH.Data ? BuildPointer<ListOfPrimitivesSerializer<byte>>(0) : default;
                set => Link(0, value);
            }

            public DynamicSerializerState List
            {
                get => which == WHICH.List ? BuildPointer<DynamicSerializerState>(0) : default;
                set => Link(0, value);
            }

            public ushort Enum
            {
                get => which == WHICH.Enum ? this.ReadDataUShort(16UL, (ushort)0) : default;
                set => this.WriteData(16UL, value, (ushort)0);
            }

            public DynamicSerializerState Struct
            {
                get => which == WHICH.Struct ? BuildPointer<DynamicSerializerState>(0) : default;
                set => Link(0, value);
            }

            public DynamicSerializerState AnyPointer
            {
                get => which == WHICH.AnyPointer ? BuildPointer<DynamicSerializerState>(0) : default;
                set => Link(0, value);
            }
        }
    }

    public class Annotation : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Id = reader.Id;
            Value = CapnpSerializable.Create<Capnp.Schema.Value>(reader.Value);
            Brand = CapnpSerializable.Create<Capnp.Schema.Brand>(reader.Brand);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Id = Id;
            Value?.serialize(writer.Value);
            Brand?.serialize(writer.Brand);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ulong Id
        {
            get;
            set;
        }

        public Capnp.Schema.Value Value
        {
            get;
            set;
        }

        public Capnp.Schema.Brand Brand
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
            public ulong Id => ctx.ReadDataULong(0UL, 0UL);
            public Capnp.Schema.Value.READER Value => ctx.ReadStruct(0, Capnp.Schema.Value.READER.create);
            public Capnp.Schema.Brand.READER Brand => ctx.ReadStruct(1, Capnp.Schema.Brand.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 2);
            }

            public ulong Id
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public Capnp.Schema.Value.WRITER Value
            {
                get => BuildPointer<Capnp.Schema.Value.WRITER>(0);
                set => Link(0, value);
            }

            public Capnp.Schema.Brand.WRITER Brand
            {
                get => BuildPointer<Capnp.Schema.Brand.WRITER>(1);
                set => Link(1, value);
            }
        }
    }

    public enum ElementSize : ushort
    {
        empty,
        bit,
        @byte,
        twoBytes,
        fourBytes,
        eightBytes,
        pointer,
        inlineComposite
    }

    public class CapnpVersion : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Major = reader.Major;
            Minor = reader.Minor;
            Micro = reader.Micro;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Major = Major;
            writer.Minor = Minor;
            writer.Micro = Micro;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ushort Major
        {
            get;
            set;
        }

        public byte Minor
        {
            get;
            set;
        }

        public byte Micro
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
            public ushort Major => ctx.ReadDataUShort(0UL, (ushort)0);
            public byte Minor => ctx.ReadDataByte(16UL, (byte)0);
            public byte Micro => ctx.ReadDataByte(24UL, (byte)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public ushort Major
            {
                get => this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, value, (ushort)0);
            }

            public byte Minor
            {
                get => this.ReadDataByte(16UL, (byte)0);
                set => this.WriteData(16UL, value, (byte)0);
            }

            public byte Micro
            {
                get => this.ReadDataByte(24UL, (byte)0);
                set => this.WriteData(24UL, value, (byte)0);
            }
        }
    }

    public class CodeGeneratorRequest : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Nodes = reader.Nodes.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Node>(_));
            RequestedFiles = reader.RequestedFiles.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.CodeGeneratorRequest.RequestedFile>(_));
            CapnpVersion = CapnpSerializable.Create<Capnp.Schema.CapnpVersion>(reader.CapnpVersion);
            SourceInfo = reader.SourceInfo.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.Node.SourceInfo>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Nodes.Init(Nodes, (_s1, _v1) => _v1?.serialize(_s1));
            writer.RequestedFiles.Init(RequestedFiles, (_s1, _v1) => _v1?.serialize(_s1));
            CapnpVersion?.serialize(writer.CapnpVersion);
            writer.SourceInfo.Init(SourceInfo, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public IReadOnlyList<Capnp.Schema.Node> Nodes
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.Schema.CodeGeneratorRequest.RequestedFile> RequestedFiles
        {
            get;
            set;
        }

        public Capnp.Schema.CapnpVersion CapnpVersion
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.Schema.Node.SourceInfo> SourceInfo
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
            public IReadOnlyList<Capnp.Schema.Node.READER> Nodes => ctx.ReadList(0).Cast(Capnp.Schema.Node.READER.create);
            public IReadOnlyList<Capnp.Schema.CodeGeneratorRequest.RequestedFile.READER> RequestedFiles => ctx.ReadList(1).Cast(Capnp.Schema.CodeGeneratorRequest.RequestedFile.READER.create);
            public Capnp.Schema.CapnpVersion.READER CapnpVersion => ctx.ReadStruct(2, Capnp.Schema.CapnpVersion.READER.create);
            public IReadOnlyList<Capnp.Schema.Node.SourceInfo.READER> SourceInfo => ctx.ReadList(3).Cast(Capnp.Schema.Node.SourceInfo.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 4);
            }

            public ListOfStructsSerializer<Capnp.Schema.Node.WRITER> Nodes
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Node.WRITER>>(0);
                set => Link(0, value);
            }

            public ListOfStructsSerializer<Capnp.Schema.CodeGeneratorRequest.RequestedFile.WRITER> RequestedFiles
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.CodeGeneratorRequest.RequestedFile.WRITER>>(1);
                set => Link(1, value);
            }

            public Capnp.Schema.CapnpVersion.WRITER CapnpVersion
            {
                get => BuildPointer<Capnp.Schema.CapnpVersion.WRITER>(2);
                set => Link(2, value);
            }

            public ListOfStructsSerializer<Capnp.Schema.Node.SourceInfo.WRITER> SourceInfo
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.Node.SourceInfo.WRITER>>(3);
                set => Link(3, value);
            }
        }

        public class RequestedFile : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Id = reader.Id;
                Filename = reader.Filename;
                Imports = reader.Imports.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Schema.CodeGeneratorRequest.RequestedFile.Import>(_));
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Id = Id;
                writer.Filename = Filename;
                writer.Imports.Init(Imports, (_s1, _v1) => _v1?.serialize(_s1));
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public ulong Id
            {
                get;
                set;
            }

            public string Filename
            {
                get;
                set;
            }

            public IReadOnlyList<Capnp.Schema.CodeGeneratorRequest.RequestedFile.Import> Imports
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
                public ulong Id => ctx.ReadDataULong(0UL, 0UL);
                public string Filename => ctx.ReadText(0, "");
                public IReadOnlyList<Capnp.Schema.CodeGeneratorRequest.RequestedFile.Import.READER> Imports => ctx.ReadList(1).Cast(Capnp.Schema.CodeGeneratorRequest.RequestedFile.Import.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 2);
                }

                public ulong Id
                {
                    get => this.ReadDataULong(0UL, 0UL);
                    set => this.WriteData(0UL, value, 0UL);
                }

                public string Filename
                {
                    get => this.ReadText(0, "");
                    set => this.WriteText(0, value, "");
                }

                public ListOfStructsSerializer<Capnp.Schema.CodeGeneratorRequest.RequestedFile.Import.WRITER> Imports
                {
                    get => BuildPointer<ListOfStructsSerializer<Capnp.Schema.CodeGeneratorRequest.RequestedFile.Import.WRITER>>(1);
                    set => Link(1, value);
                }
            }

            public class Import : ICapnpSerializable
            {
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Id = reader.Id;
                    Name = reader.Name;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Id = Id;
                    writer.Name = Name;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public ulong Id
                {
                    get;
                    set;
                }

                public string Name
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
                    public ulong Id => ctx.ReadDataULong(0UL, 0UL);
                    public string Name => ctx.ReadText(0, "");
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(1, 1);
                    }

                    public ulong Id
                    {
                        get => this.ReadDataULong(0UL, 0UL);
                        set => this.WriteData(0UL, value, 0UL);
                    }

                    public string Name
                    {
                        get => this.ReadText(0, "");
                        set => this.WriteText(0, value, "");
                    }
                }
            }
        }
    }
}