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
using Project_B.CodeServerSide.Entity;
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