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
    [DBTable("AccountIdentity")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class AccountIdentity : AbstractEntityTemplateKey<AccountIdentity, int> {

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
            [DBField(DbType.String)] Email,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Password,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Name,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] FbID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] VkID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] OkID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Vktoken,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Oktoken,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Fbtoken,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Phone,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datelastlogin,

        }

        /// <summary>
        /// 
        /// </summary>
        public AccountIdentity() {
        }

        /// <summary>
        /// 
        /// </summary>
        public AccountIdentity(Hashtable ht) : base(ht) {}
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
        public string Email {
            get { return (string) this[Fields.Email]; }
            set { ForceSetData(Fields.Email, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_256)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Password {
            get { return (string) this[Fields.Password]; }
            set { ForceSetData(Fields.Password, value); }
        }

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
        public long? FbID {
            get { return (long?) this[Fields.FbID]; }
            set { ForceSetData(Fields.FbID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long? VkID {
            get { return (long?) this[Fields.VkID]; }
            set { ForceSetData(Fields.VkID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long? OkID {
            get { return (long?) this[Fields.OkID]; }
            set { ForceSetData(Fields.OkID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Vktoken {
            get { return (string) this[Fields.Vktoken]; }
            set { ForceSetData(Fields.Vktoken, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Oktoken {
            get { return (string) this[Fields.Oktoken]; }
            set { ForceSetData(Fields.Oktoken, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Fbtoken {
            get { return (string) this[Fields.Fbtoken]; }
            set { ForceSetData(Fields.Fbtoken, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Phone {
            get { return (string) this[Fields.Phone]; }
            set { ForceSetData(Fields.Phone, value); }
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
        public DateTime? Datelastlogin {
            get { return (DateTime?) this[Fields.Datelastlogin]; }
            set { ForceSetData(Fields.Datelastlogin, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
