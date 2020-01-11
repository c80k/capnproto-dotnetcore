using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// SerializerState specialization for List(T) when T is unknown (generic), List(Data), List(Text), and List(List(...)).
    /// </summary>
    /// <typeparam name="TS">SerializerState which represents the element type</typeparam>
    public class ListOfPointersSerializer<TS>:
        SerializerState,
        IReadOnlyList<TS?>
        where TS: SerializerState, new()
    {
        /// <summary>
        /// Gets or sets the element at given index.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Serializer state representing the desired element</returns>
        /// <exception cref="InvalidOperationException">List was not initialized, or attempting to overwrite a non-null element.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        public TS this[int index]
        {
            get
            {
                if (!IsAllocated)
                    throw new InvalidOperationException("Not initialized");

                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return BuildPointer<TS>(index);
            }
            set
            {
                if (!IsAllocated)
                    throw new InvalidOperationException("Not initialized");

                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                Link(index, value, true);
            }
        }

        /// <summary>
        /// This list's element count.
        /// </summary>
        public int Count => ListElementCount;

        IEnumerable<TS?> Enumerate()
        {
            int count = Count;

            for (int i = 0; i < count; i++)
            {
                yield return TryGetPointer<TS>(i);
            }
        }

        /// <summary>
        /// Implements <see cref="IEnumerable{TS}"/>.
        /// </summary>
        public IEnumerator<TS?> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

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

            SetListOfPointers(count);
        }

        /// <summary>
        /// Initializes the list with given content.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="items">List content. Can be null in which case the list is simply not initialized.</param>
        /// <param name="init">Serialization action to transfer a particular item into the serializer state.</param>
        /// <exception cref="InvalidOperationException">The list was already initialized</exception>
        /// <exception cref="ArgumentOutOfRangeException">More than 2^29-1 items.</exception>
        public void Init<T>(IReadOnlyList<T> items, Action<TS, T> init)
        {
            if (items == null)
            {
                return;
            }

            Init(items.Count);

            for (int i = 0; i < items.Count; i++)
            {
                init(this[i], items[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
#nullable enable