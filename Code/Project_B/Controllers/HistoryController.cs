using System;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.DataProvider;
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
            filter.date = FixDateTime(filter.date, _minDateTime, MaxDateTime);
            var fromDate = FixUserTimeToSystem(filter.date);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsHistory(CurrentLanguage, null, new[] {BrokerType.Default}, fromDate, fromDate.AddDays(1), filter.id);
            var model = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel(new PageDisplaySettings {
                        DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result
                    }) {
                    Competitions = itemData,
                    Filter = new FilterModel<SportType>("Index", "History" , CurrentLanguage, FilterSettings.ToDate, filter) {
                        MinDate = _minDateTime,
                        MaxDate = MaxDateTime
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

        [ActionLog(ProjectBActions.PageHistoryCompetitionUniqueID)]
        [ActionProfile(ProjectBActions.PageHistoryCompetitionUniqueID)]
        public ActionResult Item(FilterModel<int> filter) {
            LogAction(ProjectBActions.PageHistoryCompetitionUniqueIDConcrete, filter.id);
            var itemData = ProjectProvider.Instance.CompetitionProvider
                .GetCompetitionItemsHistory(CurrentLanguage, null, new[] { BrokerType.Default }, 
                        FixDateTime(filter.from, _minDateTime, MaxDateTime), 
                        FixDateTime(filter.date, _minDateTime, MaxDateTime), null, new [] { filter.id });
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                    ControllerModel = new CompetitionRegularModel(new PageDisplaySettings {
                        DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result
                    }) {
                    Competitions = itemData,
                        Filter = filter
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

        [ActionLog(ProjectBActions.PageHistoryCompetitorID)]
        [ActionProfile(ProjectBActions.PageHistoryCompetitorID)]
        public ActionResult Competitor(FilterModel<int> filter) {
            LogAction(ProjectBActions.PageHistoryCompetitorIDConcrete, filter.id);
            var itemData = ProjectProvider.Instance.CompetitionProvider
                .GetCompetitionItemsRegularBetForCompetitor(CurrentLanguage, null, new[] { BrokerType.Default }, 
                            FixDateTime(filter.from, _minDateTime, MaxDateTime), 
                            FixDateTime(filter.date, _minDateTime, MaxDateTime),
                            filter.id);
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                    ControllerModel = new CompetitionRegularModel(new PageDisplaySettings {
                        DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result
                    }) {
                    Competitions = itemData,
                    Filter = new FilterModel<int>("Competitor", "History", CurrentLanguage, FilterSettings.FromDate | FilterSettings.ToDate, filter) {
                        MinDate = _minDateTime,
                        MaxDate = MaxDateTime
                    }
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
    }
}