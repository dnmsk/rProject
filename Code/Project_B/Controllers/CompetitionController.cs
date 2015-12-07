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
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFutured(CurrentLanguage, BrokerType.All, BrokerType.All, id);
            return GetActionResultWithStatus(
                () => true,
                () => GetNotModifiedResultForItems(itemData),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                        ControllerModel = new CompetitionRegularModel {
                            Competitions = itemData,
                            Filter = new FilterModel {
                                LimitToDisplayInGroup = 4,
                                SportType = id,
                                DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds
                            }
                        }
                    });
                });
        }

        [ActionLog(ProjectBActions.PageCompetitionUniqueID)]
        public ActionResult Item(int id) {
            LogAction(ProjectBActions.PageCompetitionUniqueIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFutured(CurrentLanguage, BrokerType.All, BrokerType.All, null, new[] {id});
            return GetActionResultWithStatus(
                () => itemData != null && itemData.Count > 0,
                () => GetNotModifiedResultForItems(itemData),
                () => {
                    itemData.Each(FixToUserTime);
                    var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel>(this) {
                        ControllerModel = new CompetitionRegularModel {
                            Competitions = itemData,
                        }
                    };
                    staticPageBaseModel.ControllerModel.Filter.DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds;
                    return View(staticPageBaseModel);
                });
        }

        [ActionLog(ProjectBActions.PageCompetitionItemID)]
        public ActionResult Game(int id) {
            LogAction(ProjectBActions.PageCompetitionItemIDConcrete, id);
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemRegularBet(CurrentLanguage, BrokerType.All, BrokerType.All, id);
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

        [ActionLog(ProjectBActions.PageCompetitionProfitable)]
        public ActionResult Profitable(SportType id = SportType.Unknown) {
            var itemData = ProjectProvider.Instance.CompetitionProvider.GetCompetitionItemsFuturedProfitable(CurrentLanguage, BrokerType.All, BrokerType.All, id);
            return GetActionResultWithStatus(
                () => true,
                () => GetNotModifiedResultForItems(itemData),
                () => {
                    itemData.Each(FixToUserTime);
                    return View(new StaticPageBaseModel<CompetitionRegularModel>(this) {
                        ControllerModel = new CompetitionRegularModel {
                            Competitions = itemData,
                            Filter = new FilterModel {
                                LimitToDisplayInGroup = 4,
                                SportType = id,
                                DisplayColumn = DisplayColumnType.MaxRoi | DisplayColumnType.TraditionalOdds
                            }
                        }
                    });
                });
        }
    }
}