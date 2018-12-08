using System;
using System.Globalization;

namespace SheepIt.ConsolePrototype
{
    public static class DateTimeExtensions
    {
        public static string FileFriendlyFormat(this DateTime dateTime)
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
        }
    }
}