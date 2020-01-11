using System;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// SerializerState specialization for List(Void).
    /// </summary>
    public class ListOfEmptySerializer: 
        SerializerState
    {
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

            SetListOfValues(0, count);
        }
    }
}
#nullable restore