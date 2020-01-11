using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Capnp
{
    class PrimitiveCoder
    {
        class Coder<T>
        {
            public static Func<T, T, T>? Fn { get; set; }
        }

        static PrimitiveCoder()
        {
            Coder<bool>.Fn = (x, y) => x != y;
            Coder<sbyte>.Fn = (x, y) => (sbyte)(x ^ y);
            Coder<byte>.Fn = (x, y) => (byte)(x ^ y);
            Coder<short>.Fn = (x, y) => (short)(x ^ y);
            Coder<ushort>.Fn = (x, y) => (ushort)(x ^ y);
            Coder<int>.Fn = (x, y) => x ^ y;
            Coder<uint>.Fn = (x, y) => x ^ y;
            Coder<long>.Fn = (x, y) => x ^ y;
            Coder<ulong>.Fn = (x, y) => x ^ y;
            Coder<float>.Fn = (x, y) =>
            {
                int xi = x.ReplacementSingleToInt32Bits();
                int yi = y.ReplacementSingleToInt32Bits();
                int zi = xi ^ yi;
                return BitConverter.ToSingle(BitConverter.GetBytes(zi), 0);
            };
            Coder<double>.Fn = (x, y) =>
            {
                long xi = BitConverter.DoubleToInt64Bits(x);
                long yi = BitConverter.DoubleToInt64Bits(y);
                long zi = xi ^ yi;
                return BitConverter.Int64BitsToDouble(zi);
            };
        }

        public static Func<T, T, T> Get<T>()
        {
            return Coder<T>.Fn ??
                throw new NotSupportedException("Generic type argument is not a supported primitive type, no coder defined");
        }
    }
}
#nullable restore