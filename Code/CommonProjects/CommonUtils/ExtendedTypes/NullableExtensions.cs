using System.Collections.Generic;
using System.Linq;

namespace CommonUtils.ExtendedTypes {
    public static class NullableExtensions {
        public static bool SafeAny<T>(this IEnumerable<T> items) {
            return items != null && items.Any();
        }
    }
}
