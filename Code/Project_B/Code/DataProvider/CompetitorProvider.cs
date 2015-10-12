using System;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO.Filters;
using Project_B.Code.DataProvider.Transport;
using Project_B.Code.Entity;
using Project_B.Code.Enums;

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
                        new DaoFilter(Competitor.Fields.NameShort, Oper.Ilike, nameShort), 
                        new DaoFilter(Competitor.Fields.NameFull, Oper.Ilike, nameFull))
                    )
                    .AsList();
                Competitor competitor = null;
                if (competitors.Count > 1) {
                    _logger.Error("{0} Competitors for nameShort='{1}' and nameFull='{2}', sport={3}, gender={4}. Take first", competitors.Count, nameShort, nameFull, sportType, genderType);
                    competitor = competitors[0];
                } else if (competitors.Count == 1) {
                    competitor = competitors[0];
                }
                if (competitor != null && nameFull.Length > nameShort.Length && competitor.NameFull.Length == competitor.NameShort.Length && competitor.NameShort.Equals(nameShort, StringComparison.InvariantCultureIgnoreCase)) {
                    competitor.NameFull = nameFull;
                    competitor.Save();
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
                }
                return new CompetitorTransport {
                    UniqueID = competitor.CompetitoruniqueID,
                    NameFull = competitor.NameFull,
                    NameShort = competitor.NameShort,
                    GenderType = genderType,
                    SportType = sportType,
                    LanguageType = languageType,
                    DateCreatedUtc = competitor.Datecreatedutc
                };
            }, null);
        }
    }
}