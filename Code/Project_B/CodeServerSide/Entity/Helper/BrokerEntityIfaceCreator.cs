using System;
using System.Collections.Generic;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.Helper {
    public static class BrokerEntityIfaceCreator {
        public static T CreateEntity<T>(BrokerType brokerType, LanguageType languageType, SportType sportType, GenderType genderType, LinkEntityStatus linkEntityStatus, IEnumerable<string> name, Action<T> postAct = null) 
            where T : 
            IBrokerTyped,
            IGenderTyped,
            ILanguageTyped,
            ILinkStatusTyped, 
            IDateCreatedTyped,
            ISportTyped,
            INamedEntity,
            new() {
            var result = CreateEntity<T>(brokerType, languageType, sportType, linkEntityStatus, entity => {
                entity.Gendertype = genderType;
                entity.Name = CompetitionHelper.ListStringToName(name);
                if (postAct != null) {
                    postAct(entity);
                }
            });
            
            return result;
        }
        public static T CreateEntity<T>(BrokerType brokerType, LanguageType languageType, SportType sportType, LinkEntityStatus linkEntityStatus, Action<T> postAct = null) 
            where T : 
            IBrokerTyped,
            ILanguageTyped,
            ILinkStatusTyped, 
            IDateCreatedTyped,
            ISportTyped,
            new() {
            var entity = new T {
                Languagetype = languageType,
                BrokerID = brokerType,
                Linkstatus = linkEntityStatus,
                SportType = sportType,
                Datecreatedutc = DateTime.UtcNow,
            };
            if (postAct != null) {
                postAct(entity);
            }
            return entity;
        }
    }
}