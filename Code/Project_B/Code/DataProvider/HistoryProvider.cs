using System;
using System.Collections.Generic;
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

        public void SaveResult(LanguageType languageType, List<CompetitionParsed> competitionToSave) {
            InvokeSafe(() => {
                foreach (var competitionParsed in competitionToSave) {
                    var competition = MainProvider.Instance.CompetitionProvider.GetCompetition(languageType, competitionParsed.Type, competitionParsed.Name);
                    foreach (var matchParsed in competitionParsed.Matches) {
                        var competitor1 = MainProvider.Instance.CompetitorProvider
                            .GetCompetitor(languageType, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullOne, matchParsed.CompetitorNameShortOne);
                        var competitor2 = MainProvider.Instance.CompetitorProvider
                            .GetCompetitor(languageType, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullTwo, matchParsed.CompetitorNameShortTwo);
                        var competitionItem = MainProvider.Instance.CompetitionProvider.GetCompetitionItem(competitor1, competitor2, competition, matchParsed.DateUtc);
                        MainProvider.Instance.ResultProvider.SaveResults(competitionItem, competitionParsed.Type, matchParsed.Result);
                    }
                }
            });
        }

        public DateTime? GetPastDateToCollect() {
            return InvokeSafe(() => {
                var systemState = SystemStateResult.DataSource
                    .Where(new DbFnSimpleOp(SystemStateResult.Fields.Stateresult, FnMathOper.BitwiseAnd, (short) SystemStateResultType.CollectForYesterday), Oper.Eq, default(short))
                    .Where(SystemStateResult.Fields.Dateutc, Oper.Less, DateTime.UtcNow.Date.AddDays(-1))
                    .Sort(SystemStateResult.Fields.Dateutc, SortDirection.Asc)
                    .First();
                return systemState != null ? systemState.Dateutc : (DateTime?) null;
            }, null);
        }

        public void SetDateCollectedWithState(DateTime dateUtc, SystemStateResultType systemStateResultType) {
            InvokeSafe(() => {
                dateUtc = dateUtc.Date;
                var systemState = SystemStateResult.DataSource
                    .WhereEquals(SystemStateResult.Fields.Dateutc, dateUtc)
                    .First();
                if (systemState == null) {
                    systemState = new SystemStateResult {
                        Dateutc = dateUtc,
                        Stateresult = SystemStateResultType.Unknown
                    };
                }
                systemState.Stateresult |= systemStateResultType;
                systemState.Save();
            });
        }
    }
}