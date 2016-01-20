using System.Collections.Generic;

namespace CommonUtils.ExtendedTypes {
    public static class DictionaryExtension {
        public static V TryGetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key, bool createEmpty = true) where V : new() {
            V val = dict != null && dict.TryGetValue(key, out val) ? val : (createEmpty ? new V() : default(V));
            return val;
        }

        public static V TryGetValueOrDefaultStruct<K, V>(this IDictionary<K, V> dict, K key) where V : struct {
            V val = dict != null && dict.TryGetValue(key, out val) ? val : default (V);
            return val;
        }
    }
}
