using System;
using System.Collections;
using System.Collections.Generic;

namespace Capnp
{
    /// <summary>
    /// ListDeserializer specialization for List(T) when T is unknown (generic), List(Data), List(Text), and List(List(...)).
    /// </summary>
    public class ListOfPointersDeserializer: ListDeserializer, IReadOnlyList<DeserializerState>
    {
        internal ListOfPointersDeserializer(ref DeserializerState state) : 
            base(ref state)
        {
        }

        /// <summary>
        /// Always <code>ListKind.ListOfPointers</code>
        /// </summary>
        public override ListKind Kind => ListKind.ListOfPointers;

        /// <summary>
        /// Gets the DeserializerState representing the element at given index.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>DeserializerState representing the element at given index</returns>
        public DeserializerState this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                var state = State;

                state.DecodePointer(index);

                return state;
            }
        }

        IEnumerable<DeserializerState> Enumerate()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        /// <summary>
        /// Implements <see cref="IEnumerable{DeserializerState}"/>.
        /// </summary>
        public IEnumerator<DeserializerState> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Applies a selector function to each element.
        /// </summary>
        /// <typeparam name="T">Element target type</typeparam>
        /// <param name="cons">Selector function</param>
        /// <returns>The desired representation</returns>
        public override IReadOnlyList<T> Cast<T>(Func<DeserializerState, T> cons)
        {
            return this.LazyListSelect(cons);
        }

        /// <summary>
        /// Interprets this instance as List(List(...)).
        /// </summary>
        /// <returns>The desired representation. Since it is evaluated lazily, type conflicts will not happen before accessing the resulting list's elements.</returns>
        public override IReadOnlyList<ListDeserializer> CastList()
        {
            return this.LazyListSelect(d => d.RequireList());
        }

        /// <summary>
        /// Interprets this instance as a list of capabilities.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <returns>The desired representation. Since it is evaluated lazily, type conflicts will not happen before accessing the resulting list's elements.</returns>
        public override IReadOnlyList<ListOfCapsDeserializer<T>> CastCapList<T>()
        {
            return this.LazyListSelect(d => d.RequireCapList<T>());
        }
    }
}
