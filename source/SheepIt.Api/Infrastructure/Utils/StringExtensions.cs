﻿using System.Collections.Generic;
using System.Globalization;

namespace SheepIt.Api.Infrastructure.Utils
{
    public static class StringExtensions
    {
        public static string JoinWith(this IEnumerable<string> items, string separator) => string.Join(separator, items);

        public static int ToInt(this string text) => text == null ? default(int) : int.Parse(text, CultureInfo.InvariantCulture);

        public static bool ToBool(this string text) => text != null && bool.Parse(text);
    }
}