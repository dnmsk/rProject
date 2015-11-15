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
    public class CompetitionController : ProjectControllerBase {
        public override SubNavigationType SubNavigationType => SubNavigationType.SportTypes;

        [ActionLog(ProjectBActions.PageCompetitionIndex)]
        public ActionResult Index(SportType id = SportType.Unknown) {
            LogAction(ProjectBActions.PageCompetitionIndexConcrete, (short)id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFutured(CurrentLanguage, id);
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel { 
                    DateUtc = DateTime.Today,
                    Competitions = itemData,
                    LimitToDisplayInGroup = 4
               }
            });
        }

        [ActionLog(ProjectBActions.PageCompetitionUniqueID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageCompetitionUniqueIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFutured(CurrentLanguage, null, new[] {id});
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            });
        }

        [ActionLog(ProjectBActions.PageCompetitionItemID)]
        public ActionResult Game(int id) {
            LogAction(ProjectBActions.PageCompetitionItemIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemRegularBet(CurrentLanguage, id);
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = new List<CompetitionTransport> { itemData },
                }
            });
        }
    }
}