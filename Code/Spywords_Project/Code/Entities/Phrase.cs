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
    [DBTable("Phrase")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Phrase : CollectionIdentityEntity<Phrase> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Text,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Textbaseform,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Status,

        /// <summary>
        /// 
        /// </summary>
        [Nullable]
            [DBField(DbType.Int32)] Showsgoogle,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] Showsyandex,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int16)] Advertisersgoogle,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int16)] Advertisersyandex,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.DateTime)] Datecollected,

            [DBField(DbType.Int16)]
            CollectionIdentity,

        }

        /// <summary>
        /// 
        /// </summary>
        public Phrase() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Phrase(Hashtable ht) : base(ht) {}
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
        public string Text {
            get { return (string) this[Fields.Text]; }
            set {
                ForceSetData(Fields.Text, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_512));
                Textbaseform = QueryProcessor.Instance.ProcessQuery(value).ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Textbaseform {
            get { return (string) this[Fields.Textbaseform]; }
            set { ForceSetData(Fields.Textbaseform, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_512)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public PhraseStatus Status {
            get { return (PhraseStatus) (short) this[Fields.Status]; }
            set { ForceSetData(Fields.Status, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Showsgoogle {
            get { return (int?) this[Fields.Showsgoogle] ?? default(int); }
            set { ForceSetData(Fields.Showsgoogle, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? Showsyandex {
            get { return (int?) this[Fields.Showsyandex] ?? default(int); }
            set { ForceSetData(Fields.Showsyandex, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short? Advertisersgoogle {
            get { return (short?) this[Fields.Advertisersgoogle] ?? default(short); }
            set { ForceSetData(Fields.Advertisersgoogle, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public short? Advertisersyandex {
            get { return (short?) this[Fields.Advertisersyandex] ?? default(short); }
            set { ForceSetData(Fields.Advertisersyandex, value); }
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

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }

        public override CollectionIdentity CollectionIdentity {
            get { return (CollectionIdentity) (short) this[Fields.CollectionIdentity]; }
            set { ForceSetData(Fields.CollectionIdentity, (short) value); }
        }
    }
}
