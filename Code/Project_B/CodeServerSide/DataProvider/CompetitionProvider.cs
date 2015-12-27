using System;
using System.Collections.Generic;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.DataHelper.RawData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class CompetitionProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitionProvider).FullName);
        
        public CompetitionProvider() : base(_logger) {}

        public RawTemplateObj<CompetitionSpecifyTransport> GetCompetitionSpecify(BrokerType brokerType, LanguageType language, SportType sportType, List<string> nameOrigin, CompetitionParsed competitionToSave, GatherBehaviorMode algoMode) {
            return InvokeSafeSingleCall(() => {
                nameOrigin = SportTypeHelper.Instance.ExcludeSportTypeFromList(nameOrigin);
                var genderDetected = GenderDetectorHelper.Instance[nameOrigin];
                var competitionSpecify = RawCompetitionHelper.GetCompetitionSpecify(brokerType, language, sportType, genderDetected, nameOrigin) ??
                                         CompetitionHelper.CreateCompetitionSpecify(brokerType, language, sportType, genderDetected, nameOrigin, competitionToSave, algoMode);
                return competitionSpecify;
            }, null);
        }

        public CompetitionItemRawTransport GetCompetitionItem(BrokerType brokerType, RawTemplateObj<CompetitorParsedTransport> competitor1ParsedTransport, RawTemplateObj<CompetitorParsedTransport> competitor2ParsedTransport, RawTemplateObj<CompetitionSpecifyTransport> competitionSpecifyTransport, DateTime eventDateUtc, GatherBehaviorMode algoMode) {
            return InvokeSafeSingleCall(() => {
                var utcNow = DateTime.UtcNow;
                if (eventDateUtc > utcNow.AddDays(14)) {
                    return null;
                }
                var competitionItemRaw = RawCompetitionItemHelper.GetCompetitionItem(brokerType, competitor1ParsedTransport, competitor2ParsedTransport, competitionSpecifyTransport, eventDateUtc, utcNow) 
                                         ?? RawCompetitionItemHelper.CreateCompetitionItem(brokerType, competitor1ParsedTransport, competitor2ParsedTransport, competitionSpecifyTransport, eventDateUtc, utcNow);
                var result = new CompetitionItemRawTransport {
                    RawCompetitionItemID = competitionItemRaw.ID
                };
                CompetitionItem competitionItem;
                var fieldsToRetrive = new Enum[] { CompetitionItem.Fields.ID, CompetitionItem.Fields.Dateeventutc, CompetitionItem.Fields.Competitoruniqueid1, CompetitionItem.Fields.Competitoruniqueid2, CompetitionItem.Fields.CompetitionSpecifyUniqueID };
                if (competitionItemRaw.CompetitionitemID == default(int)) {
                    competitionItem = CompetitionItem.DataSource
                        .Where(new DaoFilterOr(
                            new DaoFilterAnd(
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitor1ParsedTransport.Object.ID),
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitor2ParsedTransport.Object.ID)
                                ),
                            new DaoFilterAnd(
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitor1ParsedTransport.Object.ID),
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitor2ParsedTransport.Object.ID)
                                )
                            ))
                        .WhereEquals(CompetitionItem.Fields.Sporttype, (short)competitionSpecifyTransport.Object.SportType)
                        .WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionSpecifyTransport.Object.CompetitionUniqueID)
                        .Where(new DaoFilterOr(
                            new DaoFilterNull(CompetitionItem.Fields.CompetitionSpecifyUniqueID, true),
                            new DaoFilterEq(CompetitionItem.Fields.CompetitionSpecifyUniqueID, competitionSpecifyTransport.Object.CompetitionSpecifyUniqueID)))
                        .Where(eventDateUtc > DateTime.MinValue 
                            ? new DaoFilterAnd(
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, eventDateUtc.AddDays(-1.5)),
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.LessOrEq, eventDateUtc.AddDays(1.5))
                            )
                            : new DaoFilterAnd(
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, utcNow.AddDays(-1.5)),
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.LessOrEq, utcNow.AddDays(1.5))
                            )
                        )
                        .Sort(CompetitionItem.Fields.ID, SortDirection.Desc)
                        .First(fieldsToRetrive);
                } else {
                    competitionItem = CompetitionItem.DataSource.GetByKey(competitionItemRaw.CompetitionitemID, fieldsToRetrive);
                }

                if (competitionSpecifyTransport.Object.CompetitionSpecifyUniqueID != default(int) && competitionItem != null && competitionItem.CompetitionSpecifyUniqueID == default(int)) {
                    competitionItem.CompetitionSpecifyUniqueID = competitionSpecifyTransport.Object.CompetitionSpecifyUniqueID;
                    competitionItem.Save();
                }
                
                if (eventDateUtc > DateTime.MinValue && competitionItem != null && algoMode.HasFlag(GatherBehaviorMode.CreateOriginal)) {
                    var timeSpan = competitionItem.Dateeventutc - eventDateUtc;
                    if (Math.Abs(timeSpan.TotalDays) < 2) {
                        if (Math.Abs(timeSpan.TotalMinutes) > 5) {
                            competitionItem.Dateeventutc = eventDateUtc;
                            competitionItem.Save();
                        }
                    } else {
                        competitionItem = null;
                    }
                }

                if (competitionItem != null && competitionItemRaw.CompetitionitemID == default(int)) {
                    competitionItemRaw.CompetitionitemID = competitionItem.ID;
                    competitionItemRaw.Linkstatus = LinkEntityStatus.LinkByStatistics;
                    competitionItemRaw.Save();
                }

                if (competitionItem == null) {
                    if (!algoMode.HasFlag(GatherBehaviorMode.CreateOriginal)) {
                        return result;
                    }
                    competitionItem = new CompetitionItem {
                        SportType = competitionSpecifyTransport.Object.SportType,
                        Datecreatedutc = utcNow,
                        Dateeventutc = eventDateUtc != DateTime.MinValue ? eventDateUtc : utcNow,
                        CompetitionuniqueID = competitionSpecifyTransport.Object.CompetitionUniqueID,
                        CompetitionSpecifyUniqueID = competitionSpecifyTransport.Object.CompetitionSpecifyUniqueID,
                        Competitoruniqueid1 = competitor1ParsedTransport.Object.ID,
                        Competitoruniqueid2 = competitor2ParsedTransport.Object.ID
                    };
                    competitionItem.Save();
                    competitionItemRaw.Linkstatus = LinkEntityStatus.Original;
                    competitionItemRaw.CompetitionitemID = competitionItem.ID;
                    competitionItemRaw.Save();
                }
                result.CompetitionItemID = competitionItem.Competitoruniqueid1 == competitor1ParsedTransport.Object.ID &&
                                           competitionItem.Competitoruniqueid2 == competitor2ParsedTransport.Object.ID
                    ? competitionItem.ID
                    : -competitionItem.ID;
                return result;
            }, null);
        }
    }
}