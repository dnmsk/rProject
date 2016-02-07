using System;

namespace Project_B.CodeClientSide.Enums {
    [Flags]
    public enum DisplayColumnType : short {
        None = 0,
        All = BaseOdds | AdditionalOdds | HandicapOdds | TotalOdds | MaximalOdds | MinimalOdds | Result | Roi,
        TraditionalOdds = BaseOdds | HandicapOdds | TotalOdds,
        AllOdds = TraditionalOdds | AdditionalOdds,
        BaseOdds = 0x001,
        AdditionalOdds = 0x002,
        HandicapOdds = 0x004,
        TotalOdds = 0x008,
        MaximalOdds = 0x010,
        MinimalOdds = 0x020,
        Result = 0x040,
        Roi = 0x080
    }
}