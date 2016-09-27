using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.WatchfulThreads;
using MainLogic.WebFiles;
using Spywords_Project.Code;
using Spywords_Project.Code.Algorithms;

namespace Spywords_Project.Controllers {
    [Authorize]
    public class DomainController : FileControllerBase {
        // GET: Domain
        public ActionResult Index() {
            LogAction(SpywordsActions.DomainContact, null);
            return View(GetBaseModel());
        }

        [HttpPost]
        public ActionResult FromFile() {
            LogAction(SpywordsActions.DomainContactFromFile, null);
            var files = GetFilesFromRequest(Request);
            if (!files.SafeAny()) {
                return RedirectToAction("Index");
            }
            var file = files.First();
            if (file.Value.Item1 != FileFormat.Text) {
                return RedirectToAction("Index");
            }
            return BuildFileResult(Encoding.UTF8.GetString(file.Value.Item2));
        }

        [HttpPost]
        public ActionResult FromRaw(string domains) {
            LogAction(SpywordsActions.DomainContactFromRaw, null);
            return BuildFileResult(domains);
        }

        private static ActionResult BuildFileResult(string domains) {
            var splitted = domains.Split(new[] {"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var cnt = 0;
            for (var index = 0; index < splitted.Length; index++) {
                var domainRow = splitted[index].Trim();
                splitted[index] = domainRow;
                if (domainRow.IsNullOrWhiteSpace()) {
                    continue;
                }
                domainRow = domainRow.CutHttpHttps().CutWww();

                AlgoBase.GetDomainEntity(domainRow);

                var index1 = index;
                TaskRunner.Instance.AddAction(() => { CollectAndWriteToArray(domainRow, splitted, index1); });
                cnt++;
            }

            string[] subRes;
            while ((subRes = splitted.Where(row => !row.IsNullOrWhiteSpace() && !row.Contains(";")).ToArray()).Any()) {
                Thread.Sleep(500);
            }

            return new FileContentResult(Encoding.GetEncoding(1251).GetBytes(splitted.StrJoin(Environment.NewLine).Replace("+", string.Empty)), "text/css") {
                FileDownloadName = string.Format("Домены_{0}шт_{1}.csv", cnt, DateTime.Now.ToString("dd-MM-yyyy"))
            };
        }

        private static void CollectAndWriteToArray(string domain, string[] fileContent, int index) {
            domain = domain.Trim();
            var domainInfo = CollectEmailPhoneFromDomain.GetDomainInfo(domain);
            if (domainInfo == null) {
                fileContent[index] = new[] { domain, "не ответил" }.StrJoin(";");
                return;
            }
            fileContent[index] = new[] { domain, (domainInfo.Phones ?? new string[0]).StrJoin(","), (domainInfo.Emails ?? new string[0]).StrJoin(",") }.StrJoin(";");
        }
    }
}
