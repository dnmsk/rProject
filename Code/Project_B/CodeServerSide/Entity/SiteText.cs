using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("SiteText")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class SiteText : AbstractEntityTemplateKey<SiteText, short>, ILanguageTyped, IDateCreatedTyped {

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
            [DBField(DbType.Int16)] Sitetext,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Text,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreatedutc,

        }

        /// <summary>
        /// 
        /// </summary>
        public SiteText() {
        }

        /// <summary>
        /// 
        /// </summary>
        public SiteText(Hashtable ht) : base(ht) {}
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
            get { return (LanguageType) (short) this[Fields.Languagetype]; }
            set { ForceSetData(Fields.Languagetype, (short) value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public SiteTextType Sitetext {
            get { return (SiteTextType) (short) this[Fields.Sitetext]; }
            set { ForceSetData(Fields.Sitetext, (short) value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text {
            get { return (string) this[Fields.Text]; }
            set { ForceSetData(Fields.Text, value); }
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
