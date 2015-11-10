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
    [DBTable("CompetitorUnique")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class CompetitorUnique : AbstractEntityTemplateKey<CompetitorUnique, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,
            [DBField(DbType.Boolean)] IsUsed,

        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitorUnique() {
        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitorUnique(Hashtable ht) : base(ht) {}
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
        public bool IsUsed {
            get { return (bool)this[Fields.IsUsed]; }
            set { ForceSetData(Fields.IsUsed, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
