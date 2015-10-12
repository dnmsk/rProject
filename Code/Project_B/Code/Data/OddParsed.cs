using Project_B.Code.Enums;

namespace Project_B.Code.Data {
    public class OddParsed {
        public BetOddType Type { get; set; }
        public decimal? AdvancedParam { get; set; }
        public decimal Factor { get; set; }
        public string OddStringConfirmation { get; set; }
    }
}