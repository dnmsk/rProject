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
                            if (Phrase.DataSource.WhereEquals(Phrase.Fields.Text, word).IsExists()) {
                                continue;
                            }
                            new Phrase {
                                Datecreated = DateTime.Now,
                                Status = PhraseStatus.NotCollected,
                                Text = word
                            }.Save();
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