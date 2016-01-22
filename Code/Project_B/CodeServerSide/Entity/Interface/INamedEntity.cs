namespace Project_B.CodeServerSide.Entity.Interface {
    public interface INamedEntity : IUniqueID {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; set; }
    }
}