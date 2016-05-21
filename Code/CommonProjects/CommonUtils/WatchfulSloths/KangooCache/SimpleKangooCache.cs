using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;

namespace CommonUtils.WatchfulSloths.KangooCache {
    public class SimpleKangooCache<K, V> {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger("SimpleKangooCache");
        
        private readonly Func<K, V> _valueGetter;
        protected ConcurrentDictionary<K, V> Cache { get; private set; }

        public SimpleKangooCache(Func<K, V> valueGetter, IEqualityComparer<K> comparer = null) {
            _valueGetter = valueGetter;
            Cache = comparer == null
                ? new ConcurrentDictionary<K, V>()
                : new ConcurrentDictionary<K, V>(comparer);
        }
        
        protected virtual bool NeedUpdate(V inCache) {
            return false;
        }

        protected void ReplaceCacheContainer(ConcurrentDictionary<K, V> newCache) {
            Cache = newCache;
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
                    return default(V);
                }

                V cacheElement;
                if ((!Cache.TryGetValue(key, out cacheElement) || NeedUpdate(cacheElement)) && _valueGetter != null) {
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
        
        public bool TryGetValue(K key, out V value) {
            return Cache.TryGetValue(key, out value);
        }

        public bool ContainsKey(K key) {
            return Cache.ContainsKey(key);
        }

        public K[] Keys => Cache.Keys.ToArray();
    }
}
