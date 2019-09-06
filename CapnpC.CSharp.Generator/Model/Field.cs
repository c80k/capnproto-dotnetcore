namespace CapnpC.CSharp.Generator.Model
{
    class Field
    {
        public TypeDefinition DeclaringType { get; set; }
        public Field Parent { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public Value DefaultValue { get; set; }
        public bool DefaultValueIsExplicit { get; set; }
        public ushort? DiscValue { get; set; }
        public uint Offset { get; set; }
        public int CodeOrder { get; set; }

        public ulong? BitOffset => (ulong)Offset * Type?.FixedBitWidth;

        public Field Clone()
        {
            var field = new Field()
            {
                DeclaringType = DeclaringType,
                Parent = Parent,
                Name = Name,
                Type = Type,
                DefaultValue = DefaultValue,
                DefaultValueIsExplicit = DefaultValueIsExplicit,
                DiscValue = DiscValue,
                Offset = Offset,
                CodeOrder = CodeOrder,
            };
            field.InheritFreeGenericParameters();
            return field;
        }

        public void InheritFreeGenericParameters()
        {
            Type.InheritFreeParameters(DeclaringType);
        }

        public override bool Equals(object obj)
        {
            return obj is Field other &&
                DeclaringType == other.DeclaringType &&
                Name == other.Name;
        }

        public override int GetHashCode()
        {
            return (DeclaringType, Name).GetHashCode();
        }
    }
}
