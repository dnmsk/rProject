using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum SystemStateResultType : short {
        Unknown = 0,
        CollectForToday = 0x01,
        CollectForYesterday = 0x02
    }
}