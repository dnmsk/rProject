using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;
using CommonUtils.WatchfulSloths.SlothMoveRules;

namespace CommonUtils.WatchfulSloths.KangooCache {
    /// <summary>
    /// Абстрактный кеш объектов, которые должны быть получены в момент обращения.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class KangarooCache<K, V> {
        
        private readonly TimeSpan _keyActualTime;
        private readonly V _defValue;
        private readonly Func<K, V> _valueGetter;

        /// <summary>
        /// Объект для логирования ошибок и отладочной информации.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger("KangarooCache");

        private readonly ConcurrentDictionary<K, KangooCacheElement<V>> _cache;

        public KangarooCache(V defValue, IWatchfulSloth sloth, Func<K, V> valueGetter, TimeSpan? keyActualTime = null) {
            _keyActualTime = keyActualTime ?? new TimeSpan(1, 0, 0);
            _defValue = defValue;
            _valueGetter = valueGetter;
            var comparer = GetComparer();
            _cache = comparer == null 
                ? new ConcurrentDictionary<K, KangooCacheElement<V>>()
                : new ConcurrentDictionary<K, KangooCacheElement<V>>(comparer);

            if (sloth != null) {
                sloth.SetMove(new SlothMoveByTimeSingle<object>(SelfClean, new TimeSpan(0, 2, 0), null));
            } else {
                _logger.Warn("Ахтунг!!! Тип {0} будет работать без самоочистки", GetType());
            }
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
                    return _defValue;
                }

                KangooCacheElement<V> cacheElement;
                if (!_cache.TryGetValue(key, out cacheElement) || cacheElement.LastActualDate < DateTime.Now || cacheElement.Element.Equals(_defValue)) {
                    if (ConfigHelper.TestMode) {
                        return _valueGetter(key);
                    }
                    if (_cache.TryGetValue(key, out cacheElement)) {
                        return cacheElement.Element;
                    }
                    cacheElement = new KangooCacheElement<V> {
                        Element = _valueGetter(key),
                        LastActualDate = DateTime.Now.Add(_keyActualTime)

                    };                            
                    _cache[key] = cacheElement;
                }
                return cacheElement.Element;
            }
            set {
                _cache[key] = new KangooCacheElement<V> {
                    Element = value,
                    LastActualDate = DateTime.Now.Add(_keyActualTime)
                };
            }
        }

        protected virtual IEqualityComparer<K> GetComparer() {
            return null;
        }
        
        private object SelfClean() {
            ICollection<K> keys = _cache.Keys.ToArray();
            var now = DateTime.Now;
            int cleanedKeys = 0;
            foreach (var key in keys) {
                try {
                    if (_cache[key].LastActualDate < now) {
                        KangooCacheElement<V> val;
                        _cache.TryRemove(key, out val);
                        cleanedKeys++;
                    }
                } catch (Exception ex) {
                    _logger.Error(ex);
                }
            }
            _logger.Info("Для типа {0} удалил {1} устаревших ключей из кеша, осталось ключей: {2}", GetType(), cleanedKeys, _cache.Keys.Count);
            return null;
        }

        /// <summary>
        /// Очистка кеша полностью
        /// </summary>
        public void Clear() {
            _cache.Clear();
        }
    }
}
