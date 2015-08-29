using System;

namespace CommonUtils.WatchfulSloths {
    /// <summary>
    /// Класс для сравнения времени 
    /// </summary>
    public static class TimeComparer {
        /// <summary>
        /// Сравнивает время, не учитывая дату и секунды 
        /// </summary>
        /// <param name="x">Время X</param>
        /// <param name="y">Время TY</param>
        /// <returns>true, если Время X >= Время TY</returns>
        public static bool TimeGreaterOrEqual(DateTime x, DateTime y) {
            return ((x.Hour > y.Hour) || (x.Hour == y.Hour && x.Minute >= y.Minute));
        }

        /// <summary>
        /// Сравнивает время, не учитывая дату и секунды 
        /// </summary>
        /// <param name="x">Время X</param>
        /// <param name="y">Время TY</param>
        /// <returns>true, если Время X > Время TY</returns>
        public static bool TimeGreater(DateTime x, DateTime y) {
            return ((x.Hour > y.Hour) || (x.Hour == y.Hour && x.Minute > y.Minute));
        }

        /// <summary>
        /// Сравнивает время, не учитывая дату и секунды 
        /// </summary>
        /// <param name="x">Время X</param>
        /// <param name="y">Время TY</param>
        /// <returns>true, если Время X = Время TY</returns>
        public static bool TimeEquals(DateTime x, DateTime y) {
            return (x.Hour == y.Hour && x.Minute == y.Minute);
        }
    }
}