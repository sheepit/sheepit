using System.Collections.Generic;
using System.Linq;

namespace SheepIt.Api.Infrastructure.Utils
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            return keyValuePairs.ToDictionary(
                keySelector: pair => pair.Key,
                elementSelector: pair => pair.Value
            );
        }
    }
}