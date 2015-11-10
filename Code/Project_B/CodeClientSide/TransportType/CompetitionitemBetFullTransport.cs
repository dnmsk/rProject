using System;
using System.Collections.Generic;

namespace Project_B.CodeClientSide.TransportType {
    public class CompetitionitemBetFullTransport : CompetitionItemBetShortTransport {
        public Dictionary<DateTime, List<List<BetItemTransport>>> BetItems { get; set; } 
        public bool HaveRegular { get; set; }
        public bool HaveLive { get; set; }
    }
}