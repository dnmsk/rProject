using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.Code.Data;
using Project_B.Code.Entity;
using Project_B.Code.Enums;

namespace Project_B.Code.DataProvider {
    public class BetProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BetProvider).FullName);

        private static readonly SportType[] _sportWithAdvancedDetail = {
            SportType.Football, SportType.IceHockey,
        };

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

        public void AddBetParsed(int competitionItemID, BrokerType brokerType, SportType sportType, List<OddParsed> oddsParsed) {
            InvokeSafeSingleCall(() => {
                var betWithAdvancedDb = Bet.DataSource
                    .Join(JoinType.Left, BetAdvanced.Fields.BetID, Bet.Fields.ID, RetrieveMode.Retrieve)
                    .WhereEquals(Bet.Fields.CompetitionitemID, competitionItemID)
                    .WhereEquals(Bet.Fields.BrokerID, brokerType)
                    .Sort(Bet.Fields.ID, SortDirection.Desc)
                    .First();
                var betAdvancedDb = betWithAdvancedDb != null ? betWithAdvancedDb.GetJoinedEntity<BetAdvanced>() : null;

                var bet = Bet.GetBetFromOdds(oddsParsed);
                var betAdvanced = BetAdvanced.GetBetFromOdds(oddsParsed);

                if (betWithAdvancedDb == null || !betWithAdvancedDb.IsEqualsTo(bet) || 
                        _sportWithAdvancedDetail.Contains(sportType) && betAdvancedDb != null && betAdvanced != null && !betAdvancedDb.IsEqualsTo(betAdvanced)) {
                    bet.CompetitionitemID = competitionItemID;
                    bet.Save();
                    if (_sportWithAdvancedDetail.Contains(sportType) && betAdvanced != null) {
                        betAdvanced.BetID = bet.ID;
                        betAdvanced.Save();
                    }
                }
                return null;
            }, (object) null);
        }
    }
}