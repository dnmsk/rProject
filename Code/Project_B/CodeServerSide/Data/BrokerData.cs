using System.Collections.Generic;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Data {
    public class BrokerData {
        public BrokerType Broker { get; }
        public LanguageType Language { get; }
        public List<CompetitionParsed> Competitions { get; }

        public BrokerData(BrokerType brokerType, LanguageType language, List<CompetitionParsed> competitions = null) {
            Broker = brokerType;
            Language = language;
            Competitions = competitions ?? new List<CompetitionParsed>();
        }
    }
}