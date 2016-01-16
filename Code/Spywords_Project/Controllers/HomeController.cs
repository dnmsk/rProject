using System;
using System.Web.Mvc;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using MainLogic.WebFiles;
using Spywords_Project.Code;
using Spywords_Project.Code.Algorithms;
using Spywords_Project.Code.Providers;
using Spywords_Project.Models;
using Spywords_Project.Models.EntityModel;

namespace Spywords_Project.Controllers {
    public class HomeController : ApplicationControllerBase {
        private static ProgressStatusSummary _serverProgress;
        static HomeController() {
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(() => {
                _serverProgress = new PhraseProvider().GetProgress();
                return null;
            }, new TimeSpan(0, 15, 0), null));
        }
        public ActionResult Index() {
            LogAction(SpywordsActions.HomeIndex, null);
            return View(new StatusModel(GetBaseModel(), _serverProgress));
        }

        public ActionResult About() {
            LogAction(SpywordsActions.HomeAbout, null);
            return View(GetBaseModel());
        }

        public ActionResult Contact() {
            LogAction(SpywordsActions.HomeContact, null);
            return View(GetBaseModel());
        }

        public ActionResult PushConfig() {
            AlgoBase.PushConfiguration();
            return RedirectToAction("Index");
        }
    }
}