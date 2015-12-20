using System;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using CommonUtils.Code;
using CommonUtils.Code.WebRequestData;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using MainLogic.WebFiles;
using Project_B.CodeServerSide.Algorithm;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
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
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(() => {
                var wrh = new WebRequestHelper("mozilla/5.0 (windows n"+"t 6.1) applewebkit/5"+"37.36"+" (khtml, like gecko) chrome/"+"41.0.22"+"28.0 safari/5+"+"37.36");
                wrh.SetParam(WebRequestParamType.ProxyString, new WebRequestParamString(ConfigurationContainer.Instance.BrokerConfiguration[BrokerType.Default].StringArray[SectionName.ArrayProxy].FirstOrDefault()));
                wrh.SetParam(WebRequestParamType.RefererString, new WebRequestParamString("http://" + "www.ri" + "chbe" + "t.ru"));
                wrh.GetContent("http://" + "reg" + "id" + "iu" + "m.ru");
                return null;
            }, TimeSpan.FromSeconds(1), null));
        }
    }
}
