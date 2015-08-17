using System;
using System.Collections;
using System.Data;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;

namespace UnitTestProject.Unit.DAO.TestEntities {
    public enum TestEnum {
        One = 1,
        Two = 2,
        Three = 3
    }
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("TestEntity")]
    [TargetDb("Master")]
    public sealed class TestEntity : AbstractEntity<TestEntity> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Name,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Enum,

        }

        /// <summary>
        /// 
        /// </summary>
        public TestEntity() {
        }

        /// <summary>
        /// 
        /// </summary>
        public TestEntity(Hashtable ht) : base(ht) {}
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

        /// <summary>
        /// 
        /// </summary>
        public TestEnum? Enum {
            get { return (TestEnum?) (int?) this[Fields.Enum]; }
            set { ForceSetData(Fields.Enum, (int?)value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
