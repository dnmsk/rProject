using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.KangooCache;
using MainLogic;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public class GenderDetectorHelper : Singleton<GenderDetectorHelper> {
        private Dictionary<GenderType, string> _genderName = new Dictionary<GenderType, string>(); 
        private readonly MultipleKangooCache<string, GenderType> _nameToGender;

        public GenderDetectorHelper() {
            _nameToGender = new MultipleKangooCache<string, GenderType>(MainLogicProvider.WatchfulSloth,
            dictionary => {
                var newGenderName = new Dictionary<GenderType, string>();
                foreach (var genderAdvanced in GenderAdvanced.DataSource.AsList()) {
                    dictionary[genderAdvanced.Name.ToLower()] = genderAdvanced.Gendertype;
                    newGenderName[genderAdvanced.Gendertype] = genderAdvanced.Name;
                }
                _genderName = newGenderName;
            }, TimeSpan.FromMinutes(120));
        }

        public GenderType this[IEnumerable<string> strToDetect] {
            get {
                var genderType = GenderType.Unknown;
                return strToDetect.Any(s => _nameToGender.TryGetValue(s.ToLower(), out genderType)) && genderType != GenderType.Unknown ? genderType : GenderType.Default;
            }
        }

        public List<string> ExcludGenderTypeFromList(List<string> strings) {
            return strings
                .Where(s => !_nameToGender.ContainsKey(s.ToLower()))
                .ToList();
        }

        public string GetGenderName(GenderType genderType) {
            string res;
            return _genderName.TryGetValue(genderType, out res) ? res : null;
        }
    }
}