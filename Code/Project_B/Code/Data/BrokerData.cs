using System.Collections.Generic;
using Project_B.Code.Enums;

namespace Project_B.Code.Data {
    public class BrokerData {
        public BrokerType Broker { get; set; }
        public LanguageType Language { get; set; }
        public List<CompetitionParsed> Competitions { get; set; } 
    }
}