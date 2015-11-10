using System;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.TransportType {
    public class CompetitionItemShortTransport {
        public CompetitionItemShortTransport() {}

        protected CompetitionItemShortTransport(CompetitionItemShortTransport competitionItemShortTransport) {
            CompetitionID = competitionItemShortTransport.CompetitionID;
            DateUtc = competitionItemShortTransport.DateUtc;
            Competitor1 = competitionItemShortTransport.Competitor1;
            Competitor2 = competitionItemShortTransport.Competitor2;
            SportType = competitionItemShortTransport.SportType;
            Competition = competitionItemShortTransport.Competition;
        }

        public int CompetitionID { get; set; }
        public DateTime DateUtc { get; set; }
        public CompetitorTransport Competitor1 { get; set; }
        public CompetitorTransport Competitor2 { get; set; }
        public SportType SportType { get; set; }
        public CompetitionTransport Competition { get; set; }
    }
}