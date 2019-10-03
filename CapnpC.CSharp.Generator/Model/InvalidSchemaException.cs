using System;

namespace CapnpC.CSharp.Generator.Model
{
    class InvalidSchemaException : Exception
    {
        public InvalidSchemaException(string message) : base(message)
        {
        }
    }
}
