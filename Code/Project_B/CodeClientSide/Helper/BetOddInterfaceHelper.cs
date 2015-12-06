using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.Helper {
    public static class BetOddInterfaceHelper {
        private static readonly BetOddType[] _short1X2Roi = {
            BetOddType.Win1,
            BetOddType.Win2
        };
        private static readonly BetOddType[] _full1X2Roi = {
            BetOddType.Win1,
            BetOddType.Draw,
            BetOddType.Win2
        };
        private static readonly BetOddType[] _roiHandicap = {
            BetOddType.Handicap1,
            BetOddType.Handicap2
        };
        private static readonly BetOddType[] _roiTotal = {
            BetOddType.TotalUnder,
            BetOddType.TotalOver
        };
        // ReSharper disable once InconsistentNaming
        private static readonly BetOddType[] _roi1X_2 = {
            BetOddType.Win1Draw,
            BetOddType.Win2
        };
        // ReSharper disable once InconsistentNaming
        private static readonly BetOddType[] _roi12_X = {
            BetOddType.Win1Win2,
            BetOddType.Draw
        };
        // ReSharper disable once InconsistentNaming
        private static readonly BetOddType[] _roi1_X2 = {
            BetOddType.Win1,
            BetOddType.DrawWin2
        };
        private static readonly Dictionary<SportType, BetOddType[]> _roi1X2OddMapBySportTypes =
            new Dictionary<SportType, BetOddType[]> {
                {SportType.Basketball, _short1X2Roi},
                {SportType.Football, _full1X2Roi},
                {SportType.IceHockey, _full1X2Roi},
                {SportType.Tennis, _short1X2Roi},
                {SportType.Volleyball, _short1X2Roi}
            };
        private static readonly SportType[] _sportTypesTotalScoreBySum = { SportType.Tennis, SportType.Volleyball };

        public static readonly Dictionary<BetOddType, SiteTextType> BetOddNames = new Dictionary<BetOddType, SiteTextType> {
            { BetOddType.Win1, SiteTextType.GridOddTitleWin1 },
            { BetOddType.Draw, SiteTextType.GridOddTitleDraw },
            { BetOddType.Win2, SiteTextType.GridOddTitleWin2 },
            { BetOddType.Win1Draw, SiteTextType.GridOddTitleWin1Draw },
            { BetOddType.Win1Win2, SiteTextType.GridOddTitleWin1Win2 },
            { BetOddType.DrawWin2, SiteTextType.GridOddTitleDrawWin2 },
            { BetOddType.Handicap1, SiteTextType.GridOddTitleHcap1 },
            { BetOddType.Handicap2, SiteTextType.GridOddTitleHcap2 },
            { BetOddType.TotalUnder, SiteTextType.GridOddTitleUnder },
            { BetOddType.TotalOver, SiteTextType.GridOddTitleOver }, 
        }; 

        public static readonly Dictionary<RoiType, SiteTextType> RoiTexts = new Dictionary<RoiType, SiteTextType> {
            {RoiType.Roi1X2, SiteTextType.GameRoi1X2 },
            {RoiType.Roi1X_2, SiteTextType.GameRoi1X_2 },
            {RoiType.Roi12_X, SiteTextType.GameRoi12_X },
            {RoiType.Roi1_X2, SiteTextType.GameRoi1_X2 },
            {RoiType.RoiHandicap, SiteTextType.GameRoiHandicap },
            {RoiType.RoiTotal, SiteTextType.GameRoiTotal },
        };

        public static float GetMaxBetOddRoi(RoiType roiType, SportType sportType, Dictionary<BetOddType, BetItemTransport> bets) {
            return roiType.GetFlags<RoiType>().Select(r => GetBetOddRoi(r, sportType, bets)).Where(r => r != default(int)).MaxOrDefault(r => r, default(float));
        }

        public static float GetBetOddRoi(RoiType roiType, SportType sportType, Dictionary<BetOddType, BetItemTransport> bets) {
            if (bets == null) {
                return default(int);
            }
            var roiOdds = GetBetOddTypesForRoi(roiType, sportType);
            var odds = new float[roiOdds.Length];
            var dataIsGood = roiOdds.All(bets.ContainsKey) && roiOdds.Select(betType => bets[betType].AdvancedParam)
                                                                     .Distinct()
                                                                     .Count() == 1;
            if (dataIsGood) {
                for (var i = 0; i < roiOdds.Length; i++) {
                    var odd = bets[roiOdds[i]].Odd;
                    if (odd == default(float)) {
                        dataIsGood = false;
                        continue;
                    }
                    odds[i] = 1 / (odd);
                }
            }
            return dataIsGood ? (100 / odds.Sum() - 100) : default(int);
        }

        public static BetOddType[] GetBetOddTypesForRoi(RoiType roiType, SportType sportType) {
            switch (roiType) {
                case RoiType.Roi1X2:
                    return _roi1X2OddMapBySportTypes[sportType];
                case RoiType.RoiHandicap:
                    return _roiHandicap;
                case RoiType.RoiTotal:
                    return _roiTotal;
                case RoiType.Roi1X_2:
                    return _roi1X_2;
                case RoiType.Roi12_X:
                    return _roi12_X;
                case RoiType.Roi1_X2:
                    return _roi1_X2;
                default:
                    return new BetOddType[0];
            }
        }

        public static List<BetOddType> GetSuccessOddTypes(SportType sportType, ResultTransport resultTransport, Dictionary<BetOddType, BetItemTransport> currentBets) {
            var resultScore = ScoreHelper.Instance.GenerateScore(resultTransport.ScoreID);
            var successActs = new List<BetOddType> {ScoreHelper.Instance.GetResultType(resultScore.Item1, resultScore.Item2)};
            var totalScore = Tuple.Create(resultScore.Item1, resultScore.Item2);
            if (_sportTypesTotalScoreBySum.Contains(sportType)) {
                totalScore = Tuple.Create((short) 0, (short) 0);
                resultTransport.SubScore.Select(ScoreHelper.Instance.GenerateScore).Each(tuple => { totalScore = Tuple.Create((short) (totalScore.Item1 + tuple.Item1), (short) (totalScore.Item2 + tuple.Item2)); });
            }
            if (currentBets != null) {
                BetItemTransport betItemTransport;
                if (currentBets.TryGetValue(BetOddType.Handicap1, out betItemTransport)) {
                    var advancedParam = betItemTransport.AdvancedParam;
                    successActs.Add(advancedParam < totalScore.Item1 - totalScore.Item2 
                        ? BetOddType.Handicap1 
                        : advancedParam > totalScore.Item1 - totalScore.Item2 
                            ? BetOddType.Handicap2 
                            : BetOddType.Unknown);
                }
                if (currentBets.TryGetValue(BetOddType.TotalUnder, out betItemTransport)) {
                    successActs.Add(betItemTransport.AdvancedParam > totalScore.Item1 + totalScore.Item2 
                        ? BetOddType.TotalUnder 
                        : betItemTransport.AdvancedParam < totalScore.Item1 + totalScore.Item2 
                            ? BetOddType.TotalOver 
                            : BetOddType.Unknown);
                }
            }
            return successActs;
        }
    }
}