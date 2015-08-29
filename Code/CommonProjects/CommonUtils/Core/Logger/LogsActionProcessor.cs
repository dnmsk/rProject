using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;

namespace CommonUtils.Core.Logger {
    public class LogsActionProcessor : Singleton<LogsActionProcessor> {
        /// <summary>
        /// Набор правил
        /// </summary>
        private readonly Dictionary<Enum, ILogActionRule> _rulesMap;

        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(LogsActionProcessor).FullName);

        /// <summary>
        /// Конструктор
        /// </summary>
        public LogsActionProcessor() {
            _rulesMap = new Dictionary<Enum, ILogActionRule>();
            return;
            var type = typeof(ILogActionRule);
            var rulesList = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => type.IsAssignableFrom(t) && t != type)
                    .ToList();

            foreach (var rule in rulesList) {
                try {
                    var item = (ILogActionRule)Activator.CreateInstance((Type) rule);
                    foreach (var action in item.ActionsToProcess) {
                        _rulesMap.Add(action, item);
                    }
                } catch (Exception ex) {
                    _logger.Info("Ошибка при создании правила обработки действий логов: {0} {1}", rule.Name, ex);
                }
            }
        }

        /// <summary>
        /// Обрабатывает действие пользователя
        /// </summary>
        /// <param name="guestID">кто выполнял действие</param>
        /// <param name="action">айдишник действия пользователя</param>
        /// <param name="objectId">параметр логирования</param>
        /// <param name="object2Id">параметр логирования #2, если одного не хватило</param>
        /// <param name="additionalParams"></param>
        public void ProcessActionID(long guestID, Enum action, long? objectId = null, long? object2Id = null, Dictionary<string, string> additionalParams = null) {
            ILogActionRule rule;
            if (_rulesMap.TryGetValue(action, out rule)) {
                rule.ProcessAction(action, guestID, objectId, object2Id, additionalParams);
            }
        }
    }
}