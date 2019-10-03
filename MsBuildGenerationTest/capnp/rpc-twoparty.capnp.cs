using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc.Twoparty
{
    public enum Side : ushort
    {
        server,
        client
    }

    public class VatId : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Side = reader.Side;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Side = Side;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnp.Rpc.Twoparty.Side Side
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
            public Capnp.Rpc.Twoparty.Side Side => (Capnp.Rpc.Twoparty.Side)ctx.ReadDataUShort(0UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public Capnp.Rpc.Twoparty.Side Side
            {
                get => (Capnp.Rpc.Twoparty.Side)this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, (ushort)value, (ushort)0);
            }
        }
    }

    public class ProvisionId : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            JoinId = reader.JoinId;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.JoinId = JoinId;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint JoinId
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
            public uint JoinId => ctx.ReadDataUInt(0UL, 0U);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public uint JoinId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }
        }
    }

    public class RecipientId : ICapnpSerializable
    {
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

    public class ThirdPartyCapId : ICapnpSerializable
    {
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

    public class JoinKeyPart : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            JoinId = reader.JoinId;
            PartCount = reader.PartCount;
            PartNum = reader.PartNum;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.JoinId = JoinId;
            writer.PartCount = PartCount;
            writer.PartNum = PartNum;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint JoinId
        {
            get;
            set;
        }

        public ushort PartCount
        {
            get;
            set;
        }

        public ushort PartNum
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
            public uint JoinId => ctx.ReadDataUInt(0UL, 0U);
            public ushort PartCount => ctx.ReadDataUShort(32UL, (ushort)0);
            public ushort PartNum => ctx.ReadDataUShort(48UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public uint JoinId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public ushort PartCount
            {
                get => this.ReadDataUShort(32UL, (ushort)0);
                set => this.WriteData(32UL, value, (ushort)0);
            }

            public ushort PartNum
            {
                get => this.ReadDataUShort(48UL, (ushort)0);
                set => this.WriteData(48UL, value, (ushort)0);
            }
        }
    }

    public class JoinResult : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            JoinId = reader.JoinId;
            Succeeded = reader.Succeeded;
            Cap = CapnpSerializable.Create<AnyPointer>(reader.Cap);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.JoinId = JoinId;
            writer.Succeeded = Succeeded;
            writer.Cap.SetObject(Cap);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint JoinId
        {
            get;
            set;
        }

        public bool Succeeded
        {
            get;
            set;
        }

        public AnyPointer Cap
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
            public uint JoinId => ctx.ReadDataUInt(0UL, 0U);
            public bool Succeeded => ctx.ReadDataBool(32UL, false);
            public DeserializerState Cap => ctx.StructReadPointer(0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public uint JoinId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public bool Succeeded
            {
                get => this.ReadDataBool(32UL, false);
                set => this.WriteData(32UL, value, false);
            }

            public DynamicSerializerState Cap
            {
                get => BuildPointer<DynamicSerializerState>(0);
                set => Link(0, value);
            }
        }
    }
}