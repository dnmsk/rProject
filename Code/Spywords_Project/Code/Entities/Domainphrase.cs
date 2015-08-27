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
    [DBTable("Domainphrase")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Domainphrase : AbstractEntityTemplateKey<Domainphrase, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] DomainID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] PhraseID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] SE,

        }

        /// <summary>
        /// 
        /// </summary>
        public Domainphrase() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Domainphrase(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public int DomainID {
            get { return (int) this[Fields.DomainID]; }
            set { ForceSetData(Fields.DomainID, value); }
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
        public SearchEngine SE {
            get { return (SearchEngine) (short) this[Fields.PhraseID]; }
            set { ForceSetData(Fields.PhraseID, value); }
        }

        public override Enum[] KeyFields {
            get { return new Enum[] {}; }
        }
    }
}
