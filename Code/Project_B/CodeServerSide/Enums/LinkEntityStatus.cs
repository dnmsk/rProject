using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum LinkEntityStatus : short {
        /*
        Undefined = 0 => 0,
        ToLink = 1 => 0,
        Original = 2 => 3,
        LinkByStatistics = 3 => 5
        */
        Unlinked = 0,
        Linked = 0x01,
        Original = 0x02,
        LinkByStatistics = 0x04,
        ManualConfirmed = 0x08
    }
}