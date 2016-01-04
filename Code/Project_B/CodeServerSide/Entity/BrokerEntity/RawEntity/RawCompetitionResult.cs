using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Data.Result;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("RawCompetitionResult")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class RawCompetitionResult : AbstractEntityTemplateKey<RawCompetitionResult, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] RawcompetitionitemID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Rawresultstring,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Boolean)] Processed,

        }

        /// <summary>
        /// 
        /// </summary>
        public RawCompetitionResult() {
        }

        /// <summary>
        /// 
        /// </summary>
        public RawCompetitionResult(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public int RawcompetitionitemID {
            get { return (int) this[Fields.RawcompetitionitemID]; }
            set { ForceSetData(Fields.RawcompetitionitemID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public FullResult Rawresult {
            get {
                var resString = (string) this[Fields.Rawresultstring];
                return string.IsNullOrWhiteSpace(resString) ? null : ResultBuilder.BuildResultFromString(SportType.Unknown, resString);
            }
            set { ForceSetData(Fields.Rawresultstring, ResultBuilder.BuildStringFromResult(value)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Processed {
            get { return (bool) this[Fields.Processed]; }
            set { ForceSetData(Fields.Processed, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.RawcompetitionitemID }; }
        }
    }
}
