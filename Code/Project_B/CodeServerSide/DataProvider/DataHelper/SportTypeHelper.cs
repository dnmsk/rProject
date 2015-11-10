using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public class SportTypeHelper : Singleton<SportTypeHelper> {
        private Dictionary<string, SportType> _nameToSportType = new Dictionary<string, SportType>();

        public SportTypeHelper() {
            UpdateSportMap();
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(UpdateSportMap, new TimeSpan(0, 30, 0), null));
        }

        private object UpdateSportMap() {
            var sportNames = SportName.DataSource.AsList();
            var newMap = new Dictionary<string, SportType>();
            foreach (var sportName in sportNames) {
                newMap[sportName.Name.ToLower()] = sportName.SportType;
            }
            _nameToSportType = newMap;
            return null;
        }

        public List<string> ExcludeSportTypeFromList(List<string> strings) {
            return strings
                .Where(s => !_nameToSportType.ContainsKey(s.ToLower()))
                .ToList();
        } 

        public SportType this[IEnumerable<string> strToDetect] {
            get {
                var sportType = SportType.Unknown;
                return strToDetect.Any(s => _nameToSportType.TryGetValue(s.ToLower().Trim(), out sportType)) ? sportType : SportType.Unknown;
            }
        }
    }
}