using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;

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
        public string Rawresultstring {
            get { return (string) this[Fields.Rawresultstring]; }
            set { ForceSetData(Fields.Rawresultstring, value); }
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
