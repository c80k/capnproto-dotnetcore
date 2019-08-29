using System.Collections.Generic;

namespace CapnpC.Model
{
    class GenFile: IDefinition, IHasNestedDefinitions
    {
        public ulong Id { get;  }
        public TypeTag Tag { get => TypeTag.File;  }
        public IHasNestedDefinitions DeclaringElement { get; }

        public string Name { get; set; }
        public string[] Namespace { get; set; }

        public IEnumerable<TypeDefinition> NestedTypes { get => this.GetNestedTypes(); }
        public ICollection<IDefinition> NestedDefinitions { get; } = new List<IDefinition>();
        public ICollection<Constant> Constants { get; } = new List<Constant>();

        public GenFile(ulong id)
        {
            Id = id;
        }
    }
}
