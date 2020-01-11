using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// ListDeserializer specialization for List(Void).
    /// </summary>
    public class ListOfEmptyDeserializer : ListDeserializer, IReadOnlyList<DeserializerState>
    {
        internal ListOfEmptyDeserializer(ref DeserializerState state) : 
            base(ref state)
        {
        }

        /// <summary>
        /// Returns a DeserializerState representing an element at given index.
        /// This is always the null object, since Void cannot carry any data.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns><code>default(DeserializerState)</code></returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        public DeserializerState this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return default;
            }
        }

        /// <summary>
        /// Always ListKind.ListOfEmpty
        /// </summary>
        public override ListKind Kind => ListKind.ListOfEmpty;

        /// <summary>
        /// Applies a selector function to each element. 
        /// Trivia: Since each element is the null object, the selector function always gets fed with a null object.
        /// </summary>
        /// <typeparam name="T">Element target type</typeparam>
        /// <param name="cons">Selector function</param>
        /// <returns>The desired representation</returns>
        public override IReadOnlyList<T> Cast<T>(Func<DeserializerState, T> cons)
        {
            return this.LazyListSelect(cons);
        }

        /// <summary>
        /// Implements <see cref="IEnumerable{DeserializerState}"/>.
        /// </summary>
        public IEnumerator<DeserializerState> GetEnumerator()
        {
            return Enumerable.Repeat(default(DeserializerState), Count).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
#nullable restore