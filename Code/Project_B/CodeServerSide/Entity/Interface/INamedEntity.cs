namespace Project_B.CodeServerSide.Entity.Interface {
    public interface INamedEntity : IKeyBrokerEntity {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; set; }
    }
}