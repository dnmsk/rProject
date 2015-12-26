using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("SiteFileStore")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class SiteFileStore : AbstractEntityTemplateKey<SiteFileStore, short>, IDateCreatedTyped {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Fileformat,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Accesscount,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreatedutc,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] DateupDatedutc,

        }

        /// <summary>
        /// 
        /// </summary>
        public SiteFileStore() {
        }

        /// <summary>
        /// 
        /// </summary>
        public SiteFileStore(Hashtable ht) : base(ht) {}
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
        public FileFormat Fileformat {
            get { return (FileFormat) (short) this[Fields.Fileformat]; }
            set { ForceSetData(Fields.Fileformat, (short) value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Accesscount {
            get { return (int) this[Fields.Accesscount]; }
            set { ForceSetData(Fields.Accesscount, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datecreatedutc {
            get { return (DateTime) this[Fields.Datecreatedutc]; }
            set { ForceSetData(Fields.Datecreatedutc, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateupDatedutc {
            get { return (DateTime) this[Fields.DateupDatedutc]; }
            set { ForceSetData(Fields.DateupDatedutc, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
