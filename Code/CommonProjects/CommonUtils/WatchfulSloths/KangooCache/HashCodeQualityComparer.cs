using System.Collections;
using System.Collections.Generic;

namespace CommonUtils.WatchfulSloths.KangooCache {
    public class HashCodeQualityComparer<K> : IEqualityComparer<K> {
        public bool Equals(K x, K y) {
            if (x == null || y == null) {
                return false;
            }
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(K obj) {
            return obj?.GetHashCode() ?? default(int);
        }
    }
}
