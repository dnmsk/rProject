using System;
using System.Collections.Generic;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
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
                return CompetitionProcessorStatic.ProcessCompetitionPack(_logger, brokerData, algoMode,
                    (stat, type, sportType, itemRawTransport, matchParsed) => {
                        var competitionItemID = itemRawTransport.CompetitionItemID;
                        if (competitionItemID == default(int)) {
                            return;
                        }
                        switch (taskMode) {
                            case RunTaskMode.RunLiveOddsTask:
                                ProcessOdds<BetLive, long>(stat[ProcessStatType.Bet], competitionItemID, type, sportType, matchParsed.Odds);
                                ProcessLiveResult(competitionItemID, sportType, matchParsed.Result);
                                break;
                            case RunTaskMode.RunRegularOddsTask:
                                ProcessOdds<Bet, int>(stat[ProcessStatType.Bet], competitionItemID, type, sportType, matchParsed.Odds);
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
            }, null);
        }
        
        private static void ProcessOdds<T, V>(ProcessStat processStat, int competitionItemID, BrokerType brokerType, SportType sportType, List<OddParsed> odds) where T : IBet<V>, new() {
            processStat.TotalCount++;
            if (odds == null || odds.Count == 0) {
                return;
            }
            var betLiveTemplate = new T();

            var betWithAdvancedDb = betLiveTemplate.GetLastBetForCompetitionItem(competitionItemID, brokerType);
            var betAdvancedDb = betWithAdvancedDb?.GetAdvancedBet();

            var bet = BetHelper.GetBetFromOdds(betLiveTemplate, odds);
            var betAdvanced = BetHelper.GetBetFromOdds(betLiveTemplate.CreateAdvancedBet(), odds);

            BetHelper.SaveBetIfChanged(processStat, competitionItemID, brokerType, sportType, bet, betAdvanced, betWithAdvancedDb, betAdvancedDb);
            processStat.FinallySuccessCount++;
        }

        private static void ProcessLiveResult(int competitionItemID, SportType sportType, FullResult result) {
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
        }
    }
}