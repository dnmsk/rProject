using System;

namespace CommonUtils.WatchfulSloths.KangooCache {
    internal class KangooCacheElement<T> {
        public DateTime LastActualDate { get; set; }
        public T Element { get; set; }
    }
}
