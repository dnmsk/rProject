using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.Web.Script.Serialization;
using CommonUtils.Core.Logger;

namespace CommonUtils.ExtendedTypes {
    public abstract class SafeInvokerBase : ServiceHostFactory {
        private readonly Dictionary<string, object> _lockersByMethodName = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _lockersByMethodForAccount = new Dictionary<string, object>(); 

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
        protected T InvokeSafeSingleCall<T>(Func<T> action, T defValue) {
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
        protected T InvokeSafeSingleCallForAccount<T>(long accountID, Func<T> action, T defValue) {
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
        protected T InvokeSafe<T>(Func<T> action, T defValue) {
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
            var parsStr = parsObj != null
                ? new JavaScriptSerializer().Serialize(parsObj)
                : "empty";
            _logger.Error(string.Format("Ошибка при вызове метода {0}. Параметры: {1}", actionMethod, parsStr), e);
        }

        /// <summary>
        /// Обертка для методов АПИ, для безопасного вызова.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
        /// <param name="action">Метод.</param>
        /// <returns>Возвращает значение функции, или null если было брошено исключение.</returns>
        protected T InvokeSafe<T>(Func<T> action) where T : class {
            return InvokeSafe(action, null);
        }

        /// <summary>
        /// Обертка для методов АПИ, для безопасного вызова, одна функция выполняется в одно время.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
        /// <param name="action">Метод.</param>
        /// <returns>Возвращает значение функции, или null если было брошено исключение.</returns>
        protected T InvokeSafeSingleCall<T>(Func<T> action) where T : class {
            return InvokeSafeSingleCall(action, null);
        }

        /// <summary>
        /// Обертка для методов АПИ, для безопасного вызова, одна функция выполняется в одно время.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
        /// <param name="accountID">Аккаунт для которого лочим</param>
        /// <param name="action">Метод.</param>
        /// <returns>Возвращает значение функции, или null если было брошено исключение.</returns>
        protected T InvokeSafeSingleCallForAccount<T>(long accountID, Func<T> action) where T : class {
            return InvokeSafeSingleCallForAccount(accountID, action, null);
        }

        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses) {
            var host = new ServiceHost(GetType(), baseAddresses);
            var iface = GetType().GetInterfaces().First();
            var attr = (ServiceContractAttribute)Attribute.GetCustomAttribute(iface, typeof(ServiceContractAttribute));
            if (attr != null) {
                host.AddServiceEndpoint(iface, new WSHttpBinding(), "");
            }
            ServiceMetadataBehavior metadataBehavior;
            var currentDehaviors = host.Description.Behaviors;
            if (currentDehaviors[typeof(ServiceMetadataBehavior)] == null) {
                metadataBehavior = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(metadataBehavior);
            } else {
                metadataBehavior = (ServiceMetadataBehavior) currentDehaviors[typeof(ServiceMetadataBehavior)];
            }
            metadataBehavior.HttpGetEnabled = true;

            return host;
        }
    }
}
