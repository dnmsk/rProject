using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum GatherBehaviorMode : short {
        Default = 0x00,
        CreateOriginal = 0x01,
        CreateOriginalIfMatchedAll = 0x02,
        TryDetectAll = 0x04,
        CreateNewLanguageName = 0x08,
    }
}