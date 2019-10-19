using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpGen
{
    [TypeId(0xb706e295e5860f3dUL)]
    public class RpcRequest<TRequest> : ICapnpSerializable where TRequest : class
    {
        public const UInt64 typeId = 0xb706e295e5860f3dUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Method = reader.Method;
            Request = CapnpSerializable.Create<TRequest>(reader.Request);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Method = Method;
            writer.Request.SetObject(Request);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Method
        {
            get;
            set;
        }

        public TRequest Request
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
            public string Method => ctx.ReadText(0, "");
            public DeserializerState Request => ctx.StructReadPointer(1);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 2);
            }

            public string Method
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }

            public DynamicSerializerState Request
            {
                get => BuildPointer<DynamicSerializerState>(1);
                set => Link(1, value);
            }
        }
    }

    [TypeId(0xca749dac8d513c9fUL)]
    public class ArithmeticOperationRequest : ICapnpSerializable
    {
        public const UInt64 typeId = 0xca749dac8d513c9fUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            NumA = reader.NumA;
            NumB = reader.NumB;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.NumA = NumA;
            writer.NumB = NumB;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public int NumA
        {
            get;
            set;
        }

        public int NumB
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
            public int NumA => ctx.ReadDataInt(0UL, 0);
            public int NumB => ctx.ReadDataInt(32UL, 0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public int NumA
            {
                get => this.ReadDataInt(0UL, 0);
                set => this.WriteData(0UL, value, 0);
            }

            public int NumB
            {
                get => this.ReadDataInt(32UL, 0);
                set => this.WriteData(32UL, value, 0);
            }
        }
    }

    [TypeId(0xc64f52df07418506UL)]
    public class ArithmeticOperationReply : ICapnpSerializable
    {
        public const UInt64 typeId = 0xc64f52df07418506UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Result = reader.Result;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Result = Result;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public int Result
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
            public int Result => ctx.ReadDataInt(0UL, 0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public int Result
            {
                get => this.ReadDataInt(0UL, 0);
                set => this.WriteData(0UL, value, 0);
            }
        }
    }
}
