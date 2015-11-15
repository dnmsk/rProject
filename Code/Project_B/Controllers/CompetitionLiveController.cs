using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class CompetitionLiveController : ProjectControllerBase {
        public override SubNavigationType SubNavigationType => SubNavigationType.SportTypes;

        // GET: CompetitionLive
        [ActionLog(ProjectBActions.PageLiveIndex)]
        public ActionResult Index(SportType id = SportType.Unknown) {
            LogAction(ProjectBActions.PageLiveIndexConcrete, (short)id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsLive(CurrentLanguage, id);
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    DateUtc = DateTime.Today,
                    Competitions = itemData,
                }
            });
        }

        [ActionLog(ProjectBActions.PageLiveCompetitionUniqueID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageLiveCompetitionUniqueIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsLive(CurrentLanguage, null, new[] { id });
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            });
        }

        [ActionLog(ProjectBActions.PageLiveCompetitionItemID)]
        public ActionResult Game(int id) {
            LogAction(ProjectBActions.PageLiveCompetitionItemIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemLiveBetForCompetition(CurrentLanguage, id);
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = new List<CompetitionTransport> { itemData}
                }
            });
        }
    }
}