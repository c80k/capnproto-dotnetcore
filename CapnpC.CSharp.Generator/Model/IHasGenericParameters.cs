using System.Collections.Generic;

namespace CapnpC.CSharp.Generator.Model
{
    interface IHasGenericParameters
    {
        List<string> GenericParameters { get; }
    }
}
