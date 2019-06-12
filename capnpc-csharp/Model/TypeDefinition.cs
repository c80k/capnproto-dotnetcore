using System.Collections.Generic;
using System.Linq;
namespace CapnpC.Model
{
    class TypeDefinition : AbstractType, IHasNestedDefinitions, IHasGenericParameters
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

        public TypeDefinition(TypeTag tag, ulong id)
        {
            Tag = tag;
            Id = id;
        }

        public ulong Id { get; }
        public IHasNestedDefinitions DeclaringElement { get; set; }
        public Method UsingMethod { get; set; }
        public string Name { get; set; }
        public SpecialName SpecialName { get; set; }
        public DiscriminationInfo UnionInfo { get; set; }
        public new List<Field> Fields => base.Fields;
        public List<Enumerant> Enumerants { get; } = new List<Enumerant>();
        public List<TypeDefinition> NestedTypes { get; } = new List<TypeDefinition>();
        public List<TypeDefinition> NestedGroups { get; } = new List<TypeDefinition>();
        public List<Value> Constants { get; } = new List<Value>();
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
