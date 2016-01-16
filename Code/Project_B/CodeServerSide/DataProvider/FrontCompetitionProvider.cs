﻿using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Entity.Helper;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class FrontCompetitionProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (FrontCompetitionProvider).FullName);

        public FrontCompetitionProvider() : base(_logger) {}

        public List<CompetitionTransport> GetCompetitionItemsFuturedNew(LanguageType languageType, DateTime? competitionItemDate, BrokerType[] brokerTypesToRetreive, BrokerType[] brokerTypesToDisplay, SportType? sportType = null, int[] competitionUniqueIDs = null) {
            return InvokeSafe(() => {
                var competitionInfo = CompetitionBetRoiHelper.GetDataForNow(float.MinValue, sportType ?? SportType.Unknown, competitionUniqueIDs, brokerTypesToRetreive);
                var competitionItemForDateQuery = CompetitionItem.DataSource
                    .WhereIn(CompetitionItem.Fields.ID, competitionInfo.Select(cinfo => cinfo.ID))
                    .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Asc);
                if (competitionItemDate.HasValue) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .Where(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, competitionItemDate.Value)
                        .Where(CompetitionItem.Fields.Dateeventutc, Oper.Less, competitionItemDate.Value.AddDays(1));
                }
                var competitionTransports = GetCompetitionItemShortModel(languageType, competitionItemForDateQuery);
                PostProcessCompetition(languageType, competitionUniqueIDs, competitionTransports);
                BuildCompetitiontItemFullModel(competitionTransports, ci => {
                    return BetHelper.GetBetMapNew(competitionInfo.SelectMany(cinfo => cinfo.BetIDs).Distinct());
                }, ProjectProvider.Instance.ResultProvider.GetResultForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competitionTransports);
                return competitionTransports;
            }, new List<CompetitionTransport>());
        }

        public List<CompetitionTransport> GetCompetitionItemsFuturedProfitable(LanguageType languageType, BrokerType[] brokerTypesToRetreive, BrokerType[] brokerTypesToDisplay, SportType? sportType = null) {
            return InvokeSafe(() => {
                int[] competitionItemIDs;
                var minProfitable = 0f;
                while ((competitionItemIDs = CompetitionBetRoiHelper.GetDataForNow(minProfitable, sportType ?? SportType.Unknown, null, brokerTypesToRetreive)
                    .Select(vw => vw.ID).ToArray()).Length == 0) {
                    minProfitable -= 0.5f;
                }
                var competitionItemForDateQuery = CompetitionItem.DataSource
                    .Join(JoinType.Left, CompetitionResult.Fields.CompetitionitemID, CompetitionItem.Fields.ID, RetrieveMode.NotRetrieve)
                    .Join(JoinType.Left, CompetitionResultLive.Fields.CompetitionitemID, CompetitionItem.Fields.ID, RetrieveMode.NotRetrieve)
                    .WhereNull(CompetitionResult.Fields.ID)
                    .WhereNull(CompetitionResultLive.Fields.ID)
                    .WhereIn(CompetitionItem.Fields.ID, competitionItemIDs)
                    .Where(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, DateTime.UtcNow)
                    .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Asc);
                if (sportType.HasValue && sportType != SportType.Unknown) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType);
                }
                var competitionTransports = GetCompetitionItemShortModel(languageType, competitionItemForDateQuery);
                BuildCompetitiontItemFullModel(competitionTransports, ci => BetHelper.GetBetMap(ci, brokerTypesToRetreive), ProjectProvider.Instance.ResultProvider.GetResultForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competitionTransports);
                return competitionTransports;
            }, new List<CompetitionTransport>());
        }

        private static void ProcessBrokerType(BrokerType[] brokerTypesToRetreive, IEnumerable<CompetitionTransport> competitions) {
            if (brokerTypesToRetreive == null || !brokerTypesToRetreive.Any()) {
                return;
            }
            competitions.Where(c => c.CompetitionItems != null && c.CompetitionItems.Any()).Each(c => {
                c.CompetitionItems.Where(ci => ci.CurrentBets != null && ci.CurrentBets.Any()).Each(ci => ci.CurrentBets.Each(b => {
                    if (!brokerTypesToRetreive.Contains(b.Value.BrokerType)) {
                        b.Value.BrokerType = BrokerType.Default;
                    }
                }));
            });
        }

        public List<CompetitionTransport> GetCompetitionItemsHistory(LanguageType languageType, BrokerType[] brokerTypesToRetreive, BrokerType[] brokerTypesToDisplay, DateTime fromDate, DateTime toDate, SportType? sportType = null, int[] competitionUniqueIDs = null) {
            return InvokeSafe(() => {
                var competitionInfo = CompetitionBetRoiHelper.GetDataForDate(fromDate, toDate, sportType ?? SportType.Unknown, competitionUniqueIDs, brokerTypesToRetreive);
                var competitionItemForDateQuery = CompetitionItem.DataSource
                    .WhereIn(CompetitionItem.Fields.ID, competitionInfo.Select(cinfo => cinfo.ID))
                    .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Desc);
                var competitionTransports = GetCompetitionItemShortModel(languageType, competitionItemForDateQuery);
                PostProcessCompetition(languageType, competitionUniqueIDs, competitionTransports);
                BuildCompetitiontItemFullModel(competitionTransports, ci => BetHelper.GetBetMapNew(competitionInfo.SelectMany(cinfo => cinfo.BetIDs).Distinct()), ProjectProvider.Instance.ResultProvider.GetResultForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competitionTransports);
                return competitionTransports;
            }, new List<CompetitionTransport>());
        }

        public List<CompetitionTransport> GetCompetitionItemsLive(LanguageType languageType, bool allCompetitionItems, BrokerType[] brokerTypesToRetreive, BrokerType[] brokerTypesToDisplay, SportType? sportType = null, int[] competitionUniqueIDs = null) {
            return InvokeSafe(() => {
                var competitionItemForDateQuery = CompetitionItem.DataSource
                    .Join(JoinType.Left, CompetitionResult.Fields.CompetitionitemID, CompetitionItem.Fields.ID, RetrieveMode.NotRetrieve)
                    .Join(JoinType.Left, CompetitionResultLive.Fields.CompetitionitemID, CompetitionItem.Fields.ID, RetrieveMode.NotRetrieve)
                    .WhereNull(CompetitionResult.Fields.ID)
                    .WhereNotNull(CompetitionResultLive.Fields.ID)
                    .Where(CompetitionResultLive.Fields.Datecreatedutc, Oper.GreaterOrEq, DateTime.UtcNow.AddHours(-2));
                if (sportType.HasValue && sportType != SportType.Unknown) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType);
                }
                if (competitionUniqueIDs != null) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .WhereIn(CompetitionItem.Fields.CompetitionuniqueID, competitionUniqueIDs);
                }
                if (!allCompetitionItems) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .Join(JoinType.Inner, BetLive.Fields.CompetitionitemID, CompetitionItem.Fields.ID, RetrieveMode.NotRetrieve);
                }
                var competitionItemIDs = competitionItemForDateQuery
                    .GroupBy(CompetitionItem.Fields.ID)
                    .AsGroups()
                    .Select(g => (int)g[CompetitionItem.Fields.ID]);

                var competitionTransports = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereIn(CompetitionItem.Fields.ID, competitionItemIDs));
                PostProcessCompetition(languageType, competitionUniqueIDs, competitionTransports);
                BuildCompetitiontItemFullModel(competitionTransports, ci => BetHelper.GetLiveBetMap(ci, brokerTypesToRetreive), ProjectProvider.Instance.ResultProvider.GetResultLiveForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competitionTransports);
                return competitionTransports;
            }, new List<CompetitionTransport>());
        }

        private static void PostProcessCompetition(LanguageType languageType, int[] competitionUniqueIDs,
            List<CompetitionTransport> competitionTransports) {
            if (competitionUniqueIDs != null && !competitionTransports.Any()) {
                foreach (var competition in GetCompetitionNameMap(languageType, competitionUniqueIDs).Values) {
                    competitionTransports.Add(new CompetitionTransport {
                        ID = competition.CompetitionuniqueID,
                        Name = competition.Name,
                        SportType = competition.SportType,
                        Language = languageType
                    });
                }
            }
        }

        public CompetitionAdvancedTransport GetCompetitionItemRegularBet(LanguageType languageType, BrokerType[] brokerTypesToRetreive, BrokerType[] brokerTypesToDisplay, int competitionItemID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereEquals(CompetitionItem.Fields.ID, competitionItemID));
                BuildCompetitiontItemFullModel(competition, ci => BetHelper.GetBetMap(ci, brokerTypesToRetreive), ProjectProvider.Instance.ResultProvider.GetResultForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competition);
                return new CompetitionAdvancedTransport {
                    CompetitionTransport = competition.FirstOrDefault(),
                    HaveLiveData = CompetitionResultLive.DataSource.WhereEquals(CompetitionResultLive.Fields.CompetitionitemID, competitionItemID).IsExists() ||
                                   BetLive.DataSource.WhereEquals(BetLive.Fields.CompetitionitemID, competitionItemID).IsExists()
                };
            }, new CompetitionAdvancedTransport());
        }

        public CompetitionAdvancedTransport GetCompetitionItemLiveBetForCompetition(LanguageType languageType, BrokerType[] brokerTypesToRetreive, BrokerType[] brokerTypesToDisplay, int competitionID) {
            return InvokeSafe(() => {
                var competitionTransports = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereEquals(CompetitionItem.Fields.ID, competitionID));
                BuildCompetitiontItemFullModel(competitionTransports, ci => BetHelper.GetLiveBetMap(ci, brokerTypesToRetreive), ProjectProvider.Instance.ResultProvider.GetResultLiveForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competitionTransports);
                return new CompetitionAdvancedTransport {
                    CompetitionTransport = competitionTransports.FirstOrDefault(),
                    HaveLiveData = true
                };
            }, new CompetitionAdvancedTransport());
        }

        public List<CompetitionTransport> GetCompetitionItemsRegularBetForCompetitor(LanguageType languageType, BrokerType[] brokerTypesToRetreive, BrokerType[] brokerTypesToDisplay, DateTime fromDate, DateTime toDate, int competitorID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource
                    .Where(new DaoFilterOr(
                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitorID),
                        new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitorID)
                    ))
                    .Where(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, fromDate)
                    .Where(CompetitionItem.Fields.Dateeventutc, Oper.Less, toDate != DateTime.MaxValue ? toDate.AddDays(1) : toDate)

                    .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Desc));
                BuildCompetitiontItemFullModel(competition, ci => BetHelper.GetBetMap(ci, brokerTypesToRetreive), ProjectProvider.Instance.ResultProvider.GetResultForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competition);
                return competition;
            }, new List<CompetitionTransport>());
        }

        public Dictionary<DateTime, List<Dictionary<BetOddType, BetItemTransport>>> GetRowDataForGraphCompetition(BrokerType[] brokerTypesToRetreive, SportType sportType, int competitionItemID) {
            return InvokeSafe(() => {
                const int RESOLUTION = 10;
                return BuildOddsByDateByBroker(ints => BetHelper.GetBetMap(ints, brokerTypesToRetreive), sportType, competitionItemID, RESOLUTION);
            }, null);
        }

        public Dictionary<DateTime, List<Dictionary<BetOddType, BetItemTransport>>> GetRowDataForGraphCompetitionLive(BrokerType[] brokerTypesToRetreive, SportType sportType, int competitionItemID) {
            return InvokeSafe(() => {
                const int RESOLUTION = 1;
                return BuildOddsByDateByBroker(ints => BetHelper.GetLiveBetMap(ints, brokerTypesToRetreive), sportType, competitionItemID, RESOLUTION);
            }, null);
        }

        private static Dictionary<DateTime, List<Dictionary<BetOddType, BetItemTransport>>> BuildOddsByDateByBroker<T>(Func<int[], Dictionary<int, List<IBet<T>>>> getBetMap, SportType sportType, int competitionItemID, int resolution) {
            var betMap = getBetMap(new[] { competitionItemID });
            if (betMap == null || !betMap.Any()) {
                return null;
            }

            var mapBetsByDates = new Dictionary<DateTime, List<IBet<T>>>();
            var mapByBrokers = new Dictionary<BrokerType, List<KeyValuePair<DateTime, IBet<T>>>>();
            foreach (var ibet in betMap.Values.First()) {
                var ibetDate = ibet.Datecreatedutc.Round(DateRoundType.Minute, resolution);
                List<KeyValuePair<DateTime, IBet<T>>> ibetsForBroker;
                if (!mapByBrokers.TryGetValue(ibet.BrokerID, out ibetsForBroker)) {
                    ibetsForBroker = new List<KeyValuePair<DateTime, IBet<T>>>();
                    mapByBrokers[ibet.BrokerID] = ibetsForBroker;
                }
                ibetsForBroker.Add(new KeyValuePair<DateTime, IBet<T>>(ibetDate, ibet));
                List<IBet<T>> listBets;
                if (!mapBetsByDates.TryGetValue(ibetDate, out listBets)) {
                    listBets = new List<IBet<T>>();
                    mapBetsByDates[ibetDate] = listBets;
                }
                listBets.Add(ibet);
            }
            var totalDeltaMinutes = (mapBetsByDates.Min(mb => mb.Key) - mapBetsByDates.Max(mb => mb.Key)).TotalMinutes;
            //minimum interpolation area for each broker
            var minDateForBroker = mapByBrokers
                .Select(byBroker => {
                    var keyValuePairs = byBroker.Value.OrderBy(kv => kv.Key).ToArray();
                    var firstBet = keyValuePairs.First();
                    var lastBet = keyValuePairs.Last();
                    var localDeltaMinutes = (firstBet.Key == lastBet.Key ? totalDeltaMinutes : (firstBet.Key - lastBet.Key).TotalMinutes) / 16;
                    var newLastiBetDate = firstBet.Key.AddMinutes(localDeltaMinutes).Round(DateRoundType.Minute, resolution);
                    List<IBet<T>> listBets;
                    if (!mapBetsByDates.TryGetValue(newLastiBetDate, out listBets)) {
                        listBets = new List<IBet<T>>();
                        mapBetsByDates[newLastiBetDate] = listBets;
                    }
                    if (listBets.All(iBet => iBet.BrokerID != firstBet.Value.BrokerID)) {
                        listBets.Add(firstBet.Value);
                        byBroker.Value.Add(new KeyValuePair<DateTime, IBet<T>>(newLastiBetDate, firstBet.Value));
                    }
                    return byBroker;
                })
                .ToDictionary(byBroker => byBroker.Key, byBroker => byBroker.Value.Min(ibet => ibet.Key));

            var orderByDescending = mapBetsByDates.OrderByDescending(mb => mb.Key);
            var previousList = orderByDescending.First();
            foreach (var ibetsForTime in orderByDescending.Skip(1)) {
                mapBetsByDates[ibetsForTime.Key.AddSeconds(1)] = previousList.Value;
                ibetsForTime.Value.AddRange(previousList.Value
                    .Where(prev => minDateForBroker[prev.BrokerID] < ibetsForTime.Key)
                    .Where(prev => ibetsForTime.Value.All(i => i.BrokerID != prev.BrokerID)));
                previousList = ibetsForTime;
            }

            var betOddTypes = BetHelper.SportTypeWithOdds[sportType];
            var oddsByDateByBroker = new Dictionary<DateTime, List<Dictionary<BetOddType, BetItemTransport>>>();
            foreach (var mapBets in mapBetsByDates) {
                var forDate = new List<Dictionary<BetOddType, BetItemTransport>>();
                foreach (var ibet in mapBets.Value) {
                    var betOddsMap = new Dictionary<BetOddType, BetItemTransport>();
                    foreach (var betOddType in betOddTypes) {
                        betOddsMap[betOddType] = BetMappingHelper<T>.OddsGetterMap[betOddType](ibet);
                    }
                    forDate.Add(betOddsMap);
                }
                oddsByDateByBroker[mapBets.Key] = forDate;
            }
            return oddsByDateByBroker;
        }

        private static void BuildCompetitiontItemFullModel<T>(List<CompetitionTransport> competitions, Func<int[], Dictionary<int, List<IBet<T>>>> getBetMap, Func<int[], Dictionary<int, ResultTransport>> getResultMap) {
            var competitionItemIDs = competitions.SelectMany(c => c.CompetitionItems.Select(ci => ci.CompetitionItemID)).Distinct().ToArray();
            var betGrouped = getBetMap(competitionItemIDs);
            var resultGrouped = getResultMap(competitionItemIDs);
            foreach (var competitionTransport in competitions) {
                foreach (var competitionItem in competitionTransport.CompetitionItems) {
                    List<IBet<T>> betsForCompetition;
                    if (betGrouped.TryGetValue(competitionItem.CompetitionItemID, out betsForCompetition)) {
                        competitionItem.CurrentBets = BuildCurrentOddsMap(competitionTransport.SportType, betsForCompetition, true, (cur, next) => cur < next);
                        competitionItem.HistoryMaxBets = BuildCurrentOddsMap(competitionTransport.SportType, betsForCompetition, false, (cur, next) => cur < next);
                        competitionItem.HistoryMinBets = BuildCurrentOddsMap(competitionTransport.SportType, betsForCompetition, false, (cur, next) => cur > next && next != default(float));
                    }
                    ResultTransport resultTransport;
                    if (resultGrouped.TryGetValue(competitionItem.CompetitionItemID, out resultTransport)) {
                        competitionItem.Result = resultTransport;
                    }
                }
            }
        }

        private static List<CompetitionTransport> GetCompetitionItemShortModel(LanguageType languageType, DbDataSource<CompetitionItem, int> competitionItemQuery) {
            var competitionItemForDate = competitionItemQuery
               .WhereNotEquals(CompetitionItem.Fields.Dateeventutc, DateTime.MinValue)
               .Sort(CompetitionItem.Fields.Sporttype)
               .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Asc)
               .GroupBy(
                    CompetitionItem.Fields.ID,
                    CompetitionItem.Fields.Dateeventutc,
                    CompetitionItem.Fields.Competitoruniqueid1,
                    CompetitionItem.Fields.Competitoruniqueid2,
                    CompetitionItem.Fields.Sporttype,
                    CompetitionItem.Fields.CompetitionuniqueID
                )
                .AsGroups();
            var competitionNameMap = GetCompetitionNameMap(languageType, competitionItemForDate.Select(c => (int)c[CompetitionItem.Fields.CompetitionuniqueID]));
            var competitorNameMap = Competitor.DataSource
                .WhereIn(Competitor.Fields.CompetitoruniqueID, competitionItemForDate.Select(c => (int)c[CompetitionItem.Fields.Competitoruniqueid1])
                                                                                     .Union(competitionItemForDate.Select(c => (int)c[CompetitionItem.Fields.Competitoruniqueid2]))
                                                                                     .Distinct())
                .Sort(Competitor.Fields.ID, SortDirection.Asc)
                .AsList(
                    Competitor.Fields.Languagetype,
                    Competitor.Fields.CompetitoruniqueID,
                    Competitor.Fields.NameFull
                )
                .GroupBy(e => e.CompetitoruniqueID)
                .ToDictionary(e => e.Key, e => {
                    var withCurrentLanguage = e.Where(i => i.Languagetype == languageType);
                    var competitors = withCurrentLanguage as Competitor[] ?? withCurrentLanguage.ToArray();
                    return competitors.Any() ? competitors.First() : e.First();
                });

            var competitionsDict = new Dictionary<int, CompetitionTransport>();
            foreach (var competitionItem in competitionItemForDate) {
                CompetitionTransport competitionTransport;
                var competitionUniqueID = (int)competitionItem[CompetitionItem.Fields.CompetitionuniqueID];
                if (!competitionsDict.TryGetValue(competitionUniqueID, out competitionTransport)) {
                    competitionTransport = ExtractNameFromCompetition(languageType, competitionUniqueID, competitionNameMap);
                    competitionsDict[competitionTransport.ID] = competitionTransport;
                }
                competitionTransport.CompetitionItems.Add(new CompetitionItemBetShortTransport {
                    CompetitionItemID = (int)competitionItem[CompetitionItem.Fields.ID],
                    DateUtc = (DateTime)competitionItem[CompetitionItem.Fields.Dateeventutc],
                    Competitor1 = ExtractNameFromCompetitor((int)competitionItem[CompetitionItem.Fields.Competitoruniqueid1], competitorNameMap),
                    Competitor2 = ExtractNameFromCompetitor((int)competitionItem[CompetitionItem.Fields.Competitoruniqueid2], competitorNameMap),
                });
            }
            return competitionsDict.Values.ToList();
        }

        private static Dictionary<int, Competition> GetCompetitionNameMap(LanguageType languageType, IEnumerable<int> competitionuniqueID) {
            return Competition.DataSource
                                            .WhereIn(Competition.Fields.CompetitionuniqueID, competitionuniqueID)
                                            .Sort(Competition.Fields.ID, SortDirection.Asc)
                                            .AsList(
                                                Competition.Fields.CompetitionuniqueID,
                                                Competition.Fields.Name,
                                                Competition.Fields.Languagetype,
                                                Competition.Fields.Sporttype
                                            )
                                            .GroupBy(e => e.CompetitionuniqueID)
                                            .ToDictionary(e => e.Key, e => {
                                                var withCurrentLanguage = e.Where(i => i.Languagetype == languageType);
                                                var competitionUniqueAdvanceds = withCurrentLanguage as Competition[] ?? withCurrentLanguage.ToArray();
                                                return competitionUniqueAdvanceds.Any() ? competitionUniqueAdvanceds.First() : e.First();
                                            });
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
                Name = competitor.Name
            };
        }

        private static CompetitionTransport ExtractNameFromCompetition(LanguageType languageType, int competitionID, Dictionary<int, Competition> competitionNameMap) {
            Competition competition;
            if (!competitionNameMap.TryGetValue(competitionID, out competition)) {
                competition = Competition.DataSource
                    .WhereEquals(Competition.Fields.CompetitionuniqueID, competitionID)
                    .First();
                competition = competition ?? new Competition {
                    Name = "DELETED"
                };
            }
            return new CompetitionTransport {
                ID = competitionID,
                Name = competition.Name,
                SportType = competition.SportType,
                Language = languageType
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