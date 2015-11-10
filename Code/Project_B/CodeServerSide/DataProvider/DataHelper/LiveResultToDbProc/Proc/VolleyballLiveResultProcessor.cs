using System;
using System.Linq;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Entity;

namespace Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc.Proc {
    public class VolleyballLiveResultProcessor : ILiveResultProc {
        public void Process(CompetitionResultLive lastResult, CompetitionResultLiveAdvanced lastAdvancedResult, FullResult result) {
            if (result.SubResult == null || result.SubResult.Count == 0) {
                return;
            }
            var lastSubResult = result.SubResult.Last();
            var generateScoreID = ScoreHelper.Instance.GenerateScoreID(lastSubResult.CompetitorResultOne, lastSubResult.CompetitorResultTwo);
            if (lastAdvancedResult == null || lastAdvancedResult.ScoreID != generateScoreID) {
                lastAdvancedResult = new CompetitionResultLiveAdvanced {
                    ScoreID = generateScoreID,
                    CompetitionresultliveID = lastResult.ID,
                    Datecreatedutc = DateTime.UtcNow,
                    Advancedparam = TennisLiveResultProcessor.GetServeBit(lastSubResult.Serve)
                };
                lastAdvancedResult.Save();
            }
        }
    }
}