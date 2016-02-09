using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using CommonUtils.Code;
using CommonUtils.Code.WebRequestData;
using CommonUtils.ExtendedTypes;
using PinnacleWrapper;
using PinnacleWrapper.Data;
using PinnacleWrapper.Enums;
using Project_B.CodeServerSide.BrokerProvider.Helper.Configuration;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider {
    public class ASportApiProvider : BrokerBase {
        private readonly PinnacleClient _pinnacleClient;
        private static Dictionary<SportType, int> _mapSports = new Dictionary<SportType, int> {
            {SportType.Basketball, 4 },
            {SportType.Football, 29 },
            {SportType.IceHockey, 19 },
            {SportType.Tennis, 33 },
            {SportType.Volleyball, 34 },
        };

        public ASportApiProvider(WebRequestHelper requestHelper) : base(requestHelper) {
            _pinnacleClient = new PinnacleClient(CurrentConfiguration.StringSimple[SectionName.ApiLogin], CurrentConfiguration.StringSimple[SectionName.ApiPassword], "EUR", OddsFormat.DECIMAL, new HttpClientHandler{
                Proxy = new WebProxy(RequestHelper.GetParam<string>(WebRequestParamType.ProxyString)) {
                    UseDefaultCredentials = false
                }
            });
        }

        public override BrokerType BrokerType => BrokerType.ASport;

        public override BrokerData LoadResult(DateTime date, SportType sportType, LanguageType language) {
            throw new NotImplementedException();
        }

        public override BrokerData LoadLive(SportType sportType, LanguageType language) {
            throw new NotImplementedException();
        }

        public override BrokerData LoadRegular(SportType sportType, LanguageType language) {
            //var sports = _pinnacleClient.GetSports().Result.Select(r => string.Format("{0} - {1}", r.Title, r.Id)).StrJoin(Environment.NewLine);
            var result = new BrokerData(BrokerType, language);
            foreach (var sport in GetSportSimpleTypes(sportType)) {
                var sportId = _mapSports[sport];

                var leagues = _pinnacleClient.GetLeagues(sportId).Result;
                var competitionsMap = leagues.ToDictionary(l => l.Id, l => new CompetitionParsed(HtmlBlockDataMonada.FormatCompetitionName(l.Title), sport));
                Thread.Sleep(5000);

                var eventsMap = new Dictionary<int, MatchParsed>();
                var utcNow = DateTime.UtcNow;
                var fixtures = _pinnacleClient.GetFixtures(new GetFixturesRequest(sportId)   ).Result;
                fixtures.Leagues.Each(l => {
                    CompetitionParsed competition;
                    if (!competitionsMap.TryGetValue(l.Id, out competition)) {
                        return;
                    }
                    foreach (var fixturesEvent in l.Events) {
                        if (utcNow > fixturesEvent.Start) {
                            continue;
                        }
                        eventsMap[fixturesEvent.Id] = new MatchParsed(sport) {
                            DateUtc = fixturesEvent.Start,
                            CompetitorName1 = new[] {fixturesEvent.Home},
                            CompetitorName2 = new[] {fixturesEvent.Away},
                        };
                    }
                });
                Thread.Sleep(5000);

                var odds = _pinnacleClient.GetOdds(new GetOddsRequest(sportId)).Result;
                foreach (var getOddsLeague in odds.Leagues) {
                    CompetitionParsed competitinParsed;
                    if (!competitionsMap.TryGetValue(getOddsLeague.Id, out competitinParsed)) {
                        continue;
                    }
                    var matches = new List<MatchParsed>();
                    foreach (var getOddsEvent in getOddsLeague.Events) {
                        MatchParsed match;
                        if (!eventsMap.TryGetValue(getOddsEvent.Id, out match)) {
                            continue;
                        }
                        if (getOddsEvent.Periods.Count == 0) {
                            continue;
                        }
                        matches.Add(match);
                        var line = getOddsEvent.Periods[0];
                        if (line.MoneyLine != null) {
                            match.Odds.AddRange(new [] {
                                new OddParsed {
                                    Type = BetOddType.Win1,
                                    Factor = (float) line.MoneyLine.Home
                                },
                                new OddParsed {
                                    Type = BetOddType.Win2,
                                    Factor = (float) line.MoneyLine.Away
                                },
                                new OddParsed {
                                    Type = BetOddType.Draw,
                                    Factor = (float) line.MoneyLine.Draw
                                },
                            });
                        }
                        if (line.Spreads != null) {
                            var spreadType = line.Spreads[0];
                            var hcapParam = -(float) spreadType.HomeHandicap;
                            match.Odds.AddRange(new[] {
                                new OddParsed {
                                    Type = BetOddType.Handicap1,
                                    Factor = (float) spreadType.Home
                                },
                                new OddParsed {
                                    Type = BetOddType.Handicap2,
                                    Factor = (float) spreadType.Away,
                                    AdvancedParam = hcapParam
                                }
                            });
                        }
                        if (line.Totals != null) {
                            var total = line.Totals[0];
                            var pointsParam = -(float)total.Points;
                            match.Odds.AddRange(new[] {
                                new OddParsed {
                                    Type = BetOddType.TotalUnder,
                                    Factor = (float) total.Under,
                                    AdvancedParam = pointsParam
                                },
                                new OddParsed {
                                    Type = BetOddType.TotalOver,
                                    Factor = (float) total.Over,
                                    AdvancedParam = pointsParam
                                }
                            });
                        }
                    }
                    competitinParsed.Matches.AddRange(matches.Where(m => m.Odds.Any()));
                    if (competitinParsed.Matches.Any()) {
                        result.Competitions.Add(competitinParsed);
                    }
                }
                Thread.Sleep(5000);
            }
            return result;
        }
    }
}