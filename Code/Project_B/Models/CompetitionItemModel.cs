using System;
using System.Collections.Generic;
using Project_B.Code.Data.Result;
using Project_B.Code.Enums;

namespace Project_B.Models {
    public class CompetitionItemModel {
        public int ID { get; set; }
        public int CompetitionID { get; set; }
        public string CompetitionName { get; set; }
        public DateTime EventDateUtc { get; set; }
        public CompetitorModel Competitor1 { get; set; }
        public CompetitorModel Competitor2 { get; set; }
        public SportType SportType { get; set; }
        public List<OddsModel> Odds { get; set; } 
        public FullResult FullResult { get; set; }
        public List<FullResult> LiveResults { get; set; } 
        public List<OddsModel> LiveOdds { get; set; } 
    }
}