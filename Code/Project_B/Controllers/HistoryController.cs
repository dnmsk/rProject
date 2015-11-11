using System;
using System.Linq;
using System.Web.Mvc;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using Project_B.CodeClientSide;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class HistoryController : ProjectControllerBase {
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
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBet(CurrentLanguage, id, fromDate, toDate);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new StaticPageBaseModel<CompetitionRegularModel>(CurrentLanguage, ProjectBActions.PageHistoryIndex, GetBaseModel()) {
                ControllerModel = new CompetitionRegularModel {
                    CompetitionModel = itemData,
                    DateUtc = fromDate,
                    ResultMap = resultData
                }
            });
        }

        [ActionLog(ProjectBActions.PageHistoryCompetitionID)]
        public ActionResult Game(int id) {
            LogAction(ProjectBActions.PageHistoryCompetitionIDConcrete, id);
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBetForCompetition(CurrentLanguage, id, false);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new StaticPageBaseModel<CompetitionRegularModel>(CurrentLanguage, ProjectBActions.PageHistoryCompetitionID, GetBaseModel()) {
                ControllerModel = new CompetitionRegularModel {
                    CompetitionModel = itemData,
                    ResultMap = resultData
                }
            });
        }

        [ActionLog(ProjectBActions.PageHistoryCompetitorID)]
        public ActionResult Competitor(int id) {
            LogAction(ProjectBActions.PageHistoryCompetitorIDConcrete, id);
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBetForCompetitor(CurrentLanguage, id);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new StaticPageBaseModel<CompetitionRegularModel>(CurrentLanguage, ProjectBActions.PageHistoryCompetitorID, GetBaseModel()) {
                ControllerModel = new CompetitionRegularModel {
                    CompetitionModel = itemData,
                    ResultMap = resultData
                }
            });
        }
    }
}