namespace Project_B.CodeClientSide.TransportType {
    public class CompetitionItemResultTransport : CompetitionItemShortTransport {
        public CompetitionItemResultTransport() {}

        public CompetitionItemResultTransport(CompetitionItemResultTransport competitionItemShortTransport) : base(competitionItemShortTransport) {}

        public ResultTransport Result { get; set; }
    }
}