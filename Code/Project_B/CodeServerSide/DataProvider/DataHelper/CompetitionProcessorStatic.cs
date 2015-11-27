using System;
using System.Linq;
using CommonUtils.Core.Logger;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class CompetitionProcessorStatic {
        public static void ProcessCompetitionPack(LoggerWrapper logger, BrokerData brokerData, GatherBehaviorMode algoMode, Action<BrokerType, SportType, int, MatchParsed> actionForMatch) {
            var successCompetitions = 0;
            var successCompetitionItems = 0;
            var competitorProvider = ProjectProvider.Instance.CompetitorProvider;
            var competitionProvider = ProjectProvider.Instance.CompetitionProvider;
            foreach (var competitionParsed in brokerData.Competitions) {
                var competition = competitionProvider.GetCompetition(brokerData.Language, competitionParsed.Type, competitionParsed.Name, competitionParsed, algoMode);
                if (competition == null) {
                    continue;
                }
                successCompetitions++;
                foreach (var matchParsed in competitionParsed.Matches) {
                    var competitor1 = competitorProvider
                        .GetCompetitor(brokerData.Language, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullOne, matchParsed.CompetitorNameShortOne, competition.UniqueID, matchParsed, algoMode);
                    var competitor2 = competitorProvider
                        .GetCompetitor(brokerData.Language, competitionParsed.Type, competition.GenderType, matchParsed.CompetitorNameFullTwo, matchParsed.CompetitorNameShortTwo, competition.UniqueID, matchParsed, algoMode);
                    if (competitor1 == null || competitor2 == null) {
                        continue;
                    }
                    successCompetitionItems++;
                    var competitionItem = competitionProvider.GetCompetitionItem(competitor1, competitor2, competition, matchParsed.DateUtc, algoMode);
                    if (competitionItem != default(int)) {
                        actionForMatch(brokerData.Broker, competitionParsed.Type, competitionItem, matchParsed);
                    }
                }
            }
            logger.Info("SaveResults: {0}: Competitions: {1}/{2} CompetitionItems: {3}/{4} {5} {6}", brokerData.Competitions.First().Matches.First().DateUtc.Date.ToString("yyyy MMMM dd"), successCompetitions, brokerData.Competitions.Count,
                successCompetitionItems, brokerData.Competitions.Sum(c => c.Matches.Count), brokerData.Broker, brokerData.Language);
        }
    }
}