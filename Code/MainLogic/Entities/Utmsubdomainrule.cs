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
    [DBTable("Utmsubdomainrule")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class UtmSubdomainRule : AbstractEntityTemplateKey<UtmSubdomainRule, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Subdomainname,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Targetdomain,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

        }

        /// <summary>
        /// 
        /// </summary>
        public UtmSubdomainRule() {
        }

        /// <summary>
        /// 
        /// </summary>
        public UtmSubdomainRule(Hashtable ht) : base(ht) {}
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
        public string Subdomainname {
            get { return (string) this[Fields.Subdomainname]; }
            set { ForceSetData(Fields.Subdomainname, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Targetdomain {
            get { return (string) this[Fields.Targetdomain]; }
            set { ForceSetData(Fields.Targetdomain, value); }
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
