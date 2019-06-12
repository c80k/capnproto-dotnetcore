using System;

namespace Capnp
{
    /// <summary>
    /// This exception gets thrown when a Cap'n Proto object could not be deserialized correctly.
    /// </summary>
    public class DeserializationException : Exception
    {
        public DeserializationException(string message) : base(message)
        {
        }

        public DeserializationException(string message, Exception innerException): 
            base(message, innerException)
        {
        }
    }
}
