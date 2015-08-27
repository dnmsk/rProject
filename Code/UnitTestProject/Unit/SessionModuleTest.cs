using MainLogic.WebFiles;
using NUnit.Framework;

namespace UnitTestProject.Unit {
    [TestFixture]
    public class SessionModuleTest {
        [TestCase(0, 0, "{\"AccountID\":0,\"GuestID\":0}")]
        [TestCase(0, 31, "{\"AccountID\":0,\"GuestID\":31}")]
        [TestCase(13, 0, "{\"AccountID\":13,\"GuestID\":0}")]
        [TestCase(13, 31, "{\"AccountID\":13,\"GuestID\":31}")]
        public void SerializeSessionModuleTest(int accountID, int guestID, string expected) {
            var actual = new SessionModule(guestID, accountID).ModuleToString();
            Assert.AreEqual(expected, actual);
        }
        
        [TestCase(0, 0, "{}")]
        [TestCase(0, 0, "{\"AccountID\":0,\"GuestID\":0}")]
        [TestCase(0, 31, "{\"GuestID\":31}")]
        [TestCase(0, 31, "{\"AccountID\":0,\"GuestID\":31}")]
        [TestCase(13, 0, "{\"AccountID\":13}")]
        [TestCase(13, 0, "{\"AccountID\":13,\"GuestID\":0}")]
        [TestCase(13, 31, "{\"AccountID\":13,\"GuestID\":31}")]
        public void DeserializeSessionModuleTest(int accountID, int guestID, string serialized) {
            var actual = SessionModule.Deserialize(serialized);
            Assert.AreEqual(accountID, actual.AccountID);
            Assert.AreEqual(guestID, actual.GuestID);
        }

        [Test]
        public void IsAthenticatedTest() {
            var actual = new SessionModule(13, 31);
            Assert.True(actual.IsAuthenticated());
        }
    }
}
