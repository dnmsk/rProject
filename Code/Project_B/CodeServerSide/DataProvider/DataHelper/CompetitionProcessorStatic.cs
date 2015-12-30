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
            var successCompetitions = 0;
            var successRawCompetitionItems = 0;
            var successCompetitionItems = 0;
            var competitorProvider = ProjectProvider.Instance.CompetitorProvider;
            var competitionProvider = ProjectProvider.Instance.CompetitionProvider;
            foreach (var competitionParsed in brokerData.Competitions) {
                var competition = competitionProvider.GetCompetitionSpecify(brokerData.Broker, brokerData.Language, competitionParsed.Type, competitionParsed.Name, competitionParsed, algoMode);
                if (competition == null) {
                    continue;
                }
                successCompetitions++;
                foreach (var matchParsed in competitionParsed.Matches) {
                    var competitor1 = competitorProvider
                        .GetCompetitor(brokerData.Broker, brokerData.Language, competitionParsed.Type, competition.Object.GenderType, matchParsed.CompetitorNameFullOne, matchParsed.CompetitorNameShortOne, competition.Object.CompetitionUniqueID, matchParsed, algoMode);
                    var competitor2 = competitorProvider
                        .GetCompetitor(brokerData.Broker, brokerData.Language, competitionParsed.Type, competition.Object.GenderType, matchParsed.CompetitorNameFullTwo, matchParsed.CompetitorNameShortTwo, competition.Object.CompetitionUniqueID, matchParsed, algoMode);
                    if (competitor1 == null || competitor2 == null) {
                        continue;
                    }
                    successRawCompetitionItems++;
                    var competitionItemRawTransport = competitionProvider.GetCompetitionItem(brokerData.Broker, competitor1, competitor2, competition, matchParsed.DateUtc, algoMode);
                    if (competitionItemRawTransport != null) {
                        successCompetitionItems++;
                        if (competitionItemRawTransport.CompetitionItemID < default(int)) {
                            competitionItemRawTransport.CompetitionItemID = -competitionItemRawTransport.CompetitionItemID;
                            logger.Info("Inverse data for ID = {0} {1} {2}", competitionItemRawTransport, brokerData.Broker, brokerData.Language);
                            ReverseAllDataInMatch(matchParsed);
                        }
                        actionForMatch(brokerData.Broker, competitionParsed.Type, competitionItemRawTransport, matchParsed);
                    }
                }
            }
            logger.Info("SaveResults: {0}: Competitions: {1}/{2} CompetitionItems: ({3}) {4}/{5} {6} {7}", brokerData.Competitions.FirstOrDefault(c => c.Matches.Any())?.Matches.FirstOrDefault()?.DateUtc.Date.ToString("yyyy MMMM dd"), 
                successCompetitions, brokerData.Competitions.Count,
                successCompetitionItems, successRawCompetitionItems, brokerData.Competitions.Sum(c => c.Matches.Count), 
                brokerData.Broker, brokerData.Language);
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