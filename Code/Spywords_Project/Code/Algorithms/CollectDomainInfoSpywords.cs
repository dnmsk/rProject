using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonUtils.Code;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using IDEV.Hydra.DAO;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Algorithms {
    public class CollectDomainInfoSpywords : AlgoBase {
        private static readonly Regex _yandexDomainBlockRegex = new Regex("(?s)\"for_yandex.*?</table>", REGEX_OPTIONS);
        private static readonly Regex _googleDomainBlockRegex = new Regex("(?s)\"for_google.*?</table>", REGEX_OPTIONS);

        private static readonly Regex _siteSpywordsExpractor = new Regex("(?s)sword\\.php\\?site=(?<site>.*?)\"",
            REGEX_OPTIONS);

        private static readonly Regex _phraseStatsContainer = new Regex("(?s)data_table pre stat.*?</table",
            REGEX_OPTIONS);

        private static readonly Regex _containerSplitToTr = new Regex("(?s)<tr.*?</tr", REGEX_OPTIONS);
        private static readonly Regex _extractTdResult = new Regex("(?s)<td.*?>(?<td>.*?)</td", REGEX_OPTIONS);

        private static readonly Regex _allDomainsListYandexContainer = new Regex("(?s)data_table advSite.*?</table",
            REGEX_OPTIONS);

        public CollectDomainInfoSpywords() : base(new TimeSpan(0, 0, 20)) {
        }

        protected override void DoAction() {
            var phrases = GetEntitiesToProcess();
            foreach (var phrase in phrases) {
                var domains = SpywordsQueryWrapper.GetDomainsForPhrase(phrase.Text);
                phrase.Datecollected = DateTime.UtcNow;

                var statsContainer = _phraseStatsContainer.Match(domains).Value;
                var resultRows = _containerSplitToTr.Matches(statsContainer);
                if (resultRows.Count == 3) {
                    phrase.Status = PhraseStatus.Collected;
                    var yandexRowResult = _extractTdResult.Matches(resultRows[1].Value);
                    var googleRowResult = _extractTdResult.Matches(resultRows[2].Value);
                    phrase.Showsgoogle =
                        StringParser.ToInt(googleRowResult[1].Groups["td"].Value.Trim().Replace(" ", string.Empty),
                            default(int));
                    phrase.Showsyandex =
                        StringParser.ToInt(yandexRowResult[1].Groups["td"].Value.Trim().Replace(" ", string.Empty),
                            default(int));
                    phrase.Advertisersgoogle =
                        StringParser.ToShort(googleRowResult[2].Groups["td"].Value.Trim().Replace(" ", string.Empty),
                            default(short));
                    phrase.Advertisersyandex =
                        StringParser.ToShort(yandexRowResult[2].Groups["td"].Value.Trim().Replace(" ", string.Empty),
                            default(short));

                    var domainsForPhraseYandex = SpywordsQueryWrapper.GetDomainsForPhraseYandex(phrase.Text);

                    var yandexDomains = GetDomains(_yandexDomainBlockRegex.Match(domains).Value);
                    var advancedYandexDomains =
                        GetDomains(_allDomainsListYandexContainer.Match(domainsForPhraseYandex).Value);
                    yandexDomains = new HashSet<string>(yandexDomains.Union(advancedYandexDomains).Distinct());

                    var googleDomains = GetDomains(_googleDomainBlockRegex.Match(domains).Value);
                    try {
                        SlothMovePlodding.AddAction(() => {
                            var domainEntities = new List<DomainEntity>(yandexDomains.Union(googleDomains).Distinct().Select(GetDomainEntity));
                            foreach(var domainEntity in domainEntities) {
                                var seType = (
                                    (yandexDomains.Contains(domainEntity.Domain)
                                        ? SearchEngine.Yandex
                                        : SearchEngine.Default) |
                                    (googleDomains.Contains(domainEntity.Domain)
                                        ? SearchEngine.Google
                                        : SearchEngine.Default));

                                CreateOrUpdateDomainPhrase(domainEntity, phrase, seType, SourceType.Context);
                            }
                        });
                    }
                    catch (Exception ex) {
                        phrase.Status = PhraseStatus.Error;
                        Logger.Error(string.Format("PhraseID={0}\r\n{1}", phrase.ID, ex));
                    }
                } else {
                    phrase.Status = PhraseStatus.Error;
                    Logger.Error("Нет трёх строк в таблице статистики запроса для {0} ID={1}", phrase.Text, phrase.ID);
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
            var fieldsToRetrive = new Enum[] {
                Phrase.Fields.ID,
                Phrase.Fields.Advertisersgoogle,
                Phrase.Fields.Advertisersyandex,
                Phrase.Fields.Showsgoogle,
                Phrase.Fields.Showsgoogle,
                Phrase.Fields.Status,
                Phrase.Fields.Text
            };
            var phrases = Phrase.DataSource
                .Join(JoinType.Inner, Phraseaccount.Fields.PhraseID, Phrase.Fields.ID, RetrieveMode.NotRetrieve)
                .WhereEquals(Phrase.Fields.Status, (short)PhraseStatus.NotCollected)
                .AsList(
                    fieldsToRetrive
                );
            return phrases.Any() 
                ? phrases 
                : Phrase.DataSource
                         .WhereEquals(Phrase.Fields.Status, (short) PhraseStatus.NotCollected)
                         .AsList(0, 20, fieldsToRetrive);
        }
    }
}