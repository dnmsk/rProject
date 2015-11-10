using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.Transport {
    public class CompetitorParsedTransport {
        public int UniqueID { get; set; }
        public LanguageType LanguageType { get; set; }
        public string NameFull { get; set; }
        public string NameShort { get; set; }
        public SportType SportType { get; set; }
        public GenderType GenderType { get; set; }
        //public DateTime DateCreatedUtc { get; set; }
    }
}