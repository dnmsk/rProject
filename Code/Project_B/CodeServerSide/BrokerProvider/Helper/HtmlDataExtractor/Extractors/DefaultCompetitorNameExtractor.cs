using System;
using System.Linq;
using CommonUtils.ExtendedTypes;
using HtmlAgilityPack;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors {
    public class DefaultCompetitorNameExtractor<T> : DefaultExtractor<Tuple<string[], string[]>, T> {
        //TODO REMOVE FROM HERE
        public static char[] TrimmedChars { get; } = {' ', '.', ',', '\r', '\n', '(', ')'};
        private readonly Func<T, object> _customExtractor;
        public DefaultCompetitorNameExtractor(BrokerConfiguration brokerConfiguration, Func<T, object> customExtractor = null) : base(brokerConfiguration) {
            _customExtractor = customExtractor;
        }
        
        protected Tuple<string[], string[]> SplitToCompetitors(string competitorsRow) {
            if (competitorsRow.IsNullOrWhiteSpace()) {
                return null;
            }
            if ((BrokerConfiguration.StringArray[SectionName.ArrayBadNameCompetitor] ?? ConfigurationContainer.Instance.BrokerConfiguration[BrokerType.Default].StringArray[SectionName.ArrayBadNameCompetitor])
                .Any(bad => competitorsRow.Contains(bad, StringComparison.InvariantCultureIgnoreCase))) {
                return null;
            }
            var splitted = competitorsRow.Split(BrokerConfiguration.StringArray[SectionName.ArrayCompetitorSplitter], StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length <= 1) {
                return null;
            }
            var trimmedChars = TrimmedChars;
            if (splitted.Length == 2) {
                return new Tuple<string[], string[]>(new[] { splitted[0].Trim(trimmedChars) }, new[] { splitted[1].Trim(trimmedChars) });
            }
            var idx1 = 0;
            var idx2 = splitted[splitted.Length - 1].Length > 3 ? splitted.Length - 1 : splitted.Length - 2;
            return new Tuple<string[], string[]>(new[] { splitted[idx1].Trim(trimmedChars) }, new[] { splitted[idx2].Trim(trimmedChars) });
        }

        protected override void SetToMatchParsed(MatchParsed matchParsed, T container) {
            var data = ExtractData(container, matchParsed.SportType);
            if (data == null) {
                throw new Exception(BrokerConfiguration.BrokerType + ": " + (TryGetDataFromHtmlNode(container) ?? "NULL"));
            }
            matchParsed.CompetitorName1 = data.Item1;
            matchParsed.CompetitorName2 = data.Item2;
        }

        public override Tuple<string[], string[]> ExtractData(T container, SportType sportType, Func<string, Tuple<string[], string[]>> customCreator = null) {
            var data = _customExtractor?.Invoke(container) ?? container;
            data = TryGetDataFromHtmlNode(data);
            if (data is string) {
                return SplitToCompetitors(data as string);
            }
            if (data is string[]) {
                var obj = data as string[];
                return new Tuple<string[], string[]>(new[] { obj[0] }, new[] { obj[1] });
            }
            if (data is Tuple<string[], string[]>) {
                return data as Tuple<string[], string[]>;
            }
            if (data is string[][]) {
                var obj = data as string[][];
                return new Tuple<string[], string[]>(obj[0], obj[1]);
            }
            return null;
        }

        private object TryGetDataFromHtmlNode(object data) {
            if (data is HtmlNode) {
                data = HtmlBlockHelper.ExtractBlock((HtmlNode) data, BrokerConfiguration.XPath[SectionName.XPathToCompetitors])[0].InnerText;
            }
            return data;
        }
    }
}