using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpGen
{
    public class TestImport2 : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Foo = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(reader.Foo);
            Bar = CapnpSerializable.Create<Capnp.Schema.Node>(reader.Bar);
            Baz = CapnpSerializable.Create<CapnpGen.TestImport>(reader.Baz);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Foo?.serialize(writer.Foo);
            Bar?.serialize(writer.Bar);
            Baz?.serialize(writer.Baz);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestAllTypes Foo
        {
            get;
            set;
        }

        public Capnp.Schema.Node Bar
        {
            get;
            set;
        }

        public CapnpGen.TestImport Baz
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
            public Capnproto_test.Capnp.Test.TestAllTypes.READER Foo => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestAllTypes.READER.create);
            public Capnp.Schema.Node.READER Bar => ctx.ReadStruct(1, Capnp.Schema.Node.READER.create);
            public CapnpGen.TestImport.READER Baz => ctx.ReadStruct(2, CapnpGen.TestImport.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 3);
            }

            public Capnproto_test.Capnp.Test.TestAllTypes.WRITER Foo
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>(0);
                set => Link(0, value);
            }

            public Capnp.Schema.Node.WRITER Bar
            {
                get => BuildPointer<Capnp.Schema.Node.WRITER>(1);
                set => Link(1, value);
            }

            public CapnpGen.TestImport.WRITER Baz
            {
                get => BuildPointer<CapnpGen.TestImport.WRITER>(2);
                set => Link(2, value);
            }
        }
    }
}