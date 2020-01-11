using System;
using System.Collections;
using System.Collections.Generic;

namespace Capnp
{
    /// <summary>
    /// ListDeserializer specialization for a list of capabilities.
    /// </summary>
    /// <typeparam name="T">Capability interface</typeparam>
    public class ListOfCapsDeserializer<T> : ListDeserializer, IReadOnlyList<T>
        where T: class
    {
        internal ListOfCapsDeserializer(ref DeserializerState state) : base(ref state)
        {
            Rpc.CapabilityReflection.ValidateCapabilityInterface(typeof(T));
        }

        /// <summary>
        /// Returns the capability at given index.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>The capability at given index (in terms of its proxy instance)</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return (Rpc.CapabilityReflection.CreateProxy<T>(State.DecodeCapPointer(index)) as T)!;
            }
        }

        /// <summary>
        /// Always ListKind.ListOfPointers
        /// </summary>
        public override ListKind Kind => ListKind.ListOfPointers;

        /// <summary>
        /// Always throws <see cref="NotSupportedException"/>, since it is not intended to convert a capability list to anything else.
        /// </summary>
        public override IReadOnlyList<T1> Cast<T1>(Func<DeserializerState, T1> cons)
        {
            throw new NotSupportedException("Cannot cast a list of capabilities to anything else");
        }

        IEnumerable<T> Enumerate()
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Implements <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}