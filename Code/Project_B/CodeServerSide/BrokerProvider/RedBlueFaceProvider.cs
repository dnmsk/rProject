using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using HtmlAgilityPack;
using Project_B.CodeServerSide.BrokerProvider.Helper;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public sealed class RedBlueFaceProvider : BrokerBase {
        private static readonly Regex _numberExtractorRegex = new Regex(".*?\\((?<number>.*?)\\).*?", RegexOptions.Compiled);
        private static readonly Dictionary<SportType, BetOddType[]> _typeWithOrderInLine = new Dictionary<SportType, BetOddType[]> {
            { SportType.Football, new[] {BetOddType.Win1, BetOddType.Draw, BetOddType.Win2, BetOddType.Win1Draw, BetOddType.Win1Win2, BetOddType.DrawWin2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.Tennis, new[] {BetOddType.Win1, BetOddType.Win2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.Volleyball, new[] {BetOddType.Win1, BetOddType.Win2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.Basketball, new[] {BetOddType.Win1, BetOddType.Win2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
            { SportType.IceHockey, new[] {BetOddType.Win1, BetOddType.Draw, BetOddType.Win2, BetOddType.Win1Draw, BetOddType.Win1Win2, BetOddType.DrawWin2, BetOddType.Handicap1, BetOddType.Handicap2, BetOddType.TotalUnder, BetOddType.TotalOver } },
        };

        public RedBlueFaceProvider(WebRequestHelper requestHelper) : base(requestHelper) { }

        public override BrokerType BrokerType => BrokerType.RedBlue;

        public override BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language) {
            var url = FormatUrl(SectionName.UrlResultTarget, new {
                query = GetParamValueForCompetition(sportType,
                                                    CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsResultsParam],
                                                    CurrentConfiguration.StringSimple[SectionName.StringMapStringsResultsParamJoin]),
                date = date.ToString(CurrentConfiguration.StringSimple[SectionName.StringDateQueryFormat]),
                lang = GetLanguageParam(language)
            });
            return new BrokerData(BrokerType, language, BuildCompetitions(LoadPage(url), ExtractMatchesResultFromMatchesBlock));
        }

        public override BrokerData LoadLive(SportType sportType, LanguageType language) {
            var url = FormatUrl(SectionName.UrlLiveTarget, new {
                lang = GetLanguageParam(language)
            });
            return new BrokerData(BrokerType, language, BuildCompetitions(LoadPage(url), ExtractMatchesOddsFromMatchesBlock));
        }

        public override BrokerData LoadRegular(SportType sportType, LanguageType language) {
            var url = FormatUrl(SectionName.UrlOddsTarget, new {
                param = GetParamValueForCompetition(sportType, CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsOddsParam],
                                                    CurrentConfiguration.StringSimple[SectionName.StringMapStringsOddsParamJoin]),
                lang = GetLanguageParam(language)
            });
            return new BrokerData(BrokerType, language, BuildCompetitions(LoadPage(url), ExtractMatchesOddsFromMatchesBlock));
        }

        private List<CompetitionParsed> BuildCompetitions(string htmlContent, Func<HtmlNode, SportType, DateTimeToGmtFixer, List<MatchParsed>> matchesBuilderFunc) {
            var result = new List<CompetitionParsed>();
            var htmlBlockHelper = new HtmlBlockHelper(htmlContent);
            var dateTimeFixer = GetGmtFixer(htmlContent);
            var groupedCompetitions = htmlBlockHelper.ExtractBlock(CurrentConfiguration.XPath[SectionName.XPathToCategoryContainer]);
            foreach (var groupedCompetition in groupedCompetitions) {
                result.AddRange(ExtractMatchesBlockFromCompetitonGroup(groupedCompetition, dateTimeFixer, matchesBuilderFunc));
            }
            return result;
        }

        private List<CompetitionParsed> ExtractMatchesBlockFromCompetitonGroup(HtmlNode node, DateTimeToGmtFixer dateTimeFixer, Func<HtmlNode, SportType, DateTimeToGmtFixer, List<MatchParsed>> matchesBuilderFunc) {
            var result = new List<CompetitionParsed>();
            var competitionNameString = string.Empty;
            var nameBlock = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToCategoryName]);
            if (nameBlock.Count > 0) {
                competitionNameString = nameBlock[0].InnerText.Trim();
            }
            var listMatchBlockForCompetitions = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToListCompetitionInCategory]);
            foreach (var listMatchBlockForCompetition in listMatchBlockForCompetitions) {
                var competitionNameFull = new List<string>();
                if (!competitionNameString.IsNullOrWhiteSpace()) {
                    competitionNameFull.Add(competitionNameString);
                }
                nameBlock = HtmlBlockHelper.ExtractBlock(listMatchBlockForCompetition, CurrentConfiguration.XPath[SectionName.XPathToCompetitionName]);
                if (nameBlock.Count > 0) {
                    competitionNameFull.AddRange(FormatCompetitionName(nameBlock[0].InnerText));
                }
                var competiton = new CompetitionParsed(competitionNameFull);
                if (competiton.Type != SportType.Unknown) {
                    var matches = matchesBuilderFunc(listMatchBlockForCompetition, competiton.Type, dateTimeFixer);
                    if (matches.Any()) {
                        competiton.Matches.AddRange(matches);
                        result.Add(competiton);
                    }
                }
            }
            return result;
        }

        private static DateTimeToGmtFixer GetGmtFixer(string htmlContent) {
            var idx = htmlContent.IndexOf("\"gmt", StringComparison.InvariantCultureIgnoreCase);
            var num = htmlContent.Substring(idx + 4);
            num = num.Substring(0, num.IndexOf("\"", StringComparison.InvariantCultureIgnoreCase));
            return new DateTimeToGmtFixer(StringParser.ToShort(num, 0));
        }

        private List<MatchParsed> ExtractMatchesResultFromMatchesBlock(HtmlNode node, SportType type, DateTimeToGmtFixer dateTimeFixer) {
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
                    CompetitorName1 = new[] { participantsSplitted[0].Trim() },
                    CompetitorName2 = new[] { participantsSplitted[1].Trim() },
                    DateUtc = dateTimeFixer.FixToGmt(ParseDateTime(date)),
                    Result = ExtractResultFromMatchBlock(matchBlock, type, list => list.Select(h => h.InnerHtml).StrJoin(Environment.NewLine))
                };
                result.Add(match);
            }
            return result;
        }

        private FullResult ExtractResultFromMatchBlock(HtmlNode node, SportType type, Func<List<HtmlNode>, string> nodeToResultStr) {
            var resultString = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToResultValue]);
            return resultString.Any()
                ? ResultBuilder.BuildResultFromString(type, nodeToResultStr(resultString))
                : null;
        }
        
        private List<MatchParsed> ExtractMatchesOddsFromMatchesBlock(HtmlNode node, SportType type, DateTimeToGmtFixer dateTimeFixer) {
            var result = new List<MatchParsed>();
            var listMatchBlocks = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToEventInList]);
            Func<List<HtmlNode>, string> nodeToResultStr = list => list[0].InnerHtml;
            foreach (var matchBlock in listMatchBlocks) {
                try {
                    var participantsShortName = matchBlock.Attributes[CurrentConfiguration.StringSimple[SectionName.StringOddCompetitorsShortName]].Value
                        .Split(CurrentConfiguration.StringArray[SectionName.ArrayParticipantsSplitter], StringSplitOptions.RemoveEmptyEntries);
                    var participantsFullName = HtmlBlockHelper.ExtractBlock(matchBlock, CurrentConfiguration.XPath[SectionName.XPathToOddsParticipants]);
                    string date = null;
                    var dateBlock = HtmlBlockHelper.ExtractBlock(matchBlock, CurrentConfiguration.XPath[SectionName.XPathToOddsDate]);
                    if (dateBlock.Any()) {
                        date = dateBlock.First().InnerText.Trim();
                    }
                    var match = new MatchParsed {
                        CompetitorName1 = new[] { participantsFullName[0].InnerText, participantsShortName[0] },
                        CompetitorName2 = new[] { participantsFullName[1].InnerText, participantsShortName[1] },
                        DateUtc = dateTimeFixer.FixToGmt(ParseDateTime(date)),
                        Result = ExtractResultFromMatchBlock(matchBlock, type, nodeToResultStr),
                        BrokerMatchID = StringParser.ToInt(matchBlock.Attributes[CurrentConfiguration.StringSimple[SectionName.StringOddBrokerID]].Value, default(int))
                    };
                    match.Odds.AddRange(ExtractOddsFromMatchBlock(type, matchBlock));
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
                        CompetitorName1 = new[] { participants[0] },
                        CompetitorName2 = new[] { participants[1] },
                        Result = ExtractResultFromMatchBlock(additionalMatchBlock, type, nodeToResultStr),
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
    }
}