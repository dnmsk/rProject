using System.Collections.Generic;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.Configuration {
    public class SimpleConfiguration<K, V> : Dictionary<K, V> {
        public new V this[K key] {
            get {
                V val;
                return TryGetValue(key, out val) ? val : default (V);
            }
            set { base[key] = value; }
        }
    }
}