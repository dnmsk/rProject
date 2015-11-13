using System.Globalization;
using System.Web;
using System.Web.Routing;

namespace Project_B.CodeClientSide.Routes {
    public class LeftMatchingRoute : Route {
        protected readonly string NeededOnTheLeft;

        public LeftMatchingRoute(string targetUrl, IRouteHandler handler, string defaultNameSpace)
            : base(targetUrl, handler) {

            NeededOnTheLeft = FormatTargetUrl(targetUrl);
            DataTokens = new RouteValueDictionary{
                {"Namespaces", new[] { defaultNameSpace }}
            };
        }

        public override RouteData GetRouteData(HttpContextBase httpContext) {
            if (!httpContext.Request.AppRelativeCurrentExecutionFilePath.StartsWith(NeededOnTheLeft, true, CultureInfo.InvariantCulture)) {
                return null;
            }
            return base.GetRouteData(httpContext);
        }

        protected static string FormatTargetUrl(string url) {
            var idx = url.IndexOf('{');
            return "~/" + (idx >= 0 ? url.Substring(0, idx) : url).TrimEnd('/');
        }
    }
}