using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonUtils.Code;
using HtmlAgilityPack;
using Project_B.Code.BrokerProvider.Configuration;
using Project_B.Code.BrokerProvider.HtmlDataExtractor;
using Project_B.Code.Data;
using Project_B.Code.Data.Result;
using Project_B.Code.Enums;

namespace Project_B.Code.BrokerProvider.BlueRedBroker {
    public class RedBlueFaceOddsProvider : RedBlueFaceBase, IOddsProvider {
        private static readonly Regex _numberExtractorRegex = new Regex(".*?\\((?<number>.*?)\\).*?", RegexOptions.Compiled);
        private static readonly Dictionary<SportType, BetOddType[]> _typeWithOrderInLine = new Dictionary<SportType, BetOddType[]> {
            { SportType.Football, new[] {BetOddType.Win1, BetOddType.Draw, BetOddType.Win2, BetOddType.Win1Draw, BetOddType.Win1Win2, BetOddType.DrawWin2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.Tennis, new[] {BetOddType.Win1, BetOddType.Win2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.Volleyball, new[] {BetOddType.Win1, BetOddType.Win2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.Basketball, new[] {BetOddType.Win1, BetOddType.Win2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.IceHockey, new[] {BetOddType.Win1, BetOddType.Draw, BetOddType.Win2, BetOddType.Win1Draw, BetOddType.Win1Win2, BetOddType.DrawWin2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
        }; 

        public RedBlueFaceOddsProvider(WebRequestHelper requestHelper) : base(requestHelper) {}

        protected override List<MatchParsed> ExtractMatchesFromMatchesBlock(HtmlNode node, SportType type, DateTimeToGmtFixer dateTimeFixer) {
            var result = new List<MatchParsed>();
            var listMatchBlocks = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToEventInList]);
            foreach (var matchBlock in listMatchBlocks) {
                var participants = HtmlBlockHelper.ExtractBlock(matchBlock, CurrentConfiguration.XPath[SectionName.XPathToOddsParticipants]);
                if (participants.Count != 2) {
                    Logger.Error("participants count = " + participants.Count);
                }
                string date = null;
                var dateBlock = HtmlBlockHelper.ExtractBlock(matchBlock, CurrentConfiguration.XPath[SectionName.XPathToOddsDate]);
                if (dateBlock.Any()) {
                    date = dateBlock.First().InnerText.Trim();
                }
                var match = new MatchParsed {
                    CompetitorNameShortOne = participants[0].InnerText,
                    CompetitorNameShortTwo = participants[1].InnerText,
                    DateUtc = dateTimeFixer.FixToGmt(ParseDateTime(date)),
                    Odds = ExtractOddsFromMatchBlock(type, matchBlock),
                    Result = ExtractResultFromMatchBlock(matchBlock, type),
                    BrokerMatchID = StringParser.ToInt(matchBlock.Attributes[CurrentConfiguration.StringSimple[SectionName.StringOddBrokerID]].Value, default(int))
                };
                if (match.Odds.Any() || match.Result != null) {
                    result.Add(match);
                }
            }
            foreach (var additionalMatchBlock in HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToEventResult])) {
                var participants = HtmlBlockHelper.ExtractBlock(additionalMatchBlock, CurrentConfiguration.XPath[SectionName.XPathToResultParticipants])[0].InnerText
                    .Split(CurrentConfiguration.StringArray[SectionName.ArrayParticipantsSplitter], StringSplitOptions.RemoveEmptyEntries);
                var match = new MatchParsed {
                    CompetitorNameShortOne = participants[0],
                    CompetitorNameShortTwo = participants[1],
                    Result = ExtractResultFromMatchBlock(additionalMatchBlock, type),
                    BrokerMatchID = StringParser.ToInt(additionalMatchBlock.Attributes[CurrentConfiguration.StringSimple[SectionName.StringOddBrokerID]].Value, default(int))
                };
                if (match.Odds.Any() || match.Result != null) {
                    result.Add(match);
                }
            }
            return result;
        }

        private FullResult ExtractResultFromMatchBlock(HtmlNode node, SportType type) {
            var resultString = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToOddsLiveResult]);
            if (resultString.Any()) {
                return ResultBuilder.BuildResultFromString(type, resultString[0].InnerHtml);
            }
            return null;
        }

        private List<OddParsed> ExtractOddsFromMatchBlock(SportType type, HtmlNode node) {
            var odds = new List<OddParsed>();
            var betBlocks = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToOddsFactor]);
            foreach (var betBlock in betBlocks) {
                var attrValue = betBlock.Attributes["data-sel"];
                if (attrValue == null) {
                    Logger.Error("attrValue == null ");
                    continue;
                }
                var dataDict = JavaScriptSerializer.Deserialize<Dictionary<string, object>>(attrValue.Value);
                var prices = ((Dictionary<string, object>)dataDict["prices"]);
                string confirmation = null;
                var oddsConfirmation = HtmlBlockHelper.ExtractBlock(betBlock, CurrentConfiguration.XPath[SectionName.XPathToOddsConfirmation]);
                if (oddsConfirmation.Count > 0) {
                    confirmation = oddsConfirmation[0]
                        .Attributes[CurrentConfiguration.StringSimple[SectionName.StringOddConfirmation]]
                        .Value;
                }
                decimal? advancedParam = null;
                var numberMatched = _numberExtractorRegex.Match(betBlock.InnerText);
                if (numberMatched.Success && numberMatched.Groups.Count > 0) {
                    advancedParam = StringParser.ToDecimal(numberMatched.Groups["number"].Value, null);
                }
                var odd = new OddParsed {
                    Factor = StringParser.ToDecimal(prices["1"].ToString()),
                    AdvancedParam = advancedParam,
                    OddStringConfirmation = confirmation
                };
                odds.Add(odd);
            }
            TryMapOddTypes(type, odds);
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

        public List<CompetitionParsed> LoadLive(SportType sportType) {
            var url = CurrentConfiguration.StringSimple[SectionName.UrlLiveTarget];
            return BuildCompetitions(LoadPage(url));
        }

        public List<CompetitionParsed> LoadRegular(SportType sportType) {
            var url = string.Format(CurrentConfiguration.StringSimple[SectionName.UrlOddsTarget], 
                    GetParamValueForCompetition(sportType, CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsOddsParam], 
                    CurrentConfiguration.StringSimple[SectionName.StringMapStringsOddsParamJoin]));
            return BuildCompetitions(LoadPage(url));
        }
    }
}