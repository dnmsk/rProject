using System.Web.Mvc;
using System.Web.Routing;
using MainLogic.WebFiles;

namespace Project_B {
    public class MvcApplication : GlobalAsaxBase {

        public override void OnStart() {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles();
        }
    }
}
