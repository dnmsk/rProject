using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.Transport {
    public class CompetitionSpecifyTransport {
        public int CompetitionUniqueID { get; set; }
        public int CompetitionSpecifyUniqueID { get; set; }
        public int RawCompetitionID { get; set; }
        public int RawCompetitionSpecifyID { get; set; }
        public LanguageType LanguageType { get; set; }
        public string Name { get; set; }
        public SportType SportType { get; set; }
        public GenderType GenderType { get; set; }
    }
}