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
    [DBTable("Competitor")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Competitor : AbstractEntityTemplateKey<Competitor, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] CompetitoruniqueID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Languagetype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] NameShort,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] NameFull,

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
        public Competitor() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Competitor(Hashtable ht) : base(ht) {}
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
        public int CompetitoruniqueID {
            get { return (int) this[Fields.CompetitoruniqueID]; }
            set { ForceSetData(Fields.CompetitoruniqueID, value); }
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
        public string NameShort {
            get { return (string) this[Fields.NameShort]; }
            set { ForceSetData(Fields.NameShort, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string NameFull {
            get { return (string) this[Fields.NameFull]; }
            set { ForceSetData(Fields.NameFull, value); }
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
            get { return (GenderType)(short) this[Fields.Gendertype]; }
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
    }
}
