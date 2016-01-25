using System.Collections.Generic;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Data {
    public class CompetitionParsed {
        public SportType Type { get; }
        public string[] Name { get; }
        public List<MatchParsed> Matches { get; }

        public CompetitionParsed(string[] competitionName, SportType sportType = SportType.Unknown) {
            Name = competitionName;
            Type = sportType != SportType.Unknown ? sportType : SportTypeHelper.Instance[competitionName];
            Matches = new List<MatchParsed>();
        }
    }
}