using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("SystemBrokerRequestSchedule")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class SystemBrokerRequestSchedule : AbstractEntityTemplateKey<SystemBrokerRequestSchedule, short>, IBrokerTyped, ILanguageTyped, ISportTyped {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ID,
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] BrokerID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Languagetype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Betrepeat,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Livebetrepeat,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Historyrepeat,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Pasthistoryrepeat,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Pasthistorydays,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Behaviormode,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Sporttype,

        }

        /// <summary>
        /// 
        /// </summary>
        public SystemBrokerRequestSchedule() {
        }

        /// <summary>
        /// 
        /// </summary>
        public SystemBrokerRequestSchedule(Hashtable ht) : base(ht) {}

        /// <summary>
        /// 
        /// </summary>
        public BrokerType ID {
            get { return (BrokerType) (short) this[Fields.ID]; }
            set { ForceSetData(Fields.ID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BrokerType BrokerID {
            get { return (BrokerType) (short) this[Fields.BrokerID]; }
            set { ForceSetData(Fields.BrokerID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public LanguageType Languagetype {
            get { return (LanguageType) (short) this[Fields.Languagetype]; }
            set { ForceSetData(Fields.Languagetype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Betrepeat {
            get { return (short) this[Fields.Betrepeat]; }
            set { ForceSetData(Fields.Betrepeat, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Livebetrepeat {
            get { return (short) this[Fields.Livebetrepeat]; }
            set { ForceSetData(Fields.Livebetrepeat, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Historyrepeat {
            get { return (short) this[Fields.Historyrepeat]; }
            set { ForceSetData(Fields.Historyrepeat, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Pasthistoryrepeat {
            get { return (short) this[Fields.Pasthistoryrepeat]; }
            set { ForceSetData(Fields.Pasthistoryrepeat, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Pasthistorydays {
            get { return (short) this[Fields.Pasthistorydays]; }
            set { ForceSetData(Fields.Pasthistorydays, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public GatherBehaviorMode Behaviormode {
            get { return (GatherBehaviorMode) (short) this[Fields.Behaviormode]; }
            set { ForceSetData(Fields.Behaviormode, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public SportType SportType {
            get { return (SportType)(short)this[Fields.Sporttype]; }
            set { ForceSetData(Fields.Sporttype, value); }
        }


        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
