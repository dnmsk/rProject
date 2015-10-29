using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.Code.Data;
using Project_B.Code.DataProvider.DataHelper;
using Project_B.Code.DataProvider.Transport;
using Project_B.Code.Entity;
using Project_B.Code.Enums;
using Project_B.Models;

namespace Project_B.Code.DataProvider {
    public class CompetitorProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitorProvider).FullName);

        public CompetitorProvider() : base(_logger) { }

        public CompetitorTransport GetCompetitor(LanguageType languageType, SportType sportType, GenderType genderType, string nameFull, string nameShort, int competitionUnique, MatchParsed matchParsed, bool canCreateIfNew) {
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
                    var uniqueID = TryGetCompetitorUniqueByResult(nameFull, nameShort, competitionUnique, matchParsed);
                    if (uniqueID == null) {
                        if (!canCreateIfNew) {
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
                return new CompetitorTransport {
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
                .WhereBetween(CompetitionItem.Fields.Dateeventutc, matchParsed.DateUtc.AddDays(-2), matchParsed.DateUtc.AddDays(2), BetweenType.Inclusive)
                .AsMapByIds(
                    CompetitionItem.Fields.Competitoruniqueid1,
                    CompetitionItem.Fields.Competitoruniqueid2
                );
            var suitableCompetitionResults = CompetitionResult.DataSource
                .Join(JoinType.Left, CompetitionResultAdvanced.Fields.CompetitionresultID, CompetitionResult.Fields.ID, RetrieveMode.Retrieve)
                .WhereIn(CompetitionResult.Fields.CompetitionitemID, suitableCompetitionItems.Keys)
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
                    return new ResultModel {
                        ScoreID = cr.First().ScoreID,
                        SubScore = resultAdvanceds != null && resultAdvanceds.Any() ? resultAdvanceds.Select(cra => cra.ScoreID).ToArray() : null
                    };
                });
            var currentResultModel = new ResultModel {
                ScoreID = ScoreHelper.Instance.GenerateScoreID(matchParsed.Result.CompetitorResultOne, matchParsed.Result.CompetitorResultTwo),
                SubScore = matchParsed.Result.SubResult.Select(sr => ScoreHelper.Instance.GenerateScoreID(sr.CompetitorResultOne, sr.CompetitorResultTwo)).ToArray()
            };
            KeyValuePair<int, ResultModel>[] matchedByResults = suitableCompetitionResults
                .Where(kv => {
                    var res = kv.Value;
                    if (res.ScoreID != currentResultModel.ScoreID) {
                        return false;
                    }
                    if (res.SubScore == null || currentResultModel.SubScore == null) {
                        return true;
                    }
                    if (res.SubScore.Length != currentResultModel.SubScore.Length) {
                        return false;
                    }
                    for (var i = 0; i < currentResultModel.SubScore.Length; i++) {
                        if (res.SubScore[i] != currentResultModel.SubScore[i]) {
                            return false;
                        }
                    }
                    return true;
                })
                .ToArray();
            if (matchedByResults.Length == 1) {
                var competitorIsFirst =
                    nameShort.Equals(matchParsed.CompetitorNameShortOne, StringComparison.InvariantCultureIgnoreCase) ||
                    nameFull.Equals(matchParsed.CompetitorNameFullOne, StringComparison.InvariantCultureIgnoreCase);
                var competitionItem = suitableCompetitionItems[matchedByResults[0].Key];
                var competitorUniqueID = competitorIsFirst ? competitionItem.Competitoruniqueid1 : competitionItem.Competitoruniqueid2;
                _logger.Info("Для '{0}' поставляю CompetitionUniqueID {1} ({2})", nameFull, matchedByResults[0].Key,
                    Competitor.DataSource.WhereEquals(Competitor.Fields.CompetitoruniqueID, competitorUniqueID).Sort(Competitor.Fields.ID).First().NameFull);
                return CompetitorUnique.DataSource.GetByKey(competitorUniqueID);
            } else {
                return TryGetCompetitorUniqueByName(nameFull, nameShort, matchedByResults, matchParsed);
            }

            return null;
        }

        private static CompetitorUnique TryGetCompetitorUniqueByName(string nameFull, string nameShort, KeyValuePair<int, ResultModel>[] mathedCompetitionItemsByResult, MatchParsed matchParsed) {
            if (matchParsed.DateUtc == DateTime.MinValue) {
                return null;
            }
            var suitableCompetitionItems = CompetitionItem.DataSource
                .WhereIn(CompetitionItem.Fields.CompetitionuniqueID, mathedCompetitionItemsByResult.Select(ci => ci.Key))
                .WhereBetween(CompetitionItem.Fields.Dateeventutc, matchParsed.DateUtc.AddDays(-1), matchParsed.DateUtc.AddDays(1), BetweenType.Inclusive)
                .AsList(
                    CompetitionItem.Fields.ID,
                    CompetitionItem.Fields.Competitoruniqueid1,
                    CompetitionItem.Fields.Competitoruniqueid2
                );
            var nameShortHash = new HashSet<char>(Transliterator.GetTranslit(nameShort).ToLower().Select(c => c).Distinct());
            var nameFullHash = new HashSet<char>(Transliterator.GetTranslit(nameFull).ToLower().Select(c => c).Distinct());
            var suitableCompetitors = Competitor.DataSource
                .WhereIn(Competitor.Fields.CompetitoruniqueID, suitableCompetitionItems.Select(sc =>sc.Competitoruniqueid1)
                                                                                       .Union(suitableCompetitionItems.Select(sc => sc.Competitoruniqueid2))
                                                                                       .Distinct())
                .AsList(Competitor.Fields.NameFull, Competitor.Fields.NameShort, Competitor.Fields.CompetitoruniqueID)
                .GroupBy(e => e.CompetitoruniqueID)
                .ToDictionary(g => g.Key, g => g.Select(e => SuitByNameFactor(nameFullHash, nameShortHash, e.NameFull, e.NameShort)).Max())
                .OrderByDescending(kv => kv.Value)
                .ToArray();
            if (suitableCompetitors.Length > 0 && suitableCompetitors[0].Value >= .6 &&
                (suitableCompetitors.Length <= 1 || !(suitableCompetitors[0].Value - suitableCompetitors[1].Value < .15))) {
                    _logger.Info("Для '{0}' поставляю CompetitionUniqueID {1} ({2})", nameFull, suitableCompetitors[0].Key,
                                            Competitor.DataSource.WhereEquals(Competitor.Fields.CompetitoruniqueID, suitableCompetitors[0].Key)
                                                        .Sort(Competitor.Fields.ID)
                                                        .First().NameFull);
                    return CompetitorUnique.DataSource.GetByKey(suitableCompetitors[0].Key);
            }
            return null;
        }

        private static float SuitByNameFactor(HashSet<char> nameFullEn, HashSet<char> nameShortEn, string nameFullDiff, string nameShortDiff) {
            nameShortDiff = Transliterator.GetTranslit(nameShortDiff.ToLower());
            nameFullDiff = Transliterator.GetTranslit(nameFullDiff.ToLower());

            var distinctCharsShortCnt = nameShortDiff.Distinct().Count();
            var distinctCharsFullCnt = nameFullDiff.Distinct().Count();

            var fullFactor = ((float)nameFullDiff.Count(nameFullEn.Contains) / nameFullDiff.Length) * 
                (distinctCharsShortCnt > nameFullEn.Count ? nameFullEn.Count / (float) distinctCharsFullCnt : distinctCharsFullCnt / (float) nameFullEn.Count);
            var shortFactor = ((float)nameShortDiff.Count(nameShortEn.Contains) / nameShortDiff.Length) *
                (distinctCharsShortCnt > nameShortEn.Count ? nameShortEn.Count / (float)distinctCharsShortCnt : distinctCharsShortCnt / (float)nameShortEn.Count);
            return fullFactor > shortFactor ? fullFactor : shortFactor;
        }
    }
}