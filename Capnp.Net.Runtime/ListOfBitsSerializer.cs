using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Capnp
{
    /// <summary>
    /// SerializerState specialization for a List(Bool).
    /// </summary>
    public class ListOfBitsSerializer: SerializerState, IReadOnlyList<bool>
    {
        class Enumerator : IEnumerator<bool>
        {
            readonly ListOfBitsSerializer _self;
            int _pos = -1;

            public Enumerator(ListOfBitsSerializer self)
            {
                _self = self;
            }

            public bool Current => _pos >= 0 && _pos < _self.Count ? _self[_pos] : false;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return ++_pos < _self.Count;
            }

            public void Reset()
            {
                _pos = -1;
            }
        }

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
                ListSerializerHelper.EnsureAllocated(this);

                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                int wi = index / 64;
                int bi = index % 64;

                return ((RawData[wi] >> bi) & 1) != 0;
            }
            set
            {
                ListSerializerHelper.EnsureAllocated(this);

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
        public IEnumerator<bool> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}