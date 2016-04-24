using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using Project_B.CodeClientSide.TransportType;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity.BrokerEntity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider {
    public class ResultProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (ResultProvider).FullName);

        public ResultProvider() : base(_logger) {}

        public void SaveResults(ProcessStat processStat, CompetitionItemRawTransport competitionRawTransport, SportType sportType, FullResult fullResult) {
            InvokeSafeSingleCall(() => {
                processStat.TotalCount++;
                if (competitionRawTransport == null || fullResult == null) {
                    return null;
                }
                RawCompetitionResultHelper.TrySaveOrUpdateResult(processStat, competitionRawTransport.RawCompetitionItemID, fullResult);
                if (competitionRawTransport.CompetitionItemID == default(int)) {
                    return null;
                }
                CompetitionResult.ProcessResult(processStat, competitionRawTransport.CompetitionItemID, sportType, fullResult);
                processStat.FinallySuccessCount++;
                return (object) null;
            }, null);
        }

        public Dictionary<int, ResultTransport> GetResultForCompetitions(int[] competitionIDs) {
            return InvokeSafe(() => {
                var results = CompetitionResult.DataSource
                    .WhereIn(CompetitionResult.Fields.CompetitionitemID, competitionIDs)
                    .AsList(
                        CompetitionResult.Fields.CompetitionitemID,
                        CompetitionResult.Fields.ScoreID
                    );
                var mapSubresult = CompetitionResultAdvanced.DataSource
                    .WhereIn(CompetitionResultAdvanced.Fields.CompetitionresultID, results.Select(r => r.ID))
                    .Sort(CompetitionResultAdvanced.Fields.ID, SortDirection.Asc)
                    .AsMapByField<int>(CompetitionResultAdvanced.Fields.CompetitionresultID,
                        CompetitionResultAdvanced.Fields.ScoreID
                    );
                var result = new Dictionary<int, ResultTransport>();
                foreach (var r in results) {
                    var competitionID = r.CompetitionitemID;
                    var subresult = mapSubresult.TryGetValueOrDefault(r.ID) ?? new List<CompetitionResultAdvanced>();
                    result[competitionID] = new ResultTransport {
                        CompetitionID = competitionID,
                        ScoreID = r.ScoreID,
                        SubScore = subresult.Select(s => s.ScoreID).ToArray()
                    };
                }
                return result;
            }, null);
        } 

        public Dictionary<int, ResultTransport> GetResultLiveForCompetitions(int[] competitionIDs) {
            return InvokeSafe(() => {
                var results = CompetitionResultLive.DataSource
                    .WhereIn(CompetitionResultLive.Fields.CompetitionitemID, competitionIDs)
                    .Sort(CompetitionResultLive.Fields.ID, SortDirection.Desc)
                    .AsList(
                        CompetitionResultLive.Fields.CompetitionitemID,
                        CompetitionResultLive.Fields.ScoreID
                    );
                var mapSubresult = CompetitionResultLiveAdvanced.DataSource
                    .WhereIn(CompetitionResultLiveAdvanced.Fields.CompetitionresultliveID, results.Select(r => r.ID))
                    .Sort(CompetitionResultLiveAdvanced.Fields.ID, SortDirection.Asc)
                    .AsMapByField<long>(CompetitionResultLiveAdvanced.Fields.CompetitionresultliveID,
                        CompetitionResultLiveAdvanced.Fields.ScoreID
                    );
                var result = new Dictionary<int, ResultTransport>();
                foreach (var r in results) {
                    var competitionID = r.CompetitionitemID;
                    var subresult = mapSubresult.TryGetValueOrDefault(r.ID) ?? new List<CompetitionResultLiveAdvanced>();
                    if (result.ContainsKey(competitionID)) {
                        continue;
                    }
                    result[competitionID] = new ResultTransport {
                        CompetitionID = competitionID,
                        ScoreID = r.ScoreID,
                        SubScore = new [] { subresult.Select(s => s.ScoreID).LastOrDefault() }
                    };
                }
                return result;
            }, null);
        } 
    }
}