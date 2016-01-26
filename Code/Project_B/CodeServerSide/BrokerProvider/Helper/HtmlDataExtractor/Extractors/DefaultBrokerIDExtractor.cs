using System;
using System.Collections;
using CommonUtils.Code;
using HtmlAgilityPack;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors {
    public class DefaultBrokerIDExtractor<T> : DefaultExtractor<int, T> {
        private readonly Func<T, object> _customExtractor;
        public DefaultBrokerIDExtractor(BrokerConfiguration brokerConfiguration, Func<T, object> customExtractor = null) : base(brokerConfiguration) {
            _customExtractor = customExtractor;
        }

        protected override void SetToMatchParsed(MatchParsed matchParsed, T container) {
            matchParsed.BrokerMatchID = ExtractData(container, matchParsed.SportType);
        }

        protected override int ExtractData(T container, SportType sportType, Func<string, int> customCreator = null) {
            var obj = _customExtractor?.Invoke(container) ?? container;
            if (obj is int) {
                return (int) obj;
            }
            if (obj is string) {
                return StringParser.ToInt(obj as string, default(int));
            }
            if (container is HtmlNode) {
                return StringParser.ToInt((container as HtmlNode).Attributes[BrokerConfiguration.StringSimple[SectionName.StringOddBrokerID]]?.Value, default(int));
            }
            if (container is IDictionary) {
                var asMap = container as IDictionary;
                if (asMap.Contains(BrokerConfiguration.StringSimple[SectionName.StringOddBrokerID])) {
                    return StringParser.ToInt(asMap[BrokerConfiguration.StringSimple[SectionName.StringOddBrokerID]]?.ToString(), default(int));
                }
            }
            return default(int);
        }
    }
}