using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project_B.Code.Enums;

namespace Project_B.Models {
    public class CompetitionItemRegilarModel : CompetitionItemShortModel {
        public Dictionary<BetOddType, BetItem> CurrentBets { get; set; } 
        public Dictionary<BetOddType, BetItem> HistoryMinBets { get; set; } 
        public Dictionary<BetOddType, BetItem> HistoryMaxBets { get; set; }
    }
}