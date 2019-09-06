using System.Diagnostics;

namespace CapnpC.CSharp.Generator.Model
{
    class Annotation : IDefinition
    {
        public ulong Id { get; }
        public bool IsGenerated { get; }
        public TypeTag Tag { get => TypeTag.Annotation; }
        public IHasNestedDefinitions DeclaringElement { get; }

        public Type Type { get; set; }

        public Annotation(ulong id, IHasNestedDefinitions parent)
        {
            Trace.Assert(parent != null);
            Id = id;
            IsGenerated = (parent as IDefinition).IsGenerated;
            DeclaringElement = parent;
            parent.NestedDefinitions.Add(this);
        }
    }
}
