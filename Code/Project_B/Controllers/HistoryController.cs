﻿using System;
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
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsHistory(CurrentLanguage, fromDate, toDate, id);
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                    DateUtc = fromDate,
                }
            });
        }

        [ActionLog(ProjectBActions.PageHistoryCompetitionUniqueID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageHistoryCompetitionUniqueIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsHistory(CurrentLanguage, DateTime.MinValue, DateTime.MaxValue, null, new [] { id });
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            });
        }

        [ActionLog(ProjectBActions.PageHistoryCompetitorID)]
        public ActionResult Competitor(int id) {
            LogAction(ProjectBActions.PageHistoryCompetitorIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsRegularBetForCompetitor(CurrentLanguage, id);
            return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                ControllerModel = new CompetitionRegularModel {
                    Competitions = itemData,
                }
            });
        }
    }
}