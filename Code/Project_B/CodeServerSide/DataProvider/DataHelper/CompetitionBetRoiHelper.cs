using System;
using System.Data;
using System.Linq;
using CommonUtils;
using IDEV.Hydra.DAO;
using Project_B.CodeServerSide.Entity.Data;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.DataProvider.DataHelper {
    public static class CompetitionBetRoiHelper {
        public static CompetitionItemRoiRow[] GetDataForNow(float minRoi = float.MinValue, SportType sportType = SportType.Unknown, int[] competitionUniqueIDs = null, BrokerType[] brokerTypes = null) {
            var proc = new DbStoredProc()
                .NotRepeatable();
            if (minRoi != float.MinValue) {
                proc = proc
                    .WithParam("argroimin", minRoi);
            }
            return BuildResult(proc, sportType, competitionUniqueIDs, brokerTypes);
        }

        public static CompetitionItemRoiRow[] GetDataForDate(DateTime fromDate, DateTime toDate, SportType sportType = SportType.Unknown, int[] competitionUniqueIDs = null, BrokerType[] brokerTypes = null) {
            var proc = new DbStoredProc()
                .NotRepeatable()
                .WithParam("argcompetitionmindate", fromDate)
                .WithParam("argcompetitionmaxdate", toDate);
            return BuildResult(proc, sportType, competitionUniqueIDs, brokerTypes);
        }

        public static CompetitionItemRoiRow[] GetDataForCompetitionItem(int competitionItemID, BrokerType[] brokerTypes = null, int limitRows = 1) {
            var proc = new DbStoredProc()
                .NotRepeatable()
                .WithParam("argcompetitionitemid", competitionItemID)
                .WithParam("arglimittocompetition", limitRows);
            return BuildResult(proc, SportType.Unknown, null, brokerTypes);
        }

        private static CompetitionItemRoiRow[] BuildResult(DbStoredProc proc, SportType sportType, int[] competitionUniqueIDs, BrokerType[] brokerTypes) {
            if (brokerTypes != null) {
                proc = proc.WithParam("argbrokerids", brokerTypes.Select(b => (short)b), DbType.Int16);
            }
            if (competitionUniqueIDs != null) {
                proc = proc.WithParam("argcompetitionuniqueids", competitionUniqueIDs, DbType.Int32);
            }
            if (sportType != SportType.Unknown) {
                proc = proc.WithParam("argsporttype", (short) sportType);
            }
            var data = proc.MultiRowExec(DatabaseActions.GetAdapter(TargetDB.MASTER), "fn_getbetroi").Data;
            var result = new CompetitionItemRoiRow[data.Count];
            for (var i = 0; i < result.Length; i++) {
                var row = data[i];
                result[i] = new CompetitionItemRoiRow {
                    ID = (int)row[0],
                    Roi1X2 = CastSafe(row[1], -100f),
                    RoiHcap = CastSafe(row[2], -100f),
                    RoiTotal = CastSafe(row[3], -100f),
                    Roi1_X2 = CastSafe(row[4], -100f),
                    Roi1X_2 = CastSafe(row[5], -100f),
                    BetIDs = (int[])row[6],
                    RoiMax = CastSafe(row[7], -100f)
                };
            }
            return result;
        }

        private static T CastSafe<T>(object val, T def) {
            if (val is DBNull) {
                return def;
            }
            return (T) val;
        }
    }
}