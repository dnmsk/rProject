using System;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO.Filters;
using Project_B.Code.DataProvider.DataHelper;
using Project_B.Code.DataProvider.Transport;
using Project_B.Code.Entity;
using Project_B.Code.Enums;
using Project_B.Models;

namespace Project_B.Code.DataProvider {
    public class CompetitorProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitorProvider).FullName);

        public CompetitorProvider() : base(_logger) { }

        public CompetitorTransport GetCompetitor(LanguageType languageType, SportType sportType, GenderType genderType, string nameFull, string nameShort) {
            return InvokeSafeSingleCall(() => {
                nameFull = (nameFull ?? nameShort).Trim();
                nameShort = (nameShort ?? nameFull).Trim();
                var competitors = Competitor.DataSource
                    .WhereEquals(Competitor.Fields.Gendertype, (short)genderType)
                    .WhereEquals(Competitor.Fields.Languagetype, (short)languageType)
                    .WhereEquals(Competitor.Fields.Sporttype, (short) sportType)
                    .Where(new DaoFilterOr(
                        QueryHelper.GetIndexedFilterByWordIgnoreCase(nameShort.ToLower(), Competitor.Fields.NameShort),
                        QueryHelper.GetIndexedFilterByWordIgnoreCase(nameFull.ToLower(), Competitor.Fields.NameFull))
                    )
                    .AsList(
                        Competitor.Fields.CompetitoruniqueID,
                        Competitor.Fields.NameFull,
                        Competitor.Fields.NameShort
                    );
                Competitor competitor = null;
                if (competitors.Count > 1) {
                    _logger.Error("{0} Competitors for nameShort='{1}' and nameFull='{2}', sport={3}, gender={4}. Take first", competitors.Count, nameShort, nameFull, sportType, genderType);
                    competitor = competitors[0];
                } else if (competitors.Count == 1) {
                    competitor = competitors[0];
                }
                if (competitor == null) {
                    var uniqueID = new CompetitorUnique {
                        IsUsed = true
                    };
                    uniqueID.Save();
                    competitor = new Competitor {
                        CompetitoruniqueID = uniqueID.ID,
                        SportType = sportType,
                        Datecreatedutc = DateTime.UtcNow,
                        Languagetype = languageType,
                        NameFull = nameFull,
                        NameShort = nameShort,
                        Gendertype = genderType
                    };
                    competitor.Save();
                } else {
                    if (nameFull.Length != nameShort.Length) {
                        if (competitor.NameShort.Equals(nameShort, StringComparison.InvariantCultureIgnoreCase)) {
                            competitor.NameFull = nameFull;
                        } else if (competitor.NameFull.Equals(nameFull, StringComparison.InvariantCultureIgnoreCase)) {
                            competitor.NameShort = nameShort;
                        }
                    }
                    if (competitor.Changes.Count > 0) {
                        competitor.Save();
                    }
                }
                return new CompetitorTransport {
                    UniqueID = competitor.CompetitoruniqueID,
                    NameFull = competitor.NameFull,
                    NameShort = competitor.NameShort,
                    GenderType = genderType,
                    SportType = sportType,
                    LanguageType = languageType
                };
            }, null);
        }

        public CompetitorModel GetCompetitorModel(int competitorID) {
            return InvokeSafe(() => {
                var competitor = Competitor.DataSource.GetByKey(competitorID, Competitor.Fields.NameFull);
                return new CompetitorModel {
                    Name = competitor.NameFull,
                    ID = competitorID
                };
            }, null);
        }
    }
}