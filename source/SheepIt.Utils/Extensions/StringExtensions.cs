using System.Collections.Generic;

namespace SheepIt.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string JoinWith(this IEnumerable<string> items, string separator) => string.Join(separator, items);

        public static int ToInt(this string text) => text == null ? default(int) : int.Parse(text);
    }
}