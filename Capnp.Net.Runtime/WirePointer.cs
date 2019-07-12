using System;
using System.Collections.Generic;
using System.Text;

namespace Capnp
{
    /// <summary>
    /// Pointer tag, see https://capnproto.org/encoding.html/>
    /// </summary>
    public enum PointerKind : byte
    {
        /// <summary>
        /// Struct pointer
        /// </summary>
        Struct = 0,

        /// <summary>
        /// List pointer
        /// </summary>
        List = 1,

        /// <summary>
        /// Far pointer
        /// </summary>
        Far = 2,

        /// <summary>
        /// Other (capability) pointer
        /// </summary>
        Other = 3
    }

    /// <summary>
    /// Lightweight wrapper struct around a Cap'n Proto pointer. Useful for both encoding and decoding pointers.
    /// </summary>
    public struct WirePointer
    {
        ulong _ptrData;

        /// <summary>
        /// Constructs this struct from pointer raw data
        /// </summary>
        public WirePointer(ulong ptrData)
        {
            _ptrData = ptrData;
        }

        /// <summary>
        /// Interprets any ulong value as Cap'n Proto pointer
        /// </summary>
        public static implicit operator WirePointer (ulong value) => new WirePointer(value);

        /// <summary>
        /// Extracts the wire data from the pointer.
        /// </summary>
        public static implicit operator ulong (WirePointer pointer) => pointer._ptrData;

        /// <summary>
        /// Pointer tag "A"
        /// </summary>
        public PointerKind Kind
        {
            get => (PointerKind)(_ptrData & 3);
            set { _ptrData = _ptrData & ~3ul | (ulong)value; }
        }

        /// <summary>
        /// Returns true iff this is a null pointer.
        /// </summary>
        public bool IsNull => _ptrData == 0;

        /// <summary>
        /// The Offset (field "B") for struct and list pointers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown by setter if encoded value would require more than 30 bits</exception>
        public int Offset
        {
            get => unchecked((int)_ptrData) >> 2;
            set
            {
                if (value >= (1 << 29) || value < -(1 << 29))
                    throw new ArgumentOutOfRangeException(nameof(value));

                _ptrData |= (uint)(value << 2);
            }
        }

        /// <summary>
        /// Returns the landing pad offset (field "C") for inter-segment pointers.
        /// </summary>
        public int LandingPadOffset
        {
            get => unchecked((int)((_ptrData >> 3) & 0x1fffffff));
        }

        /// <summary>
        /// Returns the size of the struct's data section (field "C"), in words, for struct pointers.
        /// </summary>
        public ushort StructDataCount
        {
            get => unchecked((ushort)(_ptrData >> 32));
        }

        /// <summary>
        /// Returns the size of the struct's pointer section (field "D"), in words, for struct pointers.
        /// </summary>
        public ushort StructPtrCount
        {
            get => unchecked((ushort)(_ptrData >> 48));
        }

        /// <summary>
        /// Convenience getter which returns the sum of the struct's pointer and data section sizes.
        /// </summary>
        public uint StructSize
        {
            get => (uint)StructDataCount + StructPtrCount;
        }

        /// <summary>
        /// Begins encoding a struct pointer.
        /// </summary>
        /// <param name="dataCount">the size of the struct's data section, in words</param>
        /// <param name="ptrCount">the size of the struct's pointer section, in words</param>
        public void BeginStruct(ushort dataCount, ushort ptrCount)
        {
            _ptrData = ((ulong)dataCount << 32) | ((ulong)ptrCount << 48);
        }

        /// <summary>
        /// Returns the list "size" (field "C") for list pointers.
        /// </summary>
        public ListKind ListKind
        {
            get => (ListKind)((int)(_ptrData >> 32) & 0x7);
        }

        /// <summary>
        /// Gets or sets the element count if this pointer represents a list of fixed-width composite values.
        /// </summary>
        /// <exception cref="OverflowException">negative value, or encoded value would require more than 30 bits</exception>
        public int ListOfStructsElementCount
        {
            get => (int)((_ptrData >> 2) & 0x3fffffff);
            set { _ptrData = _ptrData & 0xffffffff00000000ul | checked((uint)value << 2); }
        }

        /// <summary>
        /// Returns the element count if this pointer represents a list of anything, except fixed-width composite values.
        /// </summary>
        public int ListElementCount
        {
            get => (int)(_ptrData >> 35);
        }

        /// <summary>
        /// Begins encoding a list pointer
        /// </summary>
        /// <param name="kind">element "size" (field "C")</param>
        /// <param name="count">element count</param>
        /// <exception cref="ArgumentOutOfRangeException">element count would require more than 29 bits</exception>
        public void BeginList(ListKind kind, int count)
        {
            if (count < 0 || count >= (1 << 29))
                throw new ArgumentOutOfRangeException(nameof(count));

            _ptrData = ((ulong)count << 35) | ((ulong)kind << 32) | (ulong)PointerKind.List;
        }

        /// <summary>
        /// Returns the target segment index (field "D") for inter-segment pointers.
        /// </summary>
        public uint TargetSegmentIndex
        {
            get => (uint)(_ptrData >> 32);
        }

        /// <summary>
        /// Whether the landing pad is two words (field "B") for inter-segment pointers.
        /// </summary>
        public bool IsDoubleFar
        {
            get => (_ptrData & 4) != 0;
        }

        /// <summary>
        /// Encodes an inter-segment pointer.
        /// </summary>
        /// <param name="targetSegmentIndex">target segment index</param>
        /// <param name="landingPadOffset">landing pad offset</param>
        /// <param name="isDoubleFar">whether the landing pad is two words</param>
        /// <exception cref="ArgumentOutOfRangeException">negative landing pad offset, or encoding would require more than 29 bits</exception>
        public void SetFarPointer(uint targetSegmentIndex, int landingPadOffset, bool isDoubleFar)
        {
            if (landingPadOffset < 0 || landingPadOffset >= (1 << 29))
                throw new ArgumentOutOfRangeException(nameof(landingPadOffset));

            _ptrData = ((ulong)targetSegmentIndex << 32) | 
                ((uint)landingPadOffset << 3) | 
                (ulong)PointerKind.Far;
            if (isDoubleFar) _ptrData |= 4;
        }

        /// <summary>
        /// Returns the sub-kind of pointer (field "B") if this is an "other" pointer.
        /// Currently, only 0 is specified, which is a capability pointer.
        /// </summary>
        public uint OtherPointerKind
        {
            get => unchecked((uint)_ptrData) >> 2;
        }

        /// <summary>
        /// Returns the capability index (field "C") if this is a capability pointer.
        /// </summary>
        public uint CapabilityIndex
        {
            get => (uint)(_ptrData >> 32);
        }

        /// <summary>
        /// Encodes a capability pointer.
        /// </summary>
        /// <param name="index">capability index</param>
        public void SetCapability(uint index)
        {
            _ptrData = ((ulong)index << 32) | (ulong)PointerKind.Other;
        }
    }
}
