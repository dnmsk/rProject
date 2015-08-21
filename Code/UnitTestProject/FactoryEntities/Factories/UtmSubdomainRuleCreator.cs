using System;
using MainLogic.Entities;

namespace UnitTestProject.FactoryEntities.Factories {
    class UtmSubdomainRuleCreator : ICreator {
        public void Bind() {
            Factory.AddCreatorDao(() => {
                return new UtmSubdomainRule {
                    Datecreated = DateTime.Now,
                    Subdomainname = "subdomain",
                    Targetdomain = "target"
                };
            });
        }
    }
}
