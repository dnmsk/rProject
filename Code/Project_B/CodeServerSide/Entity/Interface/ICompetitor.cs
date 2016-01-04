using Project_B.CodeServerSide.Entity.Interface.NameConstraint;

namespace Project_B.CodeServerSide.Entity.Interface {
    public interface ICompetitor : ISportTyped, IGenderTyped, ILanguageTyped, IDateCreatedTyped, INamedEntity, IKeyBrokerEntity {
        /// <summary>
        /// 
        /// </summary>
        int CompetitoruniqueID { get; set; }
    }
}