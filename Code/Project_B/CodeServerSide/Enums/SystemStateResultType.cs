using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum SystemStateResultType : short {
        Unknown = 0,
        CollectForToday = 0x01,
        CollectForTwoDayAgo = 0x02,
        CollectForYesterday = 0x04
    }
}