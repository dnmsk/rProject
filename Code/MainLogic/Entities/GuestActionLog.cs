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
    [DBTable("GuestActionLog")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class GuestActionLog : AbstractEntityTemplateKey<GuestActionLog, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] GuestID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] UtmsubdomainruleID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Action,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Arg,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

        }

        /// <summary>
        /// 
        /// </summary>
        public GuestActionLog() {
        }

        /// <summary>
        /// 
        /// </summary>
        public GuestActionLog(Hashtable ht) : base(ht) {}
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
        public long GuestID {
            get { return (long) this[Fields.GuestID]; }
            set { ForceSetData(Fields.GuestID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int UtmsubdomainruleID {
            get { return (int) this[Fields.UtmsubdomainruleID]; }
            set { ForceSetData(Fields.UtmsubdomainruleID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Action {
            get { return (int) this[Fields.Action]; }
            set { ForceSetData(Fields.Action, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Arg {
            get { return (int?) this[Fields.Arg]; }
            set { ForceSetData(Fields.Arg, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datecreated {
            get { return (DateTime) this[Fields.Datecreated]; }
            set { ForceSetData(Fields.Datecreated, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
