using NUnit.Framework;
using UnitTestProject.Unit.DAO.TestEntities;

namespace UnitTestProject.Unit.DAO {
    public abstract class DAO_TestBase {
        [SetUp]
        protected void ClearDb() {
            TestEntity.DataSource.Delete();
            OtherTestEntity.DataSource.Delete();
        }
    }
}
