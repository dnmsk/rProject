using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;

namespace Project_B.Code.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("CompetitionResultLive")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class CompetitionResultLive : AbstractEntityTemplateKey<CompetitionResultLive, long> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] ID,

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
            [DBField(DbType.DateTime)] Datecreatedutc,

        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionResultLive() {
        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionResultLive(Hashtable ht) : base(ht) {}
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
        public DateTime Datecreatedutc {
            get { return (DateTime) this[Fields.Datecreatedutc]; }
            set { ForceSetData(Fields.Datecreatedutc, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
