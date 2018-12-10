using System;
using System.Globalization;

namespace SheepIt.ConsolePrototype
{
    public static class DateTimeExtensions
    {
        public static string FileFriendlyFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
        }

        public static string ConsoleFriendlyFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}