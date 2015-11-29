using System;
using CommonUtils.Code;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.CodeServerSide.BrokerProvider;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Algorithm {
    public class BrokerAlgoLauncher {
        private readonly BrokerType _brokerType;
        private readonly GatherBehaviorMode _algoMode;
        private readonly LanguageType _languageType;
        private readonly RunTaskMode _runTaskMode;
        private readonly SportType _sportType;
        
        public TimeSpan TodayHistoryTaskTimespan = TimeSpan.FromHours(1);
        public TimeSpan PastDateHistoryTaskTimespan = TimeSpan.FromHours(4);
        public TimeSpan LiveOddsTaskTimespan = TimeSpan.FromSeconds(60);
        public TimeSpan RegularOddsTaskTimespan = TimeSpan.FromMinutes(5);

        public BrokerAlgoLauncher(BrokerType brokerType, LanguageType languageType, GatherBehaviorMode algoMode, RunTaskMode runTaskMode = RunTaskMode.Default, SportType sportType = SportType.All) {
            _brokerType = brokerType;
            _algoMode = algoMode;
            _languageType = languageType;
            _runTaskMode = runTaskMode;
            _sportType = sportType;
        }

        public void Schedule() {
            if (_runTaskMode.HasFlag(RunTaskMode.RunTodayHistoryTask)) {
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectHistoryForToday, TodayHistoryTaskTimespan, null));
            }
            if (_runTaskMode.HasFlag(RunTaskMode.RunPastDateHistoryTask)) {
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(() => CollectHistoryForPastDate(), PastDateHistoryTaskTimespan, null));
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectHistoryForYesterday, TodayHistoryTaskTimespan, null));
            }
            if (_runTaskMode.HasFlag(RunTaskMode.RunLiveOddsTask)) {
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectLiveOddsWithResult, LiveOddsTaskTimespan, null));
            }
            if (_runTaskMode.HasFlag(RunTaskMode.RunRegularOddsTask)) {
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectRegularOdds, RegularOddsTaskTimespan, null));
            }
        }

        public object CollectRegularOdds() {
            using (new MiniProfiler(GetProfilerString("CollectRegularOdds"))) {
                var utcNow = DateTime.UtcNow;
                if (ProjectProvider.Instance.BetProvider.GetStateRegular(_brokerType, utcNow) != SystemStateBetType.Unknown) {
                    var regularOdds = Broker.LoadRegular(_sportType, _languageType);
                    ProjectProvider.Instance.BetProvider.SaveRegular(regularOdds, _algoMode);
                    ProjectProvider.Instance.BetProvider.SetStateRegular(_brokerType, utcNow);
                }
                return null;
            }
        }

        public object CollectLiveOddsWithResult() {
            using (new MiniProfiler(GetProfilerString("CollectLiveOddsWithResult"))) {
                var liveData = Broker.LoadLive(_sportType, _languageType);
                ProjectProvider.Instance.LiveProvider.ProcessdLiveParsed(liveData, _algoMode);
                return null;
            }
        }

        public object CollectHistoryForPastDate(DateTime? pastDate = null) {
            using (new MiniProfiler(GetProfilerString("CollectHistoryForPastDate"))) {
                var minDateToCollect = pastDate ?? ProjectProvider.Instance.HistoryProvider.GetPastDateToCollect(_brokerType, _languageType, SystemStateResultType.CollectForTwoDayAgo);
                if (minDateToCollect != null) {
                    var historyData = Broker.LoadResult(minDateToCollect.Value, _sportType, _languageType);
                    ProjectProvider.Instance.HistoryProvider.SaveResult(historyData, _algoMode);
                    ProjectProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, minDateToCollect.Value, SystemStateResultType.CollectForTwoDayAgo);
                }
                return null;
            }
        }

        public object CollectHistoryForYesterday() {
            using (new MiniProfiler(GetProfilerString("CollectHistoryForYesterday"))) {
                var minDateToCollect = DateTime.UtcNow.AddDays(-1);
                if (minDateToCollect.Hour%3 == 0) {
                    var historyData = Broker.LoadResult(minDateToCollect, _sportType, _languageType);
                    ProjectProvider.Instance.HistoryProvider.SaveResult(historyData, _algoMode);
                    ProjectProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, minDateToCollect.Date, SystemStateResultType.CollectForYesterday);
                }
                return null;
            }
        }

        public object CollectHistoryForToday() {
            using (new MiniProfiler(GetProfilerString("CollectHistoryForToday"))) {
                var todayUtc = DateTime.UtcNow.Date;
                var historyData = Broker.LoadResult(todayUtc, _sportType, _languageType);
                ProjectProvider.Instance.HistoryProvider.SaveResult(historyData, _algoMode);
                ProjectProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, todayUtc, SystemStateResultType.CollectForToday);
                return null;
            }
        }

        private BrokerBase Broker => BookPage.Instance.GetBrokerProvider(_brokerType);

        private string GetProfilerString(string runTaskMode) {
            return string.Format("{0} {1} {2}", _brokerType, _languageType, runTaskMode);
        }
    }
}