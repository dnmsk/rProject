using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum SystemStateBetType {
        Unknown = 0,
        BetFor0_1 = 0x0000001,
        BetFor1_2 = 0x0000002,
        BetFor2_3 = 0x0000004,
        BetFor3_4 = 0x0000008,
        BetFor4_5 = 0x0000010,
        BetFor5_6 = 0x0000020,
        BetFor6_7 = 0x0000040,
        BetFor7_8 = 0x0000080,
        BetFor8_9 = 0x0000100,
        BetFor9_10 = 0x0000200,
        BetFor10_11 = 0x0000400,
        BetFor11_12 = 0x0000800,
        BetFor12_13 = 0x0001000,
        BetFor13_14 = 0x0002000,
        BetFor14_15 = 0x0004000,
        BetFor15_16 = 0x0008000,
        BetFor16_17 = 0x0010000,
        BetFor17_18 = 0x0020000,
        BetFor18_19 = 0x0040000,
        BetFor19_20 = 0x0080000,
        BetFor20_21 = 0x0100000,
        BetFor21_22 = 0x0200000,
        BetFor22_23 = 0x0400000,
        BetFor23_24 = 0x0800000,
    }
}