using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.TransportType.ModerateTransport {
    public class SystemStateSummaryStatus : IBrokerTyped, ILanguageTyped {
        public BrokerType BrokerID { get; set; }
        public LanguageType Languagetype { get; set; }
        public int RawCompetitionItemCount { get; set; }
        public int CompetitionItemLinkedCount { get; set; }
        public int RawCompetitorCount { get; set; }
        public int CompetitorLinkedCount { get; set; }
        public int RawCompetitionCount { get; set; }
        public int CompetitionLinkedCount { get; set; }
        public int RawCompetitionSpecifyCount { get; set; }
        public int CompetitionSpecifyLinkedCount { get; set; }
    }
}