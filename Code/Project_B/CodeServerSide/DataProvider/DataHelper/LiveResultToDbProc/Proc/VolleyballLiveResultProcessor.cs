using System;
using System.Collections.Generic;
using System.Linq;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Entity.BrokerEntity;

namespace Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc.Proc {
    public class VolleyballLiveResultProcessor : ILiveResultProc {
        public void Process(List<CompetitionResultLive> lastResultList, FullResult result) {
            if (result.SubResult == null || result.SubResult.Count == 0) {
                return;
            }
            var lastSubResult = result.SubResult.Last();
            var generateScoreID = ScoreHelper.Instance.GenerateScoreID(lastSubResult.CompetitorResultOne, lastSubResult.CompetitorResultTwo);

            var lastResult = lastResultList.FirstOrDefault(lr => {
                var lra = lr.GetJoinedEntity<CompetitionResultLiveAdvanced>();
                return lra != null && lra.ScoreID == generateScoreID;
            });
            var lastAdvancedResult = lastResult?.GetJoinedEntity<CompetitionResultLiveAdvanced>();
            if (lastAdvancedResult == null) {
                lastAdvancedResult = new CompetitionResultLiveAdvanced {
                    ScoreID = generateScoreID,
                    CompetitionresultliveID = lastResultList.First().ID,
                    Datecreatedutc = DateTime.UtcNow,
                    Advancedparam = TennisLiveResultProcessor.GetServeBit(lastSubResult.Serve)
                };
                lastAdvancedResult.Save();
            }
        }
    }
}