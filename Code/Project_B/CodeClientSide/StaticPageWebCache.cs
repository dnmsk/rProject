using System;
using System.Collections.Generic;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide {
    public class StaticPageWebCache<K, V> where V : StaticPageTransport {
        private readonly Func<Dictionary<K, List<V>>> _dataGetter;
        private readonly Func<K, K> _normalizeKey;
        private Dictionary<K, Dictionary<LanguageType, V>> _dictionaryCache;
        public StaticPageWebCache(Func<Dictionary<K, List<V>>> dataGetter, Func<K, K> normalizeKey) {
            _dataGetter = dataGetter;
            _normalizeKey = normalizeKey;
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(RebuildCache, new TimeSpan(0, 1, 0), null));
            RebuildCache();
        }

        private object RebuildCache() {
            var itemsList = _dataGetter();
            var newCache = new Dictionary<K, Dictionary<LanguageType, V>>();
            foreach (var itemList in itemsList) {
                Dictionary<LanguageType, V> languageItemCache;
                var key = _normalizeKey(itemList.Key);
                if (!newCache.TryGetValue(key, out languageItemCache)) {
                    languageItemCache = new Dictionary<LanguageType, V>();
                    newCache[key] = languageItemCache;
                }
                foreach (var item in itemList.Value) {
                    languageItemCache[item.Languagetype] = item;
                }
            }
            _dictionaryCache = newCache;
            return null;
        }

        public V GetPage(LanguageType languageType, K pageKey) {
            var key = _normalizeKey(pageKey);
            Dictionary<LanguageType, V> languageItemCache;
            if (_dictionaryCache.TryGetValue(key, out languageItemCache)) {
                V item;
                if (languageItemCache.TryGetValue(languageType, out item) ||
                    languageItemCache.TryGetValue(LanguageTypeHelper.DefaultLanguageTypeSetted, out item)) {
                    return item;
                }
            }
            return null;
        }
    } 
}