using System;
using System.Collections.Generic;
using System.Linq;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper.RawData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class RawCompetitionHelper {
        public static RawTemplateObj<CompetitionSpecifyTransport> GetCompetitionSpecify(BrokerType brokerType, LanguageType language, SportType sportType, GenderType genderDetected, List<string> nameOrigin) {
            var rawCompetitionSpecify = QueryHelper.FilterByGender(RawCompetitionSpecify.DataSource
                //.Where(QueryHelper.GetFilterByGenger(genderDetected, RawCompetitionSpecify.Fields.Gendertype))
                .WhereEquals(RawCompetitionSpecify.Fields.Languagetype, (short)language)
                .WhereEquals(RawCompetitionSpecify.Fields.Sporttype, (short)sportType)
                .WhereEquals(RawCompetitionSpecify.Fields.Brokerid, (short)brokerType)
                .Where(QueryHelper.GetFilterByWordsForField(nameOrigin, RawCompetitionSpecify.Fields.Name)), RawCompetitionSpecify.Fields.Gendertype, genderDetected,
                    RawCompetitionSpecify.Fields.CompetitionuniqueID,
                    RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID,
                    RawCompetitionSpecify.Fields.Name,
                    RawCompetitionSpecify.Fields.ID,
                    RawCompetitionSpecify.Fields.RawCompetitionID)
                .FirstOrDefault();

            if (rawCompetitionSpecify == null || rawCompetitionSpecify.CompetitionSpecifyUniqueID == default(int)) {
                return null;
            }
            return CreateCompetitionSpecifyRawObject(rawCompetitionSpecify.ID, rawCompetitionSpecify.RawCompetitionID, rawCompetitionSpecify);
        }

        private static RawTemplateObj<CompetitionSpecifyTransport> CreateCompetitionSpecifyRawObject(int rawID, int parentRawID, ICompetitionSpecify competitionSpecify) {
            var result = new RawTemplateObj<CompetitionSpecifyTransport>();
            result.RawObject.ID = rawID;
            result.RawObject.ParentID = parentRawID;

            result.Object.LanguageType = competitionSpecify.Languagetype;
            result.Object.SportType = competitionSpecify.SportType;
            result.Object.GenderType = competitionSpecify.Gendertype;

            result.Object.CompetitionUniqueID = competitionSpecify.CompetitionuniqueID;
            result.Object.CompetitionSpecifyUniqueID = competitionSpecify.CompetitionSpecifyUniqueID;
            return result;
        }

        public static RawTemplateObj<CompetitionSpecifyTransport> CreateCompetitionSpecify(BrokerType brokerType, LanguageType language, SportType sportType, GenderType genderDetected, List<string> nameOrigin, List<string> nameOriginShort, CompetitionParsed competitionToSave, GatherBehaviorMode algoMode) {
            var competition = QueryHelper.FilterByGender(RawCompetition.DataSource
                                                    .WhereEquals(RawCompetition.Fields.Languagetype, (short)language)
                                                    .WhereEquals(RawCompetition.Fields.Sporttype, (short)sportType)
                                                    .WhereEquals(RawCompetition.Fields.Brokerid, (short)brokerType)
                                                    .Where(QueryHelper.GetFilterByWordsForField(nameOriginShort, RawCompetition.Fields.Name)), 
                                                RawCompetition.Fields.Gendertype, genderDetected, RawCompetition.Fields.CompetitionuniqueID, RawCompetition.Fields.Linkstatus)
                .FirstOrDefault() 
                    ?? new RawCompetition {
                            Datecreatedutc = DateTime.UtcNow,
                            Languagetype = language,
                            SportType = sportType,
                            BrokerID = brokerType,
                            Name = CompetitionHelper.ListStringToName(nameOriginShort),
                            Gendertype = genderDetected,
                            Linkstatus = LinkEntityStatus.ToLink
                        };

            var competitionSpecifyId = default(int);
            if (algoMode.HasFlag(GatherBehaviorMode.CanDetectCompetition)) {
                var competitionUnique = CompetitionHelper.TryDetectCompetitionUniqueFromMatches(sportType, nameOrigin, competitionToSave);
                if (competitionUnique != null) {
                    competition.CompetitionuniqueID = competitionUnique.ID;
                    competition.Linkstatus = LinkEntityStatus.LinkByStatistics;
                }
            }
            competition.Save();

            var competitionSpecify = QueryHelper.FilterByGender(RawCompetitionSpecify.DataSource
                                                        .WhereEquals(RawCompetitionSpecify.Fields.Languagetype, (short)language)
                                                        .WhereEquals(RawCompetitionSpecify.Fields.Sporttype, (short)sportType)
                                                        .WhereEquals(RawCompetitionSpecify.Fields.Brokerid, (short)brokerType)
                                                        .WhereEquals(RawCompetitionSpecify.Fields.RawCompetitionID, competition.ID)
                                                        .Where(QueryHelper.GetFilterByWordsForField(nameOrigin, RawCompetitionSpecify.Fields.Name)),
                                                    RawCompetitionSpecify.Fields.Gendertype, genderDetected)
                .FirstOrDefault()
                ?? new RawCompetitionSpecify {
                        Datecreatedutc = DateTime.UtcNow,
                        Languagetype = language,
                        SportType = sportType,
                        Name = CompetitionHelper.ListStringToName(nameOrigin),
                        Gendertype = genderDetected,
                        BrokerID = brokerType,
                        Linkstatus = LinkEntityStatus.ToLink,
                        RawCompetitionID = competition.ID
                    };
            if (competitionSpecify.CompetitionuniqueID == default(int) && competition.CompetitionuniqueID != default(int)) {
                competitionSpecify.CompetitionuniqueID = competition.CompetitionuniqueID;
            }
            if (competitionSpecifyId != default(int) && algoMode.HasFlag(GatherBehaviorMode.CanDetectCompetition)) {
                competitionSpecify.CompetitionSpecifyUniqueID = competitionSpecifyId;
                competitionSpecify.Linkstatus = LinkEntityStatus.LinkByStatistics;
            }
            competitionSpecify.Save();
            return CreateCompetitionSpecifyRawObject(competitionSpecify.ID, competition.ID, competitionSpecify);
        }

        public static RawTemplateObj<CompetitionSpecifyTransport> UpdateCompetitionParsedForUniqueIDs(RawTemplateObj<CompetitionSpecifyTransport> competitionSpecifyFromRaw, LinkEntityStatus linkEntityStatus) {
            RawCompetition.DataSource
                .WhereEquals(RawCompetition.Fields.ID, competitionSpecifyFromRaw.RawObject.ParentID)
                .Update(new Dictionary<Enum, DbFunction> {
                    { RawCompetition.Fields.CompetitionuniqueID, new DbFnConst(competitionSpecifyFromRaw.Object.CompetitionUniqueID) },
                    { RawCompetition.Fields.Linkstatus, new DbFnConst((short)linkEntityStatus) },
                });
            RawCompetitionSpecify.DataSource
                .WhereEquals(RawCompetitionSpecify.Fields.ID, competitionSpecifyFromRaw.RawObject.ID)
                .Update(new Dictionary<Enum, DbFunction> {
                    { RawCompetitionSpecify.Fields.CompetitionuniqueID, new DbFnConst(competitionSpecifyFromRaw.Object.CompetitionUniqueID) },
                    { RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID, new DbFnConst(competitionSpecifyFromRaw.Object.CompetitionSpecifyUniqueID) },
                    { RawCompetitionSpecify.Fields.Linkstatus, new DbFnConst((short)linkEntityStatus) },
                });
            return competitionSpecifyFromRaw;
        }
    }
}