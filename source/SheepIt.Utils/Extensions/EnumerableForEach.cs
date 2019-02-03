using System;
using System.Collections.Generic;
using System.Linq;

namespace SheepIt.Utils.Extensions
{
    public static class EnumerableForEach
    {
        public static void ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem, int> action)
        {
            items
                .Select((item, index) => (item, index))
                .ForEach(((TItem item, int index) itemWithIndex) => action(itemWithIndex.item, itemWithIndex.index));
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