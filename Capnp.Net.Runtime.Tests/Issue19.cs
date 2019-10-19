using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpGen
{
    [TypeId(0x8f85f6df684d47b5UL), Proxy(typeof(GenericAProxy<>)), Skeleton(typeof(GenericASkeleton<>))]
    public interface IGenericA<TT> : IDisposable where TT : class
    {
        Task MethodA(TT param1, CancellationToken cancellationToken_ = default);
    }

    public class GenericAProxy<TT> : Proxy, IGenericA<TT> where TT : class
    {
        public async Task MethodA(TT param1, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<CapnpGen.GenericA<TT>.Params_methodA.WRITER>();
            var arg_ = new CapnpGen.GenericA<TT>.Params_methodA()
            {Param1 = param1};
            arg_.serialize(in_);
            var d_ = await Call(10341943558714247093UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned;
            var r_ = CapnpSerializable.Create<CapnpGen.GenericA<TT>.Result_methodA>(d_);
            return;
        }
    }

    public class GenericASkeleton<TT> : Skeleton<IGenericA<TT>> where TT : class
    {
        public GenericASkeleton()
        {
            SetMethodTable(MethodA);
        }

        public override ulong InterfaceId => 10341943558714247093UL;
        async Task<AnswerOrCounterquestion> MethodA(DeserializerState d_, CancellationToken cancellationToken_)
        {
            var in_ = CapnpSerializable.Create<CapnpGen.GenericA<TT>.Params_methodA>(d_);
            await Impl.MethodA(in_.Param1, cancellationToken_);
            var s_ = SerializerState.CreateForRpc<CapnpGen.GenericA<TT>.Result_methodA.WRITER>();
            return s_;
        }
    }

    public static class GenericA<TT>
        where TT : class
    {
        [TypeId(0xe67c3d73f913c24fUL)]
        public class Params_methodA : ICapnpSerializable
        {
            public const UInt64 typeId = 0xe67c3d73f913c24fUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Param1 = CapnpSerializable.Create<TT>(reader.Param1);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Param1.SetObject(Param1);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public TT Param1
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
                public DeserializerState Param1 => ctx.StructReadPointer(0);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public DynamicSerializerState Param1
                {
                    get => BuildPointer<DynamicSerializerState>(0);
                    set => Link(0, value);
                }
            }
        }

        [TypeId(0xd6791784c41c75f3UL)]
        public class Result_methodA : ICapnpSerializable
        {
            public const UInt64 typeId = 0xd6791784c41c75f3UL;
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
    }

    [TypeId(0xe3cd928b8338eac9UL), Proxy(typeof(B2Proxy)), Skeleton(typeof(B2Skeleton))]
    public interface IB2 : CapnpGen.IGenericA<string>
    {
        Task<string> MethodB(long param1, CancellationToken cancellationToken_ = default);
    }

    public class B2Proxy : Proxy, IB2
    {
        public async Task<string> MethodB(long param1, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<CapnpGen.B2.Params_methodB.WRITER>();
            var arg_ = new CapnpGen.B2.Params_methodB()
            {Param1 = param1};
            arg_.serialize(in_);
            var d_ = await Call(16414937344734980809UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned;
            var r_ = CapnpSerializable.Create<CapnpGen.B2.Result_methodB>(d_);
            return (r_.Res);
        }

        public async Task MethodA(string param1, CancellationToken cancellationToken_ = default)
        {
            var in_ = SerializerState.CreateForRpc<CapnpGen.GenericA<string>.Params_methodA.WRITER>();
            var arg_ = new CapnpGen.GenericA<string>.Params_methodA()
            {Param1 = param1};
            arg_.serialize(in_);
            var d_ = await Call(10341943558714247093UL, 0, in_.Rewrap<DynamicSerializerState>(), false, cancellationToken_).WhenReturned;
            var r_ = CapnpSerializable.Create<CapnpGen.GenericA<string>.Result_methodA>(d_);
            return;
        }
    }

    public class B2Skeleton : Skeleton<IB2>
    {
        public B2Skeleton()
        {
            SetMethodTable(MethodB);
        }

        public override ulong InterfaceId => 16414937344734980809UL;
        Task<AnswerOrCounterquestion> MethodB(DeserializerState d_, CancellationToken cancellationToken_)
        {
            var in_ = CapnpSerializable.Create<CapnpGen.B2.Params_methodB>(d_);
            return Impatient.MaybeTailCall(Impl.MethodB(in_.Param1, cancellationToken_), res =>
            {
                var s_ = SerializerState.CreateForRpc<CapnpGen.B2.Result_methodB.WRITER>();
                var r_ = new CapnpGen.B2.Result_methodB{Res = res};
                r_.serialize(s_);
                return s_;
            }

            );
        }
    }

    public static class B2
    {
        [TypeId(0xdf7b0019c911a776UL)]
        public class Params_methodB : ICapnpSerializable
        {
            public const UInt64 typeId = 0xdf7b0019c911a776UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Param1 = reader.Param1;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Param1 = Param1;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public long Param1
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
                public long Param1 => ctx.ReadDataLong(0UL, 0L);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public long Param1
                {
                    get => this.ReadDataLong(0UL, 0L);
                    set => this.WriteData(0UL, value, 0L);
                }
            }
        }

        [TypeId(0xea5fbbd3b20b4347UL)]
        public class Result_methodB : ICapnpSerializable
        {
            public const UInt64 typeId = 0xea5fbbd3b20b4347UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Res = reader.Res;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Res = Res;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Res
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
                public string Res => ctx.ReadText(0, "");
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public string Res
                {
                    get => this.ReadText(0, "");
                    set => this.WriteText(0, value, "");
                }
            }
        }
    }
}