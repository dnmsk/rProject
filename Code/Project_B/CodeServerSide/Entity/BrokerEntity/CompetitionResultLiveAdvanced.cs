using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;

namespace Project_B.CodeServerSide.Entity.BrokerEntity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("CompetitionResultLiveAdvanced")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class CompetitionResultLiveAdvanced : AbstractEntityTemplateKey<CompetitionResultLiveAdvanced, long> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] CompetitionresultliveID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ScoreID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Advancedparam,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreatedutc,

        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionResultLiveAdvanced() {
        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionResultLiveAdvanced(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public long ID {
            get { return (long) this[Fields.ID]; }
            set { ForceSetData(Fields.ID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long CompetitionresultliveID {
            get { return (long) this[Fields.CompetitionresultliveID]; }
            set { ForceSetData(Fields.CompetitionresultliveID, value); }
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
        public short Advancedparam {
            get { return (short) this[Fields.Advancedparam]; }
            set { ForceSetData(Fields.Advancedparam, value); }
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
    }
}
