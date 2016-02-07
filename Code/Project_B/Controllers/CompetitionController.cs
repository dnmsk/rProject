using System;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeClientSide.TransportType;
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
            var maxDateDef = dateDef.AddDays(14);
            filter.FixDates(dateDef, maxDateDef);
            var itemData = FrontCompetitionProvider.GetCompetitionItemsFuturedNew(CurrentLanguage, filter.all ? null : (DateTime?)FixUserTimeToSystem(filter.from), null, null, filter.id);
            var model = new StaticPageBaseModel<CompetitionRegularModel<CompetitionItemBetTransport>>(this) {
                ControllerModel = new CompetitionRegularModel<CompetitionItemBetTransport>(new PageDisplaySettings {
                    LimitToDisplayInGroup = 4,
                    DisplayColumn = DisplayColumnType.TraditionalOdds
                }) {
                    Competitions = itemData,
                    Filter = new FilterModel<SportType>("Index", "Competition", CurrentLanguage, FilterSettings.BtnAll | FilterSettings.FromDate, filter)
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
            var itemData = FrontCompetitionProvider.GetCompetitionItemsFuturedNew(CurrentLanguage, null, null, null, null, new[] {filter.id});
            var staticPageBaseModel = new StaticPageBaseModel<CompetitionRegularModel<CompetitionItemBetTransport>>(this) {
                ControllerModel = new CompetitionRegularModel<CompetitionItemBetTransport>(new PageDisplaySettings {
                    DisplayColumn = DisplayColumnType.TraditionalOdds
                }) {
                    Competitions = itemData
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
            var itemData = FrontCompetitionProvider.GetCompetitionItemRegularBet(CurrentLanguage, null, null, filter.id);
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
            var itemData = FrontCompetitionProvider.GetCompetitionItemsFuturedProfitable(CurrentLanguage, null, null, filter.id);
            var model = new StaticPageBaseModel<CompetitionRegularModel<CompetitionItemRoiTransport>>(this) {
                ControllerModel = new CompetitionRegularModel<CompetitionItemRoiTransport>(new PageDisplaySettings {
                        LimitToDisplayInGroup = 4,
                        DisplayColumn = DisplayColumnType.Roi
                    }) {
                    Competitions = itemData,
                    Filter = new FilterModel<SportType>(null, null, CurrentLanguage, FilterSettings.Default, filter)
                }
            };
            return new ActionResultCached(
                true,
                () => DateTime.MinValue,
                () => {
                    itemData.Each(d => d.CompetitionItems.Each(ci => {
                        ci.DateUtc = FixSystemTimeToUser(ci.DateUtc);
                        ci.Roi.Each(r => {
                            r.Odds.Each(o => {
                                o.Value.DateTimeUtc = FixSystemTimeToUser(o.Value.DateTimeUtc);
                            });
                        });
                    }));
                    return View(model);
                });
        }
    }
}