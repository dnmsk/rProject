using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using MainLogic.Consts;

namespace Spywords_Project.Code.Entities {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("Domainphone")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Domainphone : AbstractEntityTemplateKey<Domainphone, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] DomainID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Phone,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

        }

        /// <summary>
        /// 
        /// </summary>
        public Domainphone() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Domainphone(Hashtable ht) : base(ht) {}
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
        public int DomainID {
            get { return (int) this[Fields.DomainID]; }
            set { ForceSetData(Fields.DomainID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Phone {
            get { return (string) this[Fields.Phone]; }
            set { ForceSetData(Fields.Phone, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_128)); }
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
