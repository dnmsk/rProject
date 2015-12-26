using System.Collections.Generic;
using System.Linq;
using CommonUtils.ExtendedTypes;

namespace Project_B.CodeClientSide.TransportType.SubData {
    public class ResultTransportEqualityComparer : IEqualityComparer<ResultTransport> {
        public bool Equals(ResultTransport x, ResultTransport y) {
            if (x.ScoreID != y.ScoreID) {
                return false;
            }
            if (x.SubScore == null || y.SubScore == null) {
                return x.SubScore.SafeAny() == y.SubScore.SafeAny();
            }
            if (x.SubScore.Length != y.SubScore.Length) {
                return false;
            }
            for (int i = 0; i < x.SubScore.Length; i++) {
                if (x.SubScore[i] != y.SubScore[i]) {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(ResultTransport obj) {
            return (obj.ScoreID + (obj.SubScore == null ? string.Empty : obj.SubScore.StrJoin(string.Empty))).GetHashCode();
        }
    }
}