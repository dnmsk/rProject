using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.WatchfulSloths.SlothMoveRules;

namespace CommonUtils.WatchfulSloths.KangooCache {
    /// <summary>
    /// Абстрактный кеш объектов, которые должны быть получены в момент обращения.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class KangarooCache<K, V> : SimpleKangooCache<K, KangooCacheElement<V>> {
        private readonly TimeSpan _keyActualTime;

        /// <summary>
        /// Объект для логирования ошибок и отладочной информации.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger("KangarooCache");

        public KangarooCache(IWatchfulSloth sloth, Func<K, V> valueGetter, TimeSpan? keyActualTime = null) 
                                                                            : base (
                                                                                k => new KangooCacheElement<V> {
                                                                                    Element = valueGetter(k),
                                                                                    LastActualDate = DateTime.UtcNow.Add(keyActualTime ?? TimeSpan.FromMinutes(30))
                                                                            }) {
            _keyActualTime = keyActualTime ?? TimeSpan.FromMinutes(30);
            if (sloth != null) {
                sloth.SetMove(new SlothMoveByTimeSingle<object>(SelfClean, new TimeSpan(0, 5, 0), null));
            } else {
                _logger.Warn("Ахтунг!!! Тип {0} будет работать без самоочистки", GetType());
            }
        }

        /// <summary>
        /// Получение элемента из кэша. Если элемента нет - он будет получен по правилу получения этого ключа.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new V this[K key] {
            get {
                var kangooCacheElement = base[key];
                return kangooCacheElement != null ? kangooCacheElement.Element : default (V);
            }
            set {
                base[key] = new KangooCacheElement<V> {
                    Element = value,
                    LastActualDate = DateTime.UtcNow.Add(_keyActualTime)
                };
            }
        }

        protected override IEqualityComparer<K> GetComparer() {
            return null;
        }

        protected override bool NeedUpdate(KangooCacheElement<V> inCache) {
            return inCache == null || inCache.LastActualDate < DateTime.UtcNow || inCache.Element.Equals(default(V));
        }

        private object SelfClean() {
            ICollection<K> keys = Cache.Keys.ToArray();
            var now = DateTime.UtcNow;
            int cleanedKeys = 0;
            foreach (var key in keys) {
                try {
                    if (Cache[key].LastActualDate < now) {
                        KangooCacheElement<V> val;
                        Cache.TryRemove(key, out val);
                        if (val != null && val.Element != null && val.Element is IDisposable) {
                            ((IDisposable) val.Element).Dispose();
                        }
                        cleanedKeys++;
                    }
                } catch (Exception ex) {
                    _logger.Error(ex);
                }
            }
            return null;
        }

        /// <summary>
        /// Очистка кеша полностью
        /// </summary>
        public void Clear() {
            Cache.Clear();
        }
    }
}
