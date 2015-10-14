using System;
using System.Collections.Generic;
using Project_B.Code.Data;
using Project_B.Code.Enums;
using System.Linq;

namespace Project_B.Code.Entity.Interface {
    public static class BetHelper {
        private static readonly SportType[] _sportWithAdvancedDetail = {
            SportType.Football, SportType.IceHockey,
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

        public static IBet<T> GetBetFromOdds<T>(IBet<T> newBet,List<OddParsed> odds) {
            var hasAnyFactor = false;
            foreach (var odd in odds) {
                switch (odd.Type) {
                    case BetOddType.Win1:
                        newBet.Win1 = (float)odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Win2:
                        newBet.Win2 = (float)odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Handicap1:
                        newBet.Hcap1 = (float)odd.Factor;
                        newBet.Hcapdetail = (float)(odd.AdvancedParam ?? default(decimal));
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Handicap2:
                        newBet.Hcap2 = (float)odd.Factor;
                        newBet.Hcapdetail = (float)(odd.AdvancedParam ?? default(decimal));
                        hasAnyFactor = true;
                        break;
                    case BetOddType.TotalUnder:
                        newBet.Totalunder = (float)odd.Factor;
                        newBet.Totaldetail = (float)(odd.AdvancedParam ?? default(decimal));
                        hasAnyFactor = true;
                        break;
                    case BetOddType.TotalOver:
                        newBet.Totalover = (float)odd.Factor;
                        newBet.Totaldetail = (float)(odd.AdvancedParam ?? default(decimal));
                        hasAnyFactor = true;
                        break;
                }
            }
            return hasAnyFactor ? newBet : null;
        }

        public static bool IsEqualsTo<T>(this IBetAdvanced<T> t, IBetAdvanced<T> betAdvanced) {
            return t.Win1draw == betAdvanced.Win1draw
                && t.Win1win2 == betAdvanced.Win1win2
                && t.Drawwin2 == betAdvanced.Drawwin2;
        }

        public static IBetAdvanced<T> GetBetFromOdds<T>(IBetAdvanced<T> newBet, List<OddParsed> odds) {
            var hasAnyFactor = false;
            foreach (var odd in odds) {
                switch (odd.Type) {
                    case BetOddType.Win1Win2:
                        newBet.Win1win2 = (float)odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.DrawWin2:
                        newBet.Drawwin2 = (float)odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Win1Draw:
                        newBet.Win1draw = (float)odd.Factor;
                        hasAnyFactor = true;
                        break;
                }
            }
            return hasAnyFactor ? newBet : null;
        }

        public static void SaveBetIfChanged<T>(int competitionItemID, BrokerType brokerType, SportType sportType, IBet<T> newBet, IBetAdvanced<T> newBetAdvanced,  IBet<T> betDb, IBetAdvanced<T> betAdvancedDb) {
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
    }
}