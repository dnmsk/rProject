using CommonUtils.ExtendedTypes;

namespace Project_B.Code.DataProvider {
    public class MainProvider : Singleton<MainProvider> {
        public readonly BetProvider BetProvider = new BetProvider();
        public readonly CompetitionProvider CompetitionProvider = new CompetitionProvider();
        public readonly CompetitorProvider CompetitorProvider =  new CompetitorProvider();
        public readonly HistoryProvider HistoryProvider = new HistoryProvider();
        public readonly ResultProvider ResultProvider = new ResultProvider();
    }
}