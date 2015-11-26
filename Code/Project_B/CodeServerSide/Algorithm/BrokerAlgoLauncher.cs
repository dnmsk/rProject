﻿using System;
using CommonUtils.Code;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Algorithm {
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

        public BrokerAlgoLauncher(BrokerType brokerType, GatherBehaviorMode algoMode, LanguageType languageType, RunTaskMode runTaskMode, SportType sportType = SportType.All) {
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
            using (var sw = new MiniProfiler("CollectRegularOdds")) {
                var utcNow = DateTime.UtcNow;
                if (ProjectProvider.Instance.BetProvider.GetStateRegular(_brokerType, utcNow) != SystemStateBetType.Unknown) {
                    var regularOdds = BookPage.Instance.GetBrokerProvider(_brokerType).LoadRegular(_sportType, _languageType);
                    ProjectProvider.Instance.BetProvider.SaveRegular(regularOdds, _algoMode);
                    ProjectProvider.Instance.BetProvider.SetStateRegular(_brokerType, utcNow);
                }
                return null;
            }
        }

        public object CollectLiveOddsWithResult() {
            using (var sw = new MiniProfiler("CollectLiveOddsWithResult")) {
                var liveData = BookPage.Instance
                    .GetBrokerProvider(_brokerType)
                    .LoadLive(_sportType, _languageType);
                ProjectProvider.Instance.LiveProvider.ProcessdLiveParsed(liveData, _algoMode);
                return null;
            }
        }

        public object CollectHistoryForPastDate(DateTime? pastDate = null) {
            using (var sw = new MiniProfiler("CollectHistoryForPastDate")) {
                var minDateToCollect = pastDate ?? ProjectProvider.Instance.HistoryProvider.GetPastDateToCollect(_brokerType, _languageType, SystemStateResultType.CollectForTwoDayAgo);
                if (minDateToCollect != null) {
                    var historyData = BookPage.Instance
                                              .GetBrokerProvider(_brokerType)
                                              .LoadResult(minDateToCollect.Value, _sportType, _languageType);
                    ProjectProvider.Instance.HistoryProvider.SaveResult(historyData, _algoMode);
                    ProjectProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, minDateToCollect.Value, SystemStateResultType.CollectForTwoDayAgo);
                }
                return null;
            }
        }

        public object CollectHistoryForYesterday() {
            using (var sw = new MiniProfiler("CollectHistoryForYesterday")) {
                var minDateToCollect = DateTime.UtcNow.AddDays(-1);
                if (minDateToCollect.Hour%3 == 0) {
                    var historyData = BookPage.Instance
                                                .GetBrokerProvider(_brokerType)
                                                .LoadResult(minDateToCollect, _sportType, _languageType);
                    ProjectProvider.Instance.HistoryProvider.SaveResult(historyData, _algoMode);
                    ProjectProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, minDateToCollect.Date, SystemStateResultType.CollectForYesterday);
                }
                return null;
            }
        }

        public object CollectHistoryForToday() {
            using (var sw = new MiniProfiler("CollectHistoryForToday")) {
                var todayUtc = DateTime.UtcNow.Date;
                var historyData = BookPage.Instance
                    .GetBrokerProvider(_brokerType)
                    .LoadResult(todayUtc, _sportType, _languageType);
                ProjectProvider.Instance.HistoryProvider.SaveResult(historyData, _algoMode);
                ProjectProvider.Instance.HistoryProvider.SetDateCollectedWithState(_brokerType, _languageType, todayUtc, SystemStateResultType.CollectForToday);
                return null;
            }
        }
    }
}