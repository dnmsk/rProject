using System;
using System.Collections.Generic;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.Code.Data;
using Project_B.Code.Entity;
using Project_B.Code.Entity.Interface;
using Project_B.Code.Enums;

namespace Project_B.Code.DataProvider {
    public class BetProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BetProvider).FullName);

        public BetProvider() : base(_logger) { }

        public void SaveRegular(LanguageType languageType, BrokerType brokerType, List<CompetitionParsed> competitionToSave) {
            InvokeSafe(() => {
                foreach (var competitionParsed in competitionToSave) {
                    var competition = MainProvider.Instance.CompetitionProvider.GetCompetition(languageType, competitionParsed.Type, competitionParsed.Name);
                    foreach (var matchParsed in competitionParsed.Matches) {
                        var competitor1 = MainProvider.Instance.CompetitorProvider
                            .GetCompetitor(languageType, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullOne, matchParsed.CompetitorNameShortOne);
                        var competitor2 = MainProvider.Instance.CompetitorProvider
                            .GetCompetitor(languageType, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullTwo, matchParsed.CompetitorNameShortTwo);
                        var competitionItem = MainProvider.Instance.CompetitionProvider.GetCompetitionItem(competitor1, competitor2, competition, matchParsed.DateUtc);
                        AddBetParsed(competitionItem, brokerType, competitionParsed.Type, matchParsed.Odds);
                    }
                }
            });
        }

        private void AddBetParsed(int competitionItemID, BrokerType brokerType, SportType sportType, List<OddParsed> oddsParsed) {
            InvokeSafeSingleCall(() => {
                var betWithAdvancedDb = Bet.DataSource
                    .Join(JoinType.Left, BetAdvanced.Fields.BetID, Bet.Fields.ID, RetrieveMode.Retrieve)
                    .WhereEquals(Bet.Fields.CompetitionitemID, competitionItemID)
                    .WhereEquals(Bet.Fields.BrokerID, (short) brokerType)
                    .Sort(Bet.Fields.ID, SortDirection.Desc)
                    .First();
                var betAdvancedDb = betWithAdvancedDb != null ? betWithAdvancedDb.GetJoinedEntity<BetAdvanced>() : null;

                var bet = BetHelper.GetBetFromOdds(new Bet(), oddsParsed);
                var betAdvanced = BetHelper.GetBetFromOdds(new BetAdvanced(), oddsParsed);

                BetHelper.SaveBetIfChanged(competitionItemID, brokerType, sportType, bet, betAdvanced, betWithAdvancedDb, betAdvancedDb);
                return (object)null;
            });
        }

        private static readonly Dictionary<short, SystemStateBetType> _betTypeByHour = new Dictionary<short, SystemStateBetType> {
            { 0, SystemStateBetType.BetFor0_1 },
            { 1, SystemStateBetType.BetFor1_2 },
            { 2, SystemStateBetType.BetFor2_3 },
            { 3, SystemStateBetType.BetFor3_4 },
            { 4, SystemStateBetType.BetFor4_5 },
            { 5, SystemStateBetType.BetFor5_6 },
            { 6, SystemStateBetType.BetFor6_7 },
            { 7, SystemStateBetType.BetFor7_8 },
            { 8, SystemStateBetType.BetFor8_9 },
            { 9, SystemStateBetType.BetFor9_10 },
            { 10, SystemStateBetType.BetFor10_11 },
            { 11, SystemStateBetType.BetFor11_12 },
            { 12, SystemStateBetType.BetFor12_13 },
            { 13, SystemStateBetType.BetFor13_14 },
            { 14, SystemStateBetType.BetFor14_15 },
            { 15, SystemStateBetType.BetFor15_16 },
            { 16, SystemStateBetType.BetFor16_17 },
            { 17, SystemStateBetType.BetFor17_18 },
            { 18, SystemStateBetType.BetFor18_19 },
            { 19, SystemStateBetType.BetFor19_20 },
            { 20, SystemStateBetType.BetFor20_21 },
            { 21, SystemStateBetType.BetFor21_22 },
            { 22, SystemStateBetType.BetFor22_23 },
            { 23, SystemStateBetType.BetFor23_24 },
        };

        public SystemStateBetType GetStateRegular(BrokerType brokerType, DateTime dateUtc) {
            return InvokeSafe(() => {
                var state = _betTypeByHour[(short)dateUtc.Hour];
                var systemState = SystemStateBet.DataSource
                    .WhereEquals(SystemStateBet.Fields.Dateutc, dateUtc.Date)
                    .WhereEquals(SystemStateBet.Fields.Brokerid, (short) brokerType)
                    .First();
                if (systemState == null) {
                    systemState = new SystemStateBet {
                        Dateutc = dateUtc.Date,
                        BrokerID = brokerType,
                        Statebet = SystemStateBetType.Unknown
                    };
                    systemState.Save();
                }
                return (systemState.Statebet & state) != 0 ? SystemStateBetType.Unknown : state;
            }, SystemStateBetType.Unknown);
        }

        public void SetStateRegular(BrokerType brokerType, DateTime dateUtc) {
            InvokeSafe(() => {
                var state = _betTypeByHour[(short)dateUtc.Hour];
                var systemState = SystemStateBet.DataSource
                    .WhereEquals(SystemStateBet.Fields.Dateutc, dateUtc.Date)
                    .WhereEquals(SystemStateBet.Fields.Brokerid, (short) brokerType)
                    .First();
                if (systemState == null) {
                    systemState = new SystemStateBet {
                        Dateutc = dateUtc.Date,
                        BrokerID = brokerType,
                        Statebet = SystemStateBetType.Unknown
                    };
                }
                systemState.Statebet |= state;
                systemState.Save();
                return null;
            }, (object) null);
        }
    }
}