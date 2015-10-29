using System;
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

        public void SaveResult(BrokerData brokerData, bool canCreateIfNew) {
            InvokeSafe(() => {
                foreach (var competitionParsed in brokerData.Competitions) {
                    var competition = MainProvider.Instance.CompetitionProvider.GetCompetition(brokerData.Language, competitionParsed.Type, competitionParsed.Name, competitionParsed, canCreateIfNew);
                    foreach (var matchParsed in competitionParsed.Matches) {
                        var competitor1 = MainProvider.Instance.CompetitorProvider
                            .GetCompetitor(brokerData.Language, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullOne, matchParsed.CompetitorNameShortOne, competition.UniqueID, matchParsed, canCreateIfNew);
                        var competitor2 = MainProvider.Instance.CompetitorProvider
                            .GetCompetitor(brokerData.Language, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullTwo, matchParsed.CompetitorNameShortTwo, competition.UniqueID, matchParsed, canCreateIfNew);
                        var competitionItem = MainProvider.Instance.CompetitionProvider.GetCompetitionItem(competitor1, competitor2, competition, matchParsed.DateUtc, canCreateIfNew);
                        MainProvider.Instance.ResultProvider.SaveResults(competitionItem, competitionParsed.Type, matchParsed.Result);
                    }
                }
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