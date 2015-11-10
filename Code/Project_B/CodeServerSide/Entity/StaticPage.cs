using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity {
    /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("StaticPage")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class StaticPage : AbstractEntityTemplateKey<StaticPage, int>, IStaticPage {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Languagetype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Title,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Keywords,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Description,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Content,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Pagetype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreatedutc,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datepublishedutc,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datemodifiedutc,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Boolean)] Istop,

        }

        /// <summary>
        /// 
        /// </summary>
        public StaticPage() {
        }

        /// <summary>
        /// 
        /// </summary>
        public StaticPage(Hashtable ht) : base(ht) {}
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
        public LanguageType Languagetype {
            get { return (LanguageType) this[Fields.Languagetype]; }
            set { ForceSetData(Fields.Languagetype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title {
            get { return (string) this[Fields.Title] ?? string.Empty; }
            set { ForceSetData(Fields.Title, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Keywords {
            get { return (string) this[Fields.Keywords] ?? string.Empty; }
            set { ForceSetData(Fields.Keywords, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description {
            get { return (string) this[Fields.Description] ?? string.Empty; }
            set { ForceSetData(Fields.Description, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Content {
            get { return (string) this[Fields.Content] ?? string.Empty; }
            set { ForceSetData(Fields.Content, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public PageType Pagetype {
            get { return (PageType) ((short?) this[Fields.Pagetype] ?? default(short)); }
            set { ForceSetData(Fields.Pagetype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datecreatedutc {
            get { return (DateTime) this[Fields.Datecreatedutc]; }
            set { ForceSetData(Fields.Datecreatedutc, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? Datepublishedutc {
            get { return (DateTime?) this[Fields.Datepublishedutc]; }
            set { ForceSetData(Fields.Datepublishedutc, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datemodifiedutc {
            get { return (DateTime) this[Fields.Datemodifiedutc]; }
            set { ForceSetData(Fields.Datemodifiedutc, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Istop {
            get { return (bool) this[Fields.Istop]; }
            set { ForceSetData(Fields.Istop, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
