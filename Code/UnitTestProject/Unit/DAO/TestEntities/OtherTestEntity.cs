using System;
using System.Collections;
using System.Data;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;

namespace UnitTestProject.Unit.DAO.TestEntities {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("OtherTestEntity")]
    [TargetDb("Master")]
    public sealed class OtherTestEntity : AbstractEntity<OtherTestEntity> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Name,

        }

        /// <summary>
        /// 
        /// </summary>
        public OtherTestEntity() {
        }

        /// <summary>
        /// 
        /// </summary>
        public OtherTestEntity(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public long ID {
            get { return (long) this[Fields.ID]; }
            set { ForceSetData(Fields.ID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name {
            get { return (string) this[Fields.Name]; }
            set { ForceSetData(Fields.Name, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
