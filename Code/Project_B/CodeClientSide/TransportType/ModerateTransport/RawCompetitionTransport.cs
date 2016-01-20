using System.Collections.Generic;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.TransportType.ModerateTransport {
    public class RawCompetitionTransport {
        public SportType SportType { get; set; }
        public RawEntityWithLink Competition { get; set; }
        public List<RawCompetitionSpecifyTransport> CompetitionSpecifies { get; set; } 
    }
}