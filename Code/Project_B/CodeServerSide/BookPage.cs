using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.BrokerProvider;
using Project_B.CodeServerSide.BrokerProvider.Common;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Interfaces;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide {
    public class BookPage : Singleton<BookPage> {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BookPage).FullName);
        private readonly Dictionary<BrokerType, IBrokerBase> _brokerProviders = new Dictionary<BrokerType, IBrokerBase>(); 

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
                .Where(type => type != null && typeof(IBrokerBase).IsAssignableFrom(type) && !type.IsAbstract)
                .Distinct();

            var globalConfiguration = ConfigurationContainer.Instance.BrokerConfiguration[BrokerType.Default];
            var proxy = globalConfiguration.StringArray[SectionName.ArrayProxy].FirstOrDefault();
            var typeInitilizers = new IQueryableWrapper[] {
                new WebRequestWrapper(new WebRequestHelper(globalConfiguration.StringSimple[SectionName.SimpleStringUserAgent])),
                new BrowserWrapper(new WebRequestHelper(globalConfiguration.StringSimple[SectionName.SimpleStringUserAgent]), proxy)
            }
                .ToDictionary(item => item.GetType(), item => item);

            foreach (var brokerProviderType in currentBrokerProviderTypes) {
                try {
                    IQueryableWrapper queryableWrapper = null;
                    brokerProviderType.BaseType?.GenericTypeArguments.Each(type => {
                        typeInitilizers.TryGetValue(type, out queryableWrapper);
                    });
                    if (queryableWrapper == null) {
                        continue;
                    }
                    var instance = (IBrokerBase) Activator.CreateInstance(brokerProviderType, (queryableWrapper = (IQueryableWrapper) queryableWrapper.Clone()));
                    if (instance.BrokerType == BrokerType.Default) {
                        continue;
                    }
                    queryableWrapper.SetProxy(proxy);
                    var brokerConfiguration = ConfigurationContainer.Instance.BrokerConfiguration[instance.BrokerType];
                     queryableWrapper.SetCookies(brokerConfiguration.StringSimple[SectionName.Domain], brokerConfiguration.StringArray[SectionName.ArrayCookie]);
                    _brokerProviders.Add(instance.BrokerType, instance);
                } catch (Exception ex) {
                    _logger.Error(ex);
                }
            }
        }
        
        public IBrokerBase GetBrokerProvider(BrokerType brokerType) {
            return brokerType == BrokerType.Default ? null : _brokerProviders[brokerType];
        }
    }
}