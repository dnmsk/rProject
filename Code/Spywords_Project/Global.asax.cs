using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using MainLogic.WebFiles.Policy;
using Spywords_Project.Code.Algorithms;

namespace Spywords_Project {
    public class MvcApplication : HttpApplication {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(MvcApplication).FullName);
        private static AlgoBase[] _phraseAlgos;
        
        protected void Application_Start() {
            var isProduction = ProductionPolicy.IsProduction();
            _logger.Info("Web start, Production mode: " + isProduction);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders.DefaultBinder = new ExtendedModelBinder();
            if (isProduction) {
                _phraseAlgos = new[] {
                    (AlgoBase) new CollectDomainInfoSpywords(),
                    new CollectDomainsFromPhraseSpywords(),
                    new CollectEmailPhoneFromDomain(),
                    new CollectShowsDomainYadro()
                };
            }
        }

        protected void Application_End() {
            _logger.Info("Web end");
        }

        protected void Application_Error(object sender, EventArgs e) {
            _phraseAlgos.Select(s => s);//костыль

            Exception exception = Server.GetLastError();
            Response.Clear();
            var bc = Request.Browser;
            string browser = String.Format("Type: {0}, Name: {1}, Version: {2}, Major: {3}, Minor: {4}, Platform: {5}",
                                           bc.Type, bc.Browser, bc.Version, bc.MajorVersion, bc.MinorVersion,
                                           bc.Platform);
            var refferer = Request.UrlReferrer.Return(m => m.ToString(), "null");
            var guidCookie = Request.Cookies["guid"];
            var guid = guidCookie != null ? guidCookie.Value : string.Empty;
            var ip = "66.66";

            if (!guid.IsNullOrWhiteSpace() && guid != "-1") {
                if (!Request.Url.PathAndQuery.Contains("apple-touch-icon", StringComparison.InvariantCultureIgnoreCase)) {
                    _logger.Error(
                        "В Global.asax поймано исключение.\nGuestID: {0}\nБраузер: {1}\nUrl: {2}\n Пред.Урл: {3}\n IP: {4}\nException: {5}",
                        guid, browser, Request.Url, refferer, ip, exception);
                }
            } else {
            }

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