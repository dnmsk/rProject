using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.DataProvider.DataHelper.RawData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class CompetitorProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitorProvider).FullName);

        private readonly char[] _trimChars = { '(', ')', ' ', '.', '\'', ';', ':', '-' };

        public CompetitorProvider() : base(_logger) { }

        public RawTemplateObj<CompetitorParsedTransport> GetCompetitor(ProcessStat competitorStat, BrokerType brokerType, LanguageType languageType, SportType sportType, GenderType genderType, string[] names, int competitionUnique, MatchParsed matchParsed, GatherBehaviorMode algoMode) {
            return InvokeSafeSingleCall(() => {
                names = names
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => {
                        var indexOf = name.IndexOf("(", StringComparison.InvariantCultureIgnoreCase);
                        return (indexOf > 0 ? name.Substring(0, indexOf) : name).Trim(_trimChars);
                    })
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToArray();
                if (!names.Any()) {
                    throw new Exception("nameFull.IsNullOrWhiteSpace() && nameShort.IsNullOrWhiteSpace()");
                }
                var competitors = new BrokerEntityBuilder<List<RawCompetitor>>(competitorStat)
                    .SetupValidateObject(competitorsRaw => competitorsRaw.SafeAny() && competitorsRaw.All(c => c.CompetitoruniqueID != default(int)))
                    .SetupGetRaw(() => RawCompetitorHelper.GetRawCompetitor(brokerType, languageType, sportType, genderType, names))
                    .SetupTryMatchRaw(algoMode, crs => RawCompetitorHelper.DetectCompetitor(names, competitionUnique, matchParsed, crs))
                    .SetupCreateOriginal(algoMode, list => {
                        var uniqueID = new CompetitorUnique {
                            IsUsed = true
                        };
                        uniqueID.Save();
                        var competitor = new Competitor {
                            CompetitoruniqueID = uniqueID.ID,
                            SportType = sportType,
                            Datecreatedutc = DateTime.UtcNow,
                            Languagetype = languageType,
                            Name = names[0],
                            Gendertype = genderType
                        };
                        competitor.Save();
                        list.Each(el => el.Linkstatus = LinkEntityStatus.Original | LinkEntityStatus.Linked);
                        return list;
                    })
                    .SetupFinally(list => {
                        var firstElement = list.First();
                        if (algoMode.HasFlag(GatherBehaviorMode.CreateNewLanguageName) && firstElement.CompetitoruniqueID != default(int) && !Competitor.DataSource
                                    .WhereEquals(Competitor.Fields.CompetitoruniqueID, firstElement.CompetitoruniqueID)
                                    .WhereEquals(Competitor.Fields.Languagetype, (short)languageType)
                                    .IsExists()) {
                            new Competitor {
                                CompetitoruniqueID = firstElement.CompetitoruniqueID,
                                SportType = sportType,
                                Datecreatedutc = DateTime.UtcNow,
                                Languagetype = languageType,
                                Name = names[0],
                                Gendertype = genderType
                            }.Save();
                        }
                        list.Each(el => el.Save());
                        return list;
                    })
                    .MakeObject();
                return competitors.Any()
                    ? competitors.Select(c => new RawTemplateObj<CompetitorParsedTransport> {
                        RawObject = {
                            ID = c.ID
                        },
                        Object = {
                            LanguageType = languageType,
                            SportType = sportType,
                            GenderType = genderType,
                            ID = c.CompetitoruniqueID
                        }
                    }).First()
                    : new RawTemplateObj<CompetitorParsedTransport>();
            }, new RawTemplateObj<CompetitorParsedTransport>());
        }
    }
}