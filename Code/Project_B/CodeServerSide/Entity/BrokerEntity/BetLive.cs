﻿using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Entity.Interface.NameConstraint;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.BrokerEntity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("BetLive")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class BetLive : AbstractEntityTemplateKey<BetLive, long>, IBet<long>, IBrokerTyped, IDateCreatedTyped {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] CompetitionitemID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] BrokerID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Win1,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Win2,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Hcap1,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Hcap2,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Hcapdetail,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Totalunder,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Totalover,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Totaldetail,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreatedutc,
            
            [DBField(DbType.Boolean)] IsActive,
        }

        /// <summary>
        /// 
        /// </summary>
        public BetLive() {
            IsActive = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public BetLive(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public long ID {
            get { return (long) this[Fields.ID]; }
            set { ForceSetData(Fields.ID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CompetitionitemID {
            get { return (int) this[Fields.CompetitionitemID]; }
            set { ForceSetData(Fields.CompetitionitemID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BrokerType BrokerID {
            get { return (BrokerType) (int) this[Fields.BrokerID]; }
            set { ForceSetData(Fields.BrokerID, (short) value); }
        }
        public Enum BrokerField => Fields.BrokerID;

        /// <summary>
        /// 
        /// </summary>
        public float? Win1 {
            get { return (float?) this[Fields.Win1]; }
            set { ForceSetData(Fields.Win1, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float? Win2 {
            get { return (float?) this[Fields.Win2]; }
            set { ForceSetData(Fields.Win2, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float? Hcap1 {
            get { return (float?) this[Fields.Hcap1]; }
            set { ForceSetData(Fields.Hcap1, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float? Hcap2 {
            get { return (float?) this[Fields.Hcap2]; }
            set { ForceSetData(Fields.Hcap2, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float? Hcapdetail {
            get { return (float?) this[Fields.Hcapdetail]; }
            set { ForceSetData(Fields.Hcapdetail, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float? Totalunder {
            get { return (float?) this[Fields.Totalunder]; }
            set { ForceSetData(Fields.Totalunder, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float? Totalover {
            get { return (float?) this[Fields.Totalover]; }
            set { ForceSetData(Fields.Totalover, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float? Totaldetail {
            get { return (float?) this[Fields.Totaldetail]; }
            set { ForceSetData(Fields.Totaldetail, value); }
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
        public bool IsActive {
            get { return (bool) this[Fields.IsActive]; }
            private set { ForceSetData(Fields.IsActive, value); }
        }

        public IBet<long> GetLastBetForCompetitionItem(int competitionItem, BrokerType broker) {
            return DataSource
                .Join(JoinType.Left, BetLiveAdvanced.Fields.ID, Fields.ID, RetrieveMode.Retrieve)
                .WhereEquals(Fields.BrokerID, (short) broker)
                .WhereEquals(Fields.CompetitionitemID, competitionItem)
                .Sort(Fields.ID, SortDirection.Desc)
                .Sort(BetLiveAdvanced.Fields.ID, SortDirection.Desc)
                .First();
        }

        public IBetAdvanced<long> GetAdvancedBet() {
            return GetJoinedEntity<IBetAdvanced<long>>();
        }

        public IBetAdvanced<long> CreateAdvancedBet() {
            return new BetLiveAdvanced();
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
