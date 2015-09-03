using System;
using System.Web.Mvc;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using MainLogic.WebFiles;
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
            return View(new StatusModel(GetBaseModel(), _serverProgress));
        }

        public ActionResult About() {
            return View(GetBaseModel());
        }

        public ActionResult Contact() {
            return View(GetBaseModel());
        }
    }
}