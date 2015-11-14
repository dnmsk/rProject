using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeClientSide.Enums;
using Project_B.CodeServerSide.Enums;

namespace DbEntity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("LanguageSiteText")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class LanguageSiteText : AbstractEntityTemplateKey<LanguageSiteText, short> {

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
        public LanguageSiteText() {
        }

        /// <summary>
        /// 
        /// </summary>
        public LanguageSiteText(Hashtable ht) : base(ht) {}
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
        public SiteText Sitetext {
            get { return (SiteText) (short) this[Fields.Sitetext]; }
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
