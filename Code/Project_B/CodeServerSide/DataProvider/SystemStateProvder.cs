using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType.ModerateTransport;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class SystemStateProvder : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (SystemStateProvder).FullName);

        public SystemStateProvder() : base(_logger) {
        }

        public List<SystemStateSummaryStatus> SummarySystemState(DateTime fromDate, DateTime toDate) {
            return InvokeSafe(() => {
                var rci = RawCompetitionItem.DataSource
                    .WhereBetween(RawCompetitionItem.Fields.Dateeventutc, fromDate, toDate, BetweenType.Inclusive)
                    .AsList(
                        RawCompetitionItem.Fields.ID,
                        RawCompetitionItem.Fields.Rawcompetitorid1,
                        RawCompetitionItem.Fields.Rawcompetitorid2,
                        RawCompetitionItem.Fields.RawcompetitionID,
                        RawCompetitionItem.Fields.RawcompetitionspecifyID,
                        RawCompetitionItem.Fields.CompetitionitemID,
                        RawCompetitionItem.Fields.Brokerid,
                        RawCompetitionItem.Fields.Languagetype
                    );
                var mapResults = new Dictionary<string, SystemStateSummaryStatus>();
                rci
                    .GroupBy(i => i.BrokerID)
                    .Each(item => item.GroupBy(i => i.Languagetype).Each(grouped => {
                        var broker = item.Key;
                        var language = grouped.Key;
                        FillSummaryState(mapResults, broker, language, status => {
                            status.RawCompetitionItemCount = grouped.Count();
                            status.CompetitionItemLinkedCount = grouped.Count(g => g.CompetitionitemID != default(int));
                        });
                    }));

                FillSubData(mapResults, rci.Select(i => i.Competitoruniqueid1).Union(rci.Select(i => i.Rawcompetitorid2)).Distinct(), rci.Select(i => i.RawcompetitionID).Distinct(), rci.Select(i => i.RawcompetitionspecifyID).Distinct());

                return mapResults.Values.ToList();
            }, null);
        }

        public List<SystemStateSummaryStatus> SummarySystemState() {
            return InvokeSafe(() => {
                var mapResults = new Dictionary<string, SystemStateSummaryStatus>();

                var rCount = RawCompetitionItem.DataSource.AggrCount(RawCompetitionItem.Fields.ID, false);
                var rMappedCount = RawCompetitionItem.DataSource.AggrCount(RawCompetitionItem.Fields.CompetitionitemID, false);
                RawCompetitionItem.DataSource.GroupBy(
                        RawCompetitionItem.Fields.Brokerid,
                        RawCompetitionItem.Fields.Languagetype
                    ).AsGroups(
                        rCount,
                        rMappedCount
                    ).Each(grouped => {
                        var broker = (BrokerType)(short)grouped.Data[RawCompetitionItem.Fields.Brokerid];
                        var language = (LanguageType)(short)grouped.Data[RawCompetitionItem.Fields.Languagetype];
                        FillSummaryState(mapResults, broker, language, status => {
                            status.RawCompetitionItemCount = (int)(long)grouped.Data[rCount];
                            status.CompetitionItemLinkedCount = (int)(long)grouped.Data[rMappedCount];
                        });
                    });
                FillSubData(mapResults, null, null, null);
                return mapResults.Values.ToList();
            }, null);
        }

        public List<RawCompetitionTransport> GetCompetitionItems(BrokerType brokerid, LanguageType languagetype, DateTime date, StateFilter state) {
            return InvokeSafe(() => {
                var rawCompetitionDs = RawCompetitionItem.DataSource
                    .WhereEquals(RawCompetitionItem.Fields.Brokerid, (short)brokerid)
                    .WhereEquals(RawCompetitionItem.Fields.Languagetype, (short) languagetype)
                    .WhereBetween(RawCompetitionItem.Fields.Dateeventutc, date, date.AddDays(1), BetweenType.Inclusive);
                switch (state) {
                    case StateFilter.Linked:
                        rawCompetitionDs = rawCompetitionDs.WhereNotNull(RawCompetitionItem.Fields.CompetitionitemID);
                        break;
                    case StateFilter.Unlinked:
                        rawCompetitionDs = rawCompetitionDs.WhereNull(RawCompetitionItem.Fields.CompetitionitemID);
                        break;
                }

                var rCompetitionItems = rawCompetitionDs
                    .AsList();
                var competitionItems = CompetitionItem.DataSource.WhereIn(CompetitionItem.Fields.ID, rCompetitionItems.Select(rci => rci.CompetitionitemID).Distinct())
                    .AsMapByIds(CompetitionItem.Fields.Dateeventutc);

                var rCompetitions = RawCompetition.DataSource.WhereIn(RawCompetition.Fields.ID, rCompetitionItems.Select(rci => rci.RawcompetitionID).Distinct())
                    .AsMapByIds(RawCompetition.Fields.Name, RawCompetition.Fields.CompetitionuniqueID, RawCompetition.Fields.Sporttype);
                var competitions = Competition.DataSource.WhereIn(Competition.Fields.CompetitionuniqueID, rCompetitions.Values.Select(rc => rc.CompetitionuniqueID).Distinct())
                    .AsMapByField<int>(Competition.Fields.CompetitionuniqueID, Competition.Fields.Name);

                var rCompetitionSpecify = RawCompetitionSpecify.DataSource.WhereIn(RawCompetitionSpecify.Fields.ID, rCompetitionItems.Select(rci => rci.RawcompetitionspecifyID).Distinct())
                    .AsMapByIds(RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID, RawCompetitionSpecify.Fields.Name);
                var competitionSpecify = CompetitionSpecify.DataSource.WhereIn(CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, rCompetitionSpecify.Values.Select(rcs => rcs.CompetitionSpecifyUniqueID).Distinct())
                    .AsMapByField<int>(CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, CompetitionSpecify.Fields.Name);

                var rCompetitors = RawCompetitor.DataSource.WhereIn(RawCompetitor.Fields.ID, rCompetitionItems.Select(rci => rci.Rawcompetitorid1).Union(rCompetitionItems.Select(rci => rci.Rawcompetitorid2)).Distinct())
                    .AsMapByIds(RawCompetitor.Fields.CompetitoruniqueID, RawCompetitor.Fields.Name);
                var competitors = Competitor.DataSource.WhereIn(Competitor.Fields.CompetitoruniqueID, rCompetitors.Values.Select(rc => rc.CompetitoruniqueID).Distinct())
                    .AsMapByField<int>(Competitor.Fields.CompetitoruniqueID, Competitor.Fields.NameFull);

                var groupedRci = rCompetitionItems.GroupBy(rci => rci.RawcompetitionID).ToArray();
                var result = new List<RawCompetitionTransport>(groupedRci.Length);
                foreach (var rawCompetitionFromRci in groupedRci) {
                    var rawCompetition = rCompetitions.TryGetValueOrDefault(rawCompetitionFromRci.First().RawcompetitionID);
                    var rowResult = new RawCompetitionTransport {
                        SportType = rawCompetition.SportType,
                        Competition = BuildEntityWithLink(rawCompetition, competitions),
                        CompetitionSpecifies = rawCompetitionFromRci
                            .GroupBy(rci => rci.RawcompetitionspecifyID)
                            .Select(rcs => {
                                return new RawCompetitionSpecifyTransport {
                                    CompetitionSpecify = BuildEntityWithLink(rCompetitionSpecify.TryGetValueOrDefault(rcs.First().RawcompetitionspecifyID), competitionSpecify),
                                    CompetitionItems = rcs.Select(rci => new RawCompetitionItemTransport {
                                        RawID = rci.ID,
                                        EntityID = rci.CompetitionitemID,
                                        RawEventDate = rci.Dateeventutc,
                                        EventDate = competitionItems.TryGetValueOrDefault(rci.CompetitionitemID, false)?.Dateeventutc ?? DateTime.MinValue,
                                        Competitior1 = BuildEntityWithLink(rCompetitors.TryGetValueOrDefault(rci.Rawcompetitorid1), competitors),
                                        Competitior2 = BuildEntityWithLink(rCompetitors.TryGetValueOrDefault(rci.Rawcompetitorid2), competitors)
                                    })
                                    .ToList()
                                };
                            })
                            .ToList()
                    };
                    result.Add(rowResult);
                }
                return result;
            }, null);
        }

        private static RawEntityWithLink BuildEntityWithLink<K, T>(K raw, Dictionary<int, List<T>> entityMap) where K : IRawLinkEntity, INamedEntity where T : INamedEntity {
            List<T> entity;
            entityMap.TryGetValue(raw.LinkToEntityID, out entity);
            var entityIsNotEmpty = entity != null;
            return new RawEntityWithLink {
                RawID = raw.ID,
                RawName = raw.Name,
                EntityID = entityIsNotEmpty ? entity[0].ID : default(int),
                EntityName = entityIsNotEmpty ? entity.Select(e => e.Name).ToArray() : null
            };
        }


        private static void FillSubData(Dictionary<string, SystemStateSummaryStatus> mapResults, IEnumerable<int> rawCompetitorIDs, IEnumerable<int> rawCompetitionIDs, IEnumerable<int> rawCompetitionSpecifyIDs) {
            var rcompetitorCount = RawCompetitor.DataSource.AggrCount(RawCompetitor.Fields.ID, false);
            var rcompetitorMappedCount = RawCompetitor.DataSource.AggrCount(RawCompetitor.Fields.CompetitoruniqueID, false);
            var competitiorDs = rawCompetitorIDs != null 
                ? RawCompetitor.DataSource.WhereIn(RawCompetitor.Fields.ID, rawCompetitorIDs) 
                : RawCompetitor.DataSource;
            competitiorDs.GroupBy(
                    RawCompetitor.Fields.Brokerid,
                    RawCompetitor.Fields.Languagetype
                ).AsGroups(
                    rcompetitorCount,
                    rcompetitorMappedCount
                ).Each(grouped => {
                    var broker = (BrokerType) (short) grouped.Data[RawCompetitor.Fields.Brokerid];
                    var language = (LanguageType) (short) grouped.Data[RawCompetitor.Fields.Languagetype];
                    FillSummaryState(mapResults, broker, language, status => {
                        status.RawCompetitorCount = (int) (long) grouped.Data[rcompetitorCount];
                        status.CompetitorLinkedCount = (int) (long) grouped.Data[rcompetitorMappedCount];
                    });
                });

            var rcompetitionCount = RawCompetition.DataSource.AggrCount(RawCompetition.Fields.ID, false);
            var rcompetitionMappedCount = RawCompetition.DataSource.AggrCount(RawCompetition.Fields.CompetitionuniqueID, false);
            var competitionDs = rawCompetitionIDs != null
                ? RawCompetition.DataSource.WhereIn(RawCompetition.Fields.ID, rawCompetitionIDs)
                : RawCompetition.DataSource;
            competitionDs.GroupBy(
                    RawCompetition.Fields.Brokerid,
                    RawCompetition.Fields.Languagetype
                ).AsGroups(
                    rcompetitionCount,
                    rcompetitionMappedCount
                ).Each(grouped => {
                    var broker = (BrokerType) (short) grouped.Data[RawCompetition.Fields.Brokerid];
                    var language = (LanguageType) (short) grouped.Data[RawCompetition.Fields.Languagetype];
                    FillSummaryState(mapResults, broker, language, status => {
                        status.RawCompetitionCount = (int) (long) grouped.Data[rcompetitionCount];
                        status.CompetitionLinkedCount = (int) (long) grouped.Data[rcompetitionMappedCount];
                    });
                });

            var rcompetitionSpecifyCount = RawCompetitionSpecify.DataSource.AggrCount(RawCompetitionSpecify.Fields.ID, false);
            var rcompetitionSpecifyMappedCount = RawCompetitionSpecify.DataSource.AggrCount(RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID, false);
            var competitionSpecifyDs = rawCompetitionSpecifyIDs != null 
                ? RawCompetitionSpecify.DataSource.WhereIn(RawCompetitionSpecify.Fields.ID, rawCompetitionSpecifyIDs)
                : RawCompetitionSpecify.DataSource;
            competitionSpecifyDs.GroupBy(
                    RawCompetitionSpecify.Fields.Brokerid,
                    RawCompetitionSpecify.Fields.Languagetype
                ).AsGroups(
                    rcompetitionSpecifyCount,
                    rcompetitionSpecifyMappedCount
                ).Each(grouped => {
                    var broker = (BrokerType) (short) grouped.Data[RawCompetitionSpecify.Fields.Brokerid];
                    var language = (LanguageType) (short) grouped.Data[RawCompetitionSpecify.Fields.Languagetype];
                    FillSummaryState(mapResults, broker, language, status => {
                        status.RawCompetitionSpecifyCount = (int) (long) grouped.Data[rcompetitionSpecifyCount];
                        status.CompetitionSpecifyLinkedCount = (int) (long) grouped.Data[rcompetitionSpecifyMappedCount];
                    });
                });
        }

        private static void FillSummaryState(Dictionary<string, SystemStateSummaryStatus> stateMap, BrokerType brokerType, LanguageType languageType, Action<SystemStateSummaryStatus> dataSetter) {
            SystemStateSummaryStatus state;
            var key = string.Format("{0}_{1}", brokerType, languageType);
            if (!stateMap.TryGetValue(key, out state)) {
                state = new SystemStateSummaryStatus {
                    BrokerID = brokerType,
                    Languagetype = languageType
                };
                stateMap[key] = state;
            }
            dataSetter(state);
        }
    }
}