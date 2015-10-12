﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.Code.Data;
using Project_B.Code.Enums;

namespace Project_B.Code.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("Bet")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Bet : AbstractEntityTemplateKey<Bet, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] CompetitionitemID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int16)] BrokerID,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Win1,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Win2,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Hcap1,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Hcap2,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Hcapdetail,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Totalunder,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Totalover,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Totaldetail,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreatedutc,

        }

        /// <summary>
        /// 
        /// </summary>
        public Bet() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Bet(Hashtable ht) : base(ht) {}
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
        public int CompetitionitemID {
            get { return (int) this[Fields.CompetitionitemID]; }
            set { ForceSetData(Fields.CompetitionitemID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BrokerType BrokerID {
            get { return (BrokerType) (short) this[Fields.BrokerID]; }
            set { ForceSetData(Fields.BrokerID, value); }
        }

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

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }

        public bool IsEqualsTo(Bet bet) {
            return Win1 == bet.Win1
                   && Win2 == bet.Win2
                   && Hcap1 == bet.Hcap1
                   && Hcap2 == bet.Hcap2
                   && Hcapdetail == bet.Hcapdetail
                   && Totalunder == bet.Totalunder
                   && Totalover == bet.Totalover
                   && Totaldetail == bet.Totaldetail;
        }

        public static Bet GetBetFromOdds(List<OddParsed> odds) {
            var newBet = new Bet();
            var hasAnyFactor = false;
            foreach (var odd in odds) {
                switch (odd.Type) {
                    case BetOddType.Win1:
                        newBet.Win1 = (float)odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Win2:
                        newBet.Win2 = (float)odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Handicap1:
                        newBet.Hcap1 = (float)odd.Factor;
                        newBet.Hcapdetail = (float)(odd.AdvancedParam ?? default(decimal));
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Handicap2:
                        newBet.Hcap2 = (float)odd.Factor;
                        newBet.Hcapdetail = (float)(odd.AdvancedParam ?? default(decimal));
                        hasAnyFactor = true;
                        break;
                    case BetOddType.TotalUnder:
                        newBet.Totalunder = (float)odd.Factor;
                        newBet.Totaldetail = (float)(odd.AdvancedParam ?? default(decimal));
                        hasAnyFactor = true;
                        break;
                    case BetOddType.TotalOver:
                        newBet.Totalover = (float)odd.Factor;
                        newBet.Totaldetail = (float)(odd.AdvancedParam ?? default(decimal));
                        hasAnyFactor = true;
                        break;
                }
            }
            return hasAnyFactor ? newBet : null;
        }
    }
}
