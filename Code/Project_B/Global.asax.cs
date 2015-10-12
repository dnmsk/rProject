using System.Web.Routing;
using MainLogic.WebFiles;

namespace Project_B {
    public class MvcApplication : GlobalAsaxBase {

        public override void OnStart() {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}