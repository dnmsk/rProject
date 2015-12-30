using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("RawCompetitionItem")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class RawCompetitionItem : AbstractEntityTemplateKey<RawCompetitionItem, int>, ICompetitionItem, ILinkStatusTyped, ILanguageTyped, IBrokerTyped {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] RawcompetitionID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] RawcompetitionspecifyID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Rawcompetitorid1,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Rawcompetitorid2,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Languagetype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Sporttype,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Brokerid,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Linkstatus,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreatedutc,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Dateeventutc,

        /// <summary>
        /// 
        /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] CompetitionitemID,

        }

        /// <summary>
        /// 
        /// </summary>
        public RawCompetitionItem() {
        }

        /// <summary>
        /// 
        /// </summary>
        public RawCompetitionItem(Hashtable ht) : base(ht) {}
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
        public int RawcompetitionID {
            get { return (int) this[Fields.RawcompetitionID]; }
            set { ForceSetData(Fields.RawcompetitionID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RawcompetitionspecifyID {
            get { return (int) this[Fields.RawcompetitionspecifyID]; }
            set { ForceSetData(Fields.RawcompetitionspecifyID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Rawcompetitorid1 {
            get { return (int) this[Fields.Rawcompetitorid1]; }
            set { ForceSetData(Fields.Rawcompetitorid1, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Rawcompetitorid2 {
            get { return (int) this[Fields.Rawcompetitorid2]; }
            set { ForceSetData(Fields.Rawcompetitorid2, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public LanguageType Languagetype {
            get { return (LanguageType)(short)this[Fields.Languagetype]; }
            set { ForceSetData(Fields.Languagetype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public SportType SportType {
            get { return (SportType)(short)this[Fields.Sporttype]; }
            set { ForceSetData(Fields.Sporttype, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BrokerType BrokerID {
            get { return (BrokerType)(short)this[Fields.Brokerid]; }
            set { ForceSetData(Fields.Brokerid, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public LinkEntityStatus Linkstatus {
            get { return (LinkEntityStatus) (short) this[Fields.Linkstatus]; }
            set { ForceSetData(Fields.Linkstatus, value); }
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

        /// <summary>
        /// 
        /// </summary>
        public int CompetitionitemID {
            get { return (int) (this[Fields.CompetitionitemID] ?? default(int)); }
            set { ForceSetData(Fields.CompetitionitemID, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }

        public int Competitoruniqueid1 {
            get { return Rawcompetitorid1; }
            set { Rawcompetitorid1 = value; }
        }

        public int Competitoruniqueid2 {
            get { return Rawcompetitorid2; }
            set { Rawcompetitorid2 = value; }
        }
        public int CompetitionuniqueID {
            get { return RawcompetitionID; }
            set { RawcompetitionID = value; }
        }
        public int CompetitionSpecifyUniqueID {
            get { return RawcompetitionspecifyID; }
            set { RawcompetitionspecifyID = value; }
        }
    }
}
