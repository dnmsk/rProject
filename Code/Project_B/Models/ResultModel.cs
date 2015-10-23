using Project_B.Code.Enums;

namespace Project_B.Models {
    public class ResultModel {
        public int CompetitionID { get; set; }
        public short ScoreID { get; set; }
        public BetOddType ResultType { get; set; }
        public short[] SubScore { get; set; }
    }
}