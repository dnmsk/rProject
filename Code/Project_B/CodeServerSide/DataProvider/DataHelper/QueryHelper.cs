using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Filters;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class QueryHelper {
        /*
        public static DaoFilterBase GetFilterByWordsForField(IEnumerable<string> words, Enum field) {
            var filters = new List<DaoFilterBase>();
            words.Each(w => filters.Add(new DaoFilter(field, Oper.MatchInsensitive, w)));

            var allFilters = filters.Count > 1
                                           ? new DaoFilterAnd(filters)
                                           : filters[0];

            return allFilters;
        }
        */
        public static DaoFilterBase GetIndexedFilterByWordIgnoreCase(string[] words, Enum field, bool fullyEq = true) {
            var filters = new List<DaoFilterBase>();
            words.Each(word => filters.Add(GetIndexedFilterByWordIgnoreCase(word, field, fullyEq)));

            var allFilters = filters.Count > 1
                ? new DaoFilterOr(filters)
                : filters[0];
            return allFilters;
        }

        public static DaoFilterBase GetIndexedFilterByWordIgnoreCase(string word, Enum field, bool fullyEq = true) {
            return new DaoFilter(field, Oper.Like, string.Format(fullyEq ? "{0}" : "%{0}%", word));
        }
        
        private static GenderType GetNearGenderType(GenderType genderType) {
            switch (genderType) {
                case GenderType.Female:
                case GenderType.Male:
                    return GenderType.Default;
                case GenderType.Default:
                default:
                    return GenderType.Male;
            }
        }

        public static List<T> FilterByGender<T, K>(DbDataSource<T, K> ds, Enum field, GenderType genderType, params Enum[] filedsToRetrive) where T : class, IKeyedAbstractEntity<K> where K : struct, IComparable<K> {
            var result = ds.WhereEquals(field, (short) genderType).AsList(filedsToRetrive);
            return result.Any() 
                ? result 
                : ds.WhereEquals(field, (short)GetNearGenderType(genderType)).AsList(filedsToRetrive);
        }

        public static string[] StringToArray(string str) {
            return str.Split('.').Select(s => s.Trim()).ToArray();
        }
    }
}