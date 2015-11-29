﻿using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.View {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("VwBetRoiDetail")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class VwBetRoiDetail : AbstractEntityTemplateKey<VwBetRoiDetail, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Dateeventutc,

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
            [DBField(DbType.Single)] Roi1x2,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Roihcap,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Roitotal,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxwin1,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Maxwin1broker,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxwin2,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Maxwin2broker,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxdraw,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Maxdrawbroker,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxwin1draw,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Maxwin1drawbroker,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxwin1win2,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] Maxwin1win2broker,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxdrawwin2,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Maxdrawwin2broker,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxhcap1,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Maxhcap1broker,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxhcap1detail,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxhcap2,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Maxhcap2broker,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxhcap2detail,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxtotalunder,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Maxtotalunderbroker,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxtotalunderdetail,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxtotalover,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] Maxtotaloverbroker,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Maxtotaloverdetail,
        }

        /// <summary>
        /// 
        /// </summary>
        public VwBetRoiDetail() {}

        /// <summary>
        /// 
        /// </summary>
        public VwBetRoiDetail(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public int ID => (int) this[Fields.ID];

            /// <summary>
        /// 
        /// </summary>
        public DateTime Dateeventutc => (DateTime) this[Fields.Dateeventutc];

            /// <summary>
        /// 
        /// </summary>
        public int CompetitionuniqueID => (int) this[Fields.CompetitionuniqueID];

            /// <summary>
        /// 
        /// </summary>
        public short Sporttype => (short) this[Fields.Sporttype];

            /// <summary>
        /// 
        /// </summary>
        public int Competitoruniqueid1 => (int) this[Fields.Competitoruniqueid1];

            /// <summary>
        /// 
        /// </summary>
        public int Competitoruniqueid2 => (int) this[Fields.Competitoruniqueid2];

            /// <summary>
        /// 
        /// </summary>
        public float Roi1X2 => (float?) this[Fields.Roi1x2] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public float Roihcap => (float?) this[Fields.Roihcap] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public float Roitotal => (float?) this[Fields.Roitotal] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public float Maxwin1 => (float?) this[Fields.Maxwin1] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public BrokerType Maxwin1Broker => (BrokerType)((int?) this[Fields.Maxwin1broker] ?? default(int));

            /// <summary>
        /// 
        /// </summary>
        public float Maxwin2 => (float?) this[Fields.Maxwin2] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public BrokerType Maxwin2Broker => (BrokerType)((int?) this[Fields.Maxwin2broker] ?? default(int));

            /// <summary>
        /// 
        /// </summary>
        public float Maxdraw => (float?) this[Fields.Maxdraw] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public BrokerType Maxdrawbroker => (BrokerType)((int?) this[Fields.Maxdrawbroker] ?? default(int));

            /// <summary>
        /// 
        /// </summary>
        public float Maxwin1Draw => (float?) this[Fields.Maxwin1draw] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public BrokerType Maxwin1Drawbroker => (BrokerType)((int?) this[Fields.Maxwin1drawbroker] ?? default(int));

            /// <summary>
        /// 
        /// </summary>
        public float Maxwin1Win2 => (float?) this[Fields.Maxwin1win2] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public BrokerType Maxwin1Win2Broker => (BrokerType)((int?) this[Fields.Maxwin1win2broker] ?? default(int));

            /// <summary>
        /// 
        /// </summary>
        public float Maxdrawwin2 => (float?) this[Fields.Maxdrawwin2] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public BrokerType Maxdrawwin2Broker => (BrokerType)((int?) this[Fields.Maxdrawwin2broker] ?? default(int));

            /// <summary>
        /// 
        /// </summary>
        public float Maxhcap1 => (float?) this[Fields.Maxhcap1] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public BrokerType Maxhcap1Broker => (BrokerType)((int?) this[Fields.Maxhcap1broker] ?? default(int));

            /// <summary>
        /// 
        /// </summary>
        public float Maxhcap1Detail => (float?) this[Fields.Maxhcap1detail] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public float Maxhcap2 => (float?) this[Fields.Maxhcap2] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public BrokerType Maxhcap2Broker => (BrokerType)((int?) this[Fields.Maxhcap2broker] ?? default(int));

            /// <summary>
        /// 
        /// </summary>
        public float Maxhcap2Detail => (float?) this[Fields.Maxhcap2detail] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public float Maxtotalunder => (float?) this[Fields.Maxtotalunder] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public BrokerType Maxtotalunderbroker => (BrokerType)((int?) this[Fields.Maxtotalunderbroker] ?? default(int));

            /// <summary>
        /// 
        /// </summary>
        public float Maxtotalunderdetail => (float?) this[Fields.Maxtotalunderdetail] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public float Maxtotalover => (float?) this[Fields.Maxtotalover] ?? default(float);

            /// <summary>
        /// 
        /// </summary>
        public BrokerType Maxtotaloverbroker => (BrokerType) ((int?) this[Fields.Maxtotaloverbroker] ?? default(int));

            /// <summary>
        /// 
        /// </summary>
        public float Maxtotaloverdetail => (float?) this[Fields.Maxtotaloverdetail] ?? default(float);

        public override Enum[] KeyFields => new[] { (Enum) Fields.ID };
    }
}