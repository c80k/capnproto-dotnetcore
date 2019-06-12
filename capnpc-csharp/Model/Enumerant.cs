namespace CapnpC.Model
{
    class Enumerant
    {
        public TypeDefinition TypeDefinition { get; set; }
        public string Literal { get; set; }
        public ushort? Ordinal { get; set; }
        public int CodeOrder { get; set; }
    }
}
