using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO.Exceptions;
using IDEV.Hydra.DAO.MassTools;
using NUnit.Framework;
using UnitTestProject.Helpers;
using UnitTestProject.Unit.DAO.TestEntities;

namespace UnitTestProject.Unit.DAO {
    [TestFixture]
    public class EntityExtentionTest {

        [SetUp]
        [TestFixtureTearDown]
        public void SetUp() {
            TestEntity.DataSource.Delete();
        }

        /// <summary>
        /// Простая вставка.
        /// </summary>
        [Test]
        public void InsertTest() {
            // NOTE : Arrange.
            var listToSave = new[] {
                new TestEntity {
                    Name = "1",
                    Enum = TestEnum.One,
                },
                new TestEntity {
                    Name = "2",
                    Enum = TestEnum.Two,
                },
            };

            // NOTE : Act.
            listToSave.Save();

            // NOTE : Assert.
            TestEntity.DataSource
                .Sort(TestEntity.Fields.ID)
                .AsList()
                .Verify(listToSave,
                    u => u.Name,
                    u => u.Enum);
        }

        /// <summary>
        /// Обновление.
        /// </summary>
        [Test]
        public void UpdateTest() {
            // NOTE : Arrange.
            var listToUpdate = new[] {
                new TestEntity {
                    Name = "1",
                    Enum = TestEnum.One,
                },
                new TestEntity {
                    Name = "2",
                    Enum = TestEnum.Two,
                },
            };
            listToUpdate.Each(e => e.Save());
            listToUpdate.Each(e => e.Name += "modify");

            // NOTE : Act.
            listToUpdate.Save();

            // NOTE : Assert.
            TestEntity.DataSource
                .Sort(TestEntity.Fields.ID)
                .AsList()
                .Verify(listToUpdate,
                    u => u.Name,
                    u => u.Enum);
        }

        /// <summary>
        /// Обновление.
        /// </summary>
        [Test]
        [ExpectedException(typeof(DAOException))]
        public void UpdateDifferentFieldExceptionTest() {
            // NOTE : Arrange.
            var listToUpdate = new[] {
                new TestEntity {
                    Name = "1",
                    Enum = TestEnum.One,
                },
                new TestEntity {
                    Name = "2",
                    Enum = TestEnum.Two,
                },
            };
            listToUpdate.Each(e => e.Save());
            listToUpdate.Each(e => e.Name += "modify");
            listToUpdate[1].Enum = TestEnum.Three;

            // NOTE : Act.
            listToUpdate.Save();

            // NOTE : Assert.
            TestEntity.DataSource
                .Sort(TestEntity.Fields.ID)
                .AsList()
                .Verify(listToUpdate,
                    u => u.Name,
                    u => u.Enum);
        }
    }
}
