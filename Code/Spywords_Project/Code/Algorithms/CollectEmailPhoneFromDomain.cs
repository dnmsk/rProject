using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonUtils.Code;
using CommonUtils.WatchfulSloths.WatchfulThreads;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using IDEV.Hydra.DAO.MassTools;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;
using Spywords_Project.Code.TransportData;

namespace Spywords_Project.Code.Algorithms {
    public class CollectEmailPhoneFromDomain : AlgoBase {
        public CollectEmailPhoneFromDomain() : base(new TimeSpan(0, 0, 15)) { }

        protected override void DoAction() {
            var entityToProcess = GetEntitiesToProcess();
            foreach (var domainEntity in entityToProcess) {
                TaskRunner.Instance.AddAction(() => {
                    GeDomainData(domainEntity);
                });
            }
        }

        private static void GeDomainData(DomainEntity domainEntity) {
            domainEntity.Status |= DomainStatus.Loaded;
            domainEntity.Datecollected = DateTime.UtcNow;

            var domainInfo = GetDomainInfo(domainEntity.Domain);
            if (domainInfo != null) {
                domainEntity.Content = domainInfo.Content;

                if (domainInfo.Emails != null) {
                    domainInfo.Emails.Select(e => new Domainemail {
                        DomainID = domainEntity.ID,
                        Datecreated = DateTime.UtcNow,
                        Email = e,
                        CollectionIdentity = CollectionIdentity
                    })
                                .ToList()
                                .Save<Domainemail, int>();
                    domainEntity.Status |= DomainStatus.EmailPhoneCollected;
                }
                if (domainInfo.Phones != null) {
                    domainInfo.Phones.Select(ph => new Domainphone {
                        DomainID = domainEntity.ID,
                        Datecreated = DateTime.UtcNow,
                        Phone = ph,
                        CollectionIdentity = CollectionIdentity
                    })
                                .ToList()
                                .Save<Domainphone, int>();
                    domainEntity.Status |= DomainStatus.EmailPhoneCollected;
                }
            } else {
                domainEntity.Status |= DomainStatus.LoadedError;
            }

            domainEntity.Save();
        }

        public static DomainTransport GetDomainInfo(string domain) {
            try {
                var siteContent =
                    WebRequestHelper.GetContentWithStatus("http://" + DomainExtension.PunycodeDomain(domain));
                return new DomainTransport {
                    Domain = domain,
                    Content = siteContent.Item2,
                    Emails = GetEmailFromContent(siteContent.Item2),
                    Phones = GetPhoneFromContent(siteContent.Item2)
                };
            } catch (Exception ex) {
                Logger.Error(ex);
                return null;
            }
        }

        private static List<DomainEntity> GetEntitiesToProcess() {
            return DomainEntity.DataSource
                .Where(new DbFnSimpleOp(DomainEntity.Fields.Status, FnMathOper.BitwiseAnd, (short) DomainStatus.Loaded), Oper.Eq, 0)
                .WhereEquals(DomainEntity.Fields.CollectionIdentity, (short) CollectionIdentity)
                .AsList();
        }
        
        private static readonly Regex _emailRegex = new Regex("[a-z0-9_\\-\\+]+@[a-z0-9\\-]+\\.([a-z]{2,3})(?:\\.[a-z]{2})?", REGEX_OPTIONS);

        private static readonly string[] _badEmails = {
            "calltouch.ru",
            "rating@mail.ru"
        };

        public static string[] GetEmailFromContent(string content) {
            var emails = _emailRegex.Matches(content);
            var result = new List<string>();
            foreach (Match emailMatch in emails) {
                var lowerEmail = emailMatch.Value.ToLower();
                var emailIsGood = true;
                foreach (var badEmail in _badEmails) {
                    if (lowerEmail.IndexOf(badEmail, StringComparison.InvariantCultureIgnoreCase) < 0) {
                        continue;
                    }
                    emailIsGood = false;
                    break;
                }
                if (emailIsGood) {
                    result.Add(lowerEmail);
                }
            }
            return result.Distinct().ToArray();
        }

        private static readonly Regex _phoneRegex = new Regex(@"[: \.>](\+7|8)?[ (-]{0,2}\d{3}[ )-]{0,4}((\d{3}[ -]{0,3}\d{2}[ -]{0,3}\d{2})|(\d{3}[ -]{0,3}\d{1}[ -]\d{3})|(\d{3}[ -]{0,3}\d{4})|(\d{2}[ -]{0,3}\d{2}[ -]{0,3}\d{2}[ -]{0,3}\d{1}))[ <\.]", REGEX_OPTIONS);

        public static string[] GetPhoneFromContent(string content) {
            var phones = _phoneRegex.Matches(content);
            var result = new List<string>();
            foreach (Match phone in phones) {
                var phoneText = phone.Value;
                phoneText = phoneText.Substring(1, phoneText.Length - 2);
                phoneText = phoneText
                    .Replace(" ", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace(" ", string.Empty)
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty);
                result.Add(phoneText);
            }
            return result.Distinct().ToArray();
        }
    }
}