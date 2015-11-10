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
    [DBTable("Score")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Score : AbstractEntityTemplateKey<Score, short> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Score1,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Score2,

        }

        /// <summary>
        /// 
        /// </summary>
        public Score() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Score(Hashtable ht) : base(ht) {}
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
        public short Score1 {
            get { return (short) this[Fields.Score1]; }
            set { ForceSetData(Fields.Score1, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Score2 {
            get { return (short) this[Fields.Score2]; }
            set { ForceSetData(Fields.Score2, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
