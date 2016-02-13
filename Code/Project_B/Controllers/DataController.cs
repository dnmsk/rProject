using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using MainLogic.WebFiles;
using Project_B.CodeClientSide;
using Project_B.CodeClientSide.Helper;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.Enums;

namespace Project_B.Controllers {
    public class DataController : ProjectControllerBase {
        [HttpPost]
        [ActionProfile(ProjectBActions.PageDataCompetitionItemGraph)]
        public ActionResult CompetitionItemGraph(int id, SportType sportType) {
            var data = FrontCompetitionProvider.GetRowDataForGraphCompetition(null, sportType, id);
            return BuildActionResult(sportType, data);
        }

        [HttpPost]
        [ActionProfile(ProjectBActions.PageDataCompetitionItemLiveGraph)]
        public ActionResult CompetitionItemLiveGraph(int id, SportType sportType) {
            var data = FrontCompetitionProvider.GetRowDataForGraphCompetitionLive(null, sportType, id);
            return BuildActionResult(sportType, data);
        }

        private ActionResult BuildActionResult(SportType sportType, Dictionary<DateTime, List<Dictionary<BetOddType, BetItemTransport>>> data) {
            return new ActionResultCached(data != null,
                () => data.MaxOrDefault(d => d.Key, DateTime.MinValue),
                () => {
                    var res = new List<Dictionary<string, float>>();
                    var addDraw = BetHelper.SportTypeWithOdds[sportType].Contains(BetOddType.Draw);
                    data
                        .OrderBy(d => d.Key)
                        .Each(betItems => {
                            var list = betItems.Value;
                            var dict = new Dictionary<string, float> {
                                {"d", (float) (betItems.Key - ProjectBConsts.DefaultLinuxUtc).TotalMilliseconds},
                                {"w1", BetOddInterfaceHelper.GetOddValue(sportType, BetOddType.Win1, list, Enumerable.Max)},
                                {"w2", BetOddInterfaceHelper.GetOddValue(sportType, BetOddType.Win2, list, Enumerable.Max)},
                                {"r1x2", BetOddInterfaceHelper.GetBetOddRoi(RoiType.Roi1X2, sportType, list)},
                                {"h1", BetOddInterfaceHelper.GetOddValue(sportType, BetOddType.Handicap1, list, Enumerable.Max)},
                                {"h2", BetOddInterfaceHelper.GetOddValue(sportType, BetOddType.Handicap2, list, Enumerable.Max)},
                                {"rh", BetOddInterfaceHelper.GetBetOddRoi(RoiType.RoiHandicap, sportType, list)},
                                {"tu", BetOddInterfaceHelper.GetOddValue(sportType, BetOddType.TotalUnder, list, Enumerable.Max)},
                                {"to", BetOddInterfaceHelper.GetOddValue(sportType, BetOddType.TotalOver, list, Enumerable.Max)},
                                {"rt", BetOddInterfaceHelper.GetBetOddRoi(RoiType.RoiTotal, sportType, list)},
                            };
                            if (addDraw) {
                                dict["x"] = BetOddInterfaceHelper.GetOddValue(sportType, BetOddType.Draw, list, Enumerable.Max);
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