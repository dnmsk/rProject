﻿using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using Spywords_Project.Code.Algorithms;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;
using Spywords_Project.Models.EntityModel;

namespace Spywords_Project.Code.Providers {
    public class PhraseProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(PhraseProvider).FullName);
        public PhraseProvider() : base(_logger) {}

        public bool AddPhrase(int accountID, string text, SourceType sourceType) {
            return InvokeSafeSingleCall(() => {
                text = text.ToLower();
                var phrase = Phrase.DataSource
                    .WhereEquals(Phrase.Fields.Text, text)
                    .WhereEquals(Phrase.Fields.CollectionIdentity, (short)AlgoBase.CollectionIdentity)
                    .First();
                if (phrase == null) {
                    phrase = new Phrase {
                        Datecreated = DateTime.UtcNow,
                        Text = text,
                        Status = PhraseStatus.NotCollected,
                        CollectionIdentity = AlgoBase.CollectionIdentity,
                    };
                    phrase.Save();
                }
                var phraseAccount = Phraseaccount.DataSource
                    .WhereEquals(Phraseaccount.Fields.AccountidentityID, accountID)
                    .WhereEquals(Phraseaccount.Fields.PhraseID, phrase.ID)
                    .WhereEquals(Phraseaccount.Fields.CollectionIdentity, (short)AlgoBase.CollectionIdentity)
                    .First();
                if (phraseAccount == null) {
                    phraseAccount = new Phraseaccount {
                        Datecreated = DateTime.UtcNow,
                        AccountidentityID = accountID,
                        PhraseID = phrase.ID,
                        SourceType = sourceType,
                        CollectionIdentity = AlgoBase.CollectionIdentity
                    };
                } else {
                    phraseAccount.SourceType |= sourceType;
                }
                return phraseAccount.Save();
            }, false);
        }

        public List<PhraseEntityModel> GetPhrasesForAccount(int accountID, SourceType sourceType) {
            return InvokeSafe(() => {
                var phraseModels = Phrase.DataSource
                    .Join(JoinType.Inner, Phraseaccount.Fields.PhraseID, Phrase.Fields.ID, RetrieveMode.Retrieve)
                    .WhereEquals(Phraseaccount.Fields.AccountidentityID, accountID)
                    .Where(new DbFnSimpleOp(Phraseaccount.Fields.SourceType, FnMathOper.BitwiseAnd, (short) sourceType), Oper.NotEq, default(short))
                    .Sort(Phraseaccount.Fields.ID, SortDirection.Desc)
                    .AsList()
                    .Select(ph => new PhraseEntityModel {
                        PhraseID = ph.ID,
                        Text = ph.Text,
                        AdvertsGoogle = ph.Advertisersgoogle.Value,
                        AdvertsYandex = ph.Advertisersyandex.Value,
                        PhraseStatus = ph.Status,
                        PhraseAccountID = ph.GetJoinedEntity<Phraseaccount>().ID,
                        CollectedDomainsCount = 0,
                        DomainsCount = 0
                    })
                    .ToList();
                var totalDomains = DomainEntity.DataSource.AggrCount(DomainEntity.Fields.ID, false);
                var collectedDomains = DomainEntity.DataSource.AggrString(
                    string.Format("count(distinct case when {0}.{1} & {2} = {2} then {0}.{3} else null end)",
                        DomainEntity.Descriptor.TableName, 
                        DomainEntity.Fields.Status,
                        (short) (DomainStatus.Loaded | DomainStatus.EmailPhoneCollected), 
                        DomainEntity.Fields.ID), "collectedDomains");

                var phraseStats = DomainEntity.DataSource
                    .Join(JoinType.Inner, Domainphrase.Fields.DomainID, DomainEntity.Fields.ID, RetrieveMode.NotRetrieve)
                    .Join(JoinType.Inner, Phrase.Fields.ID, Domainphrase.Fields.PhraseID, RetrieveMode.Retrieve)
                    .WhereIn(Phrase.Fields.ID, phraseModels.Select(pm => pm.PhraseID))
                    .Where(new DbFnSimpleOp(Domainphrase.Fields.SourceType, FnMathOper.BitwiseAnd, (short)sourceType), Oper.NotEq, default(short))
                    .GroupBy(Phrase.Fields.ID)
                    .AsGroups(
                        totalDomains,
                        collectedDomains
                    ).ToDictionary(ge => (int) ge[Phrase.Fields.ID], ge => ge);
                foreach (var phraseModel in phraseModels) {
                    GroupedEntity ge;
                    if (phraseStats.TryGetValue(phraseModel.PhraseID, out ge)) {
                        phraseModel.DomainsCount = (int) (long) ge[totalDomains];
                        phraseModel.CollectedDomainsCount = (int)(long)ge[collectedDomains];
                    }
                }
                return phraseModels;
            }, new List<PhraseEntityModel>());
        }

        public PhraseEntityModel GetPhraseEntityModel(int accountID, int accountPhraseID, SourceType sourceType) {
            return InvokeSafe(() => {
                var phrase = Phrase.DataSource
                    .Join(JoinType.Inner, Phraseaccount.Fields.PhraseID, Phrase.Fields.ID, RetrieveMode.Retrieve)
                    .WhereEquals(Phraseaccount.Fields.AccountidentityID, accountID)
                    .WhereEquals(Phraseaccount.Fields.ID, accountPhraseID)
                    .Where(new DbFnSimpleOp(Phraseaccount.Fields.SourceType, FnMathOper.BitwiseAnd, (short)sourceType), Oper.NotEq, default(short))
                    .First();
                if (phrase == null) {
                    return null;
                }
                return new PhraseEntityModel {
                    PhraseID = phrase.ID,
                    Text = phrase.Text,
                    AdvertsGoogle = phrase.Advertisersgoogle.Value,
                    AdvertsYandex = phrase.Advertisersyandex.Value,
                    PhraseStatus = phrase.Status,
                    PhraseAccountID = phrase.GetJoinedEntity<Phraseaccount>().ID
                };
            }, null);
        }

        public List<PhraseEntityModel> GetNearPhrasesEntityModel(int accountID, int accountPhraseID, SourceType sourceType) {
            return InvokeSafe(() => {
                var phrase = Phrase.DataSource
                    .Join(JoinType.Inner, Phraseaccount.Fields.PhraseID, Phrase.Fields.ID, RetrieveMode.Retrieve)
                    .WhereEquals(Phraseaccount.Fields.AccountidentityID, accountID)
                    .WhereEquals(Phraseaccount.Fields.ID, accountPhraseID)
                    .Where(new DbFnSimpleOp(Phraseaccount.Fields.SourceType, FnMathOper.BitwiseAnd, (short)sourceType), Oper.NotEq, default(short))
                    .First();
                if (phrase == null) {
                    return null;
                }
                var baseForms = phrase.Textbaseform.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 2)
                    .ToArray();
                return Phrase.DataSource
                      .WhereEquals(Phrase.Fields.CollectionIdentity, (short) phrase.CollectionIdentity)
                      .Where(GetFilterByWordIgnoreCaseAnd(baseForms, Phrase.Fields.Textbaseform, false))
                      .AsList()
                      .Select(ph => new PhraseEntityModel {
                              PhraseID = ph.ID,
                              Text = ph.Text,
                              AdvertsGoogle = ph.Advertisersgoogle.Value,
                              AdvertsYandex = ph.Advertisersyandex.Value,
                              PhraseStatus = ph.Status,
                              PhraseAccountID = default(int)
                      })
                      .ToList();
            }, null);
        }

        private static DaoFilterBase GetFilterByWordIgnoreCaseAnd(string[] words, Enum field, bool fullyEq) {
            var filters = new List<DaoFilterBase>();
            words.Each(word => filters.Add(GetIndexedFilterByWordIgnoreCase(word, field, fullyEq)));

            var allFilters = filters.Count > 1
                ? new DaoFilterAnd(filters)
                : filters[0];
            return allFilters;
        }
        private static DaoFilterBase GetIndexedFilterByWordIgnoreCase(string word, Enum field, bool fullyEq = true) {
            return new DaoFilter(new DbFnSimpleFieldOp("lower", field), Oper.Like, string.Format(fullyEq ? "{0}" : "%{0}%", word.ToLower()));
        }

        public List<DomainStatsEntityModel> GetDomainsStatsForAccountPhrase(int accountID, int accountPhraseID, SourceType sourceType) {
            return InvokeSafe(() => {
                if (GetPhraseEntityModel(accountID, accountPhraseID, sourceType) == null) {
                     return new List<DomainStatsEntityModel>();
                }
                var domainEntityDs = DomainEntity.DataSource
                    .Join(JoinType.Inner, Domainphrase.Fields.DomainID, DomainEntity.Fields.ID, RetrieveMode.Retrieve)
                    .Join(JoinType.Inner, Phraseaccount.Fields.PhraseID, Domainphrase.Fields.PhraseID, RetrieveMode.Retrieve)
                    .WhereEquals(Phraseaccount.Fields.AccountidentityID, accountID)
                    .WhereEquals(Phraseaccount.Fields.ID, accountPhraseID)
                    .Where(new DbFnSimpleOp(Phraseaccount.Fields.SourceType, FnMathOper.BitwiseAnd, (short)sourceType), Oper.NotEq, default(short))
                    .Where(new DbFnSimpleOp(Domainphrase.Fields.SourceType, FnMathOper.BitwiseAnd, (short)sourceType), Oper.NotEq, default(short));
                return GetDomainsStats(domainEntityDs);
            }, new List<DomainStatsEntityModel>());
        }

        public List<DomainStatsEntityModel> GetDomainsStatsForPhraseIds(int[] phraseIDs) {
            return InvokeSafe(() => {
                var domainEntityDs = DomainEntity.DataSource
                    .Join(JoinType.Inner, Domainphrase.Fields.DomainID, DomainEntity.Fields.ID, RetrieveMode.Retrieve)
                    .WhereIn(Domainphrase.Fields.PhraseID, phraseIDs.Distinct());
                return GetDomainsStats(domainEntityDs);
            }, new List<DomainStatsEntityModel>());
        }

        private List<DomainStatsEntityModel> GetDomainsStats(DbDataSource<DomainEntity, int> domainEntityDs) {
            return InvokeSafe(() => {
                var domainsStats = domainEntityDs
                    .Sort(DomainEntity.Fields.Domain, SortDirection.Asc)
                    .AsList(
                        DomainEntity.Fields.ID,
                        DomainEntity.Fields.Domain,
                        DomainEntity.Fields.Visitsmonth,
                        DomainEntity.Fields.Advertsgoogle,
                        DomainEntity.Fields.Advertsyandex,
                        DomainEntity.Fields.Budgetgoogle,
                        DomainEntity.Fields.Budgetyandex,
                        DomainEntity.Fields.Datecollected,
                        DomainEntity.Fields.Phrasesgoogle,
                        DomainEntity.Fields.Phrasesyandex,
                        DomainEntity.Fields.Status,
                        Domainphrase.Fields.SE
                    )
                    .GroupBy(de => de.ID)
                    .Select(gde => gde.First())
                    .Select(d => new DomainStatsEntityModel {
                        DomainID = d.ID,
                        Domain = d.Domain,
                        VisitsMonth = d.Visitsmonth.Value,
                        Advertsgoogle = d.Advertsgoogle.Value,
                        Advertsyandex = d.Advertsyandex.Value,
                        Budgetgoogle = d.Budgetgoogle.Value,
                        Budgetyandex = d.Budgetyandex.Value,
                        Datecollected = d.Datecollected,
                        Phrasesgoogle = d.Phrasesgoogle.Value,
                        Phrasesyandex = d.Phrasesyandex.Value,
                        Status = d.Status,
                        SearchEngine = d.GetJoinedEntity<Domainphrase>().SE
                    })
                    .ToList();
                var domainIDs = domainsStats.Select(ds => ds.DomainID).ToArray();
                var emailsMap = Domainemail.DataSource
                    .WhereIn(Domainemail.Fields.DomainID, domainIDs)
                    .AsMapByField<int>(Domainemail.Fields.DomainID, Domainemail.Fields.Email);
                var phonesMap = Domainphone.DataSource
                    .WhereIn(Domainphone.Fields.DomainID, domainIDs)
                    .AsMapByField<int>(Domainphone.Fields.DomainID, Domainphone.Fields.Phone);
                foreach (var domainStat in domainsStats) {
                    List<Domainphone> phonesForDomain;
                    if (phonesMap.TryGetValue(domainStat.DomainID, out phonesForDomain)) {
                        domainStat.Phones = phonesForDomain.Select(phD => phD.Phone).ToArray();
                    }
                    List<Domainemail> emailsForDomain;
                    if (emailsMap.TryGetValue(domainStat.DomainID, out emailsForDomain)) {
                        domainStat.Emails = emailsForDomain.Select(phD => phD.Email).ToArray();
                    }
                }
                return domainsStats;
            }, new List<DomainStatsEntityModel>());
        }

        public ProgressStatusSummary GetProgress() {
            return InvokeSafe(() => {
                var last30Min = DateTime.UtcNow.AddMinutes(-30);
                var result = new ProgressStatusSummary();
                result.DomainsCount = (int) (DomainEntity.DataSource.Max(DomainEntity.Fields.ID) ?? default(decimal));
                result.PhrasesCount = (int) (Phrase.DataSource.Max(Phrase.Fields.ID) ?? default(decimal));
                result.EmailCount = (int) (Domainemail.DataSource.Max(Domainemail.Fields.ID) ?? default(decimal));
                result.PhoneCount = (int) (Domainphone.DataSource.Max(Domainphone.Fields.ID) ?? default(decimal));
                result.DomainsLast30MinCount = DomainEntity.DataSource.Where(DomainEntity.Fields.Datecollected, Oper.GreaterOrEq, last30Min).Count();
                result.PhrasesLast30MinCount = Phrase.DataSource.Where(Phrase.Fields.Datecollected, Oper.GreaterOrEq, last30Min).Count();
                result.EmailCountDistinctDomain = Domainemail.DataSource.Count(Domainemail.Fields.DomainID);
                result.PhoneCountDistinct = Domainphone.DataSource.Count(Domainphone.Fields.DomainID);
                result.UserQueriesCount = (int)(Phraseaccount.DataSource.Max(Phraseaccount.Fields.ID) ?? default(decimal));
                return result;
            }, new ProgressStatusSummary());
        }
    }
}