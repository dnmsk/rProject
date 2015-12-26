using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum GatherBehaviorMode : short {
        Default = 0x00,
        CreateOriginal = 0x01,
        TryDetectAll = CanDetectCompetition | CanDetectCompetitor,
        CreateIfEmptyToDate = 0x02,
        CanDetectCompetition = 0x04,
        CanDetectCompetitor = 0x08,
        CreateNewLanguageName = 0x010
    }
}