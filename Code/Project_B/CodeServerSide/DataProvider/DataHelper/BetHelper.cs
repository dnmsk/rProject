using System;
using System.Collections.Generic;
using System.Linq;
using IDEV.Hydra.DAO;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class BetHelper {
        private static readonly SportType[] _sportWithAdvancedDetail = {
            SportType.Football, SportType.IceHockey
        };

        public static bool IsEqualsTo<T>(this IBet<T> t, IBet<T> bet) {
            return t != null
                   && bet != null
                   && t.Win1 == bet.Win1
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
                if (odd.Factor == default(float)) {
                    continue;
                }
                switch (odd.Type) {
                    case BetOddType.Win1:
                        newBet.Win1 = odd.Factor;
                        break;
                    case BetOddType.Win2:
                        newBet.Win2 = odd.Factor;
                        break;
                    case BetOddType.Handicap1:
                        newBet.Hcap1 = odd.Factor;
                        break;
                    case BetOddType.Handicap2:
                        newBet.Hcap2 = odd.Factor;
                        newBet.Hcapdetail = (odd.AdvancedParam ?? default(float));
                        break;
                    case BetOddType.TotalUnder:
                        newBet.Totalunder = odd.Factor;
                        break;
                    case BetOddType.TotalOver:
                        newBet.Totalover = odd.Factor;
                        newBet.Totaldetail = odd.AdvancedParam ?? default(float);
                        break;
                    default:
                        continue;
                }
                hasAnyFactor = true;
            }
            return hasAnyFactor ? newBet : null;
        }

        public static bool IsEqualsTo<T>(this IBetAdvanced<T> t, IBetAdvanced<T> betAdvanced) {
            return t != null
                   && betAdvanced != null
                   && t.Win1draw == betAdvanced.Win1draw
                   && t.Draw == betAdvanced.Draw
                   && t.Win1win2 == betAdvanced.Win1win2
                   && t.Drawwin2 == betAdvanced.Drawwin2;
        }

        public static IBetAdvanced<T> GetBetFromOdds<T>(IBetAdvanced<T> newBet, List<OddParsed> odds) {
            var hasAnyFactor = false;
            foreach (var odd in odds) {
                if (odd.Factor == default(int)) {
                    continue;
                }
                switch (odd.Type) {
                    case BetOddType.Win1Win2:
                        newBet.Win1win2 = odd.Factor;
                        break;
                    case BetOddType.DrawWin2:
                        newBet.Drawwin2 = odd.Factor;
                        break;
                    case BetOddType.Win1Draw:
                        newBet.Win1draw = odd.Factor;
                        break;
                    case BetOddType.Draw:
                        newBet.Draw = odd.Factor;
                        break;
                    default:
                        continue;
                }
                hasAnyFactor = true;
            }
            return hasAnyFactor ? newBet : null;
        }

        public static bool SaveBetIfChanged<T>(ProcessStat processStat, int competitionItemID, BrokerType brokerType, SportType sportType,
            IBet<T> newBet, IBetAdvanced<T> newBetAdvanced, IBet<T> betDb, IBetAdvanced<T> betAdvancedDb) {
            if (newBet == null) {
                return false;
            }
            var mustHaveBetAdvanced = _sportWithAdvancedDetail.Contains(sportType);
            if (!mustHaveBetAdvanced && newBetAdvanced != null) {
                return false;
            }
            var canCreateNewBetAdvanced = mustHaveBetAdvanced && newBetAdvanced != null;
            var createNewBet = (!betDb?.IsActive ?? !betDb.IsEqualsTo(newBet)) ||
                               canCreateNewBetAdvanced && !betAdvancedDb.IsEqualsTo(newBetAdvanced);
            if (createNewBet) {
                processStat.CreateOriginalCount++;
                newBet.CompetitionitemID = competitionItemID;
                newBet.BrokerID = brokerType;
            } else {
                newBet = betDb;
            }
            newBet.Datecreatedutc = DateTime.UtcNow;
            newBet.Save();
            if (canCreateNewBetAdvanced && createNewBet) {
                newBetAdvanced.ID = newBet.ID;
                newBetAdvanced.Insert();
            }
            return true;
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

        public static Dictionary<int, List<IBet<int>>> GetBetMap(IEnumerable<int> competitionItemIDs, BrokerType[] brokerTypesToRetreive, bool onlyActive) {
            var bets = Bet.DataSource
                .Join(JoinType.Left, BetAdvanced.Fields.ID, Bet.Fields.ID, RetrieveMode.Retrieve)
                .WhereIn(Bet.Fields.CompetitionitemID, competitionItemIDs);
            if (brokerTypesToRetreive != null && brokerTypesToRetreive.Any()) {
                bets = bets.WhereIn(Bet.Fields.BrokerID, brokerTypesToRetreive);
            }
            if (onlyActive) {
                bets = bets.WhereTrue(Bet.Fields.IsActive);
            }
            return bets
                .AsList()
                .GroupBy(e => e.CompetitionitemID)
                .ToDictionary(e => e.Key, e => e.Select(t => (IBet<int>)t)
                .OrderByDescending(t => t.ID)
                .ToList());
        }
        public static Dictionary<int, List<IBet<int>>> GetBetMapNew(IEnumerable<int> betIDs, bool onlyActive) {
            var bets = Bet.DataSource
                .Join(JoinType.Left, BetAdvanced.Fields.ID, Bet.Fields.ID, RetrieveMode.Retrieve)
                .WhereIn(Bet.Fields.ID, betIDs);
            if (onlyActive) {
                bets = bets.WhereTrue(Bet.Fields.IsActive);
            }
            return bets
                .AsList()
                .GroupBy(e => e.CompetitionitemID)
                .ToDictionary(e => e.Key, e => e.Select(t => (IBet<int>)t)
                .OrderByDescending(t => t.ID)
                .ToList());
        }

        public static Dictionary<int, List<IBet<long>>> GetLiveBetMap(IEnumerable<int> competitionItemIDs, BrokerType[] brokerTypesToRetreive, bool onlyActive) {
            var bets = BetLive.DataSource
                .Join(JoinType.Left, BetLiveAdvanced.Fields.ID, BetLive.Fields.ID, RetrieveMode.Retrieve)
                .WhereIn(BetLive.Fields.CompetitionitemID, competitionItemIDs);
            if (brokerTypesToRetreive != null && brokerTypesToRetreive.Any()) {
                bets = bets.WhereIn(BetLive.Fields.BrokerID, brokerTypesToRetreive);
            }
            if (onlyActive) {
                bets = bets.WhereTrue(BetLive.Fields.IsActive);
            }
            return bets
                .AsList()
                .GroupBy(e => e.CompetitionitemID)
                .ToDictionary(e => e.Key, e => e.Select(t => (IBet<long>)t)
                .ToList());
        }

    }
}