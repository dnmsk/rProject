﻿using System;
using System.Collections.Generic;
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
                var betData = (int[])row[1];
                var betIDs = new Dictionary<BetOddType, int>();
                for (var j = 0; j < betData.Length; j+=2) {
                    betIDs[(BetOddType) (short) betData[j]] = betData[j + 1];
                }
                result[i] = new CompetitionItemRoiRow {
                    ID = (int)row[0],
                    BetIDs = betIDs,
                    Roi = (float) row[2],
                    RoiType = (RoiType) (short) row[3]
                };
            }
            return result;
        }
    }
}