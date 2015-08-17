using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using CommonUtils.Core.Logger;

namespace CommonUtils.ExtendedTypes {
    public static class StringToMD5Extentions {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(StringToMD5Extentions).FullName);
        const int ACTIVE_HASHER_COUNT = 5;

        static readonly Queue<MD5> _md5Queue = new Queue<MD5>();

        static StringToMD5Extentions() {
            for (var i = 0; i < ACTIVE_HASHER_COUNT; i++) {
                _md5Queue.Enqueue(MD5.Create());
            }
        }

        /// <summary>
        /// Получение MD5 от строки.
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="limitSymbols">Лимит символов (четное значение)</param>
        /// <returns></returns>
        public static string GetMD5(this string str, int? limitSymbols = null) {
            var md5Hasher = GetMD5Hasher();
            try {
                var bytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
                var s = new StringBuilder();
                if (limitSymbols.HasValue) {
                    int limit = limitSymbols.Value;
                    for (int i = 0; s.Length < limit; i++) {
                        s.Append(bytes[i].ToString("x2"));
                    }
                } else {
                    foreach (byte b in bytes) {
                        s.Append(b.ToString("x2"));
                    }
                }
                return s.ToString().ToLower();
            } catch (Exception ex) {
                _logger.Error(str ?? "пустая строка", ex);
            } finally {
                ReleaseMD5Hasher(md5Hasher);
            }
            return string.Empty;
        }

        public static byte[] GetMD5Array(this string str) {
            var md5Hasher = GetMD5Hasher();
            var computeHash = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
            ReleaseMD5Hasher(md5Hasher);
            return computeHash;
        }

        private static MD5 GetMD5Hasher() {
            while (true) {
                lock (_md5Queue) {
                    if (_md5Queue.Count > 0) {
                        return _md5Queue.Dequeue();
                    }
                }
                Thread.Sleep(1);
            }
        }

        private static void ReleaseMD5Hasher(MD5 hasher) {
//            hasher.Clear();
            lock(_md5Queue) {
                _md5Queue.Enqueue(hasher);
            }
        }
    }
}
