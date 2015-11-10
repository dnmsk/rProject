using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class CompetitionController : ProjectControllerBase {
        public ActionResult Index(SportType id = SportType.Unknown) {
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBet(CurrentLanguage, id, DateTime.UtcNow, DateTime.MaxValue);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new StaticPageBaseModel<CompetitionRegularModel>(CurrentLanguage, PageType.CompetitionIndex, GetBaseModel()) {
                ControllerModel = new CompetitionRegularModel { 
                    CompetitionModel = itemData,
                    DateUtc = DateTime.Today,
                    ResultMap = resultData,
                    LimitToDisplayInGroup = 4
               }
            });
        }

        public ActionResult Item(int id) {
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemRegularBet(CurrentLanguage, id);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(new[] { itemData.CompetitionID });
            return View(new StaticPageBaseModel<CompetitionRegularModel>(CurrentLanguage, PageType.CompetitionItemID, GetBaseModel()) {
                ControllerModel = new CompetitionRegularModel {
                    CompetitionModel = new List<CompetitionItemBetShortTransport> { itemData },
                    ResultMap = resultData
                }
            });
        }

        public ActionResult Game(int id) {
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBetForCompetition(CurrentLanguage, id, true);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new StaticPageBaseModel<CompetitionRegularModel>(CurrentLanguage, PageType.CompetitionID, GetBaseModel()) {
                ControllerModel = new CompetitionRegularModel {
                    CompetitionModel = itemData,
                    ResultMap = resultData
                }
            });
        }
    }
}