using System;

namespace Project_B.CodeServerSide.Enums {
    [Flags]
    public enum BrokerType : short {
        Default = 0,
        RedBlue = 0x01,

    }
}