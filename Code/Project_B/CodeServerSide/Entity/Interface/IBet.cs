using System;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.Interface {
    public interface IBet<T> {
        /// <summary>
        /// 
        /// </summary>
        T ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int CompetitionitemID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        BrokerType BrokerID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Win1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Win2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Hcap1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Hcap2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Hcapdetail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Totalunder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Totalover { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Totaldetail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DateTime Datecreatedutc { get; set; }

        bool Save();
        TE GetJoinedEntity<TE>();
    }
}