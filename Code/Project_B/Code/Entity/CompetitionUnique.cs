using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;

namespace Project_B.Code.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("CompetitionUnique")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class CompetitionUnique : AbstractEntityTemplateKey<CompetitionUnique, int> {

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
        public CompetitionUnique() {
        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionUnique(Hashtable ht) : base(ht) {}
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
            get { return (bool) this[Fields.IsUsed]; }
            set { ForceSetData(Fields.IsUsed, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
