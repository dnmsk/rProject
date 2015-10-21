using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.Code.DataProvider.DataHelper;
using Project_B.Code.DataProvider.Transport;
using Project_B.Code.Entity;
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

        public CompetitionItemModel GetFullCompetitionItemInfo(LanguageType languageType, int competitionItemID) {
            return InvokeSafe(() => {
                var competitionItem = CompetitionItem.DataSource.GetByKey(competitionItemID);
                var competition = CompetitionUniqueAdvanced.DataSource
                    .WhereEquals(CompetitionUniqueAdvanced.Fields.Languagetype, languageType)
                    .WhereEquals(CompetitionUniqueAdvanced.Fields.CompetitionuniqueID, competitionItem.CompetitionuniqueID)
                    .First();
                return (CompetitionItemModel)null;
            }, null);
        }
    }
}