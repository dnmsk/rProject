using System;
using System.Collections.Generic;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors {
    public class DefaultOddsExtractor<T> : DefaultExtractor<List<OddParsed>, T> {
        private readonly Func<T, SportType, List<OddParsed>> _customParser;

        public DefaultOddsExtractor(BrokerConfiguration brokerConfiguration, Func<T, SportType, List<OddParsed>> customParser = null) : base(brokerConfiguration) {
            _customParser = customParser;
        }

        protected override void SetToMatchParsed(MatchParsed matchParsed, T container) {
            matchParsed.Odds.AddRange(ExtractData(container, matchParsed.SportType));
        }

        public override List<OddParsed> ExtractData(T container, SportType sportType, Func<string, List<OddParsed>> customCreator = null) {
            if (_customParser != null) {
                return _customParser(container, sportType);
            }
            return null;
        }
    }
}