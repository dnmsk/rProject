using System;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.Code.DataProvider;
using Project_B.Code.Enums;

namespace Project_B.Code.Algorythm {
    public class CollectHistoryAlgo {
        const SportType _sportType = SportType.Basketball | SportType.Football | SportType.IceHockey | SportType.Tennis | SportType.Volleyball;

        public CollectHistoryAlgo() : this(new TimeSpan(0, 1, 0), new TimeSpan(1, 0, 0)) { }

        public CollectHistoryAlgo(TimeSpan pastCollectRepeatDelay, TimeSpan todayCollectRepeatDealy) {
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectHistoryForPastDate, pastCollectRepeatDelay, null));
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectHistoryForToday, todayCollectRepeatDealy, null));
        }
        
        private object CollectHistoryForPastDate() {
            var minDateToCollect = MainProvider.Instance.HistoryProvider.GetPastDateToCollect();
            if (minDateToCollect != null) {
                var historyData = BookPage.Instance
                    .GetHistoryProvider(BrokerType.RedBlue)
                    .Load(minDateToCollect.Value, _sportType);
                MainProvider.Instance.HistoryProvider.SaveResult(LanguageType.English, historyData);
                MainProvider.Instance.HistoryProvider.SetDateCollectedWithState(minDateToCollect.Value, SystemStateResultType.CollectForYesterday);
            }
            return null;
        }

        private object CollectHistoryForToday() {
            var todayUtc = DateTime.UtcNow.Date;
            var historyData = BookPage.Instance
                .GetHistoryProvider(BrokerType.RedBlue)
                .Load(todayUtc, _sportType);
            MainProvider.Instance.HistoryProvider.SaveResult(LanguageType.English, historyData);
            MainProvider.Instance.HistoryProvider.SetDateCollectedWithState(todayUtc, SystemStateResultType.CollectForToday);
            return null;
        }
    }
}