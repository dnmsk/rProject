using System;
using System.Collections.Generic;

namespace CommonUtils.Core.Logger {
    public class UserActionLogger {
        /// <summary>
        /// Логгер для протоколирования действий пользователей.
        /// </summary>
        private static readonly LoggerWrapper _actionLogger = LoggerManager.GetLogger("UserFeatureLog");

        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(UserActionLogger).FullName);
        
        /// <summary>
        /// Логирование действия пользователя.
        /// Действия из контроллеров в ВЕБЕ ЛОГГИРВОАТЬ ЧЕРЕЗ МЕТОД LogAction() базового контроллера
        /// </summary>
        /// <param name="guestId">guestId аккаунта.</param>
        /// <param name="feature">Фича.</param>
        /// <param name="objectId">параметр логирования</param>
        /// <param name="additionalParams"></param>
        public static void Log(long guestId, Enum feature, int? objectId = null, Dictionary<string, string> additionalParams = null) {
            LogInternal(_actionLogger, guestId, feature, objectId, additionalParams ?? new Dictionary<string, string>());
        }

        private static void LogInternal(LoggerWrapper logger, long guestId, Enum feature, int? objectId, Dictionary<string, string> additionalParams) {
            try {
                if (guestId <= 0) {
                    return;
                }
                logger.Info("{0};{1};{2}", guestId, Convert.ToInt32(feature), objectId.HasValue ? objectId.Value.ToString() : "NULL");
                
                LogsActionProcessor.Instance.ProcessActionID(guestId, feature, objectId, null, additionalParams);
            } catch (Exception e) {
                _logger.Error(e);
            }
        }
    }
}