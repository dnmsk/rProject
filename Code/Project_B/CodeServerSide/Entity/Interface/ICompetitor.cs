using Project_B.CodeServerSide.Entity.Interface.NameConstraint;

namespace Project_B.CodeServerSide.Entity.Interface {
    public interface ICompetitor : ISportTyped, IGenderTyped, ILanguageTyped, IDateCreatedTyped, INamedEntity {
        /// <summary>
        /// 
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int CompetitoruniqueID { get; set; }
    }
}