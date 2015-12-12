namespace Project_B.CodeServerSide.Entity.Data {
    public class CompetitionItemRoiRow {
        public int ID { get; set; }
        public float RoiMax { get; set; }
        public float Roi1X2 { get; set; }
        public float RoiHcap { get; set; }
        public float RoiTotal { get; set; }
        public float Roi1X_2 { get; set; }
        public float Roi1_X2 { get; set; }
        /// <summary>
        /// Order: [1, 2, X], [Hcap1, Hcap2, ], [TotalUnder, TotalOver, ]. Max 3 values. 
        /// </summary>
        public int[] BetIDs { get; set; }
    }
}