﻿using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Enums;
using Project_B.Models;

namespace Project_B.Controllers {
    public class CompetitionLiveController : ProjectControllerBase {
        public override SubNavigationType SubNavigationType => SubNavigationType.SportTypes;

        // GET: CompetitionLive
        [ActionLog(ProjectBActions.PageLiveIndex)]
        public ActionResult Index(SportType id = SportType.Unknown) {
            LogAction(ProjectBActions.PageLiveIndexConcrete, (short)id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsLive(CurrentLanguage, BrokerType.All, BrokerType.All, id);
            return GetActionResultWithStatus(
                () => true,
                () => GetNotModifiedResultForItems(itemData),
                () => {
                    itemData.Each(FixToUserTime);
                    var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                        ControllerModel = new CompetitionRegularModel {
                            Competitions = itemData,
                        }
                    };
                    staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result;
                    return View(staticPageBaseModel);
                });
        }

        [ActionLog(ProjectBActions.PageLiveCompetitionUniqueID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageLiveCompetitionUniqueIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsLive(CurrentLanguage, BrokerType.All, BrokerType.All, null, new[] { id });
            return GetActionResultWithStatus(
                () => true,
                () => GetNotModifiedResultForItems(itemData),
                () => {
                    itemData.Each(FixToUserTime);
                    var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                        ControllerModel = new CompetitionRegularModel {
                            Competitions = itemData,
                        }
                    };
                    staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds | DisplayColumnType.Result;
                    return View(staticPageBaseModel);
                });
        }

        [ActionLog(ProjectBActions.PageLiveCompetitionItemID)]
        public ActionResult Game(int id) {
            LogAction(ProjectBActions.PageLiveCompetitionItemIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemLiveBetForCompetition(CurrentLanguage, BrokerType.All, BrokerType.All, id);
            return GetActionResultWithStatus(
                () => itemData?.CompetitionTransport != null,
                () => GetNotModifiedResultForGame(itemData),
                () => {
                    FixToUserTime(itemData.CompetitionTransport);
                    return View(new StaticPageBaseModel<CompetitionAdvancedTransport>(this) {
                        ControllerModel = itemData
                    });
                });
        }
    }
}