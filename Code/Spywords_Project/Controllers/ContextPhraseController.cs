﻿using System.Web.Mvc;
using MainLogic.WebFiles;
using Spywords_Project.Code.Providers;
using Spywords_Project.Code.Statuses;
using Spywords_Project.Models;

namespace Spywords_Project.Controllers {
    [Authorize]
    public class ContextPhraseController : ApplicationControllerBase {
        // GET: Phrase
        public ActionResult Index() {
            var phrases = new PhraseProvider().GetPhrasesForAccount(CurrentUser.AccountID, SourceType.Context);
            return View(new PhraseModel(GetBaseModel(), phrases));
        }

        [HttpPost]
        public ActionResult AddPhrase(string phrase) {
            new PhraseProvider().AddPhrase(CurrentUser.AccountID, phrase, SourceType.Context);
            return RedirectToAction("Index");
        }

        public ActionResult PhraseDomains(int id) {
            var phraseProvider = new PhraseProvider();
            var phraseDomains = phraseProvider.GetDomainsStatsForAccountPhrase(CurrentUser.AccountID, id, SourceType.Context);
            var phrase = phraseProvider.GetPhraseEntityModel(CurrentUser.AccountID, id, SourceType.Context);
            return View(new DomainStatsModel(GetBaseModel(), phraseDomains, phrase));
        }
    }
}