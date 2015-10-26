using System;
using System.Collections.Generic;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.Code.Data;
using Project_B.Code.Data.Result;
using Project_B.Code.DataProvider.DataHelper;
using Project_B.Code.DataProvider.DataHelper.LiveResultToDbProc;
using Project_B.Code.Entity;
using Project_B.Code.Entity.Interface;
using Project_B.Code.Enums;

namespace Project_B.Code.DataProvider {
    public class LiveProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (LiveProvider).FullName);

        public LiveProvider() : base(_logger) {}

        public void ProcessdLiveParsed(BrokerData brokerData) {
            InvokeSafe(() => {
                foreach (var competitionParsed in brokerData.Competitions) {
                    var competition = MainProvider.Instance.CompetitionProvider.GetCompetition(brokerData.Language, competitionParsed.Type, competitionParsed.Name, competitionParsed);
                    foreach (var matchParsed in competitionParsed.Matches) {
                        var competitor1 = MainProvider.Instance.CompetitorProvider
                            .GetCompetitor(brokerData.Language, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullOne, matchParsed.CompetitorNameShortOne, competition.UniqueID, matchParsed);
                        var competitor2 = MainProvider.Instance.CompetitorProvider
                            .GetCompetitor(brokerData.Language, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullTwo, matchParsed.CompetitorNameShortTwo, competition.UniqueID, matchParsed);
                        var competitionItem = MainProvider.Instance.CompetitionProvider.GetCompetitionItem(competitor1, competitor2, competition, DateTime.MinValue);
                        AddLive(competitionItem, brokerData.Broker, competitionParsed.Type, matchParsed);
                    }
                }
            });
        }

        private void AddLive(int competitionItemID, BrokerType brokerType, SportType sportType, MatchParsed matchParsed) {
            InvokeSafe(() => {
                ProcessLiveOdds(competitionItemID, brokerType, sportType, matchParsed.Odds);
                ProcessLiveResult(competitionItemID, sportType, matchParsed.Result);
            });
        }



        private static void ProcessLiveResult(int competitionItemID, SportType sportType, FullResult result) {
            var lastResult = CompetitionResultLive.DataSource
                .Join(JoinType.Left, CompetitionResultLiveAdvanced.Fields.CompetitionresultliveID, CompetitionResultLive.Fields.ID, RetrieveMode.Retrieve)
                .WhereEquals(CompetitionResultLive.Fields.CompetitionitemID, competitionItemID)
                .Sort(CompetitionResultLive.Fields.ID, SortDirection.Desc)
                .Sort(CompetitionResultLiveAdvanced.Fields.ID, SortDirection.Desc)
                .First();
            var generateScoreID = ScoreHelper.Instance.GenerateScoreID(result.CompetitorResultOne, result.CompetitorResultTwo);
            if (lastResult == null || generateScoreID != lastResult.ScoreID) {
                lastResult = new CompetitionResultLive {
                    CompetitionitemID = competitionItemID,
                    ScoreID = generateScoreID,
                    Datecreatedutc = DateTime.UtcNow
                };
                lastResult.Save();
            }
            var lastAdvancedResult = lastResult.GetJoinedEntity<CompetitionResultLiveAdvanced>();
            LiveResultProcFactory.GetLiveResultProc(sportType)
                                 .Process(lastResult, lastAdvancedResult, result);
        }

        private static void ProcessLiveOdds(int competitionItemID, BrokerType brokerType, SportType sportType, List<OddParsed> odds) {
            if (odds == null || odds.Count == 0) {
                return;
            }
            var betWithAdvancedDb = BetLive.DataSource
                .Join(JoinType.Left, BetLiveAdvanced.Fields.BetliveID, BetLive.Fields.ID, RetrieveMode.Retrieve)
                .WhereEquals(BetLive.Fields.BrokerID, (short) brokerType)
                .WhereEquals(BetLive.Fields.CompetitionitemID, competitionItemID)
                .Sort(BetLive.Fields.ID, SortDirection.Desc)
                .Sort(BetLiveAdvanced.Fields.ID, SortDirection.Desc)
                .First();
            var betAdvancedDb = betWithAdvancedDb != null ? betWithAdvancedDb.GetJoinedEntity<BetLiveAdvanced>() : null;

            var bet = BetHelper.GetBetFromOdds(new BetLive(), odds);
            var betAdvanced = BetHelper.GetBetFromOdds(new BetLiveAdvanced(), odds);

            BetHelper.SaveBetIfChanged(competitionItemID, brokerType, sportType, bet, betAdvanced, betWithAdvancedDb, betAdvancedDb);
        }
    }
}