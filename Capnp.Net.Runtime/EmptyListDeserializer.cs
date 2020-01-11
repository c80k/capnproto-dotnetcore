using System;
using System.Collections.Generic;

#nullable enable
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
        /// Returns an empty <code><![CDATA[IReadOnlyList<bool>]]></code>/>.
        /// </summary>
        public override IReadOnlyList<bool> CastBool() => new EmptyList<bool>();

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<byte>]]></code>/>.
        /// </summary>
        public override IReadOnlyList<byte> CastByte() => new EmptyList<byte>();

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<double>]]></code>/>.
        /// </summary>
        public override IReadOnlyList<double> CastDouble() => new EmptyList<double>();

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<float>]]></code>.
        /// </summary>
        public override IReadOnlyList<float> CastFloat() => new EmptyList<float>();

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<int>]]></code>.
        /// </summary>
        public override IReadOnlyList<int> CastInt() => new EmptyList<int>();

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<ListDeserializer>]]></code>.
        /// </summary>
        public override IReadOnlyList<ListDeserializer> CastList() => new EmptyList<ListDeserializer>();

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<long>]]></code>/>.
        /// </summary>
        public override IReadOnlyList<long> CastLong() => new EmptyList<long>();

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<sbyte>]]></code>.
        /// </summary>
        public override IReadOnlyList<sbyte> CastSByte() => new EmptyList<sbyte>();

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<short>]]></code>.
        /// </summary>
        public override IReadOnlyList<short> CastShort() => new EmptyList<short>();

        /// <summary>
        /// Returns an empty string.
        /// </summary>
        public override string CastText() => string.Empty;

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<uint>]]></code>.
        /// </summary>
        public override IReadOnlyList<uint> CastUInt() => new EmptyList<uint>();

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<ulong>]]></code>.
        /// </summary>
        public override IReadOnlyList<ulong> CastULong() => new EmptyList<ulong>();

        /// <summary>
        /// Returns an empty <code><![CDATA[IReadOnlyList<ushort>]]></code>.
        /// </summary>
        public override IReadOnlyList<ushort> CastUShort() => new EmptyList<ushort>();
    }
}
#nullable restore