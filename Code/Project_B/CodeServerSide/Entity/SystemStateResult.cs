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

            /// <summary>
            /// 
            /// </summary>
            [DBField(DbType.Int16)]
            BrokerID,

            /// <summary>
            /// 
            /// </summary>
            [DBField(DbType.Int16)]
            Languagetype,
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
            set { ForceSetData(Fields.Stateresult, (short) value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Dateutc {
            get { return (DateTime) this[Fields.Dateutc]; }
            set { ForceSetData(Fields.Dateutc, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public LanguageType Languagetype {
            get { return (LanguageType)(short)this[Fields.Languagetype]; }
            set { ForceSetData(Fields.Languagetype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BrokerType BrokerID {
            get { return (BrokerType)(short)this[Fields.BrokerID]; }
            set { ForceSetData(Fields.BrokerID, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
