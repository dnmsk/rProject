using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;

namespace Project_B.CodeServerSide.Entity.BrokerEntity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("CompetitionSpecifyUnique")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class CompetitionSpecifyUnique : AbstractEntityTemplateKey<CompetitionSpecifyUnique, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] CompetitionuniqueID,

        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionSpecifyUnique() {
        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionSpecifyUnique(Hashtable ht) : base(ht) {}
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
        public int CompetitionuniqueID {
            get { return (int) this[Fields.CompetitionuniqueID]; }
            set { ForceSetData(Fields.CompetitionuniqueID, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
