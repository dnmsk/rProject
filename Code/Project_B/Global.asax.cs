using System.Web.Routing;
using MainLogic.WebFiles;
using Project_B.Code.Algorythm;

namespace Project_B {
    public class MvcApplication : GlobalAsaxBase {
        private static object[] _taskObjects = null;

        public override void OnStart() {
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            if (SiteConfiguration.NeedRunTask) {
                _taskObjects = new[] {
                    new CollectHistoryAlgo(),
                };
            }
        }
    }
}