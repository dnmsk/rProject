﻿using System;
using MainLogic.Entities;

namespace UnitTestProject.FactoryEntities.Factories {
    class UtmSubdomainRuleCreator : ICreator {
        public void Bind() {
            Factory.AddCreatorDao(() => {
                return new UtmSubdomainRule {
                    Datecreated = DateTime.UtcNow,
                    Subdomainname = "subdomain",
                    Targetdomain = "target"
                };
            });
        }
    }
}
