using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommonUtils.Code;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Algorithms {
    public class CollectDomainInfoSpywords : AlgoBase {
        private readonly static Regex _yandexDomainBlockRegex = new Regex("(?s)\"for_yandex.*?</table>", REGEX_OPTIONS);
        private readonly static Regex _googleDomainBlockRegex = new Regex("(?s)\"for_google.*?</table>", REGEX_OPTIONS);
        private readonly static Regex _siteSpywordsExpractor =new Regex("(?s)sword\\.php\\?site=(?<site>.*?)\"", REGEX_OPTIONS);

        private readonly static Regex _phraseStatsContainer =new Regex("(?s)data_table pre stat.*?</table", REGEX_OPTIONS);
        private readonly static Regex _containerSplitToTr =new Regex("(?s)<tr.*?</tr", REGEX_OPTIONS);
        private readonly static Regex _extractTdResult =new Regex("(?s)<td.*?>(?<td>.*?)</td", REGEX_OPTIONS);

        public CollectDomainInfoSpywords() : base(new TimeSpan(0, 0, 5)) {
        }

        protected override void DoAction() {
            var phrases = GetEntitiesToProcess();
            foreach (var phrase in phrases) {
                var domains = SpywordsQueryWrapper.GetDomainsForPhrase(phrase.Text);
                phrase.Datecollected = DateTime.Now;
                phrase.Status = PhraseStatus.Collected;

                var statsContainer = _phraseStatsContainer.Match(domains).Value;
                var resultRows = _containerSplitToTr.Matches(statsContainer);
                if (resultRows.Count == 3) {
                    var yandexRowResult = _extractTdResult.Matches(resultRows[1].Value);
                    var googleRowResult = _extractTdResult.Matches(resultRows[2].Value);
                    phrase.Showsgoogle = StringParser.ToInt(googleRowResult[1].Groups["td"].Value.Trim().Replace(" ", string.Empty), default(int));
                    phrase.Showsyandex = StringParser.ToInt(yandexRowResult[1].Groups["td"].Value.Trim().Replace(" ", string.Empty), default(int));
                    phrase.Advertisersgoogle = StringParser.ToShort(googleRowResult[2].Groups["td"].Value.Trim().Replace(" ", string.Empty), default(short));
                    phrase.Advertisersyandex = StringParser.ToShort(yandexRowResult[2].Groups["td"].Value.Trim().Replace(" ", string.Empty), default(short));
                } else {
                    Logger.Error("Нет трёх строк в таблице статистики запроса для {0} ID={1}", phrase.Text, phrase.ID);
                }

                var yandexDomains = GetDomains(_yandexDomainBlockRegex.Match(domains).Value);
                var googleDomains = GetDomains(_googleDomainBlockRegex.Match(domains).Value);
                var domainEntities = new List<DomainEntity>();
                try {
                    foreach (var domain in yandexDomains.Union(googleDomains).Distinct()) {
                        var d = DomainExtension.DePunycodeDomain(domain);
                        var domainEntity = DomainEntity.DataSource
                            .WhereEquals(DomainEntity.Fields.Domain, d)
                            .First();
                        if (domainEntity == null) {
                            domainEntity = new DomainEntity {
                                Datecreated = DateTime.Now,
                                Domain = d,
                                Status = DomainStatus.Default
                            };
                            domainEntity.Save();
                        }
                        domainEntities.Add(domainEntity);
                    }
                    foreach (var domainEntity in domainEntities) {
                        var domainphrase = new Domainphrase {
                            DomainID = domainEntity.ID,
                            PhraseID = phrase.ID,
                        };
                        domainphrase[Domainphrase.Fields.SE] = (short)(
                            (yandexDomains.Contains(domainEntity.Domain) ? SearchEngine.Yandex : SearchEngine.Default) |
                            (googleDomains.Contains(domainEntity.Domain) ? SearchEngine.Google : SearchEngine.Default));
                        domainphrase.Insert();
                    }
                }
                catch (Exception ex) {
                    Logger.Error(string.Format("PhraseID={0}\r\n{1}", phrase.ID, ex));
                }
                phrase.Save();
            }
        }

        private static HashSet<string> GetDomains(string content) {
            var result = new HashSet<string>();
            var domains = _siteSpywordsExpractor.Matches(content);
            foreach (Match matchDomain in domains) {
                try {
                    var domain = matchDomain.Groups["site"].Value;
                    if (!result.Contains(domain)) {
                        result.Add(domain);
                    }
                }
                catch (Exception ex) {
                    Logger.Error(ex);
                }
            }
            return result;
        }

        private static List<Phrase> GetEntitiesToProcess() {
            return Phrase.DataSource
                         .WhereEquals(Phrase.Fields.Status, (short) PhraseStatus.NotCollected)
                         .AsList();
        }
    }
}