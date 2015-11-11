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
        private Dictionary<LanguageType, Dictionary<SportType, string>> _sportTypeName = new Dictionary<LanguageType, Dictionary<SportType, string>>();

        public SportTypeHelper() {
            UpdateSportMap();
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(UpdateSportMap, new TimeSpan(0, 30, 0), null));
        }

        private object UpdateSportMap() {
            var sportNames = SportName.DataSource.AsList();
            var nameToSportType = new Dictionary<string, SportType>();
            var sportTypeName = new Dictionary<LanguageType, Dictionary<SportType, string>>();
            foreach (var sportName in sportNames) {
                nameToSportType[sportName.Name.ToLower()] = sportName.SportType;
                Dictionary<SportType, string> mapByLang;
                if (!sportTypeName.TryGetValue(sportName.Languagetype, out mapByLang)) {
                    mapByLang = new Dictionary<SportType, string>();
                    sportTypeName[sportName.Languagetype] = mapByLang;
                }
                mapByLang[sportName.SportType] = sportName.Name;
            }
            _nameToSportType = nameToSportType;
            _sportTypeName = sportTypeName;
            return null;
        }

        public string GetSportNameForLanguage(LanguageType languageType, SportType sportType) {
            Dictionary<SportType, string> mapByLang;
            if (!_sportTypeName.TryGetValue(languageType, out mapByLang) &&
                !_sportTypeName.TryGetValue(LanguageType.English, out mapByLang)) {
                return sportType.ToString();
            }
            string name;
            return mapByLang.TryGetValue(sportType, out name) ? name : sportType.ToString();
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