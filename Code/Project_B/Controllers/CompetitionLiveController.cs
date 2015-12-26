using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class CompetitionLiveController : ProjectControllerBase {
        public override SubNavigationType SubNavigationType => SubNavigationType.SportTypes;
        
        [ActionLog(ProjectBActions.PageLiveIndex)]
        [ActionProfile(ProjectBActions.PageLiveIndex)]
        public ActionResult Index(FilterModel<SportType> filter) {
            LogAction(ProjectBActions.PageLiveIndexConcrete, (short)filter.id);
            var itemData = FrontCompetitionProvider.GetCompetitionItemsLive(CurrentLanguage, filter.all, null, null, filter.id);
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                    ControllerModel = new CompetitionRegularModel(new PageDisplaySettings {
                        DisplayColumn = DisplayColumnType.TraditionalOdds | DisplayColumnType.Result
                    }) {
                    Competitions = itemData,
                    Filter = new FilterModel<SportType>("Index", "CompetitionLive", CurrentLanguage, FilterSettings.BtnAll | FilterSettings.BtnWithOdds, filter)
                }
            };
            return new ActionResultCached(
                true,
                () => TryGetNotModifiedResultForItems(itemData, staticPageBaseModel.StaticPageTransport.LastModifyDateUtc),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(staticPageBaseModel);
                });
        }

        [ActionLog(ProjectBActions.PageLiveCompetitionUniqueID)]
        [ActionProfile(ProjectBActions.PageLiveCompetitionUniqueID)]
        public ActionResult Item(FilterModel<int> filter) {
            LogAction(ProjectBActions.PageLiveCompetitionUniqueIDConcrete, filter.id);
            var itemData = FrontCompetitionProvider.GetCompetitionItemsLive(CurrentLanguage, true, null, null, null, new[] { filter.id });
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel(new PageDisplaySettings {
                        DisplayColumn = DisplayColumnType.TraditionalOdds | DisplayColumnType.Result
                    }) {
                    Competitions = itemData
                }
            };
            return new ActionResultCached(
                itemData != null && itemData.Count > 0,
                () => TryGetNotModifiedResultForItems(itemData, staticPageBaseModel.StaticPageTransport.LastModifyDateUtc),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(staticPageBaseModel);
                });
        }

        [ActionLog(ProjectBActions.PageLiveCompetitionItemID)]
        [ActionProfile(ProjectBActions.PageLiveCompetitionItemID)]
        public ActionResult Game(FilterModel<int> filter) {
            LogAction(ProjectBActions.PageLiveCompetitionItemIDConcrete, filter.id);
            var itemData = FrontCompetitionProvider.GetCompetitionItemLiveBetForCompetition(CurrentLanguage, null, null, filter.id);
            var model = new StaticPageBaseModel<CompetitionAdvancedTransport>(this) {
                ControllerModel = itemData
            };
            return new ActionResultCached(
                itemData?.CompetitionTransport != null,
                () => TryGetNotModifiedResultForGame(itemData, model.StaticPageTransport.LastModifyDateUtc),
                () => {
                    FixToUserTime(itemData.CompetitionTransport);
                    return View(model);
                });
        }
    }
}