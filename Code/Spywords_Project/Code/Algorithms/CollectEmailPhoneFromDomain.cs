using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Code;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using IDEV.Hydra.DAO.MassTools;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Algorithms {
    public class CollectEmailPhoneFromDomain : AlgoBase {
        public CollectEmailPhoneFromDomain() : base(new TimeSpan(0, 0, 20)) { }

        protected override void DoAction() {
            var entityToProcess = GetEntitiesToProcess();
            foreach (var domainEntity in entityToProcess) {
                var siteContent = WebRequestHelper.GetContentWithStatus(domainEntity.Domain);
                domainEntity.Content = siteContent.Item2;
                domainEntity.Datecollected = DateTime.Now;
                domainEntity.Status |= DomainStatus.Loaded;

                var emails = GetEmailFromContent(siteContent.Item2);
                if (emails != null && emails.Length > 0) {
                    emails.Select(e => new Domainemail {
                                            DomainID = domainEntity.ID,
                                            Datecreated = DateTime.Now,
                                            Email = e
                                        })
                          .ToList()
                          .Save<Domainemail, int>();
                    domainEntity.Status |= DomainStatus.EmailPhoneCollected;
                }
                var phones = GetPhoneFromContent(siteContent.Item2);
                if (phones != null && phones.Length > 0) {
                    phones.Select(ph => new Domainphone {
                                            DomainID = domainEntity.ID,
                                            Datecreated = DateTime.Now,
                                            Phone = ph
                                        })
                          .ToList()
                          .Save<Domainphone, int>();
                    domainEntity.Status |= DomainStatus.EmailPhoneCollected;
                }
            }
            entityToProcess.Save<DomainEntity, int>();
        }

        private static List<DomainEntity> GetEntitiesToProcess() {
            return DomainEntity.DataSource
                .Where(new DbFnSimpleOp(DomainEntity.Fields.Status, FnMathOper.BitwiseAnd, (short) DomainStatus.Loaded), Oper.Eq, 0)
                .AsList();
        }

        //TODO

        private static string[] GetEmailFromContent(string content) {
            return null;
        }

        private static string[] GetPhoneFromContent(string content) {
            return null;
        }
    }
}