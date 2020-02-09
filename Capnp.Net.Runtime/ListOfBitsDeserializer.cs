using System;
using System.Collections;
using System.Collections.Generic;

namespace Capnp
{
    /// <summary>
    /// ListDeserializer specialization for a List(Bool).
    /// </summary>
    public class ListOfBitsDeserializer: ListDeserializer, IReadOnlyList<bool>
    {
        readonly bool _defaultValue;

        internal ListOfBitsDeserializer(ref DeserializerState context, bool defaultValue) : 
            base(ref context)
        {
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Always ListKind.ListOfBits
        /// </summary>
        public override ListKind Kind => ListKind.ListOfBits;

        /// <summary>
        /// Gets the element at given index.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Element value</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                int wi = index / 64;
                int bi = index % 64;

                return ((State.CurrentSegment[State.Offset + wi] >> bi) & 1) != 
                    (_defaultValue ? 1u : 0);
            }
        }

        IEnumerable<bool> Enumerate()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        /// <summary>
        /// Implements <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<bool> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Return this
        /// </summary>
        public override IReadOnlyList<bool> CastBool() => this;

        /// <summary>
        /// Always throws <see cref="NotSupportedException"/> since it is not intended to represent a list of bits differently.
        /// </summary>
        public override IReadOnlyList<T> Cast<T>(Func<DeserializerState, T> cons)
        {
            throw new NotSupportedException("Cannot cast a list of bits to anything else");
        }
    }
}