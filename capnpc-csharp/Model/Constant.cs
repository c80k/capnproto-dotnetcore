using System.Diagnostics;

namespace CapnpC.Model
{
    class Constant : IDefinition
    {
        public ulong Id { get; }
        public bool IsGenerated { get;  }
        public TypeTag Tag { get => TypeTag.Const; }
        public IHasNestedDefinitions DeclaringElement { get; }

        public Value Value { get; set; } 

        public Constant(ulong id, IHasNestedDefinitions parent)
        {
            Trace.Assert(parent != null);
            Id = id;
            IsGenerated = (parent as IDefinition).IsGenerated;
            DeclaringElement = parent;
            parent.NestedDefinitions.Add(this);
        }
    }
}
