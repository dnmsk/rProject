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
    [DBTable("BetAdvanced")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class BetAdvanced : AbstractEntityTemplateKey<BetAdvanced, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] BetID,

        /// <summary>
        /// 
        /// </summary>
        [Nullable]
            [DBField(DbType.Single)] Win1draw,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Win1win2,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Drawwin2,

        }

        /// <summary>
        /// 
        /// </summary>
        public BetAdvanced() {
        }

        /// <summary>
        /// 
        /// </summary>
        public BetAdvanced(Hashtable ht) : base(ht) {}
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
        public int BetID {
            get { return (int) this[Fields.BetID]; }
            set { ForceSetData(Fields.BetID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Win1draw {
            get { return (double?) this[Fields.Win1draw]; }
            set { ForceSetData(Fields.Win1draw, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Win1win2 {
            get { return (double?) this[Fields.Win1win2]; }
            set { ForceSetData(Fields.Win1win2, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Drawwin2 {
            get { return (double?) this[Fields.Drawwin2]; }
            set { ForceSetData(Fields.Drawwin2, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }

        public bool IsEqualsTo(BetAdvanced betAdvanced) {
            return Win1draw == betAdvanced.Win1draw
                && Win1win2 == betAdvanced.Win1win2
                && Drawwin2 == betAdvanced.Drawwin2;
        }

        public static BetAdvanced GetBetFromOdds(List<OddParsed> odds) {
            var newBet = new BetAdvanced();
            var hasAnyFactor = false;
            foreach (var odd in odds) {
                switch (odd.Type) {
                    case BetOddType.Win1Win2:
                        newBet.Win1win2 = (double)odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.DrawWin2:
                        newBet.Drawwin2 = (double)odd.Factor;
                        hasAnyFactor = true;
                        break;
                    case BetOddType.Win1Draw:
                        newBet.Win1draw = (double)odd.Factor;
                        hasAnyFactor = true;
                        break;
                }
            }
            return hasAnyFactor ? newBet : null;
        }
    }
}
