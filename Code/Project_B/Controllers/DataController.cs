using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Helper;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.BrokerProvider;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Controllers {
    public class DataController : ProjectControllerBase {
        [HttpPost]
        [ActionProfile(ProjectBActions.PageDataCompetitionItemGraph)]
        public ActionResult CompetitionItemGraph(int id, SportType sportType) {
            var data = ProjectProvider.Instance.CompetitionProvider.GetRowDataForGraphCompetition(null, sportType, id);
            return BuildActionResult(sportType, data);
        }

        [HttpPost]
        [ActionProfile(ProjectBActions.PageDataCompetitionItemLiveGraph)]
        public ActionResult CompetitionItemLiveGraph(int id, SportType sportType) {
            var data = ProjectProvider.Instance.CompetitionProvider.GetRowDataForGraphCompetitionLive(null, sportType, id);
            return BuildActionResult(sportType, data);
        }

        private static ActionResult BuildActionResult(SportType sportType,
            Dictionary<DateTime, List<Dictionary<BetOddType, BetItemTransport>>> data) {
            return new ActionResultCached(data != null && data.Any(),
                () => data.Max(d => d.Key),
                () => {
                    var res = new List<Dictionary<string, float>>();
                    var addDraw = BetHelper.SportTypeWithOdds[sportType].Contains(BetOddType.Draw);
                    data
                        .OrderBy(d => d.Key)
                        .Each(betItems => {
                            var list = betItems.Value;
                            var dict = new Dictionary<string, float> {
                                {"d", (float) (betItems.Key - BrokerBase.LinuxUtc).TotalMilliseconds},
                                {"w1", BetOddInterfaceHelper.GetAverageOddValue(sportType, BetOddType.Win1, list)},
                                {"w2", BetOddInterfaceHelper.GetAverageOddValue(sportType, BetOddType.Win2, list)},
                                {"r1x2", BetOddInterfaceHelper.GetBetOddRoi(RoiType.Roi1X2, sportType, list)},
                                {"h1", BetOddInterfaceHelper.GetAverageOddValue(sportType, BetOddType.Handicap1, list)},
                                {"h2", BetOddInterfaceHelper.GetAverageOddValue(sportType, BetOddType.Handicap2, list)},
                                {"rh", BetOddInterfaceHelper.GetBetOddRoi(RoiType.RoiHandicap, sportType, list)},
                                {"tu", BetOddInterfaceHelper.GetAverageOddValue(sportType, BetOddType.TotalUnder, list)},
                                {"to", BetOddInterfaceHelper.GetAverageOddValue(sportType, BetOddType.TotalOver, list)},
                                {"rt", BetOddInterfaceHelper.GetBetOddRoi(RoiType.RoiTotal, sportType, list)},
                            };
                            if (addDraw) {
                                dict["x"] = BetOddInterfaceHelper.GetAverageOddValue(sportType, BetOddType.Draw, list);
                            }
                            res.Add(dict);
                        });
                    return new JsonResult {
                        Data = res
                    };
                });
        }
    }
}