using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class CompetitionProcessorStatic {
        public static SummaryProcessStat ProcessCompetitionPack(LoggerWrapper logger, BrokerData brokerData, GatherBehaviorMode algoMode, Action<SummaryProcessStat, BrokerType, SportType, CompetitionItemRawTransport, MatchParsed> actionForMatchedItem) {
            var stat = new SummaryProcessStat();
            var competitorProvider = ProjectProvider.Instance.CompetitorProvider;
            var competitionProvider = ProjectProvider.Instance.CompetitionProvider;
            foreach (var competitionParsed in brokerData.Competitions) {
                var competition = competitionProvider.GetCompetitionSpecify(stat[ProcessStatType.CompetitionFromRaw], stat[ProcessStatType.CompetitionSpecifyFromRaw], brokerData.Broker, brokerData.Language, competitionParsed.Type, competitionParsed.Name, competitionParsed, algoMode);
                foreach (var matchParsed in competitionParsed.Matches) {
                    var competitorStat = stat[ProcessStatType.CompetitorFromRaw];
                    var competitors = new[] {
                        competitorProvider
                        .GetCompetitor(competitorStat, brokerData.Broker, brokerData.Language, competitionParsed.Type, competition.Object.GenderType, matchParsed.CompetitorName1, competition.Object.CompetitionUniqueID, matchParsed, algoMode),
                        competitorProvider
                        .GetCompetitor(competitorStat, brokerData.Broker, brokerData.Language, competitionParsed.Type, competition.Object.GenderType, matchParsed.CompetitorName2, competition.Object.CompetitionUniqueID, matchParsed, algoMode)
                    };
                    var creationCiMode = (matchParsed.Odds.SafeAny() || matchParsed.Result != null) ? algoMode : algoMode.FlagDrop(GatherBehaviorMode.CreateOriginalIfMatchedAll);
                    var competitionItemRawTransport = competitionProvider.GetCompetitionItem(stat[ProcessStatType.CompetitionItemFromRaw], brokerData.Broker, competitors, competition, matchParsed.DateUtc, creationCiMode);
                    if (competitionItemRawTransport != null && competitionItemRawTransport.CompetitionItemID != default(int)) {
                        if (competitionItemRawTransport.CompetitionItemID < default(int)) {
                            competitionItemRawTransport.CompetitionItemID = -competitionItemRawTransport.CompetitionItemID;
                            logger.Info("Inverse data for ID = {0} {1} {2}", competitionItemRawTransport.CompetitionItemID, brokerData.Broker, brokerData.Language);
                            ReverseAllDataInMatch(matchParsed);
                        }
                        actionForMatchedItem(stat, brokerData.Broker, competitionParsed.Type, competitionItemRawTransport, matchParsed);
                    }
                }
            }
            logger.Info("SaveResults: {0} {1} {2}: Competitions: {3}/{4} CompetitionItems: {5}/{6} Competitors {7}/{8}", brokerData.Competitions.FirstOrDefault(c => c.Matches.Any())?.Matches.FirstOrDefault()?.DateUtc.Date.ToString("yyyy MMMM dd"),
                brokerData.Broker, brokerData.Language,
                stat[ProcessStatType.CompetitionSpecifyFromRaw].FinallySuccessCount, stat[ProcessStatType.CompetitionSpecifyFromRaw].TotalCount,
                stat[ProcessStatType.CompetitionItemFromRaw].FinallySuccessCount, stat[ProcessStatType.CompetitionItemFromRaw].TotalCount,
                stat[ProcessStatType.CompetitorFromRaw].FinallySuccessCount, stat[ProcessStatType.CompetitorFromRaw].TotalCount);
            return stat;
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