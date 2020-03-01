using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Capnp
{
    /// <summary>
    /// Implements an empty <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    /// <typeparam name="T">list element type</typeparam>
    public class EmptyList<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Always throws an <see cref="IndexOutOfRangeException"/>.
        /// </summary>
        /// <param name="index">Ignored</param>
        public T this[int index] => throw new IndexOutOfRangeException(nameof(index));

        /// <summary>
        /// Always 0.
        /// </summary>
        public int Count => 0;

        /// <summary>
        /// Returns an empty enumerator.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}