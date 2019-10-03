using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpGen
{
    public class Person : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Id = reader.Id;
            Name = reader.Name;
            Email = reader.Email;
            Phones = reader.Phones.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Person.PhoneNumber>(_));
            Employment = CapnpSerializable.Create<CapnpGen.Person.@employment>(reader.Employment);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Id = Id;
            writer.Name = Name;
            writer.Email = Email;
            writer.Phones.Init(Phones, (_s1, _v1) => _v1?.serialize(_s1));
            Employment?.serialize(writer.Employment);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public IReadOnlyList<CapnpGen.Person.PhoneNumber> Phones
        {
            get;
            set;
        }

        public CapnpGen.Person.@employment Employment
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
            public uint Id => ctx.ReadDataUInt(0UL, 0U);
            public string Name => ctx.ReadText(0, "");
            public string Email => ctx.ReadText(1, "");
            public IReadOnlyList<CapnpGen.Person.PhoneNumber.READER> Phones => ctx.ReadList(2).Cast(CapnpGen.Person.PhoneNumber.READER.create);
            public @employment.READER Employment => new @employment.READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 4);
            }

            public uint Id
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public string Name
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }

            public string Email
            {
                get => this.ReadText(1, "");
                set => this.WriteText(1, value, "");
            }

            public ListOfStructsSerializer<CapnpGen.Person.PhoneNumber.WRITER> Phones
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.Person.PhoneNumber.WRITER>>(2);
                set => Link(2, value);
            }

            public @employment.WRITER Employment
            {
                get => Rewrap<@employment.WRITER>();
            }
        }

        public class @employment : ICapnpSerializable
        {
            public enum WHICH : ushort
            {
                Unemployed = 0,
                Employer = 1,
                School = 2,
                SelfEmployed = 3,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Unemployed:
                        which = reader.which;
                        break;
                    case WHICH.Employer:
                        Employer = reader.Employer;
                        break;
                    case WHICH.School:
                        School = reader.School;
                        break;
                    case WHICH.SelfEmployed:
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
                        case WHICH.Unemployed:
                            break;
                        case WHICH.Employer:
                            _content = null;
                            break;
                        case WHICH.School:
                            _content = null;
                            break;
                        case WHICH.SelfEmployed:
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.Unemployed:
                        break;
                    case WHICH.Employer:
                        writer.Employer = Employer;
                        break;
                    case WHICH.School:
                        writer.School = School;
                        break;
                    case WHICH.SelfEmployed:
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

            public string Employer
            {
                get => _which == WHICH.Employer ? (string)_content : null;
                set
                {
                    _which = WHICH.Employer;
                    _content = value;
                }
            }

            public string School
            {
                get => _which == WHICH.School ? (string)_content : null;
                set
                {
                    _which = WHICH.School;
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
                public string Employer => which == WHICH.Employer ? ctx.ReadText(3, "") : default;
                public string School => which == WHICH.School ? ctx.ReadText(3, "") : default;
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

                public string Employer
                {
                    get => which == WHICH.Employer ? this.ReadText(3, "") : default;
                    set => this.WriteText(3, value, "");
                }

                public string School
                {
                    get => which == WHICH.School ? this.ReadText(3, "") : default;
                    set => this.WriteText(3, value, "");
                }
            }
        }

        public class PhoneNumber : ICapnpSerializable
        {
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Number = reader.Number;
                TheType = reader.TheType;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Number = Number;
                writer.TheType = TheType;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Number
            {
                get;
                set;
            }

            public CapnpGen.Person.PhoneNumber.Type TheType
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
                public string Number => ctx.ReadText(0, "");
                public CapnpGen.Person.PhoneNumber.Type TheType => (CapnpGen.Person.PhoneNumber.Type)ctx.ReadDataUShort(0UL, (ushort)0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public string Number
                {
                    get => this.ReadText(0, "");
                    set => this.WriteText(0, value, "");
                }

                public CapnpGen.Person.PhoneNumber.Type TheType
                {
                    get => (CapnpGen.Person.PhoneNumber.Type)this.ReadDataUShort(0UL, (ushort)0);
                    set => this.WriteData(0UL, (ushort)value, (ushort)0);
                }
            }

            public enum Type : ushort
            {
                mobile,
                home,
                work
            }
        }
    }

    public class AddressBook : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            People = reader.People.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Person>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.People.Init(People, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public IReadOnlyList<CapnpGen.Person> People
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
            public IReadOnlyList<CapnpGen.Person.READER> People => ctx.ReadList(0).Cast(CapnpGen.Person.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public ListOfStructsSerializer<CapnpGen.Person.WRITER> People
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.Person.WRITER>>(0);
                set => Link(0, value);
            }
        }
    }
}