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
    public class CompetitionLiveController : ProjectControllerBase {
        // GET: CompetitionLive
        [ActionLog(ProjectBActions.PageLiveIndex)]
        public ActionResult Index(SportType id = SportType.Unknown) {
            LogAction(ProjectBActions.PageLiveIndexConcrete, (short)id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsLiveBet(CurrentLanguage, id);
            var resultData = ProjectProvider.Instance.ResultProvider.GetResultLiveForCompetitions(itemData.Where(i => i.CurrentBets != null).Select(i => i.CompetitionID).ToArray());
            itemData = itemData.Where(i => resultData.ContainsKey(i.CompetitionID)).ToList();
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    CompetitionModel = itemData,
                    DateUtc = DateTime.Today,
                    ResultMap = resultData
                }
            });
        }

        [ActionLog(ProjectBActions.PageLiveCompetitionItemID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageLiveCompetitionItemIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemLiveBet(CurrentLanguage, id);
            var resultData = ProjectProvider.Instance.ResultProvider.GetResultLiveForCompetitions(new[] {itemData.CompetitionID});
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    CompetitionModel = new List<CompetitionItemBetShortTransport> {itemData},
                    ResultMap = resultData
                }
            });
        }

        [ActionLog(ProjectBActions.PageLiveGameID)]
        public ActionResult Game(int id) {
            LogAction(ProjectBActions.PageLiveGameIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemLiveBetForCompetition(CurrentLanguage, id);
            var resultData = ProjectProvider.Instance.ResultProvider.GetResultLiveForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    CompetitionModel = itemData,
                    ResultMap = resultData
                }
            });
        }
    }
}