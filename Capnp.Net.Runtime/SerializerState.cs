using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Capnp
{
    /// <summary>
    /// Implements the heart of serialization. Exposes all functionality to encode serialized data.
    /// Although it is public, you should not use it directly. Instead, use the reader, writer, and domain class adapters which are produced
    /// by the code generator. Particularly, those writer classes are actually specializations of SerializerState, adding convenience methods
    /// for accessing the struct's fields.
    /// </summary>
    public class SerializerState : IStructSerializer, IDisposable
    {
        /// <summary>
        /// Constructs a SerializerState instance for use in RPC context.
        /// This particularly means that the capability table will be initialized.
        /// </summary>
        /// <typeparam name="T">Type of state (must inherit from SerializerState)</typeparam>
        /// <returns>root object state</returns>
        public static T CreateForRpc<T>() where T: SerializerState, new()
        {
            var mb = MessageBuilder.Create();
            mb.InitCapTable();
            var s = new T();
            s.Bind(mb);
            return s;
        }

        internal MessageBuilder? MsgBuilder { get; set; }
        internal ISegmentAllocator? Allocator => MsgBuilder?.Allocator;
        internal List<Rpc.ConsumedCapability>? Caps => MsgBuilder?.Caps;
        internal SerializerState? Owner { get; set; }
        internal int OwnerSlot { get; set; }
        internal uint SegmentIndex { get; set; }
        internal int Offset { get; set; }
        internal uint WordsAllocated { get; set; }
        internal int ListElementCount { get; set; }
        internal ushort StructDataCount { get; set; }
        internal ushort StructPtrCount { get; set; }
        internal uint CapabilityIndex { get; set; }

        SerializerState[]? _linkedStates;
        bool _disposed;

        /// <summary>
        /// Constructs an unbound serializer state.
        /// </summary>
        public SerializerState()
        {
            Offset = -1;
            ListElementCount = -1;
        }

        /// <summary>
        /// Constructs a serializer state which is bound to a particular message builder.
        /// </summary>
        /// <param name="messageBuilder">message builder to bind</param>
        public SerializerState(MessageBuilder messageBuilder)
        {
            MsgBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
            Offset = -1;
            ListElementCount = -1;
        }

        internal void Bind(MessageBuilder messageBuilder)
        {
            MsgBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        }

        internal void Bind(SerializerState owner, int ownerSlot)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            OwnerSlot = ownerSlot;
            MsgBuilder = owner.MsgBuilder;
        }

        internal void InheritFrom(SerializerState other)
        {
            SegmentIndex = other.SegmentIndex;
            Offset = other.Offset;
            WordsAllocated = other.WordsAllocated;
            ListElementCount = other.ListElementCount;
            StructDataCount = other.StructDataCount;
            StructPtrCount = other.StructPtrCount;
            Kind = other.Kind;
            CapabilityIndex = other.CapabilityIndex;
            _linkedStates = other._linkedStates;
        }

        /// <summary>
        /// Represents this state by a different serializer state specialization. This is similar to a type-cast: The underlying object remains the same,
        /// but the specialization adds a particular "view" on that data.
        /// </summary>
        /// <typeparam name="TS">target serializer state type</typeparam>
        /// <returns>serializer state instance</returns>
        /// <exception cref="InvalidOperationException">The target serializer state is incompatible to this instance, because the instances do not agree on the
        /// kind of underlying object (e.g. struct with particular data/pointer section size, list of something)</exception>
        public TS Rewrap<TS>() where TS: SerializerState, new()
        {
            if (this is TS ts)
                return ts;

            ts = new TS();

            if (Kind != ObjectKind.Nil)
            {
                static InvalidOperationException InvalidWrap() =>
                    new InvalidOperationException("Incompatible cast");

                switch (ts.Kind)
                {
                    case ObjectKind.Struct:
                    case ObjectKind.ListOfStructs:
                        if (ts.Kind != Kind ||
                            ts.StructDataCount != StructDataCount ||
                            ts.StructPtrCount != StructPtrCount)
                            throw InvalidWrap();
                        break;

                    case ObjectKind.Nil:
                        break;

                    default:
                        if (ts.Kind != Kind)
                            throw InvalidWrap();
                        break;
                }

                ts.InheritFrom(this);
            }

            if (Owner != null)
                ts.Bind(Owner, OwnerSlot);
            else
                ts.Bind(MsgBuilder ?? throw Unbound());

            return ts;
        }

        /// <summary>
        /// Whether storage space for the underlying object was already allocated. Note that allocation happens
        /// lazily, i.e. constructing a SerializerState and binding it to a MessageBuilder does NOT yet result in allocation.
        /// </summary>
        public bool IsAllocated => Offset >= 0;

        Span<ulong> SegmentSpan => IsAllocated && Allocator != null ? Allocator.Segments[(int)SegmentIndex].Span : Span<ulong>.Empty;
        Span<ulong> FarSpan(uint index) => Allocator!.Segments[(int)index].Span;

        /// <summary>
        /// Given this state describes a struct and is allocated, returns the struct's data section.
        /// </summary>
        public Span<ulong> StructDataSection => SegmentSpan.Slice(Offset, StructDataCount);

        /// <summary>
        /// Returns the allocated memory slice (given this state already is allocated). Note that this definition is somewhat
        /// non-symmetric to <code>DeserializerState.RawData</code>. Never mind: You should not use it directly, anyway.
        /// </summary>
        public Span<ulong> RawData => IsAllocated ? SegmentSpan.Slice(Offset, (int)WordsAllocated) : Span<ulong>.Empty;

        /// <summary>
        /// The kind of object this state currently represents.
        /// </summary>
        public ObjectKind Kind { get; internal set; }

        void AllocateWords(uint count)
        {
            if (count == 0)
            {
                SegmentIndex = 0;
                Offset = 0;
            }
            else
            {
                if (Allocator == null)
                    throw Unbound();

                SegmentIndex = Owner?.SegmentIndex ?? SegmentIndex;
                Allocator.Allocate(count, SegmentIndex, out var slice, false);
                SegmentIndex = slice.SegmentIndex;
                Offset = slice.Offset;
            }

            WordsAllocated = count;
        }

        /// <summary>
        /// Allocates storage for the underlying object. Does nothing if it is already allocated. From the point the object is allocated, its type cannot be changed
        /// anymore (e.g. changing from struct to list, or modifying the struct's section sizes).
        /// </summary>
        public void Allocate()
        {
            if (!IsAllocated)
            {
                switch (Kind)
                {
                    case ObjectKind.ListOfBits:
                        AllocateWords(checked((uint)ListElementCount + 63u) / 64);
                        break;

                    case ObjectKind.ListOfBytes:
                        AllocateWords(checked((uint)ListElementCount + 7u) / 8);
                        break;

                    case ObjectKind.ListOfInts:
                        AllocateWords(checked((uint)ListElementCount + 1u) / 2);
                        break;

                    case ObjectKind.ListOfLongs:
                    case ObjectKind.ListOfPointers:
                        AllocateWords(checked((uint)ListElementCount));
                        break;

                    case ObjectKind.ListOfShorts:
                        AllocateWords(checked((uint)ListElementCount + 3u) / 4);
                        break;

                    case ObjectKind.ListOfStructs:
                        AllocateWords(checked(1u + (uint)ListElementCount * (uint)(StructDataCount + StructPtrCount)));
                        var tag = default(WirePointer);
                        tag.BeginStruct(StructDataCount, StructPtrCount);
                        tag.ListOfStructsElementCount = ListElementCount;
                        SegmentSpan[Offset] = tag;
                        break;

                    case ObjectKind.Struct:
                        AllocateWords((uint)(StructDataCount + StructPtrCount));
                        break;

                    default:
                        AllocateWords(0);
                        break;
                }

                Owner?.Link(OwnerSlot, this);
            }
        }

        internal Rpc.ConsumedCapability DecodeCapPointer(int offset)
        {
            if (Caps == null)
                throw new InvalidOperationException("Capbility table not set");

            if (!IsAllocated)
            {
                return Rpc.NullCapability.Instance;
            }

            WirePointer pointer = RawData[offset];

            if (pointer.IsNull)
            {
                return Rpc.NullCapability.Instance;
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

        void EncodePointer(int offset, SerializerState target, bool allowCopy)
        {
            if (SegmentSpan[offset] != 0)
            {
                throw new InvalidOperationException("Won't replace an already allocated pointer to prevent memory leaks and security flaws");
            }

            if (target.Allocator != null &&
                target.Allocator != Allocator)
            {
                if (allowCopy)
                {
                    Allocate();

                    var targetCopy = new DynamicSerializerState(MsgBuilder!);
                    Reserializing.DeepCopy(target, targetCopy);
                    target = targetCopy;
                }
                else
                {
                    throw new InvalidOperationException("target was allocated on a different segment allocator");
                }
            }
            else
            {
                Allocate();
            }

            WirePointer targetPtr = default;

            switch (target.Kind)
            {
                case ObjectKind.ListOfBits:
                    targetPtr.BeginList(ListKind.ListOfBits, target.ListElementCount);
                    break;

                case ObjectKind.ListOfBytes:
                    targetPtr.BeginList(ListKind.ListOfBytes, target.ListElementCount);
                    break;

                case ObjectKind.ListOfEmpty:
                    targetPtr.BeginList(ListKind.ListOfEmpty, target.ListElementCount);
                    break;

                case ObjectKind.ListOfInts:
                    targetPtr.BeginList(ListKind.ListOfInts, target.ListElementCount);
                    break;

                case ObjectKind.ListOfLongs:
                    targetPtr.BeginList(ListKind.ListOfLongs, target.ListElementCount);
                    break;

                case ObjectKind.ListOfPointers:
                    targetPtr.BeginList(ListKind.ListOfPointers, target.ListElementCount);
                    break;

                case ObjectKind.ListOfShorts:
                    targetPtr.BeginList(ListKind.ListOfShorts, target.ListElementCount);
                    break;

                case ObjectKind.ListOfStructs:
                    int wordCount = target.ListElementCount * (target.StructDataCount + target.StructPtrCount);
                    targetPtr.BeginList(ListKind.ListOfStructs, wordCount);
                    break;

                case ObjectKind.Capability:
                    targetPtr.SetCapability(target.CapabilityIndex);
                    SegmentSpan[offset] = targetPtr;
                    return;

                case ObjectKind.Struct:
                    targetPtr.BeginStruct(target.StructDataCount, target.StructPtrCount);
                    if (target.StructDataCount == 0 && target.StructPtrCount == 0)
                    {
                        targetPtr.Offset = -1;
                        SegmentSpan[offset] = targetPtr;
                        return;
                    }
                    break;

                case ObjectKind.Nil:
                    SegmentSpan[offset] = 0;
                    return;

                default:
                    throw new NotImplementedException();
            }

            if (SegmentIndex != target.SegmentIndex)
            {
                WirePointer farPtr = default;

                if (Allocator!.Allocate(1, target.SegmentIndex, out var landingPadSlice, true))
                {
                    farPtr.SetFarPointer(target.SegmentIndex, landingPadSlice.Offset, false);
                    SegmentSpan[offset] = farPtr;
                    targetPtr.Offset = target.Offset - (landingPadSlice.Offset + 1);
                    FarSpan(target.SegmentIndex)[landingPadSlice.Offset] = targetPtr;
                }
                else
                {
                    Allocator.Allocate(2, 0, out landingPadSlice, false);
                    farPtr.SetFarPointer(landingPadSlice.SegmentIndex, landingPadSlice.Offset, true);
                    SegmentSpan[offset] = farPtr;
                    WirePointer farPtr2 = default;
                    farPtr2.SetFarPointer(target.SegmentIndex, target.Offset, false);
                    var farSpan = FarSpan(landingPadSlice.SegmentIndex);
                    farSpan[landingPadSlice.Offset] = farPtr2;
                    targetPtr.Offset = target.Offset;
                    farSpan[landingPadSlice.Offset + 1] = targetPtr;
                }
            }
            else
            {
                targetPtr.Offset = target.Offset - (offset + 1);
                SegmentSpan[offset] = targetPtr;
            }
        }

        /// <summary>
        /// Links a sub-item (struct field or list element) of this state to another state. Usually, this operation is not necessary, since objects are constructed top-down.
        /// However, there might be some advanced scenarios where you want to reference the same object twice (also interesting for designing amplification attacks).
        /// The Cap'n Proto serialization intrinsically supports this, since messages are object graphs, not trees.
        /// </summary>
        /// <param name="slot">If this state describes a struct: Index into this struct's pointer table. 
        /// If this state describes a list of pointers: List element index.</param>
        /// <param name="target">state to be linked</param>
        /// <param name="allowCopy">Whether to deep copy the target state if it belongs to a different message builder than this state.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="slot"/> out of range</exception>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>This state does neither describe a struct, nor a list of pointers</description></item>
        /// <item><description>Another state is already linked to the specified position (sorry, no overwrite allowed)</description></item>
        /// <item><description>This state and <paramref name="target"/> belong to different message builder, and<paramref name="allowCopy"/> is false</description></item>
        /// </list>
        /// </exception>
        protected void Link(int slot, SerializerState target, bool allowCopy = true)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (slot < 0)
                throw new ArgumentOutOfRangeException(nameof(slot));

            if (!IsAllocated)
            {
                SegmentIndex = target.SegmentIndex;
                Allocate();
            }

            if (!target.IsAllocated)
            {
                target.Allocate();
            }

            switch (Kind)
            {
                case ObjectKind.Struct:
                    if (slot >= StructPtrCount)
                        throw new ArgumentOutOfRangeException(nameof(slot));

                    EncodePointer(Offset + StructDataCount + slot, target, allowCopy);

                    break;

                case ObjectKind.ListOfPointers:
                    if (slot >= ListElementCount)
                        throw new ArgumentOutOfRangeException(nameof(slot));

                    EncodePointer(Offset + slot, target, allowCopy);

                    break;

                default:
                    throw new InvalidOperationException("This object cannot own pointers to sub-objects");
            }

            _linkedStates![slot] = target;
        }

        /// <summary>
        /// Links a sub-item (struct field or list element) of this state to a capability.
        /// </summary>
        /// <param name="slot">If this state describes a struct: Index into this struct's pointer table. 
        /// If this state describes a list of pointers: List element index.</param>
        /// <param name="capabilityIndex">capability index inside the capability table</param>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>This state does neither describe a struct, nor a list of pointers</description></item>
        /// <item><description>Another state is already linked to the specified position (sorry, no overwrite allowed)</description></item></list>
        /// </exception>
        protected void LinkToCapability(int slot, uint? capabilityIndex)
        {
            var cstate = new SerializerState();
            cstate.SetCapability(capabilityIndex);
            Link(slot, cstate);
        }

        static InvalidOperationException AlreadySet() => new InvalidOperationException("The object type was already set");
        static InvalidOperationException Unbound() => new InvalidOperationException("This state is not bound to a MessageBuilder");

        void VerifyNotYetAllocated()
        {
            if (IsAllocated)
                throw new InvalidOperationException("Not permitted since the state is already allocated");
        }

        /// <summary>
        /// Determines the underlying object to be a struct.
        /// </summary>
        /// <param name="dataCount">Desired size of the struct's data section, in words</param>
        /// <param name="ptrCount">Desired size of the struct's pointer section, in words</param>
        /// <exception cref="InvalidOperationException">The object type was already set to something different</exception>
        protected void SetStruct(ushort dataCount, ushort ptrCount)
        {
            if (Kind == ObjectKind.Nil)
            {
                VerifyNotYetAllocated();

                Kind = ObjectKind.Struct;
                StructDataCount = dataCount;
                StructPtrCount = ptrCount;

                _linkedStates = new SerializerState[ptrCount];
            }
            else if (Kind != ObjectKind.Struct || StructDataCount != dataCount || StructPtrCount != ptrCount)
            {
                throw AlreadySet();
            }
        }

        protected void SetCapability(uint? capabilityIndex)
        {
            if (capabilityIndex.HasValue)
            {
                if (Kind == ObjectKind.Nil)
                {
                    VerifyNotYetAllocated();

                    Kind = ObjectKind.Capability;
                    CapabilityIndex = capabilityIndex.Value;
                    Allocate();
                }
                else if (Kind != ObjectKind.Capability || CapabilityIndex != capabilityIndex)
                {
                    throw AlreadySet();
                }
            }
            else
            {
                if (Kind != ObjectKind.Nil)
                {
                    throw AlreadySet();
                }
            }
        }

        /// <summary>
        /// Determines the underlying object to be a list of (primitive) values.
        /// </summary>
        /// <param name="bitsPerElement">Element size in bits, must be 0 (void), 1 (bool), 8, 16, 32, or 64</param>
        /// <param name="totalCount">Desired element count</param>
        /// <exception cref="InvalidOperationException">The object type was already set to something different</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitsPerElement"/> outside allowed range, 
        /// <paramref name="totalCount"/> negative or exceeding 2^29-1</exception>
        protected void SetListOfValues(byte bitsPerElement, int totalCount)
        {
            var kind = bitsPerElement switch
            {
                0 => ObjectKind.ListOfEmpty,
                1 => ObjectKind.ListOfBits,
                8 => ObjectKind.ListOfBytes,
                16 => ObjectKind.ListOfShorts,
                32 => ObjectKind.ListOfInts,
                64 => ObjectKind.ListOfLongs,
                _ => throw new ArgumentOutOfRangeException(nameof(bitsPerElement)),
            };
            if (Kind == ObjectKind.Nil)
            {
                if (totalCount < 0)
                    throw new ArgumentOutOfRangeException(nameof(totalCount));

                VerifyNotYetAllocated();

                Kind = kind;
                ListElementCount = totalCount;

                Allocate();
            }
            else if (Kind != kind || ListElementCount != totalCount)
            {
                throw AlreadySet();
            }
        }

        /// <summary>
        /// Determines the underlying object to be a list of pointers.
        /// </summary>
        /// <param name="totalCount">Desired element count</param>
        /// <exception cref="InvalidOperationException">The object type was already set to something different</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="totalCount"/> negative or exceeding 2^29-1</exception>
        protected void SetListOfPointers(int totalCount)
        {
            if (Kind == ObjectKind.Nil)
            {
                if (totalCount < 0)
                    throw new ArgumentOutOfRangeException(nameof(totalCount));

                VerifyNotYetAllocated();

                Kind = ObjectKind.ListOfPointers;
                ListElementCount = totalCount;

                _linkedStates = new SerializerState[totalCount];

                Allocate();
            }
            else if (Kind != ObjectKind.ListOfPointers || ListElementCount != totalCount)
            {
                throw AlreadySet();
            }
        }

        /// <summary>
        /// Determines the underlying object to be a list of structs (fixed-width compound list).
        /// </summary>
        /// <param name="totalCount">Desired element count</param>
        /// <param name="dataCount">Desired size of each struct's data section, in words</param>
        /// <param name="ptrCount">Desired size of each struct's pointer section, in words</param>
        /// <exception cref="InvalidOperationException">The object type was already set to something different</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="totalCount"/> negative, or total word count would exceed 2^29-1</exception>
        protected void SetListOfStructs(int totalCount, ushort dataCount, ushort ptrCount)
        {
            if (Kind == ObjectKind.Nil)
            {
                if (totalCount < 0)
                    throw new ArgumentOutOfRangeException(nameof(totalCount));

                VerifyNotYetAllocated();

                Kind = ObjectKind.ListOfStructs;
                ListElementCount = totalCount;
                StructDataCount = dataCount;
                StructPtrCount = ptrCount;

                _linkedStates = new SerializerState[totalCount];

                Allocate();
            }
            else if (Kind != ObjectKind.ListOfStructs || 
                ListElementCount != totalCount ||
                StructDataCount != dataCount ||
                StructPtrCount != ptrCount)
            {
                throw AlreadySet();
            }
        }

        /// <summary>
        /// Determines the underlying object to be a list of bytes and encodes the given text.
        /// </summary>
        /// <param name="text">text to encode</param>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is null</exception>
        /// <exception cref="EncoderFallbackException">Trying to obtain the UTF-8 encoding might throw this exception.</exception>
        /// <exception cref="InvalidOperationException">The object type was already set to something different</exception>
        /// <exception cref="ArgumentOutOfRangeException">UTF-8 encoding exceeds 2^29-2 bytes</exception>
        protected void WriteText(string? text)
        {
            if (text == null)
            {
                VerifyNotYetAllocated();
            }
            else
            {
                byte[] srcBytes = Encoding.UTF8.GetBytes(text);
                SetListOfValues(8, srcBytes.Length + 1);
                var srcSpan = new ReadOnlySpan<byte>(srcBytes);
                var dstSpan = ListGetBytes();
                dstSpan = dstSpan.Slice(0, dstSpan.Length - 1);
                srcSpan.CopyTo(dstSpan);
            }
        }

        /// <summary>
        /// Writes data (up to 64 bits) into the underlying struct's data section. 
        /// The write operation must be aligned to fit within a single word.
        /// </summary>
        /// <param name="bitOffset">Start bit relative to the struct's data section, little endian</param>
        /// <param name="bitCount">Number of bits to write</param>
        /// <param name="value">Data bits to write</param>
        /// <exception cref="InvalidOperationException">The object was not determined to be a struct</exception>
        /// <exception cref="ArgumentOutOfRangeException">The data slice specified by <paramref name="bitOffset"/> and <paramref name="bitCount"/>
        /// is not completely within the struct's data section, misaligned, exceeds one word, or <paramref name="bitCount"/> is negative</exception>
        public void StructWriteData(ulong bitOffset, int bitCount, ulong value)
        {
            if (Kind != ObjectKind.Struct)
                throw new InvalidOperationException("This is not a struct");

            Allocate();

            int index = checked((int)(bitOffset / 64));
            int relBitOffset = (int)(bitOffset % 64);

            var data = StructDataSection;

            if (relBitOffset + bitCount > 64)
                throw new ArgumentOutOfRangeException(nameof(bitCount));

            if (bitCount == 64)
            {
                data[index] = value;
            }
            else
            {
                ulong mask = ~(((1ul << bitCount) - 1) << relBitOffset);
                data[index] = data[index] & mask | (value << relBitOffset);
            }
        }

        /// <summary>
        /// Reads data (up to 64 bits) from the underlying struct's data section. 
        /// The write operation must be aligned to fit within a single word.
        /// </summary>
        /// <param name="bitOffset">Start bit relative to the struct's data section, little endian</param>
        /// <param name="count">Number of bits to read</param>
        /// <returns>Data bits which were read</returns>
        /// <exception cref="InvalidOperationException">The object was not determined to be a struct</exception>
        /// <exception cref="ArgumentOutOfRangeException">The data slice specified by <paramref name="bitOffset"/> and <paramref name="count"/>
        /// is not completely within the struct's data section, misaligned, exceeds one word, or <paramref name="count"/> is negative</exception>
        public ulong StructReadData(ulong bitOffset, int count)
        {
            if (Kind != ObjectKind.Struct)
                throw new InvalidOperationException("This is not a struct");

            if (!IsAllocated)
                return 0;

            int index = checked((int)(bitOffset / 64));
            int relBitOffset = (int)(bitOffset % 64);

            var data = StructDataSection;

            if (index >= data.Length)
                return 0; // Assume backwards-compatible change

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (relBitOffset + count > 64)
                throw new ArgumentOutOfRangeException(nameof(count));

            ulong word = data[index];

            if (count == 64)
            {
                return word;
            }
            else
            {
                ulong mask = (1ul << count) - 1;
                return (word >> relBitOffset) & mask;
            }
        }

        /// <summary>
        /// Constructs a new object at a struct field or list element, or returns the serializer state for an existing object.
        /// </summary>
        /// <typeparam name="TS">Target state type</typeparam>
        /// <param name="index">If the underlying object is a struct: index into the struct's pointer section.
        /// If the underlying object is a list of pointers: Element index</param>
        /// <returns>Bound serializer state instance</returns>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>The underlying object was not determined to be a struct or list of pointers.</description></item>
        /// <item><description>Object at given position was already built and is not compatible with the desired target serializer type.</description></item>
        /// </list></exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public TS BuildPointer<TS>(int index) where TS: SerializerState, new()
        {
            if (Kind != ObjectKind.Struct && Kind != ObjectKind.ListOfPointers)
                throw new InvalidOperationException("This is not a struct or list of pointers");

            ref var state = ref _linkedStates![index];

            if (state == null)
            {
                state = new TS();
                state.Bind(this, index);
            }

            return state.Rewrap<TS>();
        }

        /// <summary>
        /// Returns an existing serializer state for a struct field or list element, or null if no such object exists.
        /// </summary>
        /// <typeparam name="TS">Target state type</typeparam>
        /// <param name="index">If the underlying object is a struct: index into the struct's pointer section.
        /// If the underlying object is a list of pointers: Element index</param>
        /// <returns>Bound serializer state instance</returns>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>The underlying object was not determined to be a struct or list of pointers.</description></item>
        /// <item><description>Object at given position is not compatible with the desired target serializer type.</description></item>
        /// </list></exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public TS? TryGetPointer<TS>(int index) where TS : SerializerState, new()
        {
            if (Kind != ObjectKind.Struct && Kind != ObjectKind.ListOfPointers)
                throw new InvalidOperationException("This is not a struct or list of pointers");

            if (index < 0 || index >= _linkedStates!.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            var state = _linkedStates![index];

            if (state == null) return null;

            return state.Rewrap<TS>();
        }

        /// <summary>
        /// Convenience method for <code><![CDATA[BuildPointer<DynamicSerializerState>]]></code>
        /// </summary>
        /// <param name="index">If the underlying object is a struct: index into the struct's pointer section.
        /// If the underlying object is a list of pointers: Element index</param>
        /// <returns>Bound serializer state instance</returns>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>The underlying object was not determined to be a struct or list of pointers.</description></item>
        /// <item><description>Object at given position was already built and is not compatible with the desired target serializer type.</description></item>
        /// </list></exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public DynamicSerializerState BuildPointer(int index) => BuildPointer<DynamicSerializerState>(index);

        /// <summary>
        /// Convenience method for <code><![CDATA[TryGetPointer<SerializerState>]]></code>
        /// </summary>
        /// <param name="index">If the underlying object is a struct: index into the struct's pointer section.
        /// If the underlying object is a list of pointers: Element index</param>
        /// <returns>Bound serializer state instance</returns>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>The underlying object was not determined to be a struct or list of pointers.</description></item>
        /// <item><description>Object at given position is not compatible with the desired target serializer type.</description></item>
        /// </list></exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public SerializerState? TryGetPointer(int index) => TryGetPointer<SerializerState>(index);

        /// <summary>
        /// Reads text from a struct field or list element.
        /// </summary>
        /// <param name="index">If the underlying object is a struct: index into the struct's pointer section.
        /// If the underlying object is a list of pointers: Element index</param>
        /// <param name="defaultText">String to return in case of null</param>
        /// <returns>The decoded text</returns>
        public string? ReadText(int index, string? defaultText = null)
        {
            var b = BuildPointer(index);

            if (b.IsAllocated)
                return b.ListReadAsText();
            else
                return defaultText;
        }

        /// <summary>
        /// Encodes text into a struct field or list element.
        /// </summary>
        /// <param name="index">If the underlying object is a struct: index into the struct's pointer section.
        /// If the underlying object is a list of pointers: Element index</param>
        /// <param name="text">Text to encode</param>
        /// <exception cref="ArgumentNullException"><paramref name="text"/>is null</exception>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>The underlying object was not determined to be a struct or list of pointers.</description></item>
        /// <item><description>Object at given position was already set.</description></item>
        /// </list></exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void WriteText(int index, string? text)
        {
            BuildPointer(index).WriteText(text);
        }

        /// <summary>
        /// Encodes text into a struct field or list element, with fallback to a default text.
        /// </summary>
        /// <param name="index">If the underlying object is a struct: index into the struct's pointer section.
        /// If the underlying object is a list of pointers: Element index</param>
        /// <param name="text">Text to encode</param>
        /// <param name="defaultText">Default text if <paramref name="text"/>> is null</param>
        /// <exception cref="ArgumentNullException">Both <paramref name="text"/> and <paramref name="defaultText"/> are null</exception>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>The underlying object was not determined to be a struct or list of pointers.</description></item>
        /// <item><description>Object at given position was already set.</description></item>
        /// </list></exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void WriteText(int index, string? text, string? defaultText)
        {
            BuildPointer(index).WriteText(text ?? defaultText);
        }

        /// <summary>
        /// Returns a state which represents a fixed-width composite list element.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Bound serializer state</returns>
        /// <exception cref="InvalidOperationException">Underlying object was not determined to be a fixed-width composite list.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/>is out of bounds.</exception>
        public SerializerState ListBuildStruct(int index)
        {
            if (Kind != ObjectKind.ListOfStructs)
                throw new InvalidOperationException("This is not a list of structs");

            if (index < 0 || index >= _linkedStates!.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            ref var state = ref _linkedStates![index];

            if (state == null)
            {
                state = new SerializerState(MsgBuilder!);
                state.SetStruct(StructDataCount, StructPtrCount);
                state.SegmentIndex = SegmentIndex;
                state.Offset = Offset + 1 + index * (StructDataCount + StructPtrCount);
            }

            return _linkedStates[index];
        }

        /// <summary>
        /// Returns a state which represents a fixed-width composite list element.
        /// </summary>
        /// <typeparam name="TS">Target serializer state type</typeparam>
        /// <param name="index"></param>
        /// <returns>Bound serializer state</returns>
        /// <exception cref="InvalidOperationException">Underlying object was not determined to be a fixed-width composite list.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/>is out of bounds.</exception>
        public TS ListBuildStruct<TS>(int index)
            where TS: SerializerState, new()
        {
            if (Kind != ObjectKind.ListOfStructs)
                throw new InvalidOperationException("This is not a list of structs");

            if (index < 0 || index >= _linkedStates!.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            ref var state = ref _linkedStates![index];

            if (state == null)
            {
                state = new TS();
                state.Bind(MsgBuilder!);
                state.SetStruct(StructDataCount, StructPtrCount);
                state.SegmentIndex = SegmentIndex;
                int stride = StructDataCount + StructPtrCount;
                state.Offset = Offset + stride * index + 1;
            }

            return (TS)state;
        }

        internal IReadOnlyList<TS> ListEnumerateStructs<TS>()
            where TS: SerializerState, new()
        {
            if (Kind != ObjectKind.ListOfStructs)
                throw new InvalidOperationException("This is not a list of structs");

            if (ListElementCount < 0)
                throw new InvalidOperationException("Define element count first");

            int minOffset = Offset + 1;
            int maxOffset = minOffset + ListElementCount;

            for (int offset = minOffset; offset < maxOffset; offset++)
            {
                ref var state = ref _linkedStates![offset - minOffset];

                if (state == null)
                {
                    state = new TS();
                    state.Bind(MsgBuilder!);
                    state.SetStruct(StructDataCount, StructPtrCount);
                    state.SegmentIndex = SegmentIndex;
                    state.Offset = offset;
                }
            }

            return _linkedStates!.LazyListSelect(ts => (TS)ts);
        }

        /// <summary>
        /// Sets an element of a list of bits.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of bits.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, bool value, bool defaultValue = false)
        {
            if (Kind != ObjectKind.ListOfBits)
                throw new InvalidOperationException("This is not a list of bits");

            if (index < 0 || index >= ListElementCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            bool bit = value != defaultValue;
            int wordIndex = index / 64;
            int relBitOffset = index % 64;

            if (bit)
                SegmentSpan[Offset + wordIndex] |= (1ul << relBitOffset);
            else
                SegmentSpan[Offset + wordIndex] &= ~(1ul << relBitOffset);
        }

        /// <summary>
        /// Sets the list-of-bits' content.
        /// </summary>
        /// <param name="values">Content to set</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of bits.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The given element count does not match the underlying list's element count.</exception>
        public void ListWriteValues(IReadOnlyList<bool> values, bool defaultValue = false)
        {
            if (Kind != ObjectKind.ListOfBits)
                throw new InvalidOperationException("This is not a list of bits");

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Count != ListElementCount)
                throw new ArgumentOutOfRangeException(nameof(values));

            int i, w = Offset;
            ulong v;

            for (i = 0; i < ListElementCount - 63; i += 64)
            {
                v = 0;

                for (int j = 63; j >= 0; j--)
                {
                    v <<= 1;
                    if (values[i + j]) v |= 1;
                }

                if (defaultValue) v = ~v;

                SegmentSpan[w++] = v;
            }

            if (i < ListElementCount)
            {
                v = 0;

                for (int k = ListElementCount - 1; k >= i; k--)
                {
                    v <<= 1;
                    if (values[k]) v |= 1;
                }

                if (defaultValue) v = ~v;

                SegmentSpan[w] = v;
            }
        }

        /// <summary>
        /// Sets an element of a list of bytes.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of bytes.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, byte value, byte defaultValue = 0)
        {
            if (Kind != ObjectKind.ListOfBytes)
                throw new InvalidOperationException("This is not a list of bytes");

            if (index < 0 || index >= ListElementCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            byte x = (byte)(value ^ defaultValue);
            MemoryMarshal.Cast<ulong, byte>(SegmentSpan.Slice(Offset))[index] = x;
        }

        /// <summary>
        /// Returns the content of a list of bytes.
        /// </summary>
        /// <returns>The list bytes</returns>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of bytes.</exception>
        public Span<byte> ListGetBytes()
        {
            if (Kind != ObjectKind.ListOfBytes)
                throw new InvalidOperationException("This is not a list of bytes");

            return MemoryMarshal.Cast<ulong, byte>(SegmentSpan.Slice(Offset)).Slice(0, ListElementCount);
        }

        /// <summary>
        /// Decodes a list of bytes as text.
        /// </summary>
        /// <returns>The decoded text.</returns>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of bytes.</exception>
        /// <exception cref="DecoderFallbackException">Might theoretically be thrown during decoding.</exception>
        public string ListReadAsText()
        {
            var bytes = ListGetBytes();
            if (bytes.Length == 0) return string.Empty;
#if NETSTANDARD2_0
            return Encoding.UTF8.GetString(bytes.Slice(0, bytes.Length - 1).ToArray());
#else
            return Encoding.UTF8.GetString(bytes.Slice(0, bytes.Length - 1));
#endif
        }

        /// <summary>
        /// Sets an element of a list of (signed) bytes.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of bytes.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, sbyte value, sbyte defaultValue = 0)
        {
            ListWriteValue(index, unchecked((byte)value), unchecked((byte)defaultValue));
        }

        /// <summary>
        /// Sets an element of a list of 16 bit words.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of 16 bit words.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, ushort value, ushort defaultValue = 0)
        {
            if (Kind != ObjectKind.ListOfShorts)
                throw new InvalidOperationException("This is not a list of 16-bit values");

            if (index < 0 || index >= ListElementCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            Allocate();

            ushort x = (ushort)(value ^ defaultValue);
            MemoryMarshal.Cast<ulong, ushort>(SegmentSpan.Slice(Offset))[index] = x;
        }

        /// <summary>
        /// Sets an element of a list of 16 bit words.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of 16 bit words.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, short value, short defaultValue = 0)
        {
            ListWriteValue(index, unchecked((ushort)value), unchecked((ushort)defaultValue));
        }

        /// <summary>
        /// Sets an element of a list of 32 bit words.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of 32 bit words.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, uint value, uint defaultValue = 0)
        {
            if (Kind != ObjectKind.ListOfInts)
                throw new InvalidOperationException("This is not a list of 32-bit values");

            if (index < 0 || index >= ListElementCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            Allocate();

            uint x = value ^ defaultValue;
            MemoryMarshal.Cast<ulong, uint>(SegmentSpan.Slice(Offset))[index] = x;
        }

        /// <summary>
        /// Sets an element of a list of 32 bit words.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of 32 bit words.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, int value, int defaultValue = 0)
        {
            ListWriteValue(index, unchecked((uint)value), unchecked((uint)defaultValue));
        }

        /// <summary>
        /// Sets an element of a list of 32 bit words.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of 32 bit words.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, float value, float defaultValue = 0)
        {
            int rcastValue = value.ReplacementSingleToInt32Bits();
            int rcastDefaultValue = defaultValue.ReplacementSingleToInt32Bits();
            ListWriteValue(index, rcastValue, rcastDefaultValue);
        }

        /// <summary>
        /// Sets an element of a list of 64 bit words.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of 64 bit words.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, ulong value, ulong defaultValue = 0)
        {
            if (Kind != ObjectKind.ListOfLongs)
                throw new InvalidOperationException("This is not a list of 64-bit values");

            if (index < 0 || index >= ListElementCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            Allocate();

            ulong x = value ^ defaultValue;
            SegmentSpan.Slice(Offset)[index] = x;
        }

        /// <summary>
        /// Sets an element of a list of 64 bit words.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of 64 bit words.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, long value, long defaultValue = 0)
        {
            ListWriteValue(index, unchecked((ulong)value), unchecked((ulong)defaultValue));
        }

        /// <summary>
        /// Sets an element of a list of 64 bit words.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <param name="value">Element value</param>
        /// <param name="defaultValue">Element default value (serialized value will be XORed with the default value)</param>
        /// <exception cref="InvalidOperationException">The underlying object was not set to a list of 64 bit words.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
        public void ListWriteValue(int index, double value, double defaultValue = 0)
        {
            long rcastValue = BitConverter.DoubleToInt64Bits(value);
            long rcastDefaultValue = BitConverter.DoubleToInt64Bits(defaultValue);
            ListWriteValue(index, rcastValue, rcastDefaultValue);
        }

        /// <summary>
        /// Adds an entry to the capability table if the provided capability does not yet exist.
        /// </summary>
        /// <param name="capability">The low-level capability object to provide.</param>
        /// <returns>Index of the given capability in the capability table, null if capability is null</returns>
        /// <exception cref="InvalidOperationException">The underlying message builder was not configured for capability table support.</exception>
        public uint? ProvideCapability(Rpc.ConsumedCapability capability)
        {
            if (capability == null || capability == Rpc.NullCapability.Instance)
                return null;

            if (Caps == null)
                throw new InvalidOperationException("Underlying MessageBuilder was not enabled to support capabilities");

            int index = Caps.IndexOf(capability);

            if (index < 0)
            {
                index = Caps.Count;
                Caps.Add(capability);
                capability.AddRef();
            }

            return (uint)index;
        }

        /// <summary>
        /// Adds an entry to the capability table if the provided capability does not yet exist.
        /// </summary>
        /// <param name="capability">The capability to provide, in terms of its skeleton.</param>
        /// <returns>Index of the given capability in the capability table</returns>
        /// <exception cref="InvalidOperationException">The underlying message builder was not configured for capability table support.</exception>
        public uint? ProvideCapability(Rpc.Skeleton capability)
        {
            return ProvideCapability(capability.AsCapability());
        }

        /// <summary>
        /// Adds an entry to the capability table if the provided capability does not yet exist.
        /// </summary>
        /// <param name="obj">The capability, in one of the following forms:<list type="bullet">
        /// <item><description>Low-level capability (<see cref="Rpc.ConsumedCapability"/>)</description></item>
        /// <item><description>Proxy object (<see cref="Rpc.Proxy"/>). Note that the provision has "move semantics": SerializerState
        /// takes ownership, so the Proxy object will be disposed.</description></item>
        /// <item><description><see cref="Rpc.Skeleton"/> instance</description></item>
        /// <item><description>Capability interface implementation</description></item>
        /// </list></param>
        /// <returns>Index of the given capability in the capability table</returns>
        /// <exception cref="InvalidOperationException">The underlying message builder was not configured for capability table support.</exception>
        public uint? ProvideCapability(object? obj)
        {
            switch (obj)
            {
                case null:
                    return null;
                case Rpc.Proxy proxy: using (proxy)
                    return ProvideCapability(proxy.ConsumedCap);
                case Rpc.ConsumedCapability consumedCapability:
                    return ProvideCapability(consumedCapability);
                case Rpc.Skeleton providedCapability:
                    return ProvideCapability(providedCapability);
                default:
                    return ProvideCapability(Rpc.CapabilityReflection.CreateSkeletonInternal(obj));
            }
        }

        /// <summary>
        /// Links a sub-item (struct field or list element) of this state to another object. 
        /// In contrast to <see cref="Link(int, SerializerState, bool)"/>, this method also accepts deserializer states, domain objects, capabilites, and lists thereof.
        /// If necessary, it will perform a deep copy.
        /// </summary>
        /// <param name="slot">If this state describes a struct: Index into this struct's pointer table. 
        /// If this state describes a list of pointers: List element index.</param>
        /// <param name="obj">Object to be linked. Must be one of the following:<list type="bullet">
        /// <item><description>Another <see cref="SerializerState"/></description></item>
        /// <item><description>A <see cref="DeserializerState"/> (will always deep copy)</description></item>
        /// <item><description>An object implementing <see cref="ICapnpSerializable"/></description></item>
        /// <item><description>A low-level capability object (<see cref="Rpc.ConsumedCapability"/>)</description></item>
        /// <item><description>A proxy object (<see cref="Rpc.Proxy"/>)</description></item>
        /// <item><description>A skeleton object (<see cref="Rpc.Skeleton"/>)</description></item>
        /// <item><description>A capability interface implementation</description></item>
        /// <item><description>A <see cref="IReadOnlyList{T}"/> of one of the things listed here.</description></item>
        /// </list></param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="slot"/> is out of range.</exception>
        /// <exception cref="InvalidOperationException"><list type="bullet">
        /// <item><description>This state does neither describe a struct, nor a list of pointers</description></item>
        /// <item><description>Another state is already linked to the specified position (sorry, no overwrite allowed)</description></item></list>
        /// </exception>
        public void LinkObject<T>(int slot, T obj)
        {
            switch (obj)
            {
                case SerializerState s:
                    Link(slot, s);
                    break;

                case DeserializerState d:
                    Reserializing.DeepCopy(d, BuildPointer(slot));
                    break;

                case ICapnpSerializable serializable:
                    serializable.Serialize(BuildPointer(slot));
                    break;

                case IReadOnlyList<object> list:
                    {
                        var builder = BuildPointer(slot);
                        builder.SetListOfPointers(list.Count);
                        int i = 0;
                        foreach (var item in list)
                        {
                            builder.LinkObject(i++, item);
                        }
                    }
                    break;

                default:
                    if (Rpc.CapabilityReflection.IsValidCapabilityInterface(typeof(T)))
                    {
                        LinkToCapability(slot, ProvideCapability(obj));
                    }
                    break;
            }
        }

        internal Rpc.ConsumedCapability StructReadRawCap(int index)
        {
            if (Kind != ObjectKind.Struct && Kind != ObjectKind.Nil)
                throw new InvalidOperationException("Allowed on structs only");

            if (index < 0 || index >= StructPtrCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            return DecodeCapPointer(index + StructDataCount);
        }

        /// <summary>
        /// Reads a struct field as capability and returns a proxy to that capability. 
        /// </summary>
        /// <typeparam name="T">Desired capability interface</typeparam>
        /// <param name="slot">Index into this struct's pointer table.</param>
        /// <returns>The proxy instance</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="slot"/> is out of range.</exception>
        /// <exception cref="ArgumentException">The desired interface does not qualify as capability interface (<see cref="Rpc.ProxyAttribute"/>)</exception>
        /// <exception cref="InvalidOperationException">This state does not represent a struct.</exception>
        public T ReadCap<T>(int slot) where T : class
        {
            var cap = StructReadRawCap(slot);
            return (Rpc.CapabilityReflection.CreateProxy<T>(cap) as T)!;
        }

        /// <summary>
        /// Reads a struct field as capability and returns a bare (generic) proxy to that capability. 
        /// </summary>
        /// <param name="slot">Index into this struct's pointer table.</param>
        /// <returns>The proxy instance</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="slot"/> is out of range.</exception>
        /// <exception cref="InvalidOperationException">This state does not represent a struct.</exception>
        public Rpc.BareProxy ReadCap(int slot)
        {
            var cap = StructReadRawCap(slot);
            return new Rpc.BareProxy(cap);
        }

        /// <summary>
        /// Releases the capability table
        /// </summary>
        public void Dispose()
        {
            if (Caps != null && !_disposed)
            {
                foreach (var cap in Caps)
                {
                    cap.Release();
                }

                Caps.Clear();
                _disposed = true;
            }
        }
    }
}