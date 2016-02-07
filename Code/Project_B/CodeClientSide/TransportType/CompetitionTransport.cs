using System.Collections.Generic;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.TransportType {
    public class CompetitionTransport {
        public int ID { get; set; }
        public string Name { get; set; }
        public SportType SportType { get; set; }
        public LanguageType Language { get; set; }
    }

    public class CompetitionTransport<T> : CompetitionTransport where T : CompetitionItemShortTransport {
        public List<T> CompetitionItems { get; set; }

        public CompetitionTransport() {
            CompetitionItems = new List<T>();
        }
    }
}