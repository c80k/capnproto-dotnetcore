using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Capnp
{
  public static class UtilityExtensions
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
    public static bool ReplacementTryAdd<K, V>(this Dictionary<K, V> thisDict, K key, V value)
    {
      if (thisDict.ContainsKey(key))
        return false;
      thisDict.Add(key, value);
      return true;
    }
    
    /// <summary>
    /// This method exists until NET Standard 2.1 is released
    /// </summary>
    /// <param name="thisDict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <returns></returns>
    public static bool ReplacementTryRemove<K, V>(this Dictionary<K, V> thisDict, K key, out V value)
    {
      if (!thisDict.ContainsKey(key))
      {
        value = default;
        return false;
      }
      value = thisDict[key];
      return thisDict.Remove(key);
    }

    /// <summary>
    /// This method exists until NET Standard 2.1 is released
    /// </summary>
    /// <param name="task"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ReplacementTaskIsCompletedSuccessfully(this Task task)
    {
      return task.IsCompleted && !task.IsCanceled && !task.IsFaulted;
    }

    public static int SingleToInt32(this float value)
    {
      var valueBytes = BitConverter.GetBytes(value);
      return BitConverter.ToInt32(valueBytes,0);
    }
    
    public static float Int32ToSingle(this int value) => 
      BitConverter.ToSingle(BitConverter.GetBytes(value),0);
  }
}