using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class RawCompetitionResultHelper {
        public static void TrySaveOrUpdateResult(int rawCompetitionItemID, FullResult fullResult) {
            if (rawCompetitionItemID == default(int)) {
                return;
            }
            var competitionResultRaw = RawCompetitionResult.DataSource.GetByKey(rawCompetitionItemID);
            if (competitionResultRaw != null) {
                return;
            }
            competitionResultRaw = new RawCompetitionResult {
                RawcompetitionitemID = rawCompetitionItemID,
                Rawresult = fullResult
            };
            competitionResultRaw.Insert();
        }
    }
}