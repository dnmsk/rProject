using Project_B.CodeServerSide.Entity.Interface.NameConstraint;

namespace Project_B.CodeServerSide.Entity.Interface {
    public interface ICompetition : IDateCreatedTyped, ISportTyped, IGenderTyped, ILanguageTyped, INamedEntity {
        /// <summary>
        /// 
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int CompetitionuniqueID { get; set; }
    }
}