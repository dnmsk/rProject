﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class CompetitionProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitionProvider).FullName);
        
        public CompetitionProvider() : base(_logger) {}

        public CompetitionParsedTransport GetCompetition(LanguageType language, SportType sportType, List<string> nameOrigin, CompetitionParsed competitionToSave, GatherBehaviorMode algoMode) {
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
                    return GetCompetitionUnique(language, sportType, genderDetected, nameOrigin, competitionToSave, algoMode);
                }
                return new CompetitionParsedTransport {
                    Name = competition.Name,
                    GenderType = genderDetected,
                    SportType = sportType,
                    LanguageType = language,
                    UniqueID = competition.CompetitionuniqueID
                };
            }, null);
        }

        private CompetitionParsedTransport GetCompetitionUnique(LanguageType language, SportType sportType, GenderType genderDetected, List<string> nameOrigin, CompetitionParsed competitionToSave, GatherBehaviorMode algoMode) {
            var nameOriginShort = CompetitionHelper.GetShortCompetitionName(nameOrigin);
            var competitionUniqueAdvanced = CompetitionUniqueAdvanced.DataSource
                                         .WhereEquals(CompetitionUniqueAdvanced.Fields.Gendertype, (short)genderDetected)
                                         .WhereEquals(CompetitionUniqueAdvanced.Fields.Languagetype, (short)language)
                                         .WhereEquals(CompetitionUniqueAdvanced.Fields.Sporttype, (short)sportType)
                                         .Where(QueryHelper.GetFilterByWordsForField(nameOriginShort, CompetitionUniqueAdvanced.Fields.Name))
                                         .First(CompetitionUniqueAdvanced.Fields.CompetitionuniqueID);
            if (competitionUniqueAdvanced == null) {
                CompetitionUnique uniqueID = null;
                if (algoMode.HasFlag(GatherBehaviorMode.CanDetectCompetition)) {
                    uniqueID = TryDetectCompetitionUniqueFromMatches(sportType, nameOrigin, competitionToSave);
                }
                if (uniqueID == null) {
                    if (!algoMode.HasFlag(GatherBehaviorMode.CreateIfNew)) {
                        return null;
                    }
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

            return new CompetitionParsedTransport {
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
            var maxdate = dates.Any() ? dates.Max().Date.AddDays(1) : DateTime.MinValue;
            if (minDate < DateTime.UtcNow.Date) {
                var resultModelEqualityComparer = new ResultTransportEqualityComparer();
                var mapResults = CompetitionResult.DataSource
                    .Join(JoinType.Inner, CompetitionItem.Fields.ID, CompetitionResult.Fields.CompetitionitemID, RetrieveMode.Retrieve)
                    .Join(JoinType.Inner, CompetitionResultAdvanced.Fields.CompetitionresultID, CompetitionResult.Fields.ID, RetrieveMode.Retrieve)
                    .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType)
                    .Where(new DaoFilterAnd(
                        new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, minDate),
                        new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.Less, maxdate)
                    ))
                    .Sort(CompetitionItem.Fields.CompetitionuniqueID)
                    .Sort(CompetitionItem.Fields.ID)
                    .Sort(CompetitionResult.Fields.ID)
                    .Sort(CompetitionResultAdvanced.Fields.ID)
                    .AsList(CompetitionItem.Fields.CompetitionuniqueID, CompetitionResult.Fields.ScoreID, CompetitionResultAdvanced.Fields.ScoreID)
                    .GroupBy(e => e.GetJoinedEntity<CompetitionItem>().CompetitionuniqueID)
                    .ToDictionary(g => g.Key, g=> g.GroupBy(gr => gr.ID).Select(gr => new ResultTransport {
                        ScoreID = gr.First().ScoreID,
                        SubScore = gr.Select(gra => gra.GetJoinedEntity<CompetitionResultAdvanced>().ScoreID).ToArray()
                    }).Distinct(resultModelEqualityComparer).ToList());
                var mapCoefficients = new Dictionary<int, float>();
                var hashResults = competitionToSave.Matches
                    .Where(m => m.Result != null)
                    .Select(m => new ResultTransport {
                        ScoreID = ScoreHelper.Instance.GenerateScoreID(m.Result.CompetitorResultOne, m.Result.CompetitorResultTwo),
                        SubScore = m.Result.SubResult.Any() 
                            ? m.Result.SubResult.Select(sr => ScoreHelper.Instance.GenerateScoreID(sr.CompetitorResultOne, sr.CompetitorResultTwo)).ToArray()
                            : new short[0]
                    })
                    .Distinct(resultModelEqualityComparer)
                    .ToArray();
                foreach (var suitableСompetitionItem in mapResults) {
                    List<ResultTransport> resultsForCompetition;
                    if (!mapResults.TryGetValue(suitableСompetitionItem.Key, out resultsForCompetition)) {
                        mapCoefficients[suitableСompetitionItem.Key] = 0;
                        continue;
                    }
                    var successMatches = resultsForCompetition.Count(res => hashResults.Any(h => resultModelEqualityComparer.Equals(h, res)));
                    mapCoefficients[suitableСompetitionItem.Key] = (float)successMatches / competitionToSave.Matches.Count;
                }
                var orderedCompetitionCoeffs = mapCoefficients.OrderByDescending(kv => kv.Value).ToList();
                if (orderedCompetitionCoeffs.Count == 0) {
                    return null;
                }
                if (orderedCompetitionCoeffs.First().Value >= .4 && 
                        (orderedCompetitionCoeffs.Count == 1 || 
                         orderedCompetitionCoeffs.Count > 1 && (orderedCompetitionCoeffs[0].Value - orderedCompetitionCoeffs[1].Value) > .3)) {
                    var key = orderedCompetitionCoeffs.First().Key;
                    _logger.Info("Для '{0}' поставляю CompetitionUniqueID {1} ({2}). K={3}", nameOrigin.StrJoin(". "), key, 
                        Competition.DataSource.WhereEquals(Competition.Fields.CompetitionuniqueID, key).Sort(Competition.Fields.ID).First().Name, orderedCompetitionCoeffs.First().Value);
                    return CompetitionUnique.DataSource.GetByKey(key);
                }
                return null;
            }
            return null;
        }

        private static string ListStringToName(List<string> names) {
            return names.StrJoin(". ");
        }

        public int GetCompetitionItem(CompetitorParsedTransport competitor1ParsedTransport, CompetitorParsedTransport competitor2ParsedTransport, CompetitionParsedTransport competitionParsedTransport, DateTime eventDateUtc, GatherBehaviorMode algoMode) {
            return InvokeSafeSingleCall(() => {
                var utcNow = DateTime.UtcNow;
                var source = CompetitionItem.DataSource
                        .Where(new DaoFilterOr(
                            new DaoFilterAnd(
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitor1ParsedTransport.UniqueID),
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitor2ParsedTransport.UniqueID)
                                ),
                            new DaoFilterAnd(
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitor1ParsedTransport.UniqueID),
                                new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitor2ParsedTransport.UniqueID)
                                )
                            ))
                        .WhereEquals(CompetitionItem.Fields.Sporttype, (short)competitionParsedTransport.SportType)
                        .WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionParsedTransport.UniqueID)
                        .Where(eventDateUtc > DateTime.MinValue 
                            ? new DaoFilterAnd(
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, eventDateUtc.AddDays(-1.5)),
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.LessOrEq, eventDateUtc.AddDays(1.5))
                            )
                            : new DaoFilterAnd(
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, utcNow.AddDays(-1.5)),
                                new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.LessOrEq, utcNow.AddDays(1.5))
                            )
                        );
                var competitionItem = source
                    .Sort(CompetitionItem.Fields.ID, SortDirection.Desc)
                    .First(CompetitionItem.Fields.ID, CompetitionItem.Fields.Dateeventutc);
                if (eventDateUtc > DateTime.MinValue) {
                    if (competitionItem != null && Math.Abs((competitionItem.Dateeventutc - eventDateUtc).TotalDays) < 2) {
                            competitionItem.Dateeventutc = eventDateUtc;
                            competitionItem.Save();
                    } else {
                        competitionItem = null;
                    }
                }
                if (competitionItem == null) {
                    if (!algoMode.HasFlag(GatherBehaviorMode.CreateIfNew)) {
                        return default(int);
                    }
                    competitionItem = new CompetitionItem {
                        SportType = competitionParsedTransport.SportType,
                        Datecreatedutc = utcNow,
                        Dateeventutc = eventDateUtc != DateTime.MinValue ? eventDateUtc : utcNow,
                        CompetitionuniqueID = competitionParsedTransport.UniqueID,
                        Competitoruniqueid1 = competitor1ParsedTransport.UniqueID,
                        Competitoruniqueid2 = competitor2ParsedTransport.UniqueID
                    };
                    competitionItem.Save();
                }
                return competitionItem.ID;
            }, default(int));
        }

        public List<CompetitionItemBetShortTransport> GetCompetitionItemsFutured(LanguageType languageType, SportType? sportType = null, int[] competitionUniqueIDs = null) {
            return InvokeSafe(() => {
                var competitionItemForDateQuery = CompetitionItem.DataSource
                    .Join(JoinType.Left, CompetitionResult.Fields.CompetitionitemID, CompetitionItem.Fields.ID, RetrieveMode.NotRetrieve)
                    .Join(JoinType.Left, CompetitionResultLive.Fields.CompetitionitemID, CompetitionItem.Fields.ID, RetrieveMode.NotRetrieve)
                    .WhereNull(CompetitionResult.Fields.ID)
                    .WhereNull(CompetitionResultLive.Fields.ID)
                    .Where(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, DateTime.UtcNow.Date)
                    .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Asc);
                if (sportType.HasValue && sportType != SportType.Unknown) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType);
                }
                if (competitionUniqueIDs != null) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .WhereIn(CompetitionItem.Fields.CompetitionuniqueID, competitionUniqueIDs);
                }
                var shortModels = GetCompetitionItemShortModel(languageType, competitionItemForDateQuery);
                return GetCompetitiontItemBetModel(shortModels, true, GetBetMap);
            }, null);
        }

        public List<CompetitionItemBetShortTransport> GetCompetitionItemsHistory(LanguageType languageType, DateTime fromDate, DateTime toDate, SportType? sportType = null, int[] competitionUniqueIDs = null) {
            return InvokeSafe(() => {
                var competitionItemForDateQuery = CompetitionItem.DataSource
                    .Join(JoinType.Left, CompetitionResult.Fields.CompetitionitemID, CompetitionItem.Fields.ID, RetrieveMode.NotRetrieve)
                    .WhereNotNull(CompetitionResult.Fields.ID)
                    .Where(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, fromDate)
                    .Where(CompetitionItem.Fields.Dateeventutc, Oper.Less, toDate)
                    .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Asc);
                if (sportType.HasValue && sportType != SportType.Unknown) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType);
                }
                if (competitionUniqueIDs != null) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .WhereIn(CompetitionItem.Fields.CompetitionuniqueID, competitionUniqueIDs);
                }
                var shortModels = GetCompetitionItemShortModel(languageType, competitionItemForDateQuery);
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

        public List<CompetitionItemBetShortTransport> GetCompetitionItemsLive(LanguageType languageType, SportType? sportType = null, int[] competitionUniqueIDs = null) {
            return InvokeSafe(() => {
                var competitionItemForDateQuery = CompetitionItem.DataSource
                    .Join(JoinType.Left, CompetitionResult.Fields.CompetitionitemID, CompetitionItem.Fields.ID, RetrieveMode.NotRetrieve)
                    .Join(JoinType.Left, CompetitionResultLive.Fields.CompetitionitemID, CompetitionItem.Fields.ID, RetrieveMode.NotRetrieve)
                    .WhereNull(CompetitionResult.Fields.ID)
                    .WhereNotNull(CompetitionResultLive.Fields.ID)
                    .Where(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, DateTime.UtcNow.Date)
                    .Where(CompetitionResultLive.Fields.Datecreatedutc, Oper.GreaterOrEq, DateTime.UtcNow.AddHours(-2));
                if (sportType.HasValue && sportType != SportType.Unknown) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType);
                }
                if (competitionUniqueIDs != null) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .WhereIn(CompetitionItem.Fields.CompetitionuniqueID, competitionUniqueIDs);
                }
                var competitionItemIDs = competitionItemForDateQuery
                    .GroupBy(CompetitionItem.Fields.ID)
                    .AsGroups()
                    .Select(g => (int) g[CompetitionItem.Fields.ID]);

                var shortModels = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereIn(CompetitionItem.Fields.ID, competitionItemIDs));
                return GetCompetitiontItemBetModel(shortModels, true, GetLiveBetMap);
            }, null);
        }
        
        public CompetitionItemBetShortTransport GetCompetitionItemRegularBet(LanguageType languageType, int competitionItemID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereIn(CompetitionItem.Fields.ID, new[] { competitionItemID }));
                return GetCompetitiontItemBetModel(competition, true, GetBetMap).FirstOrDefault();
            }, null);
        }

        public List<CompetitionItemBetShortTransport> GetCompetitionItemLiveBetForCompetition(LanguageType languageType, int competitionID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionID));
                return GetCompetitiontItemBetModel(competition, false, GetLiveBetMap);
            }, null);
        }

        public List<CompetitionItemBetShortTransport> GetCompetitionItemsRegularBetForCompetitor(LanguageType languageType, int competitorID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource
                    .Where(new DaoFilterOr(
                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitorID),
                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitorID)
                    )));
                return GetCompetitiontItemBetModel(competition, true, GetBetMap);
            }, null);
        }
        
        private static List<CompetitionItemBetShortTransport> GetCompetitiontItemBetModel<T>(List<CompetitionItemShortTransport> competitionItemShortModels, bool addEmptyBets, Func<IEnumerable<int>, Dictionary<int, List<IBet<T>>>> getBetMap) {
            var betGrouped = getBetMap(competitionItemShortModels.Select(c => c.CompetitionID));
            var result = new List<CompetitionItemBetShortTransport>();
            foreach (var competitionItem in competitionItemShortModels) {
                var betsForCompetition = betGrouped.TryGetValueOrDefaultStruct(competitionItem.CompetitionID);
                if (betsForCompetition == null) {
                    if (addEmptyBets) {
                        result.Add(new CompetitionItemBetShortTransport(competitionItem));
                    }
                    continue;
                }
                var itemModel = new CompetitionItemBetShortTransport(competitionItem) {
                    CurrentBets = BuildCurrentOddsMap(competitionItem.SportType, betsForCompetition, true, (cur, next) => cur < next),
                    HistoryMaxBets = BuildCurrentOddsMap(competitionItem.SportType, betsForCompetition, false, (cur, next) => cur < next),
                    HistoryMinBets = BuildCurrentOddsMap(competitionItem.SportType, betsForCompetition, false, (cur, next) => cur > next && next != default(float)),
                };
                result.Add(itemModel);
            }
            return result;
        }

        private static List<CompetitionItemShortTransport> GetCompetitionItemShortModel(LanguageType languageType, DbDataSource<CompetitionItem, int> competitionItemQuery) {
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
                .Select(ci => new CompetitionItemShortTransport {
                    CompetitionID = ci.ID,
                    DateUtc = ci.Dateeventutc,
                    SportType = ci.SportType,
                    Competitor1 = ExtractNameFromCompetitor(ci.Competitoruniqueid1, competitorNameMap),
                    Competitor2 = ExtractNameFromCompetitor(ci.Competitoruniqueid2, competitorNameMap),
                    Competition = ExtractNameFromCompetition(ci.CompetitionuniqueID, competitionNameMap)
                })
                .ToList();
        }

        private static CompetitorTransport ExtractNameFromCompetitor(int competitorID, Dictionary<int, Competitor> competitorNameMap) {
            Competitor competitor;
            if (!competitorNameMap.TryGetValue(competitorID, out competitor)) {
                competitor = Competitor.DataSource
                .WhereEquals(Competitor.Fields.CompetitoruniqueID, competitorID)
                .First();
            }
            return new CompetitorTransport {
                ID = competitorID,
                Name = competitor.NameFull ?? competitor.NameShort
            };
        }

        private static CompetitionTransport ExtractNameFromCompetition(int competitionID, Dictionary<int, CompetitionUniqueAdvanced> competitionNameMap) {
            CompetitionUniqueAdvanced competition;
            if (!competitionNameMap.TryGetValue(competitionID, out competition)){
                competition = CompetitionUniqueAdvanced.DataSource
                    .WhereEquals(CompetitionUniqueAdvanced.Fields.CompetitionuniqueID, competitionID)
                    .First();
                competition = competition ?? new CompetitionUniqueAdvanced {
                    Name = "DELETED"
                };
            }
            return new CompetitionTransport {
                ID = competitionID,
                Name = competition.Name
            };
        }

        private static Dictionary<BetOddType, BetItemTransport> BuildCurrentOddsMap<T>(SportType sportType, List<IBet<T>> betsForCompetition, bool onlyCurrentOdds, Func<float, float, bool> conditionToAdd) {
            var odds = BetHelper.SportTypeWithOdds[sportType];
            betsForCompetition = betsForCompetition.OrderByDescending(b => b.ID).ToList();
            var result = new Dictionary<BetOddType, BetItemTransport>();
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
                    BetItemTransport betItemTransportInMap;
                    if (!result.TryGetValue(betOddType, out betItemTransportInMap)) {
                        betItemTransportInMap = betItemCreated;
                        result[betOddType] = betItemTransportInMap;
                    }
                    if (conditionToAdd(betItemTransportInMap.Odd, betItemCreated.Odd)) {
                        result[betOddType] = betItemCreated;
                    }
                }
            }
            return result;
        } 
    }
}