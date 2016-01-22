using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.TransportType.ModerateTransport {
    public class RawEntityWithLink {
        public int CompetitionItemID { get; set; }
        public int RawID { get; set; }
        public string RawName { get; set; }
        public int EntityID { get; set; }
        public string[] EntityName { get; set; }
        public BrokerEntityType BrokerEntityType { get; set; }
    }
}