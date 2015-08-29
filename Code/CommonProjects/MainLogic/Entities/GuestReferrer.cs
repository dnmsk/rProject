using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using MainLogic.Consts;

namespace MainLogic.Entities {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("Guestreferrer")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class GuestReferrer : AbstractEntityTemplateKey<GuestReferrer, int> {

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
            [DBField(DbType.DateTime)] Datecreated,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Urlreferrer,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Urltarget,

        }

        /// <summary>
        /// 
        /// </summary>
        public GuestReferrer() {
        }

        /// <summary>
        /// 
        /// </summary>
        public GuestReferrer(Hashtable ht) : base(ht) {}
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
        public DateTime Datecreated {
            get { return (DateTime) this[Fields.Datecreated]; }
            set { ForceSetData(Fields.Datecreated, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Urlreferrer {
            get { return (string) this[Fields.Urlreferrer]; }
            set { ForceSetData(Fields.Urlreferrer, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_512)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Urltarget {
            get { return (string) this[Fields.Urltarget]; }
            set { ForceSetData(Fields.Urltarget, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_512)); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
