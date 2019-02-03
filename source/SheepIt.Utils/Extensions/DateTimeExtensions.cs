﻿using System;
using System.Globalization;

namespace SheepIt.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        public static string FileFriendlyFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
        }
    }
}