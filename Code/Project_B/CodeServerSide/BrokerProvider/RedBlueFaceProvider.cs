using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonUtils.Code;
using HtmlAgilityPack;
using Project_B.CodeServerSide.BrokerProvider.Common;
using Project_B.CodeServerSide.BrokerProvider.Helper;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors;
using Project_B.CodeServerSide.BrokerProvider.Interfaces;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public sealed class RedBlueFaceProvider : BrokerBase<WrDnmskWrapper> {
        private static readonly Regex _numberExtractorRegex = new Regex(".*?\\((?<number>.*?)\\).*?", RegexOptions.Compiled);
        private static readonly Dictionary<SportType, BetOddType[]> _typeWithOrderInLine = new Dictionary<SportType, BetOddType[]> {
            { SportType.Football, new[] {BetOddType.Win1, BetOddType.Draw, BetOddType.Win2, BetOddType.Win1Draw, BetOddType.Win1Win2, BetOddType.DrawWin2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.Tennis, new[] {BetOddType.Win1, BetOddType.Win2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.Volleyball, new[] {BetOddType.Win1, BetOddType.Win2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.Basketball, new[] {BetOddType.Win1, BetOddType.Win2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.IceHockey, new[] {BetOddType.Win1, BetOddType.Draw, BetOddType.Win2, BetOddType.Win1Draw, BetOddType.Win1Win2, BetOddType.DrawWin2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
        };

        public RedBlueFaceProvider(IQueryableWrapper requestHelper) : base(requestHelper) { }

        public override BrokerType BrokerType => BrokerType.RedBlue;

        public override BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language) {
            var url = FormatUrl(SectionName.UrlResultTarget, new {
                query = CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsResultsParam]
                        .GetParamValueForCompetition(sportType, CurrentConfiguration.StringSimple[SectionName.StringMapStringsResultsParamJoin]),
                date = date.ToString(CurrentConfiguration.StringSimple[SectionName.StringDateQueryFormat]),
                lang = GetLanguageParam(language)
            });
            var loadPage = LoadPage(url);
            var extractors = GetDefaultExtractors();
            var data = new HtmlBlockHelper(loadPage).GetCurrentNode()
                .NodeToNodeCompetitionGroupedBySport(CurrentConfiguration)
                .SelectMany(node => node.Node.NodeToNodeCompetitionWithMatches(CurrentConfiguration, SportTypeExtractor, SectionName.XPathToEventResult)
                    .Select(tuple => tuple.NodeToCompetitionParsed(extractors.Union(new DefaultExtractor<HtmlNode>[] {
                        GetShortCompetitorNameExtractor(),
                        new DefaultDateUtcExtractor<HtmlNode>(CurrentConfiguration, GetGmtFixer(loadPage))
                    }).ToArray()))
                )
                .ToList();
            return new BrokerData(BrokerType, language, data);
        }

        public override BrokerData LoadLive(SportType sportType, LanguageType language) {
            var url = FormatUrl(SectionName.UrlLiveTarget, new {
                lang = GetLanguageParam(language)
            });
            var loadPage = LoadPage(url);
            var extractors = GetDefaultExtractors();
            var baseNode = new HtmlBlockHelper(loadPage).GetCurrentNode();
            var data = baseNode
                .NodeToNodeCompetitionGroupedBySport(CurrentConfiguration)
                .SelectMany(node => node.Node.NodeToNodeCompetitionWithMatches(CurrentConfiguration, SportTypeExtractor, SectionName.XPathToEventInList)
                    .Select(tuple => {
                        var nodeToCompetitionParsed = tuple.NodeToCompetitionParsed(extractors.Union(new[] { GetFullNameExtractor() }).ToArray());
                        return nodeToCompetitionParsed;
                    })
                ).Union(baseNode.NodeToNodeCompetitionGroupedBySport(CurrentConfiguration)
                    .SelectMany(node => node.Node.NodeToNodeCompetitionWithMatches(CurrentConfiguration, SportTypeExtractor, SectionName.XPathToEventResult)
                        .Select(tuple => tuple.NodeToCompetitionParsed(extractors.Union(new DefaultExtractor<HtmlNode>[] {
                            GetShortCompetitorNameExtractor()
                        }).ToArray()))
                    ))
                .ToList();
            return new BrokerData(BrokerType, language, data);
        }

        public override BrokerData LoadRegular(SportType sportType, LanguageType language) {
            var url = FormatUrl(SectionName.UrlOddsTarget, new {
                param = CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsOddsParam]
                            .GetParamValueForCompetition(sportType, CurrentConfiguration.StringSimple[SectionName.StringMapStringsOddsParamJoin]),
                lang = GetLanguageParam(language)
            });
            var loadPage = LoadPage(url);
            var extractors = GetDefaultExtractors();
            var data = new HtmlBlockHelper(loadPage).GetCurrentNode()
                .NodeToNodeCompetitionGroupedBySport(CurrentConfiguration)
                .SelectMany(node => node.Node.NodeToNodeCompetitionWithMatches(CurrentConfiguration, SportTypeExtractor, SectionName.XPathToEventInList)
                    .Select(tuple => tuple.NodeToCompetitionParsed(extractors.Union(new DefaultExtractor<HtmlNode>[] {
                        GetFullNameExtractor(),
                        new DefaultDateUtcExtractor<HtmlNode>(CurrentConfiguration, GetGmtFixer(loadPage))
                    }).ToArray()))
                )
                .ToList();
            return new BrokerData(BrokerType, language, data);
        }

        private SportType SportTypeExtractor(HtmlNode htmlNode) {
            var nameBlock = HtmlBlockHelper.ExtractBlock(htmlNode, CurrentConfiguration.XPath[SectionName.XPathToCategoryName]);
            if (nameBlock.Count > 0) {
                return SportTypeHelper.Instance[nameBlock[0].InnerText.Split('.')];
            }
            return SportType.Unknown;
        }

        private DefaultExtractor<HtmlNode>[] GetDefaultExtractors() {
            return new DefaultExtractor<HtmlNode>[] {
                new DefaultBrokerIDExtractor<HtmlNode>(CurrentConfiguration),
                new DefaultResultExtractor<HtmlNode>(CurrentConfiguration), 
                new DefaultOddsExtractor<HtmlNode>(CurrentConfiguration, ExtractOddsFromMatchBlock), 
            };
        }

        private DefaultCompetitorNameExtractor<HtmlNode> GetFullNameExtractor() {
            return new DefaultCompetitorNameExtractor<HtmlNode>(CurrentConfiguration, node => {
                var competitorsShortName = node.Attributes[CurrentConfiguration.StringSimple[SectionName.StringOddCompetitorsShortName]].Value
                    .Split(CurrentConfiguration.StringArray[SectionName.ArrayCompetitorSplitter], StringSplitOptions.RemoveEmptyEntries);
                var competitorsFullName = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToOddsCompetitors]);
                return Tuple.Create(new[] { competitorsFullName[0].InnerText.Trim(), competitorsShortName[0].Trim() }, new[] { competitorsFullName[1].InnerText.Trim(), competitorsShortName[1].Trim() });
            });
        }

        private DefaultCompetitorNameExtractor<HtmlNode> GetShortCompetitorNameExtractor() {
            return new DefaultCompetitorNameExtractor<HtmlNode>(CurrentConfiguration);
        }
        
        private static DateTimeToGmtFixer GetGmtFixer(string htmlContent) {
            var idx = htmlContent.IndexOf("\"gmt", StringComparison.InvariantCultureIgnoreCase);
            var num = htmlContent.Substring(idx + 4);
            num = num.Substring(0, num.IndexOf("\"", StringComparison.InvariantCultureIgnoreCase));
            return new DateTimeToGmtFixer(StringParser.ToShort(num, 0));
        }
        
        private List<OddParsed> ExtractOddsFromMatchBlock(HtmlNode node, SportType sportType) {
            var odds = new List<OddParsed>();
            var betBlocks = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToOddsFactor]);
            foreach (var betBlock in betBlocks) {
                var attrValue = betBlock.Attributes["data-sel"];
                if (attrValue == null) {
                    odds.Add(new OddParsed());
                    continue;
                }
                var dataDict = JavaScriptSerializer.Deserialize<Dictionary<string, object>>(ReplaceHtmlLiterals(attrValue.Value));
                var prices = ((Dictionary<string, object>)dataDict["prices"]);
                string confirmation = null;
                var oddsConfirmation = HtmlBlockHelper.ExtractBlock(betBlock, CurrentConfiguration.XPath[SectionName.XPathToOddsConfirmation]);
                if (oddsConfirmation.Count > 0) {
                    confirmation = oddsConfirmation[0]
                        .Attributes[CurrentConfiguration.StringSimple[SectionName.StringOddConfirmation]]
                        .Value;
                }
                float? advancedParam = null;
                var numberMatched = _numberExtractorRegex.Match(betBlock.InnerText);
                if (numberMatched.Success && numberMatched.Groups.Count > 0) {
                    advancedParam = StringParser.ToFloat(numberMatched.Groups["number"].Value);
                }
                var odd = new OddParsed {
                    Factor = StringParser.ToFloat(prices["1"].ToString()),
                    AdvancedParam = advancedParam,
                    OddStringConfirmation = confirmation
                };
                odds.Add(odd);
            }
            TryMapOddTypes(sportType, odds);
            return odds;
        }

        private void TryMapOddTypes(SportType sportType, List<OddParsed> odds) {
            BetOddType[] oddTypesForCompetition;
            if (!_typeWithOrderInLine.TryGetValue(sportType, out oddTypesForCompetition)) {
                return;
            }
            if (odds.Count == oddTypesForCompetition.Length) {
                for (var i = 0; i < oddTypesForCompetition.Length; i++) {
                    odds[i].Type = oddTypesForCompetition[i];
                }
            }
        }

        private static string ReplaceHtmlLiterals(string text) {
            return text
                .Replace("&nbsp;", " ")
                .Replace("&quot;", "\"");
        }
    }
}