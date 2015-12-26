using Project_B.CodeServerSide.Entity.Interface.NameConstraint;

namespace Project_B.CodeServerSide.Entity.Interface {
    public interface ICompetitor : ISportTyped, IGenderTyped, ILanguageTyped, IDateCreatedTyped {
        /// <summary>
        /// 
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int CompetitoruniqueID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Name { get; set; }
    }
}