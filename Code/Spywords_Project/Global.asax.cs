using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MainLogic.WebFiles;
using Spywords_Project.Code.Algorithms;

namespace Spywords_Project {
    public class MvcApplication : GlobalAsaxBase {
        private static AlgoBase[] _phraseAlgos;

        public override void OnStart() {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            if (SiteConfiguration.NeedRunTask) {
                _phraseAlgos = new[] {
                    (AlgoBase) 
                    new CollectDomainInfoSpywords(),
                    new CollectDomainsFromPhraseSpywords(),
                    new CollectEmailPhoneFromDomain(),
                    new CollectShowsDomainYadro(),
                    new CollectPhrasesForDomainSpywords(),
                    new CollectSearchEngineDomainsFromPhrase(),
                };
            }
        }
    }
}