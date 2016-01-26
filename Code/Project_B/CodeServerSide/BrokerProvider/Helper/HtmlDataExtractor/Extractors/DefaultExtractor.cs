using System;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors {
    public abstract class DefaultExtractor<K> {
        protected readonly BrokerConfiguration BrokerConfiguration;

        protected DefaultExtractor(BrokerConfiguration brokerConfiguration) {
            BrokerConfiguration = brokerConfiguration;
        }

        protected abstract void SetToMatchParsed(MatchParsed matchParsed, K container);

        public static MatchParsed CreateMatchParsed<T>(SportType sportType, T container, params DefaultExtractor<T>[] extractors) {
            var match = new MatchParsed(sportType);
            foreach (var defaultExtractor in extractors) {
                defaultExtractor.SetToMatchParsed(match, container);
            }
            return match;
        }
    }

    public abstract class DefaultExtractor<T, K> : DefaultExtractor<K> {
        protected DefaultExtractor(BrokerConfiguration brokerConfiguration) : base(brokerConfiguration) {}
        protected abstract T ExtractData(K container, SportType sportType, Func<string, T> customCreator = null);
    }
}