using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using MainLogic.WebFiles;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Algorithms {
    public abstract class AlgoBase {
        protected const RegexOptions REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.IgnoreCase;

        /// <summary>
        /// Логгер.
        /// </summary>
        protected static readonly LoggerWrapper Logger = LoggerManager.GetLogger(typeof(AlgoBase).FullName);

        public static CollectionIdentity CollectionIdentity = CollectionIdentity.April2016;
        protected static readonly SpywordsQueryWrapper SpywordsQueryWrapper;
        private static readonly WebRequestHelper _webRequestHelper;

        static AlgoBase() {
            _webRequestHelper = BuildRequestHelper("CookiesSpyword", "spywords.ru", TimeSpan.FromSeconds(1));
            SpywordsQueryWrapper = new SpywordsQueryWrapper(
                SiteConfiguration.GetConfigurationProperty("spywordsLogin"),
                SiteConfiguration.GetConfigurationProperty("spywordsPassword"),
                _webRequestHelper
            );
        }

        public static void PushConfiguration() {
            _webRequestHelper.PushToConfigurationFile();
        }

        protected AlgoBase(TimeSpan wakeupInterval) {
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(() => {
                DoAction();
                return null;
            }, wakeupInterval, null));
        }

        protected static WebRequestHelper BuildRequestHelper(string cookiesConfigRow, string targetCookiesDomain, TimeSpan? requestDealy = null) {
            var cookiesInit = new CookieContainer();
            var configurationProperty = SiteConfiguration
                .GetConfigurationProperty<Dictionary<string, string>>(cookiesConfigRow);
            if (configurationProperty != null) {
                configurationProperty
                    .Each(pair => {
                        cookiesInit.Add(new Cookie(pair.Key, pair.Value, "/", "." + targetCookiesDomain) {
                            Expires = DateTime.UtcNow.AddYears(1)
                        });
                    });
            }
            return new WebRequestHelper(
                SiteConfiguration.GetConfigurationProperty("UserAgent"),
                cookiesInit,
                (userAgent, cookies) => {
                    var res = new Dictionary<string, string>();
                    foreach (Cookie c in cookies.GetCookies(new Uri("http://" + cookiesConfigRow))) {
                        res[c.Name] = c.Value;
                    }
                    SiteConfiguration.ModifyConfigurationProperty(new Dictionary<string, string> {
                        {"UserAgent", userAgent },
                        {cookiesConfigRow, new JavaScriptSerializer().Serialize(res) }
                    });
                }
            ) {
                MinRequestDelay = requestDealy
            };
        }

        private static readonly object _domainLocker = new object();
        public static DomainEntity GetDomainEntity(string domain) {
            var d = DomainExtension.DePunycodeDomain(domain.ToLower());
            lock (_domainLocker) {
                var domainEntity = DomainEntity.DataSource
                                               .WhereEquals(DomainEntity.Fields.Domain, d)
                                               .WhereEquals(DomainEntity.Fields.CollectionIdentity, (short) CollectionIdentity)
                                               .First();
                if (domainEntity == null) {
                    domainEntity = new DomainEntity {
                        Datecreated = DateTime.UtcNow,
                        Domain = d,
                        Status = DomainStatus.Default,
                        CollectionIdentity = CollectionIdentity
                    };
                    domainEntity.Save();
                }
                return domainEntity;
            }
        }
        
        protected static void CreateOrUpdateDomainPhrase(DomainEntity domainEntity, Phrase phrase, SearchEngine seType, SourceType sourceType) {
            var firstDomainPhrase = Domainphrase.DataSource
                                                .WhereEquals(Domainphrase.Fields.DomainID, domainEntity.ID)
                                                .WhereEquals(Domainphrase.Fields.PhraseID, phrase.ID)
                                                .WhereEquals(Domainphrase.Fields.CollectionIdentity, (short) CollectionIdentity)
                                                .First();
            if (firstDomainPhrase == null) {
                var domainphrase = new Domainphrase {
                    DomainID = domainEntity.ID,
                    PhraseID = phrase.ID,
                    SourceType = sourceType,
                    SE = seType,
                    CollectionIdentity = CollectionIdentity
                };
                domainphrase.Save();
            } else {
                firstDomainPhrase.SourceType |= sourceType;
                firstDomainPhrase.SE |= seType;
                firstDomainPhrase.Save();
            }
        }

        protected abstract void DoAction();
    }
}