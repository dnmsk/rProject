using System;
using System.Web.Mvc;
using CommonUtils.Code;
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
        public ActionResult Index(SportType id = SportType.Unknown, string date = null) {
            LogAction(ProjectBActions.PageHistoryIndexConcrete, (short) id);
            var fromDateUtc = StringParser.ToDateTime(date, DateTime.MaxValue).Date;
            var now = DateTime.UtcNow.Date;
            if (fromDateUtc >= now) {
                fromDateUtc = now;
            }
            var fromDate = FixUserTimeToSystem(fromDateUtc);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsHistory(CurrentLanguage, null, new[] {BrokerType.Default}, fromDate, fromDate.AddDays(1), id);
            var model = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                    Filter = new FilterModel {
                        SportType = id,
                        DateUtc = fromDateUtc,
                        DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result
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
        public ActionResult Item(int id, string from = null, string to = null) {
            LogAction(ProjectBActions.PageHistoryCompetitionUniqueIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider
                .GetCompetitionItemsHistory(CurrentLanguage, null, new[] { BrokerType.Default }, ParseToUserTime(from, _minDateTime), ParseToUserTime(to, MaxDateTime), null, new [] { id });
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            };
            staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result;
            return new ActionResultCached(
                true,
                () => TryGetNotModifiedResultForItems(itemData, staticPageBaseModel.StaticPageTransport.LastModifyDateUtc),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(staticPageBaseModel);
                });
        }

        [ActionLog(ProjectBActions.PageHistoryCompetitorID)]
        public ActionResult Competitor(int id, string from = null, string to = null) {
            LogAction(ProjectBActions.PageHistoryCompetitorIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider
                .GetCompetitionItemsRegularBetForCompetitor(CurrentLanguage, null, new[] { BrokerType.Default }, ParseToUserTime(from, _minDateTime), ParseToUserTime(to, MaxDateTime), id);
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                    Filter = new FilterModel {
                        SelectedID = id
                    }
                }
            };
            staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result;
            return new ActionResultCached(
                true,
                () => TryGetNotModifiedResultForItems(itemData, staticPageBaseModel.StaticPageTransport.LastModifyDateUtc),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(staticPageBaseModel);
                });
        }

        private static readonly string[] _dateTimeFormats = {"dd.MM.yyyy", "MM/dd/yyyy"};
        private DateTime ParseToUserTime(string date, DateTime dateDef) {
            var dateParsed = dateDef;
            foreach (var dateTimeFormat in _dateTimeFormats) {
                if ((dateParsed = StringParser.ToDateTime(date, dateDef, dateTimeFormat)) != dateDef) {
                    break;
                }
            }
            if (dateParsed <= _minDateTime) {
                dateParsed = dateDef;
            }
            if (dateParsed > MaxDateTime) {
                dateParsed = dateDef;
            }
            return FixUserTimeToSystem(dateParsed);
        }
    }
}