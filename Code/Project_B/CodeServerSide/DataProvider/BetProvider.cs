using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class BetProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BetProvider).FullName);

        public BetProvider() : base(_logger) { }

        public SummaryProcessStat SaveBrokerState(BrokerData brokerData, GatherBehaviorMode algoMode, RunTaskMode taskMode) {
            return InvokeSafe(() => {
                var activeCompetitionItemIDs = new List<int>();
                var brokerSettings = BrokerSettingsHelper.Instance.GetSettings(brokerData.Broker);
                var result = CompetitionProcessorStatic.ProcessCompetitionPack(_logger, brokerData, algoMode, brokerSettings,
                    (stat, type, sportType, itemRawTransport, matchParsed) => {
                        var competitionItemID = itemRawTransport.CompetitionItemID;
                        if (competitionItemID == default(int)) {
                            return;
                        }
                        switch (taskMode) {
                            case RunTaskMode.RunLiveOddsTask:
                                ProcessLiveResult(competitionItemID, sportType, matchParsed.Result);
                                if (ProcessOdds<BetLive, long>(stat[ProcessStatType.Bet], competitionItemID, type, sportType, matchParsed.Odds)) {
                                    activeCompetitionItemIDs.Add(competitionItemID);
                                }
                                break;
                            case RunTaskMode.RunRegularOddsTask:
                                if (ProcessOdds<Bet, int>(stat[ProcessStatType.Bet], competitionItemID, type, sportType, matchParsed.Odds)) {
                                    activeCompetitionItemIDs.Add(competitionItemID);
                                }
                                break;
                            case RunTaskMode.RunPastDateHistoryTask:
                            case RunTaskMode.RunTodayHistoryTask:
                                ProjectProvider.Instance.ResultProvider.SaveResults(stat[ProcessStatType.Result], itemRawTransport, sportType, matchParsed.Result);
                                break;
                            default:
                                _logger.Error("Unknown mode: " + taskMode);
                                break;
                        }
                    });
                if (activeCompetitionItemIDs.Any()) {
                    const int LOOK_BEHIND = 10000;
                    switch (taskMode) {
                        case RunTaskMode.RunRegularOddsTask:
                            Bet.DataSource.FilterByBroker(brokerData.Broker)
                                .WhereTrue(Bet.Fields.IsActive)
                                .WhereNotIn(Bet.Fields.CompetitionitemID, activeCompetitionItemIDs)
                                .Where(Bet.Fields.ID, Oper.Greater, ((int) (Bet.DataSource.Max(Bet.Fields.ID) ?? decimal.MaxValue)) - LOOK_BEHIND)
                                .Update(Bet.Fields.IsActive, false);
                            break;
                        case RunTaskMode.RunLiveOddsTask:
                            BetLive.DataSource.FilterByBroker(brokerData.Broker)
                                .WhereTrue(BetLive.Fields.IsActive)
                                .WhereNotIn(BetLive.Fields.CompetitionitemID, activeCompetitionItemIDs)
                                .Where(BetLive.Fields.ID, Oper.Greater, ((long)(BetLive.DataSource.Max(BetLive.Fields.ID) ?? decimal.MaxValue)) - LOOK_BEHIND)
                                .Update(BetLive.Fields.IsActive, false);
                            break;
                    }
                }
                return result;
            }, null);
        }
        
        private static bool ProcessOdds<T, V>(ProcessStat processStat, int competitionItemID, BrokerType brokerType, SportType sportType, List<OddParsed> odds) where T : IBet<V>, new() {
            processStat.TotalCount++;
            if (odds == null || odds.Count == 0) {
                return false;
            }
            var betTemplate = new T();

            var betWithAdvancedDb = betTemplate.GetLastBetForCompetitionItem(competitionItemID, brokerType);
            var betAdvancedDb = betWithAdvancedDb?.GetAdvancedBet();

            var bet = BetHelper.GetBetFromOdds(betTemplate, odds);
            var betAdvanced = BetHelper.GetBetFromOdds(betTemplate.CreateAdvancedBet(), odds);

            if (BetHelper.SaveBetIfChanged(processStat, competitionItemID, brokerType, sportType, bet, betAdvanced, betWithAdvancedDb, betAdvancedDb)) {
                processStat.FinallySuccessCount++;
                return true;
            }
            return false;
        }

        private void ProcessLiveResult(int competitionItemID, SportType sportType, FullResult result) {
            InvokeSafe(() => {
                if (result == null) {
                    return;
                }
                var generateScoreID = ScoreHelper.Instance.GenerateScoreID(result.CompetitorResultOne, result.CompetitorResultTwo);
                var lastResult = CompetitionResultLive.DataSource
                    .Join(JoinType.Left, CompetitionResultLiveAdvanced.Fields.CompetitionresultliveID, CompetitionResultLive.Fields.ID, RetrieveMode.Retrieve)
                    .WhereEquals(CompetitionResultLive.Fields.CompetitionitemID, competitionItemID)
                    .WhereEquals(CompetitionResultLive.Fields.ScoreID, generateScoreID)
                    .Sort(CompetitionResultLive.Fields.ID, SortDirection.Desc)
                    .Sort(CompetitionResultLiveAdvanced.Fields.ID, SortDirection.Desc)
                    .AsList(
                        CompetitionResultLive.Fields.ScoreID,
                        CompetitionResultLiveAdvanced.Fields.ScoreID,
                        CompetitionResultLiveAdvanced.Fields.Advancedparam
                    );
                if (lastResult.Count == 0) {
                    var competitionResultLive = new CompetitionResultLive {
                        CompetitionitemID = competitionItemID,
                        ScoreID = generateScoreID,
                        Datecreatedutc = DateTime.UtcNow
                    };
                    lastResult.Add(competitionResultLive);
                    competitionResultLive.Save();
                }
                LiveResultProcFactory.GetLiveResultProc(sportType)
                                     .Process(lastResult, result);
            });
        }
    }
}