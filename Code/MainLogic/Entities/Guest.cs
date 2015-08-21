using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;

namespace MainLogic.Entities {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("Guest")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Guest : AbstractEntityTemplateKey<Guest, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Ip,

        }

        /// <summary>
        /// 
        /// </summary>
        public Guest() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Guest(Hashtable ht) : base(ht) {}
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
        public DateTime Datecreated {
            get { return (DateTime) this[Fields.Datecreated]; }
            set { ForceSetData(Fields.Datecreated, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Ip {
            get { return (string) this[Fields.Ip]; }
            set { ForceSetData(Fields.Ip, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
