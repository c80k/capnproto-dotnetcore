using System;
using System.Collections.Generic;

namespace CapnpC.Model
{
    class TypeDefinitionManager
    {
        readonly Dictionary<ulong, TypeDefinition> _id2def =
            new Dictionary<ulong, TypeDefinition>();

        public TypeDefinition GetOrCreate(ulong id, TypeTag tag)
        {
            if (_id2def.TryGetValue(id, out var def))
            {
                if (def.Tag == TypeTag.Unknown)
                {
                    def.Tag = tag;
                }
                else if (tag != TypeTag.Unknown && def.Tag != tag)
                {
                    throw new ArgumentOutOfRangeException(nameof(tag), "Type tag does not match existing type");
                }
            }
            else
            {
                def = new TypeDefinition(tag, id);
                _id2def.Add(id, def);
            }

            return def;
        }

        public TypeDefinition GetExisting(ulong id)
        {
            return _id2def[id];
        }
    }
}
