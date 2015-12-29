using System.Collections.Generic;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Data {
    public class CompetitionParsed {
        public SportType Type { get; }
        public List<string> Name { get; }
        public List<MatchParsed> Matches { get; }

        public CompetitionParsed(List<string> competitionName) {
            Name = competitionName;
            Type = SportTypeHelper.Instance[competitionName];
            Matches = new List<MatchParsed>();
        }

        public CompetitionParsed(List<string> competitionName, SportType sportType) {
            Name = competitionName;
            Type = sportType;
            Matches = new List<MatchParsed>();
        }
    }
}