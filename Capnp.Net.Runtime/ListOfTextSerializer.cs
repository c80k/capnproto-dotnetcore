using System;
using System.Collections;
using System.Collections.Generic;

namespace Capnp
{
    /// <summary>
    /// SerializerState specialization for List(Text)
    /// </summary>
    public class ListOfTextSerializer :
        SerializerState,
        IReadOnlyList<string?>
    {
        /// <summary>
        /// Gets or sets the text at given index. Once an element is set, it cannot be overwritten.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <exception cref="InvalidOperationException">List is not initialized</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">UTF-8 encoding exceeds 2^29-2 bytes</exception>
        public string? this[int index]
        {
            get
            {
                if (!IsAllocated)
                    throw new InvalidOperationException("Not initialized");

                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return ReadText(index);
            }
            set
            {
                if (!IsAllocated)
                    throw new InvalidOperationException("Not initialized");

                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                WriteText(index, value);
            }
        }

        /// <summary>
        /// This list's element count.
        /// </summary>
        public int Count => ListElementCount;

        IEnumerable<string?> Enumerate()
        {
            int count = Count;

            for (int i = 0; i < count; i++)
            {
                yield return TryGetPointer<SerializerState>(i)?.ListReadAsText();
            }
        }

        /// <summary>
        /// Implementation of <see cref="IEnumerable{String}"/>/>
        /// </summary>
        public IEnumerator<string?> GetEnumerator()
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
        /// <param name="items">List content. Can be null in which case the list is simply not initialized.</param>
        /// <exception cref="InvalidOperationException">The list was already initialized</exception>
        /// <exception cref="ArgumentOutOfRangeException">More than 2^29-1 items, or the UTF-8 encoding of an individual string requires more than 2^29-2 bytes.</exception>
        public void Init(IReadOnlyList<string> items)
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}