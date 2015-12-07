using MainLogic.Entities;
using MainLogic.Providers;
using MainLogic.Transport;
using MainLogic.Wrapper;
using NUnit.Framework;
using UnitTestProject.FactoryEntities;
using UnitTestProject.Helpers;

namespace UnitTestProject.Unit.MainLogic {
    [TestFixture]
    class UserProviderTest : DbTestBase {
        [TestCase("user agent", false)]
        [TestCase("bot", true)]
        public void CreateNewGuestTest(string userAgent, bool isBot) {
            string ip = "1.1.1.1";
            var guestID = new UserProvider()
                .CreateNewGuest(ip, userAgent);
            if (isBot) {
                Assert.AreEqual(0, Guest.DataSource.Count());
            }
            else {
                Assert.AreEqual(1, Guest.DataSource.Count());
                var guestEntity = Guest.DataSource.First();
                Assert.AreEqual(guestID, guestEntity.ID);
                Assert.AreEqual(ip, guestEntity.Ip);
            }
        }

        [Test]
        public void SaveReferrerTest() {
            var guest = Factory.CreateDao<Guest>();
            var referrer = "ref";
            var expected = "tar";
            new UserProvider().SaveReferrer(guest.ID, referrer, expected);
            Assert.AreEqual(1, GuestReferrer.DataSource.Count());
            var guestReferrer = GuestReferrer.DataSource.First();
            Assert.AreEqual(referrer, guestReferrer.Urlreferrer);
            Assert.AreEqual(expected, guestReferrer.Urltarget);
        }

        [Test]
        public void SaveTechInfoTest() {
            var guest = Factory.CreateDao<Guest>();
            var guestTechInfoTransport = new GuestTechInfoTransport {
                Version = 66,
                BrowserType = "Opr",
                Os = "osX",
                IsMobile = false,
                UserAgent = "UA"
            };
            new UserProvider().SaveTechInfo(guest.ID, guestTechInfoTransport);
            Assert.AreEqual(1, GuestTechInfo.DataSource.Count());
            Assert.AreEqual(1, GuestExistsBrowser.DataSource.Count());
            var guestTechInfo = GuestTechInfo.DataSource.First();
            var guestExistBrowser = GuestExistsBrowser.DataSource.First();
            Assert.AreEqual(guestTechInfoTransport.Version, guestExistBrowser.Version);
            Assert.AreEqual(guestTechInfoTransport.BrowserType, guestExistBrowser.Browsertype);
            Assert.AreEqual(guestTechInfoTransport.Os, guestExistBrowser.Os);
            Assert.AreEqual(guestTechInfoTransport.IsMobile, guestExistBrowser.Ismobile);
            Assert.AreEqual(guestTechInfoTransport.UserAgent.ToLower(), guestExistBrowser.Useragent);
            Assert.AreEqual(guestExistBrowser.ID, guestTechInfo.GuestexistsbrowserID);
            Assert.AreEqual(guest.ID, guestTechInfo.GuestID);
        }

        [Test]
        public void SaveUtm() {
            var guest = Factory.CreateDao<Guest>();
            var utmParamWrapper = new UtmParamWrapper("src", "cmp","mdm", true);
            new UserProvider().SaveUtm(guest.ID, utmParamWrapper);
            Assert.AreEqual(1, UtmGuestReferrer.DataSource.Count());
            var utmEntity = UtmGuestReferrer.DataSource.First();
            Assert.AreEqual(guest.ID, utmEntity.GuestID);
            Assert.AreEqual(utmParamWrapper.UtmCampaign, utmEntity.Campaign);
            Assert.AreEqual(utmParamWrapper.UtmMedium, utmEntity.Medium);
            Assert.AreEqual(utmParamWrapper.UtmSource, utmEntity.Source);
        }

        [Test]
        public void SaveGuestAction() {
            var guest = Factory.CreateDao<Guest>();
            var utmRule = Factory.CreateDao<UtmSubdomainRule>();
            var action = 666;
            var arg = 999;
            new UserProvider().SaveGuestAction(guest.ID, utmRule.ID, action, arg);
            Assert.AreEqual(1, GuestActionLog.DataSource.Count());
            var actionLog = GuestActionLog.DataSource.First();
            Assert.AreEqual(guest.ID, actionLog.GuestID);
            Assert.AreEqual(utmRule.ID, actionLog.UtmsubdomainruleID);
            Assert.AreEqual(action, actionLog.Action);
            Assert.AreEqual(arg, actionLog.Arg);
        }
        [Test]
        public void GetSubdomainRules() {
            var utmRule = Factory.CreateDao<UtmSubdomainRule>();
            var rules = new UserProvider().GetSubdomainRules();
            Assert.AreEqual(1, rules.Length);
            Assert.AreEqual(utmRule.ID, rules[0].ID);
            Assert.AreEqual(utmRule.Subdomainname, rules[0].SubdomainName);
            Assert.AreEqual(utmRule.Targetdomain, rules[0].TargetDomain);
        }
    }
}
