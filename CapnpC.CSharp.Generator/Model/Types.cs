using System;

namespace CapnpC.CSharp.Generator.Model
{
    static class Types
    {
        public static readonly Type Void = new Type(TypeTag.Void);
        public static readonly Type Bool = new Type(TypeTag.Bool);
        public static readonly Type S8 = new Type(TypeTag.S8);
        public static readonly Type U8 = new Type(TypeTag.U8);
        public static readonly Type S16 = new Type(TypeTag.S16);
        public static readonly Type U16 = new Type(TypeTag.U16);
        public static readonly Type S32 = new Type(TypeTag.S32);
        public static readonly Type U32 = new Type(TypeTag.U32);
        public static readonly Type S64 = new Type(TypeTag.S64);
        public static readonly Type U64 = new Type(TypeTag.U64);
        public static readonly Type F32 = new Type(TypeTag.F32);
        public static readonly Type F64 = new Type(TypeTag.F64);
        public static readonly Type AnyPointer = new Type(TypeTag.AnyPointer);
        public static readonly Type StructPointer = new Type(TypeTag.StructPointer);
        public static readonly Type ListPointer = new Type(TypeTag.ListPointer);
        public static readonly Type CapabilityPointer = new Type(TypeTag.CapabilityPointer);
        public static readonly Type Data = new Type(TypeTag.Data);
        public static readonly Type Text = new Type(TypeTag.Text);
        public static readonly Type AnyEnum = new Type(TypeTag.AnyEnum);

        public static Type List(Type elementType)
        {
            return new Type(TypeTag.List)
            {
                ElementType = elementType
            };
        }

        public static Type FromDefinition(TypeDefinition def)
        {
            if (def.Tag == TypeTag.Unknown)
            {
                throw new InvalidOperationException("Oops, type definition is not yet valid, cannot create type");
            }

            return new Type(def.Tag)
            {
                Definition = def
            };
        }

        public static Type FromParameter(GenericParameter genericParameter)
        {
            return new Type(TypeTag.AnyPointer)
            {
                Parameter = genericParameter
            };
        }
    }
}
