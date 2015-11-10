using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("CompetitionItem")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class CompetitionItem : AbstractEntityTemplateKey<CompetitionItem, int> {

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
            [DBField(DbType.Int16)] Sporttype,
            
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Competitoruniqueid1,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Competitoruniqueid2,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreatedutc,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Dateeventutc,

        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionItem() {
        }

        /// <summary>
        /// 
        /// </summary>
        public CompetitionItem(Hashtable ht) : base(ht) {}

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
        public SportType SportType {
            get { return (SportType)(short) this[Fields.Sporttype]; }
            set { ForceSetData(Fields.Sporttype, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public int Competitoruniqueid1 {
            get { return (int) this[Fields.Competitoruniqueid1]; }
            set { ForceSetData(Fields.Competitoruniqueid1, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Competitoruniqueid2 {
            get { return (int) this[Fields.Competitoruniqueid2]; }
            set { ForceSetData(Fields.Competitoruniqueid2, value); }
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
        public DateTime Dateeventutc {
            get { return (DateTime) this[Fields.Dateeventutc]; }
            set { ForceSetData(Fields.Dateeventutc, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
