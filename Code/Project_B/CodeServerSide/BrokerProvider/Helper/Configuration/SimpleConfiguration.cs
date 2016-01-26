using System;
using System.Collections.Generic;
using CommonUtils.ExtendedTypes;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.Configuration {
    public class SimpleConfiguration<K, V> : Dictionary<K, V> {
        public new V this[K key] {
            get {
                V val;
                return TryGetValue(key, out val) ? val : default (V);
            }
            set { base[key] = value; }
        }

        public string GetParamValueForCompetition<T>(T sportType, string strJoinDelim) where T : IComparable, IConvertible {
            var listParams = new List<V>();
            var intValue = Convert.ToInt32(sportType);
            foreach (var competition in this) {
                if ((intValue & Convert.ToInt32((Enum.Parse(typeof(T), competition.Key.ToString())))) > 0) {
                    listParams.Add(competition.Value);
                }
            }
            return listParams.Count > 0 ? listParams.StrJoin(strJoinDelim) : string.Empty;
        }
    }
}