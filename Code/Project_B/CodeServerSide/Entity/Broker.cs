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
    [DBTable("Broker")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Broker : AbstractEntityTemplateKey<Broker, short> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Name,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] ExternalUrl,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] InternalPage,

            /// <summary>
            /// 
            /// </summary>
            [DBField(DbType.Int16)] BrokerCompetitionSettings,
        }

        /// <summary>
        /// 
        /// </summary>
        public Broker() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Broker(Hashtable ht) : base(ht) {}
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
        public BrokerCompetitionSettings CompetitionSettings {
            get { return (BrokerCompetitionSettings) (short) this[Fields.BrokerCompetitionSettings]; }
            set { ForceSetData(Fields.BrokerCompetitionSettings, value); }
        }

        public BrokerType BrokerType => (BrokerType) ID;

        /// <summary>
        /// 
        /// </summary>
        public string Name {
            get { return (string) this[Fields.Name]; }
            set { ForceSetData(Fields.Name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExternalUrl {
            get { return (string) this[Fields.ExternalUrl]; }
            set { ForceSetData(Fields.ExternalUrl, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string InternalPage {
            get { return (string) this[Fields.InternalPage]; }
            set { ForceSetData(Fields.InternalPage, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
