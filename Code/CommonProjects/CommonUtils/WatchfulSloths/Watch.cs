using System;

namespace CommonUtils.WatchfulSloths {
    /// <summary>
    /// Часы. Служат для определения текущего времени.
    /// </summary>
    public class Watch : IWatch {
        /// <summary>
        /// Возвращает текущее время
        /// </summary>
        /// <returns></returns>
        public DateTime Now() {
            return DateTime.UtcNow;
        }
    }
}