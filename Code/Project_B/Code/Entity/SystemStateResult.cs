using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.Code.Enums;

namespace Project_B.Code.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("SystemStateResult")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class SystemStateResult : AbstractEntityTemplateKey<SystemStateResult, short> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Stateresult,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Dateutc,

        }

        /// <summary>
        /// 
        /// </summary>
        public SystemStateResult() {
        }

        /// <summary>
        /// 
        /// </summary>
        public SystemStateResult(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public short ID {
            get { return (short) this[Fields.ID]; }
            set { ForceSetData(Fields.ID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public SystemStateResultType Stateresult {
            get { return (SystemStateResultType) (short) this[Fields.Stateresult]; }
            set { ForceSetData(Fields.Stateresult, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Dateutc {
            get { return (DateTime) this[Fields.Dateutc]; }
            set { ForceSetData(Fields.Dateutc, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
