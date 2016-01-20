using System.Collections.Generic;

namespace Project_B.CodeClientSide.TransportType.ModerateTransport {
    public class RawCompetitionSpecifyTransport {
        public RawEntityWithLink CompetitionSpecify { get; set; }
        public List<RawCompetitionItemTransport> CompetitionItems { get; set; } 
    }
}