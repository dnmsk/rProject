using System;
using System.Linq;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class HistoryController : ProjectControllerBase {
        private static readonly DateTime _minDateTime = new DateTime(2014, 01, 01);
        private static DateTime MaxDateTime => DateTime.UtcNow.Date;
        public override SubNavigationType SubNavigationType => SubNavigationType.SportTypes;

        [ActionLog(ProjectBActions.PageHistoryIndex)]
        [ActionProfile(ProjectBActions.PageHistoryIndex)]
        public ActionResult Index(FilterModel<SportType> filter) {
            LogAction(ProjectBActions.PageHistoryIndexConcrete, (short)filter.id);
            filter.FixDates(_minDateTime, MaxDateTime);
            var fromDate = FixUserTimeToSystem(filter.date);
            var itemData = FrontCompetitionProvider.GetCompetitionItemsHistory(CurrentLanguage, null, new[] {BrokerType.Default}, fromDate, fromDate.AddDays(1), filter.id);
            var model = new StaticPageBaseModel<CompetitionRegularModel<CompetitionItemBetTransport>>(this) {
                ControllerModel = new CompetitionRegularModel<CompetitionItemBetTransport>(new PageDisplaySettings {
                        DisplayColumn = DisplayColumnType.TraditionalOdds | DisplayColumnType.Result
                    }) {
                    Competitions = itemData,
                    Filter = new FilterModel<SportType>("Index", "History" , CurrentLanguage, FilterSettings.ToDate, filter)
                }
            };
            return new ActionResultCached(
                true,
                model.GetLastModifiedFunc(() => TryGetNotModifiedResultForItems(itemData, model.StaticPageTransport.LastModifyDateUtc)),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(model);
                });
        }

        [ActionLog(ProjectBActions.PageHistoryCompetitionUniqueID)]
        [ActionProfile(ProjectBActions.PageHistoryCompetitionUniqueID)]
        public ActionResult Item(FilterModel<int> filter) {
            LogAction(ProjectBActions.PageHistoryCompetitionUniqueIDConcrete, filter.id);
            filter.FixDates(_minDateTime, MaxDateTime);
            var itemData = FrontCompetitionProvider
                .GetCompetitionItemsHistory(CurrentLanguage, null, new[] { BrokerType.Default }, filter.from, filter.date, null, new [] { filter.id });
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel<CompetitionItemBetTransport>>(this) {
                    ControllerModel = new CompetitionRegularModel<CompetitionItemBetTransport>(new PageDisplaySettings {
                        DisplayColumn = DisplayColumnType.TraditionalOdds | DisplayColumnType.Result
                    }) {
                    Competitions = itemData,
                        Filter = new FilterModel<int>("Item", "History", CurrentLanguage, FilterSettings.FromDate | FilterSettings.ToDate, filter)
                    }
            };
            return new ActionResultCached(
                itemData.Any(),
                staticPageBaseModel.GetLastModifiedFunc(() => TryGetNotModifiedResultForItems(itemData, staticPageBaseModel.StaticPageTransport.LastModifyDateUtc)),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(staticPageBaseModel);
                });
        }

        [ActionLog(ProjectBActions.PageHistoryCompetitorID)]
        [ActionProfile(ProjectBActions.PageHistoryCompetitorID)]
        public ActionResult Competitor(FilterModel<int> filter) {
            LogAction(ProjectBActions.PageHistoryCompetitorIDConcrete, filter.id);
            filter.FixDates(_minDateTime, MaxDateTime);
            var itemData = FrontCompetitionProvider
                .GetCompetitionItemsRegularBetForCompetitor(CurrentLanguage, null, new[] { BrokerType.Default }, filter.from, filter.date, filter.id);
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel<CompetitionItemBetTransport>>(this) {
                    ControllerModel = new CompetitionRegularModel<CompetitionItemBetTransport>(new PageDisplaySettings {
                        DisplayColumn = DisplayColumnType.TraditionalOdds | DisplayColumnType.Result
                    }) {
                    Competitions = itemData,
                    Filter = new FilterModel<int>("Competitor", "History", CurrentLanguage, FilterSettings.FromDate | FilterSettings.ToDate, filter)
                }
            };
            return new ActionResultCached(
                itemData.Any(),
                staticPageBaseModel.GetLastModifiedFunc(() => TryGetNotModifiedResultForItems(itemData, staticPageBaseModel.StaticPageTransport.LastModifyDateUtc)),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(staticPageBaseModel);
                });
        }
    }
}