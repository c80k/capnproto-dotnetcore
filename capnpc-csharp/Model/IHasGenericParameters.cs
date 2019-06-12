using System.Collections.Generic;

namespace CapnpC.Model
{
    interface IHasGenericParameters
    {
        List<string> GenericParameters { get; }
    }
}
