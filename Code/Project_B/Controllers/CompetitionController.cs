using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
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
            itemData.Each(FixToUserTime);
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel { 
                    Competitions = itemData,
                    Filter = new FilterModel {
                        LimitToDisplayInGroup = 4,
                        SportType = id,
                        DisplayColumn = DisplayColumnType.Roi1X2 | DisplayColumnType.TraditionalOdds | DisplayColumnType.HandicapOdds | DisplayColumnType.TotalOdds
                    }
               }
            });
        }

        [ActionLog(ProjectBActions.PageCompetitionUniqueID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageCompetitionUniqueIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFutured(CurrentLanguage, null, new[] {id});
            itemData.Each(FixToUserTime);
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            };
            staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.Roi1X2 | DisplayColumnType.TraditionalOdds | DisplayColumnType.HandicapOdds | DisplayColumnType.TotalOdds;
            return View(staticPageBaseModel);
        }

        [ActionLog(ProjectBActions.PageCompetitionItemID)]
        public ActionResult Game(int id) {
            LogAction(ProjectBActions.PageCompetitionItemIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemRegularBet(CurrentLanguage, id);
            FixToUserTime(itemData.CompetitionTransport);
            return View(new StaticPageBaseModel<CompetitionAdvancedTransport>(this) {
                ControllerModel = itemData
            });
        }
    }
}