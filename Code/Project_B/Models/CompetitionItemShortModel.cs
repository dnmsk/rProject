using System;
using Project_B.Code.Enums;

namespace Project_B.Models {
    public class CompetitionItemShortModel {
        public CompetitionItemShortModel() {}

        protected CompetitionItemShortModel(CompetitionItemShortModel competitionItemShortModel) {
            CompetitionID = competitionItemShortModel.CompetitionID;
            DateUtc = competitionItemShortModel.DateUtc;
            Competitor1 = competitionItemShortModel.Competitor1;
            Competitor2 = competitionItemShortModel.Competitor2;
            SportType = competitionItemShortModel.SportType;
            Competition = competitionItemShortModel.Competition;
        }

        public int CompetitionID { get; set; }
        public DateTime DateUtc { get; set; }
        public CompetitorModel Competitor1 { get; set; }
        public CompetitorModel Competitor2 { get; set; }
        public SportType SportType { get; set; }
        public CompetitionModel Competition { get; set; }
    }
}