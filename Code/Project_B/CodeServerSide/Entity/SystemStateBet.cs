using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("SystemStateBet")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class SystemStateBet : AbstractEntityTemplateKey<SystemStateBet, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Statebet,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Brokerid,

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
        public int ID {
            get { return (int) this[Fields.ID]; }
            set { ForceSetData(Fields.ID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public SystemStateBetType Statebet {
            get { return (SystemStateBetType) (int) this[Fields.Statebet]; }
            set { ForceSetData(Fields.Statebet, (int) value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BrokerType BrokerID {
            get { return (BrokerType) (int) this[Fields.Brokerid]; }
            set { ForceSetData(Fields.Brokerid, (int) value); }
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
