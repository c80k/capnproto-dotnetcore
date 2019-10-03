namespace CapnpC.CSharp.Generator.Model
{
    class GenericParameter
    {
        public IHasGenericParameters DeclaringEntity { get; set; }
        public int Index { get; set; }
        public string Name => DeclaringEntity.GenericParameters[Index];

        public override bool Equals(object obj)
        {
            // Instead of equality by Name, we could instead take (DeclaringEntity, Index), but there is a caveat:
            // Since methods can also own generic parameters, we have different classes of declaring entities involved.
            // Both the method will define generic parameters, and the Cap'n'p-generated params/result structs.
            // Therefore we end in two GenericParameter instances, one with the Method as declaring entity, the
            // other one with the params/result type definition as declaring entity. They are semantically the same,
            // and the easy way to match them is by Name. Equality by Name is the only working choice, even though
            // it feels a little less reboust than by matching declaring entity + parameter position.
            return obj is GenericParameter other && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
