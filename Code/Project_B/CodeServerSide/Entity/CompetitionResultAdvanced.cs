using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("CompetitionResultAdvanced")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class CompetitionResultAdvanced : AbstractEntityTemplateKey<CompetitionResultAdvanced, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] CompetitionitemID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] CompetitionresultID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ScoreID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Resulttype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Resultmodetype,

        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionResultAdvanced() {
        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionResultAdvanced(Hashtable ht) : base(ht) {}
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
        public int CompetitionitemID {
            get { return (int) this[Fields.CompetitionitemID]; }
            set { ForceSetData(Fields.CompetitionitemID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CompetitionresultID {
            get { return (int) this[Fields.CompetitionresultID]; }
            set { ForceSetData(Fields.CompetitionresultID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short ScoreID {
            get { return (short) this[Fields.ScoreID]; }
            set { ForceSetData(Fields.ScoreID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BetOddType Resulttype {
            get { return (BetOddType) (short) this[Fields.Resulttype]; }
            set { ForceSetData(Fields.Resulttype, (short) value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ResultModeType Resultmodetype {
            get { return (ResultModeType) (short) this[Fields.Resultmodetype]; }
            set { ForceSetData(Fields.Resultmodetype, (short)value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
