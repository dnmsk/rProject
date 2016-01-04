using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.Transport {
    public class CompetitionSpecifyTransport : IKeyBrokerEntity {
        public int CompetitionUniqueID { get; set; }
        public int ID { get; set; }
        public LanguageType LanguageType { get; set; }
        public SportType SportType { get; set; }
        public GenderType GenderType { get; set; }
    }
}