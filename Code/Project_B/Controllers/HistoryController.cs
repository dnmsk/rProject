using System;
using System.Linq;
using System.Web.Mvc;
using CommonUtils.Code;
using CommonUtils.ExtendedTypes;
using Project_B.Code;
using Project_B.Code.DataProvider;
using Project_B.Code.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class HistoryController : ProjectControllerBase {
        public ActionResult Index(SportType id = SportType.Unknown, string date = null) {
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
            return View(new CompetitionRegularModel(CurrentLanguage, GetBaseModel()) {
                CompetitionModel = itemData,
                DateUtc = fromDate,
                ResultMap = resultData
            });
        }
        
        public ActionResult Game(int id) {
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBetForCompetition(CurrentLanguage, id, false);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new CompetitionRegularModel(CurrentLanguage, GetBaseModel()) {
                CompetitionModel = itemData,
                ResultMap = resultData
            });
        }

        public ActionResult Competitor(int id) {
            var itemData = MainProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBetForCompetitor(CurrentLanguage, id);
            var resultData = MainProvider.Instance.ResultProvider.GetResultForCompetitions(itemData.Select(i => i.CompetitionID).ToArray());
            return View(new CompetitionRegularModel(CurrentLanguage, GetBaseModel()) {
                CompetitionModel = itemData,
                ResultMap = resultData
            });
        }
    }
}