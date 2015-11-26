using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum SportType : short {
        Unknown = 0,
        Football = 0x001,
        Tennis = 0x002,
        Basketball = 0x004,
        IceHockey = 0x008,
        Volleyball = 0x010,

        All = Football | Tennis | Basketball | IceHockey | Volleyball
    }
}