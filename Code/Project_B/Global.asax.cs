using System;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic.WebFiles;
using Project_B.CodeServerSide.Algorithm;
using Project_B.CodeServerSide.Enums;

namespace Project_B {
    public class MvcApplication : GlobalAsaxBase {
        private static BrokerAlgoLauncher[] _taskObjects;

        public override void OnStart() {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles();
            if (SiteConfiguration.NeedRunTask && _taskObjects == null) {
                _taskObjects = new[] {
                    new BrokerAlgoLauncher(BrokerType.RedBlue, 
                                           GatherBehaviorMode.CreateIfNew, 
                                           LanguageType.English,
                                           RunTaskMode.RunPastDateHistoryTask | RunTaskMode.RunLiveOddsTask | RunTaskMode.RunRegularOddsTask | RunTaskMode.RunTodayHistoryTask) {
                        PastDateHistoryTaskTimespan = new TimeSpan(0, 1, 0),
                    },
                    new BrokerAlgoLauncher(BrokerType.RedBlue, 
                                           GatherBehaviorMode.CanDetectCompetition | GatherBehaviorMode.CanDetectCompetitor /*| GatherBehaviorMode.CreateIfEmptyToDate*/, 
                                           LanguageType.Russian,
                                           RunTaskMode.RunPastDateHistoryTask) {
                        PastDateHistoryTaskTimespan = new TimeSpan(0, 1, 0)
                    },
                };
                _taskObjects.Each(t => t.Schedule());
            }
            SlothMovePlodding.AddAction(() => HostingEnvironment.RegisterVirtualPathProvider(new WebVirtualFileManager()));
        }
    }
}
