﻿using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Entity.Interface;

namespace Project_B.CodeServerSide.Entity.BrokerEntity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("CompetitionUnique")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class CompetitionUnique : AbstractEntityTemplateKey<CompetitionUnique, int>, IUniqueID {

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
            IsUsed = true;
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

        public int UniqueID => ID;
    }
}
