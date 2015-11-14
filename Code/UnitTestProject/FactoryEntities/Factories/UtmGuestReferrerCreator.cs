using System;
using MainLogic.Entities;

namespace UnitTestProject.FactoryEntities.Factories {
    class UtmGuestReferrerCreator : ICreator {
        public void Bind() {
            Factory.AddCreatorDao(() => {
                var guest = Factory.CreateDao<Guest>();
                return new UtmGuestReferrer {
                    Datecreated = DateTime.UtcNow,
                    GuestID = guest.ID,
                    Campaign = "campaign",
                    Medium = "medium",
                    Source = "source"
                };
            });
        }
    }
}
