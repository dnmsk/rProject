using CommonUtils.ExtendedTypes;
using MainLogic.Entities;
using MainLogic.Providers;
using NUnit.Framework;
using UnitTestProject.FactoryEntities;
using UnitTestProject.Helpers;

namespace UnitTestProject.Unit.MainLogic {
    [TestFixture]
    class AccountProviderTest : DbTestBase {
        [TestCase("test@test.test", "1234")]
        [TestCase("test.test@test.test", "4321")]
        public void RegisterWithEmailTest(string email, string password) {
            var guest = Factory.CreateDao<Guest>();
            var registerResult = new AccountProvider()
                .RegisterWithEmail(guest.ID, email, password);
            Assert.True(registerResult);
            Assert.AreEqual(1, AccountIdentity.DataSource.Count());
            var entity = AccountIdentity.DataSource.First();
            Assert.AreEqual(email, entity.Email);
            Assert.AreEqual(password.GetMD5(), entity.Password);
        }

        [TestCase("test@test.test", "1234")]
        [TestCase("test.test@test.test", "4321")]
        public void LoginWithEmailTest(string email, string password) {
            var accountIdentity = Factory.CreateDao<AccountIdentity>(identity => {
                identity.Email = email;
                identity.Password = password.GetMD5();
            });
            var loginResult = new AccountProvider()
                .LoginWithEmail(email, password);
            Assert.AreEqual(accountIdentity.ID, loginResult.Item2);
        }
    }
}
