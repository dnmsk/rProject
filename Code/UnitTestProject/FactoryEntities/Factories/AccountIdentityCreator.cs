using System;
using CommonUtils.ExtendedTypes;
using MainLogic.Entities;

namespace UnitTestProject.FactoryEntities.Factories {
    public class AccountIdentityCreator : ICreator {
        public void Bind() {
            Factory.AddCreatorDao(() => {
                var guest = Factory.CreateDao<Guest>();
                return new AccountIdentity {
                    Datecreated = DateTime.UtcNow,
                    GuestID = guest.ID,
                    Email = "test@test.test",
                    Password = "test".GetMD5()
                };
            });
        }
    }
}
