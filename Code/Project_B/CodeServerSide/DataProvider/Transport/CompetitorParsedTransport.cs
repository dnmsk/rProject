using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.Transport {
    public class CompetitorParsedTransport {
        public int RawID { get; set; }
        public int UniqueID { get; set; }
        public LanguageType LanguageType { get; set; }
        public string Name { get; set; }
        public SportType SportType { get; set; }
        public GenderType GenderType { get; set; }
    }
}