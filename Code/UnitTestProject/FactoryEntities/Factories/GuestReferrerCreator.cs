using System;
using MainLogic.Entities;

namespace UnitTestProject.FactoryEntities.Factories {
    class GuestReferrerCreator : ICreator {
        public void Bind() {
            Factory.AddCreatorDao(() => {
                var guest = Factory.CreateDao<Guest>();
                return new GuestReferrer {
                    Datecreated = DateTime.Now,
                    GuestID = guest.ID,
                    Urlreferrer = "referrer",
                    Urltarget = "target"
                };
            });
        }
    }
}
