using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.BrokerProvider;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide {
    public class BookPage : Singleton<BookPage> {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BookPage).FullName);
        private readonly Dictionary<BrokerType, BrokerBase> _brokerProviders = new Dictionary<BrokerType, BrokerBase>(); 

        public BookPage() {
            var currentBrokerProviderTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => {
                    try {
                        var type = assembly.GetTypes();
                        return type;
                    } catch (ReflectionTypeLoadException ex) {
                        _logger.Error(ex);
                        foreach (var loaderException in ex.LoaderExceptions) {
                            _logger.Error(loaderException);
                        }
                    } catch (Exception ex) {
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
            foreach (var brokerProviderType in currentBrokerProviderTypes) {
                var instance = (BrokerBase)Activator.CreateInstance(brokerProviderType, webRequestHelper);
                _brokerProviders.Add(instance.BrokerType, instance);
            }
            foreach (var brokerType in _brokerProviders.Keys) {
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
                            Expires = DateTime.UtcNow.AddYears(1)
                        });
                    }
                }
            }
        }
        
        public BrokerBase GetBrokerProvider(BrokerType brokerType) {
            return brokerType == BrokerType.Default ? _brokerProviders.Values.First() : _brokerProviders[brokerType];
        }
    }
}