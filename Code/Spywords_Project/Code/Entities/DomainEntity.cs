using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using MainLogic.Consts;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Entities {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("Domain")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class DomainEntity : AbstractEntityTemplateKey<DomainEntity, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Domain,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] Phrasesgoogle,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] Phrasesyandex,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] Advertsgoogle,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] Advertsyandex,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] Budgetgoogle,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] Budgetyandex,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] Visitsmonth,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.DateTime)] Datecollected,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Status,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.String)] Content,

        }

        /// <summary>
        /// 
        /// </summary>
        public DomainEntity() {
        }

        /// <summary>
        /// 
        /// </summary>
        public DomainEntity(Hashtable ht) : base(ht) {}
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
        public string Domain {
            get { return (string) this[Fields.Domain]; }
            set { ForceSetData(Fields.Domain, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_256)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Phrasesgoogle {
            get { return (int?) this[Fields.Phrasesgoogle] ?? default(int); }
            set { ForceSetData(Fields.Phrasesgoogle, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Phrasesyandex {
            get { return (int?) this[Fields.Phrasesyandex] ?? default(int); }
            set { ForceSetData(Fields.Phrasesyandex, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Advertsgoogle {
            get { return (int?) this[Fields.Advertsgoogle] ?? default(int); }
            set { ForceSetData(Fields.Advertsgoogle, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Advertsyandex {
            get { return (int?) this[Fields.Advertsyandex] ?? default(int); }
            set { ForceSetData(Fields.Advertsyandex, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Budgetgoogle {
            get { return (int?) this[Fields.Budgetgoogle] ?? default(int); }
            set { ForceSetData(Fields.Budgetgoogle, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Budgetyandex {
            get { return (int?) this[Fields.Budgetyandex] ?? default(int); }
            set { ForceSetData(Fields.Budgetyandex, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Visitsmonth {
            get { return (int?) this[Fields.Visitsmonth] ?? default(int); }
            set { ForceSetData(Fields.Visitsmonth, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datecreated {
            get { return (DateTime) this[Fields.Datecreated]; }
            set { ForceSetData(Fields.Datecreated, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? Datecollected {
            get { return (DateTime?) this[Fields.Datecollected]; }
            set { ForceSetData(Fields.Datecollected, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DomainStatus Status {
            get { return (DomainStatus)(short) this[Fields.Status]; }
            set { ForceSetData(Fields.Status, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Content {
            get { return (string)this[Fields.Content]; }
            set { ForceSetData(Fields.Content, value.Replace("\0", string.Empty)); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
