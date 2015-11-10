using System.Collections.Generic;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.TransportType {
    public class CompetitionItemBetShortTransport : CompetitionItemShortTransport {
        public CompetitionItemBetShortTransport() {}

        public CompetitionItemBetShortTransport(CompetitionItemShortTransport competitionItemShortTransport)
            : base(competitionItemShortTransport) {}
        public Dictionary<BetOddType, BetItemTransport> CurrentBets { get; set; } 
        public Dictionary<BetOddType, BetItemTransport> HistoryMinBets { get; set; } 
        public Dictionary<BetOddType, BetItemTransport> HistoryMaxBets { get; set; }
    }
}