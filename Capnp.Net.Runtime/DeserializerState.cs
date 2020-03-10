using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Capnp
{
    /// <summary>
    /// Implements the heart of deserialization. This stateful helper struct exposes all functionality to traverse serialized data.
    /// Although it is public, you should not use it directly. Instead, use the reader, writer, and domain class adapters which are produced
    /// by the code generator.
    /// </summary>
    public struct DeserializerState: IStructDeserializer
    {
        /// <summary>
        /// A wire message is essentially a collection of memory blocks.
        /// </summary>
        public IReadOnlyList<Memory<ulong>> Segments { get; }
        /// <summary>
        /// Index of the segment (into the Segments property) which this state currently refers to.
        /// </summary>
        public uint CurrentSegmentIndex { get; private set; }
        /// <summary>
        /// Word offset within the current segment which this state currently refers to.
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// Context-dependent meaning: Usually the number of bytes traversed until this state was reached, to prevent amplification attacks.
        /// However, if this state is of Kind == ObjectKind.Value (an artificial category which will never occur on the wire but is used to
        /// internally represent lists of primitives as lists of structs), it contains the primitive's value.
        /// </summary>
        public uint BytesTraversedOrData { get; set; }
        /// <summary>
        /// If this state currently represents a list, the number of list elements.
        /// </summary>
        public int ListElementCount { get; private set; }
        /// <summary>
        /// If this state currently represents a struct, the struct's data section word count.
        /// </summary>
        public ushort StructDataCount { get; set; }
        /// <summary>
        /// If this state currently represents a struct, the struct's pointer section word count.
        /// </summary>
        public ushort StructPtrCount { get; set; }
        /// <summary>
        /// The kind of object this state currently represents.
        /// </summary>
        public ObjectKind Kind { get; set; }
        /// <summary>
        /// The capabilities imported from the capability table. Only valid in RPC context.
        /// </summary>
        public IList<Rpc.ConsumedCapability?>? Caps { get; set; }
        /// <summary>
        /// Current segment (essentially Segments[CurrentSegmentIndex]
        /// </summary>
        public ReadOnlySpan<ulong> CurrentSegment => Segments != null ? Segments[(int)CurrentSegmentIndex].Span : default;

        DeserializerState(IReadOnlyList<Memory<ulong>> segments)
        {
            Segments = segments;
            CurrentSegmentIndex = 0;
            Offset = 0;
            BytesTraversedOrData = 0;
            ListElementCount = 0;
            StructDataCount = 0;
            StructPtrCount = 1;
            Kind = ObjectKind.Struct;
            Caps = null;
        }

        /// <summary>
        /// Constructs a state representing a message root object.
        /// </summary>
        /// <param name="frame">the message</param>
        /// <returns></returns>
        public static DeserializerState CreateRoot(WireFrame frame)
        {
            var state = new DeserializerState(frame.Segments);
            state.DecodePointer(0);
            return state;
        }

        /// <summary>
        /// Implicitly converts a serializer state into a deserializer state.
        /// The conversion is cheap, since it does not involve copying any payload.
        /// </summary>
        /// <param name="state">The serializer state to be converted</param>
        /// <exception cref="ArgumentNullException"><paramref name="state"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="state"/> is not bound to a MessageBuilder</exception>
        public static implicit operator DeserializerState(SerializerState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            if (state.MsgBuilder == null)
                throw new InvalidOperationException("state is not bound to a MessageBuilder");
            
            switch (state.Kind)
            {
                case ObjectKind.ListOfBits:
                case ObjectKind.ListOfBytes:
                case ObjectKind.ListOfEmpty:
                case ObjectKind.ListOfInts:
                case ObjectKind.ListOfLongs:
                case ObjectKind.ListOfPointers:
                case ObjectKind.ListOfShorts:
                case ObjectKind.ListOfStructs:
                case ObjectKind.Nil:
                case ObjectKind.Struct:
                    if (state.Caps != null)
                    {
                        foreach (var cap in state.Caps)
                            cap?.Release(true);
                    }
                    return new DeserializerState(state.Allocator!.Segments)
                    {
                        CurrentSegmentIndex = state.SegmentIndex,
                        Offset = state.Offset,
                        ListElementCount = state.ListElementCount,
                        StructDataCount = state.StructDataCount,
                        StructPtrCount = state.StructPtrCount,
                        Kind = state.Kind,
                        Caps = state.Caps
                    };

                case ObjectKind.Capability:
                    return new DeserializerState(state.Allocator!.Segments)
                    {
                        Kind = ObjectKind.Capability,
                        Caps = state.Caps,
                        BytesTraversedOrData = state.CapabilityIndex
                    };

                default:
                    throw new ArgumentException("Unexpected type of object, cannot convert that into DeserializerState", nameof(state));
            }
        }

        /// <summary>
        /// Constructs a state representing the given value. This kind of state is artificial and beyond the Cap'n Proto specification.
        /// We need it to internally represent list of primitive values as lists of structs.
        /// </summary>
        public static DeserializerState MakeValueState(uint value)
        {
            return new DeserializerState()
            {
                BytesTraversedOrData = value,
                Kind = ObjectKind.Value
            };
        }

        /// <summary>
        /// Increments the number of bytes traversed and checks the results against the traversal limit.
        /// </summary>
        /// <param name="additionalBytesTraversed">Amount to increase the traversed bytes</param>
        public void IncrementBytesTraversed(uint additionalBytesTraversed)
        {
            BytesTraversedOrData = checked(BytesTraversedOrData + additionalBytesTraversed);

            if (BytesTraversedOrData > SecurityOptions.TraversalLimit)
                throw new DeserializationException("Traversal limit was reached");
        }
        /// <summary>
        /// Memory span which represents this struct's data section (given this state actually represents a struct)
        /// </summary>
        public ReadOnlySpan<ulong> StructDataSection => CurrentSegment.Slice(Offset, StructDataCount);

        ReadOnlySpan<ulong> GetRawBits() => CurrentSegment.Slice(Offset, (ListElementCount + 63) / 64);
        ReadOnlySpan<ulong> GetRawBytes() => CurrentSegment.Slice(Offset, (ListElementCount + 7) / 8);
        ReadOnlySpan<ulong> GetRawShorts() => CurrentSegment.Slice(Offset, (ListElementCount + 3) / 4);
        ReadOnlySpan<ulong> GetRawInts() => CurrentSegment.Slice(Offset, (ListElementCount + 1) / 2);
        ReadOnlySpan<ulong> GetRawLongs() => CurrentSegment.Slice(Offset, ListElementCount);
        /// <summary>
        /// If this state represents a list of primitive values, returns the raw list data.
        /// </summary>
        public ReadOnlySpan<ulong> RawData
        {
            get
            {
                return Kind switch
                {
                    ObjectKind.ListOfBits => GetRawBits(),
                    ObjectKind.ListOfBytes => GetRawBytes(),
                    ObjectKind.ListOfShorts => GetRawShorts(),
                    ObjectKind.ListOfInts => GetRawInts(),
                    ObjectKind.ListOfLongs => GetRawLongs(),
                    _ => default,
                };
            }
        }

        void Validate()
        {
            try
            {
                switch (Kind)
                {
                    case ObjectKind.Struct:
                        CurrentSegment.Slice(Offset, StructDataCount + StructPtrCount);
                        break;

                    case ObjectKind.ListOfBits:
                        GetRawBits();
                        break;

                    case ObjectKind.ListOfBytes:
                        GetRawBytes();
                        break;

                    case ObjectKind.ListOfInts:
                        GetRawInts();
                        break;

                    case ObjectKind.ListOfLongs:
                    case ObjectKind.ListOfPointers:
                        GetRawLongs();
                        break;

                    case ObjectKind.ListOfStructs:
                        CurrentSegment.Slice(Offset, checked(ListElementCount * (StructDataCount + StructPtrCount)));
                        break;
                }
            }
            catch (Exception problem)
            {
                throw new DeserializationException("Invalid wire pointer", problem);
            }
        }

        /// <summary>
        /// Interprets a pointer within the current segment and mutates this state to represent the pointer's target.
        /// </summary>
        /// <param name="offset">word offset relative to this.Offset within current segment</param>
        /// <exception cref="IndexOutOfRangeException">offset negative or out of range</exception>
        /// <exception cref="DeserializationException">invalid pointer data or traversal limit exceeded</exception>
        internal void DecodePointer(int offset)
        {
            if (offset < 0)
                throw new IndexOutOfRangeException(nameof(offset));

            WirePointer pointer = CurrentSegment[Offset + offset];

            int derefCount = 0;

            do
            {
                if (pointer.IsNull)
                {
                    this = default;
                    return;
                }

                switch (pointer.Kind)
                {
                    case PointerKind.Struct:
                        Offset = checked(pointer.Offset + Offset + offset + 1);
                        IncrementBytesTraversed(checked(8u * pointer.StructSize));
                        StructDataCount = pointer.StructDataCount;
                        StructPtrCount = pointer.StructPtrCount;
                        Kind = ObjectKind.Struct;
                        Validate();
                        return;

                    case PointerKind.List:
                        Offset = checked(pointer.Offset + Offset + offset + 1);
                        ListElementCount = pointer.ListElementCount;
                        StructDataCount = 0;
                        StructPtrCount = 0;

                        switch (pointer.ListKind)
                        {
                            case ListKind.ListOfEmpty: // e.g. List(void)
                                                        // the “traversal limit” should count a list of zero-sized elements as if each element were one word instead.
                                IncrementBytesTraversed(checked(8u * (uint)ListElementCount));
                                Kind = ObjectKind.ListOfEmpty;
                                break;

                            case ListKind.ListOfBits:
                                IncrementBytesTraversed(checked((uint)ListElementCount + 7) / 8);
                                Kind = ObjectKind.ListOfBits;
                                break;

                            case ListKind.ListOfBytes:
                                IncrementBytesTraversed((uint)ListElementCount);
                                Kind = ObjectKind.ListOfBytes;
                                break;

                            case ListKind.ListOfShorts:
                                IncrementBytesTraversed(checked(2u * (uint)ListElementCount));
                                Kind = ObjectKind.ListOfShorts;
                                break;

                            case ListKind.ListOfInts:
                                IncrementBytesTraversed(checked(4u * (uint)ListElementCount));
                                Kind = ObjectKind.ListOfInts;
                                break;

                            case ListKind.ListOfLongs:
                                IncrementBytesTraversed(checked(8u * (uint)ListElementCount));
                                Kind = ObjectKind.ListOfLongs;
                                break;

                            case ListKind.ListOfPointers:
                                IncrementBytesTraversed(checked(8u * (uint)ListElementCount));
                                Kind = ObjectKind.ListOfPointers;
                                break;

                            case ListKind.ListOfStructs:
                                {
                                    WirePointer tag = CurrentSegment[Offset];
                                    if (tag.Kind != PointerKind.Struct)
                                        throw new DeserializationException("Unexpected: List of composites with non-struct type tag");
                                    IncrementBytesTraversed(checked(8u * (uint)pointer.ListElementCount + 8u));
                                    ListElementCount = tag.ListOfStructsElementCount;
                                    StructDataCount = tag.StructDataCount;
                                    StructPtrCount = tag.StructPtrCount;
                                    Kind = ObjectKind.ListOfStructs;
                                }
                                break;

                            default:
                                throw new InvalidProgramException();

                        }
                        Validate();
                        return;

                    case PointerKind.Far:

                        if (pointer.IsDoubleFar)
                        {
                            CurrentSegmentIndex = pointer.TargetSegmentIndex;
                            Offset = 0;

                            WirePointer pointer1 = CurrentSegment[pointer.LandingPadOffset];
                            if (pointer1.Kind != PointerKind.Far || pointer1.IsDoubleFar)
                                throw new DeserializationException("Error decoding double-far pointer: convention broken");

                            WirePointer pointer2 = CurrentSegment[pointer.LandingPadOffset + 1];
                            if (pointer2.Kind == PointerKind.Far)
                                throw new DeserializationException("Error decoding double-far pointer: not followed by intra-segment pointer");

                            CurrentSegmentIndex = pointer1.TargetSegmentIndex;
                            Offset = 0;
                            pointer = pointer2;
                            offset = -1;
                        }
                        else
                        {
                            CurrentSegmentIndex = pointer.TargetSegmentIndex;
                            Offset = 0;
                            offset = pointer.LandingPadOffset;
                            pointer = CurrentSegment[pointer.LandingPadOffset];
                        }
                        continue;

                    case PointerKind.Other:
                        var tmp = Caps;
                        this = default;
                        Caps = tmp;
                        Kind = ObjectKind.Capability;
                        BytesTraversedOrData = pointer.CapabilityIndex;
                        return;

                    default:
                        throw new InvalidProgramException();
                }

            } while (++derefCount < SecurityOptions.RecursionLimit);

            throw new DeserializationException("Recursion limit reached while decoding a pointer");
        }

        /// <summary>
        /// Interprets a pointer within the current segment as capability pointer and returns the according low-level capability object from
        /// the capability table. Does not mutate this state.
        /// </summary>
        /// <param name="offset">Offset relative to this.Offset within current segment</param>
        /// <returns>the low-level capability object, or null if it is a null pointer</returns>
        /// <exception cref="IndexOutOfRangeException">offset negative or out of range</exception>
        /// <exception cref="InvalidOperationException">capability table not set</exception>
        /// <exception cref="Rpc.RpcException">not a capability pointer or invalid capability index</exception>
        internal Rpc.ConsumedCapability? DecodeCapPointer(int offset)
        {
            if (offset < 0)
            {
                throw new IndexOutOfRangeException(nameof(offset));
            }

            if (Caps == null)
            {
                throw new InvalidOperationException("Capbility table not set");
            }

            WirePointer pointer = CurrentSegment[Offset + offset];

            if (pointer.IsNull)
            {
                // Despite this behavior is not officially specified, 
                // the official C++ implementation seems to send null pointers for null caps.
                return null;
            }

            if (pointer.Kind != PointerKind.Other)
            {
                throw new Rpc.RpcException("Expected a capability pointer, but got something different");
            }

            if (pointer.CapabilityIndex >= Caps.Count)
            {
                throw new Rpc.RpcException("Capability index out of range");
            }

            return Caps[(int)pointer.CapabilityIndex];
        }

        /// <summary>
        /// Reads a slice of up to 64 bits from this struct's data section, starting from the specified bit offset.
        /// The slice must be aligned within a 64 bit word boundary.
        /// </summary>
        /// <param name="bitOffset">Start bit offset relative to the data section, little endian</param>
        /// <param name="bitCount">numbers of bits to read</param>
        /// <returns>the data</returns>
        /// <exception cref="ArgumentOutOfRangeException">non-aligned access</exception>
        /// <exception cref="IndexOutOfRangeException">bitOffset exceeds the data section</exception>
        /// <exception cref="DeserializationException">this state does not represent a struct</exception>
        public ulong StructReadData(ulong bitOffset, int bitCount)
        {
            switch (Kind)
            {
                case ObjectKind.Nil:
                    return 0;

                case ObjectKind.Struct:

                    int index = checked((int)(bitOffset / 64));
                    int relBitOffset = (int)(bitOffset % 64);

                    var data = StructDataSection;

                    if (index >= data.Length)
                        return 0; // Assume backwards-compatible change

                    if (relBitOffset + bitCount > 64)
                        throw new ArgumentOutOfRangeException(nameof(bitCount));

                    ulong word = data[index];

                    if (bitCount == 64)
                    {
                        return word;
                    }
                    else
                    {
                        ulong mask = (1ul << bitCount) - 1;
                        return (word >> relBitOffset) & mask;
                    }

                case ObjectKind.Value:

                    if (bitOffset >= 32) return 0;
                    if (bitCount >= 32) return BytesTraversedOrData >> (int)bitOffset;
                    return (BytesTraversedOrData >> (int)bitOffset) & ((1u << bitCount) - 1);

                default:
                    throw new DeserializationException("This is not a struct");
            }
        }

        /// <summary>
        /// Decodes a pointer from this struct's pointer section and returns the state representing the pointer target.
        /// It is valid to specify an index beyond the pointer section, in which case a default state (representing the "null object")
        /// will be returned. This is to preserve upward compatibility with schema evolution.
        /// </summary>
        /// <param name="index">Index within the pointer section</param>
        /// <returns>the target state</returns>
        /// <exception cref="DeserializationException">this state does not represent a struct,
        /// invalid pointer, or traversal limit exceeded</exception>
        public DeserializerState StructReadPointer(int index)
        {
            if (Kind != ObjectKind.Struct && Kind != ObjectKind.Nil)
                throw new DeserializationException("This is not a struct");

            if (index >= StructPtrCount)
                return default;

            DeserializerState state = this;
            state.DecodePointer(index + StructDataCount);

            return state;
        }

        internal Rpc.ConsumedCapability? StructReadRawCap(int index)
        {
            if (Kind != ObjectKind.Struct && Kind != ObjectKind.Nil)
                throw new InvalidOperationException("Allowed on structs only");

            if (index >= StructPtrCount)
                return null;

            return DecodeCapPointer(index + StructDataCount);
        }

        /// <summary>
        /// Given this state represents a list (of anything), returns a ListDeserializer to further decode the list content.
        /// </summary>
        /// <exception cref="DeserializationException">state does not represent a list</exception>
        public ListDeserializer RequireList()
        {
            switch (Kind)
            {
                case ObjectKind.ListOfBits:
                    return new ListOfBitsDeserializer(this, false);

                case ObjectKind.ListOfBytes:
                    return new ListOfPrimitivesDeserializer<byte>(this, ListKind.ListOfBytes);

                case ObjectKind.ListOfEmpty:
                    return new ListOfEmptyDeserializer(this);

                case ObjectKind.ListOfInts:
                    return new ListOfPrimitivesDeserializer<int>(this, ListKind.ListOfInts);

                case ObjectKind.ListOfLongs:
                    return new ListOfPrimitivesDeserializer<long>(this, ListKind.ListOfLongs);

                case ObjectKind.ListOfPointers:
                    return new ListOfPointersDeserializer(this);

                case ObjectKind.ListOfShorts:
                    return new ListOfPrimitivesDeserializer<short>(this, ListKind.ListOfShorts);

                case ObjectKind.ListOfStructs:
                    return new ListOfStructsDeserializer(this);

                case ObjectKind.Nil:
                    return new EmptyListDeserializer();

                default:
                    throw new DeserializationException("Cannot deserialize this object as list");
            }
        }

        /// <summary>
        /// Given this state represents a list of pointers, returns a ListOfCapsDeserializer for decoding it as list of capabilities.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <exception cref="DeserializationException">state does not represent a list of pointers</exception>
        public ListOfCapsDeserializer<T> RequireCapList<T>() where T: class
        {
            switch (Kind)
            {
                case ObjectKind.ListOfPointers:
                    return new ListOfCapsDeserializer<T>(this);

                default:
                    throw new DeserializationException("Cannot deserialize this object as capability list");
            }
        }

        /// <summary>
        /// Convenience method. Given this state represents a struct, decodes text field from its pointer table.
        /// </summary>
        /// <param name="index">index within this struct's pointer table</param>
        /// <param name="defaultText">default text to return of pointer is null</param>
        /// <returns>the decoded text, or defaultText (which might be null)</returns>
        /// <exception cref="IndexOutOfRangeException">negative index</exception>
        /// <exception cref="DeserializationException">state does not represent a struct, invalid pointer,
        /// non-list-of-bytes pointer, traversal limit exceeded</exception>
        [return: NotNullIfNotNull("defaultText")]
        public string? ReadText(int index, string? defaultText = null)
        {
            return StructReadPointer(index).RequireList().CastText() ?? defaultText;
        }

        /// <summary>
        /// Convenience method. Given this state represents a struct, decodes a list deserializer field from its pointer table.
        /// </summary>
        /// <param name="index">index within this struct's pointer table</param>
        /// <returns>the list deserializer instance</returns>
        /// <exception cref="IndexOutOfRangeException">negative index</exception>
        /// <exception cref="DeserializationException">state does not represent a struct, invalid pointer,
        /// non-list pointer, traversal limit exceeded</exception>
        public ListDeserializer ReadList(int index)
        {
            return StructReadPointer(index).RequireList();
        }

        /// <summary>
        /// Convenience method. Given this state represents a struct, decodes a capability list field from its pointer table.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <param name="index">index within this struct's pointer table</param>
        /// <returns>the capability list deserializer instance</returns>
        /// <exception cref="IndexOutOfRangeException">negative index</exception>
        /// <exception cref="DeserializationException">state does not represent a struct, invalid pointer,
        /// non-list-of-pointers pointer, traversal limit exceeded</exception>
        public ListOfCapsDeserializer<T> ReadCapList<T>(int index) where T : class
        {
            return StructReadPointer(index).RequireCapList<T>();
        }

        /// <summary>
        /// Convenience method. Given this state represents a struct, decodes a list of structs field from its pointer table.
        /// </summary>
        /// <typeparam name="T">Struct target representation type</typeparam>
        /// <param name="index">index within this struct's pointer table</param>
        /// <param name="cons">constructs a target representation type instance from the underlying deserializer state</param>
        /// <returns>the decoded list of structs</returns>
        /// <exception cref="IndexOutOfRangeException">negative index</exception>
        /// <exception cref="DeserializationException">state does not represent a struct, invalid pointer,
        /// non-list-of-{structs,pointers} pointer, traversal limit exceeded</exception>
        public IReadOnlyList<T> ReadListOfStructs<T>(int index, Func<DeserializerState, T> cons)
        {
            return ReadList(index).Cast(cons);
        }

        /// <summary>
        /// Convenience method. Given this state represents a struct, decodes a struct field from its pointer table.
        /// </summary>
        /// <typeparam name="T">Struct target representation type</typeparam>
        /// <param name="index">index within this struct's pointer table</param>
        /// <param name="cons">constructs a target representation type instance from the underlying deserializer state</param>
        /// <returns>the decoded struct</returns>
        /// <exception cref="IndexOutOfRangeException">negative index</exception>
        /// <exception cref="DeserializationException">state does not represent a struct, invalid pointer,
        /// non-struct pointer, traversal limit exceeded</exception>
        public T ReadStruct<T>(int index, Func<DeserializerState, T> cons)
        {
            return cons(StructReadPointer(index));
        }

        /// <summary>
        /// Given this state represents a capability, returns its index into the capability table.
        /// </summary>
        public uint CapabilityIndex => Kind == ObjectKind.Capability ? BytesTraversedOrData : ~0u;

        /// <summary>
        /// Given this state represents a struct, decodes a capability field from its pointer table.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <param name="index">index within this struct's pointer table</param>
        /// <param name="memberName">debugging aid</param>
        /// <param name="sourceFilePath">debugging aid</param>
        /// <param name="sourceLineNumber">debugging aid</param>
        /// <returns>capability instance or null if pointer was null</returns>
        /// <exception cref="IndexOutOfRangeException">negative index</exception>
        /// <exception cref="DeserializationException">state does not represent a struct, invalid pointer,
        /// non-capability pointer, traversal limit exceeded</exception>
        public T? ReadCap<T>(int index,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0) where T: class
        {
            var cap = StructReadRawCap(index);
            return Rpc.CapabilityReflection.CreateProxy<T>(cap, memberName, sourceFilePath, sourceLineNumber) as T;
        }

        /// <summary>
        /// Given this state represents a struct, decodes a capability field from its pointer table and
        /// returns it as bare (generic) proxy.
        /// </summary>
        /// <param name="index">index within this struct's pointer table</param>
        /// <returns>capability instance or null if pointer was null</returns>
        /// <exception cref="IndexOutOfRangeException">negative index</exception>
        /// <exception cref="DeserializationException">state does not represent a struct, invalid pointer,
        /// non-capability pointer, traversal limit exceeded</exception>
        public Rpc.BareProxy ReadCap(int index)
        {
            var cap = StructReadRawCap(index);
            return new Rpc.BareProxy(cap);
        }

        /// <summary>
        /// Given this state represents a capability, wraps it into a proxy instance for the desired interface.
        /// </summary>
        /// <typeparam name="T">Capability interface</typeparam>
        /// <returns>capability instance or null if pointer was null</returns>
        /// <exception cref="IndexOutOfRangeException">negative index</exception>
        /// <exception cref="DeserializationException">state does not represent a capability</exception>
        public T? RequireCap<T>() where T: class
        {
            if (Kind == ObjectKind.Nil)
                return null;

            if (Kind != ObjectKind.Capability)
                throw new DeserializationException("Expected a capability");

            if (Caps == null)
                throw new InvalidOperationException("Capability table not set. This is a bug.");

            return (Rpc.CapabilityReflection.CreateProxy<T>(Caps[(int)CapabilityIndex]) as T)!;
        }
    }
}