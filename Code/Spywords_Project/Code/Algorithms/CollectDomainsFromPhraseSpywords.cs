using System;
using System.Collections.Generic;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using IDEV.Hydra.DAO.MassTools;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Algorithms {
    public class CollectDomainsFromPhraseSpywords : AlgoBase {
        public CollectDomainsFromPhraseSpywords() : base(new TimeSpan(0, 0, 30)) {
        }

        protected override void DoAction() {
            var domainEntities = GetEntitiesToProcess();
            foreach (var domainEntity in domainEntities) {
                var spywordInfo = _spywordsQueryWrapper.GetDomainInfo(domainEntity.Domain);
                domainEntity.Datecollected = DateTime.Now;
                domainEntity.Status |= DomainStatus.SpywordsCollected;
                domainEntity.Advertsgoogle = 0;
                domainEntity.Advertsyandex = 0;
                domainEntity.Budgetgoogle = 0;
                domainEntity.Budgetyandex = 0;
                domainEntity.Phrasesgoogle = 0;
                domainEntity.Phrasesyandex = 0;
            }
            domainEntities.Save<DomainEntity, int>();
        }

        private static List<DomainEntity> GetEntitiesToProcess() {
            return DomainEntity.DataSource
                .Where(new DbFnSimpleOp(DomainEntity.Fields.Status, FnMathOper.BitwiseAnd, (short)DomainStatus.SpywordsCollected), Oper.Eq, 0)
                .AsList();
        }
    }
}