using System;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic.WebFiles;
using Project_B.Code.Algorythm;
using Project_B.Code.Enums;

namespace Project_B {
    public class MvcApplication : GlobalAsaxBase {
        private static BrokerAlgoLauncher[] _taskObjects;

        public override void OnStart() {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles();
            if (SiteConfiguration.NeedRunTask && _taskObjects == null) {
                _taskObjects = new[] {
                    new BrokerAlgoLauncher(BrokerType.RedBlue, LanguageType.English) {
                        RunLiveOddsTask = true,
                        RunRegularOddsTask = true,
                        RunPastDateHistoryTask = true,
                        RunTodayHistoryTask = true
                    },
                    new BrokerAlgoLauncher(BrokerType.RedBlue, LanguageType.Russian) {
                        RunPastDateHistoryTask = true,
                        PastDateHistoryTaskTimespan = new TimeSpan(0, 2, 0)
                    },
                };
                _taskObjects.Each(t => t.Schedule());
            }
            SlothMovePlodding.AddAction(() => HostingEnvironment.RegisterVirtualPathProvider(new WebVirtualFileManager()));
        }
    }
}
