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
        [ActionLog(ProjectBActions.PageCompetitionIndex)]
        public ActionResult Index(SportType id = SportType.Unknown) {
            LogAction(ProjectBActions.PageCompetitionIndexConcrete, (short)id);
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBet(CurrentLanguage, id, DateTime.UtcNow, DateTime.MaxValue);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new StaticPageBaseModel<CompetitionRegularModel>(CurrentLanguage, ProjectBActions.PageCompetitionIndex, GetBaseModel()) {
                ControllerModel = new CompetitionRegularModel { 
                    CompetitionModel = itemData,
                    DateUtc = DateTime.Today,
                    ResultMap = resultData,
                    LimitToDisplayInGroup = 4
               }
            });
        }

        [ActionLog(ProjectBActions.PageCompetitionItemID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageCompetitionItemIDConcrete, id);
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemRegularBet(CurrentLanguage, id);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(new[] { itemData.CompetitionID });
            return View(new StaticPageBaseModel<CompetitionRegularModel>(CurrentLanguage, ProjectBActions.PageCompetitionItemID, GetBaseModel()) {
                ControllerModel = new CompetitionRegularModel {
                    CompetitionModel = new List<CompetitionItemBetShortTransport> { itemData },
                    ResultMap = resultData
                }
            });
        }

        [ActionLog(ProjectBActions.PageCompetitionGame)]
        public ActionResult Game(int id) {
            LogAction(ProjectBActions.PageCompetitionGameConcrete, id);
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBetForCompetition(CurrentLanguage, id, true);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new StaticPageBaseModel<CompetitionRegularModel>(CurrentLanguage, ProjectBActions.PageCompetitionGame, GetBaseModel()) {
                ControllerModel = new CompetitionRegularModel {
                    CompetitionModel = itemData,
                    ResultMap = resultData
                }
            });
        }
    }
}