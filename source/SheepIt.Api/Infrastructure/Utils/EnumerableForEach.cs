using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SheepIt.Api.Infrastructure.Utils
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
        
        public static async Task ForEachAsync<TItem>(this IEnumerable<TItem> items, Func<TItem, int, Task> action)
        {
            await items
                .Select((item, index) => (item, index))
                .ForEachAsync(((TItem item, int index) itemWithIndex) => action(itemWithIndex.item, itemWithIndex.index));
        }

        public static async Task ForEachAsync<TItem>(this IEnumerable<TItem> items, Func<TItem, Task> action)
        {
            foreach (var item in items)
            {
                await action(item);
            }
        }
    }
}