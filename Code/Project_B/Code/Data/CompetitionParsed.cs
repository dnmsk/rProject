using System.Collections.Generic;
using Project_B.Code.Enums;

namespace Project_B.Code.Data {
    public class CompetitionParsed {
        public SportType Type { get; set; }
        public List<string> Name { get; set; }
        public List<MatchParsed> Matches { get; set; }

        public CompetitionParsed() {
            Name = new List<string>();
            Matches = new List<MatchParsed>();
        }
    }
}