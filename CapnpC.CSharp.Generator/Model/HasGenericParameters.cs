using System.Collections.Generic;

namespace CapnpC.CSharp.Generator.Model
{
    static class HasGenericParameters
    {
        public static IEnumerable<GenericParameter> GetLocalTypeParameters(this IHasGenericParameters me)
        {
            for (int i = 0; i < me.GenericParameters.Count; i++)
            {
                yield return new GenericParameter()
                {
                    DeclaringEntity = me,
                    Index = i
                };
            }
        }
    }
}
