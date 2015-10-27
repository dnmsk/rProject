using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.Code.Data;
using Project_B.Code.DataProvider.DataHelper;
using Project_B.Code.DataProvider.Transport;
using Project_B.Code.Entity;
using Project_B.Code.Entity.Interface;
using Project_B.Code.Enums;
using Project_B.Models;

namespace Project_B.Code.DataProvider {
    public class CompetitionProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitionProvider).FullName);
        
        public CompetitionProvider() : base(_logger) {}

        public CompetitionTransport GetCompetition(LanguageType language, SportType sportType, List<string> nameOrigin, CompetitionParsed competitionToSave) {
            return InvokeSafeSingleCall(() => {
                nameOrigin = SportTypeHelper.Instance.ExcludeSportTypeFromList(nameOrigin);
                var genderDetected = GenderDetectorHelper.Instance[nameOrigin];
                var competition = Competition.DataSource
                                             .WhereEquals(Competition.Fields.Gendertype, (short) genderDetected)
                                             .WhereEquals(Competition.Fields.Languagetype, (short)language)
                                             .WhereEquals(Competition.Fields.Sporttype, (short)sportType)
                                             .Where(QueryHelper.GetFilterByWordsForField(nameOrigin, Competition.Fields.Name))
                                             .First(
                                                Competition.Fields.CompetitionuniqueID,
                                                Competition.Fields.Name
                    );
                if (competition == null) {
                    return GetCompetitionUnique(language, sportType, genderDetected, nameOrigin, competitionToSave);
                }
                return new CompetitionTransport {
                    Name = competition.Name,
                    GenderType = genderDetected,
                    SportType = sportType,
                    LanguageType = language,
                    UniqueID = competition.CompetitionuniqueID
                };
            }, null);
        }

        private CompetitionTransport GetCompetitionUnique(LanguageType language, SportType sportType, GenderType genderDetected, List<string> nameOrigin, CompetitionParsed competitionToSave) {
            var nameOriginShort = CompetitionHelper.GetShortCompetitionName(nameOrigin);
            var competitionUniqueAdvanced = CompetitionUniqueAdvanced.DataSource
                                         .WhereEquals(CompetitionUniqueAdvanced.Fields.Gendertype, (short)genderDetected)
                                         .WhereEquals(CompetitionUniqueAdvanced.Fields.Languagetype, (short)language)
                                         .WhereEquals(CompetitionUniqueAdvanced.Fields.Sporttype, (short)sportType)
                                         .Where(QueryHelper.GetFilterByWordsForField(nameOriginShort, CompetitionUniqueAdvanced.Fields.Name))
                                         .First(CompetitionUniqueAdvanced.Fields.CompetitionuniqueID);
            if (competitionUniqueAdvanced == null) {
                var uniqueID = TryDetectCompetitionUniqueFromMatches(sportType, nameOrigin, competitionToSave);
                if (uniqueID == null) {
                    uniqueID = new CompetitionUnique {
                        IsUsed = true
                    };
                    uniqueID.Save();
                }
                competitionUniqueAdvanced = new CompetitionUniqueAdvanced {
                    Datecreatedutc = DateTime.UtcNow,
                    Languagetype = language,
                    Sporttype = sportType,
                    Name = ListStringToName(nameOriginShort),
                    Gendertype = genderDetected,
                    CompetitionuniqueID = uniqueID.ID
                };
                competitionUniqueAdvanced.Save();
            }

            var competition = new Competition {
                Datecreatedutc = DateTime.UtcNow,
                Languagetype = language,
                SportType = sportType,
                Name = ListStringToName(nameOrigin),
                Gendertype = genderDetected,
                CompetitionuniqueID = competitionUniqueAdvanced.CompetitionuniqueID
            };
            competition.Save();

            return new CompetitionTransport {
                Name = competition.Name,
                GenderType = genderDetected,
                SportType = sportType,
                LanguageType = language,
                UniqueID = competition.CompetitionuniqueID
            };
        }

        private static CompetitionUnique TryDetectCompetitionUniqueFromMatches(SportType sportType, List<string> nameOrigin, CompetitionParsed competitionToSave) {
            var dates = competitionToSave.Matches.Select(c => c.DateUtc).Where(d => d != DateTime.MinValue).ToArray();
            var minDate = dates.Any() ? dates.Min().Date : DateTime.MinValue;
            var maxate = dates.Any() ? dates.Max().Date : DateTime.MinValue;
            var suitableСompetitionItems = CompetitionItem.DataSource
                .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType)
                .WhereBetween(CompetitionItem.Fields.Dateeventutc, minDate, maxate.AddDays(1), BetweenType.Inclusive)
                .AsMapByField<int>(CompetitionItem.Fields.CompetitionuniqueID, CompetitionItem.Fields.ID);
            if (minDate < DateTime.UtcNow.Date) {
                var mapResults = CompetitionResult.DataSource
                    .Join(JoinType.Inner, CompetitionItem.Fields.ID, CompetitionResult.Fields.CompetitionitemID, RetrieveMode.Retrieve)
                    .Join(JoinType.Inner, CompetitionResultAdvanced.Fields.CompetitionresultID, CompetitionResult.Fields.ID, RetrieveMode.Retrieve)
                    .WhereIn(CompetitionItem.Fields.CompetitionuniqueID, suitableСompetitionItems.Keys)
                    .WhereBetween(CompetitionItem.Fields.Dateeventutc, minDate, maxate.AddDays(1), BetweenType.Inclusive)
                    .AsList(CompetitionItem.Fields.CompetitionuniqueID, CompetitionResult.Fields.ScoreID, CompetitionResultAdvanced.Fields.ScoreID)
                    .GroupBy(e => e.GetJoinedEntity<CompetitionItem>().CompetitionuniqueID)
                    .ToDictionary(g => g.Key, g=> g.GroupBy(gr => gr.ID).Select(gr => new ResultModel {
                        ScoreID = gr.First().ScoreID,
                        SubScore = gr.Select(gra => gra.GetJoinedEntity<CompetitionResultAdvanced>().ScoreID).ToArray()
                    }).ToList());
                var mapCoefficients = new Dictionary<int, float>();
                var hashResults = competitionToSave.Matches
                    .Where(m => m.Result != null)
                    .Select(m => new ResultModel {
                        ScoreID = ScoreHelper.Instance.GenerateScoreID(m.Result.CompetitorResultOne, m.Result.CompetitorResultTwo),
                        SubScore = m.Result.SubResult.Any() 
                            ? m.Result.SubResult.Select(sr => ScoreHelper.Instance.GenerateScoreID(sr.CompetitorResultOne, sr.CompetitorResultTwo)).ToArray()
                            : new short[0]
                    })
                    .ToArray();
                foreach (var suitableСompetitionItem in suitableСompetitionItems) {
                    List<ResultModel> resultsForCompetition;
                    if (!mapResults.TryGetValue(suitableСompetitionItem.Key, out resultsForCompetition) || resultsForCompetition.Count == 0) {
                        mapCoefficients[suitableСompetitionItem.Key] = 0;
                        continue;
                    }
                    var successMatches = resultsForCompetition.Count(res => hashResults.Any(h => {
                        if (h.ScoreID != res.ScoreID) {
                            return false;
                        }
                        if (h.SubScore.Length == 0 || res.SubScore.Length == 0) {
                            return true;
                        }
                        if (h.SubScore.Length != res.SubScore.Length) {
                            return false;
                        }
                        for (var i = 0; i < h.SubScore.Length; i++) {
                            if (h.SubScore[i] != res.SubScore[i]) {
                                return false;
                            }
                        }
                        return true;
                    }));
                    mapCoefficients[suitableСompetitionItem.Key] = successMatches/(float)hashResults.Length;
                }
                var orderedCompetitionCoeffs = mapCoefficients.OrderByDescending(kv => kv.Value).ToList();
                if (orderedCompetitionCoeffs.Count == 0) {
                    return null;
                }
                if (orderedCompetitionCoeffs.First().Value > .3 && (orderedCompetitionCoeffs.Count == 1 || (orderedCompetitionCoeffs[0].Value - orderedCompetitionCoeffs[1].Value) > .1)) {
                    var key = orderedCompetitionCoeffs.First().Key;
                    _logger.Info("Для '{0}' поставляю CompetitionUniqueID {1} ({2})", nameOrigin.StrJoin(". "), key, 
                        Competition.DataSource.WhereEquals(Competition.Fields.CompetitionuniqueID, key).Sort(Competition.Fields.ID).First().Name);
                    return CompetitionUnique.DataSource.GetByKey(key);
                }
                return null;
            }
            var counts = suitableСompetitionItems.Where(sit => sit.Value.Count == competitionToSave.Matches.Count).ToArray();
            if (counts.Length == 1) {
                _logger.Info("Для '{0}' поставляю CompetitionUniqueID {1} ({2})", nameOrigin.StrJoin(". "), counts[0].Key,
                    Competition.DataSource.WhereEquals(Competition.Fields.CompetitionuniqueID, counts[0].Key).Sort(Competition.Fields.ID).First().Name);
                return CompetitionUnique.DataSource.GetByKey(counts[0].Key);
            }
            return null;
        }

        private static string ListStringToName(List<string> names) {
            return names.StrJoin(". ");
        }

        public int GetCompetitionItem(CompetitorTransport competitor1Transport, CompetitorTransport competitor2Transport, CompetitionTransport competitionTransport, DateTime eventDateUtc) {
            return InvokeSafeSingleCall(() => {
                var source = CompetitionItem.DataSource
                        .Where(new DaoFilterOr(
                            new DaoFilterAnd(
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitor1Transport.UniqueID),
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitor2Transport.UniqueID)
                                ),
                            new DaoFilterAnd(
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitor1Transport.UniqueID),
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitor2Transport.UniqueID)
                                )
                            ))
                        .WhereEquals(CompetitionItem.Fields.Sporttype, (short)competitionTransport.SportType)
                        .WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionTransport.UniqueID);
                if (eventDateUtc > DateTime.MinValue) {
                    source = source
                        .Where(new DaoFilterOr(
                            new DaoFilterAnd(
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, eventDateUtc.AddDays(-1)),
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.LessOrEq, eventDateUtc.AddDays(1))
                            ),
                            new DaoFilterAnd(
                                new DaoFilter(CompetitionItem.Fields.Datecreatedutc, Oper.GreaterOrEq, eventDateUtc.AddDays(-1)),
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.LessOrEq, eventDateUtc.AddDays(1))
                            )
                        ));
                }
                var competitionItem = source
                    .Sort(CompetitionItem.Fields.ID, SortDirection.Desc)
                    .First(CompetitionItem.Fields.ID, CompetitionItem.Fields.Dateeventutc);
                if (competitionItem != null && eventDateUtc != DateTime.MinValue && competitionItem.Dateeventutc != eventDateUtc) {
                    competitionItem.Dateeventutc = eventDateUtc;
                    competitionItem.Save();
                }
                if (competitionItem == null) {
                    competitionItem = new CompetitionItem {
                        SportType = competitionTransport.SportType,
                        Datecreatedutc = DateTime.UtcNow,
                        Dateeventutc = eventDateUtc,
                        CompetitionuniqueID = competitionTransport.UniqueID,
                        Competitoruniqueid1 = competitor1Transport.UniqueID,
                        Competitoruniqueid2 = competitor2Transport.UniqueID
                    };
                    competitionItem.Save();
                }
                return competitionItem.ID;
            }, default(int));
        }

        public List<CompetitionItemBetShortModel> GetCompetitionItemsRegularBet(LanguageType languageType, SportType sportType, DateTime fromDate, DateTime toDate) {
            return InvokeSafe(() => {
                var shortModels = GetCompetitionItemShortModelByDate(languageType, sportType, fromDate, toDate);
                return GetCompetitiontItemBetModel(shortModels, true, GetBetMap);
            }, null);
        }

        private Dictionary<int, List<IBet<int>>> GetBetMap(IEnumerable<int> ints) {
            return Bet.DataSource
                .Join(JoinType.Left, BetAdvanced.Fields.BetID, Bet.Fields.ID, RetrieveMode.Retrieve).WhereIn(Bet.Fields.CompetitionitemID, ints)
                .AsList()
                .GroupBy(e => e.CompetitionitemID)
                .ToDictionary(e => e.Key, e => e.Select(t => (IBet<int>) t)
                .ToList());
        }

        private Dictionary<int, List<IBet<long>>> GetLiveBetMap(IEnumerable<int> ints) {
            return BetLive.DataSource
                .Join(JoinType.Left, BetLiveAdvanced.Fields.BetliveID, BetLive.Fields.ID, RetrieveMode.Retrieve)
                //.Join(JoinType.Left, CompetitionResult.Fields.CompetitionitemID, BetLive.Fields.CompetitionitemID, RetrieveMode.NotRetrieve)
                //.WhereNull(CompetitionResult.Fields.ID)
                .WhereIn(BetLive.Fields.CompetitionitemID, ints)
                .AsList()
                .GroupBy(e => e.CompetitionitemID)
                .ToDictionary(e => e.Key, e => e.Select(t => (IBet<long>)t)
                .ToList());
        }

        public List<CompetitionItemBetShortModel> GetCompetitionItemsLiveBet(LanguageType languageType, SportType sportType) {
            return InvokeSafe(() => {
                var fromDate = DateTime.UtcNow.AddHours(-3);
                var toDate = DateTime.UtcNow.AddHours(24);
                var shortModels = GetCompetitionItemShortModelByDate(languageType, sportType, fromDate, toDate);
                return GetCompetitiontItemBetModel(shortModels, true, GetLiveBetMap);
            }, null);
        }

        public CompetitionItemBetShortModel GetCompetitionItemRegularBet(LanguageType languageType, int competitionItemID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereIn(CompetitionItem.Fields.ID, new[] { competitionItemID }));
                return GetCompetitiontItemBetModel(competition, true, GetBetMap).FirstOrDefault();
            }, null);
        }
        
        public CompetitionItemBetShortModel GetCompetitionItemLiveBet(LanguageType languageType, int competitionItemID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereIn(CompetitionItem.Fields.ID, new[] { competitionItemID }));
                return GetCompetitiontItemBetModel(competition, false, GetLiveBetMap).FirstOrDefault();
            }, null);
        }

        public List<CompetitionItemBetShortModel> GetCompetitionItemsRegularBetForCompetition(LanguageType languageType, int competitionID, bool takeOnlyInFutured) {
            return InvokeSafe(() => {
                var competitionItemQuery = CompetitionItem.DataSource
                    .WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionID);
                if (takeOnlyInFutured) {
                    competitionItemQuery = competitionItemQuery
                        .Where(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, DateTime.UtcNow);
                }
                var competition = GetCompetitionItemShortModel(languageType, competitionItemQuery);
                return GetCompetitiontItemBetModel(competition, true, GetBetMap);
            }, null);
        }
        
        public List<CompetitionItemBetShortModel> GetCompetitionItemLiveBetForCompetition(LanguageType languageType, int competitionID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionID));
                return GetCompetitiontItemBetModel(competition, false, GetLiveBetMap);
            }, null);
        }

        public List<CompetitionItemBetShortModel> GetCompetitionItemsRegularBetForCompetitor(LanguageType languageType, int competitorID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource
                    .Where(new DaoFilterOr(
                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitorID),
                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitorID)
                    )));
                return GetCompetitiontItemBetModel(competition, true, GetBetMap);
            }, null);
        }
        
        public List<CompetitionItemBetShortModel> GetCompetitionItemLiveBetForCompetitor(LanguageType languageType, int competitorID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource
                    .Where(new DaoFilterOr(
                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitorID),
                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitorID)
                    )));
                return GetCompetitiontItemBetModel(competition, false, GetLiveBetMap);
            }, null);
        }
        
        private static List<CompetitionItemBetShortModel> GetCompetitiontItemBetModel<T>(List<CompetitionItemShortModel> competitionItemShortModels, bool addEmptyBets, Func<IEnumerable<int>, Dictionary<int, List<IBet<T>>>> getBetMap) {
            var betGrouped = getBetMap(competitionItemShortModels.Select(c => c.CompetitionID));
            var result = new List<CompetitionItemBetShortModel>();
            foreach (var competitionItem in competitionItemShortModels) {
                var betsForCompetition = betGrouped.TryGetValueOrDefaultStruct(competitionItem.CompetitionID);
                if (betsForCompetition == null) {
                    if (addEmptyBets) {
                        result.Add(new CompetitionItemBetShortModel(competitionItem));
                    }
                    continue;
                }
                var itemModel = new CompetitionItemBetShortModel(competitionItem) {
                    CurrentBets = BuildCurrentOddsMap(competitionItem.SportType, betsForCompetition, true, (cur, next) => cur < next),
                    HistoryMaxBets = BuildCurrentOddsMap(competitionItem.SportType, betsForCompetition, false, (cur, next) => cur < next),
                    HistoryMinBets = BuildCurrentOddsMap(competitionItem.SportType, betsForCompetition, false, (cur, next) => cur > next && next != default(float)),
                };
                result.Add(itemModel);
            }
            return result;
        }

        private static List<CompetitionItemShortModel> GetCompetitionItemShortModelByDate(LanguageType languageType, SportType sportType, DateTime fromDate, DateTime toDate) {
            var competitionItemForDateQuery = CompetitionItem.DataSource
                .Where(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, fromDate)
                .Where(CompetitionItem.Fields.Dateeventutc, Oper.Less, toDate);
            if (sportType != SportType.Unknown) {
                competitionItemForDateQuery = competitionItemForDateQuery
                    .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType);
            }
            return GetCompetitionItemShortModel(languageType, competitionItemForDateQuery);
        }

        private static List<CompetitionItemShortModel> GetCompetitionItemShortModel(LanguageType languageType, DbDataSource<CompetitionItem, int> competitionItemQuery) {
            var competitionNameMapQuery = CompetitionUniqueAdvanced.DataSource;
            var competitorNameMapQuery = Competitor.DataSource;
            var competitionItemForDate = competitionItemQuery
               .WhereNotEquals(CompetitionItem.Fields.Dateeventutc, DateTime.MinValue)
               .Sort(CompetitionItem.Fields.Sporttype)
               .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Asc)
               .AsList(
                    CompetitionItem.Fields.ID,
                    CompetitionItem.Fields.Dateeventutc,
                    CompetitionItem.Fields.Competitoruniqueid1,
                    CompetitionItem.Fields.Competitoruniqueid2,
                    CompetitionItem.Fields.Sporttype,
                    CompetitionItem.Fields.CompetitionuniqueID
                );
            var competitionNameMap = competitionNameMapQuery
                .WhereIn(CompetitionUniqueAdvanced.Fields.CompetitionuniqueID, competitionItemForDate.Select(c => c.CompetitionuniqueID))
                .Sort(CompetitionUniqueAdvanced.Fields.ID, SortDirection.Asc)
                .AsList(
                    CompetitionUniqueAdvanced.Fields.CompetitionuniqueID,
                    CompetitionUniqueAdvanced.Fields.Name,
                    CompetitionUniqueAdvanced.Fields.Languagetype
                )
                .GroupBy(e => e.CompetitionuniqueID)
                .ToDictionary(e => e.Key, e => {
                    var withCurrentLanguage = e.Where(i => i.Languagetype == languageType);
                    var competitionUniqueAdvanceds = withCurrentLanguage as CompetitionUniqueAdvanced[] ?? withCurrentLanguage.ToArray();
                    return competitionUniqueAdvanceds.Any() ? competitionUniqueAdvanceds.First() : e.First();
                });
            var competitorNameMap = competitorNameMapQuery
                .WhereIn(Competitor.Fields.CompetitoruniqueID, competitionItemForDate.Select(c => c.Competitoruniqueid1)
                                                                                     .Union(competitionItemForDate.Select(c => c.Competitoruniqueid2))
                                                                                     .Distinct())
                .Sort(Competitor.Fields.ID, SortDirection.Asc)
                .AsList(
                    Competitor.Fields.Languagetype,
                    Competitor.Fields.CompetitoruniqueID,
                    Competitor.Fields.NameShort,
                    Competitor.Fields.NameFull
                )
                .GroupBy(e => e.CompetitoruniqueID)
                .ToDictionary(e => e.Key, e => {
                    var withCurrentLanguage = e.Where(i => i.Languagetype == languageType);
                    var competitors = withCurrentLanguage as Competitor[] ?? withCurrentLanguage.ToArray();
                    return competitors.Any() ? competitors.First() : e.First();
                });

            return competitionItemForDate
                .Select(ci => new CompetitionItemShortModel {
                    CompetitionID = ci.ID,
                    DateUtc = ci.Dateeventutc,
                    SportType = ci.SportType,
                    Competitor1 = ExtractNameFromCompetitor(ci.Competitoruniqueid1, competitorNameMap),
                    Competitor2 = ExtractNameFromCompetitor(ci.Competitoruniqueid2, competitorNameMap),
                    Competition = ExtractNameFromCompetition(ci.CompetitionuniqueID, competitionNameMap)
                })
                .ToList();
        }

        private static CompetitorModel ExtractNameFromCompetitor(int competitorID, Dictionary<int, Competitor> competitorNameMap) {
            Competitor competitor;
            if (!competitorNameMap.TryGetValue(competitorID, out competitor)) {
                competitor = Competitor.DataSource
                .WhereEquals(Competitor.Fields.CompetitoruniqueID, competitorID)
                .First();
            }
            return new CompetitorModel {
                ID = competitorID,
                Name = competitor.NameFull ?? competitor.NameShort
            };
        }

        private static CompetitionModel ExtractNameFromCompetition(int competitionID, Dictionary<int, CompetitionUniqueAdvanced> competitionNameMap) {
            CompetitionUniqueAdvanced competition;
            if (!competitionNameMap.TryGetValue(competitionID, out competition)){
                competition = CompetitionUniqueAdvanced.DataSource
                    .WhereEquals(CompetitionUniqueAdvanced.Fields.CompetitionuniqueID, competitionID)
                    .First();
            }
            return new CompetitionModel {
                ID = competitionID,
                Name = competition.Name
            };
        }

        private static Dictionary<BetOddType, BetItem> BuildCurrentOddsMap<T>(SportType sportType, List<IBet<T>> betsForCompetition, bool onlyCurrentOdds, Func<float, float, bool> conditionToAdd) {
            var odds = BetHelper.SportTypeWithOdds[sportType];
            betsForCompetition = betsForCompetition.OrderByDescending(b => b.ID).ToList();
            var result = new Dictionary<BetOddType, BetItem>();
            var currentFoundBrokerType = new List<BrokerType>();
            foreach (var bet in betsForCompetition) {
                if (onlyCurrentOdds) {
                    if (currentFoundBrokerType.Contains(bet.BrokerID)) {
                        continue;
                    }
                    currentFoundBrokerType.Add(bet.BrokerID);
                }
                foreach (var betOddType in odds) {
                    var betItemCreated = BetMappingHelper<T>.OddsGetterMap[betOddType](bet);
                    BetItem betItemInMap;
                    if (!result.TryGetValue(betOddType, out betItemInMap)) {
                        betItemInMap = betItemCreated;
                        result[betOddType] = betItemInMap;
                    }
                    if (conditionToAdd(betItemInMap.Odd, betItemCreated.Odd)) {
                        result[betOddType] = betItemCreated;
                    }
                }
            }
            return result;
        } 
    }
}