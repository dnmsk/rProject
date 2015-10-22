using System;
using Project_B.Code.Enums;

namespace Project_B.Models {
    public class BetItem {
        public DateTime DateTimeUtc { get; set; }
        public float Odd { get; set; }
        public float AdvancedParam { get; set; }
        public BrokerType BrokerType { get; set; }
    }
}