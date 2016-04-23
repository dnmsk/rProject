using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using CommonUtils.Code;
using CommonUtils.Code.WebRequestData;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Interfaces;

namespace Project_B.CodeServerSide.BrokerProvider.Common {
    public class WebRequestWrapper : IQueryableWrapper {
        private static int _tries = 2;

        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(WebRequestWrapper).FullName);
        public WebRequestHelper QuerySender { get; }

        public WebRequestWrapper(WebRequestHelper webRequestHelper) {
            QuerySender = webRequestHelper;
        }

        public WebRequestHelper RequestHelper => QuerySender;

        public string LoadPage(string url, List<string> postData = null, string contentType = "application/x-www-form-urlencoded") {
            for (var i = 0; i < _tries; i++) {
                try {
                    string post = null;
                    if (postData != null) {
                        post = postData.StrJoin("&");
                    }
                    var loadResult = QuerySender.GetContent(url, post, contentType);
                    if (loadResult.Item1 != HttpStatusCode.OK) {
                        _logger.Error("status = " + loadResult.Item1);
                    }
                    return loadResult.Item2;
                } catch (Exception ex) {
                    _logger.Error("url: {0} \r\n" + ex, url);
                }
                Thread.Sleep(5 * 1000);
            }
            return null;
        }

        public void SetProxy(string proxy) {
            QuerySender.SetParam(WebRequestParamType.ProxyString, new WebRequestParamString(proxy));
        }

        public void SetCookies(string domain, string[] cookies) {
            if (domain.IsNullOrWhiteSpace() || cookies == default(string[])) {
                return;
            }
            var cookieContainer = QuerySender.GetParam<CookieContainer>(WebRequestParamType.CookieContainer);
            foreach (var cookie in cookies) {
                var splittedCookie = cookie.Split('=');
                if (splittedCookie.Length != 2) {
                    _logger.Error("Cookies " + cookie);
                    continue;
                }
                cookieContainer.Add(new Cookie(splittedCookie[0], splittedCookie[1], "/", "." + domain) {
                    Expires = DateTime.UtcNow.AddYears(1)
                });
            }
        }

        public void ProcessConfig(BrokerConfiguration currentConfiguration) {}

        public object Clone() {
            return new WebRequestWrapper((WebRequestHelper) QuerySender.Clone());
        }
    }
}