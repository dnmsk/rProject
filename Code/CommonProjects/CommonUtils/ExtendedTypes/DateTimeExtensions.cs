using System;
using System.Globalization;

namespace CommonUtils.ExtendedTypes {
    public enum DateRoundType : short {
        Second = 0,
        Minute = 1,
        Hour = 2,
        Day = 3
    }
    public static class DateTimeExtensions {
        public static DateTime StartOfWeek(this DateTime date) {
            CultureInfo ci = new CultureInfo("ru-RU");
            return date.AddDays(ci.DateTimeFormat.FirstDayOfWeek - date.DayOfWeek);
        }

        public static DateTime Round(this DateTime dateTime, DateRoundType roundType, int divider = 0) {
            divider = divider == 0 ? 1 : divider;
            switch (roundType) {
                case DateRoundType.Day:
                    return new DateTime(dateTime.Year, dateTime.Month, (dateTime.Day / divider) * divider);
                case DateRoundType.Hour:
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, (dateTime.Hour / divider) * divider, 0, 0);
                case DateRoundType.Minute:
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, (dateTime.Minute / divider) * divider, 0);
                case DateRoundType.Second:
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, (dateTime.Second / divider) * divider);
            }
            return dateTime;
        }
    }
}