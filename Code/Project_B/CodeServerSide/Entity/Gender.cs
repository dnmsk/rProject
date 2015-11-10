using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;

namespace Project_B.CodeServerSide.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("Gender")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Gender : AbstractEntityTemplateKey<Gender, short> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Name,

        }

        /// <summary>
        /// 
        /// </summary>
        public Gender() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Gender(Hashtable ht) : base(ht) {}
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
        public string Name {
            get { return (string) this[Fields.Name]; }
            set { ForceSetData(Fields.Name, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
