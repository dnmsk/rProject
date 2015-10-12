using System;
using System.Collections.Generic;
using System.Linq;

namespace Project_B.Code.DataProvider.DataHelper {
    public class CompetitionHelper {
        private static readonly List<string> _stopListWithInclude = new List<string> {
            "cup",
        }; 
        private static readonly List<string> _stopList = new List<string> {
            "pool ",
            "singles",
            "doubles",
            "stage",
            "round",
            "play-off",
            "play-out"
        };

        public static List<string> GetShortCompetitionName(List<string> names) {
            var result = new List<string>();
            foreach (var name in names) {
                if (_stopListWithInclude.Any(slw => name.IndexOf(slw, StringComparison.InvariantCultureIgnoreCase) >= 0)) {
                    result.Add(name);
                    break;
                }

                if (_stopList.Any(sl => name.IndexOf(sl, StringComparison.InvariantCultureIgnoreCase) >= 0)) {
                    break;
                }
                result.Add(name);
            }
            return result;
        }
    }
}