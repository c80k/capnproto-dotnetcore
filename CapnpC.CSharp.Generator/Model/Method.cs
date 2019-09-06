using System.Collections.Generic;

namespace CapnpC.CSharp.Generator.Model
{
    class Method: IHasGenericParameters
    {
        public TypeDefinition DeclaringInterface { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Field> Params { get; } = new List<Field>();
        public List<Field> Results { get; } = new List<Field>();
        public Type ParamsStruct { get; set; }
        public Type ResultStruct { get; set; }
        public List<string> GenericParameters { get; } = new List<string>();
    }
}
