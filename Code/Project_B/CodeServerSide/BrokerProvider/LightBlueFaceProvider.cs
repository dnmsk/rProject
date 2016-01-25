using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using HtmlAgilityPack;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public class LightBlueFaceProvider : BrokerBase {
        public LightBlueFaceProvider(WebRequestHelper requestHelper) : base(requestHelper) {}

        public override BrokerType BrokerType => BrokerType.LightBlue;

        private static readonly Dictionary<int, BetOddType> _oddTypeMap = new Dictionary<int, BetOddType> {
            {1, BetOddType.Win1},
            {2, BetOddType.Draw},
            {3, BetOddType.Win2},
            {4, BetOddType.Win1Draw},
            {5, BetOddType.DrawWin2},
            {6, BetOddType.Win1Win2},
            {7, BetOddType.Handicap1},
            {8, BetOddType.Handicap2},
            {9, BetOddType.TotalUnder},
            {10, BetOddType.TotalOver},
        };

        private static readonly Dictionary<int, SportType> _sportTypeMap = new Dictionary<int, SportType> {
            {1, SportType.Football },
            {2, SportType.IceHockey },
            {3, SportType.Basketball },
            {4, SportType.Tennis },
            {6, SportType.Volleyball },
        };

        public override BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language) {
            var result = new List<CompetitionParsed>();
            foreach (var simpleType in GetSportSimpleTypes(sportType)) {
                var content = LoadPage(FormatUrl(SectionName.UrlResultTarget, new {
                    type = CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsOddsParam][simpleType.ToString()] ?? string.Empty,
                    date = date.ToString(CurrentConfiguration.StringSimple[SectionName.StringDateQueryFormat])
                }));
                result.AddRange(ExtractMatchesBlockFromCompetitonGroup(new HtmlBlockHelper(content).GetCurrentNode(), simpleType));
            }
            return new BrokerData(BrokerType, language,result);
        }
        private List<CompetitionParsed> ExtractMatchesBlockFromCompetitonGroup(HtmlNode node, SportType sportType) {
            var result = new List<CompetitionParsed>();
            var listMatchBlockForCompetitions = HtmlBlockHelper.ExtractBlock(node, CurrentConfiguration.XPath[SectionName.XPathToListCompetitionInCategory]);
            foreach (var listMatchBlockForCompetition in listMatchBlockForCompetitions) {
                var nameBlock = HtmlBlockHelper.ExtractBlock(listMatchBlockForCompetition, CurrentConfiguration.XPath[SectionName.XPathToCompetitionName]);
                var competiton = new CompetitionParsed(FormatCompetitionName(nameBlock[0].InnerText), sportType);
                HtmlBlockHelper.ExtractBlock(listMatchBlockForCompetition, CurrentConfiguration.XPath[SectionName.XPathToEventInList])
                    .Each(n => {
                        try {
                            var resultBlock = HtmlBlockHelper.ExtractBlock(n, CurrentConfiguration.XPath[SectionName.XPathToResultValue]);
                            var competitorsText = HtmlBlockHelper.ExtractBlock(n, CurrentConfiguration.XPath[SectionName.XPathToResultCompetitors])[0].InnerText;
                            if (CurrentConfiguration.StringArray[SectionName.ArrayBadNameCompetitor]?.Any(bad => competitorsText.Contains(bad, StringComparison.InvariantCultureIgnoreCase)) ?? false) {
                                return;
                            }
                            var competitors = competitorsText.Split(CurrentConfiguration.StringArray[SectionName.ArrayParticipantsSplitter], StringSplitOptions.RemoveEmptyEntries);
                            if (competitors.Length != 2) {
                                throw new Exception(competitorsText);
                            }
                            competiton.Matches.Add(new MatchParsed {
                                Result = result.Any() ? ResultBuilder.BuildResultFromString(sportType, resultBlock[0].InnerText) : null,
                                DateUtc = ParseDateTime(HtmlBlockHelper.ExtractBlock(n, CurrentConfiguration.XPath[SectionName.XPathToResultDate])[0].InnerText),
                                CompetitorName1 = new[] { competitors[0].Trim() },
                                CompetitorName2 = new[] { competitors[1].Trim() }
                            });
                        } catch(Exception ex) {
                            Logger.Info(ex.Message);
                        }
                    });
                if (competiton.Matches.Any()) {
                    result.Add(competiton);
                }
            }
            return result;
        }

        public override BrokerData LoadLive(SportType sportType, LanguageType language) {
            return new BrokerData(BrokerType, language, ParseFromJson(sportType, SectionName.UrlLiveTarget));
        }

        public override BrokerData LoadRegular(SportType sportType, LanguageType language) {
            var result = new List<CompetitionParsed>();
            foreach (var simpleType in GetSportSimpleTypes(sportType)) {
                result.AddRange(ParseFromJson(simpleType, SectionName.UrlOddsTarget));
            }
            return new BrokerData(BrokerType, language, result);
        }

        private static IEnumerable<SportType> GetSportSimpleTypes(SportType sportType) {
            return sportType.GetFlags<SportType>().Where(f => f != SportType.All && f != SportType.Unknown);
        }

        private List<CompetitionParsed> ParseFromJson(SportType simpleSportType, SectionName sectionUrlName) {
            var content = LoadPage(FormatUrl(sectionUrlName, new {
                type = CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsOddsParam][simpleSportType.ToString()] ?? string.Empty
            })).Replace("__type", "type");
            var dict = new Dictionary<string, CompetitionParsed>();
            try {
                var deserialized = ToD(JavaScriptSerializer.DeserializeObject(content));
                ToA(deserialized["Value"])
                    .EachSafe(obj => {
                        var map = ToD(obj);
                        var competitionName = (string) map["ChampEng"];
                        CompetitionParsed competition;
                        if (!dict.TryGetValue(competitionName, out competition)) {
                            competition = new CompetitionParsed(FormatCompetitionName(competitionName),
                                _sportTypeMap.TryGetValueOrDefaultStruct((int) map["SportId"]));
                            dict[competitionName] = competition;
                        }
                        var match = new MatchParsed {
                            BrokerMatchID = (int) map["MainGameId"],
                            DateUtc = LinuxUtc.ToUniversalTime().AddSeconds((int) map["Start"]),
                            CompetitorName1 = new[] {(string) map["Opp1Eng"]},
                            CompetitorName2 = new[] {(string) map["Opp2Eng"]},
                        };
                        match.Odds.AddRange(ToA(map["Events"]).Select(o => {
                            var odd = ToD(o);
                            return new OddParsed {
                                Type = _oddTypeMap.TryGetValueOrDefaultStruct((int) odd["T"]),
                                Factor = StringParser.ToFloat(odd["C"].ToString()),
                                AdvancedParam = StringParser.ToFloat(odd["P"].ToString())
                            };
                        }).ToList());
                        competition.Matches.Add(match);
                    });
            } catch (Exception ex) {
                Logger.Error(ex);
            }
            return dict.Values.ToList();
        }
    }
}