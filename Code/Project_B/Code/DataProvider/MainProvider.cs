using System.Collections.Generic;
using CommonUtils.ExtendedTypes;
using Project_B.Code.TransportType;

namespace Project_B.Code.DataProvider {
    public class MainProvider : Singleton<MainProvider> {
        public readonly BetProvider BetProvider = new BetProvider();
        public readonly CompetitionProvider CompetitionProvider = new CompetitionProvider();
        public readonly CompetitorProvider CompetitorProvider =  new CompetitorProvider();
        public readonly HistoryProvider HistoryProvider = new HistoryProvider();
        public readonly ResultProvider ResultProvider = new ResultProvider();
        public readonly LiveProvider LiveProvider = new LiveProvider();

        public List<SiteMapItem> GetSiteMapItems() {
            var result = new List<SiteMapItem>();



            return result;
        } 
    }
}