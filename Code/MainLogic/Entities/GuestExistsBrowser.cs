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
    [DBTable("GuestExistsBrowser")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class GuestExistsBrowser : AbstractEntityTemplateKey<GuestExistsBrowser, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Browsertype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Decimal, 10, 8)] Version,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Os,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Boolean)] Ismobile,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Boolean)] Isbot,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Useragent,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

        }

        /// <summary>
        /// 
        /// </summary>
        public GuestExistsBrowser() {
        }

        /// <summary>
        /// 
        /// </summary>
        public GuestExistsBrowser(Hashtable ht) : base(ht) {}
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
        public string Browsertype {
            get { return (string) this[Fields.Browsertype]; }
            set { ForceSetData(Fields.Browsertype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal? Version {
            get { return (decimal?) this[Fields.Version]; }
            set { ForceSetData(Fields.Version, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Os {
            get { return (string) this[Fields.Os]; }
            set { ForceSetData(Fields.Os, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? Ismobile {
            get { return (bool?) this[Fields.Ismobile]; }
            set { ForceSetData(Fields.Ismobile, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? Isbot {
            get { return (bool?) this[Fields.Isbot]; }
            set { ForceSetData(Fields.Isbot, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Useragent {
            get { return (string) this[Fields.Useragent]; }
            set { ForceSetData(Fields.Useragent, value); }
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
