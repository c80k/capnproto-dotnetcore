using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// SerializerState specialization for List(T) when T is a known struct (i.e. a list of fixed-width composites).
    /// </summary>
    /// <typeparam name="TS">SerializerState which represents the struct type</typeparam>
    public class ListOfStructsSerializer<TS> : 
        SerializerState, 
        IReadOnlyList<TS>
        where TS : SerializerState, new()
    {
        /// <summary>
        /// Returns the struct serializer a given index.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>The struct serializer</returns>
        public TS this[int index]
        {
            get
            {
                if (!IsAllocated)
                    throw new InvalidOperationException("Not initialized");

                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return ListBuildStruct<TS>(index);
            }
        }

        /// <summary>
        /// This list's element count.
        /// </summary>
        public int Count => ListElementCount;

        /// <summary>
        /// Implementation of <see cref="IEnumerable{TS}"/>/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TS> GetEnumerator()
        {
            if (Count == 0) return Enumerable.Empty<TS>().GetEnumerator();
            return ListEnumerateStructs<TS>().GetEnumerator();
        }

        /// <summary>
        /// Initializes this list with a specific size. The list can be initialized only once.
        /// </summary>
        /// <param name="count">List element count</param>
        public void Init(int count)
        {
            if (IsAllocated)
                throw new InvalidOperationException("Already initialized");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var sample = new TS();
            SetListOfStructs(count, sample.StructDataCount, sample.StructPtrCount);
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
#nullable restore