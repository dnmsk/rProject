using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MainLogic.WebFiles;
using Project_B.Code.DataProvider;
using Project_B.Code.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class CompetitionLiveController : ApplicationControllerBase {
        // GET: CompetitionLive
        public ActionResult Index() {
            return RedirectToAction("Regular");
        }

        // GET: Competition
        public ActionResult Regular(SportType id = SportType.Unknown) {
            var dateConverted = DateTime.UtcNow.Date;
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsLiveBet(LanguageType.English, id,
                dateConverted);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new CompetitionRegularModel(GetBaseModel()) {
                CompetitionModel = itemData,
                DateUtc = dateConverted,
                ResultMap = resultData
            });
        }

        public ActionResult Item(int id) {
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemLiveBet(LanguageType.English, id);
            var resultData = MainProvider.Instance.ResultProvider.GetResultLiveForCompetitions(new[] {itemData.CompetitionID});
            return View(new CompetitionRegularModel(GetBaseModel()) {
                CompetitionModel = new List<CompetitionItemBetShortModel> {itemData},
                ResultMap = resultData
            });
        }

        public ActionResult Game(int id) {
            var itemData =
                MainProvider.Instance.CompetitionProvider.GetCompetitionItemLiveBetForCompetition(LanguageType.English,
                    id);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new CompetitionRegularModel(GetBaseModel()) {
                CompetitionModel = itemData,
                ResultMap = resultData
            });
        }

        public ActionResult Competitor(int id) {
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemLiveBetForCompetitor(LanguageType.English, id);
            var resultData = MainProvider.Instance.ResultProvider.GetResultLiveForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new CompetitionRegularModel(GetBaseModel()) {
                CompetitionModel = itemData,
                ResultMap = resultData
            });
        }
    }
}