using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Algorithms {
    public class CollectPhrasesForDomain : AlgoBase {
        private readonly static Regex _siteSpywordsExpractor = new Regex("(?s)sword\\.php\\?word=(?<word>.*?)\"", REGEX_OPTIONS);
        public CollectPhrasesForDomain() : base(new TimeSpan(0, 1, 0)) {
        }

        protected override void DoAction() {
            var entities = GetEntitiesToProcess();
            foreach (var entity in entities) {
                var queriesForDomain = new HashSet<string>();
                entity.Status |= DomainStatus.PhrasesCollected;
                try {
                    for (var page = 1;; page++) {
                        var hasUnique = false;
                        var content = SpywordsQueryWrapper.GetQueriesForDomain(entity.Domain, page);
                        foreach (Match wordMatch in _siteSpywordsExpractor.Matches(content)) {
                            var word = wordMatch.Groups["word"].Value.ToLower();
                            if (queriesForDomain.Contains(word)) {
                                continue;
                            }
                            queriesForDomain.Add(word);
                            hasUnique = true;
                            var phrase = Phrase.DataSource
                                .WhereEquals(Phrase.Fields.Text, word)
                                .First();
                            if (phrase == null) {
                                phrase = new Phrase {
                                    Datecreated = DateTime.Now,
                                    Status = PhraseStatus.NotCollected,
                                    Text = word
                                };
                                phrase.Save();
                            }
                            if (!Domainphrase.DataSource
                                .WhereEquals(Domainphrase.Fields.DomainID, entity.ID)
                                .WhereEquals(Domainphrase.Fields.PhraseID, phrase.ID)
                                .IsExists()) {
                                var domainphrase = new Domainphrase {
                                    DomainID = entity.ID,
                                    PhraseID = phrase.ID
                                };
                                domainphrase[Domainphrase.Fields.SE] = 0;
                                domainphrase.Insert();
                            }
                        }
                        if (!hasUnique) {
                            break;
                        }
                    }
                }
                catch (Exception ex) {
                    Logger.Error(ex);
                    entity.Status |= DomainStatus.PhrasesCollectedError;
                }

                entity.Save();
            }
        }

        private static List<DomainEntity> GetEntitiesToProcess() {
            return DomainEntity.DataSource
                .Where(new DbFnSimpleOp(DomainEntity.Fields.Status, FnMathOper.BitwiseAnd, (short)DomainStatus.PhrasesCollected), Oper.Eq, 0)
                .AsList(0, 15);
        }
    }
}