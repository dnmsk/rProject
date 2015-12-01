using System.Collections.Generic;

namespace CommonUtils.ExtendedTypes {
    public static class DictionaryExtension {
        public static V TryGetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key) where V : new() {
            V val = dict != null && dict.TryGetValue(key, out val) ? val : new V();
            return val;
        }

        public static V TryGetValueOrDefaultStruct<K, V>(this IDictionary<K, V> dict, K key) {
            V val = dict != null && dict.TryGetValue(key, out val) ? val : default (V);
            return val;
        }
    }
}
