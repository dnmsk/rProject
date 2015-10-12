using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using MainLogic.Transport;
using MainLogic.Wrapper;

namespace MainLogic.WebFiles {
    public abstract class ApplicationControllerBase : Controller {
        protected override bool DisableAsyncSupport {
            get { return true; }
        }

        public const string GUEST_COOKIE_NAME = "guest";
        public const string UTM_COOKIE_NAME = "utm_data";
        public const string UTM_SOURCE_PARAM_NAME = "utm_source";
        public const string UTM_CAMPAIGN_PARAM_NAME = "utm_campaign";
        public const string UTM_MEDIUM_PARAM_NAME = "utm_medium";
        private const string _urlReferrerCookieName = "prevUrl";

        private const string _domainPattern = "(?:https?://)?(?:www)?(.+?(?=/))";
        private static readonly Regex _domainRegex = new Regex(_domainPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected static readonly MainLogicProvider BusinessLogic = new MainLogicProvider();

        public UtmParamWrapper UtmParam { get; private set; }


        /// <summary>
        /// базовая модель
        /// </summary>
        private BaseModel _baseModel;

        protected internal BaseModel GetBaseModel() {
            if (_baseModel == null) {
                _baseModel = new BaseModel(CurrentUser, BusinessLogic);
            }
            return _baseModel;
        }

        private SessionModule _currentUser;
        public SessionModule CurrentUser {
            get {
                if (_currentUser == null) {
                    int guid;
                    if (HttpContext.Request.Cookies[GUEST_COOKIE_NAME] == null || !int.TryParse(HttpContext.Request.Cookies[GUEST_COOKIE_NAME].Value, out guid)) {
                         guid = CreateGuidInfo(System.Web.HttpContext.Current);
                        LogAction(LogActionID.OpenSiteFirstTime, null);
                    }
                    _currentUser = SessionModule.CreateSessionModule(guid, HttpContext);

                }
                return _currentUser;
            }
            set {
                if (value == null) {
                    _currentUser = new SessionModule(CurrentUser.GuestID);
                    FormsAuthentication.SignOut();
                    return;
                }
                if (value.IsAuthenticated()) {
                    _currentUser = value;
                    FormsAuthentication.SetAuthCookie(_currentUser.ModuleToString(), true);
                }
            }
        }
        
        protected override void ExecuteCore() {
            InitCookies(HttpContext);
            base.ExecuteCore();
        }

        private void InitCookies(HttpContextBase httpContext) {
            InitUtmCookies(httpContext.Request, httpContext.Response);

            BusinessLogic.UserProvider.SaveReferrer(CurrentUser.GuestID, httpContext.Request.UrlReferrer?.ToString() ?? string.Empty, httpContext.Request.Url?.ToString() ?? string.Empty);
            BusinessLogic.UserProvider.SaveUtm(CurrentUser.GuestID, UtmParam);
            var browserInfo = new BrowserInfo(httpContext.Request.Browser, httpContext.Request.UserAgent);
            BusinessLogic.UserProvider.SaveTechInfo(CurrentUser.GuestID, new GuestTechInfoTransport {
                Version = browserInfo.CurrentVersion(),
                BrowserType = browserInfo.Name,
                Os = browserInfo.Os,
                IsMobile = browserInfo.Mobile,
                UserAgent = browserInfo.UserAgent
            });
        }

        protected void LogAction(Enum logID, long? objectID, Dictionary<string, string> additionalParams = null) {
            var pars = new Dictionary<string, string> {
                {"utm_source", UtmParam.UtmSource},
                {"utm_campaign", UtmParam.UtmCampaign},
                {"utm_medium", UtmParam.UtmMedium}
            };
            if (additionalParams != null) {
                additionalParams.Each(kv => {
                    pars[kv.Key] = kv.Value;
                });
            }
            UserActionLogger.Log(CurrentUser.GuestID, logID, objectID, pars);
        }

        public static string GetUserIp(HttpRequest requestContext) {
            var userIP = GetUnfilteredUserIP(requestContext);
            var indexOf = userIP.IndexOf(",", StringComparison.InvariantCulture);
            var filteredUserIP = userIP.Substring(0, indexOf > 0 ? indexOf : userIP.Length);
            return filteredUserIP;
        }

        private static string GetUnfilteredUserIP(HttpRequest requestContext) {
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

        protected static string GetUrlReffererString(HttpRequest requestContext) {
            var prevUri = requestContext.UrlReferrer;
            var refData = string.Empty;

            if (prevUri != null && requestContext.Cookies[_urlReferrerCookieName] == null) {
                refData += prevUri.ToString();
            }

            requestContext.QueryString["url_from"].Do(urlFrom => { refData += string.Format(";{0}", urlFrom); });

            return refData;
        }

        private static int CreateGuidInfo(HttpContext context) {
            var requestContext = context.Request;
            var urlRefferer = GetUrlReffererString(requestContext);
            var guid = BusinessLogic.UserProvider.CreateNewGuid(GetUserIp(requestContext), context.Request.UserAgent);

            context.Response.Cookies.Add(new HttpCookie(GUEST_COOKIE_NAME, guid.ToString(CultureInfo.InvariantCulture)) {
                Expires = DateTime.Today.AddYears(10),
            });

            if (!string.IsNullOrEmpty(urlRefferer)) {
                context.Response.Cookies.Add(
                    new HttpCookie(_urlReferrerCookieName, HttpUtility.UrlEncode(urlRefferer)) {
                        Expires = DateTime.Today.AddYears(10)
                    }
                );
            }
            return guid;
        }

        private void InitUtmCookies(HttpRequestBase request, HttpResponseBase response) {
            var domain = SiteConfiguration.ProductionHostName;
            var referrer = request.UrlReferrer != null ? request.UrlReferrer.ToString() : string.Empty;
            if (request.Cookies[UTM_COOKIE_NAME] == null) {
                UtmParam = new UtmParamWrapper(
                    // если есть, берем из параметра урла
                    request.Params.Get(UTM_SOURCE_PARAM_NAME) ??
                                // иначе ищем реферер
                                (request.UrlReferrer != null
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
                    request.Params.Get(UTM_CAMPAIGN_PARAM_NAME) ?? string.Empty,
                    request.Params.Get(UTM_MEDIUM_PARAM_NAME) ?? string.Empty
                );
                var utmCookie = new HttpCookie(UTM_COOKIE_NAME, UtmParam.SerializeStruct()) {
                    Expires = DateTime.Now.AddYears(5)
                };
                response.Cookies.Set(utmCookie);
            } else /*if (UtmSource.IsNullOrEmpty())*/ {
                UtmParam = UtmParamWrapper.DeserializeParamWrapper(request.Cookies[UTM_COOKIE_NAME].Value);
            }
        }
    }
}