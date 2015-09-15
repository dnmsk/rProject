using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using MainLogic.WebFiles;

namespace Spywords_Project.Code.Algorithms {
    public abstract class AlgoBase {
        protected const RegexOptions REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.IgnoreCase;

        /// <summary>
        /// Логгер.
        /// </summary>
        protected static readonly LoggerWrapper Logger = LoggerManager.GetLogger(typeof(AlgoBase).FullName);
        protected static readonly SpywordsQueryWrapper SpywordsQueryWrapper;
        private static readonly WebRequestHelper _webRequestHelper;

        static AlgoBase() {
            var cookiesInit = new CookieContainer();
            var configurationProperty = SiteConfiguration
                .GetConfigurationProperty<Dictionary<string, string>>("Cookies");
            if (configurationProperty != null) {
                configurationProperty
                    .Each(pair => {
                        cookiesInit.Add(new Cookie(pair.Key, pair.Value, "/", ".spywords.ru") {
                            Expires = DateTime.Now.AddYears(1)
                        });
                    });
            }
            _webRequestHelper = new WebRequestHelper(
                SiteConfiguration.GetConfigurationProperty("UserAgent"),
                cookiesInit,
                (userAgent, cookies) => {
                    var res = new Dictionary<string, string>();
                    foreach (Cookie c in cookies.GetCookies(new Uri("http://spywords.ru"))) {
                        res[c.Name] = c.Value;
                    }
                    SiteConfiguration.ModifyConfigurationProperty(new Dictionary<string, string> {
                        {"UserAgent", userAgent },
                        {"Cookies", new JavaScriptSerializer().Serialize(res) }
                    });
                }
            );
            SpywordsQueryWrapper = new SpywordsQueryWrapper(
                SiteConfiguration.GetConfigurationProperty("spywordsLogin"),
                SiteConfiguration.GetConfigurationProperty("spywordsPassword"),
                _webRequestHelper,
                new TimeSpan(0, 0, 1)
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

        protected abstract void DoAction();
    }
}