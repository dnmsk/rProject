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
    [DBTable("SystemBrokerRequest")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class SystemBrokerRequest : AbstractEntityTemplateKey<SystemBrokerRequest, int>, IBrokerTyped, ILanguageTyped, IDateCreatedTyped {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

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
            [DBField(DbType.DateTime)] Datecreatedutc,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datelaunchedutc,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] AlgoDateutc,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Taskmodetype,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] Duratation,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int16)] Counttotalrawci,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int16)] Countnewrawci,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int16)] Countnewci,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int16)] Counttotalentity,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int16)] Countnewentity,

        }

        /// <summary>
        /// 
        /// </summary>
        public SystemBrokerRequest() {
        }

        /// <summary>
        /// 
        /// </summary>
        public SystemBrokerRequest(Hashtable ht) : base(ht) {}
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
        public BrokerType BrokerID {
            get { return (BrokerType) (short) this[Fields.BrokerID]; }
            set { ForceSetData(Fields.BrokerID, value); }
        }

        public Enum BrokerField => Fields.BrokerID;

        /// <summary>
        /// 
        /// </summary>
        public LanguageType Languagetype {
            get { return (LanguageType) (short) this[Fields.Languagetype]; }
            set { ForceSetData(Fields.Languagetype, value); }
        }
        public Enum LanguageTypeField => Fields.Languagetype;

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datecreatedutc {
            get { return (DateTime) this[Fields.Datecreatedutc]; }
            set { ForceSetData(Fields.Datecreatedutc, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datelaunchedutc {
            get { return (DateTime) this[Fields.Datelaunchedutc]; }
            set { ForceSetData(Fields.Datelaunchedutc, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime AlgoDateutc {
            get { return (DateTime) this[Fields.AlgoDateutc]; }
            set { ForceSetData(Fields.AlgoDateutc, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public RunTaskMode Taskmodetype {
            get { return (RunTaskMode) (short) this[Fields.Taskmodetype]; }
            set { ForceSetData(Fields.Taskmodetype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Duratation {
            get { return (int)(this[Fields.Duratation] ?? default(int)); }
            set { ForceSetData(Fields.Duratation, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Counttotalrawci {
            get { return (short)(this[Fields.Counttotalrawci] ?? default(short)); }
            set { ForceSetData(Fields.Counttotalrawci, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Countnewrawci {
            get { return (short)(this[Fields.Countnewrawci] ?? default(short)); }
            set { ForceSetData(Fields.Countnewrawci, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Countnewci {
            get { return (short)(this[Fields.Countnewci] ?? default(short)); }
            set { ForceSetData(Fields.Countnewci, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Counttotalentity {
            get { return (short)(this[Fields.Counttotalentity] ?? default(short)); }
            set { ForceSetData(Fields.Counttotalentity, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Countnewentity {
            get { return (short) (this[Fields.Countnewentity] ?? default(short)); }
            set { ForceSetData(Fields.Countnewentity, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
