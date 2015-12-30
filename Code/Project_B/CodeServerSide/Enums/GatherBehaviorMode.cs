using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum GatherBehaviorMode : short {
        Default = 0x00,
        CreateOriginal = 0x01,
        CreateOriginalIfMatchedAll = 0x02,
        TryDetectAll = CanDetectCompetition | CanDetectCompetitor,
        CreateIfEmptyToDate = 0x04,
        CanDetectCompetition = 0x08,
        CanDetectCompetitor = 0x010,
        CreateNewLanguageName = 0x020,
    }
}