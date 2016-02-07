using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum RoiType : short {
        Undefined = 0,
        All = Roi1X2 | RoiHandicap | RoiTotal | Roi1X_2 | Roi12_X | Roi1_X2,
        Roi1X2 = 0x01,
        // ReSharper disable once InconsistentNaming
        Roi1X_2 = 0x02,
        // ReSharper disable once InconsistentNaming
        Roi12_X = 0x04,
        // ReSharper disable once InconsistentNaming
        Roi1_X2 = 0x08,
        RoiHandicap = 0x010,
        RoiTotal = 0x020,
    }
}