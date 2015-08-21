using System;
using MainLogic.Entities;

namespace UnitTestProject.FactoryEntities.Factories {
    class GuestActionLogCreator : ICreator {
        public void Bind() {
            Factory.AddCreatorDao(() => {
                var guest = Factory.CreateDao<Guest>();
                var subdomainRule = Factory.CreateDao<UtmSubdomainRule>();
                return new GuestActionLog {
                    Datecreated = DateTime.Now,
                    GuestID = guest.ID,
                    Action = 0,
                    Arg = 0,
                    UtmsubdomainruleID = subdomainRule.ID
                };
            });
        }
    }
}
