using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeClientSide.TransportType.SubData;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper.RawData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Entity.Helper;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class RawCompetitorHelper {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (RawCompetitorHelper).FullName);

        public static List<RawTemplateObj<CompetitorParsedTransport>> GetCompetitor(BrokerType brokerType, LanguageType languageType, SportType sportType, GenderType genderType, string[] names, int competitionUnique, MatchParsed matchParsed, GatherBehaviorMode algoMode) {
            var competitorsRaw = QueryHelper.FilterByGender(RawCompetitor.DataSource
                                                .WhereEquals(RawCompetitor.Fields.Languagetype, (short)languageType)
                                                .WhereEquals(RawCompetitor.Fields.Sporttype, (short)sportType)
                                                .WhereEquals(RawCompetitor.Fields.Brokerid, (short)brokerType)
                                                .Where(QueryHelper.GetIndexedFilterByWordIgnoreCase(names, RawCompetitor.Fields.Name))
                                                .Sort(RawCompetitor.Fields.ID, SortDirection.Asc), 
                        RawCompetitor.Fields.Gendertype, 
                        genderType, 
                        RawCompetitor.Fields.CompetitoruniqueID,
                        RawCompetitor.Fields.Name,
                        RawCompetitor.Fields.Linkstatus
                    );
            if (competitorsRaw.Count > 1) {
                var count = competitorsRaw.GroupBy(c => c.CompetitoruniqueID).Count();
                if (count != 1) {
                    _logger.Error("{0} {1} {2} {3} {4} {5} {6}", brokerType, languageType, sportType, genderType, competitorsRaw.Select(cr => cr.ID).StrJoin(", "), names.StrJoin(", "), count);
                    return null;
                }
            }
            var result = AppendRawNewNames(names, competitorsRaw, brokerType, languageType, sportType, genderType)
                .Select(cr => new RawTemplateObj<CompetitorParsedTransport> {
                    RawObject = { ID = cr.ID},
                    Object = {
                        LanguageType = languageType,
                        SportType = sportType,
                        GenderType = genderType,
                        ID = cr.CompetitoruniqueID
                    }
                })
                .ToList();

            if (!competitorsRaw.Any() || competitorsRaw.All(c => c.CompetitoruniqueID == default(int))) {
                result = CreateCompetitorAndDetect(languageType, sportType, genderType, names, competitionUnique, matchParsed, algoMode, result);
            }

            return result;
        }

        private static List<RawCompetitor> AppendRawNewNames(string[] names, List<RawCompetitor> competitorsRaw, BrokerType brokerType, LanguageType languageType, SportType sportType, GenderType genderType) {
            var existNames = competitorsRaw.Select(cr => cr.Name).ToList();
            names = names
                .Where(name => !existNames.Contains(name))
                .ToArray();
            if (names.Any()) {
                var lastCompetitorUniqueID = competitorsRaw.Any()
                    ? competitorsRaw.First().CompetitoruniqueID
                    : default(int);
                var raw = competitorsRaw;
                names
                    .Where(name => !raw.Any(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                    .Each(name => {
                        var competitorRaw = BrokerEntityIfaceCreator.CreateEntity<RawCompetitor>(brokerType, languageType, sportType, genderType, LinkEntityStatus.ToLink, new[] {name},
                            competitor => {
                                if (lastCompetitorUniqueID != default(int)) {
                                    competitor.CompetitoruniqueID = lastCompetitorUniqueID;
                                    competitor.Linkstatus = LinkEntityStatus.LinkByStatistics;
                                }
                            });
                        competitorRaw.Save();
                        raw.Add(competitorRaw);
                    });
            }

            return competitorsRaw;
        }

        private static List<RawTemplateObj<CompetitorParsedTransport>> CreateCompetitorAndDetect(LanguageType languageType, SportType sportType, GenderType genderType, string[] names, int competitionUnique, MatchParsed matchParsed, GatherBehaviorMode algoMode, List<RawTemplateObj<CompetitorParsedTransport>> competitorFromRaw) {
            var uniqueID = algoMode.HasFlag(GatherBehaviorMode.CanDetectCompetitor) 
                ? TryGetCompetitorUniqueByResult(genderType, names, competitionUnique, matchParsed) 
                : null;

            if (algoMode.HasFlag(GatherBehaviorMode.CreateNewLanguageName) && uniqueID != null && !Competitor.DataSource
                        .WhereEquals(Competitor.Fields.CompetitoruniqueID, uniqueID.ID)
                        .WhereEquals(Competitor.Fields.Languagetype, (short) languageType)
                        .IsExists()) {
                new Competitor {
                    CompetitoruniqueID = uniqueID.ID,
                    SportType = sportType,
                    Datecreatedutc = DateTime.UtcNow,
                    Languagetype = languageType,
                    Name = names[0],
                    Gendertype = genderType
                }.Save();
            }
            var linkStatus = LinkEntityStatus.Undefined;
            if (algoMode.HasFlag(GatherBehaviorMode.CreateOriginal) && uniqueID == null) {
                uniqueID = new CompetitorUnique {
                    IsUsed = true
                };
                uniqueID.Save();
                var competitor = new Competitor {
                    CompetitoruniqueID = uniqueID.ID,
                    SportType = sportType,
                    Datecreatedutc = DateTime.UtcNow,
                    Languagetype = languageType,
                    Name = names[0],
                    Gendertype = genderType
                };
                competitor.Save();
                linkStatus = LinkEntityStatus.Original;
            }

            if (uniqueID != null) {
                linkStatus = linkStatus == LinkEntityStatus.Undefined
                    ? LinkEntityStatus.LinkByStatistics
                    : linkStatus;
                competitorFromRaw.Each(raw => {
                    raw.Object.ID = uniqueID.ID;
                    RawCompetitor.DataSource
                        .WhereEquals(RawCompetitor.Fields.ID, raw.RawObject.ID)
                        .Update(new Dictionary<Enum, DbFunction> {
                            {RawCompetitor.Fields.CompetitoruniqueID, new DbFnConst(uniqueID.ID) },
                            {RawCompetitor.Fields.Linkstatus, new DbFnConst((short)linkStatus) },
                        });
                });
            }
            return competitorFromRaw;
        }

        private static CompetitorUnique TryGetByFullEquality(GenderType genderType, string[] names, int competitionUnique, MatchParsed matchParsed) {
            var competitorIDs = CompetitionItem.DataSource
                .WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionUnique)
                .WhereBetween(CompetitionItem.Fields.Dateeventutc, matchParsed.DateUtc.AddHours(-5), matchParsed.DateUtc.AddHours(5), BetweenType.Inclusive)
                .AsList(CompetitionItem.Fields.Competitoruniqueid1,CompetitionItem.Fields.Competitoruniqueid2)
                .SelectMany(ci => new[] {ci.Competitoruniqueid1, ci.Competitoruniqueid2})
                .Distinct();
            var res = QueryHelper.FilterByGender(Competitor.DataSource.WhereIn(Competitor.Fields.ID, competitorIDs)
                                                            .Where(QueryHelper.GetIndexedFilterByWordIgnoreCase(names, Competitor.Fields.NameFull)),
                                        Competitor.Fields.Gendertype, genderType, Competitor.Fields.CompetitoruniqueID);
            if (res.Any()) {
                var distinctIds = res.Select(r => r.CompetitoruniqueID).Distinct().ToArray();
                if (distinctIds.Length == 1) {
                    return CompetitorUnique.DataSource.GetByKey(distinctIds[0]);
                }
            }
            return null;
        }

        private static CompetitorUnique TryGetCompetitorUniqueByResult(GenderType genderType, string[] names, int competitionUnique, MatchParsed matchParsed) {
            if (matchParsed.DateUtc == DateTime.MinValue) {
                return null;
            }
            var byFullEqiality = TryGetByFullEquality(genderType, names, competitionUnique, matchParsed);
            if (byFullEqiality != null) {
                return byFullEqiality;
            }
            if (matchParsed.Result == null) {
                return null;
            }
            var suitableCompetitionItems = CompetitionItem.DataSource
                .WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionUnique)
                .WhereBetween(CompetitionItem.Fields.Dateeventutc, matchParsed.DateUtc.AddHours(-4), matchParsed.DateUtc.AddHours(4), BetweenType.Inclusive)
                .AsIds();
            var suitableCompetitionResults = CompetitionResult.DataSource
                .Join(JoinType.Left, CompetitionResultAdvanced.Fields.CompetitionresultID, CompetitionResult.Fields.ID, RetrieveMode.Retrieve)
                .WhereIn(CompetitionResult.Fields.CompetitionitemID, suitableCompetitionItems)
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
                .Where(kv => resultModelEqualityComparer.Equals(kv.Value, currentResultModel) || resultModelEqualityComparer.Equals(currentResultModel, kv.Value))
                .ToArray();
            return TryGetCompetitorUniqueByName(names, matchedByResults, matchParsed);
        }

        private static CompetitorUnique TryGetCompetitorUniqueByName(string[] names, KeyValuePair<int, ResultTransport>[] mathedCompetitionItemsByResult, MatchParsed matchParsed) {
            if (matchParsed.DateUtc == DateTime.MinValue) {
                return null;
            }
            var suitableCompetitionItems = CompetitionItem.DataSource
                .WhereIn(CompetitionItem.Fields.ID, mathedCompetitionItemsByResult.Select(ci => ci.Key))
                .WhereBetween(CompetitionItem.Fields.Dateeventutc, matchParsed.DateUtc.AddHours(-4), matchParsed.DateUtc.AddHours(4), BetweenType.Inclusive)
                .AsList(
                    CompetitionItem.Fields.ID,
                    CompetitionItem.Fields.Competitoruniqueid1,
                    CompetitionItem.Fields.Competitoruniqueid2
                );
            var competitorToDetectIsFirst = matchParsed.CompetitorName1.Any(names.Contains);
            var namesHash = names.Select(name => new HashSet<char>(Transliterator.GetTranslit(CleanString(name)))).ToArray();
            var suitableCompetitors = RawCompetitor.DataSource
                .WhereIn(RawCompetitor.Fields.CompetitoruniqueID, (competitorToDetectIsFirst
                        ? suitableCompetitionItems.Select(sc => sc.Competitoruniqueid1)
                        : suitableCompetitionItems.Select(sc => sc.Competitoruniqueid2))
                    .Distinct())
                .AsList(RawCompetitor.Fields.Name, RawCompetitor.Fields.CompetitoruniqueID)
                .Select(rc => (ICompetitor) rc)
                .Union(Competitor.DataSource
                                    .WhereIn(Competitor.Fields.CompetitoruniqueID, (competitorToDetectIsFirst
                                            ? suitableCompetitionItems.Select(sc => sc.Competitoruniqueid1)
                                            : suitableCompetitionItems.Select(sc => sc.Competitoruniqueid2))
                                        .Distinct())
                                    .AsList(Competitor.Fields.NameFull, Competitor.Fields.CompetitoruniqueID)
                                    .Select(rc => (ICompetitor)rc))
                .GroupBy(e => e.CompetitoruniqueID)
                .ToDictionary(g => g.Key, g => SuitByNameFactor(names, namesHash, g.Select(rc => rc.Name)))
                .OrderByDescending(kv => kv.Value)
                .ToArray();
            if (suitableCompetitors.Length > 0 && suitableCompetitors[0].Value >= .35 &&
                        (suitableCompetitors.Length == 1 ||
                        (suitableCompetitors.Length > 1 && suitableCompetitors[1].Value / suitableCompetitors[0].Value < .8))) {
                _logger.Info("Для '{0}' поставляю CompetitionUniqueID {1} ({2}) K={3}", names.First(), suitableCompetitors[0].Key,
                                        Competitor.DataSource.WhereEquals(Competitor.Fields.CompetitoruniqueID, suitableCompetitors[0].Key)
                                                    .Sort(Competitor.Fields.ID)
                                                    .First().Name,
                                        suitableCompetitors[0].Value);
                return CompetitorUnique.DataSource.GetByKey(suitableCompetitors[0].Key);
            }
            return null;
        }

        private static float SuitByNameFactor(string[] names, HashSet<char>[] namesHash, IEnumerable<string> namesToSearch) {
            return namesToSearch
                .Select(n => Transliterator.GetTranslit(CleanString(n)))
                .Select(name => {
                    var coeffs = new List<float>();
                    for (var i = 0; i < names.Length; i++) {
                        var hashSet = namesHash[i];
                        coeffs.Add(((float)name.Count(hashSet.Contains) / Math.Max(name.Length, names[i].Length)) *
                            (Math.Min(name.Length, hashSet.Count) / (float)Math.Max(name.Length, hashSet.Count)));
                    }
                    return coeffs.Max();
                })
                .Max();
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