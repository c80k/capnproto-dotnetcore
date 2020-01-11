using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capnp
{
#nullable enable
    internal static class UtilityExtensions
    {
        /// <summary>
        /// This method exists until NET Standard 2.1 is released
        /// </summary>
        /// <param name="thisDict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
#if NETSTANDARD2_0
        public static bool ReplacementTryAdd<K, V>(this Dictionary<K, V> thisDict, K key, V value)
        {
            if (thisDict.ContainsKey(key)) return false;
            thisDict.Add(key, value);
            return true;
        }
#else
        public static bool ReplacementTryAdd<K, V>(this Dictionary<K, V> thisDict, K key, V value) where K: struct => thisDict.TryAdd(key, value);
#endif

        /// <summary>
        /// This method exists until NET Standard 2.1 is released
        /// </summary>
        /// <param name="thisDict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
#if NETSTANDARD2_0
        public static bool ReplacementTryRemove<K, V>(this Dictionary<K, V> thisDict, K key, out V value)
        {
            if (!thisDict.ContainsKey(key))
            {
                value = default!; // OK here since .NET Standard 2.0 does not support (non-)nullability anyway.
                return false;
            }
            value = thisDict[key];
            return thisDict.Remove(key);
        }
#else
#pragma warning disable CS8714
        public static bool ReplacementTryRemove<K, V>(this Dictionary<K, V> thisDict, K key, out V value) => thisDict.Remove(key, out value);
#pragma warning restore CS8714
#endif

        /// <summary>
        /// This method exists until NET Standard 2.1 is released
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
#if NETSTANDARD2_0
        public static bool ReplacementTaskIsCompletedSuccessfully(this Task task)
        {
            return task.IsCompleted && !task.IsCanceled && !task.IsFaulted;
        }
#else
        public static bool ReplacementTaskIsCompletedSuccessfully(this Task task) => task.IsCompletedSuccessfully;
#endif

#if NETSTANDARD2_0
        public static int ReplacementSingleToInt32Bits(this float value) => BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
#else
        public static int ReplacementSingleToInt32Bits(this float value) => BitConverter.SingleToInt32Bits(value);
#endif

#if NETSTANDARD2_0
        public static float ReplacementInt32ToSingleBits(this int value) => BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
#else
        public static float ReplacementInt32ToSingleBits(this int value) => BitConverter.Int32BitsToSingle(value);
#endif
    }
}
#nullable restore