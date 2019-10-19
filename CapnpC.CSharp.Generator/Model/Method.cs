using System.Collections.Generic;
using System.Linq;

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

        public Method Clone()
        {
            var method = new Method()
            {
                DeclaringInterface = DeclaringInterface,
                Id = Id,
                Name = Name,
                ParamsStruct = ParamsStruct,
                ResultStruct = ResultStruct
            };
            method.Params.AddRange(Params.Select((p => p.Clone())));
            method.Results.AddRange(Results.Select(r => r.Clone()));
            method.GenericParameters.AddRange(GenericParameters);
            return method;
        }
    }
}
