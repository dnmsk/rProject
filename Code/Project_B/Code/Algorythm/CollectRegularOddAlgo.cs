using Project_B.Code.DataProvider;
using Project_B.Code.Enums;

namespace Project_B.Code.Algorythm {
    public class CollectRegularOddAlgo {
        const SportType _sportType = SportType.Basketball | SportType.Football | SportType.IceHockey | SportType.Tennis | SportType.Volleyball;
        public CollectRegularOddAlgo() {
            
        }

        public void Do() {
            var regularOdds = BookPage.Instance.GetOddsProvider(BrokerType.RedBlue).LoadRegular(_sportType);
            MainProvider.Instance.BetProvider.SaveRegular(LanguageType.English, BrokerType.RedBlue, regularOdds);
        }
    }
}