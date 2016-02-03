using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using CommonUtils.Core.Logger;

namespace CommonUtils.ExtendedTypes {
    public class SafeInvokerBase {
        private readonly static Dictionary<string, object> _lockersByMethodName = new Dictionary<string, object>();
        private readonly static Dictionary<string, object> _lockersByMethodForAccount = new Dictionary<string, object>(); 

        /// <summary>
        /// Логгер.
        /// </summary>
        private readonly LoggerWrapper _logger;

        protected SafeInvokerBase(LoggerWrapper logger) {
            _logger = logger;
        }
        
        /// <summary>
        /// Обертка для методов АПИ, для безопасного вызова.
        /// </summary>
        /// <param name="action">Метод.</param>
        protected void InvokeSafe(Action action) {
            try {
                action();
            } catch (Exception e) {
                LogError(action.Method.Name, action.Target, e);
            }
        }
        
        /// <summary>
        /// Обертка для методов АПИ, для безопасного вызова, одна функция выполняется в одно время.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
        /// <param name="action">Функция.</param>
        /// <param name="defValue">Значение по умолчанию, если было брошено исключение.</param>
        /// <returns>Возвращает значение функции, или <see cref="defValue"/> если было брошено исключение.</returns>
        protected T InvokeSafeSingleCall<T>(Func<T> action, T defValue = default(T)) {
            var methodName = action.Method.Name;

            object myLocker;
            lock (_lockersByMethodName) {
                if (!_lockersByMethodName.TryGetValue(methodName, out myLocker)) {
                    myLocker = new object();
                    _lockersByMethodName[methodName] = myLocker;
                }
            }

            var result = ExecuteAction(action, myLocker, defValue);
            lock (_lockersByMethodName) {
                if (_lockersByMethodName.ContainsKey(methodName)) {
                    _lockersByMethodName.Remove(methodName);
                }
            }
            return result;
        }

        /// <summary>
        /// Обертка для методов АПИ, для безопасного вызова, одна функция выполняется в одно время.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
        /// <param name="accountID">Аккаунт для которого лочим</param>
        /// <param name="action">Функция.</param>
        /// <param name="defValue">Значение по умолчанию, если было брошено исключение.</param>
        /// <returns>Возвращает значение функции, или <see cref="defValue"/> если было брошено исключение.</returns>
        protected T InvokeSafeSingleCallForAccount<T>(long accountID, Func<T> action, T defValue = default(T)) {
            var methodName = action.Method.Name;
            var lockKey = string.Format("{0}.{1}", accountID, methodName);

            object myLocker;
            lock (_lockersByMethodForAccount) {
                if (!_lockersByMethodForAccount.TryGetValue(lockKey, out myLocker)) {
                    myLocker = new object();
                    _lockersByMethodForAccount[lockKey] = myLocker;
                }
            }
            var result = ExecuteAction(action, myLocker, defValue);
            lock (_lockersByMethodForAccount) {
                if (_lockersByMethodForAccount.ContainsKey(lockKey)) {
                    _lockersByMethodForAccount.Remove(lockKey);
                }
            }
            return result;
        }

        /// <summary>
        /// Обертка для методов АПИ, для безопасного вызова.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
        /// <param name="action">Функция.</param>
        /// <param name="defValue">Значение по умолчанию, если было брошено исключение.</param>
        /// <returns>Возвращает значение функции, или <see cref="defValue"/> если было брошено исключение.</returns>
        protected T InvokeSafe<T>(Func<T> action, T defValue = default(T)) {
            return ExecuteAction(action, new object(), defValue);
        }

        private T ExecuteAction<T>(Func<T> action, object myLocker, T result) {
            try {
                lock (myLocker) {
                    result = action();
                }
            } catch (Exception e) {
                LogError(action.Method.Name, action.Target, e);
            }
            return result;
        }

        private void LogError(string actionMethod, object parsObj, Exception e) {
            string formatString = null;
            try {
                var parsStr = parsObj != null
                    ? new JavaScriptSerializer {RecursionLimit = 200}.Serialize(parsObj)
                    : "empty";
                formatString = string.Format("Ошибка при вызове метода {0}. Параметры: {1} {2}{3}", actionMethod, parsStr, Environment.NewLine, e);
            } catch (Exception ex) {
                formatString = string.Format("Ошибка при вызове метода {0}. Исключение сериализации: {1} {2}{3}", actionMethod, ex, Environment.NewLine, e);
            } finally {
                _logger.Error(formatString);
            }
        }
    }
}
