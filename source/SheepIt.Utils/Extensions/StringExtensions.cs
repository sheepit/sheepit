using System.Collections.Generic;

namespace SheepIt.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string JoinWith(this IEnumerable<string> items, string separator)
        {
            return string.Join(separator, items);
        }
    }
}