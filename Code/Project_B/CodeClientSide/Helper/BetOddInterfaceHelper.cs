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

        public static float GetAverageOddValue(SportType sportType, BetOddType betOddType, List<Dictionary<BetOddType, BetItemTransport>> bets) {
            if (bets == null || !BetHelper.SportTypeWithOdds[sportType].Contains(betOddType)) {
                return default(int);
            }
            var betsToCalc = bets.Where(b => {
                BetItemTransport betItem;
                return b.TryGetValue(betOddType, out betItem) && betItem.Odd != default(float);
            }).ToArray();
            return betsToCalc.Any() ? betsToCalc.Average(b => b[betOddType].Odd) : default(float);
        }

        public static float GetMaxBetOddRoi(RoiType roiType, SportType sportType, Dictionary<BetOddType, BetItemTransport> bets) {
            return roiType.GetFlags<RoiType>().Select(r => GetBetOddRoi(r, sportType, bets)).Where(r => r != default(int)).MaxOrDefault(r => r, default(float));
        }

        public static float GetBetOddRoi(RoiType roiType, SportType sportType, List<Dictionary<BetOddType, BetItemTransport>> bets) {
            if (bets == null || bets.Count == 0) {
                return default(float);
            }
            var roiOdds = GetBetOddTypesForRoi(roiType, sportType);
            if (roiOdds.Length == 0) {
                return default(float);
            }
            var dataIsGood = roiOdds.All(rodd => bets.All(b => b.ContainsKey(rodd)));
            if (dataIsGood) {
                var mapByAdvancedParam = new Dictionary<float, List<KeyValuePair<BetOddType, BetItemTransport>>>();
                switch (roiType) {
                    case RoiType.RoiHandicap:/*
                        bets
                            .SelectMany(bet => roiOdds.Select(odd => new KeyValuePair<BetOddType, BetItemTransport>(odd, bet[odd])))
                            .Each(bet => {
                                List<KeyValuePair<BetOddType, BetItemTransport>> odds;
                                if (!mapByAdvancedParam.TryGetValue(bet.Value.AdvancedParam, out odds)) {
                                    odds = new List<KeyValuePair<BetOddType, BetItemTransport>>();
                                    mapByAdvancedParam[bet.Value.AdvancedParam] = odds;
                                }
                                odds.Add(bet);
                            });
                        var oddsByParam = new List<KeyValuePair<float, List<KeyValuePair<BetOddType, BetItemTransport>>>>();
                        var orderedTotal = mapByAdvancedParam.OrderByDescending(odd => odd.Key).ToArray();
                        for (var i = 0; i < orderedTotal.Length; i++) {
                            for (var j = i; j < orderedTotal.Length; j++) {
                                
                            }
                        }*/
                        return bets
                            .SelectMany(bet => roiOdds.Select(odd => new KeyValuePair<BetOddType, BetItemTransport>(odd, bet[odd])))
                            .GroupBy(bet => Math.Abs(bet.Value.AdvancedParam))
                            .MaxOrDefault(grouped => BetOddRoi(roiOdds, grouped.ToList()), default(float));
                    case RoiType.RoiTotal:
                        bets
                            .SelectMany(bet => roiOdds.Select(odd => new KeyValuePair<BetOddType, BetItemTransport>(odd, bet[odd])))
                            .Each(bet => {
                                List<KeyValuePair<BetOddType, BetItemTransport>> odds;
                                if (!mapByAdvancedParam.TryGetValue(bet.Value.AdvancedParam, out odds)) {
                                    odds = new List<KeyValuePair<BetOddType, BetItemTransport>>();
                                    mapByAdvancedParam[bet.Value.AdvancedParam] = odds;
                                }
                                odds.Add(bet);
                            });
                        var currentMaxRoi = float.MinValue;
                        List<KeyValuePair<BetOddType, BetItemTransport>> prevItem = null;
                        var ordered = mapByAdvancedParam.OrderBy(odd => odd.Key).ToArray();
                        for (var i = 0; i < ordered.Length; i++) {
                            var currentItem = ordered[i].Value;
                            currentMaxRoi = Math.Max(currentMaxRoi, BetOddRoi(roiOdds, currentItem));
                            if (prevItem != null) {
                                currentMaxRoi = Math.Max(currentMaxRoi, BetOddRoi(roiOdds, prevItem
                                    .Where(pi => pi.Key == BetOddType.TotalOver)
                                    .Union(currentItem.Where(ci => ci.Key == BetOddType.TotalUnder)).ToList()));
                            }
                            prevItem = currentItem;
                        }
                        return currentMaxRoi;
                    case RoiType.Roi12_X:
                    case RoiType.Roi1X2:
                    case RoiType.Roi1X_2:
                    case RoiType.Roi1_X2:
                        return BetOddRoi(roiOdds, bets
                            .SelectMany(bet => roiOdds.Select(odd => new KeyValuePair<BetOddType, BetItemTransport>(odd, bet[odd]))).ToList());
                }
            }
            return default(float);
        }

        private static float BetOddRoi(BetOddType[] roiOdds, List<KeyValuePair<BetOddType, BetItemTransport>> vals) {
            var internalDataGood = true;
            var oddInternal = new float[roiOdds.Length];
            for (var i = 0; i < roiOdds.Length; i++) {
                var roiOdd = roiOdds[i];
                var odd = vals.Where(v => v.Key == roiOdd).MaxOrDefault(v => v.Value.Odd, default(float));
                if (odd == default(float)) {
                    internalDataGood = false;
                    continue;
                }
                oddInternal[i] = 1/(odd);
            }
            return internalDataGood ? (100/oddInternal.Sum() - 100) : default(float);
        }

        public static float GetBetOddRoi(RoiType roiType, SportType sportType, Dictionary<BetOddType, BetItemTransport> bets) {
            if (bets == null) {
                return default(float);
            }
            return GetBetOddRoi(roiType, sportType, new List<Dictionary<BetOddType, BetItemTransport>> {bets});
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