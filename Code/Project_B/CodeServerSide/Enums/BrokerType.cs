using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum BrokerType : int {
        Default = 0,
        RedBlue = 0x01,
        GrayBlue = 0x02,
    }
}