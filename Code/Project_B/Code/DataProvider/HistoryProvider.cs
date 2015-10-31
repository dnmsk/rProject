﻿using System;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using Project_B.Code.Data;
using Project_B.Code.Entity;
using Project_B.Code.Enums;

namespace Project_B.Code.DataProvider {
    public class HistoryProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (HistoryProvider).FullName);

        public HistoryProvider() : base(_logger) {}

        public void SaveResult(BrokerData brokerData, GatherBehaviorMode algoMode) {
            InvokeSafe(() => {
                if (algoMode.HasFlag(GatherBehaviorMode.CreateIfEmptyToDate)) { 
                    var minDate = brokerData.Competitions.Min(c => c.Matches.Where(m => m.DateUtc != DateTime.MinValue).Min(m => m.DateUtc));
                    var maxDate = brokerData.Competitions.Max(c => c.Matches.Max(m => m.DateUtc));
                    if ((maxDate - minDate).TotalDays <= 1 && CompetitionItem.DataSource.WhereBetween(CompetitionItem.Fields.Dateeventutc, minDate, maxDate, BetweenType.Inclusive).Count() == 0) {
                        algoMode = algoMode.FlagSet(GatherBehaviorMode.CreateIfNew);
                        _logger.Info("Date {0} enable GatherBehaviorMode.CreateIfNew", minDate);
                    }
                }
                var successCompetitions = 0;
                var successCompetitionItems = 0;
                foreach (var competitionParsed in brokerData.Competitions) {
                    var competition = MainProvider.Instance.CompetitionProvider.GetCompetition(brokerData.Language, competitionParsed.Type, competitionParsed.Name, competitionParsed, algoMode);
                    if (competition == null) {
                        continue;
                    }
                    successCompetitions++;
                    foreach (var matchParsed in competitionParsed.Matches) {
                        var competitor1 = MainProvider.Instance.CompetitorProvider
                            .GetCompetitor(brokerData.Language, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullOne, matchParsed.CompetitorNameShortOne, competition.UniqueID, matchParsed, algoMode);
                        var competitor2 = MainProvider.Instance.CompetitorProvider
                            .GetCompetitor(brokerData.Language, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullTwo, matchParsed.CompetitorNameShortTwo, competition.UniqueID, matchParsed, algoMode);
                        if (competitor1 == null || competitor2 == null) {
                            continue;
                        }
                        successCompetitionItems++;
                        var competitionItem = MainProvider.Instance.CompetitionProvider.GetCompetitionItem(competitor1, competitor2, competition, matchParsed.DateUtc, algoMode);
                        if (matchParsed.Result != null) {
                            MainProvider.Instance.ResultProvider.SaveResults(competitionItem, competitionParsed.Type, matchParsed.Result);
                        }
                    }
                }
                _logger.Info("SaveResults: {0}: {1}/{2} {3}/{4} {5} {6}", brokerData.Competitions.First().Matches.First().DateUtc.Date.ToString("yyyy MMMM dd"), successCompetitions, brokerData.Competitions.Count, 
                    successCompetitionItems, brokerData.Competitions.Sum(c => c.Matches.Count), brokerData.Broker, brokerData.Language);
            });
        }

        public DateTime? GetPastDateToCollect(BrokerType brokerType, LanguageType languageType) {
            return InvokeSafe(() => {
                var systemState = SystemStateResult.DataSource
                    .Where(new DbFnSimpleOp(SystemStateResult.Fields.Stateresult, FnMathOper.BitwiseAnd, (short) SystemStateResultType.CollectForYesterday), Oper.Eq, default(short))
                    .Where(SystemStateResult.Fields.Dateutc, Oper.Less, DateTime.UtcNow.Date.AddDays(-1))
                    .WhereEquals(SystemStateResult.Fields.BrokerID, (short)brokerType)
                    .WhereEquals(SystemStateResult.Fields.Languagetype, (short)languageType)
                    .Sort(SystemStateResult.Fields.Dateutc, SortDirection.Asc)
                    .First();
                return systemState != null ? systemState.Dateutc : (DateTime?) null;
            }, null);
        }

        public void SetDateCollectedWithState(BrokerType brokerType, LanguageType languageType, DateTime dateUtc, SystemStateResultType systemStateResultType) {
            InvokeSafe(() => {
                dateUtc = dateUtc.Date;
                var systemState = SystemStateResult.DataSource
                    .WhereEquals(SystemStateResult.Fields.Dateutc, dateUtc)
                    .WhereEquals(SystemStateResult.Fields.BrokerID, (short) brokerType)
                    .WhereEquals(SystemStateResult.Fields.Languagetype, (short) languageType)
                    .First();
                if (systemState == null) {
                    systemState = new SystemStateResult {
                        Dateutc = dateUtc,
                        Stateresult = SystemStateResultType.Unknown,
                        BrokerID = brokerType,
                        Languagetype = languageType
                    };
                }
                systemState.Stateresult |= systemStateResultType;
                systemState.Save();
            });
        }
    }
}