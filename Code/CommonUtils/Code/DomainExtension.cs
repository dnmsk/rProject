using System;
using System.Text;

namespace CommonUtils.Code {
    public class DomainExtension {
        private static readonly char[] _endDomain = new[] { '/', '?' };
        /// <summary>
        /// Декодирует имя домена из пуникода
        /// </summary>
        /// <param name="domain">Имя домена. Внимание! Без http://</param>
        /// <returns>Декодированное имя домена</returns>
        public static string DePunycodeDomain(string domain) {
            if (String.IsNullOrEmpty(domain)) {
                return String.Empty;
            }
            var sb = new StringBuilder();
            foreach (var s in domain.ToLower().Split('.')) {
                var part = s;
                if (s.StartsWith(Punycode.Prefix)) {
                    try {
                        part = Punycode.Decode(s);
                    } catch (Exception) { }
                }
                sb.AppendFormat("{0}.", part);
            }
            return sb.Length > 0 ? sb.ToString(0, sb.Length - 1) : String.Empty;
        }

        /// <summary>
        /// Кодирует имя домена в пуникод
        /// </summary>
        /// <param name="domain">Имя домена(Внимание! Без http://!)</param>
        /// <returns>Закодированное имя домена</returns>
        public static string PunycodeDomain(string domain) {
            var sb = new StringBuilder();
            foreach (var s in domain.ToLower().Split('.')) {
                if (s.StartsWith(Punycode.Prefix)) {
                    sb.AppendFormat("{0}.", s);
                } else {
                    string pun = Punycode.EncodeWithoutPrefix(s);
                    if (pun.TrimEnd('-') == s) {
                        sb.AppendFormat("{0}.", s);
                    } else {
                        sb.AppendFormat("{0}{1}.", Punycode.Prefix, pun);
                    }
                }
            }
            return sb.Length > 0 ? sb.ToString(0, sb.Length - 1) : String.Empty;
        }

        /// <summary>
        /// Декодирует имя домена из пуникода в урле
        /// </summary>
        /// <param name="url">URL-адрес</param>
        /// <returns>URL с декодированным доменом</returns>
        public static string DePunycodeUrl(string url) {
            string[] parts = SplitToParts(url);
            return parts[0] + DePunycodeDomain(parts[1]) + parts[2];
        }

        /// <summary>
        /// Кодирует имя домена в пуникод в урле
        /// </summary>
        /// <param name="url">URL-адрес</param>
        /// <returns>URL с закодированным доменом</returns>
        public static string PunycodeUrl(string url) {
            string[] parts = SplitToParts(url);
            return parts[0] + PunycodeDomain(parts[1]) + parts[2];
        }

        private static string[] SplitToParts(string url) {
            var res = new string[3];

            string noProtocol = url;
            int pos = url.IndexOf("://");
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
    }
}
