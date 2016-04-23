using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths;
using CommonUtils.WatchfulSloths.KangooCache;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Interfaces;
using VirtualBrowser;

namespace Project_B.CodeServerSide.BrokerProvider.Common {
    public class BrowserWrapper : WebRequestWrapper, IQueryableWrapper {
        private Browser _browserSender;
        private readonly BrowserSettings _browserSettings;
        private readonly KangarooCache<string, BrowserTab> _currentTabs;
        private readonly List<string> _managedByBrowserUrls = new List<string>();

        public BrowserWrapper(WebRequestHelper webRequestHelper, string proxy) : base(webRequestHelper) {
            _browserSender = BrowserCreator.BuildBrowser(_browserSettings = new BrowserSettings {
                WindowHeight = 1080,
                WindowWidth = 1920,
                Proxy = proxy
            });
            _currentTabs = new KangarooCache<string, BrowserTab>(WatchfulSloth.Instance,
                s => {
                    var tab = _browserSender.GetNewTab();
                    tab.OpenPage(s);
                    return tab;
                }, TimeSpan.FromHours(1));
        }

        public new void SetCookies(string domain, string[] cookies) {
            base.SetCookies(domain, cookies);
            _browserSender.SetCookie(domain.CutHttpHttps().CutWww(), cookies);
        }

        public new string LoadPage(string url, List<string> postData = null, string contentType = "application/x-www-form-urlencoded") {
            return _managedByBrowserUrls.Any(u => url.Contains(u, StringComparison.InvariantCultureIgnoreCase))
                ? _currentTabs[url].PageContent() 
                : base.LoadPage(url, postData, contentType);
        }

        public new void ProcessConfig(BrokerConfiguration currentConfiguration) {
            _managedByBrowserUrls.AddRange(currentConfiguration.StringArray[SectionName.ArrayManagedByBrowserUrl]?.Select(u => u.ToLower()) ?? new string[0]);
        }

        public new object Clone() {
            return new BrowserWrapper((WebRequestHelper) RequestHelper.Clone(), _browserSettings.Proxy) {
                _browserSender = _browserSender
            };
        }
    }
}