using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.KangooCache;
using MainLogic;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public class SportTypeHelper : Singleton<SportTypeHelper> {

        private readonly MultipleKangooCache<string, SportType> _nameToSportType;
        private Dictionary<LanguageType, Dictionary<SportType, string>> _sportTypeName = new Dictionary<LanguageType, Dictionary<SportType, string>>();

        public SportTypeHelper() {
            _nameToSportType = new MultipleKangooCache<string, SportType>(MainLogicProvider.WatchfulSloth,
                dictionary => {
                    var sportTypeName = new Dictionary<LanguageType, Dictionary<SportType, string>>();
                    foreach (var sportName in SportName.DataSource.AsList()) {
                        dictionary[sportName.Name.ToLower()] = sportName.SportType;
                        Dictionary<SportType, string> mapByLang;
                        if (!sportTypeName.TryGetValue(sportName.Languagetype, out mapByLang)) {
                            mapByLang = new Dictionary<SportType, string>();
                            sportTypeName[sportName.Languagetype] = mapByLang;
                        }
                        mapByLang[sportName.SportType] = sportName.Name;
                    }
                    _sportTypeName = sportTypeName;
            }, TimeSpan.FromMinutes(120));
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