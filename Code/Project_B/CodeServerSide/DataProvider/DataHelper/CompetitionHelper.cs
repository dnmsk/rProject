using System;
using System.Collections.Generic;
using System.Linq;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    internal static class CompetitionHelper {
        private static readonly List<string> _stopListWithInclude = new List<string> {
            "cup",
            "кубок",
        }; 
        private static readonly List<string> _stopList = new List<string> {
            "pool ",
            "группа ",

            "singles",
            "doubles",
            "разряд",

            "stage",
            "этап",

            "round",
            "финал",

            "play-off",
            "плей-офф",
            "плэй-оф",

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