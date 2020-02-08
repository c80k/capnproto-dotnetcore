using System.Collections.Generic;

namespace CapnpC.CSharp.Generator.Model
{
    class GenFile: IDefinition, IHasNestedDefinitions
    {
        public ulong Id { get;  }
        public bool IsGenerated { get;  }
        public TypeTag Tag { get => TypeTag.File;  }
        public IHasNestedDefinitions DeclaringElement { get => null; }

        public string Name { get; set; }
        public string[] Namespace { get; set; }
        public bool? NullableEnable { get; set; }
        public bool EmitNullableDirective { get; set; }

        public IEnumerable<TypeDefinition> NestedTypes { get => this.GetNestedTypes(); }
        public ICollection<IDefinition> NestedDefinitions { get; } = new List<IDefinition>();
        public ICollection<Constant> Constants { get; } = new List<Constant>();

        public GenFile(ulong id, bool isGenerated)
        {
            Id = id;
            IsGenerated = isGenerated;
        }
    }
}
