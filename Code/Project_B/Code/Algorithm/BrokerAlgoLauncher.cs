using System;
using CommonUtils.Code;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.Code.DataProvider;
using Project_B.Code.Enums;

namespace Project_B.Code.Algorithm {
    public class BrokerAlgoLauncher {
        private readonly BrokerType _brokerType;
        private readonly GatherBehaviorMode _algoMode;
        private readonly LanguageType _languageType;
        private readonly RunTaskMode _runTaskMode;
        private readonly SportType _sportType;
        
        public TimeSpan TodayHistoryTaskTimespan = new TimeSpan(1, 0, 0);
        public TimeSpan PastDateHistoryTaskTimespan = new TimeSpan(4, 0, 0);
        public TimeSpan LiveOddsTaskTimespan = new TimeSpan(0, 0, 15);
        public TimeSpan RegularOddsTaskTimespan = new TimeSpan(0, 5, 0);

        public BrokerAlgoLauncher(BrokerType brokerType, GatherBehaviorMode algoMode, LanguageType languageType, RunTaskMode runTaskMode, SportType sportType = SportType.Basketball | SportType.Football | SportType.IceHockey | SportType.Tennis | SportType.Volleyball) {
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
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectHistoryForPastDate, PastDateHistoryTaskTimespan, null));
            }
            if (_runTaskMode.HasFlag(RunTaskMode.RunLiveOddsTask)) {
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectLiveOddsWithResult, LiveOddsTaskTimespan, null));
            }
            if (_runTaskMode.HasFlag(RunTaskMode.RunRegularOddsTask)) {
                MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectRegularOdds, RegularOddsTaskTimespan, null));
            }
        }

        public object CollectRegularOdds() {
            using (var sw = new MiniProfiler("CollectRegularOdds")) {
                var utcNow = DateTime.UtcNow;
                if (MainProvider.Instance.BetProvider.GetStateRegular(_brokerType, utcNow) != SystemStateBetType.Unknown) {
                    var regularOdds = BookPage.Instance.GetOddsProvider(_brokerType).LoadRegular(_sportType, _languageType);
                    MainProvider.Instance.BetProvider.SaveRegular(regularOdds, _algoMode);
                    MainProvider.Instance.BetProvider.SetStateRegular(_brokerType, utcNow);
                }
                return null;
            }
        }

        public object CollectLiveOddsWithResult() {
            using (var sw = new MiniProfiler("CollectLiveOddsWithResult")) {
                var liveData = BookPage.Instance
                    .GetOddsProvider(_brokerType)
                    .LoadLive(_sportType, _languageType);
                MainProvider.Instance.LiveProvider.ProcessdLiveParsed(liveData, _algoMode);
                return null;
            }
        }

        public object CollectHistoryForPastDate() {
            using (var sw = new MiniProfiler("CollectHistoryForPastDate")) {
                var minDateToCollect = MainProvider.Instance.HistoryProvider.GetPastDateToCollect(_brokerType, _languageType);
                if (minDateToCollect != null) {
                    var historyData = BookPage.Instance
                                              .GetHistoryProvider(_brokerType)
                                              .Load(minDateToCollect.Value, _sportType, _languageType);
                    MainProvider.Instance.HistoryProvider.SaveResult(historyData, _algoMode);
                    MainProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, minDateToCollect.Value, SystemStateResultType.CollectForYesterday);
                }
                return null;
            }
        }

        public object CollectHistoryForToday() {
            using (var sw = new MiniProfiler("CollectHistoryForToday")) {
                var todayUtc = DateTime.UtcNow.Date;
                var historyData = BookPage.Instance
                    .GetHistoryProvider(_brokerType)
                    .Load(todayUtc, _sportType, _languageType);
                MainProvider.Instance.HistoryProvider.SaveResult(historyData, _algoMode);
                MainProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, todayUtc, SystemStateResultType.CollectForToday);
                return null;
            }
        }
    }
}