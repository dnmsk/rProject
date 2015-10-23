using System;
using System.Collections.Generic;

namespace Project_B.Models {
    public class CompetitionitemButFullModel : CompetitionItemBetShortModel {
        public Dictionary<DateTime, List<List<BetItem>>> BetItems { get; set; } 
        public bool HaveRegular { get; set; }
        public bool HaveLive { get; set; }
    }
}