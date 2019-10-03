using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpGen
{
    public class TestImport : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Field = CapnpSerializable.Create<Capnproto_test.Capnp.Test.TestAllTypes>(reader.Field);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Field?.serialize(writer.Field);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnproto_test.Capnp.Test.TestAllTypes Field
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
            public Capnproto_test.Capnp.Test.TestAllTypes.READER Field => ctx.ReadStruct(0, Capnproto_test.Capnp.Test.TestAllTypes.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public Capnproto_test.Capnp.Test.TestAllTypes.WRITER Field
            {
                get => BuildPointer<Capnproto_test.Capnp.Test.TestAllTypes.WRITER>(0);
                set => Link(0, value);
            }
        }
    }
}