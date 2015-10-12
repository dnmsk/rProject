using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using MainLogic;
using Project_B.Code.Entity;
using Project_B.Code.Enums;

namespace Project_B.Code.DataProvider.DataHelper {
    public class GenderDetectorHelper : Singleton<GenderDetectorHelper> {
        private Dictionary<string, GenderType> _nameToGender = new Dictionary<string, GenderType>();

        public GenderDetectorHelper() {
            UpdateGenderMap();
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(UpdateGenderMap, new TimeSpan(0, 30, 0), null));
        }

        private object UpdateGenderMap() {
            var genders = GenderAdvanced.DataSource.AsList();
            var newMap = new Dictionary<string, GenderType>();
            foreach (var genderAdvanced in genders) {
                newMap[genderAdvanced.Name.ToLower()] = genderAdvanced.Gendertype;
            }
            _nameToGender = newMap;
            return null;
        }

        public GenderType this[IEnumerable<string> strToDetect] {
            get {
                GenderType genderType = GenderType.Unknown;
                return strToDetect.Any(s => _nameToGender.TryGetValue(s.ToLower(), out genderType)) && genderType != GenderType.Unknown ? genderType : GenderType.Default;
            }
        }
    }
}