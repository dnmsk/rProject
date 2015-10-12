using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using HtmlAgilityPack;
using Project_B.Code.BrokerProvider.Configuration;
using Project_B.Code.BrokerProvider.HtmlDataExtractor;
using Project_B.Code.Data;
using Project_B.Code.Data.Result;
using Project_B.Code.Enums;

namespace Project_B.Code.BrokerProvider.BlueRedBroker {
    public class RedBlueFaceResultsProvider : RedBlueFaceBase, IResultHistoryProvider {
        public RedBlueFaceResultsProvider(WebRequestHelper requestHelper) : base(requestHelper) {}
        
        protected override List<MatchParsed> ExtractMatchesFromMatchesBlock(HtmlNode node, SportType type, DateTimeToGmtFixer dateTimeFixer) {
            var result = new List<MatchParsed>();
            var listMatchBlocks = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToEventResult]);
            foreach (var matchBlock in listMatchBlocks) {
                var participants = HtmlBlockHelper.ExtractBlock(matchBlock, CurrentConfiguration.XPath[SectionName.XPathToResultParticipants]);
                if (participants.Count != 1) {
                    Logger.Error("participants count = " + participants.Count);
                }
                string date = null;
                var dateBlock = HtmlBlockHelper.ExtractBlock(matchBlock, CurrentConfiguration.XPath[SectionName.XPathToResultDate]);
                if (dateBlock.Any()) {
                    date = dateBlock.First().InnerText.Trim();
                }
                var participantsSplitted = participants[0].InnerText.Split(CurrentConfiguration.StringArray[SectionName.ArrayParticipantsSplitter], StringSplitOptions.RemoveEmptyEntries);
                if (participantsSplitted.Length != 2) {
                    Logger.Error("participantsSplitted.Length != 2: " + participants[0].InnerText);
                    continue;
                }
                var match = new MatchParsed {
                    CompetitorNameShortOne = participantsSplitted[0].Trim(),
                    CompetitorNameShortTwo = participantsSplitted[1].Trim(),
                    DateUtc = dateTimeFixer.FixToGmt(ParseDateTime(date)),
                    Result = ExtractResultFromMatchBlock(matchBlock, type)
                };
                result.Add(match);
            }
            return result;
        }

        private FullResult ExtractResultFromMatchBlock(HtmlNode node, SportType type) {
            var resultString = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToResultValue]);
            return resultString.Any() 
                ? ResultBuilder.BuildResultFromString(type, resultString.Select(h => h.InnerHtml).StrJoin("\r\n")) 
                : null;
        }

        public List<CompetitionParsed> Load(DateTime date, SportType sportType) {
            var url = string.Format(CurrentConfiguration.StringSimple[SectionName.UrlResultTarget], 
                                        GetParamValueForCompetition(
                                            sportType, 
                                            CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsResultsParam], 
                                            CurrentConfiguration.StringSimple[SectionName.StringMapStringsResultsParamJoin]), 
                                        date.ToString(CurrentConfiguration.StringSimple[SectionName.StringDateQueryFormat]));
            return BuildCompetitions(LoadPage(url));
        }
    }
}