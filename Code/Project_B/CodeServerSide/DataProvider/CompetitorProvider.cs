using System;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.DataHelper.RawData;
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

        public RawTemplateObj<CompetitorParsedTransport> GetCompetitor(BrokerType brokerType, LanguageType languageType, SportType sportType, GenderType genderType, string[] names, int competitionUnique, MatchParsed matchParsed, GatherBehaviorMode algoMode) {
            return InvokeSafeSingleCall(() => {
                names = names
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => name.Trim(_trimChars))
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToArray();
                if (!names.Any()) {
                    throw new Exception("nameFull.IsNullOrWhiteSpace() && nameShort.IsNullOrWhiteSpace()");
                }
                var competitorFromRaw = RawCompetitorHelper.GetCompetitor(brokerType, languageType, sportType, genderType, names, competitionUnique, matchParsed, algoMode);
                return competitorFromRaw?.FirstOrDefault() ?? new RawTemplateObj<CompetitorParsedTransport>();
            }, new RawTemplateObj<CompetitorParsedTransport>());
        }
    }
}