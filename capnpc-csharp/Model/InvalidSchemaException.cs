using System;

namespace CapnpC.Model
{
    class InvalidSchemaException : Exception
    {
        public InvalidSchemaException(string message) : base(message)
        {
        }
    }
}
