using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Json
{
    public class Value : ICapnpSerializable
    {
        public enum WHICH : ushort
        {
            Null = 0,
            Boolean = 1,
            Number = 2,
            String = 3,
            Array = 4,
            Object = 5,
            TheCall = 6,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.Null:
                    which = reader.which;
                    break;
                case WHICH.Boolean:
                    Boolean = reader.Boolean;
                    break;
                case WHICH.Number:
                    Number = reader.Number;
                    break;
                case WHICH.String:
                    String = reader.String;
                    break;
                case WHICH.Array:
                    Array = reader.Array.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Json.Value>(_));
                    break;
                case WHICH.Object:
                    Object = reader.Object.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Json.Value.Field>(_));
                    break;
                case WHICH.TheCall:
                    TheCall = CapnpSerializable.Create<Capnp.Json.Value.Call>(reader.TheCall);
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
                    case WHICH.Null:
                        break;
                    case WHICH.Boolean:
                        _content = false;
                        break;
                    case WHICH.Number:
                        _content = 0;
                        break;
                    case WHICH.String:
                        _content = null;
                        break;
                    case WHICH.Array:
                        _content = null;
                        break;
                    case WHICH.Object:
                        _content = null;
                        break;
                    case WHICH.TheCall:
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
                case WHICH.Null:
                    break;
                case WHICH.Boolean:
                    writer.Boolean = Boolean.Value;
                    break;
                case WHICH.Number:
                    writer.Number = Number.Value;
                    break;
                case WHICH.String:
                    writer.String = String;
                    break;
                case WHICH.Array:
                    writer.Array.Init(Array, (_s1, _v1) => _v1?.serialize(_s1));
                    break;
                case WHICH.Object:
                    writer.Object.Init(Object, (_s1, _v1) => _v1?.serialize(_s1));
                    break;
                case WHICH.TheCall:
                    TheCall?.serialize(writer.TheCall);
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

        public bool? Boolean
        {
            get => _which == WHICH.Boolean ? (bool? )_content : null;
            set
            {
                _which = WHICH.Boolean;
                _content = value;
            }
        }

        public double? Number
        {
            get => _which == WHICH.Number ? (double? )_content : null;
            set
            {
                _which = WHICH.Number;
                _content = value;
            }
        }

        public string String
        {
            get => _which == WHICH.String ? (string)_content : null;
            set
            {
                _which = WHICH.String;
                _content = value;
            }
        }

        public IReadOnlyList<Capnp.Json.Value> Array
        {
            get => _which == WHICH.Array ? (IReadOnlyList<Capnp.Json.Value>)_content : null;
            set
            {
                _which = WHICH.Array;
                _content = value;
            }
        }

        public IReadOnlyList<Capnp.Json.Value.Field> Object
        {
            get => _which == WHICH.Object ? (IReadOnlyList<Capnp.Json.Value.Field>)_content : null;
            set
            {
                _which = WHICH.Object;
                _content = value;
            }
        }

        public Capnp.Json.Value.Call TheCall
        {
            get => _which == WHICH.TheCall ? (Capnp.Json.Value.Call)_content : null;
            set
            {
                _which = WHICH.TheCall;
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
            public bool Boolean => which == WHICH.Boolean ? ctx.ReadDataBool(16UL, false) : default;
            public double Number => which == WHICH.Number ? ctx.ReadDataDouble(64UL, 0) : default;
            public string String => which == WHICH.String ? ctx.ReadText(0, "") : default;
            public IReadOnlyList<Capnp.Json.Value.READER> Array => which == WHICH.Array ? ctx.ReadList(0).Cast(Capnp.Json.Value.READER.create) : default;
            public IReadOnlyList<Capnp.Json.Value.Field.READER> Object => which == WHICH.Object ? ctx.ReadList(0).Cast(Capnp.Json.Value.Field.READER.create) : default;
            public Capnp.Json.Value.Call.READER TheCall => which == WHICH.TheCall ? ctx.ReadStruct(0, Capnp.Json.Value.Call.READER.create) : default;
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

            public bool Boolean
            {
                get => which == WHICH.Boolean ? this.ReadDataBool(16UL, false) : default;
                set => this.WriteData(16UL, value, false);
            }

            public double Number
            {
                get => which == WHICH.Number ? this.ReadDataDouble(64UL, 0) : default;
                set => this.WriteData(64UL, value, 0);
            }

            public string String
            {
                get => which == WHICH.String ? this.ReadText(0, "") : default;
                set => this.WriteText(0, value, "");
            }

            public ListOfStructsSerializer<Capnp.Json.Value.WRITER> Array
            {
                get => which == WHICH.Array ? BuildPointer<ListOfStructsSerializer<Capnp.Json.Value.WRITER>>(0) : default;
                set => Link(0, value);
            }

            public ListOfStructsSerializer<Capnp.Json.Value.Field.WRITER> Object
            {
                get => which == WHICH.Object ? BuildPointer<ListOfStructsSerializer<Capnp.Json.Value.Field.WRITER>>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Json.Value.Call.WRITER TheCall
            {
                get => which == WHICH.TheCall ? BuildPointer<Capnp.Json.Value.Call.WRITER>(0) : default;
                set => Link(0, value);
            }
        }

        public class Field : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Name = reader.Name;
                Value = CapnpSerializable.Create<Capnp.Json.Value>(reader.Value);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Name = Name;
                Value?.serialize(writer.Value);
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

            public Capnp.Json.Value Value
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
                public Capnp.Json.Value.READER Value => ctx.ReadStruct(1, Capnp.Json.Value.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public string Name
                {
                    get => this.ReadText(0, "");
                    set => this.WriteText(0, value, "");
                }

                public Capnp.Json.Value.WRITER Value
                {
                    get => BuildPointer<Capnp.Json.Value.WRITER>(1);
                    set => Link(1, value);
                }
            }
        }

        public class Call : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Function = reader.Function;
                Params = reader.Params.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Json.Value>(_));
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Function = Function;
                writer.Params.Init(Params, (_s1, _v1) => _v1?.serialize(_s1));
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Function
            {
                get;
                set;
            }

            public IReadOnlyList<Capnp.Json.Value> Params
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
                public string Function => ctx.ReadText(0, "");
                public IReadOnlyList<Capnp.Json.Value.READER> Params => ctx.ReadList(1).Cast(Capnp.Json.Value.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public string Function
                {
                    get => this.ReadText(0, "");
                    set => this.WriteText(0, value, "");
                }

                public ListOfStructsSerializer<Capnp.Json.Value.WRITER> Params
                {
                    get => BuildPointer<ListOfStructsSerializer<Capnp.Json.Value.WRITER>>(1);
                    set => Link(1, value);
                }
            }
        }
    }

    public class FlattenOptions : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Prefix = reader.Prefix;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Prefix = Prefix;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
            Prefix = Prefix ?? "";
        }

        public string Prefix
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
            public string Prefix => ctx.ReadText(0, "");
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public string Prefix
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }
        }
    }

    public class DiscriminatorOptions : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Name = reader.Name;
            ValueName = reader.ValueName;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Name = Name;
            writer.ValueName = ValueName;
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

        public string ValueName
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
            public string ValueName => ctx.ReadText(1, "");
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 2);
            }

            public string Name
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }

            public string ValueName
            {
                get => this.ReadText(1, "");
                set => this.WriteText(1, value, "");
            }
        }
    }
}