using System;
using System.Collections.Generic;
using System.Linq;

namespace SheepIt.Utils.Extensions
{
    public static class EnumerableIndexBy
    {
        public static Dictionary<TKey, TItem> IndexBy<TKey, TItem>(this IEnumerable<TItem> enumerable, Func<TItem, TKey> keySelector)
        {
            return enumerable.ToDictionary(keySelector, value => value);
        }
    }
}