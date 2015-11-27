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
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class CompetitorProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitorProvider).FullName);

        public CompetitorProvider() : base(_logger) { }

        public CompetitorParsedTransport GetCompetitor(LanguageType languageType, SportType sportType, GenderType genderType, string nameFull, string nameShort, int competitionUnique, MatchParsed matchParsed, GatherBehaviorMode algoMode) {
            return InvokeSafeSingleCall(() => {
                nameFull = (nameFull ?? nameShort).Trim();
                nameShort = (nameShort ?? nameFull).Trim();
                var competitors = Competitor.DataSource
                    .WhereEquals(Competitor.Fields.Gendertype, (short)genderType)
                    .WhereEquals(Competitor.Fields.Languagetype, (short)languageType)
                    .WhereEquals(Competitor.Fields.Sporttype, (short) sportType)
                    .Where(new DaoFilterOr(
                        QueryHelper.GetIndexedFilterByWordIgnoreCase(nameShort.ToLower(), Competitor.Fields.NameShort),
                        QueryHelper.GetIndexedFilterByWordIgnoreCase(nameFull.ToLower(), Competitor.Fields.NameFull))
                    )
                    .Sort(Competitor.Fields.ID, SortDirection.Asc)
                    .AsList(
                        Competitor.Fields.CompetitoruniqueID,
                        Competitor.Fields.NameFull,
                        Competitor.Fields.NameShort
                    );
                Competitor competitor = null;
                if (competitors.Count > 1) {
                    competitor = competitors[0];
                } else if (competitors.Count == 1) {
                    competitor = competitors[0];
                }
                if (competitor == null) {
                    CompetitorUnique uniqueID = null;
                    if (algoMode.HasFlag(GatherBehaviorMode.CanDetectCompetitor)) {
                        uniqueID = TryGetCompetitorUniqueByResult(nameFull, nameShort, competitionUnique, matchParsed);
                    }
                    if (uniqueID == null) {
                        if (!algoMode.HasFlag(GatherBehaviorMode.CreateIfNew)) {
                            return null;
                        }
                        uniqueID = new CompetitorUnique {
                            IsUsed = true
                        };
                        uniqueID.Save();
                    }
                    competitor = new Competitor {
                        CompetitoruniqueID = uniqueID.ID,
                        SportType = sportType,
                        Datecreatedutc = DateTime.UtcNow,
                        Languagetype = languageType,
                        NameFull = nameFull,
                        NameShort = nameShort,
                        Gendertype = genderType
                    };
                    competitor.Save();
                } else {
                    if (nameFull.Length != nameShort.Length) {
                        if (competitor.NameShort.Equals(nameShort, StringComparison.InvariantCultureIgnoreCase)) {
                            competitor.NameFull = nameFull;
                        } else if (competitor.NameFull.Equals(nameFull, StringComparison.InvariantCultureIgnoreCase)) {
                            competitor.NameShort = nameShort;
                        }
                    }
                    if (competitor.Changes.Count > 0) {
                        competitor.Save();
                    }
                }
                return new CompetitorParsedTransport {
                    UniqueID = competitor.CompetitoruniqueID,
                    NameFull = competitor.NameFull,
                    NameShort = competitor.NameShort,
                    GenderType = genderType,
                    SportType = sportType,
                    LanguageType = languageType
                };
            }, null);
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
                .ToDictionary(cr=> cr.Key, cr => {
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
                        ? suitableCompetitionItems.Select(sc =>sc.Competitoruniqueid1)
                        : suitableCompetitionItems.Select(sc => sc.Competitoruniqueid2))
                    .Distinct())
                .AsList(Competitor.Fields.NameFull, Competitor.Fields.NameShort, Competitor.Fields.CompetitoruniqueID)
                .GroupBy(e => e.CompetitoruniqueID)
                .ToDictionary(g => g.Key, g => g.Select(e => SuitByNameFactor(nameFullHash, nameShortHash, e.NameFull, e.NameShort, nameFull, nameShort)).Max())
                .OrderByDescending(kv => kv.Value)
                .ToArray();
            if (suitableCompetitors.Length > 0 && 
                        (suitableCompetitors.Length == 1 && suitableCompetitors[0].Value >= .32) ||
                        (suitableCompetitors.Length > 1 && suitableCompetitors[0].Value >= .32 && (suitableCompetitors[1].Value / suitableCompetitors[0].Value < .8))) {
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