using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp
{
    public class TestJsonAnnotations : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            SomeField = reader.SomeField;
            AGroup = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@aGroup>(reader.AGroup);
            PrefixedGroup = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@prefixedGroup>(reader.PrefixedGroup);
            AUnion = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@aUnion>(reader.AUnion);
            Dependency = CapnpSerializable.Create<Capnp.TestJsonAnnotations2>(reader.Dependency);
            SimpleGroup = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@simpleGroup>(reader.SimpleGroup);
            Enums = reader.Enums;
            InnerJson = CapnpSerializable.Create<Capnp.Json.Value>(reader.InnerJson);
            CustomFieldHandler = reader.CustomFieldHandler;
            TestBase64 = reader.TestBase64;
            TestHex = reader.TestHex;
            BUnion = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@bUnion>(reader.BUnion);
            ExternalUnion = CapnpSerializable.Create<Capnp.TestJsonAnnotations3>(reader.ExternalUnion);
            UnionWithVoid = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@unionWithVoid>(reader.UnionWithVoid);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.SomeField = SomeField;
            AGroup?.serialize(writer.AGroup);
            PrefixedGroup?.serialize(writer.PrefixedGroup);
            AUnion?.serialize(writer.AUnion);
            Dependency?.serialize(writer.Dependency);
            SimpleGroup?.serialize(writer.SimpleGroup);
            writer.Enums.Init(Enums);
            InnerJson?.serialize(writer.InnerJson);
            writer.CustomFieldHandler = CustomFieldHandler;
            writer.TestBase64.Init(TestBase64);
            writer.TestHex.Init(TestHex);
            BUnion?.serialize(writer.BUnion);
            ExternalUnion?.serialize(writer.ExternalUnion);
            UnionWithVoid?.serialize(writer.UnionWithVoid);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string SomeField
        {
            get;
            set;
        }

        public Capnp.TestJsonAnnotations.@aGroup AGroup
        {
            get;
            set;
        }

        public Capnp.TestJsonAnnotations.@prefixedGroup PrefixedGroup
        {
            get;
            set;
        }

        public Capnp.TestJsonAnnotations.@aUnion AUnion
        {
            get;
            set;
        }

        public Capnp.TestJsonAnnotations2 Dependency
        {
            get;
            set;
        }

        public Capnp.TestJsonAnnotations.@simpleGroup SimpleGroup
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.TestJsonAnnotatedEnum> Enums
        {
            get;
            set;
        }

        public Capnp.Json.Value InnerJson
        {
            get;
            set;
        }

        public string CustomFieldHandler
        {
            get;
            set;
        }

        public IReadOnlyList<byte> TestBase64
        {
            get;
            set;
        }

        public IReadOnlyList<byte> TestHex
        {
            get;
            set;
        }

        public Capnp.TestJsonAnnotations.@bUnion BUnion
        {
            get;
            set;
        }

        public Capnp.TestJsonAnnotations3 ExternalUnion
        {
            get;
            set;
        }

        public Capnp.TestJsonAnnotations.@unionWithVoid UnionWithVoid
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
            public string SomeField => ctx.ReadText(0, "");
            public @aGroup.READER AGroup => new @aGroup.READER(ctx);
            public @prefixedGroup.READER PrefixedGroup => new @prefixedGroup.READER(ctx);
            public @aUnion.READER AUnion => new @aUnion.READER(ctx);
            public Capnp.TestJsonAnnotations2.READER Dependency => ctx.ReadStruct(6, Capnp.TestJsonAnnotations2.READER.create);
            public @simpleGroup.READER SimpleGroup => new @simpleGroup.READER(ctx);
            public IReadOnlyList<Capnp.TestJsonAnnotatedEnum> Enums => ctx.ReadList(8).CastEnums(_0 => (Capnp.TestJsonAnnotatedEnum)_0);
            public Capnp.Json.Value.READER InnerJson => ctx.ReadStruct(9, Capnp.Json.Value.READER.create);
            public string CustomFieldHandler => ctx.ReadText(10, "");
            public IReadOnlyList<byte> TestBase64 => ctx.ReadList(11).CastByte();
            public IReadOnlyList<byte> TestHex => ctx.ReadList(12).CastByte();
            public @bUnion.READER BUnion => new @bUnion.READER(ctx);
            public Capnp.TestJsonAnnotations3.READER ExternalUnion => ctx.ReadStruct(14, Capnp.TestJsonAnnotations3.READER.create);
            public @unionWithVoid.READER UnionWithVoid => new @unionWithVoid.READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(4, 16);
            }

            public string SomeField
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }

            public @aGroup.WRITER AGroup
            {
                get => Rewrap<@aGroup.WRITER>();
            }

            public @prefixedGroup.WRITER PrefixedGroup
            {
                get => Rewrap<@prefixedGroup.WRITER>();
            }

            public @aUnion.WRITER AUnion
            {
                get => Rewrap<@aUnion.WRITER>();
            }

            public Capnp.TestJsonAnnotations2.WRITER Dependency
            {
                get => BuildPointer<Capnp.TestJsonAnnotations2.WRITER>(6);
                set => Link(6, value);
            }

            public @simpleGroup.WRITER SimpleGroup
            {
                get => Rewrap<@simpleGroup.WRITER>();
            }

            public ListOfPrimitivesSerializer<Capnp.TestJsonAnnotatedEnum> Enums
            {
                get => BuildPointer<ListOfPrimitivesSerializer<Capnp.TestJsonAnnotatedEnum>>(8);
                set => Link(8, value);
            }

            public Capnp.Json.Value.WRITER InnerJson
            {
                get => BuildPointer<Capnp.Json.Value.WRITER>(9);
                set => Link(9, value);
            }

            public string CustomFieldHandler
            {
                get => this.ReadText(10, "");
                set => this.WriteText(10, value, "");
            }

            public ListOfPrimitivesSerializer<byte> TestBase64
            {
                get => BuildPointer<ListOfPrimitivesSerializer<byte>>(11);
                set => Link(11, value);
            }

            public ListOfPrimitivesSerializer<byte> TestHex
            {
                get => BuildPointer<ListOfPrimitivesSerializer<byte>>(12);
                set => Link(12, value);
            }

            public @bUnion.WRITER BUnion
            {
                get => Rewrap<@bUnion.WRITER>();
            }

            public Capnp.TestJsonAnnotations3.WRITER ExternalUnion
            {
                get => BuildPointer<Capnp.TestJsonAnnotations3.WRITER>(14);
                set => Link(14, value);
            }

            public @unionWithVoid.WRITER UnionWithVoid
            {
                get => Rewrap<@unionWithVoid.WRITER>();
            }
        }

        public class @aGroup : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                FlatFoo = reader.FlatFoo;
                FlatBar = reader.FlatBar;
                FlatBaz = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@aGroup.@flatBaz>(reader.FlatBaz);
                DoubleFlat = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@aGroup.@doubleFlat>(reader.DoubleFlat);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.FlatFoo = FlatFoo;
                writer.FlatBar = FlatBar;
                FlatBaz?.serialize(writer.FlatBaz);
                DoubleFlat?.serialize(writer.DoubleFlat);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint FlatFoo
            {
                get;
                set;
            }

            public string FlatBar
            {
                get;
                set;
            }

            public Capnp.TestJsonAnnotations.@aGroup.@flatBaz FlatBaz
            {
                get;
                set;
            }

            public Capnp.TestJsonAnnotations.@aGroup.@doubleFlat DoubleFlat
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
                public uint FlatFoo => ctx.ReadDataUInt(0UL, 0U);
                public string FlatBar => ctx.ReadText(1, "");
                public @flatBaz.READER FlatBaz => new @flatBaz.READER(ctx);
                public @doubleFlat.READER DoubleFlat => new @doubleFlat.READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public uint FlatFoo
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }

                public string FlatBar
                {
                    get => this.ReadText(1, "");
                    set => this.WriteText(1, value, "");
                }

                public @flatBaz.WRITER FlatBaz
                {
                    get => Rewrap<@flatBaz.WRITER>();
                }

                public @doubleFlat.WRITER DoubleFlat
                {
                    get => Rewrap<@doubleFlat.WRITER>();
                }
            }

            public class @flatBaz : ICapnpSerializable
            {
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Hello = reader.Hello;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Hello = Hello;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public bool Hello
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
                    public bool Hello => ctx.ReadDataBool(32UL, false);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public bool Hello
                    {
                        get => this.ReadDataBool(32UL, false);
                        set => this.WriteData(32UL, value, false);
                    }
                }
            }

            public class @doubleFlat : ICapnpSerializable
            {
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    FlatQux = reader.FlatQux;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.FlatQux = FlatQux;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public string FlatQux
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
                    public string FlatQux => ctx.ReadText(2, "");
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public string FlatQux
                    {
                        get => this.ReadText(2, "");
                        set => this.WriteText(2, value, "");
                    }
                }
            }
        }

        public class @prefixedGroup : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Foo = reader.Foo;
                Bar = reader.Bar;
                Baz = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@prefixedGroup.@baz>(reader.Baz);
                MorePrefix = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@prefixedGroup.@morePrefix>(reader.MorePrefix);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Foo = Foo;
                writer.Bar = Bar;
                Baz?.serialize(writer.Baz);
                MorePrefix?.serialize(writer.MorePrefix);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Foo
            {
                get;
                set;
            }

            public uint Bar
            {
                get;
                set;
            }

            public Capnp.TestJsonAnnotations.@prefixedGroup.@baz Baz
            {
                get;
                set;
            }

            public Capnp.TestJsonAnnotations.@prefixedGroup.@morePrefix MorePrefix
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
                public string Foo => ctx.ReadText(3, "");
                public uint Bar => ctx.ReadDataUInt(64UL, 0U);
                public @baz.READER Baz => new @baz.READER(ctx);
                public @morePrefix.READER MorePrefix => new @morePrefix.READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public string Foo
                {
                    get => this.ReadText(3, "");
                    set => this.WriteText(3, value, "");
                }

                public uint Bar
                {
                    get => this.ReadDataUInt(64UL, 0U);
                    set => this.WriteData(64UL, value, 0U);
                }

                public @baz.WRITER Baz
                {
                    get => Rewrap<@baz.WRITER>();
                }

                public @morePrefix.WRITER MorePrefix
                {
                    get => Rewrap<@morePrefix.WRITER>();
                }
            }

            public class @baz : ICapnpSerializable
            {
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Hello = reader.Hello;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Hello = Hello;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public bool Hello
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
                    public bool Hello => ctx.ReadDataBool(33UL, false);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public bool Hello
                    {
                        get => this.ReadDataBool(33UL, false);
                        set => this.WriteData(33UL, value, false);
                    }
                }
            }

            public class @morePrefix : ICapnpSerializable
            {
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Qux = reader.Qux;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Qux = Qux;
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
                    public string Qux => ctx.ReadText(4, "");
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public string Qux
                    {
                        get => this.ReadText(4, "");
                        set => this.WriteText(4, value, "");
                    }
                }
            }
        }

        public class @aUnion : ICapnpSerializable
        {
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
                        Foo = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@aUnion.@foo>(reader.Foo);
                        break;
                    case WHICH.Bar:
                        Bar = CapnpSerializable.Create<Capnp.TestJsonAnnotations.@aUnion.@bar>(reader.Bar);
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
                        Foo?.serialize(writer.Foo);
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

            public Capnp.TestJsonAnnotations.@aUnion.@foo Foo
            {
                get => _which == WHICH.Foo ? (Capnp.TestJsonAnnotations.@aUnion.@foo)_content : null;
                set
                {
                    _which = WHICH.Foo;
                    _content = value;
                }
            }

            public Capnp.TestJsonAnnotations.@aUnion.@bar Bar
            {
                get => _which == WHICH.Bar ? (Capnp.TestJsonAnnotations.@aUnion.@bar)_content : null;
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
                public WHICH which => (WHICH)ctx.ReadDataUShort(48U, (ushort)0);
                public @foo.READER Foo => which == WHICH.Foo ? new @foo.READER(ctx) : default;
                public @bar.READER Bar => which == WHICH.Bar ? new @bar.READER(ctx) : default;
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

                public @foo.WRITER Foo
                {
                    get => which == WHICH.Foo ? Rewrap<@foo.WRITER>() : default;
                }

                public @bar.WRITER Bar
                {
                    get => which == WHICH.Bar ? Rewrap<@bar.WRITER>() : default;
                }
            }

            public class @foo : ICapnpSerializable
            {
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    FooMember = reader.FooMember;
                    MultiMember = reader.MultiMember;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.FooMember = FooMember;
                    writer.MultiMember = MultiMember;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public string FooMember
                {
                    get;
                    set;
                }

                public uint MultiMember
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
                    public string FooMember => ctx.ReadText(5, "");
                    public uint MultiMember => ctx.ReadDataUInt(96UL, 0U);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public string FooMember
                    {
                        get => this.ReadText(5, "");
                        set => this.WriteText(5, value, "");
                    }

                    public uint MultiMember
                    {
                        get => this.ReadDataUInt(96UL, 0U);
                        set => this.WriteData(96UL, value, 0U);
                    }
                }
            }

            public class @bar : ICapnpSerializable
            {
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    BarMember = reader.BarMember;
                    MultiMember = reader.MultiMember;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.BarMember = BarMember;
                    writer.MultiMember = MultiMember;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public uint BarMember
                {
                    get;
                    set;
                }

                public string MultiMember
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
                    public uint BarMember => ctx.ReadDataUInt(96UL, 0U);
                    public string MultiMember => ctx.ReadText(5, "");
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public uint BarMember
                    {
                        get => this.ReadDataUInt(96UL, 0U);
                        set => this.WriteData(96UL, value, 0U);
                    }

                    public string MultiMember
                    {
                        get => this.ReadText(5, "");
                        set => this.WriteText(5, value, "");
                    }
                }
            }
        }

        public class @simpleGroup : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Grault = reader.Grault;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Grault = Grault;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Grault
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
                public string Grault => ctx.ReadText(7, "");
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public string Grault
                {
                    get => this.ReadText(7, "");
                    set => this.WriteText(7, value, "");
                }
            }
        }

        public class @bUnion : ICapnpSerializable
        {
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
                            _content = null;
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
                        writer.Foo = Foo;
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

            public string Foo
            {
                get => _which == WHICH.Foo ? (string)_content : null;
                set
                {
                    _which = WHICH.Foo;
                    _content = value;
                }
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
                public WHICH which => (WHICH)ctx.ReadDataUShort(128U, (ushort)0);
                public string Foo => which == WHICH.Foo ? ctx.ReadText(13, "") : default;
                public uint Bar => which == WHICH.Bar ? ctx.ReadDataUInt(160UL, 0U) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(128U, (ushort)0);
                    set => this.WriteData(128U, (ushort)value, (ushort)0);
                }

                public string Foo
                {
                    get => which == WHICH.Foo ? this.ReadText(13, "") : default;
                    set => this.WriteText(13, value, "");
                }

                public uint Bar
                {
                    get => which == WHICH.Bar ? this.ReadDataUInt(160UL, 0U) : default;
                    set => this.WriteData(160UL, value, 0U);
                }
            }
        }

        public class @unionWithVoid : ICapnpSerializable
        {
            public enum WHICH : ushort
            {
                IntValue = 0,
                VoidValue = 1,
                TextValue = 2,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.IntValue:
                        IntValue = reader.IntValue;
                        break;
                    case WHICH.VoidValue:
                        which = reader.which;
                        break;
                    case WHICH.TextValue:
                        TextValue = reader.TextValue;
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
                        case WHICH.IntValue:
                            _content = 0;
                            break;
                        case WHICH.VoidValue:
                            break;
                        case WHICH.TextValue:
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
                    case WHICH.IntValue:
                        writer.IntValue = IntValue.Value;
                        break;
                    case WHICH.VoidValue:
                        break;
                    case WHICH.TextValue:
                        writer.TextValue = TextValue;
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

            public uint? IntValue
            {
                get => _which == WHICH.IntValue ? (uint? )_content : null;
                set
                {
                    _which = WHICH.IntValue;
                    _content = value;
                }
            }

            public string TextValue
            {
                get => _which == WHICH.TextValue ? (string)_content : null;
                set
                {
                    _which = WHICH.TextValue;
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
                public WHICH which => (WHICH)ctx.ReadDataUShort(144U, (ushort)0);
                public uint IntValue => which == WHICH.IntValue ? ctx.ReadDataUInt(192UL, 0U) : default;
                public string TextValue => which == WHICH.TextValue ? ctx.ReadText(15, "") : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(144U, (ushort)0);
                    set => this.WriteData(144U, (ushort)value, (ushort)0);
                }

                public uint IntValue
                {
                    get => which == WHICH.IntValue ? this.ReadDataUInt(192UL, 0U) : default;
                    set => this.WriteData(192UL, value, 0U);
                }

                public string TextValue
                {
                    get => which == WHICH.TextValue ? this.ReadText(15, "") : default;
                    set => this.WriteText(15, value, "");
                }
            }
        }
    }

    public class TestJsonAnnotations2 : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Foo = reader.Foo;
            Cycle = CapnpSerializable.Create<Capnp.TestJsonAnnotations>(reader.Cycle);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Foo = Foo;
            Cycle?.serialize(writer.Cycle);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Foo
        {
            get;
            set;
        }

        public Capnp.TestJsonAnnotations Cycle
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
            public string Foo => ctx.ReadText(0, "");
            public Capnp.TestJsonAnnotations.READER Cycle => ctx.ReadStruct(1, Capnp.TestJsonAnnotations.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 2);
            }

            public string Foo
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }

            public Capnp.TestJsonAnnotations.WRITER Cycle
            {
                get => BuildPointer<Capnp.TestJsonAnnotations.WRITER>(1);
                set => Link(1, value);
            }
        }
    }

    public class TestJsonAnnotations3 : ICapnpSerializable
    {
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
                    Bar = CapnpSerializable.Create<Capnp.TestFlattenedStruct>(reader.Bar);
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
                    writer.Foo = Foo.Value;
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

        public uint? Foo
        {
            get => _which == WHICH.Foo ? (uint? )_content : null;
            set
            {
                _which = WHICH.Foo;
                _content = value;
            }
        }

        public Capnp.TestFlattenedStruct Bar
        {
            get => _which == WHICH.Bar ? (Capnp.TestFlattenedStruct)_content : null;
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
            public uint Foo => which == WHICH.Foo ? ctx.ReadDataUInt(0UL, 0U) : default;
            public Capnp.TestFlattenedStruct.READER Bar => which == WHICH.Bar ? ctx.ReadStruct(0, Capnp.TestFlattenedStruct.READER.create) : default;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(32U, (ushort)0);
                set => this.WriteData(32U, (ushort)value, (ushort)0);
            }

            public uint Foo
            {
                get => which == WHICH.Foo ? this.ReadDataUInt(0UL, 0U) : default;
                set => this.WriteData(0UL, value, 0U);
            }

            public Capnp.TestFlattenedStruct.WRITER Bar
            {
                get => which == WHICH.Bar ? BuildPointer<Capnp.TestFlattenedStruct.WRITER>(0) : default;
                set => Link(0, value);
            }
        }
    }

    public class TestFlattenedStruct : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Value = reader.Value;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Value = Value;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Value
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
            public string Value => ctx.ReadText(0, "");
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public string Value
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }
        }
    }

    public enum TestJsonAnnotatedEnum : ushort
    {
        foo,
        bar,
        baz,
        qux
    }
}