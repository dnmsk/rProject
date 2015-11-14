using System;
using System.Text;

namespace CommonUtils.ExtendedTypes {
    /// <summary>
    /// Методы расширения для класса string.
    /// </summary>
    public static class StringExtentions {
        /// <summary>
        /// Сокращенное использование string.Format(...).
        /// </summary>
        /// <param name="str">Строка для форматирования.</param>
        /// <param name="param">Параметры.</param>
        /// <returns>Форматированная строка.</returns>
        public static string Format(this string str, params object[] param) {
            return string.Format(str, param);
        }

        /// <summary>
        /// Сокращенное использование string.Format(...).
        /// </summary>
        /// <param name="str">Строка для форматирования.</param>
        /// <param name="param">Параметры.</param>
        /// <returns>Форматированная строка.</returns>
        public static string FormatSafe(this string str, params object[] param) {
            try {
                return string.Format(str, param);
            } catch (Exception ex) {
                return str;
            }
        }

        /// <summary>
        /// Указывает, является ли строка null или пустой строкой.
        /// </summary>
        /// <remarks>
        /// Аналог есть в ServiceStack.Common.Extensions
        /// </remarks>
        /// <param name="str">Строка для проверки.</param>
        /// <returns>Возвращает true, если строка null, или пустая, иначе false.</returns>
        public static bool IsNullOrEmpty(this string str) {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Указывает, является ли строка null, пустой строкой или строкой состоящей из пробелов/табуляций и т.д..
        /// </summary>
        /// <param name="str">Строка для проверки.</param>
        /// <returns>Возвращает true, если строка null, пустая или состоит из пробелов; иначе false.</returns>
        public static bool IsNullOrWhiteSpace(this string str) {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Возвращает первые <see cref="maxLength"/> символов строки,
        /// строка null или ее длина меньше, то возвращается она же.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <param name="maxLength">Максимальная длина строки.</param>
        /// <returns>Возвращает построку 0..<see cref="maxLength"/>.</returns>
        public static string FirstChars(this string str, int maxLength) {
            return str.IsNullOrEmpty() || str.Length <= maxLength
                ? str
                : str.Substring(0, maxLength);
        }

        /// <summary>
        /// Проверяет содержит ли строка подстроку, с заданным парамтром сравнения
        /// </summary>
        /// <param name="source">Строка</param>
        /// <param name="toCheck">Искомая подстрока</param>
        /// <param name="comp">Способ сравнения строк.</param>
        /// <returns>Признак того, что нашли строку.</returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp) {
            return source.IndexOf(toCheck, comp) >= 0;
        }



        /// <summary>
        /// Получает УТФ8 строку из строки с люой кодирвокой.
        /// </summary>
        /// <param name="str">Строка.</param>
        /// <returns>Символы в утф8.</returns>
        public static string GetUtf8String(this string str) {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string RemoveAllTags(this string stringWithTags) {
            const char START_TAG = '<';
            const char END_TAG = '>';
            var startTagFound = false;
            var result = string.Empty;
            for (var i = 0; i < stringWithTags.Length; i++) {
                if (startTagFound) {
                    if (stringWithTags[i] == END_TAG) {
                        startTagFound = false;
                    }
                    continue;
                }
                if (stringWithTags[i] == START_TAG) {
                    startTagFound = true;
                    continue;
                }
                result += stringWithTags[i];
            }
            return result;
        }
    }
}
