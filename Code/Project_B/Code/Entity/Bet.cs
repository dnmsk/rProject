using System;
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
        public short BrokerID {
            get { return (short) this[Fields.BrokerID]; }
            set { ForceSetData(Fields.BrokerID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Win1 {
            get { return (double?) this[Fields.Win1]; }
            set { ForceSetData(Fields.Win1, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Win2 {
            get { return (double?) this[Fields.Win2]; }
            set { ForceSetData(Fields.Win2, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Hcap1 {
            get { return (double?) this[Fields.Hcap1]; }
            set { ForceSetData(Fields.Hcap1, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Hcap2 {
            get { return (double?) this[Fields.Hcap2]; }
            set { ForceSetData(Fields.Hcap2, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Hcapdetail {
            get { return (double?) this[Fields.Hcapdetail]; }
            set { ForceSetData(Fields.Hcapdetail, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Totalunder {
            get { return (double?) this[Fields.Totalunder]; }
            set { ForceSetData(Fields.Totalunder, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Totalover {
            get { return (double?) this[Fields.Totalover]; }
            set { ForceSetData(Fields.Totalover, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Totaldetail {
            get { return (double?) this[Fields.Totaldetail]; }
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
            foreach (var odd in odds) {
                switch (odd.Type) {
                    case BetOddType.Win1:
                        newBet.Win1 = (double)odd.Factor;
                        break;
                    case BetOddType.Win2:
                        newBet.Win2 = (double)odd.Factor;
                        break;
                    case BetOddType.Handicap1:
                        newBet.Hcap1 = (double)odd.Factor;
                        newBet.Hcapdetail = (double)(odd.AdvancedParam ?? default(decimal));
                        break;
                    case BetOddType.Handicap2:
                        newBet.Hcap2 = (double)odd.Factor;
                        newBet.Hcapdetail = (double)(odd.AdvancedParam ?? default(decimal));
                        break;
                    case BetOddType.TotalUnder:
                        newBet.Totalunder = (double)odd.Factor;
                        newBet.Totaldetail = (double)(odd.AdvancedParam ?? default(decimal));
                        break;
                    case BetOddType.TotalOver:
                        newBet.Totalover = (double)odd.Factor;
                        newBet.Totaldetail = (double)(odd.AdvancedParam ?? default(decimal));
                        break;
                }
            }
            return newBet;
        }
    }
}
