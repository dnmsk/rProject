using System;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.WatchfulThreads;
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
                    new BrokerAlgoLauncher(BrokerType.RedBlue, LanguageType.English, GatherBehaviorMode.TryDetectAll | GatherBehaviorMode.CreateOriginalIfMatchedAll, RunTaskMode.AllTasks) {
                        PastDateHistoryTaskTimespan = TimeSpan.FromMinutes(1)
                    },
                    new BrokerAlgoLauncher(BrokerType.GrayBlue, LanguageType.English, GatherBehaviorMode.TryDetectAll, RunTaskMode.HistoryTasks | RunTaskMode.RunRegularOddsTask) {
                        TodayHistoryTaskTimespan = TimeSpan.FromHours(12),
                        PastDateHistoryTaskTimespan = TimeSpan.FromMinutes(1)
                    },
                    new BrokerAlgoLauncher(BrokerType.RedBlue, LanguageType.Russian, GatherBehaviorMode.TryDetectAll, RunTaskMode.HistoryTasks) {
                        TodayHistoryTaskTimespan = TimeSpan.FromHours(12),
                        PastDateHistoryTaskTimespan = TimeSpan.FromMinutes(1)
                    },
                };
                _taskObjects.Each(t => t.Schedule());
            }
            TaskRunner.Instance.AddAction(() => HostingEnvironment.RegisterVirtualPathProvider(new WebVirtualFileManager()));
            BaseModelConfig.ConfigureBaseModel();
        }
    }
}
