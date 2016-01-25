using System;
using System.Linq;
using HtmlAgilityPack;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors {
    public class DefaultResultExtractor<T> : DefaultExtractor<FullResult, T> {
        private readonly Func<T, SportType, object> _customExtractor;

        public DefaultResultExtractor(BrokerConfiguration brokerConfiguration, Func<T, SportType, object> customExtractor = null) : base(brokerConfiguration) {
            _customExtractor = customExtractor;
        }

        protected override void SetToMatchParsed(MatchParsed matchParsed, T container) {
            matchParsed.Result = ExtractData(container, matchParsed.SportType, s => ResultBuilder.BuildResultFromString(matchParsed.SportType, s));
        }

        public override FullResult ExtractData(T arg, SportType sportType, Func<string, FullResult> customCreator) {
            var obj = _customExtractor?.Invoke(arg, sportType) ?? arg;
            if (obj is string) {
                return customCreator(arg as string);
            }
            if (obj is HtmlNode) {
                var resultBlock = HtmlBlockHelper.ExtractBlock(obj as HtmlNode, BrokerConfiguration.XPath[SectionName.XPathToResultValue]);
                if (resultBlock.Any()) {
                    return customCreator(resultBlock[0].InnerText);
                }
                return null;
            }
            return null;
        }
    }
}