using System;
using System.Web.Mvc;
using CommonUtils.Code;
using MainLogic.WebFiles;
using Project_B.Code.DataProvider;
using Project_B.Code.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class CompetitionController : ApplicationControllerBase {
        public ActionResult Index() {
            return RedirectToAction("Regular");
        }
        // GET: Competition
        public ActionResult Regular(SportType id = SportType.Unknown, string date = null) {
            var dateConverted = StringParser.ToDateTime(date, DateTime.Today);
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemRegularBet(LanguageType.English, id, dateConverted);
            return View(new CompetitionRegularModel(GetBaseModel()) {
                CompetitionModel = itemData,
                DateUtc = dateConverted
            });
        }
        // GET: Competition
        public ActionResult Live(SportType id = SportType.Unknown, string date = null) {
            var dateConverted = StringParser.ToDateTime(date, DateTime.Today);
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemRegularBet(LanguageType.English, id, dateConverted);
            return View(new CompetitionRegularModel(GetBaseModel()) {
                CompetitionModel = itemData,
                DateUtc = dateConverted
            });
        }
    }
}