using System.Collections.Generic;
using System.Linq;
using Project_B.CodeServerSide.Entity.Data;
using Project_B.CodeServerSide.Entity.Helper;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.TransportType {
    public class RoiTransport {
        public RoiType RoiType { get; set; }
        public float Roi { get; set; }
        public Dictionary<BetOddType, BetItemTransport> Odds { get; set; }

        public static RoiTransport BuildRoiTransport(CompetitionItemRoiRow roi, Dictionary<int, IBet<int>> betMap) {
            var oddsGetterMap = BetMappingHelper<int>.OddsGetterMap;
            var odds = roi.BetIDs
                .Select(b => new KeyValuePair<BetOddType, BetItemTransport>(b.Key, oddsGetterMap[b.Key](betMap[b.Value])))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            return new RoiTransport {
                RoiType = roi.RoiType,
                Roi = roi.Roi,
                Odds = odds
            };
        }
    }
}