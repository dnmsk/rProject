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
    [DBTable("SystemStateBet")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class SystemStateBet : AbstractEntityTemplateKey<SystemStateBet, short> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Statebet,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Dateutc,

        }

        /// <summary>
        /// 
        /// </summary>
        public SystemStateBet() {
        }

        /// <summary>
        /// 
        /// </summary>
        public SystemStateBet(Hashtable ht) : base(ht) {}
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
        public short Statebet {
            get { return (short) this[Fields.Statebet]; }
            set { ForceSetData(Fields.Statebet, value); }
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
