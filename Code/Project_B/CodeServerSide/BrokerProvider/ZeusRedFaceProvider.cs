using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
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
    public class ZeusRedFaceProvider : BrokerBase<WebRequestWrapper> {
        public ZeusRedFaceProvider(IQueryableWrapper requestHelper) : base(requestHelper) {
        }

        public override BrokerType BrokerType => BrokerType.ZeusRed;
        public override BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language) {
            var url = CurrentConfiguration.StringSimple[SectionName.UrlResultTarget].HaackFormatSafe(new {
                day = date.Day,
                month = date.Month,
                year = date.Year
            });
            var content = LoadPage(url);
            var blockHelper = new HtmlBlockHelper(content);
            var rows = blockHelper.ExtractBlock(new XPathQuery(".//table[@class='smallwnd2']//table[@class='smallwnd2']/tr"));
            if (rows.Count%2 != 0) {
                throw new Exception("rows.Count % 2 == 1");
            }
            var result = new List<CompetitionParsed>();
            var extractors = new DefaultExtractor<HtmlNode[]>[] {
                new DefaultCompetitorNameExtractor<HtmlNode[]>(CurrentConfiguration, nodes => {
                    return nodes[0].InnerText.Split(new [] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)[0];
                }),
                new DefaultDateUtcExtractor<HtmlNode[]>(CurrentConfiguration, new DateTimeToGmtFixer(default(int)), true, nodes => {
                    return nodes[0].InnerText.Split(new [] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)[1];
                }),
                new DefaultResultExtractor<HtmlNode[]>(CurrentConfiguration, (nodes, type) => {
                    return nodes[1].InnerText.Replace("Score:", string.Empty);
                }), 
            };
            for (int i = 0; i < rows.Count; i+=2) {
                var competitionName = HtmlBlockHelper.ExtractBlock(rows[i], new XPathQuery(".//td[@class='smwndcap']"))[0].InnerText;
                var competitionParsed = new CompetitionParsed(HtmlBlockDataMonada.FormatCompetitionName(competitionName));
                if (competitionParsed.Type == SportType.Unknown) {
                    continue;
                }
                var matches = HtmlBlockHelper.ExtractBlock(rows[i+1], new XPathQuery(".//table[@class='koeftable']"))
                    .Select(item => DefaultExtractor<HtmlNode[]>.CreateMatchParsed(competitionParsed.Type, HtmlBlockHelper.ExtractBlock(item, new XPathQuery("./tr/td")).ToArray(), extractors))
                    .Where(m => m.IsValid())
                    .ToList();
                if (matches.SafeAny()) {
                    competitionParsed.Matches.AddRange(matches);
                    result.Add(competitionParsed);
                }
            }

            return new BrokerData(BrokerType, language, result);
        }

        public override BrokerData LoadLive(SportType sportType, LanguageType language) {
            throw new NotImplementedException();
        }

        public override BrokerData LoadRegular(SportType sportType, LanguageType language) {
            return new BrokerData(BrokerType, language, ProcessOddsPage(CurrentConfiguration.StringSimple[SectionName.UrlOddsTarget]));
        }

        private List<CompetitionParsed> ProcessOddsPage(string url) {
            var content = LoadPage(url);
            var xPathQueryForm = new XPathQuery(".//form[@name='BetLine']");
            var form = new HtmlBlockHelper(content).ExtractBlock(xPathQueryForm)[0];
            bool nodeValid = false;
            var filterIds = new List<int>();
            foreach (var htmlNode in HtmlBlockHelper.ExtractBlock(form, new XPathQuery("table[@class='smallwnd3']/tr"))) {
                if (!nodeValid || htmlNode.GetAttributeValue("class", "invalid").Equals("m_c")) {
                    nodeValid = SportTypeHelper.Instance[htmlNode.InnerText.Trim().Split('.')] != SportType.Unknown;
                }
                if (!nodeValid) {
                    continue;
                }
                filterIds.AddRange(HtmlBlockHelper.ExtractBlock(htmlNode, new XPathQuery(".//input[@name='sel[]']"))
                    .Select(node => node.GetAttributeValue("value", default(int))));
            }
            url = url.Substring(0, url.LastIndexOf("/") + 1) + form.GetAttributeValue("action", string.Empty);
            var paramsKey = CurrentConfiguration.StringSimple[SectionName.StringCompetitionIDQuery];
            var additionalParams = CurrentConfiguration.StringArray[SectionName.ArrayAddParamOdds]
                .Union(filterIds.Select(fi => paramsKey + "=" + fi))
                .ToList();

            var oddsContent = LoadPage(url, additionalParams).Replace("</nobr> \r", "</nobr> </td></tr>\r").Replace("</tr></div></td></tr></table>", "</tr></table>");
            form = new HtmlBlockHelper(oddsContent).ExtractBlock(xPathQueryForm)[0];
            var xPathQueryToBlock = new XPathQuery(".//table[@class='smallwnd2']/tr");
            var blocks = HtmlBlockHelper.ExtractBlock(form, xPathQueryToBlock);
            if (blocks.Count % 2 == 1) {
                throw new Exception("blocks.Count % 2 == 1");
            }
            var extractors = new DefaultExtractor<HtmlNode[]>[] {
                new DefaultCompetitorNameExtractor<HtmlNode[]>(CurrentConfiguration, nodes => {
                    var node = HtmlBlockHelper.ExtractBlock(nodes[0], new XPathQuery(".//font[@class='m']"));
                    if (node.Any()) {
                        var innerText = node[0].InnerText.Split(new[] {'\r', '\n', '\t'}, StringSplitOptions.RemoveEmptyEntries);
                        return innerText[0];
                    }
                    return null;
                }),
                new DefaultDateUtcExtractor<HtmlNode[]>(CurrentConfiguration, new DateTimeToGmtFixer(default(int)), true, nodes => {
                    return HtmlBlockHelper.ExtractBlock(nodes[0], new XPathQuery("./td"))[0].InnerText;
                }),
                new DefaultOddsExtractor<HtmlNode[]>(CurrentConfiguration, (nodes, sportType) => {
                    var strings = HtmlBlockHelper.ExtractBlock(nodes[1], new XPathQuery(".//nobr")).Select(n => HtmlBlockHelper.RemoveComments(n).InnerText).ToArray();
                    strings = strings.SelectMany(s => {
                        try {
                            s = s.Replace("&nbsp;", string.Empty).RemoveAllTags();
                            if (s.IndexOf("Tot", StringComparison.InvariantCultureIgnoreCase) == -1) {
                                return new[] {s};
                            }
                            var endsParam = s.IndexOf(")");
                            var startIndex = s.IndexOf("(") + 1;
                            var par = s.Substring(startIndex, endsParam - startIndex);
                            s = s.Substring(endsParam + 1);
                            var overPosition = s.IndexOf("O");
                            return new[] {
                                s.Substring(0, overPosition).Replace("U", string.Format("TU({0})", par)),
                                s.Substring(overPosition).Replace("O", string.Format("TO({0})", par))
                            };
                        } catch (Exception ex) {
                            
                        }
                        return new string[0];
                    }).ToArray();
                    return strings;
                }),
            };
            var groupedCompetitions = new List<CompetitionParsed>();
            for (int i = 0; i < blocks.Count; i+=2) {
                var competitionName = HtmlBlockHelper.ExtractBlock(blocks[i], new XPathQuery(".//td[@class='smwndcap']"))[0].InnerText;
                var competitionParsed = new CompetitionParsed(HtmlBlockDataMonada.FormatCompetitionName(competitionName));
                if (competitionParsed.Type == SportType.Unknown) {
                    continue;
                }
                var matchesBlock = HtmlBlockHelper.ExtractBlock(blocks[i + 1], new XPathQuery(".//table[contains(@class, 'koeftable')]//tr"));
                if (matchesBlock.Count%2 == 1) {
                    continue;
                }
                for (int j = 0; j < matchesBlock.Count; j+=2) {
                    var match = DefaultExtractor<HtmlNode[]>.CreateMatchParsed(competitionParsed.Type, new[] { matchesBlock[j], matchesBlock[j+1] }, extractors);
                    if (match.IsValid()) {
                        competitionParsed.Matches.Add(match);
                    }
                }
                if (competitionParsed.Matches.Any()) {
                    groupedCompetitions.Add(competitionParsed);
                }
            }

            return groupedCompetitions;
        }
    }
}