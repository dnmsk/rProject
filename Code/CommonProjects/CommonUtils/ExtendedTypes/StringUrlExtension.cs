using System;
using System.Text.RegularExpressions;

namespace CommonUtils.ExtendedTypes {
    /// <summary>
    /// Методы расширения для строк - работа с урлами.
    /// </summary>
    public static class StringUrlExtentions {
        private const string HTTP = "http://";

        private const string HTTPS = "https://";

        private const string WWW = "www.";
        
        private const int MAX_DOMAIN_LENGTH = 127;
        
        private const int MAX_SUBDOMAIN_LENGTH = 254;

        /// <summary>
        /// Символы, которыми может заканчивается домен.
        /// </summary>
        private static readonly char[] _endDomain = new[] { '/', '?' };

        /// <summary>
        /// Регэксп для валидации домана.
        /// </summary>
        private static readonly Regex _domainValidator = new Regex(
            @"^((http|https)\://)?(www\.)?(([a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,4})|([а-яА-Я0-9\-\.]+\.[а-яА-Я]{2,4}))/?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Регэксп для валидации урла.
        /// </summary>
        private static readonly Regex _urlValidator = new Regex(
            @"^((http|https|ftp)\://)?(([a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,4})|([а-яА-Я0-9\-\.]+\.[а-яА-Я]{2,4}))(:[a-zA-Z0-9]*)?/?([a-zA-Zа-яА-ЯёЁ0-9\-\._\?\,\:\'\{\}|`""@\^\*\s\(\)\[\]/\\\+&amp;%\$#\=~])*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Удаляет все "http://" в начале, если необходимо.
        /// </summary>
        /// <param name="url">Урл в строковом представлении.</param>
        /// <returns>Возвращает строку без "http://" в начале.</returns>
        public static string CutHttp(this string url) {
            while (url.IsHttpStart()) {
                url = url.Substring(HTTP.Length);
            }

            return url;
        }

        /// <summary>
        /// Удаляет все "https://" в начале, если необходимо.
        /// </summary>
        /// <param name="url">Урл в строковом представлении.</param>
        /// <returns>Возвращает строку без "https://" в начале.</returns>
        public static string CutHttps(this string url) {
            while (IsHttpsStart(url)) {
                url = url.Substring(HTTPS.Length);
            }

            return url;
        }

        /// <summary>
        /// Удаляет все "http://" и "https://" в начале, если необходимо.
        /// </summary>
        /// <param name="url">Урл в строковом представлении.</param>
        /// <returns>Возвращает строку без "http://" и "https://" в начале.</returns>
        public static string CutHttpHttps(this string url) {
            while (true) {
                if (url.IsHttpStart()) {
                    url = url.Substring(HTTP.Length);
                } else if (url.IsHttpsStart()) {
                    url = url.Substring(HTTPS.Length);
                } else {
                    break;
                }
            }

            return url;
        }

        /// <summary>
        /// Удаление всех символов начиная с ":".
        /// </summary>
        /// <param name="url">Урл в строковом представлении.</param>
        /// <remarks>Внимание! Урл должен быть без "http://...", иначе вернется только "http".</remarks>
        /// <returns>Возвращаем подстроку до ":".</returns>
        public static string CutPort(this string url) {
            var index = url.IndexOf(":", StringComparison.OrdinalIgnoreCase);
            return index > 0
                ? url.Substring(0, index)
                : url;
        }

        /// <summary>
        /// Возвращает домен без "http://" и "https://" вначале, обрезает до / или ?.
        /// </summary>
        /// <param name="url">Урл в строковом представлении.</param>
        /// <returns>Возвращает домен без "http://" и "https://".</returns>
        public static string GetDomain(this string url) {
            return url.CutHttpHttps().CutWww().Split(_endDomain, 2)[0];
        }

        /// <summary>
        /// Возвращает относительный путь страницы
        /// </summary>
        /// <param name="url">Урл в строковом представлении.</param>
        /// <returns>Возвращает домен без "http://" и "https://".</returns>
        public static string GetRelativePath(this string url) {
            var res = url.CutHttpHttps().Split(_endDomain, 2);
            return res.Length > 1 && !string.IsNullOrEmpty(res[1]) ? res[1] : string.Empty;
        }

        /// <summary>
        /// Удаляет все "www." в начале, если необходимо.
        /// </summary>
        /// <param name="url">Урл в строковом представлении.</param>
        /// <returns>Возвращает строку без "www." в начале.</returns>
        public static string CutWww(this string url) {
            while (IsWwwStart(url)) {
                url = url.Substring(WWW.Length);
            }

            return url;
        }
        
        private static string[] SplitToParts(string url) {
            var res = new string[3];

            string noProtocol = url;
            int pos = url.IndexOf("://", StringComparison.OrdinalIgnoreCase);
            if (pos != -1) {
                res[0] = url.Substring(0, pos + 3);
                noProtocol = url.Substring(pos + 3);
            } else {
                res[0] = String.Empty;
            }
            pos = noProtocol.IndexOfAny(_endDomain);
            res[1] = pos < 0 ? noProtocol : noProtocol.Substring(0, pos);
            res[2] = pos < 0 ? String.Empty : noProtocol.Substring(pos);
            return res;
        }

        /// <summary>
        /// Проверяет строку, является ли она доменом в допустимом формате.
        /// </summary>
        /// <param name="domain">Домен для проверки. Может быть с "http://".</param>
        /// <returns>Возвращает true, если строка является правильным доменом, иначе false.</returns>
        public static bool IsDomain(this string domain) {
            if (string.IsNullOrEmpty(domain)) {
                return false;
            }

            var splitted = domain.Split('.');
            if ((splitted.Length == 2 && domain.Length > MAX_DOMAIN_LENGTH)
                || (splitted.Length > 2 &&
                    (domain.Length > MAX_SUBDOMAIN_LENGTH ||
                    string.Format("{0}.{1}", splitted[splitted.Length - 2], splitted[splitted.Length - 1]).Length > MAX_DOMAIN_LENGTH))) {
                return false;
            }

            return _domainValidator.IsMatch(domain);
        }

        /// <summary>
        /// Добавляет в начало к урлу в случае необходимости "http://".
        /// </summary>
        /// <param name="url">Урл для добавления.</param>
        /// <returns>Возвращает урл с "http://" вначале.</returns>
        public static string WithHttp(this string url) {
            if (url.IsNullOrEmpty() || url.IsHttpStart() || url.IsHttpsStart()) {
                return url;
            }

            if (url.StartsWith("//")) {
                return "http:" + url;
            }

            return HTTP + url;
        }

        /// <summary>
        /// Отрезает "http(s)://" и добавляет в начало к урлу в случае необходимости "www.".
        /// </summary>
        /// <param name="url">Урл для добавления www.</param>
        /// <returns>Возвращает урл без "http(s)://" с "www." вначале.</returns>
        public static string WithWww(this string url) {
            if (url.IsNullOrEmpty()) {
                return url;
            }

            url = url.CutHttpHttps();
            return url.IsWwwStart()
                ? url
                : WWW + url;
        }

        private static bool IsHttpStart(this string url) {
            return url.StartsWith(HTTP, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsHttpsStart(this string url) {
            return url.StartsWith(HTTPS, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Проверяет начинается ли урл с ввв
        /// </summary>
        /// <param name="url">урл</param>
        /// <returns></returns>
        public static bool IsWwwStart(this string url) {
            return url.StartsWith(WWW, StringComparison.OrdinalIgnoreCase);
        }
    }
}
