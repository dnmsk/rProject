using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeClientSide.TransportType.SubData;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class RawCompetitorHelper {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (RawCompetitorHelper).FullName);
        private static readonly object _lockObj = new object();

        public static List<RawCompetitor> GetCompetitor(BrokerType brokerType, LanguageType languageType, SportType sportType, GenderType genderType, string nameShort, string nameFull) {
            var competitors = RawCompetitor.DataSource
                    .WhereEquals(RawCompetitor.Fields.Gendertype, (short)genderType)
                    .WhereEquals(RawCompetitor.Fields.Languagetype, (short)languageType)
                    .WhereEquals(RawCompetitor.Fields.Sporttype, (short)sportType)
                    .WhereEquals(RawCompetitor.Fields.Brokerid, (short)brokerType)
                    .Where(new DaoFilterOr(
                        QueryHelper.GetIndexedFilterByWordIgnoreCase(nameShort, RawCompetitor.Fields.Name),
                        QueryHelper.GetIndexedFilterByWordIgnoreCase(nameFull, RawCompetitor.Fields.Name))
                    )
                    .Sort(RawCompetitor.Fields.ID, SortDirection.Asc)
                    .AsList(
                        RawCompetitor.Fields.CompetitoruniqueID,
                        RawCompetitor.Fields.Name,
                        RawCompetitor.Fields.Linkstatus
                    );
            if (competitors.Count > 1) {
                var count = competitors.GroupBy(c => c.CompetitoruniqueID).Count();
                if (count != 1) {
                    _logger.Error("{0} {1} {2} {3} {4} {5} {6}", brokerType, languageType, sportType, genderType, nameShort, nameFull, count);
                    return null;
                }
            }
            if (!nameFull.Equals(nameShort, StringComparison.InvariantCultureIgnoreCase) && competitors.Any()) {
                new[] {nameShort, nameFull }
                    .Where(name => !competitors.Any(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                    .Each(name => {
                        var firstCompetitorRow = competitors.First();
                        var competitorRaw = new RawCompetitor {
                            BrokerID = brokerType,
                            Datecreatedutc = DateTime.UtcNow,
                            Languagetype = languageType,
                            Name = name,
                            Gendertype = genderType,
                            SportType = sportType,
                            Linkstatus = firstCompetitorRow.Linkstatus
                        };
                        if (firstCompetitorRow.CompetitoruniqueID != default(int)) {
                            competitorRaw.CompetitoruniqueID = firstCompetitorRow.CompetitoruniqueID;
                        }
                        competitorRaw.Save();
                    });
            }
            return competitors;
        }

        public static List<RawCompetitor> CreateCompetitorAndDetect(BrokerType brokerType, LanguageType languageType, SportType sportType, GenderType genderType, string nameShort, string nameFull, int competitionUnique, MatchParsed matchParsed, GatherBehaviorMode algoMode) {
            lock (_lockObj) {
                var names = nameShort.Equals(nameFull, StringComparison.InvariantCultureIgnoreCase) ? new[] {nameFull } : new []{nameShort, nameFull };
                var competitors = names.Select(name => {
                    var competitorRaw = new RawCompetitor {
                        BrokerID = brokerType,
                        Datecreatedutc = DateTime.UtcNow,
                        Languagetype = languageType,
                        Name = name,
                        Gendertype = genderType,
                        SportType = sportType,
                        Linkstatus = LinkEntityStatus.ToLink
                    };
                    competitorRaw.Save();
                    return competitorRaw;
                }).ToList();

                CompetitorUnique uniqueID = null;
                if (algoMode.HasFlag(GatherBehaviorMode.CanDetectCompetitor)) {
                    uniqueID = TryGetCompetitorUniqueByResult(nameFull, nameShort, competitionUnique, matchParsed);
                }

                if (uniqueID != null && algoMode.HasFlag(GatherBehaviorMode.CreateNewLanguageName) && !Competitor.DataSource
                            .WhereEquals(Competitor.Fields.CompetitoruniqueID, uniqueID.ID)
                            .WhereEquals(Competitor.Fields.Languagetype, (short) languageType)
                            .IsExists()) {
                    new Competitor {
                        CompetitoruniqueID = uniqueID.ID,
                        SportType = sportType,
                        Datecreatedutc = DateTime.UtcNow,
                        Languagetype = languageType,
                        NameFull = nameFull,
                        NameShort = nameShort,
                        Gendertype = genderType
                    }.Save();
                }
                var linkStatus = LinkEntityStatus.Undefined;
                if (uniqueID == null && algoMode.HasFlag(GatherBehaviorMode.CreateOriginal)) {
                    uniqueID = new CompetitorUnique {
                        IsUsed = true
                    };
                    uniqueID.Save();
                    var competitor = new Competitor {
                        CompetitoruniqueID = uniqueID.ID,
                        SportType = sportType,
                        Datecreatedutc = DateTime.UtcNow,
                        Languagetype = languageType,
                        NameFull = nameFull,
                        NameShort = nameShort,
                        Gendertype = genderType
                    };
                    competitor.Save();
                    linkStatus = LinkEntityStatus.Original;
                }
                if (uniqueID != null) {
                    linkStatus = linkStatus == LinkEntityStatus.Undefined
                        ? LinkEntityStatus.LinkByStatistics
                        : linkStatus;
                    competitors.Each(raw => {
                        raw.CompetitoruniqueID = uniqueID.ID;
                        raw.Linkstatus = linkStatus;
                        raw.Save();
                    });
                }
                return competitors;
            }
        }

        private static CompetitorUnique TryGetCompetitorUniqueByResult(string nameFull, string nameShort, int competitionUnique, MatchParsed matchParsed) {
            if (matchParsed.Result == null || matchParsed.DateUtc == DateTime.MinValue) {
                return null;
            }
            var suitableCompetitionItems = CompetitionItem.DataSource
                .WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionUnique)
                .WhereBetween(CompetitionItem.Fields.Dateeventutc, matchParsed.DateUtc.AddDays(-1), matchParsed.DateUtc.AddDays(1), BetweenType.Inclusive)
                .AsMapByIds(
                    CompetitionItem.Fields.Competitoruniqueid1,
                    CompetitionItem.Fields.Competitoruniqueid2
                );
            var suitableCompetitionResults = CompetitionResult.DataSource
                .Join(JoinType.Left, CompetitionResultAdvanced.Fields.CompetitionresultID, CompetitionResult.Fields.ID, RetrieveMode.Retrieve)
                .WhereIn(CompetitionResult.Fields.CompetitionitemID, suitableCompetitionItems.Keys)
                .Sort(CompetitionResult.Fields.ID)
                .Sort(CompetitionResultAdvanced.Fields.ID)
                .AsList(
                    CompetitionResult.Fields.CompetitionitemID,
                    CompetitionResult.Fields.ScoreID,
                    CompetitionResultAdvanced.Fields.ScoreID
                )
                .GroupBy(cr => cr.CompetitionitemID)
                .ToDictionary(cr => cr.Key, cr => {
                    var competitionResultAdvanceds = cr
                        .Select(cra => cra.GetJoinedEntity<CompetitionResultAdvanced>())
                        .Where(je => je != null);
                    var enumerable = competitionResultAdvanceds as CompetitionResultAdvanced[] ?? competitionResultAdvanceds.ToArray();
                    var resultAdvanceds = enumerable.Any() ? enumerable.ToArray() : null;
                    return new ResultTransport {
                        ScoreID = cr.First().ScoreID,
                        SubScore = resultAdvanceds != null && resultAdvanceds.Any() ? resultAdvanceds.Select(cra => cra.ScoreID).ToArray() : null
                    };
                });
            var currentResultModel = new ResultTransport {
                ScoreID = ScoreHelper.Instance.GenerateScoreID(matchParsed.Result.CompetitorResultOne, matchParsed.Result.CompetitorResultTwo),
                SubScore = matchParsed.Result.SubResult.Select(sr => ScoreHelper.Instance.GenerateScoreID(sr.CompetitorResultOne, sr.CompetitorResultTwo)).ToArray()
            };
            var resultModelEqualityComparer = new ResultTransportEqualityComparer();
            var matchedByResults = suitableCompetitionResults
                .Where(kv => resultModelEqualityComparer.Equals(kv.Value, currentResultModel))
                .ToArray();
            return TryGetCompetitorUniqueByName(nameFull, nameShort, matchedByResults, matchParsed);
        }

        private static CompetitorUnique TryGetCompetitorUniqueByName(string nameFull, string nameShort, KeyValuePair<int, ResultTransport>[] mathedCompetitionItemsByResult, MatchParsed matchParsed) {
            if (matchParsed.DateUtc == DateTime.MinValue) {
                return null;
            }
            var suitableCompetitionItems = CompetitionItem.DataSource
                .WhereIn(CompetitionItem.Fields.ID, mathedCompetitionItemsByResult.Select(ci => ci.Key))
                .WhereBetween(CompetitionItem.Fields.Dateeventutc, matchParsed.DateUtc.AddDays(-1), matchParsed.DateUtc.AddDays(1), BetweenType.Inclusive)
                .AsList(
                    CompetitionItem.Fields.ID,
                    CompetitionItem.Fields.Competitoruniqueid1,
                    CompetitionItem.Fields.Competitoruniqueid2
                );
            var competitorToDetectIsFirst =
                nameShort.Equals(matchParsed.CompetitorNameShortOne, StringComparison.InvariantCultureIgnoreCase) ||
                nameFull.Equals(matchParsed.CompetitorNameFullOne, StringComparison.InvariantCultureIgnoreCase);
            var nameShortHash = new HashSet<char>(Transliterator.GetTranslit(CleanString(nameShort)));
            var nameFullHash = new HashSet<char>(Transliterator.GetTranslit(CleanString(nameFull)));
            var suitableCompetitors = Competitor.DataSource
                .WhereIn(Competitor.Fields.CompetitoruniqueID, (competitorToDetectIsFirst
                        ? suitableCompetitionItems.Select(sc => sc.Competitoruniqueid1)
                        : suitableCompetitionItems.Select(sc => sc.Competitoruniqueid2))
                    .Distinct())
                .AsList(Competitor.Fields.NameFull, Competitor.Fields.NameShort, Competitor.Fields.CompetitoruniqueID)
                .GroupBy(e => e.CompetitoruniqueID)
                .ToDictionary(g => g.Key, g => g.Select(e => SuitByNameFactor(nameFullHash, nameShortHash, e.NameFull, e.NameShort, nameFull, nameShort)).Max())
                .OrderByDescending(kv => kv.Value)
                .ToArray();
            if (suitableCompetitors.Length > 0 &&
                        (suitableCompetitors.Length == 1 && suitableCompetitors[0].Value >= .35) ||
                        (suitableCompetitors.Length > 1 && suitableCompetitors[0].Value >= .35 && 
                            ((suitableCompetitors[1].Value / suitableCompetitors[0].Value < .8) || suitableCompetitors[1].Key == suitableCompetitors[0].Key))) {
                _logger.Info("Для '{0}' поставляю CompetitionUniqueID {1} ({2}) K={3}", nameFull, suitableCompetitors[0].Key,
                                        Competitor.DataSource.WhereEquals(Competitor.Fields.CompetitoruniqueID, suitableCompetitors[0].Key)
                                                    .Sort(Competitor.Fields.ID)
                                                    .First().NameFull,
                                        suitableCompetitors[0].Value);
                return CompetitorUnique.DataSource.GetByKey(suitableCompetitors[0].Key);
            }
            return null;
        }

        private static float SuitByNameFactor(HashSet<char> nameFullEn, HashSet<char> nameShortEn, string nameFullTranslit, string nameShortTranslit, string nameFullSrc, string nameShortSrc) {
            nameShortTranslit = Transliterator.GetTranslit(CleanString(nameShortTranslit));
            nameFullTranslit = Transliterator.GetTranslit(CleanString(nameFullTranslit));

            var fullFactor = ((float)nameFullTranslit.Count(nameFullEn.Contains) / Math.Max(nameFullTranslit.Length, nameFullSrc.Length)) *
                (Math.Min(nameFullTranslit.Length, nameFullEn.Count) / (float)Math.Max(nameFullTranslit.Length, nameFullEn.Count));
            var shortFactor = ((float)nameShortTranslit.Count(nameShortEn.Contains) / Math.Max(nameShortTranslit.Length, nameShortSrc.Length)) *
                (Math.Min(nameShortTranslit.Length, nameShortEn.Count) / (float)Math.Max(nameShortTranslit.Length, nameShortEn.Count));
            return Math.Max(fullFactor, shortFactor);
        }

        private static string CleanString(string str) {
            return str
                .Replace("U-", string.Empty)
                .Replace(" до ", string.Empty)
                .Replace("ь", string.Empty)
                .Replace("'", string.Empty)
                .Replace(".", string.Empty)
                .Replace(",", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .ToLower();
        }
    }
}