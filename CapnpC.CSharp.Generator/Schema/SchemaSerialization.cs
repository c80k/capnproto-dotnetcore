#pragma warning disable CS1591
using Capnp;
using Capnp.Rpc;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpC.CSharp.Generator.Schema
{
    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xe682ab4cf923a417UL)]
    public class Node : ICapnpSerializable
    {
        public const UInt64 typeId = 0xe682ab4cf923a417UL;
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
                    Struct = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node.@struct>(reader.Struct);
                    break;
                case WHICH.Enum:
                    Enum = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node.@enum>(reader.Enum);
                    break;
                case WHICH.Interface:
                    Interface = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node.@interface>(reader.Interface);
                    break;
                case WHICH.Const:
                    Const = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node.@const>(reader.Const);
                    break;
                case WHICH.Annotation:
                    Annotation = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node.annotation>(reader.Annotation);
                    break;
            }

            Id = reader.Id;
            DisplayName = reader.DisplayName;
            DisplayNamePrefixLength = reader.DisplayNamePrefixLength;
            ScopeId = reader.ScopeId;
            NestedNodes = reader.NestedNodes?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node.NestedNode>(_));
            Annotations = reader.Annotations?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Annotation>(_));
            Parameters = reader.Parameters?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node.Parameter>(_));
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

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.NestedNode> NestedNodes
        {
            get;
            set;
        }

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Annotation> Annotations
        {
            get;
            set;
        }

        public CapnpC.CSharp.Generator.Schema.Node.@struct Struct
        {
            get => _which == WHICH.Struct ? (CapnpC.CSharp.Generator.Schema.Node.@struct)_content : null;
            set
            {
                _which = WHICH.Struct;
                _content = value;
            }
        }

        public CapnpC.CSharp.Generator.Schema.Node.@enum Enum
        {
            get => _which == WHICH.Enum ? (CapnpC.CSharp.Generator.Schema.Node.@enum)_content : null;
            set
            {
                _which = WHICH.Enum;
                _content = value;
            }
        }

        public CapnpC.CSharp.Generator.Schema.Node.@interface Interface
        {
            get => _which == WHICH.Interface ? (CapnpC.CSharp.Generator.Schema.Node.@interface)_content : null;
            set
            {
                _which = WHICH.Interface;
                _content = value;
            }
        }

        public CapnpC.CSharp.Generator.Schema.Node.@const Const
        {
            get => _which == WHICH.Const ? (CapnpC.CSharp.Generator.Schema.Node.@const)_content : null;
            set
            {
                _which = WHICH.Const;
                _content = value;
            }
        }

        public CapnpC.CSharp.Generator.Schema.Node.annotation Annotation
        {
            get => _which == WHICH.Annotation ? (CapnpC.CSharp.Generator.Schema.Node.annotation)_content : null;
            set
            {
                _which = WHICH.Annotation;
                _content = value;
            }
        }

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.Parameter> Parameters
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
            public string DisplayName => ctx.ReadText(0, null);
            public uint DisplayNamePrefixLength => ctx.ReadDataUInt(64UL, 0U);
            public ulong ScopeId => ctx.ReadDataULong(128UL, 0UL);
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.NestedNode.READER> NestedNodes => ctx.ReadList(1).Cast(CapnpC.CSharp.Generator.Schema.Node.NestedNode.READER.create);
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Annotation.READER> Annotations => ctx.ReadList(2).Cast(CapnpC.CSharp.Generator.Schema.Annotation.READER.create);
            public @struct.READER Struct => which == WHICH.Struct ? new @struct.READER(ctx) : default;
            public @enum.READER Enum => which == WHICH.Enum ? new @enum.READER(ctx) : default;
            public @interface.READER Interface => which == WHICH.Interface ? new @interface.READER(ctx) : default;
            public @const.READER Const => which == WHICH.Const ? new @const.READER(ctx) : default;
            public annotation.READER Annotation => which == WHICH.Annotation ? new annotation.READER(ctx) : default;
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.Parameter.READER> Parameters => ctx.ReadList(5).Cast(CapnpC.CSharp.Generator.Schema.Node.Parameter.READER.create);
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
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
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

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.NestedNode.WRITER> NestedNodes
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.NestedNode.WRITER>>(1);
                set => Link(1, value);
            }

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Annotation.WRITER> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Annotation.WRITER>>(2);
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

            public annotation.WRITER Annotation
            {
                get => which == WHICH.Annotation ? Rewrap<annotation.WRITER>() : default;
            }

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.Parameter.WRITER> Parameters
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.Parameter.WRITER>>(5);
                set => Link(5, value);
            }

            public bool IsGeneric
            {
                get => this.ReadDataBool(288UL, false);
                set => this.WriteData(288UL, value, false);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x9ea0b19b37fb4435UL)]
        public class @struct : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9ea0b19b37fb4435UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                DataWordCount = reader.DataWordCount;
                PointerCount = reader.PointerCount;
                PreferredListEncoding = reader.PreferredListEncoding;
                IsGroup = reader.IsGroup;
                DiscriminantCount = reader.DiscriminantCount;
                DiscriminantOffset = reader.DiscriminantOffset;
                Fields = reader.Fields?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Field>(_));
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

            public CapnpC.CSharp.Generator.Schema.ElementSize PreferredListEncoding
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

            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Field> Fields
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
                public CapnpC.CSharp.Generator.Schema.ElementSize PreferredListEncoding => (CapnpC.CSharp.Generator.Schema.ElementSize)ctx.ReadDataUShort(208UL, (ushort)0);
                public bool IsGroup => ctx.ReadDataBool(224UL, false);
                public ushort DiscriminantCount => ctx.ReadDataUShort(240UL, (ushort)0);
                public uint DiscriminantOffset => ctx.ReadDataUInt(256UL, 0U);
                public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Field.READER> Fields => ctx.ReadList(3).Cast(CapnpC.CSharp.Generator.Schema.Field.READER.create);
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

                public CapnpC.CSharp.Generator.Schema.ElementSize PreferredListEncoding
                {
                    get => (CapnpC.CSharp.Generator.Schema.ElementSize)this.ReadDataUShort(208UL, (ushort)0);
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

                public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Field.WRITER> Fields
                {
                    get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Field.WRITER>>(3);
                    set => Link(3, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xb54ab3364333f598UL)]
        public class @enum : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb54ab3364333f598UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Enumerants = reader.Enumerants?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Enumerant>(_));
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

            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Enumerant> Enumerants
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
                public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Enumerant.READER> Enumerants => ctx.ReadList(3).Cast(CapnpC.CSharp.Generator.Schema.Enumerant.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Enumerant.WRITER> Enumerants
                {
                    get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Enumerant.WRITER>>(3);
                    set => Link(3, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xe82753cff0c2218fUL)]
        public class @interface : ICapnpSerializable
        {
            public const UInt64 typeId = 0xe82753cff0c2218fUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Methods = reader.Methods?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Method>(_));
                Superclasses = reader.Superclasses?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Superclass>(_));
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

            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Method> Methods
            {
                get;
                set;
            }

            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Superclass> Superclasses
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
                public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Method.READER> Methods => ctx.ReadList(3).Cast(CapnpC.CSharp.Generator.Schema.Method.READER.create);
                public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Superclass.READER> Superclasses => ctx.ReadList(4).Cast(CapnpC.CSharp.Generator.Schema.Superclass.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Method.WRITER> Methods
                {
                    get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Method.WRITER>>(3);
                    set => Link(3, value);
                }

                public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Superclass.WRITER> Superclasses
                {
                    get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Superclass.WRITER>>(4);
                    set => Link(4, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xb18aa5ac7a0d9420UL)]
        public class @const : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb18aa5ac7a0d9420UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Type = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type>(reader.Type);
                Value = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Value>(reader.Value);
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

            public CapnpC.CSharp.Generator.Schema.Type Type
            {
                get;
                set;
            }

            public CapnpC.CSharp.Generator.Schema.Value Value
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
                public CapnpC.CSharp.Generator.Schema.Type.READER Type => ctx.ReadStruct(3, CapnpC.CSharp.Generator.Schema.Type.READER.create);
                public CapnpC.CSharp.Generator.Schema.Value.READER Value => ctx.ReadStruct(4, CapnpC.CSharp.Generator.Schema.Value.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public CapnpC.CSharp.Generator.Schema.Type.WRITER Type
                {
                    get => BuildPointer<CapnpC.CSharp.Generator.Schema.Type.WRITER>(3);
                    set => Link(3, value);
                }

                public CapnpC.CSharp.Generator.Schema.Value.WRITER Value
                {
                    get => BuildPointer<CapnpC.CSharp.Generator.Schema.Value.WRITER>(4);
                    set => Link(4, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xec1619d4400a0290UL)]
        public class annotation : ICapnpSerializable
        {
            public const UInt64 typeId = 0xec1619d4400a0290UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Type = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type>(reader.Type);
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

            public CapnpC.CSharp.Generator.Schema.Type Type
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
                public CapnpC.CSharp.Generator.Schema.Type.READER Type => ctx.ReadStruct(3, CapnpC.CSharp.Generator.Schema.Type.READER.create);
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

                public CapnpC.CSharp.Generator.Schema.Type.WRITER Type
                {
                    get => BuildPointer<CapnpC.CSharp.Generator.Schema.Type.WRITER>(3);
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

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xb9521bccf10fa3b1UL)]
        public class Parameter : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb9521bccf10fa3b1UL;
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
                public string Name => ctx.ReadText(0, null);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string Name
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xdebf55bbfa0fc242UL)]
        public class NestedNode : ICapnpSerializable
        {
            public const UInt64 typeId = 0xdebf55bbfa0fc242UL;
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
                public string Name => ctx.ReadText(0, null);
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
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public ulong Id
                {
                    get => this.ReadDataULong(0UL, 0UL);
                    set => this.WriteData(0UL, value, 0UL);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xf38e1de3041357aeUL)]
        public class SourceInfo : ICapnpSerializable
        {
            public const UInt64 typeId = 0xf38e1de3041357aeUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Id = reader.Id;
                DocComment = reader.DocComment;
                Members = reader.Members?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node.SourceInfo.Member>(_));
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

            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.SourceInfo.Member> Members
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
                public string DocComment => ctx.ReadText(0, null);
                public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.SourceInfo.Member.READER> Members => ctx.ReadList(1).Cast(CapnpC.CSharp.Generator.Schema.Node.SourceInfo.Member.READER.create);
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
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.SourceInfo.Member.WRITER> Members
                {
                    get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.SourceInfo.Member.WRITER>>(1);
                    set => Link(1, value);
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xc2ba9038898e1fa2UL)]
            public class Member : ICapnpSerializable
            {
                public const UInt64 typeId = 0xc2ba9038898e1fa2UL;
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
                    public string DocComment => ctx.ReadText(0, null);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 1);
                    }

                    public string DocComment
                    {
                        get => this.ReadText(0, null);
                        set => this.WriteText(0, value, null);
                    }
                }
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x9aad50a41f4af45fUL)]
    public class Field : ICapnpSerializable
    {
        public const UInt64 typeId = 0x9aad50a41f4af45fUL;
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
                    Slot = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Field.slot>(reader.Slot);
                    break;
                case WHICH.Group:
                    Group = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Field.@group>(reader.Group);
                    break;
            }

            Name = reader.Name;
            CodeOrder = reader.CodeOrder;
            Annotations = reader.Annotations?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Annotation>(_));
            DiscriminantValue = reader.DiscriminantValue;
            Ordinal = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Field.ordinal>(reader.Ordinal);
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

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Annotation> Annotations
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
        public CapnpC.CSharp.Generator.Schema.Field.slot Slot
        {
            get => _which == WHICH.Slot ? (CapnpC.CSharp.Generator.Schema.Field.slot)_content : null;
            set
            {
                _which = WHICH.Slot;
                _content = value;
            }
        }

        public CapnpC.CSharp.Generator.Schema.Field.@group Group
        {
            get => _which == WHICH.Group ? (CapnpC.CSharp.Generator.Schema.Field.@group)_content : null;
            set
            {
                _which = WHICH.Group;
                _content = value;
            }
        }

        public CapnpC.CSharp.Generator.Schema.Field.ordinal Ordinal
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
            public string Name => ctx.ReadText(0, null);
            public ushort CodeOrder => ctx.ReadDataUShort(0UL, (ushort)0);
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Annotation.READER> Annotations => ctx.ReadList(1).Cast(CapnpC.CSharp.Generator.Schema.Annotation.READER.create);
            public ushort DiscriminantValue => ctx.ReadDataUShort(16UL, (ushort)65535);
            public slot.READER Slot => which == WHICH.Slot ? new slot.READER(ctx) : default;
            public @group.READER Group => which == WHICH.Group ? new @group.READER(ctx) : default;
            public ordinal.READER Ordinal => new ordinal.READER(ctx);
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
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public ushort CodeOrder
            {
                get => this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, value, (ushort)0);
            }

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Annotation.WRITER> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Annotation.WRITER>>(1);
                set => Link(1, value);
            }

            public ushort DiscriminantValue
            {
                get => this.ReadDataUShort(16UL, (ushort)65535);
                set => this.WriteData(16UL, value, (ushort)65535);
            }

            public slot.WRITER Slot
            {
                get => which == WHICH.Slot ? Rewrap<slot.WRITER>() : default;
            }

            public @group.WRITER Group
            {
                get => which == WHICH.Group ? Rewrap<@group.WRITER>() : default;
            }

            public ordinal.WRITER Ordinal
            {
                get => Rewrap<ordinal.WRITER>();
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xc42305476bb4746fUL)]
        public class slot : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc42305476bb4746fUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Offset = reader.Offset;
                Type = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type>(reader.Type);
                DefaultValue = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Value>(reader.DefaultValue);
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

            public CapnpC.CSharp.Generator.Schema.Type Type
            {
                get;
                set;
            }

            public CapnpC.CSharp.Generator.Schema.Value DefaultValue
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
                public CapnpC.CSharp.Generator.Schema.Type.READER Type => ctx.ReadStruct(2, CapnpC.CSharp.Generator.Schema.Type.READER.create);
                public CapnpC.CSharp.Generator.Schema.Value.READER DefaultValue => ctx.ReadStruct(3, CapnpC.CSharp.Generator.Schema.Value.READER.create);
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

                public CapnpC.CSharp.Generator.Schema.Type.WRITER Type
                {
                    get => BuildPointer<CapnpC.CSharp.Generator.Schema.Type.WRITER>(2);
                    set => Link(2, value);
                }

                public CapnpC.CSharp.Generator.Schema.Value.WRITER DefaultValue
                {
                    get => BuildPointer<CapnpC.CSharp.Generator.Schema.Value.WRITER>(3);
                    set => Link(3, value);
                }

                public bool HadExplicitDefault
                {
                    get => this.ReadDataBool(128UL, false);
                    set => this.WriteData(128UL, value, false);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xcafccddb68db1d11UL)]
        public class @group : ICapnpSerializable
        {
            public const UInt64 typeId = 0xcafccddb68db1d11UL;
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

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xbb90d5c287870be6UL)]
        public class ordinal : ICapnpSerializable
        {
            public const UInt64 typeId = 0xbb90d5c287870be6UL;
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
                get => _which == WHICH.Explicit ? (ushort?)_content : null;
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

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x978a7cebdc549a4dUL)]
    public class Enumerant : ICapnpSerializable
    {
        public const UInt64 typeId = 0x978a7cebdc549a4dUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Name = reader.Name;
            CodeOrder = reader.CodeOrder;
            Annotations = reader.Annotations?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Annotation>(_));
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

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Annotation> Annotations
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
            public string Name => ctx.ReadText(0, null);
            public ushort CodeOrder => ctx.ReadDataUShort(0UL, (ushort)0);
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Annotation.READER> Annotations => ctx.ReadList(1).Cast(CapnpC.CSharp.Generator.Schema.Annotation.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 2);
            }

            public string Name
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public ushort CodeOrder
            {
                get => this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, value, (ushort)0);
            }

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Annotation.WRITER> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Annotation.WRITER>>(1);
                set => Link(1, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xa9962a9ed0a4d7f8UL)]
    public class Superclass : ICapnpSerializable
    {
        public const UInt64 typeId = 0xa9962a9ed0a4d7f8UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Id = reader.Id;
            Brand = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Brand>(reader.Brand);
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

        public CapnpC.CSharp.Generator.Schema.Brand Brand
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
            public CapnpC.CSharp.Generator.Schema.Brand.READER Brand => ctx.ReadStruct(0, CapnpC.CSharp.Generator.Schema.Brand.READER.create);
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

            public CapnpC.CSharp.Generator.Schema.Brand.WRITER Brand
            {
                get => BuildPointer<CapnpC.CSharp.Generator.Schema.Brand.WRITER>(0);
                set => Link(0, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x9500cce23b334d80UL)]
    public class Method : ICapnpSerializable
    {
        public const UInt64 typeId = 0x9500cce23b334d80UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Name = reader.Name;
            CodeOrder = reader.CodeOrder;
            ParamStructType = reader.ParamStructType;
            ResultStructType = reader.ResultStructType;
            Annotations = reader.Annotations?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Annotation>(_));
            ParamBrand = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Brand>(reader.ParamBrand);
            ResultBrand = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Brand>(reader.ResultBrand);
            ImplicitParameters = reader.ImplicitParameters?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node.Parameter>(_));
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

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Annotation> Annotations
        {
            get;
            set;
        }

        public CapnpC.CSharp.Generator.Schema.Brand ParamBrand
        {
            get;
            set;
        }

        public CapnpC.CSharp.Generator.Schema.Brand ResultBrand
        {
            get;
            set;
        }

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.Parameter> ImplicitParameters
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
            public string Name => ctx.ReadText(0, null);
            public ushort CodeOrder => ctx.ReadDataUShort(0UL, (ushort)0);
            public ulong ParamStructType => ctx.ReadDataULong(64UL, 0UL);
            public ulong ResultStructType => ctx.ReadDataULong(128UL, 0UL);
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Annotation.READER> Annotations => ctx.ReadList(1).Cast(CapnpC.CSharp.Generator.Schema.Annotation.READER.create);
            public CapnpC.CSharp.Generator.Schema.Brand.READER ParamBrand => ctx.ReadStruct(2, CapnpC.CSharp.Generator.Schema.Brand.READER.create);
            public CapnpC.CSharp.Generator.Schema.Brand.READER ResultBrand => ctx.ReadStruct(3, CapnpC.CSharp.Generator.Schema.Brand.READER.create);
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.Parameter.READER> ImplicitParameters => ctx.ReadList(4).Cast(CapnpC.CSharp.Generator.Schema.Node.Parameter.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(3, 5);
            }

            public string Name
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
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

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Annotation.WRITER> Annotations
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Annotation.WRITER>>(1);
                set => Link(1, value);
            }

            public CapnpC.CSharp.Generator.Schema.Brand.WRITER ParamBrand
            {
                get => BuildPointer<CapnpC.CSharp.Generator.Schema.Brand.WRITER>(2);
                set => Link(2, value);
            }

            public CapnpC.CSharp.Generator.Schema.Brand.WRITER ResultBrand
            {
                get => BuildPointer<CapnpC.CSharp.Generator.Schema.Brand.WRITER>(3);
                set => Link(3, value);
            }

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.Parameter.WRITER> ImplicitParameters
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.Parameter.WRITER>>(4);
                set => Link(4, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xd07378ede1f9cc60UL)]
    public class Type : ICapnpSerializable
    {
        public const UInt64 typeId = 0xd07378ede1f9cc60UL;
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
                    List = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type.list>(reader.List);
                    break;
                case WHICH.Enum:
                    Enum = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type.@enum>(reader.Enum);
                    break;
                case WHICH.Struct:
                    Struct = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type.@struct>(reader.Struct);
                    break;
                case WHICH.Interface:
                    Interface = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type.@interface>(reader.Interface);
                    break;
                case WHICH.AnyPointer:
                    AnyPointer = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type.anyPointer>(reader.AnyPointer);
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

        public CapnpC.CSharp.Generator.Schema.Type.list List
        {
            get => _which == WHICH.List ? (CapnpC.CSharp.Generator.Schema.Type.list)_content : null;
            set
            {
                _which = WHICH.List;
                _content = value;
            }
        }

        public CapnpC.CSharp.Generator.Schema.Type.@enum Enum
        {
            get => _which == WHICH.Enum ? (CapnpC.CSharp.Generator.Schema.Type.@enum)_content : null;
            set
            {
                _which = WHICH.Enum;
                _content = value;
            }
        }

        public CapnpC.CSharp.Generator.Schema.Type.@struct Struct
        {
            get => _which == WHICH.Struct ? (CapnpC.CSharp.Generator.Schema.Type.@struct)_content : null;
            set
            {
                _which = WHICH.Struct;
                _content = value;
            }
        }

        public CapnpC.CSharp.Generator.Schema.Type.@interface Interface
        {
            get => _which == WHICH.Interface ? (CapnpC.CSharp.Generator.Schema.Type.@interface)_content : null;
            set
            {
                _which = WHICH.Interface;
                _content = value;
            }
        }

        public CapnpC.CSharp.Generator.Schema.Type.anyPointer AnyPointer
        {
            get => _which == WHICH.AnyPointer ? (CapnpC.CSharp.Generator.Schema.Type.anyPointer)_content : null;
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
            public list.READER List => which == WHICH.List ? new list.READER(ctx) : default;
            public @enum.READER Enum => which == WHICH.Enum ? new @enum.READER(ctx) : default;
            public @struct.READER Struct => which == WHICH.Struct ? new @struct.READER(ctx) : default;
            public @interface.READER Interface => which == WHICH.Interface ? new @interface.READER(ctx) : default;
            public anyPointer.READER AnyPointer => which == WHICH.AnyPointer ? new anyPointer.READER(ctx) : default;
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

            public list.WRITER List
            {
                get => which == WHICH.List ? Rewrap<list.WRITER>() : default;
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

            public anyPointer.WRITER AnyPointer
            {
                get => which == WHICH.AnyPointer ? Rewrap<anyPointer.WRITER>() : default;
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x87e739250a60ea97UL)]
        public class list : ICapnpSerializable
        {
            public const UInt64 typeId = 0x87e739250a60ea97UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                ElementType = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type>(reader.ElementType);
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

            public CapnpC.CSharp.Generator.Schema.Type ElementType
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
                public CapnpC.CSharp.Generator.Schema.Type.READER ElementType => ctx.ReadStruct(0, CapnpC.CSharp.Generator.Schema.Type.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public CapnpC.CSharp.Generator.Schema.Type.WRITER ElementType
                {
                    get => BuildPointer<CapnpC.CSharp.Generator.Schema.Type.WRITER>(0);
                    set => Link(0, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x9e0e78711a7f87a9UL)]
        public class @enum : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9e0e78711a7f87a9UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                TypeId = reader.TypeId;
                Brand = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Brand>(reader.Brand);
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

            public CapnpC.CSharp.Generator.Schema.Brand Brand
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
                public CapnpC.CSharp.Generator.Schema.Brand.READER Brand => ctx.ReadStruct(0, CapnpC.CSharp.Generator.Schema.Brand.READER.create);
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

                public CapnpC.CSharp.Generator.Schema.Brand.WRITER Brand
                {
                    get => BuildPointer<CapnpC.CSharp.Generator.Schema.Brand.WRITER>(0);
                    set => Link(0, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xac3a6f60ef4cc6d3UL)]
        public class @struct : ICapnpSerializable
        {
            public const UInt64 typeId = 0xac3a6f60ef4cc6d3UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                TypeId = reader.TypeId;
                Brand = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Brand>(reader.Brand);
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

            public CapnpC.CSharp.Generator.Schema.Brand Brand
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
                public CapnpC.CSharp.Generator.Schema.Brand.READER Brand => ctx.ReadStruct(0, CapnpC.CSharp.Generator.Schema.Brand.READER.create);
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

                public CapnpC.CSharp.Generator.Schema.Brand.WRITER Brand
                {
                    get => BuildPointer<CapnpC.CSharp.Generator.Schema.Brand.WRITER>(0);
                    set => Link(0, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xed8bca69f7fb0cbfUL)]
        public class @interface : ICapnpSerializable
        {
            public const UInt64 typeId = 0xed8bca69f7fb0cbfUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                TypeId = reader.TypeId;
                Brand = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Brand>(reader.Brand);
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

            public CapnpC.CSharp.Generator.Schema.Brand Brand
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
                public CapnpC.CSharp.Generator.Schema.Brand.READER Brand => ctx.ReadStruct(0, CapnpC.CSharp.Generator.Schema.Brand.READER.create);
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

                public CapnpC.CSharp.Generator.Schema.Brand.WRITER Brand
                {
                    get => BuildPointer<CapnpC.CSharp.Generator.Schema.Brand.WRITER>(0);
                    set => Link(0, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xc2573fe8a23e49f1UL)]
        public class anyPointer : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc2573fe8a23e49f1UL;
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
                        Unconstrained = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type.anyPointer.unconstrained>(reader.Unconstrained);
                        break;
                    case WHICH.Parameter:
                        Parameter = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type.anyPointer.parameter>(reader.Parameter);
                        break;
                    case WHICH.ImplicitMethodParameter:
                        ImplicitMethodParameter = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type.anyPointer.implicitMethodParameter>(reader.ImplicitMethodParameter);
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

            public CapnpC.CSharp.Generator.Schema.Type.anyPointer.unconstrained Unconstrained
            {
                get => _which == WHICH.Unconstrained ? (CapnpC.CSharp.Generator.Schema.Type.anyPointer.unconstrained)_content : null;
                set
                {
                    _which = WHICH.Unconstrained;
                    _content = value;
                }
            }

            public CapnpC.CSharp.Generator.Schema.Type.anyPointer.parameter Parameter
            {
                get => _which == WHICH.Parameter ? (CapnpC.CSharp.Generator.Schema.Type.anyPointer.parameter)_content : null;
                set
                {
                    _which = WHICH.Parameter;
                    _content = value;
                }
            }

            public CapnpC.CSharp.Generator.Schema.Type.anyPointer.implicitMethodParameter ImplicitMethodParameter
            {
                get => _which == WHICH.ImplicitMethodParameter ? (CapnpC.CSharp.Generator.Schema.Type.anyPointer.implicitMethodParameter)_content : null;
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
                public unconstrained.READER Unconstrained => which == WHICH.Unconstrained ? new unconstrained.READER(ctx) : default;
                public parameter.READER Parameter => which == WHICH.Parameter ? new parameter.READER(ctx) : default;
                public implicitMethodParameter.READER ImplicitMethodParameter => which == WHICH.ImplicitMethodParameter ? new implicitMethodParameter.READER(ctx) : default;
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

                public unconstrained.WRITER Unconstrained
                {
                    get => which == WHICH.Unconstrained ? Rewrap<unconstrained.WRITER>() : default;
                }

                public parameter.WRITER Parameter
                {
                    get => which == WHICH.Parameter ? Rewrap<parameter.WRITER>() : default;
                }

                public implicitMethodParameter.WRITER ImplicitMethodParameter
                {
                    get => which == WHICH.ImplicitMethodParameter ? Rewrap<implicitMethodParameter.WRITER>() : default;
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x8e3b5f79fe593656UL)]
            public class unconstrained : ICapnpSerializable
            {
                public const UInt64 typeId = 0x8e3b5f79fe593656UL;
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

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x9dd1f724f4614a85UL)]
            public class parameter : ICapnpSerializable
            {
                public const UInt64 typeId = 0x9dd1f724f4614a85UL;
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

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xbaefc9120c56e274UL)]
            public class implicitMethodParameter : ICapnpSerializable
            {
                public const UInt64 typeId = 0xbaefc9120c56e274UL;
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

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x903455f06065422bUL)]
    public class Brand : ICapnpSerializable
    {
        public const UInt64 typeId = 0x903455f06065422bUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Scopes = reader.Scopes?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Brand.Scope>(_));
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

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Brand.Scope> Scopes
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
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Brand.Scope.READER> Scopes => ctx.ReadList(0).Cast(CapnpC.CSharp.Generator.Schema.Brand.Scope.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Brand.Scope.WRITER> Scopes
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Brand.Scope.WRITER>>(0);
                set => Link(0, value);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xabd73485a9636bc9UL)]
        public class Scope : ICapnpSerializable
        {
            public const UInt64 typeId = 0xabd73485a9636bc9UL;
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
                        Bind = reader.Bind?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Brand.Binding>(_));
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

            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Brand.Binding> Bind
            {
                get => _which == WHICH.Bind ? (IReadOnlyList<CapnpC.CSharp.Generator.Schema.Brand.Binding>)_content : null;
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
                public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Brand.Binding.READER> Bind => which == WHICH.Bind ? ctx.ReadList(0).Cast(CapnpC.CSharp.Generator.Schema.Brand.Binding.READER.create) : default;
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

                public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Brand.Binding.WRITER> Bind
                {
                    get => which == WHICH.Bind ? BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Brand.Binding.WRITER>>(0) : default;
                    set => Link(0, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xc863cd16969ee7fcUL)]
        public class Binding : ICapnpSerializable
        {
            public const UInt64 typeId = 0xc863cd16969ee7fcUL;
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
                        Type = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Type>(reader.Type);
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

            public CapnpC.CSharp.Generator.Schema.Type Type
            {
                get => _which == WHICH.Type ? (CapnpC.CSharp.Generator.Schema.Type)_content : null;
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
                public CapnpC.CSharp.Generator.Schema.Type.READER Type => which == WHICH.Type ? ctx.ReadStruct(0, CapnpC.CSharp.Generator.Schema.Type.READER.create) : default;
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

                public CapnpC.CSharp.Generator.Schema.Type.WRITER Type
                {
                    get => which == WHICH.Type ? BuildPointer<CapnpC.CSharp.Generator.Schema.Type.WRITER>(0) : default;
                    set => Link(0, value);
                }
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xce23dcd2d7b00c9bUL)]
    public class Value : ICapnpSerializable
    {
        public const UInt64 typeId = 0xce23dcd2d7b00c9bUL;
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
                    List = CapnpSerializable.Create<object>(reader.List);
                    break;
                case WHICH.Enum:
                    Enum = reader.Enum;
                    break;
                case WHICH.Struct:
                    Struct = CapnpSerializable.Create<object>(reader.Struct);
                    break;
                case WHICH.Interface:
                    which = reader.which;
                    break;
                case WHICH.AnyPointer:
                    AnyPointer = CapnpSerializable.Create<object>(reader.AnyPointer);
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
            get => _which == WHICH.Bool ? (bool?)_content : null;
            set
            {
                _which = WHICH.Bool;
                _content = value;
            }
        }

        public sbyte? Int8
        {
            get => _which == WHICH.Int8 ? (sbyte?)_content : null;
            set
            {
                _which = WHICH.Int8;
                _content = value;
            }
        }

        public short? Int16
        {
            get => _which == WHICH.Int16 ? (short?)_content : null;
            set
            {
                _which = WHICH.Int16;
                _content = value;
            }
        }

        public int? Int32
        {
            get => _which == WHICH.Int32 ? (int?)_content : null;
            set
            {
                _which = WHICH.Int32;
                _content = value;
            }
        }

        public long? Int64
        {
            get => _which == WHICH.Int64 ? (long?)_content : null;
            set
            {
                _which = WHICH.Int64;
                _content = value;
            }
        }

        public byte? Uint8
        {
            get => _which == WHICH.Uint8 ? (byte?)_content : null;
            set
            {
                _which = WHICH.Uint8;
                _content = value;
            }
        }

        public ushort? Uint16
        {
            get => _which == WHICH.Uint16 ? (ushort?)_content : null;
            set
            {
                _which = WHICH.Uint16;
                _content = value;
            }
        }

        public uint? Uint32
        {
            get => _which == WHICH.Uint32 ? (uint?)_content : null;
            set
            {
                _which = WHICH.Uint32;
                _content = value;
            }
        }

        public ulong? Uint64
        {
            get => _which == WHICH.Uint64 ? (ulong?)_content : null;
            set
            {
                _which = WHICH.Uint64;
                _content = value;
            }
        }

        public float? Float32
        {
            get => _which == WHICH.Float32 ? (float?)_content : null;
            set
            {
                _which = WHICH.Float32;
                _content = value;
            }
        }

        public double? Float64
        {
            get => _which == WHICH.Float64 ? (double?)_content : null;
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

        public object List
        {
            get => _which == WHICH.List ? (object)_content : null;
            set
            {
                _which = WHICH.List;
                _content = value;
            }
        }

        public ushort? Enum
        {
            get => _which == WHICH.Enum ? (ushort?)_content : null;
            set
            {
                _which = WHICH.Enum;
                _content = value;
            }
        }

        public object Struct
        {
            get => _which == WHICH.Struct ? (object)_content : null;
            set
            {
                _which = WHICH.Struct;
                _content = value;
            }
        }

        public object AnyPointer
        {
            get => _which == WHICH.AnyPointer ? (object)_content : null;
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
            public string Text => which == WHICH.Text ? ctx.ReadText(0, null) : default;
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
                get => which == WHICH.Text ? this.ReadText(0, null) : default;
                set => this.WriteText(0, value, null);
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

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xf1c8950dab257542UL)]
    public class Annotation : ICapnpSerializable
    {
        public const UInt64 typeId = 0xf1c8950dab257542UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Id = reader.Id;
            Value = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Value>(reader.Value);
            Brand = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Brand>(reader.Brand);
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

        public CapnpC.CSharp.Generator.Schema.Value Value
        {
            get;
            set;
        }

        public CapnpC.CSharp.Generator.Schema.Brand Brand
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
            public CapnpC.CSharp.Generator.Schema.Value.READER Value => ctx.ReadStruct(0, CapnpC.CSharp.Generator.Schema.Value.READER.create);
            public CapnpC.CSharp.Generator.Schema.Brand.READER Brand => ctx.ReadStruct(1, CapnpC.CSharp.Generator.Schema.Brand.READER.create);
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

            public CapnpC.CSharp.Generator.Schema.Value.WRITER Value
            {
                get => BuildPointer<CapnpC.CSharp.Generator.Schema.Value.WRITER>(0);
                set => Link(0, value);
            }

            public CapnpC.CSharp.Generator.Schema.Brand.WRITER Brand
            {
                get => BuildPointer<CapnpC.CSharp.Generator.Schema.Brand.WRITER>(1);
                set => Link(1, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xd1958f7dba521926UL)]
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

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xd85d305b7d839963UL)]
    public class CapnpVersion : ICapnpSerializable
    {
        public const UInt64 typeId = 0xd85d305b7d839963UL;
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

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xbfc546f6210ad7ceUL)]
    public class CodeGeneratorRequest : ICapnpSerializable
    {
        public const UInt64 typeId = 0xbfc546f6210ad7ceUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Nodes = reader.Nodes?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node>(_));
            RequestedFiles = reader.RequestedFiles?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile>(_));
            CapnpVersion = CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.CapnpVersion>(reader.CapnpVersion);
            SourceInfo = reader.SourceInfo?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.Node.SourceInfo>(_));
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

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node> Nodes
        {
            get;
            set;
        }

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile> RequestedFiles
        {
            get;
            set;
        }

        public CapnpC.CSharp.Generator.Schema.CapnpVersion CapnpVersion
        {
            get;
            set;
        }

        public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.SourceInfo> SourceInfo
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
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.READER> Nodes => ctx.ReadList(0).Cast(CapnpC.CSharp.Generator.Schema.Node.READER.create);
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile.READER> RequestedFiles => ctx.ReadList(1).Cast(CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile.READER.create);
            public CapnpC.CSharp.Generator.Schema.CapnpVersion.READER CapnpVersion => ctx.ReadStruct(2, CapnpC.CSharp.Generator.Schema.CapnpVersion.READER.create);
            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.Node.SourceInfo.READER> SourceInfo => ctx.ReadList(3).Cast(CapnpC.CSharp.Generator.Schema.Node.SourceInfo.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 4);
            }

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.WRITER> Nodes
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.WRITER>>(0);
                set => Link(0, value);
            }

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile.WRITER> RequestedFiles
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile.WRITER>>(1);
                set => Link(1, value);
            }

            public CapnpC.CSharp.Generator.Schema.CapnpVersion.WRITER CapnpVersion
            {
                get => BuildPointer<CapnpC.CSharp.Generator.Schema.CapnpVersion.WRITER>(2);
                set => Link(2, value);
            }

            public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.SourceInfo.WRITER> SourceInfo
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.Node.SourceInfo.WRITER>>(3);
                set => Link(3, value);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xcfea0eb02e810062UL)]
        public class RequestedFile : ICapnpSerializable
        {
            public const UInt64 typeId = 0xcfea0eb02e810062UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Id = reader.Id;
                Filename = reader.Filename;
                Imports = reader.Imports?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile.Import>(_));
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

            public IReadOnlyList<CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile.Import> Imports
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
                public string Filename => ctx.ReadText(0, null);
                public IReadOnlyList<CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile.Import.READER> Imports => ctx.ReadList(1).Cast(CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile.Import.READER.create);
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
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile.Import.WRITER> Imports
                {
                    get => BuildPointer<ListOfStructsSerializer<CapnpC.CSharp.Generator.Schema.CodeGeneratorRequest.RequestedFile.Import.WRITER>>(1);
                    set => Link(1, value);
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xae504193122357e5UL)]
            public class Import : ICapnpSerializable
            {
                public const UInt64 typeId = 0xae504193122357e5UL;
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
                    public string Name => ctx.ReadText(0, null);
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
                        get => this.ReadText(0, null);
                        set => this.WriteText(0, value, null);
                    }
                }
            }
        }
    }
}