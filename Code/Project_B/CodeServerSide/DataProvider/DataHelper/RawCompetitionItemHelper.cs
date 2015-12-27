using System;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeServerSide.DataProvider.DataHelper.RawData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class RawCompetitionItemHelper {
        public static RawCompetitionItem GetCompetitionItem(BrokerType brokerType, RawTemplateObj<CompetitorParsedTransport> competitor1ParsedTransport, RawTemplateObj<CompetitorParsedTransport> competitor2ParsedTransport, RawTemplateObj<CompetitionSpecifyTransport> competitionSpecifyTransport, DateTime eventDateUtc, DateTime utcNow) {
            var competitionItem = RawCompetitionItem.DataSource
                        .Where(new DaoFilterOr(
                            new DaoFilterAnd(
                                new DaoFilterEq(RawCompetitionItem.Fields.Rawcompetitorid1, competitor1ParsedTransport.RawObject.ID),
                                new DaoFilterEq(RawCompetitionItem.Fields.Rawcompetitorid2, competitor2ParsedTransport.RawObject.ID)
                                ),
                            new DaoFilterAnd(
                                new DaoFilterEq(RawCompetitionItem.Fields.Rawcompetitorid2, competitor1ParsedTransport.RawObject.ID),
                                new DaoFilterEq(RawCompetitionItem.Fields.Rawcompetitorid1, competitor2ParsedTransport.RawObject.ID)
                                )
                            ))
                        .WhereEquals(RawCompetitionItem.Fields.Sporttype, (short)competitionSpecifyTransport.Object.SportType)
                        .WhereEquals(RawCompetitionItem.Fields.Brokerid, (short)brokerType)
                        .WhereEquals(RawCompetitionItem.Fields.RawcompetitionID, competitionSpecifyTransport.RawObject.ParentID)
                        .WhereEquals(RawCompetitionItem.Fields.RawcompetitionspecifyID, competitionSpecifyTransport.RawObject.ID)
                        .Where(eventDateUtc > DateTime.MinValue
                            ? new DaoFilterAnd(
                                new DaoFilter(RawCompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, eventDateUtc.AddDays(-1.5)),
                                new DaoFilter(RawCompetitionItem.Fields.Dateeventutc, Oper.LessOrEq, eventDateUtc.AddDays(1.5))
                            )
                            : new DaoFilterAnd(
                                new DaoFilter(RawCompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, utcNow.AddDays(-1.5)),
                                new DaoFilter(RawCompetitionItem.Fields.Dateeventutc, Oper.LessOrEq, utcNow.AddDays(1.5))
                            )
                        )
                        .Sort(RawCompetitionItem.Fields.ID, SortDirection.Desc)
                        .First(RawCompetitionItem.Fields.ID, RawCompetitionItem.Fields.Dateeventutc, RawCompetitionItem.Fields.Rawcompetitorid1, RawCompetitionItem.Fields.Rawcompetitorid2, RawCompetitionItem.Fields.RawcompetitionID, RawCompetitionItem.Fields.CompetitionitemID);
            if (eventDateUtc > DateTime.MinValue && competitionItem != null) {
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
            return competitionItem;
        }

        public static RawCompetitionItem CreateCompetitionItem(BrokerType brokerType, RawTemplateObj<CompetitorParsedTransport> competitor1ParsedTransport, RawTemplateObj<CompetitorParsedTransport> competitor2ParsedTransport, RawTemplateObj<CompetitionSpecifyTransport> competitionSpecifyTransport, DateTime eventDateUtc, DateTime utcNow) {
            var competitionItemRaw = new RawCompetitionItem {
                BrokerID = brokerType,
                SportType = competitionSpecifyTransport.Object.SportType,
                Languagetype = competitionSpecifyTransport.Object.LanguageType,
                Dateeventutc = eventDateUtc != DateTime.MinValue ? eventDateUtc : utcNow,
                Datecreatedutc = utcNow,
                RawcompetitionID = competitionSpecifyTransport.RawObject.ParentID,
                RawcompetitionspecifyID = competitionSpecifyTransport.RawObject.ID,
                Rawcompetitorid1 = competitor1ParsedTransport.RawObject.ID,
                Rawcompetitorid2 = competitor2ParsedTransport.RawObject.ID,
                Linkstatus = LinkEntityStatus.ToLink
            };
            competitionItemRaw.Save();
            return competitionItemRaw;
        }
    }
}