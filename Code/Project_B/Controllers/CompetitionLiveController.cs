using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class CompetitionLiveController : ProjectControllerBase {
        public override SubNavigationType SubNavigationType => SubNavigationType.SportTypes;
        
        [ActionLog(ProjectBActions.PageLiveIndex)]
        public ActionResult Index(SportType id = SportType.Unknown) {
            LogAction(ProjectBActions.PageLiveIndexConcrete, (short)id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsLive(CurrentLanguage, null, null, id);
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            };
            staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.TraditionalOdds | DisplayColumnType.Result;
            return GetActionResultWithCacheStatus(
                true,
                () => TryGetNotModifiedResultForItems(itemData, staticPageBaseModel.StaticPageTransport.LastModifyDateUtc),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(staticPageBaseModel);
                });
        }

        [ActionLog(ProjectBActions.PageLiveCompetitionUniqueID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageLiveCompetitionUniqueIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsLive(CurrentLanguage, null, null, null, new[] { id });
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            };
            staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.TraditionalOdds | DisplayColumnType.Result;
            return GetActionResultWithCacheStatus(
                itemData != null && itemData.Count > 0,
                () => TryGetNotModifiedResultForItems(itemData, staticPageBaseModel.StaticPageTransport.LastModifyDateUtc),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(staticPageBaseModel);
                });
        }

        [ActionLog(ProjectBActions.PageLiveCompetitionItemID)]
        public ActionResult Game(int id) {
            LogAction(ProjectBActions.PageLiveCompetitionItemIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemLiveBetForCompetition(CurrentLanguage, null, null, id);
            var model = new StaticPageBaseModel<CompetitionAdvancedTransport>(this) {
                ControllerModel = itemData
            };
            return GetActionResultWithCacheStatus(
                itemData?.CompetitionTransport != null,
                () => TryGetNotModifiedResultForGame(itemData, model.StaticPageTransport.LastModifyDateUtc),
                () => {
                    FixToUserTime(itemData.CompetitionTransport);
                    return View(model);
                });
        }
    }
}