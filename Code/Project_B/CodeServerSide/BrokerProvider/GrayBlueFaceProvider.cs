﻿using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.BrokerProvider.Common;
using Project_B.CodeServerSide.BrokerProvider.Helper;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors;
using Project_B.CodeServerSide.BrokerProvider.Interfaces;
using Project_B.CodeServerSide.BrokerProvider.SubData;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public class GrayBlueFaceProvider : BrokerBase<WebRequestWrapper> {
        private readonly GrayBlueOddTypeProvider _blueOddTypeProvider;

        public GrayBlueFaceProvider(IQueryableWrapper requestHelper) : base(requestHelper) {
            _blueOddTypeProvider = new GrayBlueOddTypeProvider(CurrentConfiguration.StringSimple[SectionName.UrlKab], requestHelper.RequestHelper, JavaScriptSerializer, ToA, ToD);
        }

        public override BrokerType BrokerType => BrokerType.GrayBlue;

        public override BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language) {
            var data = LoadPage(FormatUrl(SectionName.UrlResultTarget, new {
                datestamp = ((int)(DateTime.UtcNow - ProjectBConsts.DefaultLinuxUtc).TotalSeconds) / 10,
                date = date.ToString(CurrentConfiguration.StringSimple[SectionName.StringDateQueryFormat]),
                lang = GetLanguageParam(language)
            }));
            var rows = data.Split(new[] {"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            var brokerData = new BrokerData(BrokerType, language);
            var internalCompetitionItemIDToCompetitionParsed = new Dictionary<int, MatchParsed>();
            var extractors = new DefaultExtractor<object[]>[] {
                new DefaultDateUtcExtractor<object[]>(CurrentConfiguration, new DateTimeToGmtFixer(default(short)), true, objects => objects[2]),
                new DefaultResultExtractor<object[]>(CurrentConfiguration, (objects, spType) => objects[4]),
                new DefaultCompetitorNameExtractor<object[]>(CurrentConfiguration, objects => objects[3])
            };
            foreach (var row in rows) {
                try {
                    var firstIndex = row.IndexOf("(", StringComparison.InvariantCulture);
                    var lastIndex = row.LastIndexOf(")", StringComparison.InvariantCulture);
                    if (firstIndex == -1 || lastIndex == -1) {
                        continue;
                    }
                    var funcName = row.Substring(0, firstIndex);
                    object[] dataRowJson;
                    if (funcName.Equals(CurrentConfiguration.StringSimple[SectionName.StringMatchRow], StringComparison.InvariantCultureIgnoreCase)) {
                        dataRowJson = BuildJsonObject(row, firstIndex, lastIndex);
                        if ((int) dataRowJson[5] != 3) {
                            continue;
                        }
                        var matchParsed = DefaultExtractor<object[]>.CreateMatchParsed(SportType.Unknown, dataRowJson, extractors);
                        if (matchParsed.IsValid()) {
                            internalCompetitionItemIDToCompetitionParsed[(int)dataRowJson[0]] = matchParsed;
                        }
                    } else if (funcName.Equals(CurrentConfiguration.StringSimple[SectionName.StringCompetitionRow], StringComparison.InvariantCultureIgnoreCase)) {
                        dataRowJson = BuildJsonObject(row, firstIndex, lastIndex);
                        var formatCompetitionName = HtmlBlockDataMonada.FormatCompetitionName((string) dataRowJson[2]);
                        var competitionParsed = new CompetitionParsed(formatCompetitionName);
                        if (competitionParsed.Type != SportType.Unknown) {
                            brokerData.Competitions.Add(competitionParsed);
                            foreach (int item in ToA(dataRowJson[3])) {
                                MatchParsed matchParsed;
                                if (internalCompetitionItemIDToCompetitionParsed.TryGetValue(item, out matchParsed)) {
                                    competitionParsed.Matches.Add(matchParsed);
                                }
                            }
                        }
                    }
                } catch (Exception ex) {
                    Logger.Error(ex);
                }
            }
            return brokerData;
        }

        public override BrokerData LoadLive(SportType sportType, LanguageType language) {
            var deserializedLine = ToD(JavaScriptSerializer.DeserializeObject(LoadPage(FormatUrl(SectionName.UrlLiveTarget, new {
                lang = GetLanguageParam(language)
            }))));
            var competitionsDict = BuildCompetitionParsed(deserializedLine, matches => {
                ToA(deserializedLine["eventMiscs"])
                    .EachSafe(obj => {
                        var map = ToD(obj);
                        var id = (int) map["id"];
                        MatchParsed matchParsed;
                        if (matches.TryGetValue(id, out matchParsed)) {
                            matchParsed.Result = ResultBuilder.BuildResultFromString(SportType.Unknown, string.Format("{0}:{1} {2}", map["score1"], map["score2"], map["comment"]));
                        }
                    });
            });
            return new BrokerData(BrokerType, language, competitionsDict.Where(c => c.Matches.Count > 0).ToList());
        }

        public override BrokerData LoadRegular(SportType sportType, LanguageType language) {
            var deserializedLine = ToD(JavaScriptSerializer.DeserializeObject(LoadPage(FormatUrl(SectionName.UrlOddsTarget, new {
                lang = GetLanguageParam(language)
            }))));
            var competitionsDict = BuildCompetitionParsed(deserializedLine);
            return new BrokerData(BrokerType, language, competitionsDict.Where(c => c.Matches.Count > 0).ToList());
        }

        private List<CompetitionParsed> BuildCompetitionParsed(Dictionary<string, object> deserializedLine, Action<Dictionary<int, MatchParsed>> postProcessFunc = null) {
            var oddMapper = _blueOddTypeProvider.GetOddMapCreator((int) deserializedLine["factorsVersion"]);
            var competitionsDict = new Dictionary<int, CompetitionParsed>();
            var matchesDict = new Dictionary<int, MatchParsed>();
            var extractors = new DefaultExtractor<Dictionary<string, object>>[] {
                new DefaultDateUtcExtractor<Dictionary<string, object>>(CurrentConfiguration, new DateTimeToGmtFixer(default(short)), true, objects => objects["startTime"]),
                new DefaultCompetitorNameExtractor<Dictionary<string, object>>(CurrentConfiguration, objects => new[] { objects.TryGetValueOrDefault("team1") as string, objects.TryGetValueOrDefault("team2") as string }),
                new DefaultBrokerIDExtractor<Dictionary<string, object>>(CurrentConfiguration, objects => objects["id"])
            };
            ToA(deserializedLine["sports"])
                .EachSafe(obj => {
                    var map = ToD(obj);
                    var competitionName = HtmlBlockDataMonada.FormatCompetitionName(map["name"].ToString());
                    if ((string) map["kind"] != "segment") {
                        return;
                    }
                    var competition = new CompetitionParsed(competitionName);
                    if (competition.Type != SportType.Unknown) {
                        competitionsDict[(int) map["id"]] = competition;
                    }
                });
            ToA(deserializedLine["events"])
                .EachSafe(obj => {
                    var map = ToD(obj);
                    var competitionID = (int) map["sportId"];
                    CompetitionParsed competitionParsed;
                    if ((int) map["level"] == 1 && competitionsDict.TryGetValue(competitionID, out competitionParsed)) {
                        var matchParsed = DefaultExtractor<Dictionary<string, object>>.CreateMatchParsed(competitionParsed.Type, map, extractors);
                        if (matchParsed.IsValid()) {
                            matchesDict[matchParsed.BrokerMatchID] = matchParsed;
                            competitionParsed.Matches.Add(matchParsed);
                        }
                    }
                });
            ToA(deserializedLine["customFactors"])
                .EachSafe(obj => {
                    var map = ToD(obj);
                    Func<OddParsed> oddCreator;
                    if (oddMapper.TryGetValue((int) map["f"], out oddCreator)) {
                        MatchParsed matchParsed;
                        if (matchesDict.TryGetValue((int)map["e"], out matchParsed)) {
                            var odd = oddCreator();
                            odd.Factor = (float)(decimal)map["v"];
                            object advParam;
                            if (map.TryGetValue("pt", out advParam)) {
                                odd.AdvancedParam = StringParser.ToFloat(advParam.ToString());
                            }
                            matchParsed.Odds.Add(odd);
                        }
                    }
                });
            if (postProcessFunc != null) {
                postProcessFunc(matchesDict);
            }
            competitionsDict.Values
                .EachSafe(c => {
                    var matchParseds = c.Matches.Where(match => match.Odds.Any() || match.Result != null).ToList();
                    c.Matches.Clear();
                    c.Matches.AddRange(matchParseds);
                });
            return competitionsDict.Values
                .Where(c => c.Matches.Count > 0)
                .ToList();
        }
        
        private static object[] BuildJsonObject(string row, int startIndex, int endIndex) {
            return ToA(JavaScriptSerializer.DeserializeObject("[" + row.Substring(startIndex + 1, (endIndex - startIndex) - 1) + "]"));
        }
    }
}