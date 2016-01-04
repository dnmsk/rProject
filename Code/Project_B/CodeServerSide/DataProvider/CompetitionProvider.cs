using System;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.DataProvider.DataHelper.RawData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class CompetitionProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitionProvider).FullName);
        
        public CompetitionProvider() : base(_logger) {}

        public RawTemplateObj<CompetitionSpecifyTransport> GetCompetitionSpecify(ProcessStat competitionStat, ProcessStat competitionSpecifyStat, BrokerType brokerType, LanguageType language, SportType sportType, string[] nameOrigin, CompetitionParsed competitionToSave, GatherBehaviorMode algoMode) {
            return InvokeSafeSingleCall(() => {
                var genderDetected = GenderDetectorHelper.Instance[nameOrigin];
                nameOrigin = CleanCompetitionName(nameOrigin);
                var rawCompetitionSpecify = new BrokerEntityBuilder<RawCompetitionSpecify>(competitionSpecifyStat)
                    .SetupValidateObject(specify => true/*NOTE!! && specify.CompetitionSpecifyUniqueID != default(int)*/)
                    .SetupGetRaw(() => RawCompetitionHelper.GetRawCompetitionSpecify(brokerType, language, sportType, genderDetected, nameOrigin))
                    .SetupCreateRaw(() => RawCompetitionHelper.CreateCompetitionSpecify(competitionStat, brokerType, language, sportType, genderDetected, nameOrigin, competitionToSave, algoMode))
                    .SetupTryMatchRaw(algoMode, specify => {
                        if (false) {
                            specify.Linkstatus = LinkEntityStatus.LinkByStatistics;
                        }
                        return specify;
                    })
                    .SetupCreateOriginal(algoMode, specify => {
                        if (specify.CompetitionSpecifyUniqueID == default(int)) {
                            var competitionSpecifyUnique = new CompetitionSpecifyUnique {
                                CompetitionuniqueID = specify.CompetitionuniqueID
                            };
                            competitionSpecifyUnique.Save();
                            specify.CompetitionSpecifyUniqueID = competitionSpecifyUnique.ID;
                            specify.Save();
                        }
                        var competitionSpecify = new CompetitionSpecify {
                            Datecreatedutc = DateTime.UtcNow,
                            Languagetype = language,
                            SportType = sportType,
                            Name = CompetitionHelper.ListStringToName(nameOrigin),
                            Gendertype = genderDetected,
                            CompetitionuniqueID = specify.CompetitionuniqueID
                        };
                        specify.CompetitionSpecifyUniqueID = competitionSpecify.CompetitionSpecifyUniqueID;
                        if (competitionSpecify.CompetitionuniqueID != specify.CompetitionuniqueID) {
                            _logger.Error("{0} != {1}. {2}. SKIP", competitionSpecify.CompetitionuniqueID, specify.CompetitionuniqueID, CompetitionHelper.ListStringToName(nameOrigin));
                            return null;
                        }
                        specify.Linkstatus = LinkEntityStatus.Original;
                        return specify;
                    })
                    .SetupFinally(specify => {
                        if (specify.CompetitionSpecifyUniqueID != default(int)) {
                            if (algoMode.HasFlag(GatherBehaviorMode.CreateNewLanguageName)) {
                                if (specify.CompetitionSpecifyUniqueID != default(int) &&
                                    specify.CompetitionuniqueID != default(int) && !CompetitionSpecify.DataSource
                                        .WhereEquals(CompetitionSpecify.Fields.CompetitionSpecifyUniqueID, specify.CompetitionSpecifyUniqueID)
                                        .WhereEquals(CompetitionSpecify.Fields.Languagetype, (short)language)
                                        .IsExists()) {
                                    new CompetitionSpecify {
                                        Languagetype = language,
                                        Name = CompetitionHelper.ListStringToName(nameOrigin),
                                        CompetitionSpecifyUniqueID = specify.CompetitionSpecifyUniqueID,
                                        CompetitionuniqueID = specify.CompetitionuniqueID,
                                        Datecreatedutc = DateTime.UtcNow,
                                        Gendertype = genderDetected,
                                        SportType = sportType
                                    }.Save();
                                }
                            }
                        }
                        specify.Save();
                        return specify;
                    })
                    .MakeObject();
                return RawCompetitionHelper.CreateCompetitionSpecifyRawObject(rawCompetitionSpecify.ID, rawCompetitionSpecify.RawCompetitionID, rawCompetitionSpecify, language, sportType, genderDetected);
            }, new RawTemplateObj<CompetitionSpecifyTransport>());
        }

        private static readonly char[] _badSymbols = {'(', ')'};

        private static string[] CleanCompetitionName(string[] nameOrigin) {
            nameOrigin = SportTypeHelper.Instance.ExcludeSportTypeFromList(nameOrigin);
            nameOrigin = GenderDetectorHelper.Instance.ExcludGenderTypeFromList(nameOrigin);
            int digit;
            nameOrigin = nameOrigin
                .Where(s => !int.TryParse(s, out digit))
                .Select(name => {
                    _badSymbols.Each(ch => {
                        name = name.Replace(ch.ToString(), string.Empty);
                    });
                    return name;
                })
                .ToArray();
            return nameOrigin;
        } 

        public CompetitionItemRawTransport GetCompetitionItem(ProcessStat competitorStat, BrokerType brokerType, RawTemplateObj<CompetitorParsedTransport>[] competitors, RawTemplateObj<CompetitionSpecifyTransport> competitionSpecifyTransport, DateTime eventDateUtc, GatherBehaviorMode algoMode) {
            return InvokeSafeSingleCall(() => {
                var utcNow = DateTime.UtcNow;
                if (eventDateUtc > utcNow.AddDays(14)) {
                    return null;
                }
                var competitionItemRaw = new BrokerEntityBuilder<RawCompetitionItem>(competitorStat)
                    .SetupValidateObject(item => item.CompetitionitemID != default(int))
                    .SetupGetRaw(() => RawCompetitionItemHelper.GetCompetitionItem(brokerType, competitors, competitionSpecifyTransport, eventDateUtc, utcNow))
                    .SetupCreateRaw(() => RawCompetitionItemHelper.CreateCompetitionItem(brokerType, competitors, competitionSpecifyTransport, eventDateUtc, utcNow))
                    .SetupTryMatchRaw(GatherBehaviorMode.TryDetectAll, item => {
                        var ci = CompetitionItem.DataSource
                            .Where(new DaoFilterOr(
                                new DaoFilterAnd(
                                    new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitors[0].Object.ID),
                                    new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitors[1].Object.ID)
                                    ),
                                new DaoFilterAnd(
                                    new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid2, competitors[0].Object.ID),
                                    new DaoFilterEq(CompetitionItem.Fields.Competitoruniqueid1, competitors[1].Object.ID)
                                    )
                                ))
                            .WhereEquals(CompetitionItem.Fields.Sporttype, (short)competitionSpecifyTransport.Object.SportType)
                            .WhereEquals(CompetitionItem.Fields.CompetitionuniqueID, competitionSpecifyTransport.Object.CompetitionUniqueID)
                            .Where(new DaoFilterOr(
                                new DaoFilterNull(CompetitionItem.Fields.CompetitionSpecifyUniqueID, true),
                                new DaoFilterEq(CompetitionItem.Fields.CompetitionSpecifyUniqueID, competitionSpecifyTransport.Object.ID)))
                            .Where(eventDateUtc > DateTime.MinValue
                                ? new DaoFilterAnd(
                                    new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, eventDateUtc.AddDays(-1.5)),
                                    new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.LessOrEq, eventDateUtc.AddDays(1.5))
                                )
                                : new DaoFilterAnd(
                                    new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.GreaterOrEq, utcNow.AddDays(-1.5)),
                                    new DaoFilter(CompetitionItem.Fields.Dateeventutc, Oper.LessOrEq, utcNow.AddDays(1.5))
                                )
                            )
                            .Sort(CompetitionItem.Fields.ID, SortDirection.Desc)
                            .First(CompetitionItem.Fields.ID);
                        if (ci != null) {
                            item.CompetitionitemID = ci.ID;
                            item.Linkstatus = LinkEntityStatus.LinkByStatistics;
                        }
                        return item;
                    })
                    .SetupCreateOriginal(algoMode, item => {
                        CreateCompetitionItem(competitors, competitionSpecifyTransport, eventDateUtc, utcNow, item);
                        return item;
                    })
                    .SetupFinally(item => {
                        if (algoMode.HasFlag(GatherBehaviorMode.CreateOriginalIfMatchedAll) && item.CompetitionitemID == default(int) &&
                                                                                                  competitionSpecifyTransport.Object.CompetitionUniqueID != default(int) &&
                                                                                                  //NOTE competitionSpecifyTransport.Object.CompetitionSpecifyUniqueID != default(int) &&
                                                                                                  competitors[0].Object.ID != default(int) &&
                                                                                                  competitors[1].Object.ID != default(int)) {
                            CreateCompetitionItem(competitors, competitionSpecifyTransport, eventDateUtc, utcNow, item);
                        }
                        item.Save();
                        return item;
                    })
                    .MakeObject();
                if (competitionItemRaw.CompetitionitemID == default(int)) {
                    return null;
                }
                var competitionItem = CompetitionItem.DataSource.GetByKey(competitionItemRaw.CompetitionitemID, CompetitionItem.Fields.ID, CompetitionItem.Fields.Dateeventutc, CompetitionItem.Fields.Competitoruniqueid1, CompetitionItem.Fields.Competitoruniqueid2, CompetitionItem.Fields.CompetitionSpecifyUniqueID);
                if (competitionSpecifyTransport.Object.ID != default(int) && competitionItem.CompetitionSpecifyUniqueID == default(int)) {
                    competitionItem.CompetitionSpecifyUniqueID = competitionSpecifyTransport.Object.ID;
                    competitionItem.Save();
                }
                
                if (eventDateUtc > DateTime.MinValue && algoMode.HasFlag(GatherBehaviorMode.CreateOriginal)) {
                    if (Math.Abs((competitionItem.Dateeventutc - eventDateUtc).TotalMinutes) > 5) {
                        competitionItem.Dateeventutc = eventDateUtc;
                        competitionItem.Save();
                    }
                }
                
                return new CompetitionItemRawTransport {
                    CompetitionItemID = competitionItem.Competitoruniqueid1 == competitors[0].Object.ID &&
                                           competitionItem.Competitoruniqueid2 == competitors[1].Object.ID
                                    ? competitionItem.ID
                                    : -competitionItem.ID,
                    RawCompetitionItemID = competitionItemRaw.ID
                };
            }, null);
        }

        private static void CreateCompetitionItem(RawTemplateObj<CompetitorParsedTransport>[] competitors, RawTemplateObj<CompetitionSpecifyTransport> competitionSpecifyTransport, DateTime eventDateUtc,
            DateTime utcNow, RawCompetitionItem item) {
            var ci = new CompetitionItem {
                SportType = competitionSpecifyTransport.Object.SportType,
                Datecreatedutc = utcNow,
                Dateeventutc = eventDateUtc != DateTime.MinValue ? eventDateUtc : utcNow,
                CompetitionuniqueID = competitionSpecifyTransport.Object.CompetitionUniqueID,
                //CompetitionSpecifyUniqueID = competitionSpecifyTransport.Object.CompetitionSpecifyUniqueID,
                Competitoruniqueid1 = competitors[0].Object.ID,
                Competitoruniqueid2 = competitors[1].Object.ID
            };
            if (competitionSpecifyTransport.Object.ID != default(int)) {
                ci.CompetitionSpecifyUniqueID = competitionSpecifyTransport.Object.ID;
            }
            ci.Save();
            item.Linkstatus = LinkEntityStatus.Original;
            item.CompetitionitemID = ci.ID;
        }
    }
}