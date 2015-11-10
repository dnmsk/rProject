using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum GatherBehaviorMode : short {
        Default = 0x00,
        CreateIfNew = 0x01,
        CreateIfEmptyToDate = 0x02,
        CanDetectCompetition = 0x04,
        CanDetectCompetitor = 0x08,
    }
}