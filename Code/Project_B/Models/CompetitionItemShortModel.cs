using System;
using Project_B.Code.Enums;

namespace Project_B.Models {
    public class CompetitionItemShortModel {
        public int ID { get; set; }
        public DateTime DateUtc { get; set; }
        public CompetitorModel Competitor1 { get; set; }
        public CompetitorModel Competitor2 { get; set; }
        public SportType SportType { get; set; }
        public CompetitionModel Competition { get; set; }
    }
}