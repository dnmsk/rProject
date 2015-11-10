using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class QueryHelper {

        public static DaoFilterBase GetFilterByWordsForField(IEnumerable<string> words, Enum field) {
            var filters = new List<DaoFilterBase>();
            words.Each(w => filters.Add(new DaoFilter(field, Oper.MatchInsensitive, w)));

            var allFilters = filters.Count > 1
                                           ? new DaoFilterAnd(filters)
                                           : filters[0];

            return allFilters;
        }

        public static DaoFilterBase GetIndexedFilterByWordIgnoreCase(string word, Enum field, bool fullyEq = true) {
            return new DaoFilter(new DbFnSimpleFieldOp("lower", field), Oper.Like, string.Format(fullyEq ? "{0}" : "%{0}%", word.ToLower()));
        }

        public static string[] StringToArray(string str) {
            return str.Split('.').Select(s => s.Trim()).ToArray();
        }
    }
}