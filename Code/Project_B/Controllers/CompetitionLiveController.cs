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
        public ActionResult Index(SportType id = SportType.Unknown) {
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsLiveBet(LanguageType.English, id);
            var resultData = MainProvider.Instance.ResultProvider.GetResultLiveForCompetitions(itemData.Where(i => i.CurrentBets != null).Select(i => i.CompetitionID).ToArray());
            itemData = itemData.Where(i => resultData.ContainsKey(i.CompetitionID)).ToList();
            return View(new CompetitionRegularModel(GetBaseModel()) {
                CompetitionModel = itemData,
                DateUtc = DateTime.Today,
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
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemLiveBetForCompetition(LanguageType.English, id);
            var resultData = MainProvider.Instance.ResultProvider.GetResultLiveForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new CompetitionRegularModel(GetBaseModel()) {
                CompetitionModel = itemData,
                ResultMap = resultData
            });
        }
    }
}