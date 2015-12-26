using System;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class CompetitorProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitorProvider).FullName);

        private readonly char[] _trimChars = { '(', ')', ' ', '.', '\'', ';', ':', '-' };

        public CompetitorProvider() : base(_logger) { }

        public CompetitorParsedTransport GetCompetitor(BrokerType brokerType, LanguageType languageType, SportType sportType, GenderType genderType, string nameFull, string nameShort, int competitionUnique, MatchParsed matchParsed, GatherBehaviorMode algoMode) {
            return InvokeSafeSingleCall(() => {
                if (nameFull.IsNullOrWhiteSpace() && nameShort.IsNullOrWhiteSpace()) {
                    throw new Exception("nameFull.IsNullOrWhiteSpace() && nameShort.IsNullOrWhiteSpace()");
                }
                if (nameFull.IsNullOrWhiteSpace()) {
                    nameFull = nameShort.Trim();
                }
                if (nameShort.IsNullOrWhiteSpace()) {
                    nameShort = nameFull.Trim();
                }
                nameFull = nameFull.Trim(_trimChars);
                nameShort = nameShort.Trim(_trimChars);

                var competitorFromRaw = RawCompetitorHelper.GetCompetitor(brokerType, languageType, sportType, genderType, nameShort, nameFull);
                if (competitorFromRaw == null) {
                    return null;
                }

                if (!competitorFromRaw.Any() || competitorFromRaw.All(c => c.CompetitoruniqueID == default(int))) {
                    competitorFromRaw = RawCompetitorHelper.CreateCompetitorAndDetect(brokerType, languageType, sportType, genderType, nameShort, nameFull, competitionUnique, matchParsed, algoMode);
                }
                var firstRow = competitorFromRaw.First(c => c.CompetitoruniqueID != default(int));
                return new CompetitorParsedTransport {
                    RawID = firstRow.ID,
                    UniqueID = firstRow.CompetitoruniqueID,
                    Name = firstRow.Name,
                    GenderType = genderType,
                    SportType = sportType,
                    LanguageType = languageType
                };
            }, null);
        }
    }
}