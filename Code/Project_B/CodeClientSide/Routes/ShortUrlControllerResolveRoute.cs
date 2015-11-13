using System.Web;
using System.Web.Routing;
using Project_B.Models;

namespace Project_B.CodeClientSide.Routes {
    public class ShortUrlControllerResolveRoute : LeftMatchingRoute {

        public ShortUrlControllerResolveRoute(string targetUrl, IRouteHandler handler, string defaultNameSpace)
            : base(targetUrl, handler, defaultNameSpace) {
        }

        public override RouteData GetRouteData(HttpContextBase httpContext) {
            var baseRouteData = base.GetRouteData(httpContext);
            if (baseRouteData == null) {
                return null;
            }
            var shortName = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(NeededOnTheLeft.Length).Trim('/');
            return true || BrokerPageModel.ContainsBroker(shortName) ? baseRouteData : null;
        }
    }
}