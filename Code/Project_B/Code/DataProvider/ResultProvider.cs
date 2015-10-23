using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.MassTools;
using Project_B.Code.Data.Result;
using Project_B.Code.DataProvider.DataHelper;
using Project_B.Code.Entity;
using Project_B.Code.Enums;
using Project_B.Models;

namespace Project_B.Code.DataProvider {
    public class ResultProvider : SafeInvokerBase {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (ResultProvider).FullName);

        public ResultProvider() : base(_logger) {}

        public void SaveResults(int competitionItemID, SportType sportType, FullResult fullResult) {
            InvokeSafeSingleCall(() => {
                if (!CompetitionResult.DataSource
                            .WhereEquals(CompetitionResult.Fields.CompetitionitemID, competitionItemID)
                            .IsExists()) {
                    var competitionResult = new CompetitionResult {
                        CompetitionitemID = competitionItemID,
                        Datecreatedutc = DateTime.UtcNow,
                        ScoreID = ScoreHelper.Instance.GenerateScoreID(fullResult.CompetitorResultOne, fullResult.CompetitorResultTwo),
                        Resulttype = ScoreHelper.Instance.GetResultType(fullResult.CompetitorResultOne, fullResult.CompetitorResultTwo)
                    };
                    competitionResult.Save();
                    if (fullResult.SubResult != null && fullResult.SubResult.Count > 0) {
                        var listSubResult = new List<CompetitionResultAdvanced>();
                        for (var subResultIndex = 0; subResultIndex < fullResult.SubResult.Count; subResultIndex++) {
                            var subResult = fullResult.SubResult[subResultIndex];
                            listSubResult.Add(new CompetitionResultAdvanced {
                                CompetitionitemID = competitionItemID,
                                Resulttype = competitionResult.Resulttype,
                                CompetitionresultID = competitionResult.ID,
                                ScoreID = ScoreHelper.Instance.GenerateScoreID(subResult.CompetitorResultOne, subResult.CompetitorResultTwo),
                                Resultmodetype = ScoreHelper.Instance.GetResultModeType(sportType, subResultIndex, subResult.ModeTypeString)
                            });
                        }
                        listSubResult.Save<CompetitionResultAdvanced, int>();
                    }
                }
                return (object) null;
            }, null);
        }

        public Dictionary<int, ResultModel> GetResultForCompetitions(int[] competitionIDs) {
            return InvokeSafe(() => {
                var results = CompetitionResult.DataSource
                    .WhereIn(CompetitionResult.Fields.CompetitionitemID, competitionIDs)
                    .AsList(
                        CompetitionResult.Fields.CompetitionitemID,
                        CompetitionResult.Fields.ScoreID
                    );
                var mapSubresult = CompetitionResultAdvanced.DataSource
                    .WhereIn(CompetitionResultAdvanced.Fields.CompetitionresultID, results.Select(r => r.ID))
                    .AsMapByField<int>(CompetitionResultAdvanced.Fields.CompetitionresultID,
                        CompetitionResultAdvanced.Fields.ScoreID
                    );
                var result = new Dictionary<int, ResultModel>();
                foreach (var r in results) {
                    var competitionID = r.CompetitionitemID;
                    var subresult = mapSubresult.TryGetValueOrDefault(r.ID) ?? new List<CompetitionResultAdvanced>();
                    result[competitionID] = new ResultModel {
                        CompetitionID = competitionID,
                        ScoreID = r.ScoreID,
                        SubScore = subresult.Select(s => s.ScoreID).ToArray()
                    };
                }
                return result;
            }, null);
        } 

        public Dictionary<int, ResultModel> GetResultLiveForCompetitions(int[] competitionIDs) {
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
                    .AsMapByField<long>(CompetitionResultLiveAdvanced.Fields.CompetitionresultliveID,
                        CompetitionResultLiveAdvanced.Fields.ScoreID
                    );
                var result = new Dictionary<int, ResultModel>();
                foreach (var r in results) {
                    var competitionID = r.CompetitionitemID;
                    var subresult = mapSubresult.TryGetValueOrDefault(r.ID) ?? new List<CompetitionResultLiveAdvanced>();
                    if (result.ContainsKey(competitionID)) {
                        continue;
                    }
                    result[competitionID] = new ResultModel {
                        CompetitionID = competitionID,
                        ScoreID = r.ScoreID,
                        SubScore = subresult.Select(s => s.ScoreID).ToArray()
                    };
                }
                return result;
            }, null);
        } 
    }
}