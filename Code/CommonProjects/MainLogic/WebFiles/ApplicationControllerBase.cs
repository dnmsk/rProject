using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using MainLogic.Transport;
using MainLogic.WebFiles.UserPolicy.Enum;
using MainLogic.Wrapper;

namespace MainLogic.WebFiles {
    public abstract class ApplicationControllerBase : Controller {
        public static readonly StringCryptoManagerDES CryptoManager = new StringCryptoManagerDES(SiteConfiguration.GetConfigurationProperty<string>("DataEncryptorKey"));
        protected override bool DisableAsyncSupport => true;

        public const string GUEST_COOKIE_NAME = "guest";
        public const string UTM_COOKIE_NAME = "utm_data";
        public const string UTM_SOURCE_PARAM_NAME = "utm_source";
        public const string UTM_CAMPAIGN_PARAM_NAME = "utm_campaign";
        public const string UTM_MEDIUM_PARAM_NAME = "utm_medium";
        private const string _urlReferrerCookieName = "prevUrl";

        protected static readonly MainLogicProvider BusinessLogic = new MainLogicProvider();

        public UtmParamWrapper UtmParam { get; private set; }

        /// <summary>
        /// базовая модель
        /// </summary>
        private BaseModel _baseModel;

        public BaseModel GetBaseModel() {
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
                    var isFirstTimeOpen = false;
                    if (HttpContext.Request.Cookies[GUEST_COOKIE_NAME] == null || 
                                    !int.TryParse(CryptoManager.DecryptString(HttpContext.Request.Cookies[GUEST_COOKIE_NAME].Value), out guid)) {
                        guid = CreateGuidInfo(HttpContext);
                        isFirstTimeOpen = true;
                    } else if (!BusinessLogic.UserProvider.CheckGuest(guid)) {
                        guid = CreateGuidInfo(HttpContext);
                        isFirstTimeOpen = true;
                    }
                    _currentUser = SessionModule.CreateSessionModule(guid, HttpContext);
                    if (isFirstTimeOpen) {
                        LogAction(LogActionID.OpenSiteFirstTime, null);
                    }
                }
                return _currentUser;
            } set {
                if (value == null) {
                    _currentUser = new SessionModule(CurrentUser.GuestID);
                    FormsAuthentication.SignOut();
                    HttpContext.Response.Cookies.Add(new HttpCookie(GUEST_COOKIE_NAME, CryptoManager.EncryptString(CurrentUser.GuestID.ToString(CultureInfo.InvariantCulture))) {
                        Expires = DateTime.Today.AddYears(10)
                    });
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

        public void LogAction(Enum logID, int? objectID, Dictionary<string, string> additionalParams = null) {
            if (GetBaseModel().GetUserPolicyState<bool>(UserPolicyGlobal.IsStatisticsDisabled)) {
                return;
            }
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

        protected static string GetUrlReffererString(HttpRequestBase requestContext) {
            var prevUri = requestContext.UrlReferrer;
            var refData = string.Empty;
            if (prevUri != null && requestContext.Cookies[_urlReferrerCookieName] == null) {
                refData += prevUri.ToString();
            }
            requestContext.QueryString["url_from"].Do(urlFrom => { refData += string.Format(";{0}", urlFrom); });
            return refData;
        }

        private static int CreateGuidInfo(HttpContextBase context) {
            var requestContext = context.Request;
            var responseContext = context.Response;
            var urlRefferer = GetUrlReffererString(requestContext);
            var guid = BusinessLogic.UserProvider.CreateNewGuest(GetUserIp(System.Web.HttpContext.Current.Request), requestContext.UserAgent);

            responseContext.Cookies.Add(new HttpCookie(GUEST_COOKIE_NAME, CryptoManager.EncryptString(guid.ToString(CultureInfo.InvariantCulture))) {
                Expires = DateTime.Today.AddYears(10)
            });

            if (!string.IsNullOrEmpty(urlRefferer)) {
                responseContext.Cookies.Add(
                    new HttpCookie(_urlReferrerCookieName, CryptoManager.EncryptString(urlRefferer)) {
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
                                    ? (referrer.IndexOf(domain, StringComparison.Ordinal) >= 0
                                        ? domain
                                        : referrer.GetDomain())
                                    // если заход по прямому урлу
                                    : string.Empty),
                    request.Params.Get(UTM_CAMPAIGN_PARAM_NAME) ?? string.Empty,
                    request.Params.Get(UTM_MEDIUM_PARAM_NAME) ?? string.Empty,
                    true
                );
                var utmCookie = new HttpCookie(UTM_COOKIE_NAME, CryptoManager.EncryptString(UtmParam.SerializeStruct())) {
                    Expires = DateTime.UtcNow.AddYears(5)
                };
                response.Cookies.Set(utmCookie);
            } else {
                UtmParam = UtmParamWrapper.DeserializeParamWrapper(CryptoManager.DecryptString(request.Cookies[UTM_COOKIE_NAME].Value));
            }
        }
    }
}