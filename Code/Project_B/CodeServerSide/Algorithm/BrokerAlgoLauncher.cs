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
        public TimeSpan RegularOddsTaskTimespan = TimeSpan.FromMinutes(30);

        public bool UseDbData { get; set; }

        public BrokerAlgoLauncher(BrokerType brokerType, LanguageType languageType, GatherBehaviorMode algoMode, RunTaskMode runTaskMode = RunTaskMode.Default, SportType sportType = SportType.All) {
            _brokerType = brokerType;
            _algoMode = algoMode;
            _languageType = languageType;
            _runTaskMode = runTaskMode;
            _sportType = sportType;
            UseDbData = false;
        }

        public void Schedule() {
            if (_runTaskMode.HasFlag(RunTaskMode.RunTodayHistoryTask)) {
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(() => CollectHistoryForToday(RunTaskMode.RunTodayHistoryTask), TodayHistoryTaskTimespan, null));
            }
            if (_runTaskMode.HasFlag(RunTaskMode.RunPastDateHistoryTask)) {
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(() => CollectHistoryForPastDate(RunTaskMode.RunPastDateHistoryTask), PastDateHistoryTaskTimespan, null));
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(() => CollectHistoryForYesterday(RunTaskMode.RunPastDateHistoryTask), TodayHistoryTaskTimespan, null));
            }
            if (_runTaskMode.HasFlag(RunTaskMode.RunLiveOddsTask)) {
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(() => CollectLiveOddsWithResult(RunTaskMode.RunLiveOddsTask), LiveOddsTaskTimespan, null));
            }
            if (_runTaskMode.HasFlag(RunTaskMode.RunRegularOddsTask)) {
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(() => CollectRegularOdds(RunTaskMode.RunRegularOddsTask), RegularOddsTaskTimespan, null));
            }
        }

        public object CollectRegularOdds(RunTaskMode taskMode) {
            using (new MiniProfiler(GetProfilerString(taskMode.ToString()))) {
                var regularOdds = Broker.LoadRegular(_sportType, _languageType);
                ProjectProvider.Instance.BetProvider.SaveBrokerState(regularOdds, _algoMode, taskMode);
                ProjectProvider.Instance.BetProvider.SetStateRegular(_brokerType, DateTime.UtcNow);
                return null;
            }
        }

        public object CollectLiveOddsWithResult(RunTaskMode taskMode) {
            using (new MiniProfiler(GetProfilerString(taskMode.ToString()))) {
                var liveData = Broker.LoadLive(_sportType, _languageType);
                ProjectProvider.Instance.BetProvider.SaveBrokerState(liveData, _algoMode, taskMode);
                return null;
            }
        }

        public object CollectHistoryForPastDate(RunTaskMode taskMode, DateTime? pastDate = null) {
            using (new MiniProfiler(GetProfilerString(taskMode.ToString()))) {
                var minDateToCollect = pastDate ?? ProjectProvider.Instance.HistoryProvider.GetPastDateToCollect(_brokerType, _languageType, SystemStateResultType.CollectForTwoDayAgo);
                if (minDateToCollect != null) {
                    var historyData = Broker.LoadResult(minDateToCollect.Value, _sportType, _languageType);
                    ProjectProvider.Instance.BetProvider.SaveBrokerState(historyData, _algoMode, taskMode);
                    ProjectProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, minDateToCollect.Value, SystemStateResultType.CollectForTwoDayAgo);
                }
                return null;
            }
        }

        public object CollectHistoryForYesterday(RunTaskMode taskMode) {
            using (new MiniProfiler(GetProfilerString("CollectHistoryForYesterday"))) {
                var minDateToCollect = DateTime.UtcNow.AddDays(-1);
                if (minDateToCollect.Hour%3 == 0) {
                    var historyData = Broker.LoadResult(minDateToCollect, _sportType, _languageType);
                    ProjectProvider.Instance.BetProvider.SaveBrokerState(historyData, _algoMode, taskMode);
                    ProjectProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, minDateToCollect.Date, SystemStateResultType.CollectForYesterday);
                }
                return null;
            }
        }

        public object CollectHistoryForToday(RunTaskMode taskMode) {
            using (new MiniProfiler(GetProfilerString(taskMode.ToString()))) {
                var todayUtc = DateTime.UtcNow.Date;
                var historyData = Broker.LoadResult(todayUtc, _sportType, _languageType);
                ProjectProvider.Instance.BetProvider.SaveBrokerState(historyData, _algoMode, taskMode);
                ProjectProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, todayUtc, SystemStateResultType.CollectForToday);
                return null;
            }
        }

        private BrokerBase Broker => UseDbData ? new DbBrokerProvider(_brokerType) : BookPage.Instance.GetBrokerProvider(_brokerType);

        private string GetProfilerString(string runTaskMode) {
            return string.Format("{0} {1} {2}", _brokerType, _languageType, runTaskMode);
        }
    }
}