﻿using System;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
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
                CompetitionProcessorStatic.ProcessCompetitionPack(_logger, brokerData, algoMode,
                    (type, sportType, competitionItemID, matchParsed) => ProjectProvider.Instance.ResultProvider.SaveResults(competitionItemID, sportType, matchParsed.Result));
            });
        }

        public DateTime? GetPastDateToCollect(BrokerType brokerType, LanguageType languageType, SystemStateResultType pastDateType) {
            return InvokeSafe(() => {
                var systemState = SystemStateResult.DataSource
                    .Where(new DbFnSimpleOp(SystemStateResult.Fields.Stateresult, FnMathOper.BitwiseAnd, (short)pastDateType), Oper.Eq, default(short))
                    .Where(SystemStateResult.Fields.Dateutc, Oper.Less, DateTime.UtcNow.Date.AddDays(-1))
                    .WhereEquals(SystemStateResult.Fields.BrokerID, (int)brokerType)
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
                    .WhereEquals(SystemStateResult.Fields.BrokerID, (int) brokerType)
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