using System;

namespace CommonUtils.WatchfulSloths {
    /// <summary>
    /// Часы. Служат для определения текущего времени.
    /// </summary>
    public interface IWatch {
        /// <summary>
        /// Возвращает текущее время
        /// </summary>
        /// <returns></returns>
        DateTime Now();
    }
}
