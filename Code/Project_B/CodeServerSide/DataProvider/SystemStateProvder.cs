using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType.ModerateTransport;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class SystemStateProvder : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (SystemStateProvder).FullName);

        public SystemStateProvder() : base(_logger) {}

        public List<SystemStateSummaryStatus> SummarySystemState(DateTime fromDate, DateTime toDate) {
            return InvokeSafe(() => {
                if (fromDate >= DateTime.UtcNow.Date) {
                    fromDate = DateTime.UtcNow.AddHours(-2);
                    toDate = fromDate.AddDays(14);
                }
                var rci = Broker.DataSource
                    .Join(JoinType.Left, typeof(RawCompetitionItem), new DaoFilterAnd(
                            new DaoFilterEq(RawCompetitionItem.Fields.Brokerid, new DbFnEmpty(Broker.Fields.ID)),
                            new DaoFilter(RawCompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, fromDate),
                            new DaoFilter(RawCompetitionItem.Fields.Dateeventutc, Oper.Less, toDate)), RetrieveMode.Retrieve)
                    .AsList(
                        Broker.Fields.ID,
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
                    .GroupBy(i => i.ID)
                    .Each(item => item.GroupBy(i => i.GetJoinedEntity<RawCompetitionItem>()?.Languagetype ?? LanguageType.Default)
                        .Each(grouped => {
                            var broker = item.First().BrokerType;
                            var language = grouped.FirstOrDefault()?.GetJoinedEntity<RawCompetitionItem>()?.Languagetype ?? LanguageType.English;
                            FillSummaryState(mapResults, broker, language, status => {
                                status.RawCompetitionItemCount = grouped.Select(g => g?.GetJoinedEntity<RawCompetitionItem>()?.ID).Distinct().Count();
                                status.CompetitionItemLinkedCount = grouped.Select(g => g?.GetJoinedEntity<RawCompetitionItem>()?.CompetitionitemID).Distinct().Count(g => g != default(int));
                            });
                        })
                    );

                FillSubData(mapResults, rci.SelectMany(i => {
                    var entity = i?.GetJoinedEntity<RawCompetitionItem>();
                    return new[] {entity?.Rawcompetitorid1 ?? default(int), entity?.Rawcompetitorid2 ?? default(int) };
                }).Distinct(), rci.Select(i => i?.GetJoinedEntity<RawCompetitionItem>()?.RawcompetitionID ?? default(int)).Distinct(), rci.Select(i => i?.GetJoinedEntity<RawCompetitionItem>()?.RawcompetitionspecifyID ?? default(int)).Distinct());

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
                var dateTo = date >= DateTime.UtcNow.Date ? DateTime.Today.AddDays(14) : date.AddDays(1);
                var rawCompetitionDs = RawCompetitionItem.DataSource.FilterByBroker(brokerid).FilterByLanguage(languagetype)
                    .WhereBetween(RawCompetitionItem.Fields.Dateeventutc, date, dateTo, BetweenType.Inclusive);
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
                    .AsMapByIds(RawCompetition.Fields.Name, RawCompetition.Fields.CompetitionuniqueID, RawCompetition.Fields.Sporttype, RawCompetition.Fields.Gendertype);
                var competitions = Competition.DataSource.WhereIn(Competition.Fields.CompetitionuniqueID, rCompetitions.Values.Select(rc => rc.CompetitionuniqueID).Distinct())
                    .AsMapByField<int>(Competition.Fields.CompetitionuniqueID, Competition.Fields.Name, Competition.Fields.Gendertype);

                var rCompetitionSpecify = RawCompetitionSpecify.DataSource.WhereIn(RawCompetitionSpecify.Fields.ID, rCompetitionItems.Select(rci => rci.RawcompetitionspecifyID).Distinct())
                    .AsMapByIds(RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID, RawCompetitionSpecify.Fields.Name);
                var competitionSpecify = CompetitionSpecify.DataSource.WhereIn(CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, rCompetitionSpecify.Values.Select(rcs => rcs.CompetitionSpecifyUniqueID).Distinct())
                    .AsMapByField<int>(CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, CompetitionSpecify.Fields.Name, CompetitionSpecify.Fields.Gendertype);

                var rCompetitors = RawCompetitor.DataSource.WhereIn(RawCompetitor.Fields.ID, rCompetitionItems.Select(rci => rci.Rawcompetitorid1).Union(rCompetitionItems.Select(rci => rci.Rawcompetitorid2)).Distinct())
                    .AsMapByIds(RawCompetitor.Fields.CompetitoruniqueID, RawCompetitor.Fields.Name);
                var competitors = Competitor.DataSource.WhereIn(Competitor.Fields.CompetitoruniqueID, rCompetitors.Values.Select(rc => rc.CompetitoruniqueID).Distinct())
                    .AsMapByField<int>(Competitor.Fields.CompetitoruniqueID, Competitor.Fields.NameFull, Competitor.Fields.Gendertype);

                var groupedRci = rCompetitionItems.GroupBy(rci => rci.RawcompetitionID).ToArray();
                var result = new List<RawCompetitionTransport>(groupedRci.Length);
                foreach (var rawCompetitionFromRci in groupedRci) {
                    var rawCompetition = rCompetitions.TryGetValueOrDefault(rawCompetitionFromRci.First().RawcompetitionID);
                    var gender = rawCompetition.Gendertype;
                    var buildEntityWithLink = BuildEntityWithLink(default(int), rawCompetition, competitions);
                    buildEntityWithLink.RawName += ". " + GenderDetectorHelper.Instance.GetGenderName(gender);
                    var rowResult = new RawCompetitionTransport {
                        SportType = rawCompetition.SportType,
                        Competition = buildEntityWithLink,
                        CompetitionSpecifies = rawCompetitionFromRci
                            .GroupBy(rci => rci.RawcompetitionspecifyID)
                            .Select(rcs => {
                                return new RawCompetitionSpecifyTransport {
                                    CompetitionSpecify = BuildEntityWithLink(default(int), rCompetitionSpecify.TryGetValueOrDefault(rcs.First().RawcompetitionspecifyID), competitionSpecify),
                                    CompetitionItems = rcs.Select(rci => {
                                        var competitionitemID = rci.CompetitionitemID;
                                        return new RawCompetitionItemTransport {
                                            RawID = rci.ID,
                                            EntityID = competitionitemID,
                                            RawName = rci.Dateeventutc.ToString("dd.MM HH:mm"),
                                            EntityName = new [] { (competitionItems.TryGetValueOrDefault(competitionitemID, false)?.Dateeventutc ?? DateTime.MinValue).ToString("dd.MM HH:mm") },
                                            Competitior1 = BuildEntityWithLink(competitionitemID, rCompetitors.TryGetValueOrDefault(rci.Rawcompetitorid1), competitors),
                                            Competitior2 = BuildEntityWithLink(competitionitemID, rCompetitors.TryGetValueOrDefault(rci.Rawcompetitorid2), competitors),
                                            BrokerEntityType = rci.EntityType
                                        };
                                    })
                                    .ToList()
                                };
                            })
                            .ToList()
                    };
                    result.Add(rowResult);
                }
                return result;
            });
        }

        private static RawEntityWithLink BuildEntityWithLink<TK, T>(int rawCompetitionItemID, TK raw, Dictionary<int, List<T>> entityMap) where TK : IRawLinkEntity, INamedEntity, IKeyBrokerEntity where T : INamedEntity, IUniqueID, IGenderTyped {
            List<T> entity;
            var entityIsNotEmpty = entityMap.TryGetValue(raw.LinkToEntityID, out entity);
            return new RawEntityWithLink {
                RawID = raw.ID,
                RawName = raw.Name,
                EntityID = entityIsNotEmpty ? entity[0].UniqueID : default(int),
                EntityName = entityIsNotEmpty ? entity.Select(e => e.Name + ". " + (GenderDetectorHelper.Instance.GetGenderName(e.Gendertype)??string.Empty)).ToArray() : null,
                BrokerEntityType = raw.EntityType,
                CompetitionItemID = rawCompetitionItemID
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
                        status.RawCompetitorCount += (int) (long) grouped.Data[rcompetitorCount];
                        status.CompetitorLinkedCount += (int) (long) grouped.Data[rcompetitorMappedCount];
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
                        status.RawCompetitionCount += (int) (long) grouped.Data[rcompetitionCount];
                        status.CompetitionLinkedCount += (int) (long) grouped.Data[rcompetitionMappedCount];
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
                        status.RawCompetitionSpecifyCount += (int) (long) grouped.Data[rcompetitionSpecifyCount];
                        status.CompetitionSpecifyLinkedCount += (int) (long) grouped.Data[rcompetitionSpecifyMappedCount];
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

        public RawEntityWithLink GetEntity(int id, BrokerEntityType entityType) {
            return InvokeSafe(() => {
                INamedEntity rawEntity = null;
                INamedEntity entity = null;
                switch (entityType) {
                    case BrokerEntityType.Competition:
                        var rCompetition = RawCompetition.DataSource
                            .Join(JoinType.Left, Competition.Fields.CompetitionuniqueID, RawCompetition.Fields.CompetitionuniqueID, RetrieveMode.Retrieve)
                            .WhereEquals(RawCompetition.Fields.ID, id)
                            .First();
                        rawEntity = rCompetition;
                        entity = rCompetition.GetJoinedEntity<Competition>();
                        break;
                    case BrokerEntityType.CompetitionSpecify:
                        var rCompetitionSpecify = RawCompetition.DataSource
                            .Join(JoinType.Left, CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID, RetrieveMode.Retrieve)
                            .WhereEquals(RawCompetition.Fields.ID, id)
                            .First();
                        rawEntity = rCompetitionSpecify;
                        entity = rCompetitionSpecify.GetJoinedEntity<CompetitionSpecify>();
                        break;
                    case BrokerEntityType.Competitor:
                        var rCompetitor = RawCompetitor.DataSource
                            .Join(JoinType.Left, Competitor.Fields.CompetitoruniqueID, RawCompetitor.Fields.CompetitoruniqueID, RetrieveMode.Retrieve)
                            .WhereEquals(RawCompetitor.Fields.ID, id)
                            .First();
                        rawEntity = rCompetitor;
                        entity = rCompetitor.GetJoinedEntity<Competitor>();
                        break;
                    case BrokerEntityType.CompetitionItem:
                        var rCompetitionItem = RawCompetitionItem.DataSource
                            .WhereEquals(RawCompetitionItem.Fields.ID, id)
                            .First();
                        return new RawEntityWithLink {
                            EntityID = rCompetitionItem.CompetitionitemID,
                            EntityName = new [] { rCompetitionItem.Dateeventutc.ToShortDateString() },
                            BrokerEntityType = BrokerEntityType.CompetitionItem,
                            RawName = rCompetitionItem.Dateeventutc.ToShortDateString(),
                            RawID = rCompetitionItem.ID
                        };
                }
                return new RawEntityWithLink {
                    RawID = id,
                    RawName = rawEntity?.Name ?? string.Empty,
                    EntityID = entity?.UniqueID ?? default(int),
                    EntityName = new [] {entity?.Name ?? string.Empty},
                    BrokerEntityType = entityType
                };
            });
        }
        
        public List<RawEntityWithLink> LiveSearch(BrokerEntityType type, int id, string search, bool all) {
            return InvokeSafe(() => {
                ISportTyped sportTyped = null;
                switch (type) {
                    case BrokerEntityType.Competition:
                        sportTyped = RawCompetition.DataSource.GetByKey(id, RawCompetition.Fields.Sporttype);
                        break;
                    case BrokerEntityType.CompetitionSpecify:
                        sportTyped = RawCompetitionSpecify.DataSource.GetByKey(id, RawCompetitionSpecify.Fields.Sporttype);
                        break;
                    case BrokerEntityType.Competitor:
                        sportTyped = RawCompetitor.DataSource.GetByKey(id, RawCompetitor.Fields.Sporttype);
                        break;
                }
                if (sportTyped == null) {
                    return null;
                }
                var namedEntities = new List<INamedEntity>();
                switch (type) {
                    case BrokerEntityType.Competition:
                        namedEntities.AddRange(Competition.DataSource.FilterBySportType(sportTyped.SportType).FilterByName(search, all)
                            .AsList(Competition.Fields.Name, Competition.Fields.CompetitionuniqueID, Competition.Fields.Gendertype));
                        break;
                    case BrokerEntityType.CompetitionSpecify:
                        namedEntities.AddRange(CompetitionSpecify.DataSource.FilterBySportType(sportTyped.SportType).FilterByName(search, all)
                            .AsList(CompetitionSpecify.Fields.Name, CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, CompetitionSpecify.Fields.Gendertype));
                        break;
                    case BrokerEntityType.Competitor:
                        namedEntities.AddRange(Competitor.DataSource.FilterBySportType(sportTyped.SportType).FilterByName(search, all)
                            .AsList(Competitor.Fields.NameFull, Competitor.Fields.CompetitoruniqueID, Competitor.Fields.Gendertype));
                        break;
                }

                return namedEntities
                    .Select(e => new RawEntityWithLink {
                        EntityID = e.UniqueID,
                        EntityName = new[] {e.Name + ". " + GenderDetectorHelper.Instance.GetGenderName(((IGenderTyped)e).Gendertype)}
                    })
                    .GroupBy(re => re.EntityID)
                    .Select(ge => new RawEntityWithLink {
                        EntityID = ge.Key,
                        EntityName = ge.SelectMany(e => e.EntityName).OrderBy(n => n).ToArray(),
                        RawID = id,
                        BrokerEntityType = type
                    })
                    .OrderBy(e => e.EntityName[0])
                    .ToList();
            });
        }

        public void EntityLinkerPut(int id, BrokerEntityType type) {
            InvokeSafe(() => {
                IUniqueID newTargetID = null;
                switch (type) {
                    case BrokerEntityType.Competition:
                        var rawCompetition = RawCompetition.DataSource.GetByKey(id);
                        newTargetID = CreateUniqueID<CompetitionUnique>();
                        new Competition {
                            CompetitionuniqueID = newTargetID.UniqueID,
                            Datecreatedutc = DateTime.UtcNow,
                            Name = rawCompetition.Name,
                            Gendertype = rawCompetition.Gendertype,
                            Languagetype = rawCompetition.Languagetype,
                            SportType = rawCompetition.SportType
                        }.Save();
                        break;
                    case BrokerEntityType.CompetitionSpecify:
                        var rawCompetitionSpecify = RawCompetitionSpecify.DataSource.GetByKey(id);
                        newTargetID = CreateUniqueID<CompetitionSpecifyUnique>();
                        new CompetitionSpecify {
                            Gendertype = rawCompetitionSpecify.Gendertype,
                            Languagetype = rawCompetitionSpecify.Languagetype,
                            SportType = rawCompetitionSpecify.SportType,
                            Name = rawCompetitionSpecify.Name,
                            CompetitionuniqueID = rawCompetitionSpecify.CompetitionuniqueID,
                            CompetitionSpecifyUniqueID = newTargetID.UniqueID,
                            Datecreatedutc = DateTime.UtcNow
                        }.Save();
                        break;
                    case BrokerEntityType.Competitor:
                        var rawCompetitor = RawCompetitor.DataSource.GetByKey(id);
                        newTargetID = CreateUniqueID<CompetitorUnique>();
                        new Competitor {
                            Name = rawCompetitor.Name,
                            Datecreatedutc = DateTime.UtcNow,
                            CompetitoruniqueID = newTargetID.UniqueID,
                            Gendertype = rawCompetitor.Gendertype,
                            Languagetype = rawCompetitor.Languagetype,
                            SportType = rawCompetitor.SportType
                        }.Save();
                        break;
                    case BrokerEntityType.CompetitionItem:
                        var rawCompetitionItem = RawCompetitionItem.DataSource.GetByKey(id);
                        var rawCompetitionCr = RawCompetition.DataSource.GetByKey(rawCompetitionItem.RawcompetitionID);
                        var rawCompetitors = RawCompetitor.DataSource.WhereIn(RawCompetitor.Fields.ID, new[] {rawCompetitionItem.Rawcompetitorid1, rawCompetitionItem.Rawcompetitorid2}).AsList();
                        if (rawCompetitionCr.CompetitionuniqueID != default(int) && rawCompetitors.All(c => c.CompetitoruniqueID != default(int))) {
                            var firstIsFirst = rawCompetitors[0].ID == rawCompetitionItem.Rawcompetitorid1;
                            var competition = new CompetitionItem {
                                SportType = rawCompetitionItem.SportType,
                                CompetitionuniqueID = rawCompetitionCr.CompetitionuniqueID,
                                Datecreatedutc = DateTime.UtcNow,
                                Dateeventutc = rawCompetitionItem.Datecreatedutc,
                                Competitoruniqueid1 = (firstIsFirst ? rawCompetitors[0] : rawCompetitors[1]).CompetitoruniqueID,
                                Competitoruniqueid2 = (firstIsFirst ? rawCompetitors[1] : rawCompetitors[0]).CompetitoruniqueID
                            };
                            competition.Save();
                            rawCompetitionItem.CompetitionitemID = competition.ID;
                            rawCompetitionItem.Save();
                        }
                        break;
                }
                EntityLinkerPost(id, type, new [] { newTargetID?.UniqueID ?? default(int) });
            });
        }

        private static IUniqueID CreateUniqueID<T>() where T : IUniqueID, IAbstractEntity, new() {
            var entity = new T();
            entity.Save();
            return entity;
        }

        public void EntityLinkerPost(int id, BrokerEntityType type, int[] ids) {
            InvokeSafe(() => {
                var targetID = ids.FirstOrDefault();
                var needJoin = ids?.Length > 1;
                if (targetID == default(int) && !needJoin) {
                    return;
                }
                if (needJoin) {
                    targetID = ids.Min();
                }
                var bitwiseStatus = (short)(LinkEntityStatus.Linked | LinkEntityStatus.ManualConfirmed);
                switch (type) {
                    case BrokerEntityType.Competition:
                        RawCompetitionHelper.GetRawCompetitionSpecify.Clear();
                        RawCompetition.DataSource
                            .WhereEquals(RawCompetition.Fields.ID, id)
                            .Update(new Dictionary<Enum, DbFunction> {
                                {RawCompetition.Fields.CompetitionuniqueID, new DbFnConst(targetID) },
                                {RawCompetition.Fields.Linkstatus, new DbFnSimpleOp(RawCompetition.Fields.Linkstatus, FnMathOper.BitwiseOr, bitwiseStatus) }
                            });
                        RawCompetitionSpecify.DataSource
                            .WhereEquals(RawCompetitionSpecify.Fields.RawCompetitionID, id)
                            .Update(RawCompetitionSpecify.Fields.CompetitionuniqueID, targetID);
                        break;
                    case BrokerEntityType.CompetitionSpecify:
                        RawCompetitionHelper.GetRawCompetitionSpecify.Clear();
                        RawCompetitionSpecify.DataSource
                            .WhereEquals(RawCompetitionSpecify.Fields.ID, id)
                            .Update(new Dictionary<Enum, DbFunction> {
                                {RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID, new DbFnConst(targetID) },
                                {RawCompetitionSpecify.Fields.Linkstatus, new DbFnSimpleOp(RawCompetitionSpecify.Fields.Linkstatus, FnMathOper.BitwiseOr, bitwiseStatus) }
                            });
                        break;
                    case BrokerEntityType.Competitor:
                        RawCompetitorHelper.GetRawCompetitor.Clear();
                        RawCompetitor.DataSource
                            .WhereEquals(RawCompetitor.Fields.ID, id)
                            .Update(new Dictionary<Enum, DbFunction> {
                                {RawCompetitor.Fields.CompetitoruniqueID, new DbFnConst(targetID) },
                                {RawCompetitor.Fields.Linkstatus, new DbFnSimpleOp(RawCompetitor.Fields.Linkstatus, FnMathOper.BitwiseOr, bitwiseStatus) }
                            });
                        break;
                }
                if (needJoin) {
                    EntityJoin(type, targetID, ids);
                }
            });
        }

        public void ApplyLinker(int targetId, BrokerEntityType type, bool ignoreRelinked = true) {
            InvokeSafe(() => {
                if (type != BrokerEntityType.Competitor) {
                    return;
                }
                var rawCompetitor = RawCompetitor.DataSource.GetByKey(targetId, RawCompetitor.Fields.Linkstatus, RawCompetitor.Fields.CompetitoruniqueID);
                if (rawCompetitor.Linkstatus.HasFlag(LinkEntityStatus.LinkByRelinkerSub) || rawCompetitor.CompetitoruniqueID == default(int)) {
                    return;
                }
                rawCompetitor.Linkstatus |= LinkEntityStatus.LinkByRelinkerSub;
                rawCompetitor.Save();

                var rawCompetitionItems = RawCompetitionItem.DataSource.Where(new DaoFilterOr(
                        new DaoFilterEq(RawCompetitionItem.Fields.Rawcompetitorid1, targetId),
                        new DaoFilterEq(RawCompetitionItem.Fields.Rawcompetitorid2, targetId)
                    ))
                    .Sort(RawCompetitionItem.Fields.Dateeventutc, SortDirection.Desc)
                    .AsList(
                        RawCompetitionItem.Fields.Rawcompetitorid1,
                        RawCompetitionItem.Fields.Rawcompetitorid2,
                        RawCompetitionItem.Fields.Dateeventutc
                    );
                var rawCompetitorsToProcess = RawCompetitor.DataSource
                    .WhereIn(RawCompetitor.Fields.ID, rawCompetitionItems.Select(rci => rci.Competitoruniqueid1).Union(rawCompetitionItems.Select(rci => rci.Competitoruniqueid2)).Distinct())
                    .Where(new DaoFilter(new DbFnSimpleOp(RawCompetitor.Fields.Linkstatus, FnMathOper.BitwiseAnd, 
                        (short)((ignoreRelinked ? LinkEntityStatus.LinkByRelinker : LinkEntityStatus.Unlinked) | LinkEntityStatus.ManualConfirmed)), Oper.Eq, default(int)))
                    .AsMapByIds(RawCompetitor.Fields.CompetitoruniqueID, RawCompetitor.Fields.Linkstatus, RawCompetitor.Fields.Name);

                var competitionItems = CompetitionItem.DataSource.Where(new DaoFilterOr(
                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, rawCompetitor.CompetitoruniqueID),
                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, rawCompetitor.CompetitoruniqueID)
                    ))
                    .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Desc)
                    .AsList(
                        CompetitionItem.Fields.Competitoruniqueid1,
                        CompetitionItem.Fields.Competitoruniqueid2,
                        CompetitionItem.Fields.Dateeventutc
                    );
                var nameCompetitors = Competitor.DataSource.WhereIn(Competitor.Fields.CompetitoruniqueID, competitionItems.Select(ci => ci.Competitoruniqueid1).Union(competitionItems.Select(ci => ci.Competitoruniqueid2)).Distinct())
                    .AsMapByField<int>(Competitor.Fields.CompetitoruniqueID, Competitor.Fields.NameFull);

                var linkedIds = new HashSet<int>();
                foreach (var rawCompetitionItem in rawCompetitionItems.GroupBy(rci => rci.Competitoruniqueid1 == targetId ? rci.Competitoruniqueid2 : rci.Competitoruniqueid1)) {
                    var mapMatches = new Dictionary<int, int>();
                    foreach (var oneCompetition in rawCompetitionItem) {
                        var nearByTime = competitionItems.Where(ci => Math.Abs((ci.Dateeventutc - oneCompetition.Dateeventutc).TotalHours) <= 1).ToArray();
                        if (!nearByTime.Any() || nearByTime.Length > 1) {
                            continue;
                        }
                        var idToSet = nearByTime[0].Competitoruniqueid1 == rawCompetitor.CompetitoruniqueID
                            ? nearByTime[0].Competitoruniqueid2 : nearByTime[0].Competitoruniqueid1;
                        mapMatches[idToSet] = mapMatches.TryGetValueOrDefault(idToSet) + 1;
                    }
                    var targetCompetitor = rawCompetitorsToProcess.TryGetValueOrDefault(rawCompetitionItem.Key, false);
                    if (!mapMatches.Any() || targetCompetitor == null) {
                        continue;
                    }
                    var sorted = mapMatches.OrderByDescending(m => m.Value).ToArray();
                    if (sorted[0].Value / (float) rawCompetitionItem.Count() > 0.7) {
                        if (targetCompetitor.CompetitoruniqueID != sorted[0].Key) {
                            targetCompetitor.CompetitoruniqueID = sorted[0].Key;
                            _logger.Info("Link: {0} ({1}) to {2} ({3})", targetCompetitor.ID, targetCompetitor.Name, targetCompetitor.CompetitoruniqueID,
                                nameCompetitors[targetCompetitor.CompetitoruniqueID][0].Name);
                        }
                        targetCompetitor.Linkstatus |= LinkEntityStatus.LinkByRelinker;
                        targetCompetitor.Save();
                        linkedIds.Add(targetCompetitor.ID);
                    }
                }
                foreach (var linkedId in linkedIds) {
                    ApplyLinker(linkedId, type);
                }
            });
        }

        public void EntityLinkerDelete(int id, BrokerEntityType type) {
            InvokeSafe(() => {
                switch (type) {
                    case BrokerEntityType.Competition:
                        RawCompetition.DataSource
                            .WhereEquals(RawCompetition.Fields.ID, id)
                            .Update(RawCompetition.Fields.CompetitionuniqueID, DBNull.Value);
                        break;
                    case BrokerEntityType.CompetitionSpecify:
                        RawCompetitionSpecify.DataSource
                            .WhereEquals(RawCompetitionSpecify.Fields.ID, id)
                            .Update(RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID, DBNull.Value);
                        break;
                    case BrokerEntityType.Competitor:
                        RawCompetitor.DataSource
                            .WhereEquals(RawCompetitor.Fields.ID, id)
                            .Update(RawCompetitor.Fields.CompetitoruniqueID, DBNull.Value);
                        break;
                }
            });
        }

        private int EntityJoin(BrokerEntityType type, int targetID, int[] ids) {
            return InvokeSafe(() => {
                var log = new StringBuilder();
                log.AppendLine(string.Format("{0}. {1} <= {2}", type, targetID, ids.StrJoin(", ")));
                int stat;
                switch (type) {
                    case BrokerEntityType.Competition:
                        stat = RawCompetition.DataSource
                            .WhereIn(RawCompetition.Fields.CompetitionuniqueID, ids)
                            .Update(RawCompetition.Fields.CompetitionuniqueID, targetID);
                        log.AppendFormat("{0}: {1}; ", "RawCompetition", stat);
                        stat = CompetitionSpecify.DataSource
                            .WhereIn(CompetitionSpecify.Fields.CompetitionuniqueID, ids)
                            .Update(CompetitionSpecify.Fields.CompetitionuniqueID, targetID);
                        log.AppendFormat("{0}: {1}; ", "CompetitionSpecify", stat);
                        stat = CompetitionItem.DataSource
                            .WhereIn(CompetitionItem.Fields.CompetitionuniqueID, ids)
                            .Update(CompetitionItem.Fields.CompetitionuniqueID, targetID);
                        log.AppendFormat("{0}: {1}; ", "CompetitionItem", stat);
                        stat = Competition.DataSource
                            .WhereIn(Competition.Fields.CompetitionuniqueID, ids)
                            .Update(Competition.Fields.CompetitionuniqueID, targetID);
                        log.AppendFormat("{0}: {1}; ", "Competition", stat);
                        break;
                    case BrokerEntityType.CompetitionSpecify:
                        stat = RawCompetitionSpecify.DataSource
                            .WhereIn(RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID, ids)
                            .Update(RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID, targetID);
                        log.AppendFormat("{0}: {1}; ", "RawCompetitionSpecify", stat);
                        stat = CompetitionItem.DataSource
                            .WhereIn(CompetitionItem.Fields.CompetitionSpecifyUniqueID, ids)
                            .Update(CompetitionItem.Fields.CompetitionSpecifyUniqueID, targetID);
                        log.AppendFormat("{0}: {1}; ", "CompetitionItem", stat);
                        stat = CompetitionSpecify.DataSource
                            .WhereIn(CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, ids)
                            .Update(CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, targetID);
                        log.AppendFormat("{0}: {1}; ", "CompetitionSpecify", stat);
                        break;
                    case BrokerEntityType.Competitor:
                        stat = RawCompetitor.DataSource
                            .WhereIn(RawCompetitor.Fields.CompetitoruniqueID, ids)
                            .Update(RawCompetitor.Fields.CompetitoruniqueID, targetID);
                        log.AppendFormat("{0}: {1}; ", "RawCompetitor", stat);
                        stat = CompetitionItem.DataSource
                            .WhereIn(CompetitionItem.Fields.Competitoruniqueid1, ids)
                            .Update(CompetitionItem.Fields.Competitoruniqueid1, targetID);
                        log.AppendFormat("{0}: {1}; ", "Competitoruniqueid1", stat);
                        stat = CompetitionItem.DataSource
                            .WhereIn(CompetitionItem.Fields.Competitoruniqueid2, ids)
                            .Update(CompetitionItem.Fields.Competitoruniqueid2, targetID);
                        log.AppendFormat("{0}: {1}; ", "Competitoruniqueid2", stat);
                        stat = Competitor.DataSource
                            .WhereIn(Competitor.Fields.CompetitoruniqueID, ids)
                            .Update(Competitor.Fields.CompetitoruniqueID, targetID);
                        log.AppendFormat("{0}: {1}; ", "Competitor", stat);
                        break;
                }
                _logger.Info(log.ToString());
                return targetID;
            });
        }

        public List<string> GetTooltip(BrokerEntityType type, int id) {
            return InvokeSafe(() => {
                var named = new List<INamedEntity>();
                switch (type) {
                    case BrokerEntityType.Competition:
                        named.AddRange(
                            Competition.DataSource
                                .WhereEquals(Competition.Fields.CompetitionuniqueID, id)
                                .AsList(Competition.Fields.Name, Competition.Fields.Gendertype)
                        );
                        break;
                    case BrokerEntityType.CompetitionSpecify:
                        named.AddRange(
                            Competition.DataSource
                                .WhereIn(Competition.Fields.CompetitionuniqueID, CompetitionSpecify.DataSource
                                                                                    .WhereEquals(CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, id)
                                                                                    .AsList(CompetitionSpecify.Fields.CompetitionuniqueID)
                                                                                    .Select(cs => cs.CompetitionuniqueID).Distinct())
                                .AsList(Competition.Fields.Name, Competition.Fields.Gendertype)
                        );
                        break;
                    case BrokerEntityType.Competitor:
                        named.AddRange(
                            Competition.DataSource
                                .WhereIn(Competition.Fields.CompetitionuniqueID, CompetitionItem.DataSource
                                                                                    .Where(new DaoFilterOr(
                                                                                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, id),
                                                                                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, id)
                                                                                    ))
                                                                                    .AsColumn<int>(CompetitionItem.Fields.CompetitionuniqueID)
                                                                                    .Distinct())
                                .AsList(Competition.Fields.Name, Competition.Fields.Gendertype)
                        );
                        break;
                }
                return named
                    .Select(n => n.Name + " " + GenderDetectorHelper.Instance.GetGenderName(((IGenderTyped)n).Gendertype))
                    .OrderBy(n => n)
                    .ToList();
            });
        }
    }
}