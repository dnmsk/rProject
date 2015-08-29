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
    [DBTable("GuestTechInfo")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class GuestTechInfo : AbstractEntityTemplateKey<GuestTechInfo, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] GuestID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] GuestexistsbrowserID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

        }

        /// <summary>
        /// 
        /// </summary>
        public GuestTechInfo() {
        }

        /// <summary>
        /// 
        /// </summary>
        public GuestTechInfo(Hashtable ht) : base(ht) {}
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
        public int GuestID {
            get { return (int) this[Fields.GuestID]; }
            set { ForceSetData(Fields.GuestID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int GuestexistsbrowserID {
            get { return (int) this[Fields.GuestexistsbrowserID]; }
            set { ForceSetData(Fields.GuestexistsbrowserID, value); }
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
