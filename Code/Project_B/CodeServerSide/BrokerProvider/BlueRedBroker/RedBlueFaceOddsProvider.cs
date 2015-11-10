﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonUtils.Code;
using HtmlAgilityPack;
using Project_B.CodeServerSide.BrokerProvider.Configuration;
using Project_B.CodeServerSide.BrokerProvider.HtmlDataExtractor;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.BlueRedBroker {
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
                try {
                    var participantsShortName = matchBlock.Attributes[CurrentConfiguration.StringSimple[SectionName.StringOddCompetitorsShortName]].Value
                        .Split(CurrentConfiguration.StringArray[SectionName.ArrayParticipantsSplitter], StringSplitOptions.RemoveEmptyEntries);
                    var participantsFullName = HtmlBlockHelper.ExtractBlock(matchBlock, CurrentConfiguration.XPath[SectionName.XPathToOddsParticipants]);
                    if (participantsFullName.Count != 2) {
                        Logger.Error("participants full name count = " + participantsFullName.Count);
                    }
                    if (participantsShortName.Length != 2) {
                        Logger.Error("participants full name count = " + participantsFullName.Count);
                    }
                    string date = null;
                    var dateBlock = HtmlBlockHelper.ExtractBlock(matchBlock, CurrentConfiguration.XPath[SectionName.XPathToOddsDate]);
                    if (dateBlock.Any()) {
                        date = dateBlock.First().InnerText.Trim();
                    }
                    var match = new MatchParsed {
                        CompetitorNameFullOne = participantsFullName[0].InnerText,
                        CompetitorNameFullTwo = participantsFullName[1].InnerText,
                        CompetitorNameShortOne = participantsShortName[0],
                        CompetitorNameShortTwo = participantsShortName[1],
                        DateUtc = dateTimeFixer.FixToGmt(ParseDateTime(date)),
                        Odds = ExtractOddsFromMatchBlock(type, matchBlock),
                        Result = ExtractResultFromMatchBlock(matchBlock, type),
                        BrokerMatchID = StringParser.ToInt(matchBlock.Attributes[CurrentConfiguration.StringSimple[SectionName.StringOddBrokerID]].Value, default(int))
                    };
                    if (match.Odds.Any() || match.Result != null) {
                        result.Add(match);
                    }
                } catch (Exception ex) {
                    Logger.Error(ex);
                }
            }
            foreach (var additionalMatchBlock in HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToEventResult])) {
                try {
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
                } catch (Exception ex) {
                    Logger.Error(ex);
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
                    odds.Add(new OddParsed());
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

        public BrokerData LoadLive(SportType sportType, LanguageType language) {
            var url = FormatUrl(SectionName.UrlLiveTarget, new {
                lang = GetLanguageParam(language)
            });//CurrentConfiguration.StringSimple[SectionName.UrlLiveTarget];
            return new BrokerData {
                Competitions = BuildCompetitions(LoadPage(url)),
                Broker = BrokerType,
                Language = language
            };
        }

        public BrokerData LoadRegular(SportType sportType, LanguageType language) {
            var url = FormatUrl(SectionName.UrlOddsTarget,  new {
                param = GetParamValueForCompetition(sportType, CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsOddsParam],
                    CurrentConfiguration.StringSimple[SectionName.StringMapStringsOddsParamJoin]),
                lang = GetLanguageParam(language)
            });
                /*string.Format(CurrentConfiguration.StringSimple[SectionName.UrlOddsTarget], 
                    GetParamValueForCompetition(sportType, CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsOddsParam], 
                    CurrentConfiguration.StringSimple[SectionName.StringMapStringsOddsParamJoin]));*/
            return new BrokerData {
                Competitions = BuildCompetitions(LoadPage(url)),
                Broker = BrokerType,
                Language = language
            };
        }
    }
}