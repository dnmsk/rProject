using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class CompetitionProcessorStatic {
        public static void ProcessCompetitionPack(LoggerWrapper logger, BrokerData brokerData, GatherBehaviorMode algoMode, Action<BrokerType, SportType, CompetitionItemRawTransport, MatchParsed> actionForMatch) {
            var successCompetitionItems = 0;
            var successCompetitions = 0;
            var successCompetitors = 0;
            var competitorProvider = ProjectProvider.Instance.CompetitorProvider;
            var competitionProvider = ProjectProvider.Instance.CompetitionProvider;
            foreach (var competitionParsed in brokerData.Competitions) {
                var competition = competitionProvider.GetCompetitionSpecify(brokerData.Broker, brokerData.Language, competitionParsed.Type, competitionParsed.Name, competitionParsed, algoMode);
                successCompetitions += competition.Object.CompetitionSpecifyUniqueID != default(int) ? 1 : 0;
                foreach (var matchParsed in competitionParsed.Matches) {
                    try {
                        var competitor1 = competitorProvider
                            .GetCompetitor(brokerData.Broker, brokerData.Language, competitionParsed.Type,
                                competition.Object.GenderType, matchParsed.CompetitorNameFullOne,
                                matchParsed.CompetitorNameShortOne, competition.Object.CompetitionUniqueID, matchParsed,
                                algoMode);
                        var competitor2 = competitorProvider
                            .GetCompetitor(brokerData.Broker, brokerData.Language, competitionParsed.Type,
                                competition.Object.GenderType, matchParsed.CompetitorNameFullTwo,
                                matchParsed.CompetitorNameShortTwo, competition.Object.CompetitionUniqueID, matchParsed,
                                algoMode);
                        successCompetitors += (competitor1.Object.ID != default(int) ? 1 : 0) +
                                              (competitor2.Object.ID != default(int) ? 1 : 0);
                        var competitionItemRawTransport = competitionProvider.GetCompetitionItem(brokerData.Broker,
                            competitor1, competitor2, competition, matchParsed.DateUtc, algoMode);
                        if (competitionItemRawTransport.CompetitionItemID != default(int)) {
                            if (competitionItemRawTransport.CompetitionItemID < default(int)) {
                                competitionItemRawTransport.CompetitionItemID =
                                    -competitionItemRawTransport.CompetitionItemID;
                                logger.Info("Inverse data for ID = {0} {1} {2}", competitionItemRawTransport,
                                    brokerData.Broker, brokerData.Language);
                                ReverseAllDataInMatch(matchParsed);
                            }
                            actionForMatch(brokerData.Broker, competitionParsed.Type, competitionItemRawTransport,
                                matchParsed);
                            successCompetitionItems++;
                        }
                    } catch (Exception ex) {
                        logger.Error("{0} {1} {2} {3} {4} \r\n {5}", brokerData.Broker, brokerData.Language, competitionParsed.Name, matchParsed.CompetitorNameFullOne, matchParsed.CompetitorNameFullTwo, ex);
                    }
                }
            }
            var totalGames = brokerData.Competitions.Sum(c => c.Matches.Count);
            logger.Info("SaveResults: {0} {1} {2}: Competitions: {3}/{4} CompetitionItems: {5}/{6} Competitors {7}/{8}", brokerData.Competitions.FirstOrDefault(c => c.Matches.Any())?.Matches.FirstOrDefault()?.DateUtc.Date.ToString("yyyy MMMM dd"),
                brokerData.Broker, brokerData.Language,
                successCompetitions, brokerData.Competitions.Count,
                successCompetitionItems, totalGames,
                successCompetitors, totalGames*2);
        }

        private static readonly Dictionary<BetOddType, BetOddType> _inversionMap = new Dictionary<BetOddType, BetOddType> {
            {BetOddType.Win1, BetOddType.Win2}, 
            {BetOddType.Win2, BetOddType.Win1}, 
            {BetOddType.Win1Draw, BetOddType.DrawWin2}, 
            {BetOddType.DrawWin2, BetOddType.Win1Draw}, 
            {BetOddType.Handicap1, BetOddType.Handicap2}, 
            {BetOddType.Handicap2, BetOddType.Handicap1}, 
        }; 

        private static void ReverseAllDataInMatch(MatchParsed matchParsed) {
            if (matchParsed.Result != null) {
                var resultOld = matchParsed.Result;
                matchParsed.Result = new FullResult {
                    CompetitorResultOne = resultOld.CompetitorResultTwo,
                    CompetitorResultTwo = resultOld.CompetitorResultOne,
                    SubResult = resultOld.SubResult.Select(sr => new SimpleResult {
                        CompetitorResultOne = sr.CompetitorResultTwo,
                        CompetitorResultTwo = sr.CompetitorResultOne,
                        Serve = sr.Serve != Serve.Unknown 
                            ? (sr.Serve == Serve.Serve1Player ? Serve.Serve2Player : Serve.Serve1Player) 
                            : Serve.Unknown
                    }).ToList()
                };
            }
            if (matchParsed.Odds != null && matchParsed.Odds.Any()) {
                var hcapDetail = (int) (matchParsed.Odds.Where(o => o.Type == BetOddType.Handicap2).MaxOrDefault(o => o.AdvancedParam, null) ?? default(int));
                matchParsed.Odds.Each(odds => {
                    BetOddType newOddType;
                    if (_inversionMap.TryGetValue(odds.Type, out newOddType)) {
                        odds.Type = newOddType;
                        switch (newOddType) {
                            case BetOddType.Handicap2:
                                odds.AdvancedParam = hcapDetail;
                                break;
                        }
                    }
                });
            }
        }
    }
}