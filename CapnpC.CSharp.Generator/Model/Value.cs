using Capnp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapnpC.CSharp.Generator.Model
{
    class Value
    {
        public static Value Scalar<T>(T scalar)
        {
            var value = new Value()
            {
                ScalarValue = scalar
            };

            if (typeof(T) == typeof(bool))
                value.Type = Types.Bool;
            else if (typeof(T) == typeof(float))
                value.Type = Types.F32;
            else if (typeof(T) == typeof(double))
                value.Type = Types.F64;
            else if (typeof(T) == typeof(sbyte))
                value.Type = Types.S8;
            else if (typeof(T) == typeof(byte))
                value.Type = Types.U8;
            else if (typeof(T) == typeof(short))
                value.Type = Types.S16;
            else if (typeof(T) == typeof(ushort))
                value.Type = Types.U16;
            else if (typeof(T) == typeof(int))
                value.Type = Types.S32;
            else if (typeof(T) == typeof(uint))
                value.Type = Types.U32;
            else if (typeof(T) == typeof(long))
                value.Type = Types.S64;
            else if (typeof(T) == typeof(ulong))
                value.Type = Types.U64;
            else if (typeof(T) == typeof(string))
                value.Type = Types.Text;
            else
                throw new NotImplementedException();

            return value;
        }

        public Type Type { get; set; }
        public object ScalarValue { get; set; }
        public Capnp.DeserializerState RawValue { get; set; }
        public List<Value> Items { get; } = new List<Value>();
        public int VoidListCount { get; set; }
        public ushort DiscriminatorValue { get; private set; }
        public List<(Field, Value)> Fields { get; } = new List<(Field, Value)>();

        public Enumerant GetEnumerant()
        {
            if (Type.Tag != TypeTag.Enum)
                throw new InvalidOperationException();

            if (Type.Definition.Enumerants[0].Ordinal.HasValue)
                return Type.Definition.Enumerants.Single(e => e.Ordinal == (ushort)ScalarValue);
            else
                return Type.Definition.Enumerants[(ushort)ScalarValue];
        }

        void DecodeStruct()
        {
            if (RawValue.Kind != Capnp.ObjectKind.Struct)
            {
                throw new NotSupportedException();
            }

            var def = Type.Definition;

            if (def.UnionInfo != null)
            {
                DiscriminatorValue = RawValue.ReadDataUShort(def.UnionInfo.TagOffset, ushort.MaxValue);
            }

            foreach (var field in Type.Fields)
            {
                if (field.DiscValue.HasValue && field.DiscValue.Value != DiscriminatorValue)
                    continue;

                Value value = new Value()
                {
                    Type = field.Type
                };

                switch (field.Type.Tag)
                {
                    case TypeTag.AnyEnum:
                        value.ScalarValue = field.DefaultValue?.ScalarValue as ushort? ?? 0;
                        break;

                    case TypeTag.AnyPointer:
                        value.RawValue = RawValue.StructReadPointer((int)field.Offset);
                        break;

                    case TypeTag.Bool:
                        value.ScalarValue = RawValue.ReadDataBool(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as bool?) ?? false);
                        break;

                    case TypeTag.CapabilityPointer:
                    case TypeTag.Interface:
                        continue;

                    case TypeTag.Data:
                    case TypeTag.Group:
                    case TypeTag.Struct:
                    case TypeTag.List:
                    case TypeTag.Text:
                        value.RawValue = RawValue.StructReadPointer((int)field.Offset);
                        value.Decode();
                        break;

                    case TypeTag.ListPointer:
                    case TypeTag.StructPointer:
                        value.RawValue = RawValue.StructReadPointer((int)field.Offset);
                        break;

                    case TypeTag.Enum:
                        value.ScalarValue = field.DefaultValue?.ScalarValue as ushort? ?? ushort.MaxValue;
                        break;

                    case TypeTag.F32:
                        value = Scalar(RawValue.ReadDataFloat(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as float?) ?? 0.0f));
                        break;

                    case TypeTag.F64:
                        value = Scalar(RawValue.ReadDataDouble(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as double?) ?? 0.0f));
                        break;

                    case TypeTag.S16:
                        value = Scalar(RawValue.ReadDataShort(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as short?) ?? 0));
                        break;

                    case TypeTag.S32:
                        value = Scalar(RawValue.ReadDataInt(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as int?) ?? 0));
                        break;

                    case TypeTag.S64:
                        value = Scalar(RawValue.ReadDataLong(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as long?) ?? 0));
                        break;

                    case TypeTag.S8:
                        value = Scalar(RawValue.ReadDataSByte(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as sbyte?) ?? 0));
                        break;

                    case TypeTag.U16:
                        value = Scalar(RawValue.ReadDataUShort(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as ushort?) ?? 0));
                        break;

                    case TypeTag.U32:
                        value = Scalar(RawValue.ReadDataUInt(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as uint?) ?? 0));
                        break;

                    case TypeTag.U64:
                        value = Scalar(RawValue.ReadDataULong(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as ulong?) ?? 0));
                        break;

                    case TypeTag.U8:
                        value = Scalar(RawValue.ReadDataByte(field.BitOffset.Value, (field.DefaultValue?.ScalarValue as byte?) ?? 0));
                        break;

                    case TypeTag.Void:
                        continue;

                    default:
                        throw new NotImplementedException();
                }

                Fields.Add((field, value));
            }
        }

        void DecodeList()
        {
            switch (Type.Tag)
            {
                case TypeTag.Data:
                    Items.AddRange(RawValue.RequireList().CastByte().Select(Scalar));
                    break;

                case TypeTag.List:
                    switch (Type.ElementType.Tag)
                    {
                        case TypeTag.AnyEnum:
                        case TypeTag.Enum:
                            Items.AddRange(RawValue.RequireList().CastUShort().Select(u => {
                                var v = Scalar(u);
                                v.Type = Type.ElementType;
                                return v; }));
                            break;

                        case TypeTag.AnyPointer:
                            Items.AddRange(RawValue.RequireList().Cast(d => new Value() { Type = Type.ElementType, RawValue = d }));
                            break;

                        case TypeTag.Bool:
                            Items.AddRange(RawValue.RequireList().CastBool().Select(Scalar));
                            break;

                        case TypeTag.Data:
                        case TypeTag.Group:
                        case TypeTag.Struct:
                        case TypeTag.List:
                            Items.AddRange(RawValue.RequireList().Cast(d => {
                                var v = new Value() { Type = Type.ElementType, RawValue = d };
                                v.Decode();
                                return v;
                            }));
                            break;

                        case TypeTag.Text:
                            Items.AddRange(RawValue.RequireList().CastText2().Select(Scalar));
                            break;

                        case TypeTag.F32:
                            Items.AddRange(RawValue.RequireList().CastFloat().Select(Scalar));
                            break;

                        case TypeTag.F64:
                            Items.AddRange(RawValue.RequireList().CastDouble().Select(Scalar));
                            break;

                        case TypeTag.S8:
                            Items.AddRange(RawValue.RequireList().CastSByte().Select(Scalar));
                            break;

                        case TypeTag.S16:
                            Items.AddRange(RawValue.RequireList().CastShort().Select(Scalar));
                            break;

                        case TypeTag.S32:
                            Items.AddRange(RawValue.RequireList().CastInt().Select(Scalar));
                            break;

                        case TypeTag.S64:
                            Items.AddRange(RawValue.RequireList().CastLong().Select(Scalar));
                            break;

                        case TypeTag.Void:
                            VoidListCount = RawValue.RequireList().Count;
                            break;

                        case TypeTag.U16:
                            Items.AddRange(RawValue.RequireList().CastUShort().Select(Scalar));
                            break;

                        case TypeTag.U32:
                            Items.AddRange(RawValue.RequireList().CastUInt().Select(Scalar));
                            break;

                        case TypeTag.U64:
                            Items.AddRange(RawValue.RequireList().CastULong().Select(Scalar));
                            break;

                        case TypeTag.U8:
                            Items.AddRange(RawValue.RequireList().CastByte().Select(Scalar));
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    break;

                case TypeTag.ListPointer:
                    Items.AddRange(RawValue.RequireList().Cast(d => new Value() { Type = Type.ElementType, RawValue = d }));
                    break;

                case TypeTag.Text:
                    ScalarValue = RawValue.RequireList().CastText();
                    break;
            }
        }

        public void Decode()
        {
            if (RawValue.Kind == ObjectKind.Nil) return;

            switch (Type.Tag)
            {
                case TypeTag.Group:
                case TypeTag.Struct:
                    DecodeStruct();
                    break;

                case TypeTag.List:
                case TypeTag.ListPointer:
                case TypeTag.Text:
                case TypeTag.Data:
                    DecodeList();
                    break;
            }

            RawValue = default(Capnp.DeserializerState);
        }
    }
}
