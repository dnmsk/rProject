using System;
using System.Collections.Generic;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Data {
    public class MatchParsed {
        public SportType SportType { get; }
        public int BrokerMatchID { get; set; }
        public string[] CompetitorName1 { get; set; }
        public string[] CompetitorName2 { get; set; }
        public DateTime DateUtc { get; set; }
        public List<OddParsed> Odds { get; }
        public FullResult Result { get; set; }

        public MatchParsed(SportType sportType) {
            SportType = sportType;
            Odds = new List<OddParsed>();
        }
    }
}