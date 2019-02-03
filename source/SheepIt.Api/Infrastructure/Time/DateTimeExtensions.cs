using System;
using System.Globalization;

namespace SheepIt.Api.Infrastructure.Time
{
    public static class DateTimeExtensions
    {
        public static string FileFriendlyFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
        }
    }
}