using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Interfaces;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Common {
    public class WrDnmskWrapper : WebRequestWrapper, IQueryableWrapper {
        private readonly List<string> _managedByBrowserUrls = new List<string>();
        private readonly List<string> _managedByBrowserPollUrls = new List<string>();

        protected static readonly JavaScriptSerializer JavaScriptSerializer = new JavaScriptSerializer {
            MaxJsonLength = 99999999,
            RecursionLimit = 9999
        };

        private enum WrType : short {
            BrowserPoll = 1,
            Browser = 2,
            Loader = 3
        }

        private string _targetUrl;
        private string _targetToken;

        private string TargetUrl {
            get {
                if (_targetUrl == null) {
                    var globalConfiguration = ConfigurationContainer.Instance.BrokerConfiguration[BrokerType.Default];
                    _targetUrl = globalConfiguration.StringSimple[SectionName.ArrayWrTargetUrl];
                }
                return _targetUrl;
            }
        }

        private string TargetToken {
            get {
                if (_targetToken == null) {
                    var globalConfiguration = ConfigurationContainer.Instance.BrokerConfiguration[BrokerType.Default];
                    _targetToken = globalConfiguration.StringSimple[SectionName.ArrayWrTargetToken];
                }
                return _targetToken;
            }
        }

        public WrDnmskWrapper(WebRequestHelper webRequestHelper) : base(webRequestHelper) { }

        public new void SetCookies(string domain, string[] cookies) {
            base.SetCookies(domain, cookies);
        }

        public new void SetProxy(string proxy) { }

        public new string LoadPage(string url, List<string> postData = null, string contentType = "application/x-www-form-urlencoded") {
            WrType wrType;
            if (_managedByBrowserPollUrls.Any(u => url.Contains(u, StringComparison.InvariantCultureIgnoreCase))) {
                wrType = WrType.BrowserPoll;
            } else if (_managedByBrowserUrls.Any(u => url.Contains(u, StringComparison.InvariantCultureIgnoreCase))) {
                wrType = WrType.Browser;
            } else {
                wrType = WrType.Loader;
            }
            for (var i = 0; i < 3; i++) {
                try {
                    var response = base.LoadPage(BuildRemoteUrl(url, wrType), postData, contentType);
                    var data = JavaScriptSerializer.Deserialize<Dictionary<string, string>>(response);
                    return data["content"];
                } catch(Exception ex) { }
            }
            return String.Empty;
        }

        public new void ProcessConfig(BrokerConfiguration currentConfiguration) {
            _managedByBrowserUrls.AddRange(currentConfiguration.StringArray[SectionName.ArrayManagedByBrowserUrl]?.Select(u => u.ToLower()) ?? new string[0]);
            _managedByBrowserPollUrls.AddRange(currentConfiguration.StringArray[SectionName.ArrayManagedByBrowserPollUrl]?.Select(u => u.ToLower()) ?? new string[0]);
        }

        private string BuildRemoteUrl(string baseUrl, WrType wrType) {
            string fragment;
            switch (wrType) {
                case WrType.BrowserPoll:
                    fragment = "long_poll";
                    break;
                case WrType.Browser:
                    fragment = "browser";
                    break;
                default:
                    fragment = "http";
                    break;
            }
            return string.Format(
                "{0}/{1}?token={2}&url={3}",
                TargetUrl,
                fragment,
                TargetToken,
                HttpUtility.UrlEncode(baseUrl)
            );
        }
        public new object Clone() {
            return new WrDnmskWrapper((WebRequestHelper)RequestHelper.Clone());
        }
    }
}