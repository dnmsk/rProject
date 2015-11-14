using System;
using System.Collections.Generic;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;
using CommonUtils.WatchfulSloths.SlothMoveRules;

namespace CommonUtils.WatchfulSloths.KangooCache {

    public class ThriftyKangooSimpleCache<T> : ThriftyKangarooCache<bool, T> where T : class, new() {
        private readonly Func<T> _getData;

        public ThriftyKangooSimpleCache(Func<T> getData, T defValue = null, TimeSpan? actualTime = null) : base (defValue ?? new T(), actualTime) {
            _getData = getData;
        }

        public T Object() {
            return this[false];
        }

        protected override T GetVal(bool key) {
            return _getData();
        }
    }


    public abstract class ThriftyKangarooCache<K, V> where V : class {
        private readonly TimeSpan _keyActualTime;
        private readonly V _defValue;
        /// <summary>
        /// Объект для логирования ошибок и отладочной информации.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger("ThriftyKangaroo");

        private readonly Dictionary<K, KangooCacheElement<V>> _cache;
        private readonly HashSet<K> _requestedKeys; 

        protected ThriftyKangarooCache(V defValue, TimeSpan? keyActualTime = null) {
            _keyActualTime = keyActualTime ?? new TimeSpan(8, 0, 0);
            _defValue = defValue;
            _cache = new Dictionary<K, KangooCacheElement<V>>();
            _requestedKeys = new HashSet<K>();
        } 

        public V this[K key] {
            get {
                KangooCacheElement<V> cacheElement;
                if (!_cache.TryGetValue(key, out cacheElement) || cacheElement.LastActualDate < DateTime.UtcNow || cacheElement.Element == _defValue) {
                    cacheElement = cacheElement ?? new KangooCacheElement<V> {
                        Element = _defValue,
                        LastActualDate = DateTime.UtcNow
                    };

                    if (!_requestedKeys.Contains(key)) {
                        if (ConfigHelper.TestMode) {
                            return GetVal(key);
                        }
                        lock (_requestedKeys) {
                            if (_requestedKeys.Contains(key)) {
                                return cacheElement.Element;
                            }
                            _requestedKeys.Add(key);
                        }
                        KangarooFillmentRule.AddUpdateAction(() => {
                            try {
                                var data = GetVal(key);
                                lock (_cache) {
                                    _cache[key] = new KangooCacheElement<V> {
                                        Element = data,
                                        LastActualDate = DateTime.UtcNow.Add(_keyActualTime)
                                    };
                                }
                                PostProcessResult(data);
                            } catch (Exception ex) {
                                _logger.Error(ex);
                            }
                            lock (_requestedKeys) {
                                if (_requestedKeys.Contains(key)) {
                                    _requestedKeys.Remove(key);
                                }
                            }
                        });
                    }
                }
                return cacheElement.Element;
            }
        }

        public void Clear() {
            lock (_cache) {
                _cache.Clear();
            }
        }

        protected virtual void PostProcessResult(V val) {
            
        }

        protected abstract V GetVal(K key);
    }
}
