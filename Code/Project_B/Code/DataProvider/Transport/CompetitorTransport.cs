using System;
using Project_B.Code.Enums;

namespace Project_B.Code.DataProvider.Transport {
    public class CompetitorTransport {
        public int UniqueID { get; set; }
        public LanguageType LanguageType { get; set; }
        public string NameFull { get; set; }
        public string NameShort { get; set; }
        public SportType SportType { get; set; }
        public GenderType GenderType { get; set; }
        //public DateTime DateCreatedUtc { get; set; }
    }
}