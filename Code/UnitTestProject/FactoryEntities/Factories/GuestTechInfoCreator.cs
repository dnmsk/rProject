using System;
using MainLogic.Entities;

namespace UnitTestProject.FactoryEntities.Factories {
    class GuestTechInfoCreator : ICreator {
        public void Bind() {
            Factory.AddCreatorDao(() => {
                var guest = Factory.CreateDao<Guest>();
                var guestExistBrowser = Factory.CreateDao<GuestExistsBrowser>();
                return new GuestTechInfo {
                    GuestID = guest.ID,
                    Datecreated = DateTime.UtcNow,
                    GuestexistsbrowserID = guestExistBrowser.ID
                };
            });
        }
    }
}
