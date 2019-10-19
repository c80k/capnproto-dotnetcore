using System.Collections.Generic;

namespace CapnpC.CSharp.Generator.Model
{
    abstract class AbstractType
    {
        public TypeTag Tag { get; set; }

        public uint? FixedBitWidth
        {
            get
            {
                switch (Tag)
                {
                    case TypeTag.Bool:
                        return 1;

                    case TypeTag.U8:
                    case TypeTag.S8:
                        return 8;

                    case TypeTag.U16:
                    case TypeTag.S16:
                    case TypeTag.Enum:
                    case TypeTag.AnyEnum:
                        return 16;

                    case TypeTag.U32:
                    case TypeTag.S32:
                    case TypeTag.F32:
                        return 32;

                    case TypeTag.U64:
                    case TypeTag.S64:
                    case TypeTag.F64:
                        return 64;

                    default:
                        return null;
                }
            }
        }
    }
}
