using System;
using MainLogic.Entities;

namespace UnitTestProject.FactoryEntities.Factories {
    class GuestCreator : ICreator {
        public void Bind() {
            Factory.AddCreatorDao(() => new Guest {
                Datecreated = DateTime.Now,
                Ip = "8.8.8.8"
            });
        }
    }
}
