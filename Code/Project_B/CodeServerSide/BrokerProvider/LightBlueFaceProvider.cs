using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using HtmlAgilityPack;
using Project_B.CodeServerSide.BrokerProvider.Helper;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors;
using Project_B.CodeServerSide.Data;
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
            {5, BetOddType.Win1Win2},
            {6, BetOddType.DrawWin2},
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
            var competitiorExtractor = new DefaultCompetitorNameExtractor<HtmlNode>(CurrentConfiguration);
            var resultExtractor = new DefaultResultExtractor<HtmlNode>(CurrentConfiguration);
            var dateTimeExtractor = new DefaultDateUtcExtractor<HtmlNode>(CurrentConfiguration, new DateTimeToGmtFixer(0), () => " " + date.Year);
            foreach (var simpleType in GetSportSimpleTypes(sportType)) {
                var content = LoadPage(FormatUrl(SectionName.UrlResultTarget, new {
                    type = CurrentConfiguration.CompetitionConfiguration[SectionName.MapStringsOddsParam][simpleType.ToString()] ?? string.Empty,
                    date = date.ToString(CurrentConfiguration.StringSimple[SectionName.StringDateQueryFormat])
                }));
                result.AddRange(new HtmlBlockHelper(content).GetCurrentNode()
                        .NodeToNodeCompetitionWithMatches(CurrentConfiguration, htmlNode => simpleType, SectionName.XPathToEventInList)
                        .Select(n => n.NodeToCompetitionParsed(competitiorExtractor, resultExtractor, dateTimeExtractor))
                        .ToList());
            }
            return new BrokerData(BrokerType, language,result);
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
            var extractors = new DefaultExtractor<Dictionary<string, object>>[] {
                new DefaultDateUtcExtractor<Dictionary<string, object>>(CurrentConfiguration, new DateTimeToGmtFixer(default(short)), true, objects => objects["Start"]), 
                new DefaultBrokerIDExtractor<Dictionary<string, object>>(CurrentConfiguration, objects => objects["MainGameId"]), 
                new DefaultCompetitorNameExtractor<Dictionary<string, object>>(CurrentConfiguration, objects => new string[] {objects["Opp1Eng"] as string, objects["Opp2Eng"] as string}), 
                new DefaultOddsExtractor<Dictionary<string, object>>(CurrentConfiguration, (objects, sportType) => ToA(objects["Events"]).Select(o => {
                            var odd = ToD(o);
                            return new OddParsed {
                                Type = _oddTypeMap.TryGetValueOrDefaultStruct((int) odd["T"]),
                                Factor = StringParser.ToFloat(odd["C"].ToString()),
                                AdvancedParam = StringParser.ToFloat(odd["P"].ToString())
                            };
                        }).ToList()), 
            };
            var dict = new Dictionary<string, CompetitionParsed>();
            try {
                var deserialized = ToD(JavaScriptSerializer.DeserializeObject(content));
                ToA(deserialized["Value"])
                    .EachSafe(obj => {
                        var map = ToD(obj);
                        var competitionName = (string) map["ChampEng"];
                        CompetitionParsed competition;
                        if (!dict.TryGetValue(competitionName, out competition)) {
                            competition = new CompetitionParsed(HtmlBlockDataMonada.FormatCompetitionName(competitionName),
                                _sportTypeMap.TryGetValueOrDefaultStruct((int) map["SportId"]));
                            dict[competitionName] = competition;
                        }
                        var match = DefaultExtractor<Dictionary<string, object>>.CreateMatchParsed(competition.Type, map, extractors);
                        competition.Matches.Add(match);
                    });
            } catch (Exception ex) {
                Logger.Error(ex);
            }
            return dict.Values.ToList();
        }
    }
}