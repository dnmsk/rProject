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
    [DBTable("BrokerPage")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class BrokerPage : AbstractEntityTemplateKey<BrokerPage, int>, IStaticPage {

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
            [DBField(DbType.String)] Pageurl,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] TargetUrl,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Largeiconclass,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Orderindex,

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
            [DBField(DbType.Int32)] Brokertype,

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
        public BrokerPage() {
        }

        /// <summary>
        /// 
        /// </summary>
        public BrokerPage(Hashtable ht) : base(ht) {}
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
        public string Pageurl {
            get { return (string) this[Fields.Pageurl] ?? string.Empty; }
            set { ForceSetData(Fields.Pageurl, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TargetUrl {
            get { return (string) this[Fields.TargetUrl] ?? string.Empty; }
            set { ForceSetData(Fields.TargetUrl, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Largeiconclass {
            get { return (string) this[Fields.Largeiconclass] ?? string.Empty; }
            set { ForceSetData(Fields.Largeiconclass, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short Orderindex {
            get { return (short?) this[Fields.Orderindex] ?? short.MaxValue; }
            set { ForceSetData(Fields.Orderindex, value); }
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
        public BrokerType Brokertype {
            get { return (BrokerType) ((int?) this[Fields.Brokertype] ?? default(int)); }
            set { ForceSetData(Fields.Brokertype, value); }
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
