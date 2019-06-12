using System;
using System.Collections.Generic;
using System.Text;

namespace CapnpC.Model
{
    class GenFile: IHasNestedDefinitions
    {
        public string Name { get; set; }
        public string[] Namespace { get; set; }

        public List<TypeDefinition> NestedTypes { get; } = new List<TypeDefinition>();
        public List<Value> Constants { get; } = new List<Value>();
    }
}
