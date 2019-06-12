using Capnp;
using Capnp.Rpc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capnp.Rpc
{
    public class Message : ICapnpSerializable
    {
        public enum WHICH : ushort
        {
            Unimplemented = 0,
            Abort = 1,
            Call = 2,
            Return = 3,
            Finish = 4,
            Resolve = 5,
            Release = 6,
            ObsoleteSave = 7,
            Bootstrap = 8,
            ObsoleteDelete = 9,
            Provide = 10,
            Accept = 11,
            Join = 12,
            Disembargo = 13,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.Unimplemented:
                    Unimplemented = CapnpSerializable.Create<Capnp.Rpc.Message>(reader.Unimplemented);
                    break;
                case WHICH.Abort:
                    Abort = CapnpSerializable.Create<Capnp.Rpc.Exception>(reader.Abort);
                    break;
                case WHICH.Call:
                    Call = CapnpSerializable.Create<Capnp.Rpc.Call>(reader.Call);
                    break;
                case WHICH.Return:
                    Return = CapnpSerializable.Create<Capnp.Rpc.Return>(reader.Return);
                    break;
                case WHICH.Finish:
                    Finish = CapnpSerializable.Create<Capnp.Rpc.Finish>(reader.Finish);
                    break;
                case WHICH.Resolve:
                    Resolve = CapnpSerializable.Create<Capnp.Rpc.Resolve>(reader.Resolve);
                    break;
                case WHICH.Release:
                    Release = CapnpSerializable.Create<Capnp.Rpc.Release>(reader.Release);
                    break;
                case WHICH.ObsoleteSave:
                    ObsoleteSave = CapnpSerializable.Create<AnyPointer>(reader.ObsoleteSave);
                    break;
                case WHICH.Bootstrap:
                    Bootstrap = CapnpSerializable.Create<Capnp.Rpc.Bootstrap>(reader.Bootstrap);
                    break;
                case WHICH.ObsoleteDelete:
                    ObsoleteDelete = CapnpSerializable.Create<AnyPointer>(reader.ObsoleteDelete);
                    break;
                case WHICH.Provide:
                    Provide = CapnpSerializable.Create<Capnp.Rpc.Provide>(reader.Provide);
                    break;
                case WHICH.Accept:
                    Accept = CapnpSerializable.Create<Capnp.Rpc.Accept>(reader.Accept);
                    break;
                case WHICH.Join:
                    Join = CapnpSerializable.Create<Capnp.Rpc.Join>(reader.Join);
                    break;
                case WHICH.Disembargo:
                    Disembargo = CapnpSerializable.Create<Capnp.Rpc.Disembargo>(reader.Disembargo);
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
                    case WHICH.Unimplemented:
                        _content = null;
                        break;
                    case WHICH.Abort:
                        _content = null;
                        break;
                    case WHICH.Call:
                        _content = null;
                        break;
                    case WHICH.Return:
                        _content = null;
                        break;
                    case WHICH.Finish:
                        _content = null;
                        break;
                    case WHICH.Resolve:
                        _content = null;
                        break;
                    case WHICH.Release:
                        _content = null;
                        break;
                    case WHICH.ObsoleteSave:
                        _content = null;
                        break;
                    case WHICH.Bootstrap:
                        _content = null;
                        break;
                    case WHICH.ObsoleteDelete:
                        _content = null;
                        break;
                    case WHICH.Provide:
                        _content = null;
                        break;
                    case WHICH.Accept:
                        _content = null;
                        break;
                    case WHICH.Join:
                        _content = null;
                        break;
                    case WHICH.Disembargo:
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
                case WHICH.Unimplemented:
                    Unimplemented?.serialize(writer.Unimplemented);
                    break;
                case WHICH.Abort:
                    Abort?.serialize(writer.Abort);
                    break;
                case WHICH.Call:
                    Call?.serialize(writer.Call);
                    break;
                case WHICH.Return:
                    Return?.serialize(writer.Return);
                    break;
                case WHICH.Finish:
                    Finish?.serialize(writer.Finish);
                    break;
                case WHICH.Resolve:
                    Resolve?.serialize(writer.Resolve);
                    break;
                case WHICH.Release:
                    Release?.serialize(writer.Release);
                    break;
                case WHICH.ObsoleteSave:
                    writer.ObsoleteSave.SetObject(ObsoleteSave);
                    break;
                case WHICH.Bootstrap:
                    Bootstrap?.serialize(writer.Bootstrap);
                    break;
                case WHICH.ObsoleteDelete:
                    writer.ObsoleteDelete.SetObject(ObsoleteDelete);
                    break;
                case WHICH.Provide:
                    Provide?.serialize(writer.Provide);
                    break;
                case WHICH.Accept:
                    Accept?.serialize(writer.Accept);
                    break;
                case WHICH.Join:
                    Join?.serialize(writer.Join);
                    break;
                case WHICH.Disembargo:
                    Disembargo?.serialize(writer.Disembargo);
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

        public Capnp.Rpc.Message Unimplemented
        {
            get => _which == WHICH.Unimplemented ? (Capnp.Rpc.Message)_content : null;
            set
            {
                _which = WHICH.Unimplemented;
                _content = value;
            }
        }

        public Capnp.Rpc.Exception Abort
        {
            get => _which == WHICH.Abort ? (Capnp.Rpc.Exception)_content : null;
            set
            {
                _which = WHICH.Abort;
                _content = value;
            }
        }

        public Capnp.Rpc.Call Call
        {
            get => _which == WHICH.Call ? (Capnp.Rpc.Call)_content : null;
            set
            {
                _which = WHICH.Call;
                _content = value;
            }
        }

        public Capnp.Rpc.Return Return
        {
            get => _which == WHICH.Return ? (Capnp.Rpc.Return)_content : null;
            set
            {
                _which = WHICH.Return;
                _content = value;
            }
        }

        public Capnp.Rpc.Finish Finish
        {
            get => _which == WHICH.Finish ? (Capnp.Rpc.Finish)_content : null;
            set
            {
                _which = WHICH.Finish;
                _content = value;
            }
        }

        public Capnp.Rpc.Resolve Resolve
        {
            get => _which == WHICH.Resolve ? (Capnp.Rpc.Resolve)_content : null;
            set
            {
                _which = WHICH.Resolve;
                _content = value;
            }
        }

        public Capnp.Rpc.Release Release
        {
            get => _which == WHICH.Release ? (Capnp.Rpc.Release)_content : null;
            set
            {
                _which = WHICH.Release;
                _content = value;
            }
        }

        public AnyPointer ObsoleteSave
        {
            get => _which == WHICH.ObsoleteSave ? (AnyPointer)_content : null;
            set
            {
                _which = WHICH.ObsoleteSave;
                _content = value;
            }
        }

        public Capnp.Rpc.Bootstrap Bootstrap
        {
            get => _which == WHICH.Bootstrap ? (Capnp.Rpc.Bootstrap)_content : null;
            set
            {
                _which = WHICH.Bootstrap;
                _content = value;
            }
        }

        public AnyPointer ObsoleteDelete
        {
            get => _which == WHICH.ObsoleteDelete ? (AnyPointer)_content : null;
            set
            {
                _which = WHICH.ObsoleteDelete;
                _content = value;
            }
        }

        public Capnp.Rpc.Provide Provide
        {
            get => _which == WHICH.Provide ? (Capnp.Rpc.Provide)_content : null;
            set
            {
                _which = WHICH.Provide;
                _content = value;
            }
        }

        public Capnp.Rpc.Accept Accept
        {
            get => _which == WHICH.Accept ? (Capnp.Rpc.Accept)_content : null;
            set
            {
                _which = WHICH.Accept;
                _content = value;
            }
        }

        public Capnp.Rpc.Join Join
        {
            get => _which == WHICH.Join ? (Capnp.Rpc.Join)_content : null;
            set
            {
                _which = WHICH.Join;
                _content = value;
            }
        }

        public Capnp.Rpc.Disembargo Disembargo
        {
            get => _which == WHICH.Disembargo ? (Capnp.Rpc.Disembargo)_content : null;
            set
            {
                _which = WHICH.Disembargo;
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
            public Capnp.Rpc.Message.READER Unimplemented => which == WHICH.Unimplemented ? ctx.ReadStruct(0, Capnp.Rpc.Message.READER.create) : default;
            public Capnp.Rpc.Exception.READER Abort => which == WHICH.Abort ? ctx.ReadStruct(0, Capnp.Rpc.Exception.READER.create) : default;
            public Capnp.Rpc.Call.READER Call => which == WHICH.Call ? ctx.ReadStruct(0, Capnp.Rpc.Call.READER.create) : default;
            public Capnp.Rpc.Return.READER Return => which == WHICH.Return ? ctx.ReadStruct(0, Capnp.Rpc.Return.READER.create) : default;
            public Capnp.Rpc.Finish.READER Finish => which == WHICH.Finish ? ctx.ReadStruct(0, Capnp.Rpc.Finish.READER.create) : default;
            public Capnp.Rpc.Resolve.READER Resolve => which == WHICH.Resolve ? ctx.ReadStruct(0, Capnp.Rpc.Resolve.READER.create) : default;
            public Capnp.Rpc.Release.READER Release => which == WHICH.Release ? ctx.ReadStruct(0, Capnp.Rpc.Release.READER.create) : default;
            public DeserializerState ObsoleteSave => which == WHICH.ObsoleteSave ? ctx.StructReadPointer(0) : default;
            public Capnp.Rpc.Bootstrap.READER Bootstrap => which == WHICH.Bootstrap ? ctx.ReadStruct(0, Capnp.Rpc.Bootstrap.READER.create) : default;
            public DeserializerState ObsoleteDelete => which == WHICH.ObsoleteDelete ? ctx.StructReadPointer(0) : default;
            public Capnp.Rpc.Provide.READER Provide => which == WHICH.Provide ? ctx.ReadStruct(0, Capnp.Rpc.Provide.READER.create) : default;
            public Capnp.Rpc.Accept.READER Accept => which == WHICH.Accept ? ctx.ReadStruct(0, Capnp.Rpc.Accept.READER.create) : default;
            public Capnp.Rpc.Join.READER Join => which == WHICH.Join ? ctx.ReadStruct(0, Capnp.Rpc.Join.READER.create) : default;
            public Capnp.Rpc.Disembargo.READER Disembargo => which == WHICH.Disembargo ? ctx.ReadStruct(0, Capnp.Rpc.Disembargo.READER.create) : default;
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

            public Capnp.Rpc.Message.WRITER Unimplemented
            {
                get => which == WHICH.Unimplemented ? BuildPointer<Capnp.Rpc.Message.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Exception.WRITER Abort
            {
                get => which == WHICH.Abort ? BuildPointer<Capnp.Rpc.Exception.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Call.WRITER Call
            {
                get => which == WHICH.Call ? BuildPointer<Capnp.Rpc.Call.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Return.WRITER Return
            {
                get => which == WHICH.Return ? BuildPointer<Capnp.Rpc.Return.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Finish.WRITER Finish
            {
                get => which == WHICH.Finish ? BuildPointer<Capnp.Rpc.Finish.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Resolve.WRITER Resolve
            {
                get => which == WHICH.Resolve ? BuildPointer<Capnp.Rpc.Resolve.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Release.WRITER Release
            {
                get => which == WHICH.Release ? BuildPointer<Capnp.Rpc.Release.WRITER>(0) : default;
                set => Link(0, value);
            }

            public DynamicSerializerState ObsoleteSave
            {
                get => which == WHICH.ObsoleteSave ? BuildPointer<DynamicSerializerState>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Bootstrap.WRITER Bootstrap
            {
                get => which == WHICH.Bootstrap ? BuildPointer<Capnp.Rpc.Bootstrap.WRITER>(0) : default;
                set => Link(0, value);
            }

            public DynamicSerializerState ObsoleteDelete
            {
                get => which == WHICH.ObsoleteDelete ? BuildPointer<DynamicSerializerState>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Provide.WRITER Provide
            {
                get => which == WHICH.Provide ? BuildPointer<Capnp.Rpc.Provide.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Accept.WRITER Accept
            {
                get => which == WHICH.Accept ? BuildPointer<Capnp.Rpc.Accept.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Join.WRITER Join
            {
                get => which == WHICH.Join ? BuildPointer<Capnp.Rpc.Join.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Disembargo.WRITER Disembargo
            {
                get => which == WHICH.Disembargo ? BuildPointer<Capnp.Rpc.Disembargo.WRITER>(0) : default;
                set => Link(0, value);
            }
        }
    }

    public class Bootstrap : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            QuestionId = reader.QuestionId;
            DeprecatedObjectId = CapnpSerializable.Create<AnyPointer>(reader.DeprecatedObjectId);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.QuestionId = QuestionId;
            writer.DeprecatedObjectId.SetObject(DeprecatedObjectId);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint QuestionId
        {
            get;
            set;
        }

        public AnyPointer DeprecatedObjectId
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
            public uint QuestionId => ctx.ReadDataUInt(0UL, 0U);
            public DeserializerState DeprecatedObjectId => ctx.StructReadPointer(0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public uint QuestionId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public DynamicSerializerState DeprecatedObjectId
            {
                get => BuildPointer<DynamicSerializerState>(0);
                set => Link(0, value);
            }
        }
    }

    public class Call : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            QuestionId = reader.QuestionId;
            Target = CapnpSerializable.Create<Capnp.Rpc.MessageTarget>(reader.Target);
            InterfaceId = reader.InterfaceId;
            MethodId = reader.MethodId;
            Params = CapnpSerializable.Create<Capnp.Rpc.Payload>(reader.Params);
            SendResultsTo = CapnpSerializable.Create<Capnp.Rpc.Call.@sendResultsTo>(reader.SendResultsTo);
            AllowThirdPartyTailCall = reader.AllowThirdPartyTailCall;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.QuestionId = QuestionId;
            Target?.serialize(writer.Target);
            writer.InterfaceId = InterfaceId;
            writer.MethodId = MethodId;
            Params?.serialize(writer.Params);
            SendResultsTo?.serialize(writer.SendResultsTo);
            writer.AllowThirdPartyTailCall = AllowThirdPartyTailCall;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint QuestionId
        {
            get;
            set;
        }

        public Capnp.Rpc.MessageTarget Target
        {
            get;
            set;
        }

        public ulong InterfaceId
        {
            get;
            set;
        }

        public ushort MethodId
        {
            get;
            set;
        }

        public Capnp.Rpc.Payload Params
        {
            get;
            set;
        }

        public Capnp.Rpc.Call.@sendResultsTo SendResultsTo
        {
            get;
            set;
        }

        public bool AllowThirdPartyTailCall
        {
            get;
            set;
        }

        = false;
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
            public uint QuestionId => ctx.ReadDataUInt(0UL, 0U);
            public Capnp.Rpc.MessageTarget.READER Target => ctx.ReadStruct(0, Capnp.Rpc.MessageTarget.READER.create);
            public ulong InterfaceId => ctx.ReadDataULong(64UL, 0UL);
            public ushort MethodId => ctx.ReadDataUShort(32UL, (ushort)0);
            public Capnp.Rpc.Payload.READER Params => ctx.ReadStruct(1, Capnp.Rpc.Payload.READER.create);
            public @sendResultsTo.READER SendResultsTo => new @sendResultsTo.READER(ctx);
            public bool AllowThirdPartyTailCall => ctx.ReadDataBool(128UL, false);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(3, 3);
            }

            public uint QuestionId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public Capnp.Rpc.MessageTarget.WRITER Target
            {
                get => BuildPointer<Capnp.Rpc.MessageTarget.WRITER>(0);
                set => Link(0, value);
            }

            public ulong InterfaceId
            {
                get => this.ReadDataULong(64UL, 0UL);
                set => this.WriteData(64UL, value, 0UL);
            }

            public ushort MethodId
            {
                get => this.ReadDataUShort(32UL, (ushort)0);
                set => this.WriteData(32UL, value, (ushort)0);
            }

            public Capnp.Rpc.Payload.WRITER Params
            {
                get => BuildPointer<Capnp.Rpc.Payload.WRITER>(1);
                set => Link(1, value);
            }

            public @sendResultsTo.WRITER SendResultsTo
            {
                get => Rewrap<@sendResultsTo.WRITER>();
            }

            public bool AllowThirdPartyTailCall
            {
                get => this.ReadDataBool(128UL, false);
                set => this.WriteData(128UL, value, false);
            }
        }

        public class @sendResultsTo : ICapnpSerializable
        {
            public enum WHICH : ushort
            {
                Caller = 0,
                Yourself = 1,
                ThirdParty = 2,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Caller:
                        which = reader.which;
                        break;
                    case WHICH.Yourself:
                        which = reader.which;
                        break;
                    case WHICH.ThirdParty:
                        ThirdParty = CapnpSerializable.Create<AnyPointer>(reader.ThirdParty);
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
                        case WHICH.Caller:
                            break;
                        case WHICH.Yourself:
                            break;
                        case WHICH.ThirdParty:
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
                    case WHICH.Caller:
                        break;
                    case WHICH.Yourself:
                        break;
                    case WHICH.ThirdParty:
                        writer.ThirdParty.SetObject(ThirdParty);
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

            public AnyPointer ThirdParty
            {
                get => _which == WHICH.ThirdParty ? (AnyPointer)_content : null;
                set
                {
                    _which = WHICH.ThirdParty;
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
                public DeserializerState ThirdParty => which == WHICH.ThirdParty ? ctx.StructReadPointer(2) : default;
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

                public DynamicSerializerState ThirdParty
                {
                    get => which == WHICH.ThirdParty ? BuildPointer<DynamicSerializerState>(2) : default;
                    set => Link(2, value);
                }
            }
        }
    }

    public class Return : ICapnpSerializable
    {
        public enum WHICH : ushort
        {
            Results = 0,
            Exception = 1,
            Canceled = 2,
            ResultsSentElsewhere = 3,
            TakeFromOtherQuestion = 4,
            AcceptFromThirdParty = 5,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.Results:
                    Results = CapnpSerializable.Create<Capnp.Rpc.Payload>(reader.Results);
                    break;
                case WHICH.Exception:
                    Exception = CapnpSerializable.Create<Capnp.Rpc.Exception>(reader.Exception);
                    break;
                case WHICH.Canceled:
                    which = reader.which;
                    break;
                case WHICH.ResultsSentElsewhere:
                    which = reader.which;
                    break;
                case WHICH.TakeFromOtherQuestion:
                    TakeFromOtherQuestion = reader.TakeFromOtherQuestion;
                    break;
                case WHICH.AcceptFromThirdParty:
                    AcceptFromThirdParty = CapnpSerializable.Create<AnyPointer>(reader.AcceptFromThirdParty);
                    break;
            }

            AnswerId = reader.AnswerId;
            ReleaseParamCaps = reader.ReleaseParamCaps;
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
                    case WHICH.Results:
                        _content = null;
                        break;
                    case WHICH.Exception:
                        _content = null;
                        break;
                    case WHICH.Canceled:
                        break;
                    case WHICH.ResultsSentElsewhere:
                        break;
                    case WHICH.TakeFromOtherQuestion:
                        _content = 0;
                        break;
                    case WHICH.AcceptFromThirdParty:
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
                case WHICH.Results:
                    Results?.serialize(writer.Results);
                    break;
                case WHICH.Exception:
                    Exception?.serialize(writer.Exception);
                    break;
                case WHICH.Canceled:
                    break;
                case WHICH.ResultsSentElsewhere:
                    break;
                case WHICH.TakeFromOtherQuestion:
                    writer.TakeFromOtherQuestion = TakeFromOtherQuestion.Value;
                    break;
                case WHICH.AcceptFromThirdParty:
                    writer.AcceptFromThirdParty.SetObject(AcceptFromThirdParty);
                    break;
            }

            writer.AnswerId = AnswerId;
            writer.ReleaseParamCaps = ReleaseParamCaps;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint AnswerId
        {
            get;
            set;
        }

        public bool ReleaseParamCaps
        {
            get;
            set;
        }

        = true;
        public Capnp.Rpc.Payload Results
        {
            get => _which == WHICH.Results ? (Capnp.Rpc.Payload)_content : null;
            set
            {
                _which = WHICH.Results;
                _content = value;
            }
        }

        public Capnp.Rpc.Exception Exception
        {
            get => _which == WHICH.Exception ? (Capnp.Rpc.Exception)_content : null;
            set
            {
                _which = WHICH.Exception;
                _content = value;
            }
        }

        public uint? TakeFromOtherQuestion
        {
            get => _which == WHICH.TakeFromOtherQuestion ? (uint? )_content : null;
            set
            {
                _which = WHICH.TakeFromOtherQuestion;
                _content = value;
            }
        }

        public AnyPointer AcceptFromThirdParty
        {
            get => _which == WHICH.AcceptFromThirdParty ? (AnyPointer)_content : null;
            set
            {
                _which = WHICH.AcceptFromThirdParty;
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
            public uint AnswerId => ctx.ReadDataUInt(0UL, 0U);
            public bool ReleaseParamCaps => ctx.ReadDataBool(32UL, true);
            public Capnp.Rpc.Payload.READER Results => which == WHICH.Results ? ctx.ReadStruct(0, Capnp.Rpc.Payload.READER.create) : default;
            public Capnp.Rpc.Exception.READER Exception => which == WHICH.Exception ? ctx.ReadStruct(0, Capnp.Rpc.Exception.READER.create) : default;
            public uint TakeFromOtherQuestion => which == WHICH.TakeFromOtherQuestion ? ctx.ReadDataUInt(64UL, 0U) : default;
            public DeserializerState AcceptFromThirdParty => which == WHICH.AcceptFromThirdParty ? ctx.StructReadPointer(0) : default;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 1);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(48U, (ushort)0);
                set => this.WriteData(48U, (ushort)value, (ushort)0);
            }

            public uint AnswerId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public bool ReleaseParamCaps
            {
                get => this.ReadDataBool(32UL, true);
                set => this.WriteData(32UL, value, true);
            }

            public Capnp.Rpc.Payload.WRITER Results
            {
                get => which == WHICH.Results ? BuildPointer<Capnp.Rpc.Payload.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Exception.WRITER Exception
            {
                get => which == WHICH.Exception ? BuildPointer<Capnp.Rpc.Exception.WRITER>(0) : default;
                set => Link(0, value);
            }

            public uint TakeFromOtherQuestion
            {
                get => which == WHICH.TakeFromOtherQuestion ? this.ReadDataUInt(64UL, 0U) : default;
                set => this.WriteData(64UL, value, 0U);
            }

            public DynamicSerializerState AcceptFromThirdParty
            {
                get => which == WHICH.AcceptFromThirdParty ? BuildPointer<DynamicSerializerState>(0) : default;
                set => Link(0, value);
            }
        }
    }

    public class Finish : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            QuestionId = reader.QuestionId;
            ReleaseResultCaps = reader.ReleaseResultCaps;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.QuestionId = QuestionId;
            writer.ReleaseResultCaps = ReleaseResultCaps;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint QuestionId
        {
            get;
            set;
        }

        public bool ReleaseResultCaps
        {
            get;
            set;
        }

        = true;
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
            public uint QuestionId => ctx.ReadDataUInt(0UL, 0U);
            public bool ReleaseResultCaps => ctx.ReadDataBool(32UL, true);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public uint QuestionId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public bool ReleaseResultCaps
            {
                get => this.ReadDataBool(32UL, true);
                set => this.WriteData(32UL, value, true);
            }
        }
    }

    public class Resolve : ICapnpSerializable
    {
        public enum WHICH : ushort
        {
            Cap = 0,
            Exception = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.Cap:
                    Cap = CapnpSerializable.Create<Capnp.Rpc.CapDescriptor>(reader.Cap);
                    break;
                case WHICH.Exception:
                    Exception = CapnpSerializable.Create<Capnp.Rpc.Exception>(reader.Exception);
                    break;
            }

            PromiseId = reader.PromiseId;
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
                    case WHICH.Cap:
                        _content = null;
                        break;
                    case WHICH.Exception:
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
                case WHICH.Cap:
                    Cap?.serialize(writer.Cap);
                    break;
                case WHICH.Exception:
                    Exception?.serialize(writer.Exception);
                    break;
            }

            writer.PromiseId = PromiseId;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint PromiseId
        {
            get;
            set;
        }

        public Capnp.Rpc.CapDescriptor Cap
        {
            get => _which == WHICH.Cap ? (Capnp.Rpc.CapDescriptor)_content : null;
            set
            {
                _which = WHICH.Cap;
                _content = value;
            }
        }

        public Capnp.Rpc.Exception Exception
        {
            get => _which == WHICH.Exception ? (Capnp.Rpc.Exception)_content : null;
            set
            {
                _which = WHICH.Exception;
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
            public uint PromiseId => ctx.ReadDataUInt(0UL, 0U);
            public Capnp.Rpc.CapDescriptor.READER Cap => which == WHICH.Cap ? ctx.ReadStruct(0, Capnp.Rpc.CapDescriptor.READER.create) : default;
            public Capnp.Rpc.Exception.READER Exception => which == WHICH.Exception ? ctx.ReadStruct(0, Capnp.Rpc.Exception.READER.create) : default;
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

            public uint PromiseId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public Capnp.Rpc.CapDescriptor.WRITER Cap
            {
                get => which == WHICH.Cap ? BuildPointer<Capnp.Rpc.CapDescriptor.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.Exception.WRITER Exception
            {
                get => which == WHICH.Exception ? BuildPointer<Capnp.Rpc.Exception.WRITER>(0) : default;
                set => Link(0, value);
            }
        }
    }

    public class Release : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Id = reader.Id;
            ReferenceCount = reader.ReferenceCount;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Id = Id;
            writer.ReferenceCount = ReferenceCount;
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

        public uint ReferenceCount
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
            public uint ReferenceCount => ctx.ReadDataUInt(32UL, 0U);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public uint Id
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public uint ReferenceCount
            {
                get => this.ReadDataUInt(32UL, 0U);
                set => this.WriteData(32UL, value, 0U);
            }
        }
    }

    public class Disembargo : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Target = CapnpSerializable.Create<Capnp.Rpc.MessageTarget>(reader.Target);
            Context = CapnpSerializable.Create<Capnp.Rpc.Disembargo.@context>(reader.Context);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Target?.serialize(writer.Target);
            Context?.serialize(writer.Context);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public Capnp.Rpc.MessageTarget Target
        {
            get;
            set;
        }

        public Capnp.Rpc.Disembargo.@context Context
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
            public Capnp.Rpc.MessageTarget.READER Target => ctx.ReadStruct(0, Capnp.Rpc.MessageTarget.READER.create);
            public @context.READER Context => new @context.READER(ctx);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public Capnp.Rpc.MessageTarget.WRITER Target
            {
                get => BuildPointer<Capnp.Rpc.MessageTarget.WRITER>(0);
                set => Link(0, value);
            }

            public @context.WRITER Context
            {
                get => Rewrap<@context.WRITER>();
            }
        }

        public class @context : ICapnpSerializable
        {
            public enum WHICH : ushort
            {
                SenderLoopback = 0,
                ReceiverLoopback = 1,
                Accept = 2,
                Provide = 3,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.SenderLoopback:
                        SenderLoopback = reader.SenderLoopback;
                        break;
                    case WHICH.ReceiverLoopback:
                        ReceiverLoopback = reader.ReceiverLoopback;
                        break;
                    case WHICH.Accept:
                        which = reader.which;
                        break;
                    case WHICH.Provide:
                        Provide = reader.Provide;
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
                        case WHICH.SenderLoopback:
                            _content = 0;
                            break;
                        case WHICH.ReceiverLoopback:
                            _content = 0;
                            break;
                        case WHICH.Accept:
                            break;
                        case WHICH.Provide:
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
                    case WHICH.SenderLoopback:
                        writer.SenderLoopback = SenderLoopback.Value;
                        break;
                    case WHICH.ReceiverLoopback:
                        writer.ReceiverLoopback = ReceiverLoopback.Value;
                        break;
                    case WHICH.Accept:
                        break;
                    case WHICH.Provide:
                        writer.Provide = Provide.Value;
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

            public uint? SenderLoopback
            {
                get => _which == WHICH.SenderLoopback ? (uint? )_content : null;
                set
                {
                    _which = WHICH.SenderLoopback;
                    _content = value;
                }
            }

            public uint? ReceiverLoopback
            {
                get => _which == WHICH.ReceiverLoopback ? (uint? )_content : null;
                set
                {
                    _which = WHICH.ReceiverLoopback;
                    _content = value;
                }
            }

            public uint? Provide
            {
                get => _which == WHICH.Provide ? (uint? )_content : null;
                set
                {
                    _which = WHICH.Provide;
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
                public uint SenderLoopback => which == WHICH.SenderLoopback ? ctx.ReadDataUInt(0UL, 0U) : default;
                public uint ReceiverLoopback => which == WHICH.ReceiverLoopback ? ctx.ReadDataUInt(0UL, 0U) : default;
                public uint Provide => which == WHICH.Provide ? ctx.ReadDataUInt(0UL, 0U) : default;
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

                public uint SenderLoopback
                {
                    get => which == WHICH.SenderLoopback ? this.ReadDataUInt(0UL, 0U) : default;
                    set => this.WriteData(0UL, value, 0U);
                }

                public uint ReceiverLoopback
                {
                    get => which == WHICH.ReceiverLoopback ? this.ReadDataUInt(0UL, 0U) : default;
                    set => this.WriteData(0UL, value, 0U);
                }

                public uint Provide
                {
                    get => which == WHICH.Provide ? this.ReadDataUInt(0UL, 0U) : default;
                    set => this.WriteData(0UL, value, 0U);
                }
            }
        }
    }

    public class Provide : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            QuestionId = reader.QuestionId;
            Target = CapnpSerializable.Create<Capnp.Rpc.MessageTarget>(reader.Target);
            Recipient = CapnpSerializable.Create<AnyPointer>(reader.Recipient);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.QuestionId = QuestionId;
            Target?.serialize(writer.Target);
            writer.Recipient.SetObject(Recipient);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint QuestionId
        {
            get;
            set;
        }

        public Capnp.Rpc.MessageTarget Target
        {
            get;
            set;
        }

        public AnyPointer Recipient
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
            public uint QuestionId => ctx.ReadDataUInt(0UL, 0U);
            public Capnp.Rpc.MessageTarget.READER Target => ctx.ReadStruct(0, Capnp.Rpc.MessageTarget.READER.create);
            public DeserializerState Recipient => ctx.StructReadPointer(1);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 2);
            }

            public uint QuestionId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public Capnp.Rpc.MessageTarget.WRITER Target
            {
                get => BuildPointer<Capnp.Rpc.MessageTarget.WRITER>(0);
                set => Link(0, value);
            }

            public DynamicSerializerState Recipient
            {
                get => BuildPointer<DynamicSerializerState>(1);
                set => Link(1, value);
            }
        }
    }

    public class Accept : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            QuestionId = reader.QuestionId;
            Provision = CapnpSerializable.Create<AnyPointer>(reader.Provision);
            Embargo = reader.Embargo;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.QuestionId = QuestionId;
            writer.Provision.SetObject(Provision);
            writer.Embargo = Embargo;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint QuestionId
        {
            get;
            set;
        }

        public AnyPointer Provision
        {
            get;
            set;
        }

        public bool Embargo
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
            public uint QuestionId => ctx.ReadDataUInt(0UL, 0U);
            public DeserializerState Provision => ctx.StructReadPointer(0);
            public bool Embargo => ctx.ReadDataBool(32UL, false);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public uint QuestionId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public DynamicSerializerState Provision
            {
                get => BuildPointer<DynamicSerializerState>(0);
                set => Link(0, value);
            }

            public bool Embargo
            {
                get => this.ReadDataBool(32UL, false);
                set => this.WriteData(32UL, value, false);
            }
        }
    }

    public class Join : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            QuestionId = reader.QuestionId;
            Target = CapnpSerializable.Create<Capnp.Rpc.MessageTarget>(reader.Target);
            KeyPart = CapnpSerializable.Create<AnyPointer>(reader.KeyPart);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.QuestionId = QuestionId;
            Target?.serialize(writer.Target);
            writer.KeyPart.SetObject(KeyPart);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint QuestionId
        {
            get;
            set;
        }

        public Capnp.Rpc.MessageTarget Target
        {
            get;
            set;
        }

        public AnyPointer KeyPart
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
            public uint QuestionId => ctx.ReadDataUInt(0UL, 0U);
            public Capnp.Rpc.MessageTarget.READER Target => ctx.ReadStruct(0, Capnp.Rpc.MessageTarget.READER.create);
            public DeserializerState KeyPart => ctx.StructReadPointer(1);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 2);
            }

            public uint QuestionId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public Capnp.Rpc.MessageTarget.WRITER Target
            {
                get => BuildPointer<Capnp.Rpc.MessageTarget.WRITER>(0);
                set => Link(0, value);
            }

            public DynamicSerializerState KeyPart
            {
                get => BuildPointer<DynamicSerializerState>(1);
                set => Link(1, value);
            }
        }
    }

    public class MessageTarget : ICapnpSerializable
    {
        public enum WHICH : ushort
        {
            ImportedCap = 0,
            PromisedAnswer = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.ImportedCap:
                    ImportedCap = reader.ImportedCap;
                    break;
                case WHICH.PromisedAnswer:
                    PromisedAnswer = CapnpSerializable.Create<Capnp.Rpc.PromisedAnswer>(reader.PromisedAnswer);
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
                    case WHICH.ImportedCap:
                        _content = 0;
                        break;
                    case WHICH.PromisedAnswer:
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
                case WHICH.ImportedCap:
                    writer.ImportedCap = ImportedCap.Value;
                    break;
                case WHICH.PromisedAnswer:
                    PromisedAnswer?.serialize(writer.PromisedAnswer);
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

        public uint? ImportedCap
        {
            get => _which == WHICH.ImportedCap ? (uint? )_content : null;
            set
            {
                _which = WHICH.ImportedCap;
                _content = value;
            }
        }

        public Capnp.Rpc.PromisedAnswer PromisedAnswer
        {
            get => _which == WHICH.PromisedAnswer ? (Capnp.Rpc.PromisedAnswer)_content : null;
            set
            {
                _which = WHICH.PromisedAnswer;
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
            public uint ImportedCap => which == WHICH.ImportedCap ? ctx.ReadDataUInt(0UL, 0U) : default;
            public Capnp.Rpc.PromisedAnswer.READER PromisedAnswer => which == WHICH.PromisedAnswer ? ctx.ReadStruct(0, Capnp.Rpc.PromisedAnswer.READER.create) : default;
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

            public uint ImportedCap
            {
                get => which == WHICH.ImportedCap ? this.ReadDataUInt(0UL, 0U) : default;
                set => this.WriteData(0UL, value, 0U);
            }

            public Capnp.Rpc.PromisedAnswer.WRITER PromisedAnswer
            {
                get => which == WHICH.PromisedAnswer ? BuildPointer<Capnp.Rpc.PromisedAnswer.WRITER>(0) : default;
                set => Link(0, value);
            }
        }
    }

    public class Payload : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Content = CapnpSerializable.Create<AnyPointer>(reader.Content);
            CapTable = reader.CapTable.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Rpc.CapDescriptor>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Content.SetObject(Content);
            writer.CapTable.Init(CapTable, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public AnyPointer Content
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.Rpc.CapDescriptor> CapTable
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
            public DeserializerState Content => ctx.StructReadPointer(0);
            public IReadOnlyList<Capnp.Rpc.CapDescriptor.READER> CapTable => ctx.ReadList(1).Cast(Capnp.Rpc.CapDescriptor.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 2);
            }

            public DynamicSerializerState Content
            {
                get => BuildPointer<DynamicSerializerState>(0);
                set => Link(0, value);
            }

            public ListOfStructsSerializer<Capnp.Rpc.CapDescriptor.WRITER> CapTable
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Rpc.CapDescriptor.WRITER>>(1);
                set => Link(1, value);
            }
        }
    }

    public class CapDescriptor : ICapnpSerializable
    {
        public enum WHICH : ushort
        {
            None = 0,
            SenderHosted = 1,
            SenderPromise = 2,
            ReceiverHosted = 3,
            ReceiverAnswer = 4,
            ThirdPartyHosted = 5,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.None:
                    which = reader.which;
                    break;
                case WHICH.SenderHosted:
                    SenderHosted = reader.SenderHosted;
                    break;
                case WHICH.SenderPromise:
                    SenderPromise = reader.SenderPromise;
                    break;
                case WHICH.ReceiverHosted:
                    ReceiverHosted = reader.ReceiverHosted;
                    break;
                case WHICH.ReceiverAnswer:
                    ReceiverAnswer = CapnpSerializable.Create<Capnp.Rpc.PromisedAnswer>(reader.ReceiverAnswer);
                    break;
                case WHICH.ThirdPartyHosted:
                    ThirdPartyHosted = CapnpSerializable.Create<Capnp.Rpc.ThirdPartyCapDescriptor>(reader.ThirdPartyHosted);
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
                    case WHICH.None:
                        break;
                    case WHICH.SenderHosted:
                        _content = 0;
                        break;
                    case WHICH.SenderPromise:
                        _content = 0;
                        break;
                    case WHICH.ReceiverHosted:
                        _content = 0;
                        break;
                    case WHICH.ReceiverAnswer:
                        _content = null;
                        break;
                    case WHICH.ThirdPartyHosted:
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
                case WHICH.None:
                    break;
                case WHICH.SenderHosted:
                    writer.SenderHosted = SenderHosted.Value;
                    break;
                case WHICH.SenderPromise:
                    writer.SenderPromise = SenderPromise.Value;
                    break;
                case WHICH.ReceiverHosted:
                    writer.ReceiverHosted = ReceiverHosted.Value;
                    break;
                case WHICH.ReceiverAnswer:
                    ReceiverAnswer?.serialize(writer.ReceiverAnswer);
                    break;
                case WHICH.ThirdPartyHosted:
                    ThirdPartyHosted?.serialize(writer.ThirdPartyHosted);
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

        public uint? SenderHosted
        {
            get => _which == WHICH.SenderHosted ? (uint? )_content : null;
            set
            {
                _which = WHICH.SenderHosted;
                _content = value;
            }
        }

        public uint? SenderPromise
        {
            get => _which == WHICH.SenderPromise ? (uint? )_content : null;
            set
            {
                _which = WHICH.SenderPromise;
                _content = value;
            }
        }

        public uint? ReceiverHosted
        {
            get => _which == WHICH.ReceiverHosted ? (uint? )_content : null;
            set
            {
                _which = WHICH.ReceiverHosted;
                _content = value;
            }
        }

        public Capnp.Rpc.PromisedAnswer ReceiverAnswer
        {
            get => _which == WHICH.ReceiverAnswer ? (Capnp.Rpc.PromisedAnswer)_content : null;
            set
            {
                _which = WHICH.ReceiverAnswer;
                _content = value;
            }
        }

        public Capnp.Rpc.ThirdPartyCapDescriptor ThirdPartyHosted
        {
            get => _which == WHICH.ThirdPartyHosted ? (Capnp.Rpc.ThirdPartyCapDescriptor)_content : null;
            set
            {
                _which = WHICH.ThirdPartyHosted;
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
            public uint SenderHosted => which == WHICH.SenderHosted ? ctx.ReadDataUInt(32UL, 0U) : default;
            public uint SenderPromise => which == WHICH.SenderPromise ? ctx.ReadDataUInt(32UL, 0U) : default;
            public uint ReceiverHosted => which == WHICH.ReceiverHosted ? ctx.ReadDataUInt(32UL, 0U) : default;
            public Capnp.Rpc.PromisedAnswer.READER ReceiverAnswer => which == WHICH.ReceiverAnswer ? ctx.ReadStruct(0, Capnp.Rpc.PromisedAnswer.READER.create) : default;
            public Capnp.Rpc.ThirdPartyCapDescriptor.READER ThirdPartyHosted => which == WHICH.ThirdPartyHosted ? ctx.ReadStruct(0, Capnp.Rpc.ThirdPartyCapDescriptor.READER.create) : default;
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

            public uint SenderHosted
            {
                get => which == WHICH.SenderHosted ? this.ReadDataUInt(32UL, 0U) : default;
                set => this.WriteData(32UL, value, 0U);
            }

            public uint SenderPromise
            {
                get => which == WHICH.SenderPromise ? this.ReadDataUInt(32UL, 0U) : default;
                set => this.WriteData(32UL, value, 0U);
            }

            public uint ReceiverHosted
            {
                get => which == WHICH.ReceiverHosted ? this.ReadDataUInt(32UL, 0U) : default;
                set => this.WriteData(32UL, value, 0U);
            }

            public Capnp.Rpc.PromisedAnswer.WRITER ReceiverAnswer
            {
                get => which == WHICH.ReceiverAnswer ? BuildPointer<Capnp.Rpc.PromisedAnswer.WRITER>(0) : default;
                set => Link(0, value);
            }

            public Capnp.Rpc.ThirdPartyCapDescriptor.WRITER ThirdPartyHosted
            {
                get => which == WHICH.ThirdPartyHosted ? BuildPointer<Capnp.Rpc.ThirdPartyCapDescriptor.WRITER>(0) : default;
                set => Link(0, value);
            }
        }
    }

    public class PromisedAnswer : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            QuestionId = reader.QuestionId;
            Transform = reader.Transform.ToReadOnlyList(_ => CapnpSerializable.Create<Capnp.Rpc.PromisedAnswer.Op>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.QuestionId = QuestionId;
            writer.Transform.Init(Transform, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint QuestionId
        {
            get;
            set;
        }

        public IReadOnlyList<Capnp.Rpc.PromisedAnswer.Op> Transform
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
            public uint QuestionId => ctx.ReadDataUInt(0UL, 0U);
            public IReadOnlyList<Capnp.Rpc.PromisedAnswer.Op.READER> Transform => ctx.ReadList(0).Cast(Capnp.Rpc.PromisedAnswer.Op.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public uint QuestionId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public ListOfStructsSerializer<Capnp.Rpc.PromisedAnswer.Op.WRITER> Transform
            {
                get => BuildPointer<ListOfStructsSerializer<Capnp.Rpc.PromisedAnswer.Op.WRITER>>(0);
                set => Link(0, value);
            }
        }

        public class Op : ICapnpSerializable
        {
            public enum WHICH : ushort
            {
                Noop = 0,
                GetPointerField = 1,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Noop:
                        which = reader.which;
                        break;
                    case WHICH.GetPointerField:
                        GetPointerField = reader.GetPointerField;
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
                        case WHICH.Noop:
                            break;
                        case WHICH.GetPointerField:
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
                    case WHICH.Noop:
                        break;
                    case WHICH.GetPointerField:
                        writer.GetPointerField = GetPointerField.Value;
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

            public ushort? GetPointerField
            {
                get => _which == WHICH.GetPointerField ? (ushort? )_content : null;
                set
                {
                    _which = WHICH.GetPointerField;
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
                public ushort GetPointerField => which == WHICH.GetPointerField ? ctx.ReadDataUShort(16UL, (ushort)0) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                    set => this.WriteData(0U, (ushort)value, (ushort)0);
                }

                public ushort GetPointerField
                {
                    get => which == WHICH.GetPointerField ? this.ReadDataUShort(16UL, (ushort)0) : default;
                    set => this.WriteData(16UL, value, (ushort)0);
                }
            }
        }
    }

    public class ThirdPartyCapDescriptor : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Id = CapnpSerializable.Create<AnyPointer>(reader.Id);
            VineId = reader.VineId;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Id.SetObject(Id);
            writer.VineId = VineId;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public AnyPointer Id
        {
            get;
            set;
        }

        public uint VineId
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
            public DeserializerState Id => ctx.StructReadPointer(0);
            public uint VineId => ctx.ReadDataUInt(0UL, 0U);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public DynamicSerializerState Id
            {
                get => BuildPointer<DynamicSerializerState>(0);
                set => Link(0, value);
            }

            public uint VineId
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }
        }
    }

    public class Exception : ICapnpSerializable
    {
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Reason = reader.Reason;
            ObsoleteIsCallersFault = reader.ObsoleteIsCallersFault;
            ObsoleteDurability = reader.ObsoleteDurability;
            TheType = reader.TheType;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Reason = Reason;
            writer.ObsoleteIsCallersFault = ObsoleteIsCallersFault;
            writer.ObsoleteDurability = ObsoleteDurability;
            writer.TheType = TheType;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Reason
        {
            get;
            set;
        }

        public bool ObsoleteIsCallersFault
        {
            get;
            set;
        }

        public ushort ObsoleteDurability
        {
            get;
            set;
        }

        public Capnp.Rpc.Exception.Type TheType
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
            public string Reason => ctx.ReadText(0, "");
            public bool ObsoleteIsCallersFault => ctx.ReadDataBool(0UL, false);
            public ushort ObsoleteDurability => ctx.ReadDataUShort(16UL, (ushort)0);
            public Capnp.Rpc.Exception.Type TheType => (Capnp.Rpc.Exception.Type)ctx.ReadDataUShort(32UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public string Reason
            {
                get => this.ReadText(0, "");
                set => this.WriteText(0, value, "");
            }

            public bool ObsoleteIsCallersFault
            {
                get => this.ReadDataBool(0UL, false);
                set => this.WriteData(0UL, value, false);
            }

            public ushort ObsoleteDurability
            {
                get => this.ReadDataUShort(16UL, (ushort)0);
                set => this.WriteData(16UL, value, (ushort)0);
            }

            public Capnp.Rpc.Exception.Type TheType
            {
                get => (Capnp.Rpc.Exception.Type)this.ReadDataUShort(32UL, (ushort)0);
                set => this.WriteData(32UL, (ushort)value, (ushort)0);
            }
        }

        public enum Type : ushort
        {
            failed,
            overloaded,
            disconnected,
            unimplemented
        }
    }
}