using System;
using System.Linq;
using CommonUtils.Core.Logger;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.DataProvider.DataHelper.RawData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Entity.Helper;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class RawCompetitionHelper {
        public static RawCompetitionSpecify GetRawCompetitionSpecify(BrokerType brokerType, LanguageType language,
            SportType sportType, GenderType genderDetected, string[] nameOrigin) {
            return RawCompetitionSpecify.DataSource.FilterByBroker(brokerType).FilterByLanguage(language).FilterBySportType(sportType)
                .FilterByNameCompetition(nameOrigin)
                .FilterByGender(genderDetected,
                    RawCompetitionSpecify.Fields.CompetitionuniqueID,
                    RawCompetitionSpecify.Fields.CompetitionspecifyuniqueID,
                    RawCompetitionSpecify.Fields.Name,
                    RawCompetitionSpecify.Fields.ID,
                    RawCompetitionSpecify.Fields.RawCompetitionID)
                .FirstOrDefault();
        }

        public static RawTemplateObj<CompetitionSpecifyTransport> CreateCompetitionSpecifyRawObject(int rawID, int parentRawID, ICompetitionSpecify competitionSpecify, LanguageType language, SportType sportType, GenderType genderDetected) {
            var result = new RawTemplateObj<CompetitionSpecifyTransport>();
            result.RawObject.ID = rawID;
            result.RawObject.ParentID = parentRawID;

            result.Object.LanguageType = language;
            result.Object.SportType = sportType;
            result.Object.GenderType = genderDetected;

            result.Object.CompetitionUniqueID = competitionSpecify.CompetitionuniqueID;
            result.Object.ID = competitionSpecify.CompetitionSpecifyUniqueID;
            return result;
        }
        
        public static RawCompetitionSpecify CreateCompetitionSpecify(ProcessStat competitionStat, BrokerType brokerType, LanguageType language, SportType sportType, GenderType genderDetected, string[] nameOrigin, CompetitionParsed competitionToSave, GatherBehaviorMode algoMode) {
            var nameOriginShort = CompetitionHelper.GetShortCompetitionName(nameOrigin, sportType);
            var competitionRaw = new BrokerEntityBuilder<RawCompetition>(competitionStat)
                .SetupValidateObject(rawCompetition => rawCompetition.CompetitionuniqueID != default(int))
                .SetupGetRaw(() => {
                    return RawCompetition.DataSource.FilterByBroker(brokerType).FilterByLanguage(language).FilterBySportType(sportType)
                        .FilterByNameCompetition(nameOriginShort)
                        .FilterByGender(genderDetected, RawCompetition.Fields.CompetitionuniqueID, RawCompetition.Fields.Linkstatus).FirstOrDefault();
                })
                .SetupCreateRaw(() => BrokerEntityIfaceCreator.CreateEntity<RawCompetition>(brokerType, language, sportType, genderDetected, LinkEntityStatus.Unlinked, nameOriginShort))
                .SetupCreateOriginal(algoMode, rawCompetition => {
                    var competition = Competition.DataSource.FilterByLanguage(language).FilterBySportType(sportType)
                        .FilterByNameCompetition(nameOriginShort)
                        .FilterByGender(genderDetected, Competition.Fields.CompetitionuniqueID)
                        .FirstOrDefault();
                    if (competition == null) {
                        var competitionUnique = new CompetitionUnique {
                            IsUsed = true
                        };
                        competitionUnique.Save();
                        competition = new Competition {
                            Datecreatedutc = DateTime.UtcNow,
                            Languagetype = language,
                            SportType = sportType,
                            Name = CompetitionHelper.ListStringToName(nameOriginShort),
                            Gendertype = genderDetected,
                            CompetitionuniqueID = competitionUnique.ID
                        };
                        competition.Save();
                    }
                    rawCompetition.CompetitionuniqueID = competition.CompetitionuniqueID;
                    rawCompetition.Linkstatus = LinkEntityStatus.Original | LinkEntityStatus.Linked;
                    return rawCompetition;
                })
                .SetupFinally(rawCompetition => {
                    if (algoMode.HasFlag(GatherBehaviorMode.CreateNewLanguageName)) {
                        if (rawCompetition.CompetitionuniqueID != default(int) && !Competition.DataSource
                            .WhereEquals(Competition.Fields.CompetitionuniqueID, rawCompetition.CompetitionuniqueID)
                            .FilterByLanguage(language)
                            .IsExists()) {
                            new Competition {
                                CompetitionuniqueID = rawCompetition.CompetitionuniqueID,
                                Datecreatedutc = DateTime.UtcNow,
                                Gendertype = genderDetected,
                                SportType = sportType,
                                Name = CompetitionHelper.ListStringToName(nameOriginShort),
                                Languagetype = language
                            }.Save();
                        }
                    }
                    rawCompetition.Save();
                    return rawCompetition;
                })
                .MakeObject();
            
            return BrokerEntityIfaceCreator.CreateEntity<RawCompetitionSpecify>(brokerType, language, sportType, genderDetected, LinkEntityStatus.Unlinked, nameOrigin, specify => {
                specify.RawCompetitionID = competitionRaw.ID;
                if (specify.CompetitionuniqueID == default(int) && competitionRaw.CompetitionuniqueID != default(int)) {
                    specify.CompetitionuniqueID = competitionRaw.CompetitionuniqueID;
                }
            });
        }
    }
}