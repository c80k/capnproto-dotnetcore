using System;
using System.Runtime.InteropServices;

namespace Capnp
{
    /// <summary>
    /// Provides extensions to the <see cref="IStructDeserializer"/> and <see cref="IStructSerializer"/> interfaces for type-safe reading and writing.
    /// </summary>
    public static class SerializerExtensions
    {
        /// <summary>
        /// Reads a boolean field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static bool ReadDataBool<T>(this T d, ulong bitOffset, bool defaultValue = false)
            where T: IStructDeserializer
        {
            return (d.StructReadData(bitOffset, 1) != 0) != defaultValue;
        }

        /// <summary>
        /// Writes a boolean field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, bool value, bool defaultValue = false)
            where T : IStructSerializer
        {
            d.StructWriteData(bitOffset, 1, value != defaultValue ? 1ul : 0);
        }

        /// <summary>
        /// Reads a byte field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static byte ReadDataByte<T>(this T d, ulong bitOffset, byte defaultValue = 0)
            where T : IStructDeserializer
        {
            return (byte)(d.StructReadData(bitOffset, 8) ^ defaultValue);
        }

        /// <summary>
        /// Writes a byte field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, byte value, byte defaultValue = 0)
            where T : IStructSerializer
        {
            d.StructWriteData(bitOffset, 8, (byte)(value ^ defaultValue));
        }

        /// <summary>
        /// Reads a signed byte field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static sbyte ReadDataSByte<T>(this T d, ulong bitOffset, sbyte defaultValue = 0)
            where T : IStructDeserializer
        {
            return (sbyte)((sbyte)d.StructReadData(bitOffset, 8) ^ defaultValue);
        }

        /// <summary>
        /// Writes a signed byte field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, sbyte value, sbyte defaultValue = 0)
            where T : IStructSerializer
        {
            d.StructWriteData(bitOffset, 8, unchecked((ulong)(value ^ defaultValue)));
        }

        /// <summary>
        /// Reads a ushort field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static ushort ReadDataUShort<T>(this T d, ulong bitOffset, ushort defaultValue = 0)
            where T : IStructDeserializer
        {
            return (ushort)(d.StructReadData(bitOffset, 16) ^ defaultValue);
        }

        /// <summary>
        /// Writes a ushort field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, ushort value, ushort defaultValue = 0)
            where T : IStructSerializer
        {
            d.StructWriteData(bitOffset, 16, (ushort)(value ^ defaultValue));
        }

        /// <summary>
        /// Reads a short field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static short ReadDataShort<T>(this T d, ulong bitOffset, short defaultValue = 0)
            where T : IStructDeserializer
        {
            return (short)((short)d.StructReadData(bitOffset, 16) ^ defaultValue);
        }

        /// <summary>
        /// Writes a short field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, short value, short defaultValue = 0)
            where T : IStructSerializer
        {
            d.StructWriteData(bitOffset, 16, unchecked((ulong)(value ^ defaultValue)));
        }

        /// <summary>
        /// Reads a uint field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static uint ReadDataUInt<T>(this T d, ulong bitOffset, uint defaultValue = 0)
            where T : IStructDeserializer
        {
            return (uint)(d.StructReadData(bitOffset, 32) ^ defaultValue);
        }

        /// <summary>
        /// Writes a uint field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, uint value, uint defaultValue = 0)
            where T : IStructSerializer
        {
            d.StructWriteData(bitOffset, 32, value ^ defaultValue);
        }

        /// <summary>
        /// Performs a "reinterpret cast" of the struct's data section and returns a reference to a single element of the cast result.
        /// </summary>
        /// <typeparam name="U">The cast target type. Must be a primitive type which qualifies for <code><![CDATA[MemoryMarshal.Cast<ulong, U>()]]></code></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="woffset">Index within the cast result, conceptually into <code>U[]</code></param>
        /// <returns>A reference to the data element</returns>
        public static ref U RefData<U>(this IStructSerializer d, int woffset)
            where U: struct
        {
            var data = MemoryMarshal.Cast<ulong, U>(d.StructDataSection);
            return ref MemoryMarshal.GetReference(data.Slice(woffset, 1));
        }

        /// <summary>
        /// Reads an int field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static int ReadDataInt<T>(this T d, ulong bitOffset, int defaultValue = 0)
            where T : IStructDeserializer
        {
            return (int)d.StructReadData(bitOffset, 32) ^ defaultValue;
        }

        /// <summary>
        /// Writes an int field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, int value, int defaultValue = 0)
            where T : IStructSerializer
        {
            d.StructWriteData(bitOffset, 32, unchecked((ulong)(value ^ defaultValue)));
        }

        /// <summary>
        /// Reads a ulong field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static ulong ReadDataULong<T>(this T d, ulong bitOffset, ulong defaultValue = 0)
            where T : IStructDeserializer
        {
            return d.StructReadData(bitOffset, 64) ^ defaultValue;
        }

        /// <summary>
        /// Writes a ulong field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, ulong value, ulong defaultValue = 0)
            where T : IStructSerializer
        {
            d.StructWriteData(bitOffset, 64, value ^ defaultValue);
        }

        /// <summary>
        /// Reads a long field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static long ReadDataLong<T>(this T d, ulong bitOffset, long defaultValue = 0)
            where T : IStructDeserializer
        {
            return (long)d.StructReadData(bitOffset, 64) ^ defaultValue;
        }

        /// <summary>
        /// Writes a long field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, long value, long defaultValue = 0)
            where T : IStructSerializer
        {
            d.StructWriteData(bitOffset, 64, unchecked((ulong)(value ^ defaultValue)));
        }

        /// <summary>
        /// Reads a float field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (raw bits will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static float ReadDataFloat<T>(this T d, ulong bitOffset, float defaultValue = 0)
            where T : IStructDeserializer
        {
            int defaultBits = defaultValue.SingleToInt32();
            int bits = (int)d.StructReadData(bitOffset, 32) ^ defaultBits;
            return bits.Int32ToSingle();
        }

        /// <summary>
        /// Writes a float field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (raw bits will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, float value, float defaultValue = 0.0f)
            where T : IStructSerializer
        {
            int bits = value.SingleToInt32();
            int defaultBits = defaultValue.SingleToInt32();
            WriteData(d, bitOffset, bits, defaultBits);
        }

        /// <summary>
        /// Reads a double field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructDeserializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="defaultValue">Field default value (raw bits will be XORed with the result)</param>
        /// <returns>The read value</returns>
        public static double ReadDataDouble<T>(this T d, ulong bitOffset, double defaultValue = 0)
            where T : IStructDeserializer
        {
            long defaultBits = BitConverter.DoubleToInt64Bits(defaultValue);
            long bits = (long)d.StructReadData(bitOffset, 64) ^ defaultBits;
            return BitConverter.Int64BitsToDouble(bits);
        }

        /// <summary>
        /// Writes a double field.
        /// </summary>
        /// <typeparam name="T">Type implementing <see cref="IStructSerializer"/></typeparam>
        /// <param name="d">"this" instance</param>
        /// <param name="bitOffset">Start bit</param>
        /// <param name="value">Value to write</param>
        /// <param name="defaultValue">Field default value (raw bits will be XORed with the value to write)</param>
        public static void WriteData<T>(this T d, ulong bitOffset, double value, double defaultValue = 0.0)
            where T : IStructSerializer
        {
            long bits = BitConverter.DoubleToInt64Bits(value);
            long defaultBits = BitConverter.DoubleToInt64Bits(defaultValue);
            WriteData(d, bitOffset, bits, defaultBits);
        }
    }
}
