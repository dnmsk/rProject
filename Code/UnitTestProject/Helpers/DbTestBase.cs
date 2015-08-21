using NUnit.Framework;

namespace UnitTestProject.Helpers {
    abstract class DbTestBase {
        [SetUp]
        public void SetUp() {
            new DbTestHelper().Clear();
        }
        
        [TestFixtureTearDown]
        public void FixtureTearDown() {
            new DbTestHelper().Clear();
        }
    }
}
