using System;
using System.Collections.Generic;
using IDEV.Hydra.DAO.MassTools;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Algorithms {
    public class CollectDomainInfoSpywords : AlgoBase {
        public CollectDomainInfoSpywords() : base(new TimeSpan(0, 0, 5)) {
        }

        protected override void DoAction() {
            var phrases = GetEntitiesToProcess();
            foreach (var phrase in phrases) {
                var domains = _spywordsQueryWrapper.GetDomainsForPhrase(phrase.Text);
                phrase.Datecollected = DateTime.Now;
                phrase.Status = PhraseStatus.Collected;

                phrase.Showsgoogle = 0;
                phrase.Showsyandex = 0;
                phrase.Advertisersgoogle= 0;
                phrase.Advertisersyandex = 0;

                var domainEntities = new List<DomainEntity>();
                foreach (var domain in new string[0]) {
                    domainEntities.Add(new DomainEntity {
                        Datecreated = DateTime.Now,
                        Domain = domain,
                        Status = DomainStatus.Default
                    });
                }
                domainEntities.Save<DomainEntity, int>();
                foreach (var domainEntity in domainEntities) {
                    new Domainphrase {
                        DomainID = domainEntity.ID,
                        PhraseID = phrase.ID,
                        SE = SearchEngine.Yandex //TODO
                    }.Save();
                }
            }
            phrases.Save<Phrase, int>();
        }

        private static List<Phrase> GetEntitiesToProcess() {
            return Phrase.DataSource
                         .WhereEquals(Phrase.Fields.Status, PhraseStatus.NotCollected)
                         .AsList();
        }
    }
}