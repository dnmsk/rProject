﻿using System;
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
    [DBTable("RawCompetitor")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class RawCompetitor : AbstractEntityTemplateKey<RawCompetitor, int>, ICompetitor, IBrokerTyped, ILinkStatusTyped, IRawLinkEntity {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
        [Nullable]
            [DBField(DbType.Int32)] CompetitoruniqueID,

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

        }

        /// <summary>
        /// 
        /// </summary>
        public RawCompetitor() {
        }

        /// <summary>
        /// 
        /// </summary>
        public RawCompetitor(Hashtable ht) : base(ht) {}
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
        public int CompetitoruniqueID {
            get { return (int) (this[Fields.CompetitoruniqueID] ?? default(int)); }
            set { ForceSetData(Fields.CompetitoruniqueID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name {
            get { return (string) this[Fields.Name]; }
            set { ForceSetData(Fields.Name, value); }
        }
        public Enum NameField => Fields.Name;

        /// <summary>
        /// 
        /// </summary>
        public LanguageType Languagetype {
            get { return (LanguageType)(short)this[Fields.Languagetype]; }
            set { ForceSetData(Fields.Languagetype, value); }
        }
        public Enum LanguageTypeField => Fields.Languagetype;

        /// <summary>
        /// 
        /// </summary>
        public SportType SportType {
            get { return (SportType)(short)this[Fields.Sporttype]; }
            set { ForceSetData(Fields.Sporttype, value); }
        }
        public Enum SportTypeField => Fields.Sporttype;

        /// <summary>
        /// 
        /// </summary>
        public GenderType Gendertype {
            get { return (GenderType)(short)this[Fields.Gendertype]; }
            set { ForceSetData(Fields.Gendertype, value); }
        }
        public Enum GenderTypeField => Fields.Gendertype;

        /// <summary>
        /// 
        /// </summary>
        public BrokerType BrokerID {
            get { return (BrokerType)(short)this[Fields.Brokerid]; }
            set { ForceSetData(Fields.Brokerid, value); }
        }
        public Enum BrokerField => Fields.Brokerid;

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

        public int LinkToEntityID => CompetitoruniqueID;
        public BrokerEntityType EntityType => BrokerEntityType.Competitor;
        public int UniqueID => CompetitoruniqueID;
    }
}
