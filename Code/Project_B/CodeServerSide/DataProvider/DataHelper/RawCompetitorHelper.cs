using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Entity.Helper;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class RawCompetitorHelper {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (RawCompetitorHelper).FullName);

        public static List<RawCompetitor> GetRawCompetitor(BrokerType brokerType, LanguageType languageType, SportType sportType, GenderType genderType, string[] names) {
            var competitorsRaw = RawCompetitor.DataSource.FilterByLanguage(languageType).FilterBySportType(sportType).FilterByBroker(brokerType)
                                                .FilterByNameCompetitor(names)
                                                .FilterByGender(genderType, 
                        RawCompetitor.Fields.CompetitoruniqueID,
                        RawCompetitor.Fields.Name,
                        RawCompetitor.Fields.Linkstatus);
            if (competitorsRaw.Count > 1) {
                var groupBy = competitorsRaw.Where(c => c.CompetitoruniqueID != default(int)).GroupBy(c => c.CompetitoruniqueID).ToArray();
                if (groupBy.Length > 1) {
                    _logger.Error("{0} {1} {2} {3} {4} <=> {5}", brokerType, sportType, genderType, 
                        competitorsRaw.Select(cr => cr.ID).StrJoin(", "), names.StrJoin(", "), groupBy.Select(g => g.SelectMany(ge => ge.Name)).StrJoin(", "));
                    return groupBy.First().ToList();
                }
                if (groupBy.Length == 1) {
                    foreach (var rawCompetitor in competitorsRaw.Where(cr => cr.CompetitoruniqueID == default(int))) {
                        rawCompetitor.CompetitoruniqueID = groupBy[0].Key;
                        rawCompetitor.Save();
                    }
                }
            }
            return CreateRawCompetitor(names, competitorsRaw, brokerType, languageType, sportType, genderType);
        }

        public static List<RawCompetitor> CreateRawCompetitor(string[] names, List<RawCompetitor> competitorsRaw, BrokerType brokerType, LanguageType languageType, SportType sportType, GenderType genderType) {
            var existNames = competitorsRaw.Select(cr => cr.Name).ToList();
            names = names
                .Where(name => !existNames.Contains(name))
                .ToArray();
            if (names.Any()) {
                var lastCompetitorUniqueID = competitorsRaw.Any()
                    ? competitorsRaw.First().CompetitoruniqueID
                    : default(int);
                var raw = competitorsRaw;
                names
                    .Where(name => !raw.Any(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                    .Each(name => {
                        var competitorRaw = BrokerEntityIfaceCreator.CreateEntity<RawCompetitor>(brokerType, languageType, sportType, genderType, LinkEntityStatus.Unlinked, new[] {name},
                            competitor => {
                                if (lastCompetitorUniqueID != default(int)) {
                                    competitor.CompetitoruniqueID = lastCompetitorUniqueID;
                                    competitor.Linkstatus = LinkEntityStatus.LinkByStatistics | LinkEntityStatus.Linked;
                                }
                            });
                        competitorRaw.Save();
                        raw.Add(competitorRaw);
                    });
            }

            return competitorsRaw;
        }
    }
}