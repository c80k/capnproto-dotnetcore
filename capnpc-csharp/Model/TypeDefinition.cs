using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace CapnpC.Model
{
    class TypeDefinition : AbstractType, IDefinition, IHasNestedDefinitions, IHasGenericParameters
    {
        public class DiscriminationInfo
        {
            public DiscriminationInfo(ushort numOptions, uint tagOffset)
            {
                NumOptions = numOptions;
                TagOffset = tagOffset;
            }

            public ushort NumOptions { get; }
            public uint TagOffset { get; }
        }

        public TypeDefinition(TypeTag tag, ulong id, IHasNestedDefinitions parent)
        {
            Trace.Assert(parent != null);
            Tag = tag;
            Id = id;
            DeclaringElement = parent;
            if (tag == TypeTag.Group)
                ((TypeDefinition)parent).NestedGroups.Add(this);
            else
                parent.NestedDefinitions.Add(this);
        }

        public ulong Id { get; }
        public IHasNestedDefinitions DeclaringElement { get; }

        public Method UsingMethod { get; set; }
        public string Name { get; set; }
        public SpecialName SpecialName { get; set; }
        public DiscriminationInfo UnionInfo { get; set; }
        public new List<Field> Fields => base.Fields;
        public List<Enumerant> Enumerants { get; } = new List<Enumerant>();
        public ICollection<IDefinition> NestedDefinitions { get; } = new List<IDefinition>();
        public IEnumerable<TypeDefinition> NestedTypes { get => this.GetNestedTypes(); }
        public List<TypeDefinition> NestedGroups { get; } = new List<TypeDefinition>();
        public ICollection<Constant> Constants { get; } = new List<Constant>();
        public List<Method> Methods { get; } = new List<Method>();
        public List<Type> Superclasses { get; } = new List<Type>();
        public List<string> GenericParameters { get; } = new List<string>();
        public bool IsGeneric { get; set; }
        public ushort StructDataWordCount { get; set; }
        public ushort StructPointerCount { get; set; }

        public IEnumerable<TypeDefinition> DefinitionHierarchy
        {
            get
            {
                IHasNestedDefinitions cur = this;

                while (cur is TypeDefinition def)
                {
                    yield return def;
                    cur = def.DeclaringElement;
                }
            }
        }

        public GenFile File
        {
            get
            {
                IHasNestedDefinitions cur = this;
                while (cur is TypeDefinition def) cur = def.DeclaringElement;
                return cur as GenFile;
            }
        }

        public IEnumerable<GenericParameter> AllTypeParameters
        {
            get
            {
                return from def in DefinitionHierarchy
                       from p in def.GetLocalTypeParameters()
                       select p;
            }
        }
    }
}
