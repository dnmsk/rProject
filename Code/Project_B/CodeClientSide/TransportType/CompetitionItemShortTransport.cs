using System;

namespace Project_B.CodeClientSide.TransportType {
    public class CompetitionItemShortTransport {
        public CompetitionItemShortTransport() {}

        protected CompetitionItemShortTransport(CompetitionItemShortTransport competitionItemShortTransport) {
            CompetitionItemID = competitionItemShortTransport.CompetitionItemID;
            DateUtc = competitionItemShortTransport.DateUtc;
            Competitor1 = competitionItemShortTransport.Competitor1;
            Competitor2 = competitionItemShortTransport.Competitor2;
        }

        public int CompetitionItemID { get; set; }
        public DateTime DateUtc { get; set; }
        public CompetitorTransport Competitor1 { get; set; }
        public CompetitorTransport Competitor2 { get; set; }
    }
}