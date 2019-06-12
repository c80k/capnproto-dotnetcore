using System;
using System.Collections.Generic;

namespace Capnp
{
    /// <summary>
    /// ListDeserializer specialization for empty lists.
    /// </summary>
    public class EmptyListDeserializer : ListDeserializer
    {
        /// <summary>
        /// Always ListKind.ListOfEmpty (despite the fact the empty list != List(Void)
        /// </summary>
        public override ListKind Kind => ListKind.ListOfEmpty;

        /// <summary>
        /// Returns am empty <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <typeparam name="T">Element ype</typeparam>
        /// <param name="cons">Ignored</param>
        public override IReadOnlyList<T> Cast<T>(Func<DeserializerState, T> cons) => new EmptyList<T>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{Boolean}"/>.
        /// </summary>
        public override IReadOnlyList<bool> CastBool() => new EmptyList<bool>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{Byte}"/>.
        /// </summary>
        public override IReadOnlyList<byte> CastByte() => new EmptyList<byte>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{Double}"/>.
        /// </summary>
        public override IReadOnlyList<double> CastDouble() => new EmptyList<double>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{Single}"./>
        /// </summary>
        public override IReadOnlyList<float> CastFloat() => new EmptyList<float>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{Int32}"/>.
        /// </summary>
        public override IReadOnlyList<int> CastInt() => new EmptyList<int>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{ListDeserializer}"/>.
        /// </summary>
        public override IReadOnlyList<ListDeserializer> CastList() => new EmptyList<ListDeserializer>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{Int64}"/>.
        /// </summary>
        public override IReadOnlyList<long> CastLong() => new EmptyList<long>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{SByte}"/>.
        /// </summary>
        public override IReadOnlyList<sbyte> CastSByte() => new EmptyList<sbyte>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{Int16}"/>.
        /// </summary>
        public override IReadOnlyList<short> CastShort() => new EmptyList<short>();

        /// <summary>
        /// Returns an empty string.
        /// </summary>
        public override string CastText() => string.Empty;

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{UInt32}"/>.
        /// </summary>
        public override IReadOnlyList<uint> CastUInt() => new EmptyList<uint>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{UInt64}"/>.
        /// </summary>
        public override IReadOnlyList<ulong> CastULong() => new EmptyList<ulong>();

        /// <summary>
        /// Returns an empty <see cref="IReadOnlyList{UInt16}"/>.
        /// </summary>
        public override IReadOnlyList<ushort> CastUShort() => new EmptyList<ushort>();
    }
}
