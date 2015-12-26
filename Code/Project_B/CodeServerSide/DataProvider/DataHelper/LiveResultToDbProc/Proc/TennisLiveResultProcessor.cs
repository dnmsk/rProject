using System;
using System.Collections.Generic;
using System.Linq;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Entity.BrokerEntity;

namespace Project_B.CodeServerSide.DataProvider.DataHelper.LiveResultToDbProc.Proc {
    public class TennisLiveResultProcessor : ILiveResultProc {
        public void Process(List<CompetitionResultLive> lastResultList, FullResult result) {
            var totalSubResults = result.SubResult.Count;
            if (result.SubResult == null || totalSubResults <= 1) {
                return;
            }
            var setSubResult = result.SubResult[totalSubResults - 2];
            var generateScoreID = ScoreHelper.Instance.GenerateScoreID(setSubResult.CompetitorResultOne, setSubResult.CompetitorResultTwo);
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
                    Advancedparam = GetServeBit(setSubResult.Serve)
                };
            }
            var gameSubResult = result.SubResult[totalSubResults - 1];
            var internalScore1 = GetInternalScore(gameSubResult.CompetitorResultOne);
            var internalScore2 = GetInternalScore(gameSubResult.CompetitorResultTwo);

            lastAdvancedResult.Advancedparam = GetMinimalAdvanceParamForScore(lastAdvancedResult.Advancedparam, internalScore1, internalScore2);
            lastAdvancedResult.Save();
        }

        public static short GetMinimalAdvanceParamForScore(short currentAdvanceParam, short score1, short score2) {
            var lastScore2Position = -1;
            var score1Count = 0;
            var score2Count = 0;
            for (var i = 0; i < 15; i++) {
                var currentBit = (1 << i);
                if ((currentAdvanceParam & currentBit) > 0) {
                    score2Count++;
                    lastScore2Position = i;
                } else {
                    score1Count++;
                }
                if (Math.Abs(score1Count - score2Count) > 3) {
                    break;
                }
            }
            if (lastScore2Position >= 0) {
                score1Count = (lastScore2Position + 1) - score2Count;
                if (score1Count - score2Count > score1 - score2 && lastScore2Position < 14) {
                    return (short) (currentAdvanceParam | (1 << (lastScore2Position + 1)));
                }
                return currentAdvanceParam;
            }
            score1Count = score1;
            lastScore2Position = score1;
            if (score1Count - score2Count > score1 - score2 && lastScore2Position < 15) {
                return (short) (currentAdvanceParam | (1 << lastScore2Position));
            }
            return currentAdvanceParam;
        }
        
        private static readonly Dictionary<short, short> _scoreRealToDb = new Dictionary<short, short> {
            { 0, 0 },
            { 15, 1 },
            { 30, 2 },
            { 40, 3 },
            { 45, 4 }
        }; 

        private static short GetInternalScore(short score) {
            short s;
            return _scoreRealToDb.TryGetValue(score, out s) ? s : score;
        }

        public static short GetServeBit(Serve serve) {
            switch (serve) {
                case Serve.Serve1Player:
                    return default(short);
                case Serve.Serve2Player:
                    var number = (1 << 15);
                    return (short)number;
                default:
                    return default(short);
            }
        }
    }
}