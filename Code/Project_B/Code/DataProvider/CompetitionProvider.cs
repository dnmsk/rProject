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
                    .WhereEquals(CompetitionItem.Fields.Competitoruniqueid1, competitor1Transport.UniqueID)
                    .WhereEquals(CompetitionItem.Fields.Competitoruniqueid2, competitor2Transport.UniqueID)
                    .WhereEquals(CompetitionItem.Fields.Sporttype, (short)competitionTransport.SportType)
                    .WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionTransport.UniqueID)
                    .Sort(CompetitionItem.Fields.Dateeventutc, SortDirection.Desc)
                    .First(CompetitionItem.Fields.ID);
                if (competitionItem != null) {
                    if (eventDateUtc != DateTime.MinValue && Math.Abs((competitionItem.Dateeventutc - eventDateUtc).TotalDays) > 2) {
                        competitionItem = null;
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

        public List<CompetitionItemRegilarModel> GetCompetitionItemRegular(LanguageType languageType, SportType sportType, DateTime date) {
            return InvokeSafe(() => {
                var competitionItemForDateQuery = CompetitionItem.DataSource
                    .Where(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, date.Date)
                    .Where(CompetitionItem.Fields.Dateeventutc, Oper.Less, date.Date.AddDays(1));
                var competitionNameMapQuery = Competition.DataSource
                    .WhereEquals(Competition.Fields.Languagetype, (short) languageType);
                var competitorNameMapQuery = Competitor.DataSource
                    .WhereEquals(Competitor.Fields.Languagetype, (short) languageType);
                if (sportType != SportType.Unknown) {
                    competitionItemForDateQuery = competitionItemForDateQuery
                        .WhereEquals(CompetitionItem.Fields.Sporttype, (short) sportType);
                    competitionNameMapQuery = competitionNameMapQuery
                        .WhereEquals(Competition.Fields.Sporttype, (short)sportType);
                    competitorNameMapQuery = competitorNameMapQuery
                        .WhereEquals(Competitor.Fields.Sporttype, (short)sportType);
                }
                var competitionItemForDate = competitionItemForDateQuery
                   .AsList(
                        CompetitionItem.Fields.ID,
                        CompetitionItem.Fields.Dateeventutc,
                        CompetitionItem.Fields.Competitoruniqueid1,
                        CompetitionItem.Fields.Competitoruniqueid2,
                        CompetitionItem.Fields.Sporttype,
                        CompetitionItem.Fields.CompetitionuniqueID
                    );
                var competitionNameMap = competitionNameMapQuery
                    .WhereIn(Competition.Fields.CompetitionuniqueID, competitionItemForDate.Select(c => c.CompetitionuniqueID))
                    .Sort(Competition.Fields.ID, SortDirection.Asc)
                    .AsList(
                        Competition.Fields.CompetitionuniqueID,
                        Competition.Fields.Name
                    )
                    .GroupBy(e => e.CompetitionuniqueID)
                    .ToDictionary(e => e.Key, e => e.First());
                var competitorNameMap = competitorNameMapQuery
                    .WhereIn(Competitor.Fields.CompetitoruniqueID, competitionItemForDate.Select(c => c.Competitoruniqueid1)
                                                                                         .Union(competitionItemForDate.Select(c=> c.Competitoruniqueid2))
                                                                                         .Distinct())
                    .Sort(Competitor.Fields.ID, SortDirection.Asc)
                    .AsList(
                        Competitor.Fields.CompetitoruniqueID,
                        Competitor.Fields.NameShort,
                        Competitor.Fields.NameFull
                    )
                    .GroupBy(e => e.CompetitoruniqueID)
                    .ToDictionary(e => e.Key, e=> e.First());
                var betGrouped = Bet.DataSource
                    .Join(JoinType.Left, BetAdvanced.Fields.BetID, Bet.Fields.ID, RetrieveMode.Retrieve)
                    .WhereIn(Bet.Fields.CompetitionitemID, competitionItemForDate.Select(c => c.ID))
                    .AsList()
                    .GroupBy(e => e.CompetitionitemID)
                    .ToDictionary(e => e.Key, e => e.ToList());

                var result = new List<CompetitionItemRegilarModel>();
                foreach (var competitionItem in competitionItemForDate) {
                    var betsForCompetition = betGrouped.TryGetValueOrDefaultStruct(competitionItem.ID);
                    if (betsForCompetition == null) {
                        continue;
                    }
                    var itemModel = new CompetitionItemRegilarModel {
                        ID = competitionItem.ID,
                        DateUtc = competitionItem.Dateeventutc,
                        SportType = competitionItem.SportType,
                        Competitor1 = ExtractNameFromCompetitor(competitionItem.Competitoruniqueid1, competitorNameMap),
                        Competitor2 = ExtractNameFromCompetitor(competitionItem.Competitoruniqueid2, competitorNameMap),
                        CurrentBets = BuildCurrentOddsMap(competitionItem.SportType, betsForCompetition, true, (cur, next) => cur < next),
                        HistoryMaxBets = BuildCurrentOddsMap(competitionItem.SportType, betsForCompetition, false, (cur, next) => cur < next),
                        HistoryMinBets = BuildCurrentOddsMap(competitionItem.SportType, betsForCompetition, false, (cur, next) => cur > next),
                        Competition = ExtractNameFromCompetition(competitionItem.CompetitionuniqueID, competitionNameMap)
                    };
                    result.Add(itemModel);
                }
                return result;
            }, null);
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

        private static CompetitionModel ExtractNameFromCompetition(int competitionID, Dictionary<int, Competition> competitionNameMap) {
            var competition = competitionNameMap.TryGetValueOrDefault(competitionID) ?? Competition.DataSource
                .WhereEquals(Competition.Fields.CompetitionuniqueID, competitionID)
                .First();
            return new CompetitionModel {
                ID = competitionID,
                Name = competition.Name
            };
        }

        private static Dictionary<BetOddType, BetItem> BuildCurrentOddsMap(SportType sportType, List<Bet> betsForCompetition, bool onlyCurrentOdds, Func<float, float, bool> conditionToAdd) {
            var odds = BetHelper.SportTypeWithOdds[sportType];
            betsForCompetition = betsForCompetition.OrderByDescending(b => b.ID).ToList();
            var result = new Dictionary<BetOddType, BetItem>();
            var currentFoundBrokerType = new List<BrokerType>();
            foreach (var bet in betsForCompetition) {
                if (onlyCurrentOdds && currentFoundBrokerType.Contains(bet.BrokerID)) {
                    continue;
                }
                currentFoundBrokerType.Add(bet.BrokerID);
                foreach (var betOddType in odds) {
                    var betItemCreated = BetHelper.OddsGetterMap[betOddType](bet);
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