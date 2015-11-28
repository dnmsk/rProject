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
                    new BrokerAlgoLauncher(BrokerType.RedBlue, LanguageType.English, GatherBehaviorMode.CreateIfNew, RunTaskMode.AllTasks),
                    new BrokerAlgoLauncher(BrokerType.GrayBlue, LanguageType.English, GatherBehaviorMode.TryDetectOnly, RunTaskMode.AllTasks) {
                        TodayHistoryTaskTimespan = TimeSpan.FromHours(12)
                    },
                    new BrokerAlgoLauncher(BrokerType.RedBlue, LanguageType.Russian, GatherBehaviorMode.TryDetectOnly, RunTaskMode.HistoryTasks) {
                        TodayHistoryTaskTimespan = TimeSpan.FromHours(12)
                    },
                };
                _taskObjects.Each(t => t.Schedule());
            }
            SlothMovePlodding.AddAction(() => HostingEnvironment.RegisterVirtualPathProvider(new WebVirtualFileManager()));
            BaseModelConfig.ConfigureBaseModel();
        }
    }
}
