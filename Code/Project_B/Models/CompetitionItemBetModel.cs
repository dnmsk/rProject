using System.Collections.Generic;
using Project_B.Code.Enums;

namespace Project_B.Models {
    public class CompetitionItemBetModel : CompetitionItemShortModel {
        public Dictionary<BetOddType, BetItem> CurrentBets { get; set; } 
        public Dictionary<BetOddType, BetItem> HistoryMinBets { get; set; } 
        public Dictionary<BetOddType, BetItem> HistoryMaxBets { get; set; }
    }
}