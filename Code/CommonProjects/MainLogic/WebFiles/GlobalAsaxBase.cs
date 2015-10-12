using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;

namespace MainLogic.WebFiles {
    public abstract class GlobalAsaxBase : HttpApplication {
        /// <summary>
        /// Логгер.
        /// </summary>
        protected static readonly LoggerWrapper Logger = LoggerManager.GetLogger(typeof(GlobalAsaxBase).FullName);

        public void Application_Start() {
            Logger.Info("Web start, RunTask mode: " + SiteConfiguration.NeedRunTask);
            AreaRegistration.RegisterAllAreas();
            ModelBinders.Binders.DefaultBinder = new ExtendedModelBinder();
            OnStart();
        }

        public abstract void OnStart();

        public void Application_End() {
            Logger.Info("Web end");
        }

        public void Application_Error(object sender, EventArgs e) {
            Exception exception = Server.GetLastError();
            Response.Clear();
            var bc = Request.Browser;
            string browser = string.Format("Type: {0}, Name: {1}, Version: {2}, Major: {3}, Minor: {4}, Platform: {5}",
                                           bc.Type, bc.Browser, bc.Version, bc.MajorVersion, bc.MinorVersion,
                                           bc.Platform);
            var refferer = Request.UrlReferrer.Return(m => m.ToString(), "null");
            var guidCookie = Request.Cookies["guid"];
            var guid = guidCookie != null ? guidCookie.Value : string.Empty;
            var ip = ApplicationControllerBase.GetUserIp(Request);

            //if (!guid.IsNullOrWhiteSpace() && guid != "-1") {
                if (!Request.Url.PathAndQuery.Contains("apple-touch-icon", StringComparison.InvariantCultureIgnoreCase)) {
                    Logger.Error(
                        "В Global.asax поймано исключение.\nGuestID: {0}\nБраузер: {1}\nUrl: {2}\n Пред.Урл: {3}\n IP: {4}\nException: {5}",
                        guid, browser, Request.Url, refferer, ip, exception);
                }
            /*} else {
            }
           */

            var httpException = exception as HttpException;
            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            if (httpException == null) {
                routeData.Values.Add("action", "Internal");
            } else {
                switch (httpException.GetHttpCode()) {
                    case 404:
                        routeData.Values.Add("action", "NotFound");
                        break;
                    default:
                        routeData.Values.Add("action", "Internal");
                        break;
                }
            }

            routeData.Values.Add("error", exception);
            Server.ClearError();
            Response.TrySkipIisCustomErrors = true;
            //IController errorController = new ErrorController();
            //errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }
    }
}
