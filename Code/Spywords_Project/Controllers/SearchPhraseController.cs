using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using Spywords_Project.Code.Providers;
using Spywords_Project.Code.Statuses;
using Spywords_Project.Models;
using Spywords_Project.Models.EntityModel;

namespace Spywords_Project.Controllers {
    [Authorize]
    public class SearchPhraseController : ApplicationControllerBase {
        // GET: Phrase
        public ActionResult Index() {
            var phrases = new PhraseProvider().GetPhrasesForAccount(CurrentUser.AccountID, SourceType.Search);
            return View(new PhraseModel(GetBaseModel(), phrases));
        }

        [HttpPost]
        public ActionResult AddPhrase(string phrase) {
            new PhraseProvider().AddPhrase(CurrentUser.AccountID, phrase, SourceType.Search);
            return RedirectToAction("Index");
        }

        public ActionResult PhraseDomains(int id) {
            var phraseProvider = new PhraseProvider();
            var phraseDomains = phraseProvider.GetDomainsStatsForAccountPhrase(CurrentUser.AccountID, id, SourceType.Search);
            var phrase = phraseProvider.GetPhraseEntityModel(CurrentUser.AccountID, id, SourceType.Search);
            return View(new DomainStatsModel(GetBaseModel(), phraseDomains, phrase));
        }

        public ActionResult ExportPrase(int id) {
            var phraseProvider = new PhraseProvider();
            var phraseDomains = phraseProvider.GetDomainsStatsForAccountPhrase(CurrentUser.AccountID, id, SourceType.Search);
            var phrase = phraseProvider.GetPhraseEntityModel(CurrentUser.AccountID, id, SourceType.Search);

            return new FileContentResult(Encoding.UTF8.GetBytes(BuildExportCsv(phraseDomains)), "text/css") {
                FileDownloadName = string.Format("Фраза_{0}.csv", phrase.Text.Replace(" ", "_"))
            };
        }

        private static string BuildExportCsv(List<DomainStatsEntityModel> domainStats) {
            return domainStats
                .Select(ds => new [] {
                    ds.Domain,
                    ds.VisitsMonth.ToString(),
                    (ds.Emails ?? new string[0]).StrJoin(","),
                    (ds.Phones ?? new string[0]).StrJoin(",")
                }.StrJoin(";"))
                .StrJoin(Environment.NewLine);
        }
    }
}