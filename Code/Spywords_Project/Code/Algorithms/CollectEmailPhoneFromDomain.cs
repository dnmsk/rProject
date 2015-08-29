using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonUtils.Code;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using IDEV.Hydra.DAO.MassTools;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Algorithms {
    public class CollectEmailPhoneFromDomain : AlgoBase {
        public CollectEmailPhoneFromDomain() : base(new TimeSpan(0, 0, 30)) { }

        protected override void DoAction() {
            var entityToProcess = GetEntitiesToProcess();
            foreach (var domainEntity in entityToProcess) {
                domainEntity.Status |= DomainStatus.Loaded;
                domainEntity.Datecollected = DateTime.Now;
                try {
                    var siteContent = WebRequestHelper.GetContentWithStatus("http://" + DomainExtension.PunycodeDomain(domainEntity.Domain));
                    domainEntity.Content = siteContent.Item2;

                    var emails = GetEmailFromContent(siteContent.Item2);
                    if (emails != null) {
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
                    if (phones != null) {
                        phones.Select(ph => new Domainphone {
                                                DomainID = domainEntity.ID,
                                                Datecreated = DateTime.Now,
                                                Phone = ph
                                            })
                              .ToList()
                              .Save<Domainphone, int>();
                        domainEntity.Status |= DomainStatus.EmailPhoneCollected;
                    }
                } catch (Exception ex) {
                    Logger.Error(ex);
                    domainEntity.Status |= DomainStatus.LoadedError;
                }
                domainEntity.Save();
            }
        }

        private static List<DomainEntity> GetEntitiesToProcess() {
            return DomainEntity.DataSource
                .Where(new DbFnSimpleOp(DomainEntity.Fields.Status, FnMathOper.BitwiseAnd, (short) DomainStatus.Loaded), Oper.Eq, 0)
                .AsList();
        }
        
        private static readonly Regex _emailRegex = new Regex("[a-z0-9_\\-\\+]+@[a-z0-9\\-]+\\.([a-z]{2,3})(?:\\.[a-z]{2})?", REGEX_OPTIONS);

        private static readonly string[] _badEmails = {
            "calltouch.ru",
            "rating@mail.ru"
        };
        private static string[] GetEmailFromContent(string content) {
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
        private static string[] GetPhoneFromContent(string content) {
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