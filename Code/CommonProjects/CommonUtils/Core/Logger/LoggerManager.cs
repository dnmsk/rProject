using System.Collections.Generic;

namespace CommonUtils.Core.Logger {
    public class LoggerManager {
        private static readonly Dictionary<string, LoggerWrapper> _nLoggers = new Dictionary<string, LoggerWrapper>();

        /// <summary>
        /// Возвращает соответствующий логгер
        /// </summary>
        /// <param name="system">Имя логгера</param>
        /// <returns>Логгер</returns>
        public static LoggerWrapper GetLogger(string system) {
            return GetNLogger(system);
        }
        
        private static LoggerWrapper GetNLogger(string system) {
            LoggerWrapper logger;
            if (!_nLoggers.TryGetValue(system, out logger)) {
                lock (_nLoggers) {
                    if (!_nLoggers.TryGetValue(system, out logger)) {
                        logger = new LoggerWrapper(system);
                        _nLoggers.Add(system, logger);
                    }
                }
            }
            return logger;
        }
    }
}