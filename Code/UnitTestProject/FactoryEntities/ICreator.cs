namespace UnitTestProject.FactoryEntities {
    internal interface ICreator {
        /// <summary>
        /// Связать фабрику с функциями создания сущностей.
        /// </summary>
        void Bind();
    }
}
