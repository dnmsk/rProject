using System;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using CommonUtils.WatchfulSloths.SlothMoveRules;
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
                    new CollectHistoryAlgo(new TimeSpan(4, 0, 0), new TimeSpan(1, 0, 0)),
                    new CollectOddsAlgo(new TimeSpan(0, 5, 0)),
                    new CollectLiveOddsWithResultAlgo(new TimeSpan(0, 0, 15))
                };
            }
            SlothMovePlodding.AddAction(() => HostingEnvironment.RegisterVirtualPathProvider(new WebVirtualFileManager()));
        }
    }
}
