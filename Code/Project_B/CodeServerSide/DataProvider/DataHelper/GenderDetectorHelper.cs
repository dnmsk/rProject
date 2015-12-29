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
        private readonly MultipleKangooCache<string, GenderType> _nameToGender = new MultipleKangooCache<string, GenderType>(MainLogicProvider.WatchfulSloth,
            dictionary => {
                foreach (var genderAdvanced in GenderAdvanced.DataSource.AsList()) {
                    dictionary[genderAdvanced.Name.ToLower()] = genderAdvanced.Gendertype;
                }
            }, TimeSpan.FromMinutes(120)); 
        
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
    }
}