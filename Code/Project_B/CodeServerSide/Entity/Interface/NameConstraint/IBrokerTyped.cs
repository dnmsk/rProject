using System;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.Interface.NameConstraint {
    public interface IBrokerTyped {
        /// <summary>
        /// 
        /// </summary>
        BrokerType BrokerID { get; set; }
        Enum BrokerField { get; }
    }
}