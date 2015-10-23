using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
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

        public CompetitionTransport GetCompetition(LanguageType language, SportType sportType, List<string> nameOrigin) {
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
                    return GetCompetitionUnique(language, sportType, genderDetected, nameOrigin);
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

        private CompetitionTransport GetCompetitionUnique(LanguageType language, SportType sportType, GenderType genderDetected, List<string> nameOrigin) {
            var nameOriginShort = CompetitionHelper.GetShortCompetitionName(nameOrigin);
            var competitionUniqueAdvanced = CompetitionUniqueAdvanced.DataSource
                                         .WhereEquals(CompetitionUniqueAdvanced.Fields.Gendertype, (short)genderDetected)
                                         .WhereEquals(CompetitionUniqueAdvanced.Fields.Languagetype, (short)language)
                                         .WhereEquals(CompetitionUniqueAdvanced.Fields.Sporttype, (short)sportType)
                                         .Where(QueryHelper.GetFilterByWordsForField(nameOriginShort, CompetitionUniqueAdvanced.Fields.Name))
                                         .First(
                                            CompetitionUniqueAdvanced.Fields.CompetitionuniqueID
                );
            if (competitionUniqueAdvanced == null) {
                var uniqueID = new CompetitionUnique {
                    IsUsed = true
                };
                uniqueID.Save();

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

        private static string ListStringToName(List<string> names) {
            return names.StrJoin(". ");
        }

        public int GetCompetitionItem(CompetitorTransport competitor1Transport, CompetitorTransport competitor2Transport, CompetitionTransport competitionTransport, DateTime eventDateUtc) {
            return InvokeSafeSingleCall(() => {
                var competitionItem = CompetitionItem.DataSource
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
                    .WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionTransport.UniqueID)
                    .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Desc)
                    .First(CompetitionItem.Fields.ID);
                if (competitionItem != null) {
                    if (eventDateUtc != DateTime.MinValue) {
                        if (Math.Abs((competitionItem.Dateeventutc - eventDateUtc).TotalDays) > 2) {
                            competitionItem = null;
                        } else {
                            competitionItem.Dateeventutc = eventDateUtc;
                            competitionItem.Save();
                        }
                    }
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

        public List<CompetitionItemBetShortModel> GetCompetitionItemsRegularBet(LanguageType languageType, SportType sportType, DateTime date) {
            return InvokeSafe(() => {
                var shortModels = GetCompetitionItemShortModelByDate(languageType, sportType, date);
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
                .Join(JoinType.Left, CompetitionResult.Fields.CompetitionitemID, BetLive.Fields.CompetitionitemID, RetrieveMode.NotRetrieve)
                //.WhereNull(CompetitionResult.Fields.ID)
                .WhereIn(BetLive.Fields.CompetitionitemID, ints)
                .AsList()
                .GroupBy(e => e.CompetitionitemID)
                .ToDictionary(e => e.Key, e => e.Select(t => (IBet<long>)t)
                .ToList());
        }

        public List<CompetitionItemBetShortModel> GetCompetitionItemsLiveBet(LanguageType languageType, SportType sportType, DateTime date) {
            return InvokeSafe(() => {
                var shortModels = GetCompetitionItemShortModelByDate(languageType, sportType, date);
                return GetCompetitiontItemBetModel(shortModels, false, GetLiveBetMap);
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

        public List<CompetitionItemBetShortModel> GetCompetitionItemsRegularBetForCompetition(LanguageType languageType, int competitionID) {
            return InvokeSafe(() => {
                var competition = GetCompetitionItemShortModel(languageType, CompetitionItem.DataSource.WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionID));
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

        private static List<CompetitionItemShortModel> GetCompetitionItemShortModelByDate(LanguageType languageType, SportType sportType, DateTime date) {
            var competitionItemForDateQuery = CompetitionItem.DataSource
                .Where(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, date.Date)
                .Where(CompetitionItem.Fields.Dateeventutc, Oper.Less, date.Date.AddDays(1));
            if (sportType != SportType.Unknown) {
                competitionItemForDateQuery = competitionItemForDateQuery
                    .WhereEquals(CompetitionItem.Fields.Sporttype, (short)sportType);
            }
            return GetCompetitionItemShortModel(languageType, competitionItemForDateQuery);
        }

        private static List<CompetitionItemShortModel> GetCompetitionItemShortModel(LanguageType languageType, DbDataSource<CompetitionItem, int> competitionItemQuery) {
            var competitionNameMapQuery = CompetitionUniqueAdvanced.DataSource
                .WhereEquals(CompetitionUniqueAdvanced.Fields.Languagetype, (short)languageType);
            var competitorNameMapQuery = Competitor.DataSource
                .WhereEquals(Competitor.Fields.Languagetype, (short)languageType);
            var competitionItemForDate = competitionItemQuery
               .WhereNotEquals(CompetitionItem.Fields.Dateeventutc, DateTime.MinValue)
               .Sort(CompetitionItem.Fields.Sporttype)
               .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Desc)
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
                    CompetitionUniqueAdvanced.Fields.Name
                )
                .GroupBy(e => e.CompetitionuniqueID)
                .ToDictionary(e => e.Key, e => e.First());
            var competitorNameMap = competitorNameMapQuery
                .WhereIn(Competitor.Fields.CompetitoruniqueID, competitionItemForDate.Select(c => c.Competitoruniqueid1)
                                                                                     .Union(competitionItemForDate.Select(c => c.Competitoruniqueid2))
                                                                                     .Distinct())
                .Sort(Competitor.Fields.ID, SortDirection.Asc)
                .AsList(
                    Competitor.Fields.CompetitoruniqueID,
                    Competitor.Fields.NameShort,
                    Competitor.Fields.NameFull
                )
                .GroupBy(e => e.CompetitoruniqueID)
                .ToDictionary(e => e.Key, e => e.First());

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
            var competitor = competitorNameMap.TryGetValueOrDefault(competitorID) ?? Competitor.DataSource
                .WhereEquals(Competitor.Fields.CompetitoruniqueID, competitorID)
                .First();
            return new CompetitorModel {
                ID = competitorID,
                Name = competitor.NameFull ?? competitor.NameShort
            };
        }

        private static CompetitionModel ExtractNameFromCompetition(int competitionID, Dictionary<int, CompetitionUniqueAdvanced> competitionNameMap) {
            var competition = competitionNameMap.TryGetValueOrDefault(competitionID) ?? CompetitionUniqueAdvanced.DataSource
                .WhereEquals(CompetitionUniqueAdvanced.Fields.CompetitionuniqueID, competitionID)
                .First();
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