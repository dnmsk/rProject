using System;
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
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFutured(CurrentLanguage, null, null, id);
            var model = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                    Filter = new FilterModel {
                        LimitToDisplayInGroup = 4,
                        SportType = id,
                        DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds
                    }
                }
            };
            return GetActionResultWithCacheStatus(
                true,
                () => TryGetNotModifiedResultForItems(itemData, model.StaticPageTransport.LastModifyDateUtc),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(model);
                });
        }

        [ActionLog(ProjectBActions.PageCompetitionUniqueID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageCompetitionUniqueIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFutured(CurrentLanguage, null, null, null, new[] {id});
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            };
            staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds;
            return GetActionResultWithCacheStatus(
                itemData != null && itemData.Count > 0,
                () => TryGetNotModifiedResultForItems(itemData, staticPageBaseModel.StaticPageTransport.LastModifyDateUtc),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(staticPageBaseModel);
                });
        }

        [ActionLog(ProjectBActions.PageCompetitionItemID)]
        public ActionResult Game(int id) {
            LogAction(ProjectBActions.PageCompetitionItemIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemRegularBet(CurrentLanguage, null, null, id);
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

        [ActionLog(ProjectBActions.PageCompetitionProfitable)]
        public ActionResult Profitable(SportType id = SportType.Unknown) {
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFuturedProfitable(CurrentLanguage, null, null, id);
            var model = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                    Filter = new FilterModel {
                        LimitToDisplayInGroup = 4,
                        SportType = id,
                        DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds
                    }
                }
            };
            return GetActionResultWithCacheStatus(
                true,
                () => TryGetNotModifiedResult(DateTime.MinValue),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(model);
                });
        }
    }
}