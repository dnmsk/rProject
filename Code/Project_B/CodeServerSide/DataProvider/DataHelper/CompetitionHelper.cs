using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeClientSide.TransportType.SubData;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class CompetitionHelper {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (CompetitionHelper).FullName);

        private static readonly string[] _stopListWithGetNext = {
            "games",
            "tour",
            "турнир",
            "competition"
        };

        private static readonly string[] _stopListWithIncludeCurrent = {
            "copa",
            "cup",
            " кап",
            "кубок",
            "trophy",
            "трофей",
            "league",
            "liga",
            "лига",
            "singles",
            "doubles",
            "разряд",
            "tournament",
            "чемпионат",
            "champion"
        }; 
        private static readonly string[] _stopListWithExcludeCurrent = {
            "pool ",
            "группа",
            "group",
            
            "stage",
            "этап",
            "матчи",

            "round",
            "final",
            "раунд",
            "финал",

            "playof",
            "play of",
            "play-of",
            "плэй-оф",
            "плэйоф",
            "плэй оф",
            "1/",

            "play-out",
            "play out",
            "плей аут",
            "плэй аут",
            "плей-аут",
            "плэй-аут",

            "bowl",

            "cезон",
            "season",
        };
        
        public static string[] GetShortCompetitionName(string[] names, SportType sportType) {
            var result = new List<string>();
            for (var i = 0; i < names.Length; i++) {
                var name = names[i];
                if (sportType != SportType.Tennis && _stopListWithGetNext.Any(slw => name.IndexOf(slw, StringComparison.InvariantCultureIgnoreCase) >= 0)) {
                    result.Add(name);
                    var nextIdx = i + 1;
                    if (nextIdx < names.Length) {
                        result.Add(names[nextIdx]);
                    }
                    break;
                }

                if (_stopListWithIncludeCurrent.Any(slw => name.IndexOf(slw, StringComparison.InvariantCultureIgnoreCase) >= 0)) {
                    result.Add(name);
                    break;
                }

                if (_stopListWithExcludeCurrent.Any(sl => name.IndexOf(sl, StringComparison.InvariantCultureIgnoreCase) >= 0) && result.Count > 0) {
                    break;
                }
                result.Add(name);
            }
            return result.ToArray();
        }

        public static string ListStringToName(IEnumerable<string> names) {
            return names.StrJoin(". ");
        }
    }
}