using System;
using MainLogic.Entities;

namespace UnitTestProject.FactoryEntities.Factories {
    class GuestExistBrowserCreator : ICreator {
        public void Bind() {
            Factory.AddCreatorDao(() => 
                new GuestExistsBrowser {
                    Isbot = false,
                    Datecreated = DateTime.UtcNow,
                    Version = 1,
                    Browsertype = "Opera",
                    Ismobile = false,
                    Os = "Mac",
                    Useragent = "Test user agent"
            });
        }
    }
}
