using System;
using System.Globalization;

namespace CommonUtils.ExtendedTypes {
    public static class DateTimeExtensions {
        public static DateTime StartOfWeek(this DateTime date) {
            CultureInfo ci = new CultureInfo("ru-RU");
            return date.AddDays(ci.DateTimeFormat.FirstDayOfWeek - date.DayOfWeek);
        }
    }
}