using System;
using NUnit.Framework;
using UnitTestProject.Unit.DAO.TestEntities;

namespace UnitTestProject.Unit.DAO {
	[TestFixture]
    public class EqualityTestDAO : DAO_TestBase {

		[Test]
		public void EqualsWithTwoNullObjectsReturnsTrue()
		{
			 const TestEntity obj1 = null;
             const TestEntity obj2 = null;
	 
			 var equality = Equals(obj1, obj2);
	 
			 Assert.AreEqual(true, equality);
		}
	 
		[Test]
		public void EqualsWithNullObjectReturnsFalse()
		{
            const TestEntity obj1 = null;
            var obj2 = new TestEntity() {
                Name = "Vasya",
            };
	 
			var equality = Equals(obj1, obj2);
	 
			Assert.AreEqual(false, equality);
		}
	 
		[Test]
		public void EqualsWithTransientObjectsReturnsFalse()
		{
            var obj1 = new TestEntity() {
                Name = "Vasya",
            };
            var obj2 = new TestEntity() {
                Name = "Vasya",
            };
	 
			var equality = Equals(obj1, obj2);
	 
			Assert.AreEqual(false, equality);
		}
	 
		[Test]
		public void EqualsWithOneTransientObjectReturnsFalse()
		{
            var obj1 = new TestEntity() {
                Name = "Vasya",
            };
            var obj2 = new TestEntity() {
                Name = "Vasya",
            };

		    obj1.Save();
	 
			var equality = Equals(obj1, obj2);
	 
			Assert.AreEqual(false, equality);
		}
	 
		[Test]
		public void EqualsWithDifferentIdsReturnsFalse()
		{
            var obj1 = new TestEntity() {
                Name = "Vasya",
            };
            var obj2 = new TestEntity() {
                Name = "Vasya",
            };

		    obj1.Save();
		    obj2.Save();
	 
			var equality = Equals(obj1, obj2);
	 
			Assert.AreEqual(false, equality);
		}
	 
		[Test]
		public void EqualsWithSameIdsReturnsTrue()
		{
            var obj1 = new TestEntity() {
                Name = "Vasya",
            };

		    obj1.Save();
		    var obj2 = TestEntity.DataSource.First();

            foreach (TestEntity.Fields en in Enum.GetValues(typeof(TestEntity.Fields))) {
                Assert.AreEqual(obj1[en], obj2[en]);   
		    }
		}
	 
 
		[Test]
		public void EqualsWithSameIdsInDisparateClassesReturnsFalse()
		{
            var obj1 = new TestEntity() {
                ID = 1,
                Name = "Vasya",
            };
            var obj2 = new OtherTestEntity() {
                ID = 1,
                Name = "Vasya",
            };
		    
            obj1.Save();
		    obj2.Save();

			var equality = Equals(obj1, obj2);
	 
			Assert.AreEqual(false, equality);
		}
	}
}
