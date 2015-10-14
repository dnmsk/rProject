using System;
using CommonUtils.Code;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.Code.DataProvider;
using Project_B.Code.Enums;

namespace Project_B.Code.Algorythm {
    public class CollectLiveOddsWithResultAlgo {
        const SportType _sportType = SportType.Basketball | SportType.Football | SportType.IceHockey | SportType.Tennis | SportType.Volleyball;
        public CollectLiveOddsWithResultAlgo() : this(new TimeSpan(0, 0, 15)) {}

        public CollectLiveOddsWithResultAlgo(TimeSpan intervalAlgo) {
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectLiveOddsWithResult, intervalAlgo, null));
        }

        public object CollectLiveOddsWithResult() {
            using (var sw = new MiniProfiler("CollectLiveOddsWithResult")) {
                var liveData = BookPage.Instance
                    .GetOddsProvider(BrokerType.RedBlue)
                    .LoadLive(_sportType);
                MainProvider.Instance.LiveProvider.ProcessdLiveParsed(BrokerType.RedBlue, LanguageType.English, liveData);
                return null;
            }
        }
    }
}