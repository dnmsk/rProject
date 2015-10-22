using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.Code.BrokerProvider;
using Project_B.Code.BrokerProvider.Configuration;
using Project_B.Code.Enums;

namespace Project_B.Code {
    public class BookPage : Singleton<BookPage> {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BookPage).FullName);
        private readonly Dictionary<BrokerType, IOddsProvider> _oddsProviders = new Dictionary<BrokerType, IOddsProvider>(); 
        private readonly Dictionary<BrokerType, IResultHistoryProvider> _resultsProviders = new Dictionary<BrokerType, IResultHistoryProvider>(); 

        public BookPage() {
            var currentBrokerProviderTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => {
                    try {
                        var type = assembly.GetTypes();
                        return type;
                    }
                    catch (ReflectionTypeLoadException ex) {
                        _logger.Error(ex);
                        foreach (var loaderException in ex.LoaderExceptions) {
                            _logger.Error(loaderException);
                        }
                    }
                    catch (Exception ex) {
                        _logger.Error(ex);
                    }
                    return null;
                })
                .Where(type => type != null && typeof(BrokerBase).IsAssignableFrom(type) && !type.IsAbstract)
                .Distinct();

            var globalConfiguration = ConfigurationContainer.Instance.BrokerConfiguration[BrokerType.Default];
            var webRequestHelper = new WebRequestHelper(globalConfiguration.StringSimple[SectionName.SimpleStringUserAgent]) {
                Proxy = globalConfiguration.StringArray[SectionName.ArrayProxy].First()
            };
            var availableBrokerType = new List<BrokerType>();
            foreach (var brokerProviderType in currentBrokerProviderTypes) {
                var instance = (BrokerBase)Activator.CreateInstance(brokerProviderType, webRequestHelper);
                if (typeof (IOddsProvider).IsAssignableFrom(brokerProviderType)) {
                    _oddsProviders.Add(instance.BrokerType, (IOddsProvider)instance);
                } else if (typeof (IResultHistoryProvider).IsAssignableFrom(brokerProviderType)) {
                    _resultsProviders.Add(instance.BrokerType, (IResultHistoryProvider)instance);
                }
                if (!availableBrokerType.Contains(instance.BrokerType)) {
                    availableBrokerType.Add(instance.BrokerType);
                }
            }
            foreach (var brokerType in availableBrokerType) {
                var brokerConfiguration = ConfigurationContainer.Instance.BrokerConfiguration[brokerType];
                var domain = brokerConfiguration.StringSimple[SectionName.Domain];
                var cookies = brokerConfiguration.StringArray[SectionName.ArrayCookie];
                if (domain != default(string) && cookies != default(string[])) {
                    foreach (var cookie in cookies) {
                        var splittedCookie = cookie.Split('=');
                        if (splittedCookie.Length != 2) {
                            _logger.Error("Cookies " + cookie);
                            continue;
                        }
                        webRequestHelper.Cookies.Add(new Cookie(splittedCookie[0], splittedCookie[1], "/", "." + domain) {
                            Expires = DateTime.Now.AddYears(1)
                        });
                    }
                }
            }
        }

        public IOddsProvider GetOddsProvider(BrokerType brokerType) {
            return _oddsProviders[brokerType];
        }

        public IResultHistoryProvider GetHistoryProvider(BrokerType brokerType) {
            return brokerType == BrokerType.Unknown ? _resultsProviders.Values.First() : _resultsProviders[brokerType];
        }
    }
}