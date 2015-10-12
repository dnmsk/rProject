using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.Code.Entity;
using Project_B.Code.Enums;

namespace Project_B.Code.DataProvider.DataHelper {
    public class SportTypeHelper : Singleton<SportTypeHelper> {
        private Dictionary<string, SportType> _nameToGender = new Dictionary<string, SportType>();

        public SportTypeHelper() {
            UpdateSportMap();
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(UpdateSportMap, new TimeSpan(0, 30, 0), null));
        }

        private object UpdateSportMap() {
            var genders = SportName.DataSource.AsList();
            var newMap = new Dictionary<string, SportType>();
            foreach (var genderAdvanced in genders) {
                newMap[genderAdvanced.Name.ToLower()] = genderAdvanced.SportType;
            }
            _nameToGender = newMap;
            return null;
        }

        public List<string> ExcludeSportTypeFromList(List<string> strings) {
            return strings
                .Where(s => !_nameToGender.ContainsKey(s.ToLower()))
                .ToList();
        } 

        public SportType this[IEnumerable<string> strToDetect] {
            get {
                var genderType = SportType.Unknown;
                return strToDetect.Any(s => _nameToGender.TryGetValue(s.ToLower(), out genderType)) ? genderType : genderType;
            }
        }
    }
}