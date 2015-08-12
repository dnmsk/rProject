using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CommonUtils.Code;
using CommonUtils.Core.Logger;

namespace Project_R.Code {
    public abstract class ApplicationControllerBase : Controller {
        public const string GUID_COOKIE_NAME = "guid";

        public const string UTM_COOKIE_NAME = "utm_data";
        public const string UTM_SOURCE_PARAM_NAME = "utm_source";
        public const string UTM_CAMPAIGN_PARAM_NAME = "utm_campaign";
        public const string UTM_MEDIUM_PARAM_NAME = "utm_medium";
        public const string DATE_ARIVED_COOKIE_NAME = "date";
        public const string CITY_REQUEST_PARAM_NAME = "city";
        public const string CITY_COOKIE_NAME = "city";
        private const string _urlReferrerCookieName = "prevUrl";

        private const string _domainPattern = "(?:https?://)?(?:www)?(.+?(?=/))";
        private static readonly Regex _domainRegex = new Regex(_domainPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public UtmParamWrapper UtmParam { get; private set; }
        private int _guid;
        public int CurrentGuid {
            get { return _guid; }

            set {
                Response.Cookies.Add(new HttpCookie(GUID_COOKIE_NAME, value.ToString(CultureInfo.InvariantCulture)) {
                    Expires = DateTime.Today.AddYears(10),
                });
                _guid = value;
            }
        }

        protected override void ExecuteCore() {
            InitCookies(HttpContext);
            base.ExecuteCore();
        }

        private void InitCookies(HttpContextBase httpContext) {
            InitUtmCookies(httpContext.Request);

            if (httpContext.Request.Cookies[GUID_COOKIE_NAME] == null || !int.TryParse(httpContext.Request.Cookies[GUID_COOKIE_NAME].Value, out _guid)) {
                CreateGuidInfo(httpContext);
                httpContext.Response.Cookies.Add(
                    new HttpCookie(DATE_ARIVED_COOKIE_NAME, DateTime.Now.ToString(CultureInfo.InvariantCulture)) {
                        Expires = DateTime.Today.AddYears(1)
                    }
                );
            }
            int cityID;
            if (httpContext.Request.Cookies[CITY_COOKIE_NAME] != null && int.TryParse(httpContext.Request.Cookies[CITY_COOKIE_NAME].Value, out cityID)) {
                _userCityID = cityID;
            } else {
                cityID = CurrentUserCityID;
                Response.Cookies.Add(new HttpCookie(CITY_COOKIE_NAME, cityID.ToString()) {
                    Expires = DateTime.Now.AddYears(5)
                });
            }
        }

        private int? _userCityID;
        protected int CurrentUserCityID {
            get {
                if (_userCityID.HasValue) {
                    return _userCityID.Value;
                }
                var townRequestParam = Request[CITY_REQUEST_PARAM_NAME];
                int cityId;
                if (!string.IsNullOrEmpty(townRequestParam) && int.TryParse(townRequestParam, out cityId)) {
                    return cityId;
                }
                return GetCityIDByHost(GetUserIp(Request));
            }
        }
        protected int GetCityIDByHost(string hostAddress) {
            IPAddress address;
//TODO            CityDetails cityDetails = null;
//            if (!string.IsNullOrEmpty(hostAddress) && IPAddress.TryParse(hostAddress, out address)) {
//                cityDetails = _businessLogic.GetCityDetails(address);
//            }
//            _userCityID = cityDetails.Return(m => m.TownFullID, MapConsts.MOSCOW_TOWN_ID_NEW) ?? MapConsts.MOSCOW_TOWN_ID_NEW;
            return _userCityID.Value;
        }

        protected void LogAction(LogActionID logID, long? objectID, Dictionary<string, string> additionalParams = null) {
            var pars = new Dictionary<string, string> {
                {"utm_source", UtmParam.UtmSource},
                {"utm_campaign", UtmParam.UtmCampaign},
                {"utm_medium", UtmParam.UtmMedium},
                {"TownFullID", CurrentUserCityID.ToString(CultureInfo.InvariantCulture) }
            };
            if (additionalParams != null) {
                additionalParams.Each(kv => {
                    pars[kv.Key] = kv.Value;
                });
            }
            UserActionLogger.Log(CurrentGuid, logID, objectID, pars);
        }
        private static string GetUserAgent(HttpRequestBase requestContext) {
            return requestContext.UserAgent;
        }

        private static string GetUserIp(HttpRequestBase requestContext) {
            var userIP = GetUnfilteredUserIP(requestContext);
            var indexOf = userIP.IndexOf(",", StringComparison.InvariantCulture);
            var filteredUserIP = userIP.Substring(0, indexOf > 0 ? indexOf : userIP.Length);
            return filteredUserIP;
        }

        private static string GetUnfilteredUserIP(HttpRequestBase requestContext) {
            if (requestContext == null) {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(requestContext.Headers["X_REAL_IP"])) {
                return requestContext.Headers["X_REAL_IP"];
            }
            if (!string.IsNullOrEmpty(requestContext.Headers["X-Real-IP"])) {
                return requestContext.Headers["X-Real-IP"];
            }
            if (!string.IsNullOrEmpty(requestContext.Headers["X-Forwarded-For"])) {
                return requestContext.Headers["X-Forwarded-For"];
            }
            return requestContext.UserHostAddress;
        }

        protected string GetUrlReffererString(HttpRequestBase requestContext) {
            var prevUri = requestContext.UrlReferrer;
            var refData = string.Empty;

            if (prevUri != null && requestContext.Cookies[_urlReferrerCookieName] == null) {
                refData += prevUri.ToString();
            }

            requestContext.QueryString["url_from"].Do(urlFrom => { refData += string.Format(";{0}", urlFrom); });

            return refData;
        }

        private void CreateGuidInfo(HttpContextBase context) {
            var requestContext = context.Request;
            var urlRefferer = GetUrlReffererString(requestContext);
//TODO            var guid = _businessLogic.CreateNewGuid(urlRefferer, GetUserIp(requestContext), GetUserAgent(requestContext));
//
//            CurrentGuid = guid;

            LogAction(LogActionID.OpenSiteFirstTime, null);

            if (!string.IsNullOrEmpty(urlRefferer)) {
                context.Response.Cookies.Add(
                    new HttpCookie(_urlReferrerCookieName, HttpUtility.UrlEncode(urlRefferer)) {
                        Expires = DateTime.Today.AddYears(1)
                    }
                );
            }
        }

        private void InitUtmCookies(HttpRequestBase request) {
            var domain = SiteConfiguration.ProductionHostName;
            var referrer = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : string.Empty;
            if (request.Cookies[UTM_COOKIE_NAME] == null) {
                UtmParam = new UtmParamWrapper(
                    // если есть, берем из параметра урла
                    Request.Params.Get(UTM_SOURCE_PARAM_NAME) ??
                                // иначе ищем реферер
                                (Request.UrlReferrer != null
                                    // проверяем, внутренний ли переход
                                    ? (referrer.IndexOf(domain, StringComparison.Ordinal) > 0
                                        // скажем что внутренний переход
                                        ? domain
                                        // иначе попробуем распарсить регуляркой
                                        : (_domainRegex.Match(referrer).Groups.Count > 1
                                            // найдем домен...
                                            ? _domainRegex.Match(referrer).Groups[1].ToString()
                                            // ...или извинимся
                                            : "НЕ РАСПАРСИЛОСЬ" + referrer))
                                    // если заход по прямому урлу
                                    : "target"),
                    Request.Params.Get(UTM_CAMPAIGN_PARAM_NAME) ?? string.Empty,
                    Request.Params.Get(UTM_MEDIUM_PARAM_NAME) ?? string.Empty
                );
                var utmCookie = new HttpCookie(UTM_COOKIE_NAME, UtmParam.SerializeStruct()) {
                    Expires = DateTime.Now.AddYears(1)
                };
                Response.Cookies.Set(utmCookie);
            } else /*if (UtmSource.IsNullOrEmpty())*/ {
                UtmParam = UtmParamWrapper.DeserializeParamWrapper(request.Cookies[UTM_COOKIE_NAME].Value);
            }
        }
    }
}