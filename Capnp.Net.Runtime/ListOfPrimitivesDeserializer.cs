using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Capnp
{
    /// <summary>
    /// ListDeserializer specialization for List(Int*), List(UInt*), List(Float*), and List(Enum).
    /// </summary>
    /// <typeparam name="T">List element type</typeparam>
    public class ListOfPrimitivesDeserializer<T>: ListDeserializer, IReadOnlyList<T>
        where T: struct
    {
        class ListOfULongAsStructView<U> : IReadOnlyList<U>
        {
            readonly ListOfPrimitivesDeserializer<ulong> _lpd;
            readonly Func<DeserializerState, U> _sel;

            public ListOfULongAsStructView(ListOfPrimitivesDeserializer<ulong> lpd, Func<DeserializerState, U> sel)
            {
                _lpd = lpd;
                _sel = sel;
            }

            public U this[int index]
            {
                get
                {
                    var state = _lpd.State;

                    if (index < 0 || index >= _lpd.Count)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    state.Offset += index;
                    state.Kind = ObjectKind.Struct;
                    state.StructDataCount = 1;
                    state.StructPtrCount = 0;

                    return _sel(state);
                }
            }

            public int Count => _lpd.Count;

            IEnumerable<U> Enumerate()
            {
                var state = _lpd.State;

                state.Kind = ObjectKind.Struct;
                state.StructDataCount = 1;
                state.StructPtrCount = 0;

                for (int i = 0; i < Count; i++)
                {
                    yield return _sel(state);
                    ++state.Offset;
                }

            }

            public IEnumerator<U> GetEnumerator()
            {
                return Enumerate().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        readonly ListKind _kind;

        internal ListOfPrimitivesDeserializer(ref DeserializerState state, ListKind kind) : 
            base(ref state)
        {
            _kind = kind;
        }

        /// <summary>
        /// One of ListOfBytes, ListOfShorts, ListOfInts, ListOfLongs.
        /// </summary>
        public override ListKind Kind => _kind;

        ReadOnlySpan<T> Data => MemoryMarshal.Cast<ulong, T>(State.CurrentSegment.Slice(State.Offset)).Slice(0, Count);

        /// <summary>
        /// Returns the element at given index.
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Element value</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        public T this[int index] => Data[index];

        ListOfPrimitivesDeserializer<U> PrimitiveCast<U>() where U: struct
        {
            if (Marshal.SizeOf<U>() != Marshal.SizeOf<T>())
                throw new NotSupportedException("Source and target types have different sizes, cannot cast");

            var stateCopy = State;
            return new ListOfPrimitivesDeserializer<U>(ref stateCopy, Kind);
        }

        /// <summary>
        /// Always throws <see cref="NotSupportedException"/> because this specialization can never represent a List(bool).
        /// </summary>
        public override IReadOnlyList<bool> CastBool() => throw new NotSupportedException("Cannot cast to list of bits");

        /// <summary>
        /// Attempts to interpret this instance as List(UInt8). 
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">Element size is different from 1 byte.</exception>
        public override IReadOnlyList<byte> CastByte() => PrimitiveCast<byte>();

        /// <summary>
        /// Attempts to interpret this instance as List(Int8). 
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">Element size is different from 1 byte.</exception>
        public override IReadOnlyList<sbyte> CastSByte() => PrimitiveCast<sbyte>();

        /// <summary>
        /// Attempts to interpret this instance as List(UInt16). 
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">Element size is different from 2 bytes.</exception>
        public override IReadOnlyList<ushort> CastUShort() => PrimitiveCast<ushort>();

        /// <summary>
        /// Attempts to interpret this instance as List(Int16). 
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">Element size is different from 2 bytes.</exception>
        public override IReadOnlyList<short> CastShort() => PrimitiveCast<short>();

        /// <summary>
        /// Attempts to interpret this instance as List(UInt32). 
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">Element size is different from 4 bytes.</exception>
        public override IReadOnlyList<uint> CastUInt() => PrimitiveCast<uint>();

        /// <summary>
        /// Attempts to interpret this instance as List(Int32). 
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">Element size is different from 4 bytes.</exception>
        public override IReadOnlyList<int> CastInt() => PrimitiveCast<int>();

        /// <summary>
        /// Attempts to interpret this instance as List(UInt64). 
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">Element size is different from 8 bytes.</exception>
        public override IReadOnlyList<ulong> CastULong() => PrimitiveCast<ulong>();

        /// <summary>
        /// Attempts to interpret this instance as List(Int64). 
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">Element size is different from 8 bytes.</exception>
        public override IReadOnlyList<long> CastLong() => PrimitiveCast<long>();

        /// <summary>
        /// Attempts to interpret this instance as List(Float32). 
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">Element size is different from 4 bytes.</exception>
        public override IReadOnlyList<float> CastFloat() => PrimitiveCast<float>();

        /// <summary>
        /// Attempts to interpret this instance as List(Float64). 
        /// </summary>
        /// <returns>The desired representation</returns>
        /// <exception cref="NotSupportedException">Element size is different from 8 bytes.</exception>
        public override IReadOnlyList<double> CastDouble() => PrimitiveCast<double>();

        /// <summary>
        /// Attempts to interpret this instance as List(U) whereby U is a struct, applying a selector function to each element.
        /// </summary>
        /// <param name="cons">Selector function</param>
        /// <returns>The desired representation</returns>
        public override IReadOnlyList<U> Cast<U>(Func<DeserializerState, U> cons)
        {
            switch (Marshal.SizeOf<T>())
            {
                case 1: return PrimitiveCast<byte>().LazyListSelect(x => cons(DeserializerState.MakeValueState(x)));
                case 2: return PrimitiveCast<ushort>().LazyListSelect(x => cons(DeserializerState.MakeValueState(x)));
                case 4: return PrimitiveCast<uint>().LazyListSelect(x => cons(DeserializerState.MakeValueState(x)));
                case 8: return new ListOfULongAsStructView<U>(PrimitiveCast<ulong>(), cons);
                default:
                    throw new InvalidProgramException("This program path should not be reachable");
            }
        }

        /// <summary>
        /// Attempts to interpret this instance as Text and returns the string representation.
        /// </summary>
        /// <returns>The decoded string</returns>
        /// <exception cref="NotSupportedException">Element size is different from 1 byte.</exception>
        public override string CastText()
        {
            var utf8Bytes = PrimitiveCast<byte>().Data;
            if (utf8Bytes.Length == 0) return string.Empty;
            var utf8GytesNoZterm = utf8Bytes.Slice(0, utf8Bytes.Length - 1);
            return Encoding.UTF8.GetString(utf8GytesNoZterm.ToArray());
        }

        IEnumerable<T> Enumerate()
        {
            for (int i = 0; i < Count; i++)
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