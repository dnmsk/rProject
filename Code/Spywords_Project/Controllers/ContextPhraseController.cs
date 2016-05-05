using System.Linq;
using System.Web.Mvc;
using MainLogic.WebFiles;
using Spywords_Project.Code;
using Spywords_Project.Code.Providers;
using Spywords_Project.Code.Statuses;
using Spywords_Project.Models;

namespace Spywords_Project.Controllers {
    [Authorize]
    public class ContextPhraseController : ApplicationControllerBase {
        // GET: Phrase
        public ActionResult Index() {
            LogAction(SpywordsActions.ContextPhraseIndex, null);
            var phrases = new PhraseProvider().GetPhrasesForAccount(CurrentUser.AccountID, SourceType.Context);
            return View(new PhraseModel(GetBaseModel(), phrases));
        }

        [HttpPost]
        public ActionResult AddPhrase(string phrase) {
            LogAction(SpywordsActions.ContextPhraseAddPhrase, null);
            new PhraseProvider().AddPhrase(CurrentUser.AccountID, phrase, SourceType.Context);
            return RedirectToAction("Index");
        }

        public ActionResult PhraseDomains(int id) {
            LogAction(SpywordsActions.ContextPhrasePhraseDomains, id);
            var phraseProvider = new PhraseProvider();
            var phraseDomains = phraseProvider.GetDomainsStatsForAccountPhrase(CurrentUser.AccountID, id, SourceType.Context);
            var phrase = phraseProvider.GetPhraseEntityModel(CurrentUser.AccountID, id, SourceType.Context);
            return View(new DomainStatsModel(GetBaseModel(), phraseDomains, phrase));
        }

        public ActionResult ExportPhraseDomains(int id) {
            LogAction(SpywordsActions.ContextPhrasePhraseDomains, id);
            var phraseProvider = new PhraseProvider();
            var phraseDomains = phraseProvider.GetDomainsStatsForAccountPhrase(CurrentUser.AccountID, id, SourceType.Context);
            var phrase = phraseProvider.GetPhraseEntityModel(CurrentUser.AccountID, id, SourceType.Context);
            return SearchPhraseController.BuildExportCsv(phraseDomains, phrase.Text);
        }

        public ActionResult NearPhrases(int id) {
            var phraseProvider = new PhraseProvider();
            var phrases = phraseProvider.GetNearPhrasesEntityModel(CurrentUser.AccountID, id, SourceType.Context);
            var phraseDomains = phraseProvider.GetDomainsStatsForPhraseIds(phrases.Select(ph => ph.PhraseID).ToArray());
            return View(new NearPhraseStatsModel(GetBaseModel(), phraseDomains, phrases));
        }

        public ActionResult ExportNearPhrases(int id) {
            var phraseProvider = new PhraseProvider();
            var phrases = phraseProvider.GetNearPhrasesEntityModel(CurrentUser.AccountID, id, SourceType.Context);
            var phraseDomains = phraseProvider.GetDomainsStatsForPhraseIds(phrases.Select(ph => ph.PhraseID).ToArray());
            return SearchPhraseController.BuildExportCsv(phraseDomains, phrases.FirstOrDefault()?.Text ?? "empty");
        }
    }
}