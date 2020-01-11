using System;
using System.Collections;
using System.Collections.Generic;

namespace Capnp
{
    /// <summary>
    /// ListDeserializer specialization for List(T) when T is a known struct (i.e. a list of fixed-width composites).
    /// </summary>
    public class ListOfStructsDeserializer: ListDeserializer, IReadOnlyList<DeserializerState>
    {
        internal ListOfStructsDeserializer(ref DeserializerState context):
            base(ref context)
        {
        }

        /// <summary>
        /// Always returns <code>ListKind.ListOfStructs</code>.
        /// </summary>
        public override ListKind Kind => ListKind.ListOfStructs;

        /// <summary>
        /// Returns the deserializer state at given index.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Element deserializer state</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="DeserializationException">Traversal limit reached</exception>
        public DeserializerState this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                int stride = State.StructDataCount + State.StructPtrCount;

                var state = State;
                // the “traversal limit” should count a list of zero-sized elements as if each element were one word instead.
                state.IncrementBytesTraversed(checked(8u * (uint)(stride == 0 ? 1 : stride)));
                state.Offset = checked(state.Offset + 1 + index * stride);
                state.Kind = ObjectKind.Struct;

                return state;
            }
        }

        /// <summary>
        /// Converts this list to a different representation by applying an element selector function.
        /// </summary>
        /// <typeparam name="T">Target type after applying the selector function</typeparam>
        /// <param name="cons">The selector function</param>
        /// <returns>The new list representation</returns>
        public override IReadOnlyList<T> Cast<T>(Func<DeserializerState, T> cons)
        {
            return this.LazyListSelect(cons);
        }

        IEnumerable<DeserializerState> Enumerate()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
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
    }
}