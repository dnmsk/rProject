using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Entities {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("Phraseaccount")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Phraseaccount : CollectionIdentityEntity<Phraseaccount> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] AccountidentityID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] PhraseID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,
            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int16)] SourceType,

            [DBField(DbType.Int16)]
            CollectionIdentity,

        }

        /// <summary>
        /// 
        /// </summary>
        public Phraseaccount() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Phraseaccount(Hashtable ht) : base(ht) {}
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
        public int AccountidentityID {
            get { return (int) this[Fields.AccountidentityID]; }
            set { ForceSetData(Fields.AccountidentityID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PhraseID {
            get { return (int) this[Fields.PhraseID]; }
            set { ForceSetData(Fields.PhraseID, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public SourceType SourceType {
            get { return (SourceType)((short?)this[Fields.SourceType] ?? default(short)); }
            set { ForceSetData(Fields.SourceType, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datecreated {
            get { return (DateTime) this[Fields.Datecreated]; }
            set { ForceSetData(Fields.Datecreated, value); }
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
