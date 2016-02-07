using System.Collections.Generic;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.Data {
    public class CompetitionItemRoiRow {
        public int ID { get; set; }
        public float Roi { get; set; }
        public RoiType RoiType { get; set; }
        /// <summary>
        /// Order: [1, 2, X], [Hcap1, Hcap2], [TotalUnder, TotalOver], [1, X2], [12, X], [1X, 2]. 
        /// </summary>
        public Dictionary<BetOddType, int> BetIDs { get; set; }
    }
}