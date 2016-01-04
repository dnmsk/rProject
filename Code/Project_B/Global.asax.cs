using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using CommonUtils.WatchfulSloths.WatchfulThreads;
using MainLogic.WebFiles;
using Project_B.CodeServerSide.Algorithm;

namespace Project_B {
    public class MvcApplication : GlobalAsaxBase {
        private static BrokerAlgoLauncher _taskObject;

        public override void OnStart() {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles();
            if (SiteConfiguration.NeedRunTask && _taskObject == null) {
                _taskObject = new BrokerAlgoLauncher();
            }
            TaskRunner.Instance.AddAction(() => HostingEnvironment.RegisterVirtualPathProvider(new WebVirtualFileManager()));
            BaseModelConfig.ConfigureBaseModel();
        }
    }
}
