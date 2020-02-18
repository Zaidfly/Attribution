using System.Collections.Generic;

namespace Attribution.Domain.Collections
{
    public static class DictionaryExtensions
    {
        public static TValue TryPopValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            if (dict.TryGetValue(key, out var value))
            {
                dict.Remove(key);
            }
            return value;
        }
    }
}