using System;
using System.Collections.Generic;
using System.Linq;

namespace SheepIt.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static Dictionary<TKey, TItem> IndexBy<TKey, TItem>(this IEnumerable<TItem> enumerable, Func<TItem, TKey> keySelector)
        {
            return enumerable.ToDictionary(keySelector, value => value);
        }

        public static void ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem, int> action)
        {
            items
                .Select((item, index) => (item, index))
                .ForEach(((TItem item, int index) tuple) => action(tuple.item, tuple.index));
        }

        public static void ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}