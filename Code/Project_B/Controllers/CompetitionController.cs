using System;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
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
        [ActionProfile(ProjectBActions.PageCompetitionIndex)]
        public ActionResult Index(FilterModel<SportType> filter) {
            LogAction(ProjectBActions.PageCompetitionIndexConcrete, (short)filter.id);
            var dateDef = DateTime.UtcNow.Date;
            filter.date = FixDateTime(filter.date, dateDef, dateDef.AddDays(14));
            var fromDate = FixUserTimeToSystem(filter.date);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFuturedNew(CurrentLanguage, null, null, filter.id);
            var model = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel(new PageDisplaySettings {
                    LimitToDisplayInGroup = 4,
                    DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds
                }) {
                    Competitions = itemData,
                    Filter = new FilterModel<SportType>("Index", "Competition", CurrentLanguage, FilterSettings.BtnAll | FilterSettings.ToDate, filter) {
                        MinDate = dateDef,
                        MaxDate = dateDef.AddDays(7)
                    }
                }
            };
            return new ActionResultCached(
                true,
                () => TryGetNotModifiedResultForItems(itemData, model.StaticPageTransport.LastModifyDateUtc),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(model);
                });
        }

        [ActionLog(ProjectBActions.PageCompetitionUniqueID)]
        [ActionProfile(ProjectBActions.PageCompetitionUniqueID)]
        public ActionResult Item(FilterModel<int> filter) {
            LogAction(ProjectBActions.PageCompetitionUniqueIDConcrete, filter.id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFuturedNew(CurrentLanguage, null, null, null, new[] {filter.id});
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel(new PageDisplaySettings {
                    DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds
                }) {
                    Competitions = itemData,
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

        [ActionLog(ProjectBActions.PageCompetitionItemID)]
        [ActionProfile(ProjectBActions.PageCompetitionItemID)]
        public ActionResult Game(FilterModel<int> filter) {
            LogAction(ProjectBActions.PageCompetitionItemIDConcrete, filter.id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemRegularBet(CurrentLanguage, null, null, filter.id);
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

        [ActionLog(ProjectBActions.PageCompetitionProfitable)]
        [ActionProfile(ProjectBActions.PageCompetitionProfitable)]
        public ActionResult Profitable(FilterModel<SportType> filter) {
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFuturedProfitable(CurrentLanguage, null, null, filter.id);
            var model = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel(new PageDisplaySettings {
                        LimitToDisplayInGroup = 4,
                        DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds
                    }) {
                    Competitions = itemData,
                    Filter = new FilterModel<SportType>(null, null, CurrentLanguage, FilterSettings.Default, filter)
                }
            };
            return new ActionResultCached(
                true,
                () => DateTime.MinValue,
                () => {
                    itemData.Each(FixToUserTime);
                    return View(model);
                });
        }
    }
}