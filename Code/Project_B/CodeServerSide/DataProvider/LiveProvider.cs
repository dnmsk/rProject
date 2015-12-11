﻿using System;
using System.Collections.Generic;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class LiveProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (LiveProvider).FullName);

        public LiveProvider() : base(_logger) {}

        public void ProcessdLiveParsed(BrokerData brokerData, GatherBehaviorMode algoMode) {
            InvokeSafe(() => {
                CompetitionProcessorStatic.ProcessCompetitionPack(_logger, brokerData, algoMode,
                    (type, sportType, competitionItemID, matchParsed) => AddLive(competitionItemID, type, sportType, matchParsed));
            });
        }

        private void AddLive(int competitionItemID, BrokerType brokerType, SportType sportType, MatchParsed matchParsed) {
            InvokeSafe(() => {
                if (competitionItemID == default(int)) {
                    return;
                }
                ProcessLiveOdds(competitionItemID, brokerType, sportType, matchParsed.Odds);
                ProcessLiveResult(competitionItemID, sportType, matchParsed.Result);
            });
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

        private static void ProcessLiveOdds(int competitionItemID, BrokerType brokerType, SportType sportType, List<OddParsed> odds) {
            if (odds == null || odds.Count == 0) {
                return;
            }
            var betWithAdvancedDb = BetLive.DataSource
                .Join(JoinType.Left, BetLiveAdvanced.Fields.ID, BetLive.Fields.ID, RetrieveMode.Retrieve)
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