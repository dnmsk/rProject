using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using IDEV.Hydra.DAO.MassTools;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.BrokerEntity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("CompetitionResult")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class CompetitionResult : AbstractEntityTemplateKey<CompetitionResult, int>, IDateCreatedTyped {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] CompetitionitemID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ScoreID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Resulttype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreatedutc,

        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionResult() {
        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionResult(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public int ID {
            get { return (int) this[Fields.ID]; }
            set { ForceSetData(Fields.ID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CompetitionitemID {
            get { return (int) this[Fields.CompetitionitemID]; }
            set { ForceSetData(Fields.CompetitionitemID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short ScoreID {
            get { return (short) this[Fields.ScoreID]; }
            set { ForceSetData(Fields.ScoreID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BetOddType Resulttype {
            get { return (BetOddType)(short) this[Fields.Resulttype]; }
            set { ForceSetData(Fields.Resulttype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datecreatedutc {
            get { return (DateTime) this[Fields.Datecreatedutc]; }
            set { ForceSetData(Fields.Datecreatedutc, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }

        public static CompetitionResult ProcessResult(ProcessStat processStat, int competitionItemID, SportType sportType, FullResult fullResult) {
            var result = DataSource
                            .WhereEquals(Fields.CompetitionitemID, competitionItemID)
                            .First();
            if (result != null &&
                result.ScoreID != ScoreHelper.Instance.GenerateScoreID(fullResult.CompetitorResultOne, fullResult.CompetitorResultTwo)) {
                CompetitionResultAdvanced.DataSource
                    .WhereEquals(CompetitionResultAdvanced.Fields.CompetitionresultID, result.ID)
                    .Delete();
                result.Delete();
                result = null;
            }
            if (result == null) {
                result = new CompetitionResult {
                    CompetitionitemID = competitionItemID,
                    Datecreatedutc = DateTime.UtcNow,
                    ScoreID = ScoreHelper.Instance.GenerateScoreID(fullResult.CompetitorResultOne, fullResult.CompetitorResultTwo),
                    Resulttype = ScoreHelper.Instance.GetResultType(fullResult.CompetitorResultOne, fullResult.CompetitorResultTwo)
                };
                result.Save();
                if (fullResult.SubResult != null && fullResult.SubResult.Count > 0) {
                    var listSubResult = new List<CompetitionResultAdvanced>();
                    for (var subResultIndex = 0; subResultIndex < fullResult.SubResult.Count; subResultIndex++) {
                        var subResult = fullResult.SubResult[subResultIndex];
                        listSubResult.Add(new CompetitionResultAdvanced {
                            CompetitionitemID = competitionItemID,
                            Resulttype = result.Resulttype,
                            CompetitionresultID = result.ID,
                            ScoreID = ScoreHelper.Instance.GenerateScoreID(subResult.CompetitorResultOne, subResult.CompetitorResultTwo),
                            Resultmodetype = ScoreHelper.Instance.GetResultModeType(sportType, subResultIndex, subResult.ModeTypeString)
                        });
                    }
                    listSubResult.Save<CompetitionResultAdvanced, int>();
                }
                processStat.CreateOriginalCount++;
            }
            return result;
        }
    }
}
