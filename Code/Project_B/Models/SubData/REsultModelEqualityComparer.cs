using System.Collections.Generic;
using CommonUtils.ExtendedTypes;

namespace Project_B.Models.SubData {
    public class ResultModelEqualityComparer : IEqualityComparer<ResultModel> {
        public bool Equals(ResultModel x, ResultModel y) {
            if (x.ScoreID != y.ScoreID) {
                return false;
            }
            if (x.SubScore == null || y.SubScore == null) {
                return x.SubScore == null && y.SubScore == null;
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

        public int GetHashCode(ResultModel obj) {
            return (obj.ScoreID + (obj.SubScore == null ? string.Empty : obj.SubScore.StrJoin(string.Empty))).GetHashCode();
        }
    }
}