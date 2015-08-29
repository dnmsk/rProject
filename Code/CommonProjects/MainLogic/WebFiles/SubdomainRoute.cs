using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MainLogic.WebFiles {
    public class SubdomainRoute : RouteBase {
        private readonly string _controller;
        private readonly string _defaultAction;
        private readonly MvcRouteHandler _mvcRouteHandler;

        public SubdomainRoute(string controller, string defaultAction, MvcRouteHandler mvcRouteHandler) {
            _controller = controller;
            _defaultAction = defaultAction;
            _mvcRouteHandler = mvcRouteHandler;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext) {
            var host = httpContext.Request.Headers["HOST"];
            var index = host.IndexOf(".");
            var segments = httpContext.Request.Url.PathAndQuery.Split('/');

            if (index < 0) {
                return null;
            }

            var subdomain = host.Substring(0, index);
            var controller = (segments.Length > 0) ? segments[0] : _controller;
            var action = (segments.Length > 1) ? segments[1] : _defaultAction;

            var routeData = new RouteData(this, _mvcRouteHandler);
            routeData.Values.Add("controller", controller);
            routeData.Values.Add("action", action);
            routeData.Values.Add("subdomain", subdomain);
            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {
            return null;
        }
    }
}