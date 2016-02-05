using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum LinkEntityStatus : short {
        Unlinked = 0,
        Linked = 0x01,
        Original = 0x02,
        LinkByStatistics = 0x04,
        ManualConfirmed = 0x08,
        LinkByRelinker = 0x010,
        LinkByRelinkerSub = 0x020,
    }
}