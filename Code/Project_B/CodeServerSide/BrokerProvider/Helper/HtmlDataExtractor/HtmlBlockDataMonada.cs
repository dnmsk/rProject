using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using HtmlAgilityPack;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor {
    public static class HtmlBlockDataMonada {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (HtmlBlockDataMonada).FullName);

        public static List<HtmlBlockData> NodeToNodeCompetitionGroupedBySport(this HtmlNode node, BrokerConfiguration brokerConfiguration) {
            return HtmlBlockHelper.ExtractBlock(node, brokerConfiguration.XPath[SectionName.XPathToCategoryContainer])
                .Select(n => new HtmlBlockData(n))
                .ToList();
        }

        public static List<Tuple<CompetitionParsed, HtmlNode, List<HtmlNode>>> NodeToNodeCompetitionWithMatches(this HtmlNode node, BrokerConfiguration brokerConfiguration, Func<HtmlNode, SportType> sportTypeExtractor, SectionName xPathToMatches) {
            var result = new List<Tuple<CompetitionParsed, HtmlNode, List<HtmlNode>>>();
            var sportType = sportTypeExtractor(node);
            var listMatchBlockForCompetitions = HtmlBlockHelper.ExtractBlock(node, brokerConfiguration.XPath[SectionName.XPathToListCompetitionInCategory]);
            foreach (var listMatchBlockForCompetition in listMatchBlockForCompetitions) {
                var competitionNameFull = new List<string>();
                var nameBlock = HtmlBlockHelper.ExtractBlock(listMatchBlockForCompetition, brokerConfiguration.XPath[SectionName.XPathToCompetitionName]);
                if (nameBlock.Count > 0) {
                    competitionNameFull.AddRange(FormatCompetitionName(nameBlock[0].InnerText));
                }
                var competiton = new CompetitionParsed(competitionNameFull.ToArray(), sportType);
                if (competiton.Type != SportType.Unknown) {
                    var extractBlock = HtmlBlockHelper.ExtractBlock(listMatchBlockForCompetition, brokerConfiguration.XPath[xPathToMatches]);
                    if (extractBlock.SafeAny()) {
                        result.Add(new Tuple<CompetitionParsed, HtmlNode, List<HtmlNode>>(competiton, listMatchBlockForCompetition, extractBlock));
                    }
                }
            }
            return result;
        }

        public static CompetitionParsed NodeToCompetitionParsed<T>(this Tuple<CompetitionParsed, T, List<T>> competitionWithMatches, params DefaultExtractor<T>[] matchExtractor) {
            var matches = new List<MatchParsed>(competitionWithMatches.Item3.Count);
            foreach (var n in competitionWithMatches.Item3) {
                try {
                    var match = DefaultExtractor<T>.CreateMatchParsed(competitionWithMatches.Item1.Type, n, matchExtractor);
                    if (match.CompetitorName1.SafeAny() && match.CompetitorName2.SafeAny()) {
                        matches.Add(match);
                    }
                } catch (Exception ex) {
                    _logger.Info(ex.ToString());
                }
            }

            competitionWithMatches.Item1.Matches.AddRange(matches);
            return competitionWithMatches.Item1;
        }

        public static string[] FormatCompetitionName(string competitionName) {
            return competitionName.RemoveAllTags()
                                  .Replace("&nbsp;", " ")
                                  .Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => s.Trim())
                                  .ToArray();
        }
    }
}