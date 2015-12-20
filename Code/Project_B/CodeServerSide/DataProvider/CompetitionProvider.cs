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
                if (orderedCompetitionCoeffs.First().Value > .5 && 
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

        public int GetCompetitionItemID(CompetitorParsedTransport competitor1ParsedTransport, CompetitorParsedTransport competitor2ParsedTransport, CompetitionParsedTransport competitionParsedTransport, DateTime eventDateUtc, GatherBehaviorMode algoMode) {
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
                    .First(CompetitionItem.Fields.ID, CompetitionItem.Fields.Dateeventutc, CompetitionItem.Fields.Competitoruniqueid1, CompetitionItem.Fields.Competitoruniqueid2);
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
                return competitionItem.Competitoruniqueid1 == competitor1ParsedTransport.UniqueID && 
                       competitionItem.Competitoruniqueid2 == competitor2ParsedTransport.UniqueID 
                        ?  competitionItem.ID 
                        : -competitionItem.ID;
            }, default(int));
        }

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
                BuildCompetitiontItemFullModel(competitionTransports, ci => GetBetMapNew(competitionInfo.SelectMany(cinfo => cinfo.BetIDs).Distinct()), ProjectProvider.Instance.ResultProvider.GetResultForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competitionTransports);
                return competitionTransports;
            }, new List<CompetitionTransport>());
        }

        public List<CompetitionTransport> GetCompetitionItemsFuturedProfitable(LanguageType languageType, BrokerType[] brokerTypesToRetreive, BrokerType[] brokerTypesToDisplay, SportType? sportType = null) {
            return InvokeSafe(() => {
                int[] competitionItemIDs;
                var minProfitable = 0f;
                while((competitionItemIDs = CompetitionBetRoiHelper.GetDataForNow(minProfitable, sportType ?? SportType.Unknown, null, brokerTypesToRetreive)
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
                BuildCompetitiontItemFullModel(competitionTransports, ci => GetBetMap(ci, brokerTypesToRetreive), ProjectProvider.Instance.ResultProvider.GetResultForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competitionTransports);
                return competitionTransports;
            }, new List<CompetitionTransport>());
        }

        private static void ProcessBrokerType(BrokerType[] brokerTypesToRetreive, IEnumerable<CompetitionTransport> competitions) {
            if (brokerTypesToRetreive == null || !brokerTypesToRetreive.Any()) {
                return;
            }
            competitions.Where(c=> c.CompetitionItems != null && c.CompetitionItems.Any()).Each(c => {
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
                BuildCompetitiontItemFullModel(competitionTransports, ci => GetBetMapNew(competitionInfo.SelectMany(cinfo => cinfo.BetIDs).Distinct()), ProjectProvider.Instance.ResultProvider.GetResultForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competitionTransports);
                return competitionTransports;
            }, new List<CompetitionTransport>());
        }

        private Dictionary<int, List<IBet<int>>> GetBetMap(IEnumerable<int> competitionItemIDs, BrokerType[] brokerTypesToRetreive) {
            var bets = Bet.DataSource
                .Join(JoinType.Left, BetAdvanced.Fields.ID, Bet.Fields.ID, RetrieveMode.Retrieve)
                .WhereIn(Bet.Fields.CompetitionitemID, competitionItemIDs);
            if (brokerTypesToRetreive != null && brokerTypesToRetreive.Any()) {
                bets = bets.WhereIn(Bet.Fields.BrokerID, brokerTypesToRetreive);
            }
            return bets
                .AsList()
                .GroupBy(e => e.CompetitionitemID)
                .ToDictionary(e => e.Key, e => e.Select(t => (IBet<int>) t)
                .OrderByDescending(t => t.ID)
                .ToList());
        }
        private Dictionary<int, List<IBet<int>>> GetBetMapNew(IEnumerable<int> betIDs) {
            var bets = Bet.DataSource
                .Join(JoinType.Left, BetAdvanced.Fields.ID, Bet.Fields.ID, RetrieveMode.Retrieve)
                .WhereIn(Bet.Fields.ID, betIDs);
            return bets
                .AsList()
                .GroupBy(e => e.CompetitionitemID)
                .ToDictionary(e => e.Key, e => e.Select(t => (IBet<int>) t)
                .OrderByDescending(t => t.ID)
                .ToList());
        }

        private Dictionary<int, List<IBet<long>>> GetLiveBetMap(IEnumerable<int> ints, BrokerType[] brokerTypesToRetreive) {
            var bets = BetLive.DataSource
                .Join(JoinType.Left, BetLiveAdvanced.Fields.ID, BetLive.Fields.ID, RetrieveMode.Retrieve)
                .WhereIn(BetLive.Fields.CompetitionitemID, ints);
            if (brokerTypesToRetreive != null && brokerTypesToRetreive.Any()) {
                bets = bets.WhereIn(BetLive.Fields.BrokerID, brokerTypesToRetreive);
            }
            return bets
                .AsList()
                .GroupBy(e => e.CompetitionitemID)
                .ToDictionary(e => e.Key, e => e.Select(t => (IBet<long>)t)
                .ToList());
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
                    .Select(g => (int) g[CompetitionItem.Fields.ID]);

                var competitionTransports = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereIn(CompetitionItem.Fields.ID, competitionItemIDs));
                PostProcessCompetition(languageType, competitionUniqueIDs, competitionTransports);
                BuildCompetitiontItemFullModel(competitionTransports, ci => GetLiveBetMap(ci, brokerTypesToRetreive), ProjectProvider.Instance.ResultProvider.GetResultLiveForCompetitions);
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
                        SportType = competition.Sporttype,
                        Language = languageType
                    });
                }
            }
        }

        public CompetitionAdvancedTransport GetCompetitionItemRegularBet(LanguageType languageType, BrokerType[] brokerTypesToRetreive, BrokerType[] brokerTypesToDisplay, int competitionItemID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereEquals(CompetitionItem.Fields.ID, competitionItemID));
                BuildCompetitiontItemFullModel(competition, ci => GetBetMap(ci, brokerTypesToRetreive), ProjectProvider.Instance.ResultProvider.GetResultForCompetitions);
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
                BuildCompetitiontItemFullModel(competitionTransports, ci => GetLiveBetMap(ci, brokerTypesToRetreive), ProjectProvider.Instance.ResultProvider.GetResultLiveForCompetitions);
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
                BuildCompetitiontItemFullModel(competition, ci => GetBetMap(ci, brokerTypesToRetreive), ProjectProvider.Instance.ResultProvider.GetResultForCompetitions);
                ProcessBrokerType(brokerTypesToDisplay, competition);
                return competition;
            }, new List<CompetitionTransport>());
        }

        public Dictionary<DateTime, List<Dictionary<BetOddType, BetItemTransport>>> GetRowDataForGraphCompetition(BrokerType[] brokerTypesToRetreive, SportType sportType, int competitionItemID) {
            return InvokeSafe(() => {
                return BuildOddsByDateByBroker(ints => GetBetMap(ints, brokerTypesToRetreive), sportType, competitionItemID);
            }, null);
        }

        public Dictionary<DateTime, List<Dictionary<BetOddType, BetItemTransport>>> GetRowDataForGraphCompetitionLive(BrokerType[] brokerTypesToRetreive, SportType sportType, int competitionItemID) {
            return InvokeSafe(() => {
                return BuildOddsByDateByBroker(ints => GetLiveBetMap(ints, brokerTypesToRetreive), sportType, competitionItemID);
            }, null);
        }

        private static Dictionary<DateTime, List<Dictionary<BetOddType, BetItemTransport>>> BuildOddsByDateByBroker<T>(Func<int[], Dictionary<int, List<IBet<T>>>> getBetMap, SportType sportType, int competitionItemID) {
            const int RESOLUTION = 10;
            var betMap = getBetMap(new[] { competitionItemID });
            if (betMap == null || !betMap.Any()) {
                return null;
            }

            var mapBetsByDates = new Dictionary<DateTime, List<IBet<T>>>();
            var mapByBrokers = new Dictionary<BrokerType, List<KeyValuePair<DateTime, IBet<T>>>>();
            foreach (var ibet in betMap.Values.First()) {
                var ibetDate = ibet.Datecreatedutc.Round(DateTimeExtensions.DateRoundType.Minute, RESOLUTION);
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
                    var newLastiBetDate = firstBet.Key.AddMinutes(localDeltaMinutes).Round(DateTimeExtensions.DateRoundType.Minute, RESOLUTION);
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
            var competitionNameMap = GetCompetitionNameMap(languageType, competitionItemForDate.Select(c => (int) c[CompetitionItem.Fields.CompetitionuniqueID]));
            var competitorNameMap = Competitor.DataSource
                .WhereIn(Competitor.Fields.CompetitoruniqueID, competitionItemForDate.Select(c => (int) c[CompetitionItem.Fields.Competitoruniqueid1])
                                                                                     .Union(competitionItemForDate.Select(c =>(int) c[CompetitionItem.Fields.Competitoruniqueid2]))
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

            var competitionsDict = new Dictionary<int, CompetitionTransport>();
            foreach (var competitionItem in competitionItemForDate) {
                CompetitionTransport competitionTransport;
                var competitionUniqueID = (int)competitionItem[CompetitionItem.Fields.CompetitionuniqueID];
                if (!competitionsDict.TryGetValue(competitionUniqueID, out competitionTransport)) {
                    competitionTransport = ExtractNameFromCompetition(languageType, competitionUniqueID, competitionNameMap);
                    competitionsDict[competitionTransport.ID] = competitionTransport;
                }
                competitionTransport.CompetitionItems.Add(new CompetitionItemBetShortTransport {
                    CompetitionItemID = (int) competitionItem[CompetitionItem.Fields.ID],
                    DateUtc = (DateTime) competitionItem[CompetitionItem.Fields.Dateeventutc],
                    Competitor1 = ExtractNameFromCompetitor((int) competitionItem[CompetitionItem.Fields.Competitoruniqueid1], competitorNameMap),
                    Competitor2 = ExtractNameFromCompetitor((int) competitionItem[CompetitionItem.Fields.Competitoruniqueid2], competitorNameMap),
                });
            }
            return competitionsDict.Values.ToList();
        }

        private static Dictionary<int, CompetitionUniqueAdvanced> GetCompetitionNameMap(LanguageType languageType, IEnumerable<int> competitionuniqueID) {
            return CompetitionUniqueAdvanced.DataSource
                                            .WhereIn(CompetitionUniqueAdvanced.Fields.CompetitionuniqueID, competitionuniqueID)
                                            .Sort(CompetitionUniqueAdvanced.Fields.ID, SortDirection.Asc)
                                            .AsList(
                                                CompetitionUniqueAdvanced.Fields.CompetitionuniqueID,
                                                CompetitionUniqueAdvanced.Fields.Name,
                                                CompetitionUniqueAdvanced.Fields.Languagetype,
                                                CompetitionUniqueAdvanced.Fields.Sporttype
                                            )
                                            .GroupBy(e => e.CompetitionuniqueID)
                                            .ToDictionary(e => e.Key, e => {
                                                var withCurrentLanguage = e.Where(i => i.Languagetype == languageType);
                                                var competitionUniqueAdvanceds = withCurrentLanguage as CompetitionUniqueAdvanced[] ?? withCurrentLanguage.ToArray();
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
                Name = competitor.NameFull ?? competitor.NameShort
            };
        }

        private static CompetitionTransport ExtractNameFromCompetition(LanguageType languageType, int competitionID, Dictionary<int, CompetitionUniqueAdvanced> competitionNameMap) {
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
                Name = competition.Name,
                SportType = competition.Sporttype,
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