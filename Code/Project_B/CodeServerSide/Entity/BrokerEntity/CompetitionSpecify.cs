using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.BrokerEntity {
    /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("CompetitionSpecify")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class CompetitionSpecify : AbstractEntityTemplateKey<CompetitionSpecify, int>, ICompetitionSpecify {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] CompetitionuniqueID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)]
            [Nullable]
            CompetitionSpecifyUniqueID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Languagetype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Name,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Sporttype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Gendertype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreatedutc,

        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionSpecify() {
        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionSpecify(Hashtable ht) : base(ht) {}
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

        /// <summary>
        /// 
        /// </summary>
        public int CompetitionSpecifyUniqueID {
            get { return (int) (this[Fields.CompetitionSpecifyUniqueID] ?? default(int)); }
            set { ForceSetData(Fields.CompetitionSpecifyUniqueID, value); }
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
        public string Name {
            get { return (string) this[Fields.Name]; }
            set { ForceSetData(Fields.Name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public SportType SportType {
            get { return (SportType)(short) this[Fields.Sporttype]; }
            set { ForceSetData(Fields.Sporttype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public GenderType Gendertype {
            get { return (GenderType)(short)this[Fields.Gendertype]; }
            set { ForceSetData(Fields.Gendertype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datecreatedutc {
            get { return (DateTime) this[Fields.Datecreatedutc]; }
            set { ForceSetData(Fields.Datecreatedutc, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }

        public int UniqueID => CompetitionSpecifyUniqueID;
    }
}
