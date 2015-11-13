using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CommonUtils.WatchfulSloths.SlothMoveRules;

namespace CommonUtils.WatchfulSloths.KangooCache {
    public class MultipleKangooCache<K, V> : SimpleKangooCache<K, V> {
        private readonly Action<IDictionary<K, V>> _filler;
        public MultipleKangooCache(IWatchfulSloth sloth, Action<IDictionary<K, V>> filler, TimeSpan? keyActualTime = null) : base(null) {
            _filler = filler;
            InitCache();
            if (sloth != null) {
                sloth.SetMove(new SlothMoveByTimeSingle<object>(InitCache, keyActualTime ?? TimeSpan.FromMinutes(30), null));
            }
        }

        private object InitCache() {
            var newCache = new ConcurrentDictionary<K, V>();
            _filler(newCache);
            ReplaceCacheContainer(newCache);
            return null;
        }
    }
}
