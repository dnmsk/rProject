using System;
using System.Globalization;
using CommonUtils.ExtendedTypes;

namespace CommonUtils.Code {
    public class StringParser {
        /// <summary>
        /// Парсим в long со значением по умолчанию
        /// </summary>
        /// <param name="val">Строка для конверта</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Возвращает long, если ошибка формата возвращаем значение по умолчанию</returns>
        public static long ToLong(string val, long def) {
            long res;
            if (!Int64.TryParse(val, out res)) {
                res = def;
            }
            return res;
        }

        /// <summary>
        /// Парсим в int со значением по умолчанию
        /// </summary>
        /// <param name="val">Строка для конврета</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Возвращает int, если ошибка формата возвращаем значение по умолчанию</returns>
        public static int ToInt(string val, int def) {
            int res;
            if (!Int32.TryParse(val, out res)) {
                res = def;
            }
            return res;
        }

        /// <summary>
        /// Парсим в bool со значением по умолчанию, позволяет парсить 0 и 1
        /// </summary>
        /// <param name="val">Строка для конврета, как объект</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Возвращает bool, если ошибка формата возвращаем значение по умолчанию</returns>
        public static bool ToBool(string val, bool def = false) {
            bool res;
            if (!Boolean.TryParse(val, out res)) {
                int intRes;
                res = Int32.TryParse(val, out intRes) ? intRes > 0 : def;
            }
            return res;
        }

        /// <summary>
        /// Парсим в decimal со значением по умолчанию
        /// </summary>
        /// <param name="val">Строка для конверта, как объект</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Возвращает decimal, если ошибка формата возвращаем значение по умолчанию</returns>
        public static decimal? ToDecimal(string val, decimal? def) {
            if (string.IsNullOrEmpty(val)) {
                return def;
            }

            val = ReplaceSeparators(val);
            decimal res;
            if (!Decimal.TryParse(val, out res)) {
                return null;
            }
            return res;
        }

        /// <summary>
        /// Парсим в decimal со значением по умолчанию
        /// </summary>
        /// <param name="val">Строка для конверта, как объект</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Возвращает decimal, если ошибка формата возвращаем значение по умолчанию</returns>
        public static decimal ToDecimal(string val, decimal def = default(decimal)) {
            if (string.IsNullOrEmpty(val)) {
                return def;
            }

            val = ReplaceSeparators(val);
            decimal res;
            if (!Decimal.TryParse(val, out res)) {
                res = def;
            }
            return res;
        }

        /// <summary>
        /// Парсим в float со значением по умолчанию
        /// </summary>
        /// <param name="s">Строка для распознавания</param>
        /// <param name="defaultValue">значение по умолчанию</param>
        /// <returns>Сконвертированное значение, а в случае ошибки <paramref name="defaultValue"/></returns>
        public static float ToFloat(string s, float defaultValue = default(float)) {
            if (s != null) {
                string str = ReplaceSeparators(s);
                float result;
                if (!Single.TryParse(str, out result)) {
                    result = defaultValue;
                }
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Парсим в double
        /// </summary>
        /// <param name="s">Строка для распознавания</param>
        /// <returns>Сконвертированное значение, а в случае ошибки 0</returns>
        public static double ToDouble(string s) {
            if (s != null) {
                string str = ReplaceSeparators(s);
                double result;
                Double.TryParse(str, out result);
                return result;
            }

            return default(double);
        }

        /// <summary>
        /// Преобразует строку к decimal, в слуаче неудачи кидает исключение
        /// </summary>
        /// <param name="s">Строка с числом</param>
        /// <returns>Возвращает decimal преобразованный из <paramref name="s"/></returns>
        public static double ToDoubleWithException(string s) {
            if (s == null) {
                throw new ArgumentNullException("s", "Argument cann`t be null.");
            }

            double result;
            if (!Double.TryParse(ReplaceSeparators(s), out result)) {
                throw new ArgumentException("Argument has incorrect format.");
            }
            return result;
        }

        /// <summary>
        /// Преобразует строку к decimal, в слуаче неудачи кидает исключение
        /// </summary>
        /// <param name="s">Строка с числом</param>
        /// <returns>Возвращает decimal преобразованный из s</returns>
        /// <exception cref="ArgumentNullException">Пытаемся распарсить не число.</exception>
        public static decimal ToDecimalWithException(string s) {
            if (s == null) {
                throw new ArgumentNullException("s", "Argument cann`t be null.");
            }

            string str = ReplaceSeparators(s);
            decimal result;
            if (!Decimal.TryParse(str, out result)) {
                throw new ArgumentException("Argument has incorrect format.");
            }
            return result;
        }

        private readonly static CultureInfo[] _cultureToParseDateTime = {
            CultureInfo.InvariantCulture,
            CultureInfo.GetCultureInfo("ru-RU")
        };

        /// <summary>
        /// Конвертируем строку в дату со значением по умолчанию.
        /// </summary>
        /// <param name="strDate">Дата в виде строки.</param>
        /// <param name="def">Значение по умолчанию.</param>
        /// <returns>В случае успеха возвращает сконвертируемую дату, иначе значение по умолчанию.</returns>
        public static DateTime ToDateTime(string strDate, DateTime def, string format = null) {
            DateTime date;
            if (format.IsNullOrWhiteSpace()) {
                return DateTime.TryParse(strDate, out date) ? date : def;
            }
            foreach (var culture in _cultureToParseDateTime) {
                if (DateTime.TryParseExact(strDate, format, culture, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AllowTrailingWhite, out date)) {
                    return date;
                }
            }
            return def;
        }
        /// <summary>
        /// Парсим в byte со значением по умолчанию
        /// </summary>
        /// <param name="val">Строка для конврета</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Возвращает bool, если ошибка формата возвращаем значение по умолчанию</returns>
        public static Int16 ToShort(string val, short def) {
            short res;
            if (!Int16.TryParse(val, out res)) {
                res = def;
            }
            return res;
        }

        /// <summary>
        /// Парсим в byte со значением по умолчанию
        /// </summary>
        /// <param name="val">Строка для конврета</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Возвращает byte, если ошибка формата возвращаем значение по умолчанию</returns>
        public static byte ToByte(string val, byte def) {
            byte res;
            if (!byte.TryParse(val, out res)) {
                res = def;
            }
            return res;
        }

        /// <summary>
        /// Приводит строку к текущему формату чисел Windows
        /// </summary>
        /// <param name="str">Входная строка</param>
        /// <returns>Выходная строка</returns>
        private static string ReplaceSeparators(string str) {
            char separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            return str.Replace(separator == '.' ? ',' : '.', separator);
        }
    }
}
