using NUnit.Framework;
using TestEntity = UnitTestProject.Unit.DAO.TestEntities.TestEntity;

namespace UnitTestProject.Unit.DAO {
    /// <summary>
    /// Тест на метод Save
    /// </summary>
    [TestFixture]
    public class SaveTests : DAO_TestBase {
        /// <summary>
        /// TODO : Добавить описание.
        /// </summary>
        [Test]
        public void SucessTest() {
            // NOTE : Arrange.
            var customer = new TestEntity {
                Name = "Vasya",
            };
            customer.Save();

            // NOTE : Act.
            var res = customer.Save();

            // NOTE : Assert.
            Assert.True(res);
        }

        /// <summary>
        /// Таких айдишников в базе нету - соответственно записи не должен проапдейтить
        /// </summary>
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1234567)]
        public void FalseTest(int id) {
            // NOTE : Arrange.
            var customer = new TestEntity {
                Name = "Vasya",
            };
            customer.Save();
            customer.ID = id;

            // NOTE : Act.
            var res = customer.Save();

            // NOTE : Assert.
            Assert.False(res);
        }
   }
}