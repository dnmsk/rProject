using System;
using System.Collections.Generic;
using Project_B.CodeServerSide.Data.Result;

namespace Project_B.CodeServerSide.Data {
    public class MatchParsed {
        public int BrokerMatchID { get; set; }
        public string CompetitorNameShortOne { get; set; }
        public string CompetitorNameFullOne { get; set; }
        public string CompetitorNameShortTwo { get; set; }
        public string CompetitorNameFullTwo { get; set; }
        public DateTime DateUtc { get; set; }
        public List<OddParsed> Odds { get; }
        public FullResult Result { get; set; }

        public MatchParsed() {
            Odds = new List<OddParsed>();
        }
    }
}