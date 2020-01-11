using System;
using System.Collections;
using System.Collections.Generic;

namespace Capnp
{
    /// <summary>
    /// SerializerState specialization for a list of capabilities.
    /// </summary>
    /// <typeparam name="T">Capability interface</typeparam>
    public class ListOfCapsSerializer<T> :
        SerializerState,
        IReadOnlyList<T>
        where T : class
    {
        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <exception cref="Rpc.InvalidCapabilityInterfaceException"><typeparamref name="T"/> does not quality as capability interface.
        /// The implementation might attempt to throw this exception earlier in the static constructor (during type load). Currently it doesn't
        /// because there is a significant risk of messing something up and ending with a hard-to-debug <see cref="TypeLoadException"/>.</exception>
        public ListOfCapsSerializer()
        {
            Rpc.CapabilityReflection.ValidateCapabilityInterface(typeof(T));
        }

        /// <summary>
        /// Gets or sets the capability at given element index.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Proxy object of capability at given element index</returns>
        /// <exception cref="InvalidOperationException">List was not initialized, or attempting to overwrite an already set element.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        public T this[int index]
        {
            get => (Rpc.CapabilityReflection.CreateProxy<T>(DecodeCapPointer(index)) as T)!;
            set
            {
                if (!IsAllocated)
                    throw new InvalidOperationException("Call Init() first");

                if (index < 0 || index >= RawData.Length)
                    throw new IndexOutOfRangeException("index out of range");

                uint id = ProvideCapability(value);
                WirePointer ptr = default;
                ptr.SetCapability(id);
                RawData[index] = id;
            }
        }

        /// <summary>
        /// Initializes this list with a specific size. The list can be initialized only once.
        /// </summary>
        /// <param name="count">List element count</param>
        /// <exception cref="InvalidOperationException">The list was already initialized</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative or greater than 2^29-1</exception>
        public void Init(int count)
        {
            SetListOfPointers(count);
        }

        /// <summary>
        /// Initializes the list with given content.
        /// </summary>
        /// <param name="caps">List content. Can be null in which case the list is simply not initialized.</param>
        /// <exception cref="InvalidOperationException">The list was already initialized</exception>
        /// <exception cref="ArgumentOutOfRangeException">More than 2^29-1 items.</exception>
        public void Init(IReadOnlyList<T> caps)
        {
            if (caps == null)
            {
                return;
            }

            Init(caps.Count);

            for (int i = 0; i < caps.Count; i++)
            {
                this[i] = caps[i];
            }
        }

        /// <summary>
        /// This list's element count.
        /// </summary>
        public int Count => ListElementCount;

        IEnumerable<T> Enumerate()
        {
            int count = Count;
            for (int i = 0; i < count; i++)
                yield return this[i];
        }

        /// <summary>
        /// Implements <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}