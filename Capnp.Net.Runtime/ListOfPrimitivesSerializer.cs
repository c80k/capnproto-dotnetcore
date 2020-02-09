using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Capnp
{
    /// <summary>
    /// SerializerState specialization for List(Int*), List(UInt*), List(Float*), and List(Enum).
    /// </summary>
    /// <typeparam name="T">List element type, must be primitive. Static constructor will throw if the type does not work.</typeparam>
    public class ListOfPrimitivesSerializer<T> :
        SerializerState,
        IReadOnlyList<T>
        where T : struct
    {
        static readonly int ElementSize;

        static ListOfPrimitivesSerializer()
        {
            if (typeof(T).IsEnum)
            {
                ElementSize = Marshal.SizeOf(Enum.GetUnderlyingType(typeof(T)));
            }
            else
            {
                ElementSize = Marshal.SizeOf<T>();
            }
        }

        Span<T> Data => MemoryMarshal.Cast<ulong, T>(RawData);

        /// <summary>
        /// Gets or sets the value at given index.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Element value</returns>
        public T this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
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

            SetListOfValues((byte)(8 * ElementSize), count);
        }

        /// <summary>
        /// Initializes the list with given content.
        /// </summary>
        /// <param name="items">List content. Can be null in which case the list is simply not initialized.</param>
        /// <exception cref="InvalidOperationException">The list was already initialized</exception>
        /// <exception cref="ArgumentOutOfRangeException">More than 2^29-1 items.</exception>
        public void Init(IReadOnlyList<T>? items)
        {
            if (items == null)
            {
                return;
            }

            Init(items.Count);

            if (items is T[] array)
            {
                array.CopyTo(Data);
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    this[i] = items[i];
                }
            }
        }

        /// <summary>
        /// Implements <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)Data.ToArray().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Data.ToArray().GetEnumerator();
    }
}