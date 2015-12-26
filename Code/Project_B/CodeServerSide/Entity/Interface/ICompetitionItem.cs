using System;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;

namespace Project_B.CodeServerSide.Entity.Interface {
    public interface ICompetitionItem : ISportTyped, IDateCreatedTyped {
        /// <summary>
        /// 
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int CompetitionuniqueID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int CompetitionSpecifyUniqueID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Competitoruniqueid1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Competitoruniqueid2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        DateTime Dateeventutc { get; set; }
    }
}