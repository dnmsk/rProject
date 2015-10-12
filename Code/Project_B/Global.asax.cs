using System.Web.Routing;
using MainLogic.WebFiles;
using Project_B.Code.Algorythm;

namespace Project_B {
    public class MvcApplication : GlobalAsaxBase {
        private static object[] _taskObjects;

        public override void OnStart() {
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            if (SiteConfiguration.NeedRunTask && _taskObjects == null) {
                _taskObjects = new object[] {
                    new CollectHistoryAlgo(),
                    new CollectOddsAlgo(), 
                };
            }
        }
    }
}