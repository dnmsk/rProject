using System;
using System.Collections.Generic;
using System.Net;
using CommonUtils.ExtendedTypes;

namespace CommonUtils.Code.WebRequestData {
    public static class WebRequestStaticProcessor {
        public static HttpWebRequest ProcessRequestParams(HttpWebRequest httpWebRequest, IDictionary<WebRequestParamType, WebRequestParamBase> webRequestParams) {
            webRequestParams.Each(webRequestParam => {
                switch (webRequestParam.Key) {
                    case WebRequestParamType.CookieContainer:
                        httpWebRequest.CookieContainer = ConvertToTypeT<CookieContainer, WebRequestParamCookieContainer>(new CookieContainer(), webRequestParam.Value);
                        break;
                    case WebRequestParamType.AcceptString:
                        httpWebRequest.Accept = ConvertToTypeT<string, WebRequestParamString>(httpWebRequest.Accept, webRequestParam.Value);
                        break;
                    case WebRequestParamType.HeadersArrayKeyValue:
                        httpWebRequest.Headers = ConvertToTypeT<WebHeaderCollection, WebRequestParamWebHeaderCollection>(httpWebRequest.Headers, webRequestParam.Value,
                            (collection, value) => {
                                var webHeaderCollection = (WebHeaderCollection) value.Value;
                                foreach (string key in webHeaderCollection) {
                                    collection[key] = webHeaderCollection[key];
                                }
                            });
                        break;
                    case WebRequestParamType.KeepAliveBool:
                        httpWebRequest.KeepAlive = ConvertToTypeT<bool, WebRequestParamBool>(httpWebRequest.KeepAlive, webRequestParam.Value);
                        break;
                    case WebRequestParamType.TimeoutInt:
                        httpWebRequest.Timeout = ConvertToTypeT<int, WebRequestParamInt>(httpWebRequest.Timeout, webRequestParam.Value);
                        break;
                    case WebRequestParamType.ReadWriteTimeoutInt:
                        httpWebRequest.ReadWriteTimeout = ConvertToTypeT<int, WebRequestParamInt>(httpWebRequest.ReadWriteTimeout, webRequestParam.Value);
                        break;
                    case WebRequestParamType.UserAgentString:
                        httpWebRequest.UserAgent = ConvertToTypeT<string, WebRequestParamString>(httpWebRequest.UserAgent, webRequestParam.Value);
                        break;
                    case WebRequestParamType.AllowAutoRedirectBool:
                        httpWebRequest.AllowAutoRedirect = ConvertToTypeT<bool, WebRequestParamBool>(httpWebRequest.AllowAutoRedirect, webRequestParam.Value);
                        break;
                    case WebRequestParamType.ProxyString:
                        var proxy = ConvertToTypeT<string, WebRequestParamString>(null,  webRequestParam.Value);
                        if (!proxy.IsNullOrEmpty()) {
                            httpWebRequest.Proxy = new WebProxy(new Uri(proxy.StartsWith("http") ? proxy : "http://" + proxy));
                        }
                        break;
                    case WebRequestParamType.RefererString:
                        httpWebRequest.Referer = ConvertToTypeT<string, WebRequestParamString>(httpWebRequest.Referer, webRequestParam.Value);
                        break;
                    case WebRequestParamType.ContentTypeString:
                        httpWebRequest.ContentType = ConvertToTypeT<string, WebRequestParamString>(httpWebRequest.ContentType, webRequestParam.Value);
                        break;
                    /*
                    case WebRequestParamType.AcceptString:
                        break;
                        */
                }
            });
            
            return httpWebRequest;
        }

        private static T ConvertToTypeT<T, Base>(T paramValue, WebRequestParamBase webRequestParam, Action<T, Base> processAction = null) where Base : WebRequestParamBase {
            var casted = webRequestParam as Base;
            if (casted != null) {
                if (processAction != null) {
                    processAction(paramValue, casted);
                }
                return (T) casted.Value;
            }
            return paramValue;
        }
    }
}
