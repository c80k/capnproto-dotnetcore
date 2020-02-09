using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Capnp
{
    /// <summary>
    /// SerializerState specialization for a List(Bool).
    /// </summary>
    public class ListOfBitsSerializer: SerializerState, IReadOnlyList<bool>
    {

        /// <summary>
        /// Gets or sets the element at given index.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Element value</returns>
        /// <exception cref="InvalidOperationException">List was not initialized, or attempting to overwrite a non-null element.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                int wi = index / 64;
                int bi = index % 64;

                return ((RawData[wi] >> bi) & 1) != 0;
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                int wi = index / 64;
                int bi = index % 64;

                if (value)
                    RawData[wi] |= (1ul << bi);
                else
                    RawData[wi] &= ~(1ul << bi);
            }
        }

        /// <summary>
        /// This list's element count.
        /// </summary>
        public int Count => ListElementCount;

        /// <summary>
        /// Initializes this list with a specific size. The list can be initialized only once.
        /// </summary>
        /// <param name="count">List element count</param>
        /// <exception cref="InvalidOperationException">The list was already initialized</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative or greater than 2^29-1</exception>
        public void Init(int count)
        {
            if (IsAllocated)
                throw new InvalidOperationException("Already initialized");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            SetListOfValues(1, count);
        }

        /// <summary>
        /// Initializes the list with given content.
        /// </summary>
        /// <param name="items">List content. Can be null in which case the list is simply not initialized.</param>
        /// <exception cref="InvalidOperationException">The list was already initialized</exception>
        /// <exception cref="ArgumentOutOfRangeException">More than 2^29-1 items.</exception>
        public void Init(IReadOnlyList<bool>? items)
        {
            if (items == null)
            {
                return;
            }

            Init(items.Count);

            for (int i = 0; i < items.Count; i++)
            {
                this[i] = items[i];
            }
        }

        /// <summary>
        /// Implements <see cref="IEnumerable{Boolean}"/>
        /// </summary>
        public IEnumerator<bool> GetEnumerator() => (IEnumerator<bool>)this.ToArray().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.ToArray().GetEnumerator();
    }
}