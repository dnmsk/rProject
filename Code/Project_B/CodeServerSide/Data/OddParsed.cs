using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Data {
    public class OddParsed {
        public BetOddType Type { get; set; }
        public float? AdvancedParam { get; set; }
        public float Factor { get; set; }
        public string OddStringConfirmation { get; set; }
    }
}