using System.Collections.Generic;
using System.Linq;

namespace CapnpC.Model
{
    interface IHasNestedDefinitions
    {
        IEnumerable<TypeDefinition> NestedTypes { get; }
        ICollection<IDefinition> NestedDefinitions { get; }
        ICollection<Constant> Constants { get; }
    }

    static partial class Extensions
    {
        public static IEnumerable<TypeDefinition> GetNestedTypes(this IHasNestedDefinitions def)
            => def.NestedDefinitions.Select(d => d as TypeDefinition).Where(d => d != null);
    }
}
