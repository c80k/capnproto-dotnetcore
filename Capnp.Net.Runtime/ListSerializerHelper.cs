using System;

namespace Capnp
{
    static class ListSerializerHelper
    {
        public static void EnsureAllocated(SerializerState serializer)
        {
            if (!serializer.IsAllocated)
                throw new InvalidOperationException("Call Init() first");
        }
    }
}