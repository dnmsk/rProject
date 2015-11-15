using System;
using System.Collections.Generic;
using Project_B.CodeClientSide.TransportType;

namespace Project_B.Models {
    public class CompetitionRegularModel {
        public DateTime DateUtc { get; set; }
        public List<CompetitionTransport> Competitions { get; set; } 

        public int LimitToDisplayInGroup = int.MaxValue;
    }
}