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
    [DBTable("RawCompetitionSpecify")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class RawCompetitionSpecify : AbstractEntityTemplateKey<RawCompetitionSpecify, int>, ICompetitionSpecify, IBrokerTyped, ILinkStatusTyped, IRawLinkEntity {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Int32)] CompetitionspecifyuniqueID,

        /// <summary>
        /// 
        /// </summary>
        [Nullable]
            [DBField(DbType.Int32)] CompetitionuniqueID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Name,

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
            [DBField(DbType.Int16)] Gendertype,

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
            [DBField(DbType.Int32)]
            RawCompetitionID,

        }

        /// <summary>
        /// 
        /// </summary>
        public RawCompetitionSpecify() {
        }

        /// <summary>
        /// 
        /// </summary>
        public RawCompetitionSpecify(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public int ID {
            get { return (int) this[Fields.ID]; }
            set { ForceSetData(Fields.ID, value); }
        }

        public int LinkToEntityID => CompetitionSpecifyUniqueID;
        public BrokerEntityType EntityType => BrokerEntityType.CompetitionSpecify;

        /// <summary>
        /// 
        /// </summary>
        public int CompetitionSpecifyUniqueID {
            get { return (int) (this[Fields.CompetitionspecifyuniqueID] ?? default(int)); }
            set { ForceSetData(Fields.CompetitionspecifyuniqueID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CompetitionuniqueID {
            get { return (int) (this[Fields.CompetitionuniqueID] ?? default(int)); }
            set { ForceSetData(Fields.CompetitionuniqueID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RawCompetitionID {
            get { return (int) this[Fields.RawCompetitionID]; }
            set { ForceSetData(Fields.RawCompetitionID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name {
            get { return (string) this[Fields.Name]; }
            set { ForceSetData(Fields.Name, value); }
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
        public GenderType Gendertype {
            get { return (GenderType)(short)this[Fields.Gendertype]; }
            set { ForceSetData(Fields.Gendertype, value); }
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

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }

        public int UniqueID => CompetitionSpecifyUniqueID;
    }
}
