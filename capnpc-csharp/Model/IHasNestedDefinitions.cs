using System.Collections.Generic;

namespace CapnpC.Model
{
    interface IHasNestedDefinitions
    {
        List<TypeDefinition> NestedTypes { get; }
        List<Value> Constants { get; }
    }
}
