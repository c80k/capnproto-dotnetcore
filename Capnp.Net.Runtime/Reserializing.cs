using System;
using System.Collections.Generic;
using System.Text;

namespace Capnp
{
    /// <summary>
    /// Provides deep-copy functionality to re-serialize an existing deserializer state into another serializer state.
    /// </summary>
    public static class Reserializing
    {
        /// <summary>
        /// Performs a deep copy of an existing deserializer state into another serializer state.
        /// This implementation does not analyze the source object graph and therefore cannot detect multiple references to the same object.
        /// Such cases will result in object duplication.
        /// </summary>
        /// <param name="from">source state</param>
        /// <param name="to">target state</param>
        /// <exception cref="ArgumentNullException"><paramref name="to"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Target state was already set to a different object type than the source state.</exception>
        /// <exception cref="DeserializationException">Security violation due to amplification attack or stack overflow DoS attack, 
        /// or illegal pointer detected during deserialization.</exception>
        public static void DeepCopy(DeserializerState from, SerializerState to)
        {
            if (to == null)
                throw new ArgumentNullException(nameof(to));

            var ds = to.Rewrap<DynamicSerializerState>();

            IReadOnlyList<DeserializerState> items;

            switch (from.Kind)
            {
                case ObjectKind.Struct:
                    ds.SetStruct(from.StructDataCount, from.StructPtrCount);
                    ds.Allocate();
                    from.StructDataSection.CopyTo(ds.StructDataSection);
                    for (int i = 0; i < from.StructPtrCount; i++)
                    {
                        DeepCopy(from.StructReadPointer(i), ds.BuildPointer(i));
                    }
                    break;

                case ObjectKind.ListOfBits:
                    ds.SetListOfValues(1, from.ListElementCount);
                    from.RawData.CopyTo(ds.RawData);
                    break;

                case ObjectKind.ListOfBytes:
                    ds.SetListOfValues(8, from.ListElementCount);
                    from.RawData.CopyTo(ds.RawData);
                    break;

                case ObjectKind.ListOfEmpty:
                    ds.SetListOfValues(0, from.ListElementCount);
                    break;

                case ObjectKind.ListOfInts:
                    ds.SetListOfValues(32, from.ListElementCount);
                    from.RawData.CopyTo(ds.RawData);
                    break;

                case ObjectKind.ListOfLongs:
                    ds.SetListOfValues(64, from.ListElementCount);
                    from.RawData.CopyTo(ds.RawData);
                    break;

                case ObjectKind.ListOfShorts:
                    ds.SetListOfValues(16, from.ListElementCount);
                    from.RawData.CopyTo(ds.RawData);
                    break;

                case ObjectKind.ListOfPointers:
                    ds.SetListOfPointers(from.ListElementCount);
                    items = (IReadOnlyList<DeserializerState>)from.RequireList();
                    for (int i = 0; i < from.ListElementCount; i++)
                    {
                        DeepCopy(items[i], ds.BuildPointer(i));
                    }
                    break;

                case ObjectKind.ListOfStructs:
                    ds.SetListOfStructs(from.ListElementCount, from.StructDataCount, from.StructPtrCount);
                    items = (IReadOnlyList<DeserializerState>)from.RequireList();
                    for (int i = 0; i < from.ListElementCount; i++)
                    {
                        DeepCopy(items[i], ds.ListBuildStruct(i));
                    }
                    break;

                case ObjectKind.Capability:
                    ds.SetCapability(from.CapabilityIndex);
                    break;
            }

            to.InheritFrom(ds);
        }
    }
}
