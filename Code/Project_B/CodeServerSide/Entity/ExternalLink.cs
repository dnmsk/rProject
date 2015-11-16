using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;

namespace Project_B.CodeServerSide.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("ExternalLink")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class ExternalLink : AbstractEntityTemplateKey<ExternalLink, short> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Link,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Followcount,

        }

        /// <summary>
        /// 
        /// </summary>
        public ExternalLink() {
        }

        /// <summary>
        /// 
        /// </summary>
        public ExternalLink(Hashtable ht) : base(ht) {}
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
        public string Link {
            get { return (string) this[Fields.Link]; }
            set { ForceSetData(Fields.Link, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Followcount {
            get { return (int) this[Fields.Followcount]; }
            set { ForceSetData(Fields.Followcount, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
