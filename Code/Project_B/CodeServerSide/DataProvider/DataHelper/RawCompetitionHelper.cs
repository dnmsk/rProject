using System;
using System.Collections.Generic;
using System.Linq;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper.RawData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Entity.Helper;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class RawCompetitionHelper {
        public static RawTemplateObj<CompetitionSpecifyTransport> GetCompetitionSpecify(BrokerType brokerType, LanguageType language, SportType sportType, GenderType genderDetected, List<string> nameOrigin) {
            var rawCompetitionSpecify = GetRawCompetitionSpecify(brokerType, language, sportType, genderDetected, nameOrigin);
            if (rawCompetitionSpecify == null || rawCompetitionSpecify.CompetitionSpecifyUniqueID == default(int)) {
                return null;
            }
            return CreateCompetitionSpecifyRawObject(rawCompetitionSpecify.ID, rawCompetitionSpecify.RawCompetitionID, rawCompetitionSpecify, language, sportType, genderDetected);
        }

        private static RawCompetitionSpecify GetRawCompetitionSpecify(BrokerType brokerType, LanguageType language,
            SportType sportType, GenderType genderDetected, List<string> nameOrigin) {
            var dbDataSource = RawCompetitionSpecify.DataSource
                                                    .WhereEquals(RawCompetitionSpecify.Fields.Languagetype, (short) language)
                                                    .WhereEquals(RawCompetitionSpecify.Fields.Sporttype, (short) sportType)
                                                    .WhereEquals(RawCompetitionSpecify.Fields.Brokerid, (short) brokerType);
            var fieldsToRetrive = new Enum[] {
                RawCompetitionSpecify.Fields.CompetitionuniqueID,
                RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID,
                RawCompetitionSpecify.Fields.Name,
                RawCompetitionSpecify.Fields.ID,
                RawCompetitionSpecify.Fields.RawCompetitionID
            };
            var rawCompetitionSpecify = QueryHelper.FilterByGender(dbDataSource.Where(QueryHelper.GetIndexedFilterByWordIgnoreCase(CompetitionHelper.ListStringToName(nameOrigin), RawCompetitionSpecify.Fields.Name)),
                    RawCompetitionSpecify.Fields.Gendertype, genderDetected, fieldsToRetrive).FirstOrDefault();
            return rawCompetitionSpecify;
        }

        private static RawTemplateObj<CompetitionSpecifyTransport> CreateCompetitionSpecifyRawObject(int rawID, int parentRawID, ICompetitionSpecify competitionSpecify, LanguageType language, SportType sportType, GenderType genderDetected) {
            var result = new RawTemplateObj<CompetitionSpecifyTransport>();
            result.RawObject.ID = rawID;
            result.RawObject.ParentID = parentRawID;

            result.Object.LanguageType = language;
            result.Object.SportType = sportType;
            result.Object.GenderType = genderDetected;

            result.Object.CompetitionUniqueID = competitionSpecify.CompetitionuniqueID;
            result.Object.CompetitionSpecifyUniqueID = competitionSpecify.CompetitionSpecifyUniqueID;
            return result;
        }

        public static RawTemplateObj<CompetitionSpecifyTransport> CreateCompetitionSpecify(BrokerType brokerType, LanguageType language, SportType sportType, GenderType genderDetected, List<string> nameOrigin, List<string> nameOriginShort, CompetitionParsed competitionToSave, GatherBehaviorMode algoMode) {
            var dbDataSource = RawCompetition.DataSource
                                                      .WhereEquals(RawCompetition.Fields.Languagetype, (short)language)
                                                      .WhereEquals(RawCompetition.Fields.Sporttype, (short)sportType)
                                                      .WhereEquals(RawCompetition.Fields.Brokerid, (short)brokerType);
            var filedToRetreive = new Enum[] {RawCompetition.Fields.CompetitionuniqueID, RawCompetition.Fields.Linkstatus};
            var competition = QueryHelper.FilterByGender(dbDataSource.Where(QueryHelper.GetIndexedFilterByWordIgnoreCase(CompetitionHelper.ListStringToName(nameOriginShort), RawCompetition.Fields.Name)), 
                                                RawCompetition.Fields.Gendertype, genderDetected, filedToRetreive).FirstOrDefault()
                ?? BrokerEntityIfaceCreator.CreateEntity<RawCompetition>(brokerType, language, sportType, genderDetected, LinkEntityStatus.ToLink, nameOriginShort);

            var competitionSpecifyId = default(int);
            if (algoMode.HasFlag(GatherBehaviorMode.CanDetectCompetition)) {
                var competitionUnique = CompetitionHelper.TryDetectCompetitionUniqueFromMatches(sportType, nameOrigin, competitionToSave);
                if (competitionUnique != null) {
                    competition.CompetitionuniqueID = competitionUnique.ID;
                    competition.Linkstatus = LinkEntityStatus.LinkByStatistics;
                }
            }
            competition.Save();

            var competitionSpecify = GetRawCompetitionSpecify(brokerType, language, sportType, genderDetected, nameOrigin)
                ?? BrokerEntityIfaceCreator.CreateEntity<RawCompetitionSpecify>(brokerType, language, sportType, genderDetected, LinkEntityStatus.ToLink, nameOrigin, specify => {
                    specify.RawCompetitionID = competition.ID;
                });
            if (competitionSpecify.CompetitionuniqueID == default(int) && competition.CompetitionuniqueID != default(int)) {
                competitionSpecify.CompetitionuniqueID = competition.CompetitionuniqueID;
            }
            if (competitionSpecifyId != default(int) && algoMode.HasFlag(GatherBehaviorMode.CanDetectCompetition)) {
                competitionSpecify.CompetitionSpecifyUniqueID = competitionSpecifyId;
                competitionSpecify.Linkstatus = LinkEntityStatus.LinkByStatistics;
            }
            competitionSpecify.Save();
            return CreateCompetitionSpecifyRawObject(competitionSpecify.ID, competition.ID, competitionSpecify, language, sportType, genderDetected);
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