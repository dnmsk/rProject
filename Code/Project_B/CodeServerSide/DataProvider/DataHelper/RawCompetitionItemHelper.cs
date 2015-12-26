using System;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class RawCompetitionItemHelper {
        public static RawCompetitionItem GetCompetitionItem(BrokerType brokerType, CompetitorParsedTransport competitor1ParsedTransport, CompetitorParsedTransport competitor2ParsedTransport, CompetitionSpecifyTransport competitionSpecifyTransport, DateTime eventDateUtc, DateTime utcNow) {
            var competitionItem = RawCompetitionItem.DataSource
                        .Where(new DaoFilterOr(
                            new DaoFilterAnd(
                                new DaoFilterEq(RawCompetitionItem.Fields.Rawcompetitorid1, competitor1ParsedTransport.RawID),
                                new DaoFilterEq(RawCompetitionItem.Fields.Rawcompetitorid2, competitor2ParsedTransport.RawID)
                                ),
                            new DaoFilterAnd(
                                new DaoFilterEq(RawCompetitionItem.Fields.Rawcompetitorid2, competitor1ParsedTransport.RawID),
                                new DaoFilterEq(RawCompetitionItem.Fields.Rawcompetitorid1, competitor2ParsedTransport.RawID)
                                )
                            ))
                        .WhereEquals(RawCompetitionItem.Fields.Sporttype, (short)competitionSpecifyTransport.SportType)
                        .WhereEquals(RawCompetitionItem.Fields.Brokerid, (short)brokerType)
                        .WhereEquals(RawCompetitionItem.Fields.RawcompetitionID, competitionSpecifyTransport.RawCompetitionID)
                        .WhereEquals(RawCompetitionItem.Fields.RawcompetitionspecifyID, competitionSpecifyTransport.RawCompetitionSpecifyID)
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

        public static RawCompetitionItem CreateCompetitionItem(BrokerType brokerType, CompetitorParsedTransport competitor1ParsedTransport, CompetitorParsedTransport competitor2ParsedTransport, CompetitionSpecifyTransport competitionSpecifyTransport, DateTime eventDateUtc, DateTime utcNow) {
            var competitionItemRaw = new RawCompetitionItem {
                BrokerID = brokerType,
                SportType = competitionSpecifyTransport.SportType,
                Languagetype = competitionSpecifyTransport.LanguageType,
                Dateeventutc = eventDateUtc,
                Datecreatedutc = utcNow,
                RawcompetitionID = competitionSpecifyTransport.RawCompetitionID,
                RawcompetitionspecifyID = competitionSpecifyTransport.RawCompetitionSpecifyID,
                Rawcompetitorid1 = competitor1ParsedTransport.RawID,
                Rawcompetitorid2 = competitor2ParsedTransport.RawID,
                Linkstatus = LinkEntityStatus.ToLink
            };
            competitionItemRaw.Save();
            return competitionItemRaw;
        }
    }
}