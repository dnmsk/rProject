using System.Collections.Generic;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO.Exceptions;
using NUnit.Framework;
using UnitTestProject.Unit.DAO.TestEntities;
using TestEntity = UnitTestProject.Unit.DAO.TestEntities.TestEntity;

namespace UnitTestProject.Unit.DAO {
    /// <summary>
    /// Тест на правильный маппинг сущностей через  DAO
    /// </summary>
    [TestFixture]
    public class DAO_Test : DAO_TestBase {

        /// <summary>
        /// TODO : Добавить описание.
        /// </summary>
        [Test]
        public void CustomersEmptyTest() {
            // NOTE : Arrange.

            // NOTE : Act.
            IList<TestEntity> customers = TestEntity.DataSource.AsList();

            // NOTE : Assert.
            Assert.NotNull(customers);
            Assert.AreEqual(0, customers.Count);
        }

        /// <summary>
        /// TODO : Добавить описание.
        /// </summary>
        [Test]
        public void CreateCustomersTest() {
            // NOTE : Arrange.
            var customer = new TestEntity {
                Name = "Vasya",
            };

            // NOTE : Act.
            customer.Save();
            var customers = TestEntity.DataSource.AsList();

            // NOTE : Assert.
            Assert.NotNull(customers);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual(customer.Name, customers[0].Name);
        }


        /// <summary>
        /// TODO : Добавить описание.
        /// </summary>
        [Test]
        [ExpectedException(typeof(DAOException))]
        public void RestrictionLengthTest() {
            // NOTE : Arrange.
            string name = "a";
            0.Step(1000, (i) => {
                name += "a";
            });
            var customer = new TestEntity {
                Name = name
            };

            // NOTE : Act.
            customer.Save();
        }

        /// <summary>
        /// TODO : Добавить описание.
        /// </summary>
        [Test]
        public void NullFieldTest() {
            // NOTE : Arrange.
            var customer = new TestEntity {
                Name = "Vasya",
                Enum = null
            };

            // NOTE : Act.
            customer.Save();
            IList<TestEntity> customers = TestEntity.DataSource.AsList();

            // NOTE : Assert.
            Assert.NotNull(customers);
            Assert.AreEqual(1, customers.Count);
            Assert.Null(customers[0].Enum);
        }

        /// <summary>
        /// TODO : Добавить описание.
        /// </summary>
        [Test]
        public void EnumParsingTest() {
            // NOTE : Arrange.
            var customer = new TestEntity {
                Name = "Vasya",
                Enum = TestEnum.Three
            };

            // NOTE : Act.
            customer.Save();
            IList<TestEntity> customers = TestEntity.DataSource.AsList();

            // NOTE : Assert.
            Assert.NotNull(customers);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual(TestEnum.Three, customers[0].Enum);
        }

        /// <summary>
        /// TODO : Добавить описание.
        /// </summary>
        [Test]
        public void UpdateTest() {
            // NOTE : Arrange.
            var customer = new TestEntity {
                Name = "Vasya",
                Enum = TestEnum.Three
            };
            customer.Save();
            long id = customer.ID;
            
            customer.Enum = TestEnum.Two;
            // NOTE : Act.
            customer.Save();

            IList<TestEntity> customers = TestEntity.DataSource.AsList();

            // NOTE : Assert.
            Assert.NotNull(customers);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual(TestEnum.Two, customers[0].Enum);
        }

        /// <summary>
        /// TODO : Добавить описание.
        /// </summary>
        [Test]
        public void DeleteTest() {
            // NOTE : Arrange.
            var customer = new TestEntity {
                Name = "Vasya",
            };
            customer.Save();

            // NOTE : Act.
            customer.Delete();

            IList<TestEntity> customers = TestEntity.DataSource.AsList();

            // NOTE : Assert.
            Assert.NotNull(customers);
            Assert.AreEqual(0, customers.Count);
        }
    }
}