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
    [DBTable("UtmGuestReferrer")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class UtmGuestReferrer : AbstractEntityTemplateKey<UtmGuestReferrer, int> {

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
            [DBField(DbType.String)] Campaign,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Medium,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Source,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

        }

        /// <summary>
        /// 
        /// </summary>
        public UtmGuestReferrer() {
        }

        /// <summary>
        /// 
        /// </summary>
        public UtmGuestReferrer(Hashtable ht) : base(ht) {}
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
        public string Campaign {
            get { return (string) this[Fields.Campaign]; }
            set { ForceSetData(Fields.Campaign, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_128)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Medium {
            get { return (string) this[Fields.Medium]; }
            set { ForceSetData(Fields.Medium, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_128)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Source {
            get { return (string) this[Fields.Source]; }
            set { ForceSetData(Fields.Source, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_128)); }
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
