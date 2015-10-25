using System;
using CommonUtils.Code;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.Code.DataProvider;
using Project_B.Code.Enums;

namespace Project_B.Code.Algorythm {
    public class CollectOddsAlgo {
        const SportType _sportType = SportType.Basketball | SportType.Football | SportType.IceHockey | SportType.Tennis | SportType.Volleyball;

        public CollectOddsAlgo() {}

        public CollectOddsAlgo(TimeSpan regularOddsDelay) {
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(CollectRegularOdds, regularOddsDelay, null));
        }

        public object CollectRegularOdds() {
            using (var sw = new MiniProfiler("CollectRegularOdds")) {
                var utcNow = DateTime.UtcNow;
                if (MainProvider.Instance.BetProvider.GetStateRegular(BrokerType.RedBlue, utcNow) != SystemStateBetType.Unknown) {
                    var regularOdds = BookPage.Instance.GetOddsProvider(BrokerType.RedBlue).LoadRegular(_sportType);
                    MainProvider.Instance.BetProvider.SaveRegular(LanguageType.English, BrokerType.RedBlue, regularOdds);
                    MainProvider.Instance.BetProvider.SetStateRegular(BrokerType.RedBlue, utcNow);
                }
                return null;
            }
        }
    }
}