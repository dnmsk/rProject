using System.Web.Mvc;
using MainLogic.WebFiles;
using Spywords_Project.Code.Providers;
using Spywords_Project.Models;

namespace Spywords_Project.Controllers {
    [Authorize]
    public class PhraseController : ApplicationControllerBase {
        static readonly PhraseProvider _phraseProvider = new PhraseProvider();
        // GET: Phrase
        public ActionResult Index() {
            var phrases = _phraseProvider.GetPhrasesForAccount(CurrentUser.AccountID);

            return View(new PhraseModel(GetBaseModel(), phrases));
        }

        [HttpPost]
        public ActionResult AddPhrase(string phrase) {
            _phraseProvider.AddPhrase(CurrentUser.AccountID, phrase);
            return RedirectToAction("Index");
        }

        public ActionResult PhraseDomains(int id) {
            var phraseDomains = _phraseProvider.GetDomainsStatsForAccountPhrase(CurrentUser.AccountID, id);
            var phrase = _phraseProvider.GetPhraseEntityModel(CurrentUser.AccountID, id);
            return View(new DomainStatsModel(GetBaseModel(), phraseDomains, phrase));
        }
    }
}