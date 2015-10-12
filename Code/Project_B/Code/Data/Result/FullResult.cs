using System.Collections.Generic;

namespace Project_B.Code.Data.Result {
    public class FullResult : ResultBase {
        public List<SimpleResult> SubResult { get; set; }

        public FullResult() {
            SubResult = new List<SimpleResult>();
        }
    }
}