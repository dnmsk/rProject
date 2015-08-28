using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CommonUtils.Code;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using Spywords_Project.Code.Entities;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Algorithms {
    public class CollectDomainsFromPhraseSpywords : AlgoBase {
        private readonly static Regex _domainStatsContainer = new Regex("(?s)data_table stat.*?</table", REGEX_OPTIONS);
        private readonly static Regex _containerSplitToTr = new Regex("(?s)<tr.*?</tr", REGEX_OPTIONS);
        private readonly static Regex _extractTdResult = new Regex("<td.*?>(?<td>.*?)</td", REGEX_OPTIONS);
        public CollectDomainsFromPhraseSpywords() : base(new TimeSpan(0, 0, 30)) {
        }

        protected override void DoAction() {
            var domainEntities = GetEntitiesToProcess();
            foreach (var domainEntity in domainEntities) {
                var spywordInfo = SpywordsQueryWrapper.GetDomainInfo(domainEntity.Domain);
                domainEntity.Datecollected = DateTime.Now;
                domainEntity.Status |= DomainStatus.SpywordsCollected;

                var statsContainer = _domainStatsContainer.Match(spywordInfo).Value;
                var resultRows = _containerSplitToTr.Matches(statsContainer);
                if (resultRows.Count == 3) {
                    var yandexRowResult = _extractTdResult.Matches(resultRows[1].Value);
                    var googleRowResult = _extractTdResult.Matches(resultRows[2].Value);

                    domainEntity.Advertsgoogle = StringParser.ToInt(googleRowResult[2].Groups["td"].Value.Trim().Replace(" ", string.Empty), default(int));
                    domainEntity.Advertsyandex = StringParser.ToInt(yandexRowResult[2].Groups["td"].Value.Trim().Replace(" ", string.Empty), default(int));
                    domainEntity.Budgetgoogle = StringParser.ToInt(googleRowResult[4].Groups["td"].Value.Trim().Replace(" ", string.Empty).Replace("&nbsp;p.", string.Empty), default(int));
                    domainEntity.Budgetyandex = StringParser.ToInt(yandexRowResult[4].Groups["td"].Value.Trim().Replace(" ", string.Empty).Replace("&nbsp;p.", string.Empty), default(int));
                    domainEntity.Phrasesgoogle = StringParser.ToInt(googleRowResult[1].Groups["td"].Value.Trim().Replace(" ", string.Empty), default(int));
                    domainEntity.Phrasesyandex = StringParser.ToInt(yandexRowResult[1].Groups["td"].Value.Trim().Replace(" ", string.Empty), default(int));
                } else {
                    Logger.Error("Нет трёх строк в таблице статистики запроса для {0} ID={1}", domainEntity.Domain, domainEntity.ID);
                }
                domainEntity.Save();
            }
        }

        private static List<DomainEntity> GetEntitiesToProcess() {
            return DomainEntity.DataSource
                .Where(new DbFnSimpleOp(DomainEntity.Fields.Status, FnMathOper.BitwiseAnd, (short)DomainStatus.SpywordsCollected), Oper.Eq, 0)
                .AsList();
        }
    }
}