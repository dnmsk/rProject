using System.Collections.Generic;
using CommonUtils.ExtendedTypes;
using Project_B.CodeClientSide.TransportType;

namespace Project_B.CodeServerSide.DataProvider {
    public class ProjectProvider : Singleton<ProjectProvider> {
        public readonly BetProvider BetProvider = new BetProvider();
        public readonly CompetitionProvider CompetitionProvider = new CompetitionProvider();
        public readonly CompetitorProvider CompetitorProvider =  new CompetitorProvider();
        public readonly HistoryProvider HistoryProvider = new HistoryProvider();
        public readonly ResultProvider ResultProvider = new ResultProvider();
        public readonly LiveProvider LiveProvider = new LiveProvider();
        public readonly StaticPageProvider StaticPageProvider = new StaticPageProvider();

        public List<SiteMapItem> GetSiteMapItems() {
            var result = new List<SiteMapItem>();



            return result;
        } 
    }
}