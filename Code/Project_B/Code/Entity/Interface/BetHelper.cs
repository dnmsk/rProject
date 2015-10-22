using System;
using System.Collections.Generic;
using System.Linq;
using Project_B.Code.Data;
using Project_B.Code.Enums;
using Project_B.Models;

namespace Project_B.Code.Entity.Interface {
    public static class BetHelper {
        private static readonly SportType[] _sportWithAdvancedDetail = {
            SportType.Football, SportType.IceHockey
        };

        public static bool IsEqualsTo<T>(this IBet<T> t, IBet<T> bet) {
            return t.Win1 == bet.Win1
                   && t.Win2 == bet.Win2
                   && t.Hcap1 == bet.Hcap1
                   && t.Hcap2 == bet.Hcap2
                   && t.Hcapdetail == bet.Hcapdetail
                   && t.Totalunder == bet.Totalunder
                   && t.Totalover == bet.Totalover
                   && t.Totaldetail == bet.Totaldetail;
        }

        public static IBet<T> GetBetFromOdds<T>(IBet<T> newBet, List<OddParsed> odds) {
            var hasAnyFactor = false;
            foreach (var odd in odds) {
                switch (odd.Type) {
                    case BetOddType.Win1:
                        newBet.Win1 = odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Win2:
                        newBet.Win2 = odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Handicap1:
                        newBet.Hcap1 = odd.Factor;
                        newBet.Hcapdetail = (odd.AdvancedParam ?? default(float));
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Handicap2:
                        newBet.Hcap2 = odd.Factor;
                        newBet.Hcapdetail = (odd.AdvancedParam ?? default(float));
                        hasAnyFactor = true;
                        break;
                    case BetOddType.TotalUnder:
                        newBet.Totalunder = odd.Factor;
                        newBet.Totaldetail = (odd.AdvancedParam ?? default(float));
                        hasAnyFactor = true;
                        break;
                    case BetOddType.TotalOver:
                        newBet.Totalover = odd.Factor;
                        newBet.Totaldetail = odd.AdvancedParam ?? default(float);
                        hasAnyFactor = true;
                        break;
                }
            }
            return hasAnyFactor ? newBet : null;
        }

        public static bool IsEqualsTo<T>(this IBetAdvanced<T> t, IBetAdvanced<T> betAdvanced) {
            return t.Win1draw == betAdvanced.Win1draw
                   && t.Draw == betAdvanced.Draw
                   && t.Win1win2 == betAdvanced.Win1win2
                   && t.Drawwin2 == betAdvanced.Drawwin2;
        }

        public static IBetAdvanced<T> GetBetFromOdds<T>(IBetAdvanced<T> newBet, List<OddParsed> odds) {
            var hasAnyFactor = false;
            foreach (var odd in odds) {
                switch (odd.Type) {
                    case BetOddType.Win1Win2:
                        newBet.Win1win2 = odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.DrawWin2:
                        newBet.Drawwin2 = odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Win1Draw:
                        newBet.Win1draw = odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Draw:
                        newBet.Draw = odd.Factor;
                        hasAnyFactor = true;
                        break;
                }
            }
            return hasAnyFactor ? newBet : null;
        }

        public static void SaveBetIfChanged<T>(int competitionItemID, BrokerType brokerType, SportType sportType,
            IBet<T> newBet, IBetAdvanced<T> newBetAdvanced, IBet<T> betDb, IBetAdvanced<T> betAdvancedDb) {
            if (newBet != null) {
                if (betDb == null || !betDb.IsEqualsTo(newBet) ||
                    _sportWithAdvancedDetail.Contains(sportType) && betAdvancedDb != null && newBetAdvanced != null &&
                    !betAdvancedDb.IsEqualsTo(newBetAdvanced)) {
                    newBet.CompetitionitemID = competitionItemID;
                    newBet.BrokerID = brokerType;
                    newBet.Datecreatedutc = DateTime.UtcNow;
                    newBet.Save();
                    if (_sportWithAdvancedDetail.Contains(sportType) && newBetAdvanced != null) {
                        newBetAdvanced.BetID = newBet.ID;
                        newBetAdvanced.Save();
                    }
                }
            }
        }
        
        public static readonly Dictionary<BetOddType, Func<Bet, BetItem>> OddsGetterMap = new Dictionary<BetOddType, Func<Bet, BetItem>> {
            {BetOddType.Win1, bet => FillBetItem(BuildBetItem(bet), bet.Win1, null) },
            {BetOddType.Win2, bet => FillBetItem(BuildBetItem(bet), bet.Win2, null) },
            {BetOddType.Handicap1, bet => FillBetItem(BuildBetItem(bet), bet.Hcap1, bet.Hcapdetail) },
            {BetOddType.Handicap2, bet => FillBetItem(BuildBetItem(bet), bet.Hcap2, bet.Hcapdetail) },
            {BetOddType.TotalUnder, bet => FillBetItem(BuildBetItem(bet), bet.Totalunder, bet.Totaldetail) },
            {BetOddType.TotalOver, bet => FillBetItem(BuildBetItem(bet), bet.Totalover, bet.Totaldetail) },
            {BetOddType.Win1Draw, bet => ReturnIfNotNull(advanced => FillBetItem(BuildBetItem(bet), advanced.Win1draw, null), bet) },
            {BetOddType.Win1Win2, bet => ReturnIfNotNull(advanced => FillBetItem(BuildBetItem(bet), advanced.Win1win2, null), bet) },
            {BetOddType.DrawWin2, bet => ReturnIfNotNull(advanced => FillBetItem(BuildBetItem(bet), advanced.Drawwin2, null), bet) },
            {BetOddType.Draw, bet => ReturnIfNotNull(advanced => FillBetItem(BuildBetItem(bet), advanced.Draw, null), bet) }
        };

        private static BetItem ReturnIfNotNull(Func<BetAdvanced, BetItem> func, Bet bet) {
            var betAdvanced = bet.GetJoinedEntity<BetAdvanced>();
            return betAdvanced == null ? new BetItem() : func(betAdvanced);
        }

        private static BetItem BuildBetItem(Bet bet) {
            return new BetItem {
                BrokerType = bet.BrokerID,
                DateTimeUtc = bet.Datecreatedutc
            };
        }

        private static BetItem FillBetItem(BetItem betItem, float? value, float? advanced) {
            betItem.Odd = value ?? default(float);
            betItem.AdvancedParam = advanced ?? default(float);
            return betItem;
        }
        private static readonly BetOddType[] _standartOdds = {
            BetOddType.Win1,
            BetOddType.Win2,
            BetOddType.Handicap1,
            BetOddType.Handicap2,
            BetOddType.TotalUnder,
            BetOddType.TotalOver
        };

        private static readonly BetOddType[] _advancedOdds = {
            BetOddType.Win1,
            BetOddType.Draw,
            BetOddType.Win2,
            BetOddType.Win1Draw,
            BetOddType.Win1Win2,
            BetOddType.DrawWin2,
            BetOddType.Handicap1,
            BetOddType.Handicap2,
            BetOddType.TotalUnder,
            BetOddType.TotalOver
        };

        public static readonly Dictionary<SportType, BetOddType[]> SportTypeWithOdds = new Dictionary<SportType, BetOddType[]> {
            {SportType.Basketball, _standartOdds },
            {SportType.IceHockey, _advancedOdds },
            {SportType.Football, _advancedOdds },
            {SportType.Tennis, _standartOdds },
            {SportType.Volleyball, _standartOdds },
        };

    }
}