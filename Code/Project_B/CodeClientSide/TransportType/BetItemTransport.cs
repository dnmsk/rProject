using System;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeClientSide.TransportType {
    public class BetItemTransport {
        public DateTime DateTimeUtc { get; set; }
        public float Odd { get; set; }
        public float AdvancedParam { get; set; }
        public BrokerType BrokerType { get; set; }
    }
}