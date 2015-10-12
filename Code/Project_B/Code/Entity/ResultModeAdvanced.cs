using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.Code.Enums;

namespace Project_B.Code.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("ResultModeAdvanced")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class ResultModeAdvanced : AbstractEntityTemplateKey<ResultModeAdvanced, short> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Languagetype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Resultmodetype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Name,

        }

        /// <summary>
        /// 
        /// </summary>
        public ResultModeAdvanced() {
        }

        /// <summary>
        /// 
        /// </summary>
        public ResultModeAdvanced(Hashtable ht) : base(ht) {}
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
        public LanguageType Languagetype {
            get { return (LanguageType)(short) this[Fields.Languagetype]; }
            set { ForceSetData(Fields.Languagetype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ResultModeType Resultmodetype {
            get { return (ResultModeType) (short) this[Fields.Resultmodetype]; }
            set { ForceSetData(Fields.Resultmodetype, value); }
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
