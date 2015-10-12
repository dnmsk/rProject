using System;
using System.Collections.Generic;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using IDEV.Hydra.DAO.MassTools;
using Project_B.Code.Data.Result;
using Project_B.Code.DataProvider.DataHelper;
using Project_B.Code.Entity;
using Project_B.Code.Enums;

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
    }
}