using Project_B.CodeServerSide.Entity.Interface.NameConstraint;

namespace Project_B.CodeServerSide.Entity.Interface {
    public interface ICompetitionSpecify : ISportTyped, ILanguageTyped, IGenderTyped, IDateCreatedTyped, INamedEntity, IKeyBrokerEntity {
        /// <summary>
        /// 
        /// </summary>
        int CompetitionuniqueID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int CompetitionSpecifyUniqueID { get; set; }
    }
}