using System;
using System.Web.Mvc;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class HistoryController : ProjectControllerBase {
        public override SubNavigationType SubNavigationType => SubNavigationType.SportTypes;

        [ActionLog(ProjectBActions.PageHistoryIndex)]
        public ActionResult Index(SportType id = SportType.Unknown, string date = null) {
            LogAction(ProjectBActions.PageHistoryIndexConcrete, (short) id);
            DateTime fromDate;
            DateTime toDate;
            if (!date.IsNullOrWhiteSpace()) {
                fromDate = StringParser.ToDateTime(date, DateTime.UtcNow).Date;
                if (fromDate.Date >= DateTime.UtcNow.Date) {
                    toDate = DateTime.UtcNow;
                    fromDate = toDate.Date;
                } else {
                    toDate = fromDate.AddDays(1);
                }
            } else {
                toDate = DateTime.UtcNow;
                fromDate = toDate.Date;
            }
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsHistory(CurrentLanguage, BrokerType.All, BrokerType.Default, fromDate, toDate, id);
            itemData.Each(FixToUserTime);
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                    Filter = new FilterModel {
                        SportType = id,
                        DateUtc = fromDate,
                        DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result
                    }
                }
            });
        }

        [ActionLog(ProjectBActions.PageHistoryCompetitionUniqueID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageHistoryCompetitionUniqueIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsHistory(CurrentLanguage, BrokerType.All, BrokerType.Default, DateTime.MinValue, DateTime.MaxValue, null, new [] { id });
            itemData.Each(FixToUserTime);
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            };
            staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result;
            return View(staticPageBaseModel);
        }

        [ActionLog(ProjectBActions.PageHistoryCompetitorID)]
        public ActionResult Competitor(int id) {
            LogAction(ProjectBActions.PageHistoryCompetitorIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBetForCompetitor(CurrentLanguage, BrokerType.All, BrokerType.Default, id);
            itemData.Each(FixToUserTime);
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            };
            staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result;
            return View(staticPageBaseModel);
        }
    }
}