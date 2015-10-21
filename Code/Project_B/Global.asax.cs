using System.Web.Mvc;
using System.Web.Routing;
using MainLogic.WebFiles;
using Project_B.Code.Algorythm;

namespace Project_B {
    public class MvcApplication : GlobalAsaxBase {
        private static object[] _taskObjects;

        public override void OnStart() {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles();
            if (SiteConfiguration.NeedRunTask && _taskObjects == null) {
                _taskObjects = new object[] {
                    new CollectHistoryAlgo(),
                    new CollectOddsAlgo(),
                    new CollectLiveOddsWithResultAlgo()
                };
            }
        }
    }
}
