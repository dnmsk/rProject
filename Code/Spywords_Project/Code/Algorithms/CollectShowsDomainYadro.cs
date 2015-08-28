using System;
using System.Collections.Generic;
using CommonUtils.Code;
using IDEV.Hydra.DAO.MassTools;
using Spywords_Project.Code.Entities;

namespace Spywords_Project.Code.Algorithms {
    public class CollectShowsDomainYadro : AlgoBase {
        public CollectShowsDomainYadro() : base(new TimeSpan(0, 0, 30)) {
        }

        protected override void DoAction() {
            var entitiesToCollect = GetDomainEntitiesToProcess();
            if (entitiesToCollect.Count == 0) {
                return;
            }
            foreach (var domain in entitiesToCollect) {
                domain.Visitsmonth = GetVisitsFromYadro(domain.Domain);
            }
            entitiesToCollect.Save<DomainEntity, int>();
        }

        private static List<DomainEntity> GetDomainEntitiesToProcess() {
            return DomainEntity.DataSource
                .WhereNull(DomainEntity.Fields.Visitsmonth)
                .AsList(
                    DomainEntity.Fields.ID,
                    DomainEntity.Fields.Domain
                );
        }

        /*  http://counter.yadro.ru/values?site=oknastroypro.ru  */
        public static int GetVisitsFromYadro(string domain) {
            var yadroContent = WebRequestHelper.GetContentWithStatus("http://counter.yadro.ru/values?site=" + domain);
            var trimmedContent = yadroContent.Item2.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var resultMap = new Dictionary<string, string>();
            foreach (var str in trimmedContent) {
                var kv = str.Trim(';').Split('=');
                if (kv.Length == 2) {
                    resultMap[kv[0].Trim()] = kv[1].Trim();
                }
            }

            string monthVisit;
            int parsed;
            return resultMap.TryGetValue("LI_month_vis", out monthVisit) && int.TryParse(monthVisit, out parsed) ? parsed : default(short);
        }
    }
}