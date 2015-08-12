using System;
using System.Collections.Generic;

namespace CommonUtils.Core.Logger {
    public interface ILogActionRule {
        /// <summary>
        /// На какую фичу повешено правило
        /// </summary>
        List<Enum> ActionsToProcess { get; }

        /// <summary>
        /// Обработка действия
        /// </summary>
        /// <param name="feature">фича</param>
        /// <param name="guestID">кто выполнял действие</param>
        /// <param name="objectId">параметр логирования</param>
        /// <param name="object2Id">параметр логирования #2, если одного не хватило</param>
        /// <param name="additionalParams"></param>
        void ProcessAction(Enum feature, long guestID, long? objectId = null, long? object2Id = null, Dictionary<string, string> additionalParams = null);
    }
}