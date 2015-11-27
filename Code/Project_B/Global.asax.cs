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
                    new BrokerAlgoLauncher(BrokerType.RedBlue, LanguageType.English, 
                                GatherBehaviorMode.CreateIfNew, 
                                RunTaskMode.RunPastDateHistoryTask | RunTaskMode.RunLiveOddsTask | RunTaskMode.RunRegularOddsTask | RunTaskMode.RunTodayHistoryTask) {
                    },
                    new BrokerAlgoLauncher(BrokerType.GrayBlue, LanguageType.English, 
                                GatherBehaviorMode.CanDetectCompetition | GatherBehaviorMode.CanDetectCompetitor, 
                                RunTaskMode.RunPastDateHistoryTask | RunTaskMode.RunLiveOddsTask | RunTaskMode.RunRegularOddsTask | RunTaskMode.RunTodayHistoryTask) {
                        PastDateHistoryTaskTimespan = TimeSpan.FromMinutes(1),
                        TodayHistoryTaskTimespan = TimeSpan.FromHours(6)
                    },
                    new BrokerAlgoLauncher(BrokerType.RedBlue, LanguageType.Russian, 
                                GatherBehaviorMode.CanDetectCompetition | GatherBehaviorMode.CanDetectCompetitor,
                                RunTaskMode.RunPastDateHistoryTask | RunTaskMode.RunTodayHistoryTask) {
                        PastDateHistoryTaskTimespan = TimeSpan.FromMinutes(1),
                        TodayHistoryTaskTimespan = TimeSpan.FromHours(6)
                    },
                };
                _taskObjects.Each(t => t.Schedule());
            }
            SlothMovePlodding.AddAction(() => HostingEnvironment.RegisterVirtualPathProvider(new WebVirtualFileManager()));
            BaseModelConfig.ConfigureBaseModel();
        }
    }
}
