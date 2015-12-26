using System;
using System.Collections.Generic;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class RawCompetitionHelper {
        public static RawCompetitionSpecify GetCompetitionSpecify(BrokerType brokerType, LanguageType language, SportType sportType, GenderType genderDetected, List<string> nameOrigin) {
            return RawCompetitionSpecify.DataSource
                        .Where(QueryHelper.GetFilterByGenger(genderDetected, RawCompetitionSpecify.Fields.Gendertype))
                        .WhereEquals(RawCompetitionSpecify.Fields.Languagetype, (short)language)
                        .WhereEquals(RawCompetitionSpecify.Fields.Sporttype, (short)sportType)
                        .WhereEquals(RawCompetitionSpecify.Fields.Brokerid, (short)brokerType)
                        .Where(QueryHelper.GetFilterByWordsForField(nameOrigin, RawCompetitionSpecify.Fields.Name))
                        .First(
                            RawCompetitionSpecify.Fields.CompetitionuniqueID,
                            RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID,
                            RawCompetitionSpecify.Fields.Name,                            
                            RawCompetitionSpecify.Fields.ID,                            
                            RawCompetitionSpecify.Fields.RawCompetitionID
                        );
        }

        public static CompetitionSpecifyTransport CreateCompetitionSpecify(BrokerType brokerType, LanguageType language, SportType sportType, GenderType genderDetected, List<string> nameOrigin, List<string> nameOriginShort, CompetitionParsed competitionToSave, GatherBehaviorMode algoMode) {
            var competition = RawCompetition.DataSource
                .Where(QueryHelper.GetFilterByGenger(genderDetected, RawCompetition.Fields.Gendertype))
                .WhereEquals(RawCompetition.Fields.Languagetype, (short)language)
                .WhereEquals(RawCompetition.Fields.Sporttype, (short)sportType)
                .WhereEquals(RawCompetition.Fields.Brokerid, (short)brokerType)
                .Where(QueryHelper.GetFilterByWordsForField(nameOriginShort, RawCompetition.Fields.Name))
                .First(RawCompetition.Fields.CompetitionuniqueID, RawCompetition.Fields.Linkstatus) 
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

            var competitionSpecify = RawCompetitionSpecify.DataSource
                .Where(QueryHelper.GetFilterByGenger(genderDetected, RawCompetitionSpecify.Fields.Gendertype))
                .WhereEquals(RawCompetitionSpecify.Fields.Languagetype, (short)language)
                .WhereEquals(RawCompetitionSpecify.Fields.Sporttype, (short)sportType)
                .WhereEquals(RawCompetitionSpecify.Fields.Brokerid, (short)brokerType)
                .WhereEquals(RawCompetitionSpecify.Fields.RawCompetitionID, competition.ID)
                .Where(QueryHelper.GetFilterByWordsForField(nameOrigin, RawCompetitionSpecify.Fields.Name))
                .First()
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

            return new CompetitionSpecifyTransport {
                Name = competitionSpecify.Name,
                GenderType = genderDetected,
                SportType = sportType,
                LanguageType = language,
                CompetitionUniqueID = competitionSpecify.CompetitionuniqueID,
                CompetitionSpecifyUniqueID = competitionSpecify.CompetitionSpecifyUniqueID,
                RawCompetitionID = competition.ID,
                RawCompetitionSpecifyID = competitionSpecify.ID
            };
        }

        public static CompetitionSpecifyTransport UpdateCompetitionParsedForUniqueIDs(CompetitionSpecifyTransport competitionSpecifyFromRaw, LinkEntityStatus linkEntityStatus) {
            RawCompetition.DataSource
                .WhereEquals(RawCompetition.Fields.ID, competitionSpecifyFromRaw.RawCompetitionID)
                .Update(new Dictionary<Enum, DbFunction> {
                    { RawCompetition.Fields.CompetitionuniqueID, new DbFnConst(competitionSpecifyFromRaw.CompetitionUniqueID) },
                    { RawCompetition.Fields.Linkstatus, new DbFnConst((short)linkEntityStatus) },
                });
            RawCompetitionSpecify.DataSource
                .WhereEquals(RawCompetitionSpecify.Fields.ID, competitionSpecifyFromRaw.RawCompetitionSpecifyID)
                .Update(new Dictionary<Enum, DbFunction> {
                    { RawCompetitionSpecify.Fields.CompetitionuniqueID, new DbFnConst(competitionSpecifyFromRaw.CompetitionUniqueID) },
                    { RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID, new DbFnConst(competitionSpecifyFromRaw.CompetitionSpecifyUniqueID) },
                    { RawCompetitionSpecify.Fields.Linkstatus, new DbFnConst((short)linkEntityStatus) },
                });
            return competitionSpecifyFromRaw;
        }
    }
}