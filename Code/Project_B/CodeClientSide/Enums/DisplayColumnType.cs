using System;

namespace Project_B.CodeClientSide.Enums {
    [Flags]
    public enum DisplayColumnType : short {
        None = 0,
        All = TraditionalOdds | AdditionalOdds | HandicapOdds | TotalOdds | MaximalOdds | MinimalOdds | Result | Roi1X2,
        TraditionalOdds = 0x001,
        AdditionalOdds = 0x002,
        HandicapOdds = 0x004,
        TotalOdds = 0x008,
        MaximalOdds = 0x010,
        MinimalOdds = 0x020,
        Result = 0x040,
        Roi1X2 = 0x080
    }
}