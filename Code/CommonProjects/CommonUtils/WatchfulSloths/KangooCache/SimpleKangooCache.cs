using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;

namespace CommonUtils.WatchfulSloths.KangooCache {
    public class SimpleKangooCache<K, V> {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger("SimpleKangooCache");

        protected readonly V DefValue;
        private readonly Func<K, V> _valueGetter;
        protected readonly ConcurrentDictionary<K, V> Cache;

        public SimpleKangooCache(V defValue, Func<K, V> valueGetter) {
            DefValue = defValue;
            _valueGetter = valueGetter;
            var comparer = GetComparer();
            Cache = comparer == null
                ? new ConcurrentDictionary<K, V>()
                : new ConcurrentDictionary<K, V>(comparer);
        }

        protected virtual IEqualityComparer<K> GetComparer() {
            return null;
        }

        protected virtual bool NeedUpdate(V inCache) {
            return false;
        }

        /// <summary>
        /// Получение элемента из кэша. Если элемента нет - он будет получен по правилу получения этого ключа.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public V this[K key] {
            get {
                if (key == null) {
                    _logger.Error(
                        new Exception(
                            string.Format("Ключ null для типа {0}, класс {1}. Такого исключения не должно быть, что-то сломалось. Будет возвращено дефолтное значение.", typeof(V), GetType())
                        )
                    );
                    return DefValue;
                }

                V cacheElement;
                if (!Cache.TryGetValue(key, out cacheElement) || NeedUpdate(cacheElement) ) {
                    if (ConfigHelper.TestMode) {
                        return _valueGetter(key);
                    }
                    cacheElement = _valueGetter(key);
                    Cache[key] = cacheElement;
                }
                return cacheElement;
            }
            set {
                Cache[key] = value;
            }
        }
    }
}
